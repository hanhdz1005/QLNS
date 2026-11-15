<script setup lang="ts">
import { ref, reactive, computed, onMounted, watch } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useToastStore } from '@/stores/toast';
import { useCompanySettingsStore } from '@/stores/companySettings';
import { useI18n } from '@/locales';
import { usePagination } from '@/composables/usePagination';
import { overtimeApi } from '@/api/overtime.api';
import {
  OvertimeRequestStatus,
  OvertimeType,
  OvertimeCompensationType,
  type OvertimeRequestDto,
  type CreateOvertimeRequestDto,
  type ReviewOvertimeRequestDto,
  type OvertimeMonthlySummaryDto,
} from '@/types';
import { formatDate, formatTime } from '@/utils/formatters';
import BasePagination from '@/components/BasePagination.vue';
import OvertimeSummaryCards from './OvertimeSummaryCards.vue';
import OvertimeRequestModal from './OvertimeRequestModal.vue';
import OvertimeReviewModal from './OvertimeReviewModal.vue';
import { Flame, CheckCircle2, XCircle, Clock, Plus, Eye } from 'lucide-vue-next';

const authStore = useAuthStore();
const toastStore = useToastStore();
const settingsStore = useCompanySettingsStore();
const { t } = useI18n();

const activeOtTab = ref<'my' | 'pending' | 'all'>('my');
const myOtList = ref<OvertimeRequestDto[]>([]);
const pendingOtList = ref<OvertimeRequestDto[]>([]);
const allOtList = ref<OvertimeRequestDto[]>([]);
const summary = ref<OvertimeMonthlySummaryDto | null>(null);
const loading = ref(false);

const currentList = computed(() =>
  activeOtTab.value === 'my' ? myOtList.value : activeOtTab.value === 'pending' ? pendingOtList.value : allOtList.value
);

const { pageIndex, pageSize, onPageChange, resetPage } = usePagination(10);
const totalCount = computed(() => currentList.value.length);
const totalPages = computed(() => Math.max(1, Math.ceil(totalCount.value / pageSize.value)));
const paginatedItems = computed(() => {
  const start = (pageIndex.value - 1) * pageSize.value;
  return currentList.value.slice(start, start + pageSize.value);
});

const fetchSummary = async () => {
  try {
    const isCompanyScope = activeOtTab.value !== 'my' && authStore.canApproveRequests;
    const now = new Date();
    const res = await overtimeApi.getMonthlySummary(now.getMonth() + 1, now.getFullYear(), isCompanyScope);
    if (res.isSuccess && res.data) summary.value = res.data;
  } catch {
    // ignore
  }
};

const fetchOtData = async () => {
  loading.value = true;
  try {
    if (activeOtTab.value === 'my') {
      const res = await overtimeApi.getMyRequests();
      if (res.isSuccess && res.data) myOtList.value = res.data.items || [];
    } else if (activeOtTab.value === 'pending' && authStore.canApproveRequests) {
      const res = await overtimeApi.getPendingApprovals();
      if (res.isSuccess && res.data) pendingOtList.value = res.data.items || [];
    } else if (activeOtTab.value === 'all') {
      const res = await overtimeApi.getAllRequests();
      if (res.isSuccess && res.data) allOtList.value = res.data.items || [];
    }
    resetPage();
    await fetchSummary();
  } catch {
    toastStore.error(t('e_ot_load_failed'));
  } finally {
    loading.value = false;
  }
};

const refreshPendingCount = async () => {
  if (!authStore.canApproveRequests) return;
  try {
    const res = await overtimeApi.getPendingApprovals();
    if (res.isSuccess && res.data) pendingOtList.value = res.data.items || [];
  } catch {
    // ignore
  }
};

const showOtModal = ref(false);
const createLoading = ref(false);
const otForm = reactive<CreateOvertimeRequestDto>({
  date: new Date().toISOString().slice(0, 10),
  startTime: settingsStore.settings.afternoonEnd || '17:30',
  endTime: '19:30',
  registeredHours: 2,
  otType: OvertimeType.Weekday,
  compensationType: settingsStore.settings.defaultOtMode,
  reason: '',
});

