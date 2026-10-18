<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useToastStore } from '@/stores/toast';
import { useI18n } from '@/locales';
import { reportApi } from '@/api/report.api';
import { exportApi } from '@/api/export.api';
import type { SalaryReportDto, LeaveReportDto } from '@/types';
import { formatCurrency } from '@/utils/formatters';
import {
  BarChart3,
  Download,
  FileSpreadsheet,
  DollarSign,
  CalendarOff,
} from 'lucide-vue-next';

const authStore = useAuthStore();
const toastStore = useToastStore();
const { t } = useI18n();

// Salary Report State
const now = new Date();
const salaryMonth = ref(now.getMonth() + 1);
const salaryYear = ref(now.getFullYear());
const salaryLoading = ref(false);
const salaryReport = ref<SalaryReportDto | null>(null);

const fetchSalaryReport = async () => {
  salaryLoading.value = true;
  try {
    const res = await reportApi.getSalaryReport(salaryMonth.value, salaryYear.value);
    if (res.isSuccess && res.data) {
      salaryReport.value = res.data;
    }
  } catch {
    toastStore.error(t('e_load_salary_report_failed'));
  } finally {
    salaryLoading.value = false;
  }
};

const handleExportSalaryCsv = async () => {
  try {
    const blob = await exportApi.exportSalaryCsv(salaryMonth.value, salaryYear.value);
    exportApi.downloadBlob(blob, `Bao_cao_luong_thang_${salaryMonth.value}_${salaryYear.value}.csv`);
    toastStore.success(t('t_export_salary_csv_success'));
  } catch {
    toastStore.error(t('e_export_failed'));
  }
};

// Leave Report State
const defaultFrom = new Date(now.getFullYear(), now.getMonth(), 1).toISOString().slice(0, 10);
const defaultTo = new Date(now.getFullYear(), now.getMonth() + 1, 0).toISOString().slice(0, 10);
const leaveFrom = ref(defaultFrom);
const leaveTo = ref(defaultTo);
const leaveLoading = ref(false);
const leaveReport = ref<LeaveReportDto | null>(null);

const fetchLeaveReport = async () => {
  leaveLoading.value = true;
  try {
    const res = await reportApi.getLeaveReport(leaveFrom.value, leaveTo.value);
    if (res.isSuccess && res.data) {
      leaveReport.value = res.data;
    }
  } catch {
    toastStore.error(t('e_load_leave_report_failed'));
  } finally {
    leaveLoading.value = false;
  }
};

const handleExportLeaveCsv = async () => {
  try {
    const blob = await exportApi.exportLeaveCsv(leaveFrom.value, leaveTo.value);
    exportApi.downloadBlob(blob, `Bao_cao_nghi_phep_${leaveFrom.value}_${leaveTo.value}.csv`);
    toastStore.success(t('t_export_leave_csv_success'));
  } catch {
    toastStore.error(t('e_export_failed'));
  }
};

// Employee CSV
const handleExportEmployees = async () => {
  try {
    const blob = await exportApi.exportEmployeeCsv();
    exportApi.downloadBlob(blob, `Danh_sach_nhan_su_${new Date().toISOString().slice(0, 10)}.csv`);
    toastStore.success(t('t_export_employees_csv_success'));
  } catch {
    toastStore.error(t('e_export_failed'));
  }
};

const activeTab = ref<'salary' | 'leave'>('salary');

onMounted(() => {
  fetchSalaryReport();
  fetchLeaveReport();
});
</script>

