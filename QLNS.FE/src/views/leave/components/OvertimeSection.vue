<script setup lang="ts">
import { ref, reactive, onMounted, watch } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useToastStore } from '@/stores/toast';
import { useI18n } from '@/locales';
import { overtimeApi } from '@/api/overtime.api';
import type { OvertimeRequestDto, CreateOvertimeRequestDto } from '@/types';
import { OvertimeRequestStatus } from '@/types';
import { formatDate } from '@/utils/formatters';
import OvertimeRequestModal from './OvertimeRequestModal.vue';
import { Flame, CheckCircle2, XCircle, Clock, Plus } from 'lucide-vue-next';

const authStore = useAuthStore();
const toastStore = useToastStore();
const { t } = useI18n();

const activeOtTab = ref<'my' | 'pending' | 'all'>('my');
const myOtList = ref<OvertimeRequestDto[]>([]);
const pendingOtList = ref<OvertimeRequestDto[]>([]);
const allOtList = ref<OvertimeRequestDto[]>([]);
const loading = ref(false);

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

// Register OT Modal
const showOtModal = ref(false);
const createLoading = ref(false);
const otForm = reactive<CreateOvertimeRequestDto>({
  date: new Date().toISOString().slice(0, 10),
  registeredHours: 2,
  reason: '',
});

const openRegisterModal = () => {
  otForm.date = new Date().toISOString().slice(0, 10);
  otForm.registeredHours = 2;
  otForm.reason = '';
  showOtModal.value = true;
};

const handleCreateOt = async () => {
  if (!otForm.date || !otForm.reason.trim()) {
    toastStore.warning(t('e_required_fields'));
    return;
  }
  if (!otForm.registeredHours || otForm.registeredHours < 0.5 || otForm.registeredHours > 12) {
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
    const msg = error.response?.data?.message || t('e_ot_create_failed');
    toastStore.error(msg);
  } finally {
    createLoading.value = false;
  }
};

