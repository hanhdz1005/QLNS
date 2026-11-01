<script setup lang="ts">
import { ref, reactive, onMounted, watch } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useToastStore } from '@/stores/toast';
import { useI18n } from '@/locales';
import { leaveApi } from '@/api/leave.api';
import { employeeApi } from '@/api/employee.api';
import type { LeaveRequestDto, AddLeaveRequestDto, LeaveBalanceDto, EmployeeDto, UpdateLeaveBalanceDto } from '@/types';
import { LeaveType } from '@/types';
import { usePagination } from '@/composables/usePagination';
import LeaveBalanceCards from './leave/components/LeaveBalanceCards.vue';
import LeaveRequestsTable from './leave/components/LeaveRequestsTable.vue';
import LeaveApprovalsTable from './leave/components/LeaveApprovalsTable.vue';
import LeaveRequestModal from './leave/components/LeaveRequestModal.vue';
import LeaveConfigBalanceModal from './leave/components/LeaveConfigBalanceModal.vue';
import BasePagination from '@/components/BasePagination.vue';
import { CalendarOff, CheckCircle2, Clock, Plus, Search, Sliders } from 'lucide-vue-next';

const authStore = useAuthStore();
const toastStore = useToastStore();
const { t } = useI18n();

const activeTab = ref<'my' | 'approvals' | 'all'>('my');

// Personal leave balance
const myBalance = ref<LeaveBalanceDto>({
  id: '',
  employeeId: '',
  employeeName: '',
  year: new Date().getFullYear(),
  totalLeaveDays: 12,
  usedLeaveDays: 0,
  remainingLeaveDays: 12,
});

const loadMyBalance = async () => {
  try {
    const res = await leaveApi.getMyBalance();
    if (res.isSuccess && res.data) {
      myBalance.value = res.data;
    }
  } catch {
    // keep default
  }
};

// All Requests / My Requests with pagination
const { pageIndex, pageSize, totalCount, totalPages, setPaginationData, onPageChange, resetPage } = usePagination(10);
const loading = ref(false);
const requests = ref<LeaveRequestDto[]>([]);
const search = ref('');

const fetchRequests = async () => {
  loading.value = true;
  try {
    const myOnly = activeTab.value === 'my';
    const res = await leaveApi.getFilterRequests(pageIndex.value, pageSize.value, search.value.trim(), myOnly);
    if (res.isSuccess && res.data) {
      requests.value = res.data.items || [];
      setPaginationData(res.data.totalCount, res.data.totalPages);
    }
  } catch {
    toastStore.error(t('e_load_failed'));
  } finally {
    loading.value = false;
  }
};

// Pending Approvals
const pendingApprovals = ref<LeaveRequestDto[]>([]);
const pendingLoading = ref(false);

const fetchPendingApprovals = async () => {
  if (!authStore.isAdmin) return;
  pendingLoading.value = true;
  try {
    const res = await leaveApi.getPendingApprovals(1, 20);
    if (res.isSuccess && res.data) {
      pendingApprovals.value = res.data.items || [];
    }
  } catch {
    toastStore.error(t('e_load_failed'));
  } finally {
    pendingLoading.value = false;
  }
};