<template>
  <div class="space-y-4 sm:space-y-5">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3">
      <div>
        <h1 class="text-xl sm:text-2xl font-bold text-slate-800 tracking-tight flex items-center gap-2">
          <BarChart3 class="w-6 h-6 text-indigo-600" />
          {{ t('l_report_title') }}
        </h1>
        <p class="text-xs sm:text-sm text-slate-500 mt-1">
          {{ t('l_report_subtitle') }}
        </p>
      </div>

      <div class="flex flex-wrap items-center gap-2.5">
        <!-- Navigation Tabs -->
        <div class="flex items-center p-1 bg-slate-100 border border-slate-200/80 rounded-2xl text-xs sm:text-sm font-semibold">
          <button
            @click="activeTab = 'salary'"
            :class="[
              'flex items-center gap-1.5 px-3.5 py-2 rounded-xl transition-all',
              activeTab === 'salary' ? 'bg-white text-emerald-700 shadow-xs' : 'text-slate-600 hover:text-slate-900',
            ]"
          >
            <DollarSign class="w-4 h-4" />
            {{ t('l_salary_report_section_title') }}
          </button>

          <button
            @click="activeTab = 'leave'"
            :class="[
              'flex items-center gap-1.5 px-3.5 py-2 rounded-xl transition-all',
              activeTab === 'leave' ? 'bg-white text-amber-700 shadow-xs' : 'text-slate-600 hover:text-slate-900',
            ]"
          >
            <CalendarOff class="w-4 h-4" />
            {{ t('l_leave_report_section_title') }}
          </button>
        </div>

        <button
          @click="handleExportEmployees"
          class="inline-flex items-center gap-2 px-4 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-xs sm:text-sm font-semibold shadow-xs shadow-indigo-600/20 transition-colors"
        >
          <FileSpreadsheet class="w-4 h-4" />
          {{ t('b_export_all_staff_csv') }}
        </button>
      </div>
    </div>

    <!-- Section 1: Salary Report Tab -->
    <div v-if="activeTab === 'salary'" class="bg-white rounded-2xl p-5 sm:p-6 border border-slate-200/80 shadow-xs space-y-5">
      <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3 border-b border-slate-100 pb-4">
        <div>
          <h2 class="text-base font-bold text-slate-800 flex items-center gap-2">
            <DollarSign class="w-4 h-4 text-emerald-600" />
            {{ t('l_salary_report_section_title') }}
          </h2>
          <p class="text-xs text-slate-500 mt-0.5">{{ t('l_salary_report_section_subtitle') }}</p>
        </div>

        <div class="flex flex-wrap items-center gap-2 text-xs sm:text-sm">
          <select v-model.number="salaryMonth" class="py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl font-semibold">
            <option v-for="m in 12" :key="m" :value="m">{{ t('l_month_prefix') }} {{ m }}</option>
          </select>
          <select v-model.number="salaryYear" class="py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl font-semibold">
            <option v-for="y in [2024, 2025, 2026, 2027]" :key="y" :value="y">{{ t('l_year_prefix') }} {{ y }}</option>
          </select>
          <button
            @click="fetchSalaryReport"
            class="px-3.5 py-2 bg-slate-100 hover:bg-slate-200 text-slate-700 font-semibold rounded-xl transition-colors"
          >
            {{ t('b_filter') }}
          </button>
          <button
            @click="handleExportSalaryCsv"
            class="px-3.5 py-2 bg-emerald-50 hover:bg-emerald-100 text-emerald-700 border border-emerald-200 font-semibold rounded-xl flex items-center gap-1.5 transition-colors"
          >
            <Download class="w-3.5 h-3.5" />
            {{ t('b_export_csv') }}
          </button>
        </div>
      </div>

      <div v-if="salaryLoading" class="text-center py-12 text-slate-400 text-xs sm:text-sm flex items-center justify-center">
        {{ t('l_loading_salary_report') }}
      </div>

      <div v-else-if="salaryReport" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3.5 sm:gap-4">
        <div class="p-4 sm:p-5 rounded-2xl bg-slate-50/80 border border-slate-100 flex flex-col justify-center">
          <span class="text-xs text-slate-500 font-medium">{{ t('l_total_paid_employees') }}</span>
          <div class="text-xl sm:text-2xl font-bold text-slate-800 mt-1">{{ salaryReport.totalRecords }} {{ t('l_people_count_unit') }}</div>
        </div>

        <div class="p-4 sm:p-5 rounded-2xl bg-slate-50/80 border border-slate-100 flex flex-col justify-center">
          <span class="text-xs text-slate-500 font-medium">{{ t('l_total_base_salary_fund') }}</span>
          <div class="text-xl sm:text-2xl font-bold text-indigo-700 font-mono mt-1">{{ formatCurrency(salaryReport.totalBasicSalary) }}</div>
        </div>

        <div class="p-4 sm:p-5 rounded-2xl bg-emerald-50/60 border border-emerald-100 flex flex-col justify-center">
          <span class="text-xs text-emerald-800 font-medium">{{ t('l_net_payout') }}</span>
          <div class="text-xl sm:text-2xl font-bold text-emerald-600 font-mono mt-1">{{ formatCurrency(salaryReport.totalNetSalary) }}</div>
        </div>

        <div class="p-4 sm:p-5 rounded-2xl bg-slate-50/80 border border-slate-100 flex flex-col justify-center">
          <span class="text-xs text-slate-500 font-medium">{{ t('l_average_net_salary') }}</span>
          <div class="text-lg sm:text-xl font-bold text-slate-700 font-mono mt-1">{{ formatCurrency(salaryReport.averageNetSalary) }}</div>
        </div>

        <div class="p-4 sm:p-5 rounded-2xl bg-slate-50/80 border border-slate-100 flex flex-col justify-center">
          <span class="text-xs text-slate-500 font-medium">{{ t('l_max_net_salary') }}</span>
          <div class="text-lg sm:text-xl font-bold text-slate-700 font-mono mt-1">{{ formatCurrency(salaryReport.maxSalary) }}</div>
        </div>

        <div class="p-4 sm:p-5 rounded-2xl bg-slate-50/80 border border-slate-100 flex flex-col justify-center">
          <span class="text-xs text-slate-500 font-medium">{{ t('l_min_net_salary') }}</span>
          <div class="text-lg sm:text-xl font-bold text-slate-700 font-mono mt-1">{{ formatCurrency(salaryReport.minSalary) }}</div>
        </div>
      </div>
    </div>

    <!-- Section 2: Leave Report Tab -->
    <div v-else-if="activeTab === 'leave'" class="bg-white rounded-2xl p-5 sm:p-6 border border-slate-200/80 shadow-xs space-y-5">
      <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3 border-b border-slate-100 pb-4">
        <div>
          <h2 class="text-base font-bold text-slate-800 flex items-center gap-2">
            <CalendarOff class="w-4 h-4 text-amber-500" />
            {{ t('l_leave_report_section_title') }}
          </h2>
          <p class="text-xs text-slate-500 mt-0.5">{{ t('l_leave_report_section_subtitle') }}</p>
        </div>

        <div class="flex flex-wrap items-center gap-2 text-xs sm:text-sm">
          <input v-model="leaveFrom" type="date" class="py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm font-semibold" />
          <span class="text-slate-400">&rarr;</span>
          <input v-model="leaveTo" type="date" class="py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm font-semibold" />
          <button
            @click="fetchLeaveReport"
            class="px-3.5 py-2 bg-slate-100 hover:bg-slate-200 text-slate-700 font-semibold rounded-xl transition-colors"
          >
            {{ t('b_filter') }}
          </button>
          <button
            @click="handleExportLeaveCsv"
            class="px-3.5 py-2 bg-amber-50 hover:bg-amber-100 text-amber-700 border border-amber-200 font-semibold rounded-xl flex items-center gap-1.5 transition-colors"
          >
            <Download class="w-3.5 h-3.5" />
            {{ t('b_export_csv') }}
          </button>
        </div>
      </div>

      <div v-if="leaveLoading" class="text-center py-12 text-slate-400 text-xs sm:text-sm flex items-center justify-center">
        {{ t('l_loading_leave_report') }}
      </div>

      <div v-else-if="leaveReport" class="space-y-4">
        <div class="grid grid-cols-2 sm:grid-cols-5 gap-3 sm:gap-4">
          <div class="p-4 rounded-2xl bg-slate-50/80 border border-slate-100 text-center">
            <span class="text-xs text-slate-500 font-medium">{{ t('l_total_requests') }}</span>
            <div class="text-xl sm:text-2xl font-bold text-slate-800 mt-1">{{ leaveReport.totalRequests }}</div>
          </div>

          <div class="p-4 rounded-2xl bg-amber-50/60 border border-amber-100 text-center">
            <span class="text-xs text-amber-700 font-medium">{{ t('s_pending') }}</span>
            <div class="text-xl sm:text-2xl font-bold text-amber-600 mt-1">{{ leaveReport.pending }}</div>
          </div>

          <div class="p-4 rounded-2xl bg-emerald-50/60 border border-emerald-100 text-center">
            <span class="text-xs text-emerald-700 font-medium">{{ t('s_approved') }}</span>
            <div class="text-xl sm:text-2xl font-bold text-emerald-600 mt-1">{{ leaveReport.approved }}</div>
          </div>

          <div class="p-4 rounded-2xl bg-rose-50/60 border border-rose-100 text-center">
            <span class="text-xs text-rose-700 font-medium">{{ t('s_rejected') }}</span>
            <div class="text-xl sm:text-2xl font-bold text-rose-600 mt-1">{{ leaveReport.rejected }}</div>
          </div>

          <div class="p-4 rounded-2xl bg-slate-50/80 border border-slate-100 text-center">
            <span class="text-xs text-slate-500 font-medium">{{ t('s_cancelled') }}</span>
            <div class="text-xl sm:text-2xl font-bold text-slate-500 mt-1">{{ leaveReport.cancelled }}</div>
          </div>
        </div>

        <!-- Visual breakdown bar -->
        <div v-if="leaveReport.totalRequests > 0" class="p-4 sm:p-5 rounded-2xl bg-slate-50/80 border border-slate-100 space-y-2">
          <div class="text-xs sm:text-sm font-semibold text-slate-700">{{ t('l_status_breakdown_ratio') }}</div>
          <div class="w-full bg-slate-200 h-3.5 rounded-full overflow-hidden flex">
            <div
              class="bg-emerald-500 h-full transition-all"
              :style="{ width: `${(leaveReport.approved / leaveReport.totalRequests) * 100}%` }"
              :title="`${t('s_approved')}: ${leaveReport.approved}`"
            />
            <div
              class="bg-amber-400 h-full transition-all"
              :style="{ width: `${(leaveReport.pending / leaveReport.totalRequests) * 100}%` }"
              :title="`${t('s_pending')}: ${leaveReport.pending}`"
            />
            <div
              class="bg-rose-500 h-full transition-all"
              :style="{ width: `${(leaveReport.rejected / leaveReport.totalRequests) * 100}%` }"
              :title="`${t('s_rejected')}: ${leaveReport.rejected}`"
            />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
