<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useToastStore } from '@/stores/toast';
import { useI18n } from '@/locales';
import { reportApi } from '@/api/report.api';
import { exportApi } from '@/api/export.api';
import type { DashboardDto, EmployeeStatsDto, HrAlertsDto } from '@/types';
import { formatCurrency } from '@/utils/formatters';
import DashboardWelcomeBanner from './dashboard/components/DashboardWelcomeBanner.vue';
import DashboardKpiCards from './dashboard/components/DashboardKpiCards.vue';
import DashboardDepartmentStats from './dashboard/components/DashboardDepartmentStats.vue';
import DashboardHrAlerts from './dashboard/components/DashboardHrAlerts.vue';

const authStore = useAuthStore();
const toastStore = useToastStore();
const { t } = useI18n();

const loading = ref(true);
const dashboardData = ref<DashboardDto>({
  totalEmployees: 0,
  activeEmployees: 0,
  totalDepartments: 0,
  pendingLeaveRequests: 0,
  totalAttendancesThisMonth: 0,
  totalSalaryLastMonth: 0,
  averageSalary: 0,
});

const employeeStats = ref<EmployeeStatsDto[]>([]);
const hrAlerts = ref<HrAlertsDto>({
  expiringContracts: [],
  upcomingBirthdays: [],
  probationEndingEmployees: [],
});

const loadDashboard = async () => {
  loading.value = true;
  try {
    const [dashRes, statsRes, alertsRes] = await Promise.allSettled([
      reportApi.getDashboard(),
      reportApi.getEmployeeStats(),
      authStore.canManagePayroll ? reportApi.getHrAlerts() : Promise.resolve(null),
    ]);

    if (dashRes.status === 'fulfilled' && dashRes.value.isSuccess) {
      dashboardData.value = dashRes.value.data;
    }
    if (statsRes.status === 'fulfilled' && statsRes.value.isSuccess) {
      employeeStats.value = statsRes.value.data.items || [];
    }
    if (alertsRes.status === 'fulfilled' && alertsRes.value && alertsRes.value.isSuccess) {
      hrAlerts.value = alertsRes.value.data;
    }
  } catch (error) {
    console.error('Error fetching dashboard data:', error);
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  loadDashboard();
});

const exportLoading = ref(false);

const handleExportEmployees = async () => {
  exportLoading.value = true;
  try {
    const blob = await exportApi.exportEmployeeCsv();
    exportApi.downloadBlob(blob, `Danh_sach_nhan_su_${new Date().toISOString().slice(0, 10)}.csv`);
    toastStore.success(t('t_export_success'));
  } catch {
    toastStore.error(t('e_export_failed'));
  } finally {
    exportLoading.value = false;
  }
};
</script>

<template>
  <div class="space-y-5 lg:space-y-6">
    <!-- Welcome Banner with Quick Actions -->
    <DashboardWelcomeBanner
      :export-loading="exportLoading"
      @export-employees="handleExportEmployees"
    />

    <!-- KPI Overview Cards -->
    <DashboardKpiCards :data="dashboardData" />

    <!-- Two-column Layout: Analytics & Alerts -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-5 lg:gap-6">
      <div :class="[authStore.canManagePayroll ? 'lg:col-span-2' : 'lg:col-span-3', 'space-y-5 lg:space-y-6']">
        <!-- Payroll Quick Overview (Admin & Accountant only) -->
        <div v-if="authStore.canManagePayroll" class="bg-white p-5 rounded-2xl border border-slate-200/80 shadow-xs">
          <div class="flex items-center justify-between mb-4">
            <div>
              <h2 class="text-sm sm:text-base font-bold text-slate-800">{{ t('l_payroll_and_income_average') }}</h2>
              <p class="text-xs text-slate-500 mt-0.5">{{ t('l_payroll_recent_data_desc') }}</p>
            </div>
            <router-link to="/payroll" class="text-xs font-semibold text-indigo-600 hover:text-indigo-800">
              {{ t('l_view_payroll_details') }} &rarr;
            </router-link>
          </div>

          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div class="p-4 rounded-xl bg-slate-50/80 border border-slate-100">
              <span class="text-xs text-slate-500 font-medium">{{ t('l_total_net_salary_fund') }}</span>
              <div class="text-xl sm:text-2xl font-bold text-indigo-700 font-mono mt-1">
                {{ formatCurrency(dashboardData.totalSalaryLastMonth) }}
              </div>
              <div class="text-xs text-slate-400 mt-1">{{ t('l_payout_last_period_desc') }}</div>
            </div>

            <div class="p-4 rounded-xl bg-slate-50/80 border border-slate-100">
              <span class="text-xs text-slate-500 font-medium">{{ t('l_average_salary') }}</span>
              <div class="text-xl sm:text-2xl font-bold text-slate-800 font-mono mt-1">
                {{ formatCurrency(dashboardData.averageSalary) }}
              </div>
              <div class="text-xs text-slate-400 mt-1">{{ t('l_average_salary_desc') }}</div>
            </div>
          </div>
        </div>

        <!-- Department Staff Distribution -->
        <DashboardDepartmentStats
          :stats="employeeStats"
          :total-employees="dashboardData.totalEmployees"
        />
      </div>

      <!-- Right Column: HR Alerts & Reminders (Admin & Accountant only) -->
      <div v-if="authStore.canManagePayroll">
        <DashboardHrAlerts :alerts="hrAlerts" />
      </div>
    </div>
  </div>
</template>