const openRegisterModal = () => {
  const today = new Date();
  const isWeekend =
    today.getDay() === 0 || (today.getDay() === 6 && settingsStore.settings.excludeSaturday);
  otForm.date = today.toISOString().slice(0, 10);
  otForm.startTime = settingsStore.settings.afternoonEnd || '17:30';
  otForm.endTime = '19:30';
  otForm.registeredHours = 2;
  otForm.otType = isWeekend ? OvertimeType.Weekend : OvertimeType.Weekday;
  otForm.compensationType = settingsStore.settings.defaultOtMode;
  otForm.reason = '';
  showOtModal.value = true;
};

const handleCreateOt = async () => {
  if (!otForm.date || !otForm.reason.trim()) {
    toastStore.warning(t('e_required_fields'));
    return;
  }
  if (!otForm.registeredHours || otForm.registeredHours < 0.5 || otForm.registeredHours > 16) {
    toastStore.warning(t('e_ot_invalid_hours'));
    return;
  }
  createLoading.value = true;
  try {
    const res = await overtimeApi.createRequest(otForm);
    if (res.isSuccess) {
      toastStore.success(t('t_ot_request_created'));
      showOtModal.value = false;
      fetchOtData();
      refreshPendingCount();
    }
  } catch (error: any) {
    toastStore.error(error.response?.data?.message || t('e_ot_create_failed'));
  } finally {
    createLoading.value = false;
  }
};

const showReviewModal = ref(false);
const activeReviewItem = ref<OvertimeRequestDto | null>(null);
const reviewLoading = ref(false);

const openReviewModal = (item: OvertimeRequestDto) => {
  activeReviewItem.value = item;
  showReviewModal.value = true;
};

const handleApproveOt = async (payload: ReviewOvertimeRequestDto) => {
  if (!activeReviewItem.value) return;
  reviewLoading.value = true;
  try {
    const res = await overtimeApi.approveRequest(activeReviewItem.value.id, payload);
    if (res.isSuccess) {
      toastStore.success(t('t_ot_request_approved'));
      showReviewModal.value = false;
      fetchOtData();
      refreshPendingCount();
    }
  } catch (error: any) {
    toastStore.error(error.response?.data?.message || t('e_ot_approve_failed'));
  } finally {
    reviewLoading.value = false;
  }
};

const handleRejectOt = async (payload: ReviewOvertimeRequestDto) => {
  if (!activeReviewItem.value) return;
  reviewLoading.value = true;
  try {
    const res = await overtimeApi.rejectRequest(activeReviewItem.value.id, payload);
    if (res.isSuccess) {
      toastStore.info(t('t_ot_request_rejected'));
      showReviewModal.value = false;
      fetchOtData();
      refreshPendingCount();
    }
  } catch (error: any) {
    toastStore.error(error.response?.data?.message || t('e_ot_reject_failed'));
  } finally {
    reviewLoading.value = false;
  }
};

const handleCancelOt = async (id: string) => {
  try {
    const res = await overtimeApi.cancelRequest(id);
    if (res.isSuccess) {
      toastStore.info(t('t_ot_request_cancelled'));
      fetchOtData();
      refreshPendingCount();
    }
  } catch (error: any) {
    toastStore.error(error.response?.data?.message || t('e_ot_cancel_failed'));
  }
};

const isPendingStatus = (status: OvertimeRequestStatus | number | string) =>
  status === OvertimeRequestStatus.Pending || status === 1 || status === '1';

const getOtStatusBadge = (status: OvertimeRequestStatus | number | string) => {
  if (status === OvertimeRequestStatus.Approved || status === 2 || status === '2') {
    return { label: t('s_approved'), cls: 'bg-emerald-50 text-emerald-700 border-emerald-200' };
  }
  if (status === OvertimeRequestStatus.Rejected || status === 3 || status === '3') {
    return { label: t('s_rejected'), cls: 'bg-rose-50 text-rose-700 border-rose-200' };
  }
  if (status === OvertimeRequestStatus.Cancelled || status === 4 || status === '4') {
    return { label: t('s_cancelled'), cls: 'bg-slate-100 text-slate-500 border-slate-200' };
  }
  return { label: t('s_pending'), cls: 'bg-amber-50 text-amber-700 border-amber-200' };
};

const getCompBadge = (comp: OvertimeCompensationType | number | string) => {
  if (comp === OvertimeCompensationType.RecognitionOnly || comp === 2 || comp === '2') {
    return { label: t('l_ot_comp_recognition'), cls: 'bg-indigo-50 text-indigo-700 border-indigo-200' };
  }
  if (comp === OvertimeCompensationType.Paid || comp === 3 || comp === '3') {
    return { label: t('l_ot_comp_paid'), cls: 'bg-amber-50 text-amber-700 border-amber-200' };
  }
  return { label: t('l_ot_comp_leave'), cls: 'bg-emerald-50 text-emerald-700 border-emerald-200' };
};