const handleApproveOt = async (id: string) => {
  try {
    const res = await overtimeApi.approveRequest(id);
    if (res.isSuccess) {
      toastStore.success(t('t_ot_request_approved'));
      fetchOtData();
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_ot_approve_failed');
    toastStore.error(msg);
  }
};

const handleRejectOt = async (id: string) => {
  try {
    const res = await overtimeApi.rejectRequest(id);
    if (res.isSuccess) {
      toastStore.info(t('t_ot_request_rejected'));
      fetchOtData();
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_ot_reject_failed');
    toastStore.error(msg);
  }
};

const getOtStatusBadge = (status: OvertimeRequestStatus | number | string) => {
  if (status === OvertimeRequestStatus.Approved || status === 1 || status === '1') {
    return { label: t('s_approved'), cls: 'bg-emerald-50 text-emerald-700 border-emerald-200' };
  }
  if (status === OvertimeRequestStatus.Rejected || status === 2 || status === '2') {
    return { label: t('s_rejected'), cls: 'bg-rose-50 text-rose-700 border-rose-200' };
  }
  return { label: t('s_pending'), cls: 'bg-amber-50 text-amber-700 border-amber-200' };
};

watch(activeOtTab, () => {
  fetchOtData();
});

onMounted(() => {
  fetchOtData();
  refreshPendingCount();
});
</script>

<template>
  <div class="space-y-4">
    <!-- OT Header & Sub-tabs -->
    <div class="bg-white p-4 rounded-2xl border border-slate-200/80 shadow-xs flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3">
      <div>
        <h2 class="text-sm sm:text-base font-bold text-slate-800 flex items-center gap-2">
          <Flame class="w-4 h-4 text-amber-500" />
          {{ t('l_ot_section_title') }}
        </h2>
        <p class="text-xs text-slate-500 mt-0.5">
          {{ t('l_ot_section_subtitle') }}
        </p>
      </div>

      <div class="flex flex-wrap items-center gap-2.5 w-full sm:w-auto justify-between sm:justify-end">
        <div class="flex items-center p-1 bg-slate-100 rounded-xl text-xs font-semibold">
          <button
            @click="activeOtTab = 'my'"
            :class="[
              'px-3 py-1.5 rounded-lg transition-all flex items-center gap-1.5',
              activeOtTab === 'my' ? 'bg-white text-indigo-700 shadow-2xs' : 'text-slate-600 hover:text-slate-900',
            ]"
          >
            <Clock class="w-3.5 h-3.5" />
            {{ t('l_tab_my_ot') }}
          </button>

          <button
            v-if="authStore.canApproveRequests"
            @click="activeOtTab = 'pending'"
            :class="[
              'px-3 py-1.5 rounded-lg transition-all flex items-center gap-1.5',
              activeOtTab === 'pending' ? 'bg-white text-indigo-700 shadow-2xs' : 'text-slate-600 hover:text-slate-900',
            ]"
          >
            <CheckCircle2 class="w-3.5 h-3.5" />
            {{ t('l_tab_pending_ot') }} ({{ pendingOtList.length }})
          </button>

          <button
            v-if="authStore.canManageHR || authStore.canManagePayroll || authStore.canApproveRequests"
            @click="activeOtTab = 'all'"
            :class="[
              'px-3 py-1.5 rounded-lg transition-all flex items-center gap-1.5',
              activeOtTab === 'all' ? 'bg-white text-indigo-700 shadow-2xs' : 'text-slate-600 hover:text-slate-900',
            ]"
          >
            <Flame class="w-3.5 h-3.5" />
            {{ t('l_tab_all_ot') }}
          </button>
        </div>

        <button
          @click="openRegisterModal"
          class="inline-flex items-center gap-1.5 px-3.5 py-2 bg-amber-500 hover:bg-amber-600 text-white rounded-xl text-xs font-semibold shadow-xs transition-colors"
        >
          <Plus class="w-4 h-4" />
          {{ t('b_register_ot') }}
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
              <th class="py-3 px-4">{{ t('l_department') }}</th>
              <th class="py-3 px-4">{{ t('l_ot_date') }}</th>
              <th class="py-3 px-4">{{ t('l_ot_registered_hours') }}</th>
              <th class="py-3 px-4">{{ t('l_ot_reason') }}</th>
              <th class="py-3 px-4">{{ t('l_status') }}</th>
              <th class="py-3 px-4">{{ t('l_ot_approved_by') }}</th>
              <th v-if="activeOtTab === 'pending' && authStore.canApproveRequests" class="py-3 px-4 text-right">
                {{ t('l_actions') }}
              </th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr v-if="loading">
              <td :colspan="activeOtTab === 'pending' ? 8 : 7" class="py-10 text-center text-slate-400">
                {{ t('l_loading_data') }}
              </td>
            </tr>

            <tr
              v-else-if="
                (activeOtTab === 'my' && myOtList.length === 0) ||
                (activeOtTab === 'pending' && pendingOtList.length === 0) ||
                (activeOtTab === 'all' && allOtList.length === 0)
              "
            >
              <td :colspan="activeOtTab === 'pending' ? 8 : 7" class="py-10 text-center text-slate-400">
                {{ activeOtTab === 'pending' ? t('l_no_pending_ot') : t('l_no_ot_requests') }}
              </td>
            </tr>

            <tr
              v-for="item in activeOtTab === 'my' ? myOtList : activeOtTab === 'pending' ? pendingOtList : allOtList"
              :key="item.id"
              class="hover:bg-slate-50/70 transition-colors"
            >
              <td class="py-3 px-4">
                <div class="font-semibold text-slate-800">{{ item.employeeName }}</div>
                <div class="text-[11px] font-mono text-slate-400">{{ item.employeeCode }}</div>
              </td>
              <td class="py-3 px-4 text-slate-600">{{ item.departmentName || '—' }}</td>
              <td class="py-3 px-4 font-medium text-slate-700">{{ formatDate(item.date) }}</td>
              <td class="py-3 px-4">
                <span class="inline-flex items-center gap-1 px-2.5 py-1 rounded-lg bg-amber-50 text-amber-700 border border-amber-200 font-mono font-bold text-xs">
                  <Flame class="w-3.5 h-3.5 text-amber-500" />
                  +{{ item.registeredHours }}h
                </span>
              </td>
              <td class="py-3 px-4 text-slate-700 max-w-xs truncate" :title="item.reason">
                {{ item.reason }}
              </td>
              <td class="py-3 px-4">
                <span :class="['px-2.5 py-1 rounded-lg text-xs font-semibold border', getOtStatusBadge(item.status).cls]">
                  {{ getOtStatusBadge(item.status).label }}
                </span>
              </td>
              <td class="py-3 px-4 text-slate-500">
                <div v-if="item.approvedByName || item.approvedBy" class="text-slate-700 font-medium">
                  {{ item.approvedByName || item.approvedBy }}
                </div>
                <div v-else class="text-slate-400">—</div>
              </td>
              <td v-if="activeOtTab === 'pending' && authStore.canApproveRequests" class="py-3 px-4 text-right">
                <div class="inline-flex items-center gap-1.5">
                  <button
                    @click="handleApproveOt(item.id)"
                    class="inline-flex items-center gap-1 px-2.5 py-1.5 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg text-xs font-semibold transition-colors"
                  >
                    <CheckCircle2 class="w-3.5 h-3.5" />
                    {{ t('b_approve_ot') }}
                  </button>
                  <button
                    @click="handleRejectOt(item.id)"
                    class="inline-flex items-center gap-1 px-2.5 py-1.5 bg-rose-50 hover:bg-rose-100 text-rose-700 border border-rose-200 rounded-lg text-xs font-semibold transition-colors"
                  >
                    <XCircle class="w-3.5 h-3.5" />
                    {{ t('b_reject_ot') }}
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal -->
    <OvertimeRequestModal
      v-model="showOtModal"
      :ot-form="otForm"
      :loading="createLoading"
      @submit="handleCreateOt"
    />
  </div>
</template>