const handleApprove = async (id: string) => {
  try {
    const res = await leaveApi.approveRequest(id);
    if (res.isSuccess) {
      toastStore.success(t('t_leave_approved'));
      fetchPendingApprovals();
      if (activeTab.value === 'all') fetchRequests();
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_save_failed');
    toastStore.error(msg);
  }
};

const handleReject = async (id: string) => {
  try {
    const res = await leaveApi.rejectRequest(id);
    if (res.isSuccess) {
      toastStore.info(t('t_leave_rejected'));
      fetchPendingApprovals();
      if (activeTab.value === 'all') fetchRequests();
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_save_failed');
    toastStore.error(msg);
  }
};

// Create Request Modal
const showCreateModal = ref(false);
const createLoading = ref(false);
const employees = ref<EmployeeDto[]>([]);

const requestForm = reactive<AddLeaveRequestDto>({
  employeeId: '',
  fromDate: new Date().toISOString().slice(0, 10),
  toDate: new Date().toISOString().slice(0, 10),
  reason: '',
  leaveType: LeaveType.AnnualLeave,
});

const loadEmployees = async () => {
  try {
    if (authStore.isAdmin) {
      const res = await employeeApi.getAll();
      if (res.isSuccess && res.data?.items) {
        employees.value = res.data.items;
      }
    } else {
      const res = await employeeApi.getProfileMe();
      if (res.isSuccess && res.data) {
        employees.value = [res.data];
      }
    }
    if (employees.value.length > 0 && employees.value[0] && !requestForm.employeeId) {
      requestForm.employeeId = employees.value[0].id;
    }
  } catch {
    // ignore
  }
};

const openCreateModal = async () => {
  if (employees.value.length === 0) await loadEmployees();
  requestForm.fromDate = new Date().toISOString().slice(0, 10);
  requestForm.toDate = new Date().toISOString().slice(0, 10);
  requestForm.reason = '';
  requestForm.leaveType = LeaveType.AnnualLeave;
  showCreateModal.value = true;
};

const handleCreateRequest = async () => {
  if (!requestForm.employeeId || !requestForm.fromDate || !requestForm.toDate || !requestForm.reason) {
    toastStore.warning(t('e_required_fields'));
    return;
  }

  createLoading.value = true;
  try {
    const res = await leaveApi.createRequest(requestForm);
    if (res.isSuccess) {
      toastStore.success(t('t_leave_created'));
      showCreateModal.value = false;
      fetchRequests();
      loadMyBalance();
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_save_failed');
    toastStore.error(msg);
  } finally {
    createLoading.value = false;
  }
};

// Quota adjustment modal (Admin only)
const showQuotaModal = ref(false);
const quotaLoading = ref(false);
const quotaForm = reactive<UpdateLeaveBalanceDto>({
  employeeId: '',
  year: new Date().getFullYear(),
  totalLeaveDays: 12,
});

const openQuotaModal = async () => {
  if (employees.value.length === 0) await loadEmployees();
  quotaForm.employeeId = employees.value[0]?.id || '';
  quotaForm.year = new Date().getFullYear();
  quotaForm.totalLeaveDays = 12;
  showQuotaModal.value = true;
};

const handleUpdateQuota = async () => {
  quotaLoading.value = true;
  try {
    const res = await leaveApi.updateTotalDays(quotaForm);
    if (res.isSuccess) {
      toastStore.success(t('t_quota_updated'));
      showQuotaModal.value = false;
      loadMyBalance();
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_save_failed');
    toastStore.error(msg);
  } finally {
    quotaLoading.value = false;
  }
};

let searchTimeout: ReturnType<typeof setTimeout> | null = null;
watch(search, () => {
  if (searchTimeout) clearTimeout(searchTimeout);
  searchTimeout = setTimeout(() => {
    resetPage();
    fetchRequests();
  }, 400);
});

watch(activeTab, (tab) => {
  if (tab === 'approvals') fetchPendingApprovals();
  if (tab === 'all' || tab === 'my') fetchRequests();
});

onMounted(() => {
  loadMyBalance();
  fetchRequests();
  if (authStore.isAdmin) {
    fetchPendingApprovals();
  }
});
</script>

<template>
  <div class="space-y-4 sm:space-y-5">
    <!-- Header Page Actions -->
    <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3 shrink-0">
      <div>
        <h1 class="text-lg sm:text-xl font-bold text-slate-800 tracking-tight flex items-center gap-2.5">
          <CalendarOff class="w-5 h-5 text-indigo-600" />
          {{ t('n_leaves') }}
        </h1>
        <p class="text-xs text-slate-500 hidden sm:block mt-0.5">
          {{ t('l_leaves_subtitle') }}
        </p>
      </div>

      <div class="flex items-center gap-2.5">
        <button
          v-if="authStore.isAdmin"
          @click="openQuotaModal"
          class="inline-flex items-center gap-2 px-3.5 py-2 bg-white hover:bg-slate-50 text-slate-700 border border-slate-200 rounded-xl text-xs sm:text-sm font-semibold shadow-2xs transition-colors"
        >
          <Sliders class="w-4 h-4 text-slate-500" />
          {{ t('b_adjust_quota') }}
        </button>

        <button
          @click="openCreateModal"
          class="inline-flex items-center gap-2 px-3.5 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-xs sm:text-sm font-semibold shadow-md shadow-indigo-600/20 transition-colors"
        >
          <Plus class="w-4 h-4" />
          {{ t('b_create_leave_request') }}
        </button>
      </div>
    </div>

    <!-- Personal Leave Balance KPI Cards -->
    <LeaveBalanceCards :balance="myBalance" />

    <!-- Consolidated Tabs & Search Toolbar -->
    <div class="bg-white p-3 sm:p-4 rounded-2xl border border-slate-200/80 shadow-xs flex flex-col sm:flex-row items-center justify-between gap-3 shrink-0">
      <div class="flex items-center p-1 bg-slate-200/80 rounded-xl text-xs sm:text-sm font-semibold w-full sm:w-auto">
        <button
          @click="activeTab = 'my'"
          :class="[
            'flex items-center gap-2 px-3.5 py-1.5 rounded-lg transition-all',
            activeTab === 'my' ? 'bg-white text-indigo-700 shadow-xs' : 'text-slate-600 hover:text-slate-900',
          ]"
        >
          <Clock class="w-4 h-4" />
          {{ t('l_tab_my_leaves') }}
        </button>

        <button
          v-if="authStore.isAdmin"
          @click="activeTab = 'approvals'"
          :class="[
            'flex items-center gap-2 px-3.5 py-1.5 rounded-lg transition-all',
            activeTab === 'approvals' ? 'bg-white text-indigo-700 shadow-xs' : 'text-slate-600 hover:text-slate-900',
          ]"
        >
          <CheckCircle2 class="w-4 h-4" />
          {{ t('l_tab_pending_approvals') }} ({{ pendingApprovals.length }})
        </button>

        <button
          v-if="authStore.canManagePayroll"
          @click="activeTab = 'all'"
          :class="[
            'flex items-center gap-2 px-3.5 py-1.5 rounded-lg transition-all',
            activeTab === 'all' ? 'bg-white text-indigo-700 shadow-xs' : 'text-slate-600 hover:text-slate-900',
          ]"
        >
          <CalendarOff class="w-4 h-4" />
          {{ t('l_tab_all_leaves') }}
        </button>
      </div>

      <div v-if="activeTab === 'my' || activeTab === 'all'" class="relative w-full sm:w-80">
        <Search class="w-4 h-4 text-slate-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
        <input
          v-model="search"
          type="text"
          :placeholder="t('l_search_placeholder')"
          class="w-full pl-10 pr-3.5 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm text-slate-800 placeholder-slate-400 focus:outline-hidden focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
        />
      </div>
    </div>

    <!-- Tab 1 & 3: Requests Table Container -->
    <div v-if="activeTab === 'my' || activeTab === 'all'" class="bg-white rounded-2xl border border-slate-200/80 shadow-xs overflow-hidden">
      <LeaveRequestsTable :requests="requests" :loading="loading" />
      <BasePagination
        :page-index="pageIndex"
        :page-size="pageSize"
        :total-count="totalCount"
        :total-pages="totalPages"
        @change-page="(p) => { onPageChange(p); fetchRequests(); }"
      />
    </div>

    <!-- Approvals Tab -->
    <div v-else-if="activeTab === 'approvals'" class="bg-white rounded-2xl border border-slate-200/80 shadow-xs overflow-hidden">
      <LeaveApprovalsTable
        :pending-approvals="pendingApprovals"
        :loading="pendingLoading"
        @approve="handleApprove"
        @reject="handleReject"
      />
    </div>

    <!-- Modals -->
    <LeaveRequestModal
      v-model="showCreateModal"
      :request-form="requestForm"
      :employees="employees"
      :loading="createLoading"
      @submit="handleCreateRequest"
    />

    <LeaveConfigBalanceModal
      v-model="showQuotaModal"
      :quota-form="quotaForm"
      :employees="employees"
      :loading="quotaLoading"
      @submit="handleUpdateQuota"
    />
  </div>
</template>