watch(activeOtTab, () => fetchOtData());
onMounted(() => {
  fetchOtData();
  refreshPendingCount();
});
</script>

<template>
  <div class="space-y-4">
    <OvertimeSummaryCards :summary="summary" />

    <!-- OT Header & Sub-tabs -->
    <div class="bg-white p-4 rounded-2xl border border-slate-200/80 shadow-xs flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3">
      <div>
        <h2 class="text-sm sm:text-base font-bold text-slate-800 flex items-center gap-2">
          <Flame class="w-4 h-4 text-amber-500" />
          {{ t('l_ot_section_title') }}
        </h2>
        <p class="text-xs text-slate-500 mt-0.5">{{ t('l_ot_section_subtitle') }}</p>
      </div>

      <div class="flex flex-wrap items-center gap-2.5 w-full sm:w-auto justify-between sm:justify-end">
        <div class="flex items-center p-1 bg-slate-100 rounded-xl text-xs font-semibold">
          <button
            @click="activeOtTab = 'my'"
            :class="['px-3 py-1.5 rounded-lg transition-all flex items-center gap-1.5', activeOtTab === 'my' ? 'bg-white text-indigo-700 shadow-2xs' : 'text-slate-600 hover:text-slate-900']"
          >
            <Clock class="w-3.5 h-3.5" /> {{ t('l_tab_my_ot') }}
          </button>

          <button
            v-if="authStore.canApproveRequests"
            @click="activeOtTab = 'pending'"
            :class="['px-3 py-1.5 rounded-lg transition-all flex items-center gap-1.5', activeOtTab === 'pending' ? 'bg-white text-indigo-700 shadow-2xs' : 'text-slate-600 hover:text-slate-900']"
          >
            <CheckCircle2 class="w-3.5 h-3.5" /> {{ t('l_tab_pending_ot') }} ({{ pendingOtList.length }})
          </button>

          <button
            v-if="authStore.canManageHR || authStore.canManagePayroll || authStore.canApproveRequests"
            @click="activeOtTab = 'all'"
            :class="['px-3 py-1.5 rounded-lg transition-all flex items-center gap-1.5', activeOtTab === 'all' ? 'bg-white text-indigo-700 shadow-2xs' : 'text-slate-600 hover:text-slate-900']"
          >
            <Flame class="w-3.5 h-3.5" /> {{ t('l_tab_all_ot') }}
          </button>
        </div>

        <button
          @click="openRegisterModal"
          class="inline-flex items-center gap-1.5 px-3.5 py-2 bg-amber-500 hover:bg-amber-600 text-white rounded-xl text-xs font-semibold shadow-xs transition-colors"
        >
          <Plus class="w-4 h-4" /> {{ t('b_register_ot') }}
        </button>
      </div>
    </div>

    <!-- OT Table -->
    <div class="bg-white rounded-2xl border border-slate-200/80 shadow-xs overflow-hidden">
      <div class="overflow-x-auto" tabindex="0" role="region" :aria-label="t('l_ot_section_title')">
        <table class="w-full text-left border-collapse text-xs">
          <thead>
            <tr class="bg-slate-50/90 border-b border-slate-200/80 text-slate-500 font-semibold uppercase tracking-wider">
              <th class="py-3 px-4">{{ t('l_full_name') }}</th>
              <th class="py-3 px-4">{{ t('l_ot_date') }} &amp; {{ t('l_ot_time_window') }}</th>
              <th class="py-3 px-4">{{ t('l_ot_registered_hours') }}</th>
              <th class="py-3 px-4">{{ t('l_ot_compensation_label') }}</th>
              <th class="py-3 px-4">{{ t('l_ot_actual_attendance_col') }}</th>
              <th class="py-3 px-4">{{ t('l_ot_reason') }}</th>
              <th class="py-3 px-4">{{ t('l_status') }}</th>
              <th class="py-3 px-4 text-right">{{ t('l_actions') }}</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr v-if="loading">
              <td colspan="8" class="py-10 text-center text-slate-400">{{ t('l_loading_data') }}</td>
            </tr>
            <tr v-else-if="paginatedItems.length === 0">
              <td colspan="8" class="py-10 text-center text-slate-400">
                {{ activeOtTab === 'pending' ? t('l_no_pending_ot') : t('l_no_ot_requests') }}
              </td>
            </tr>
            <tr v-for="item in paginatedItems" :key="item.id" class="hover:bg-slate-50/70 transition-colors">
              <td class="py-3 px-4">
                <div class="font-semibold text-slate-800">{{ item.employeeName }}</div>
                <div class="text-[11px] text-slate-400">{{ item.departmentName || item.employeeCode || '—' }}</div>
              </td>
              <td class="py-3 px-4">
                <div class="font-semibold text-slate-800">{{ formatDate(item.date) }}</div>
                <div v-if="item.startTime && item.endTime" class="text-[11px] font-mono text-slate-500">
                  {{ formatTime(item.startTime) }} &rarr; {{ formatTime(item.endTime) }}
                </div>
              </td>
              <td class="py-3 px-4">
                <div class="inline-flex items-center gap-1 px-2.5 py-1 rounded-lg bg-amber-50 text-amber-700 border border-amber-200 font-mono font-bold text-xs">
                  <Flame class="w-3.5 h-3.5 text-amber-500" />
                  +{{ item.registeredHours }}h
                  <span class="text-[10px] opacity-80">({{ item.multiplier || 1.5 }}x)</span>
                </div>
              </td>
              <td class="py-3 px-4">
                <span :class="['px-2.5 py-1 rounded-lg text-[11px] font-semibold border', getCompBadge(item.compensationType).cls]">
                  {{ getCompBadge(item.compensationType).label }}
                </span>
              </td>
              <td class="py-3 px-4">
                <span
                  v-if="item.actualCheckIn || item.actualCheckOut"
                  class="font-mono text-[11px] font-semibold text-emerald-700 bg-emerald-50 px-2 py-0.5 rounded-md border border-emerald-200"
                >
                  {{ formatTime(item.actualCheckIn) }} &rarr; {{ formatTime(item.actualCheckOut) }}
                </span>
                <span v-else class="text-[11px] text-slate-400">{{ t('l_ot_no_attendance_yet') }}</span>
              </td>
              <td class="py-3 px-4 max-w-xs">
                <div class="text-slate-700 truncate" :title="item.reason">{{ item.reason }}</div>
                <div v-if="item.approverNote" class="text-[11px] text-indigo-600 truncate mt-0.5" :title="item.approverNote">
                  {{ item.approverNote }}
                </div>
              </td>
              <td class="py-3 px-4">
                <span :class="['px-2.5 py-1 rounded-lg text-xs font-semibold border', getOtStatusBadge(item.status).cls]">
                  {{ getOtStatusBadge(item.status).label }}
                </span>
              </td>
              <td class="py-3 px-4 text-right">
                <div class="inline-flex items-center gap-1.5">
                  <button
                    v-if="authStore.canApproveRequests && (activeOtTab === 'pending' || isPendingStatus(item.status))"
                    @click="openReviewModal(item)"
                    class="inline-flex items-center gap-1 px-2.5 py-1.5 bg-indigo-600 hover:bg-indigo-700 text-white rounded-lg text-xs font-semibold transition-colors"
                  >
                    <Eye class="w-3.5 h-3.5" /> {{ t('b_review_ot') }}
                  </button>
                  <button
                    v-if="activeOtTab === 'my' && isPendingStatus(item.status)"
                    @click="handleCancelOt(item.id)"
                    class="inline-flex items-center gap-1 px-2.5 py-1.5 bg-rose-50 hover:bg-rose-100 text-rose-700 border border-rose-200 rounded-lg text-xs font-semibold transition-colors"
                  >
                    <XCircle class="w-3.5 h-3.5" /> {{ t('b_cancel_ot') }}
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <BasePagination
        :page-index="pageIndex"
        :page-size="pageSize"
        :total-count="totalCount"
        :total-pages="totalPages"
        @change-page="onPageChange"
      />
    </div>

    <OvertimeRequestModal v-model="showOtModal" :ot-form="otForm" :loading="createLoading" @submit="handleCreateOt" />
    <OvertimeReviewModal
      v-model="showReviewModal"
      :request="activeReviewItem"
      :loading="reviewLoading"
      @approve="handleApproveOt"
      @reject="handleRejectOt"
    />
  </div>
</template>
