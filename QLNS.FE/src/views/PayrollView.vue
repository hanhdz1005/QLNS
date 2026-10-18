<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useToastStore } from '@/stores/toast';
import { useI18n } from '@/locales';
import { salaryApi } from '@/api/salary.api';
import { exportApi } from '@/api/export.api';
import type { PayrollDto, PayrollGenerateDto } from '@/types';
import PayrollSummaryCards from './payroll/components/PayrollSummaryCards.vue';
import PayrollFilterBar from './payroll/components/PayrollFilterBar.vue';
import PayrollTable from './payroll/components/PayrollTable.vue';
import PayrollPayslipModal from './payroll/components/PayrollPayslipModal.vue';
import PayrollGenerateModal from './payroll/components/PayrollGenerateModal.vue';
import MyPayslipView from './payroll/components/MyPayslipView.vue';
import { DollarSign, FileText, User } from 'lucide-vue-next';

const authStore = useAuthStore();
const toastStore = useToastStore();
const { t } = useI18n();

const activeTab = ref<'company' | 'my-payslip'>('company');

// Company Payroll Table
const now = new Date();
const selectedMonth = ref(now.getMonth() + 1);
const selectedYear = ref(now.getFullYear());
const payrollList = ref<PayrollDto[]>([]);
const loading = ref(false);

const totalGrossSum = computed(() => payrollList.value.reduce((acc, p) => acc + (p.grossSalary || 0), 0));
const totalNetSum = computed(() => payrollList.value.reduce((acc, p) => acc + (p.netSalary || 0), 0));
const totalTaxSum = computed(() => payrollList.value.reduce((acc, p) => acc + (p.tax || 0), 0));
const totalInsuranceSum = computed(() => payrollList.value.reduce((acc, p) => acc + (p.insurance || 0), 0));

const fetchPayroll = async () => {
  loading.value = true;
  try {
    const res = await salaryApi.getAllPayroll(selectedMonth.value, selectedYear.value);
    if (res.isSuccess && res.data) {
      payrollList.value = res.data.items || [];
    }
  } catch {
    toastStore.error(t('e_load_failed'));
  } finally {
    loading.value = false;
  }
};

// Generate Bulk Modal
const showGenerateModal = ref(false);
const generateLoading = ref(false);
const generateForm = reactive<PayrollGenerateDto>({
  month: selectedMonth.value,
  year: selectedYear.value,
  calculateAttendance: true,
  standardWorkingDays: 22,
  includeInsurance: true,
  includeTax: true,
  saveToDatabase: true,
});

const openGenerateModal = () => {
  generateForm.month = selectedMonth.value;
  generateForm.year = selectedYear.value;
  showGenerateModal.value = true;
};

const handleGeneratePayroll = async () => {
  generateLoading.value = true;
  try {
    const res = await salaryApi.generateBulkPayroll(generateForm);
    if (res.isSuccess && res.data) {
      payrollList.value = res.data.items || [];
      toastStore.success(t('t_payroll_generated'));
      showGenerateModal.value = false;
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_save_failed');
    toastStore.error(msg);
  } finally {
    generateLoading.value = false;
  }
};

// Payslip Detail Modal
const showPayslipModal = ref(false);
const activePayslip = ref<PayrollDto | null>(null);

const viewPayslip = (p: PayrollDto) => {
  activePayslip.value = p;
  showPayslipModal.value = true;
};

// My Payslip (Employee self-service)
const myMonth = ref(selectedMonth.value);
const myYear = ref(selectedYear.value);
const myPayslip = ref<PayrollDto | null>(null);
const myLoading = ref(false);

const fetchMyPayslip = async () => {
  myLoading.value = true;
  try {
    const res = await salaryApi.getMyPayslip(myMonth.value, myYear.value);
    if (res.isSuccess && res.data) {
      myPayslip.value = res.data;
    } else {
      myPayslip.value = null;
    }
  } catch {
    myPayslip.value = null;
  } finally {
    myLoading.value = false;
  }
};

const exportSalaryCsv = async () => {
  try {
    const blob = await exportApi.exportSalaryCsv(selectedMonth.value, selectedYear.value);
    exportApi.downloadBlob(blob, `Bang_luong_thang_${selectedMonth.value}_${selectedYear.value}.csv`);
    toastStore.success(t('t_export_success'));
  } catch {
    toastStore.error(t('e_export_failed'));
  }
};

onMounted(() => {
  if (authStore.isAdmin || authStore.isLeader) {
    fetchPayroll();
  } else {
    activeTab.value = 'my-payslip';
    fetchMyPayslip();
  }
});
</script>

<template>
  <div class="space-y-4 sm:space-y-5">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3 shrink-0">
      <div>
        <h1 class="text-lg sm:text-xl font-bold text-slate-800 tracking-tight flex items-center gap-2.5">
          <DollarSign class="w-5 h-5 text-indigo-600" />
          {{ t('n_payroll') }}
        </h1>
        <p class="text-xs text-slate-500 hidden sm:block mt-0.5">
          {{ t('l_payroll_subtitle') }}
        </p>
      </div>

      <!-- Navigation Tabs -->
      <div class="flex items-center p-1 bg-slate-200/80 rounded-xl text-xs sm:text-sm font-semibold">
        <button
          v-if="authStore.isAdmin || authStore.isLeader"
          @click="activeTab = 'company'; fetchPayroll()"
          :class="[
            'flex items-center gap-2 px-3.5 py-1.5 rounded-lg transition-all',
            activeTab === 'company' ? 'bg-white text-indigo-700 shadow-xs' : 'text-slate-600 hover:text-slate-900',
          ]"
        >
          <FileText class="w-4 h-4" />
          {{ t('l_tab_company_payroll') }}
        </button>

        <button
          @click="activeTab = 'my-payslip'; fetchMyPayslip()"
          :class="[
            'flex items-center gap-2 px-3.5 py-1.5 rounded-lg transition-all',
            activeTab === 'my-payslip' ? 'bg-white text-indigo-700 shadow-xs' : 'text-slate-600 hover:text-slate-900',
          ]"
        >
          <User class="w-4 h-4" />
          {{ t('l_tab_my_payslip') }}
        </button>
      </div>
    </div>

    <!-- Company Tab -->
    <div v-if="activeTab === 'company'" class="space-y-4 sm:space-y-5">
      <!-- 4 KPI Cards -->
      <PayrollSummaryCards
        :total-gross-sum="totalGrossSum"
        :total-net-sum="totalNetSum"
        :total-tax-sum="totalTaxSum"
        :total-insurance-sum="totalInsuranceSum"
      />

      <!-- Toolbar -->
      <PayrollFilterBar
        v-model:month="selectedMonth"
        v-model:year="selectedYear"
        :is-admin="authStore.isAdmin"
        @open-generate="openGenerateModal"
        @export-csv="exportSalaryCsv"
        @reload="fetchPayroll"
      />

      <!-- Payroll Table -->
      <div class="bg-white rounded-2xl border border-slate-200/80 shadow-xs overflow-hidden">
        <PayrollTable
          :payroll-list="payrollList"
          :loading="loading"
          @view-payslip="viewPayslip"
        />
      </div>
    </div>

    <!-- My Payslip Tab -->
    <div v-else-if="activeTab === 'my-payslip'" class="py-1">
      <MyPayslipView
        :my-payslip="myPayslip"
        :loading="myLoading"
        v-model:month="myMonth"
        v-model:year="myYear"
        @reload="fetchMyPayslip"
      />
    </div>

    <!-- Modals -->
    <PayrollGenerateModal
      v-model="showGenerateModal"
      :generate-form="generateForm"
      :loading="generateLoading"
      @submit="handleGeneratePayroll"
    />

    <PayrollPayslipModal
      v-model="showPayslipModal"
      :payslip="activePayslip"
    />
  </div>
</template>
