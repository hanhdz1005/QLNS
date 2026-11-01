<script setup lang="ts">
import { ref, reactive, onMounted, watch } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useToastStore } from '@/stores/toast';
import { useI18n } from '@/locales';
import { salaryApi } from '@/api/salary.api';
import { employeeApi } from '@/api/employee.api';
import type { BonusPenaltyDto, AddBonusPenaltyDto, EmployeeDto } from '@/types';
import { BonusPenaltyType } from '@/types';
import { formatDate, formatCurrency } from '@/utils/formatters';
import BasePagination from '@/components/BasePagination.vue';
import StatusBadge from '@/components/StatusBadge.vue';
import ConfirmModal from '@/components/ConfirmModal.vue';
import BonusPenaltyFormModal from '@/views/bonus-penalty/components/BonusPenaltyFormModal.vue';
import { Award, Search, Plus, Edit, Trash2 } from 'lucide-vue-next';

const authStore = useAuthStore();
const toastStore = useToastStore();
const { t } = useI18n();

const loading = ref(false);
const list = ref<BonusPenaltyDto[]>([]);
const employees = ref<EmployeeDto[]>([]);

const pageIndex = ref(1);
const pageSize = ref(10);
const totalCount = ref(0);
const totalPages = ref(1);
const search = ref('');
const selectedType = ref<number | undefined>(undefined);

const loadEmployees = async () => {
  try {
    const res = await employeeApi.getAll();
    if (res.isSuccess && res.data?.items) {
      employees.value = res.data.items;
    }
  } catch {
    // ignore
  }
};

const fetchList = async () => {
  loading.value = true;
  try {
    const res = await salaryApi.getFilterBonusPenalties(
      pageIndex.value,
      pageSize.value,
      search.value.trim(),
      selectedType.value
    );
    if (res.isSuccess && res.data) {
      list.value = res.data.items || [];
      totalCount.value = res.data.totalCount;
      totalPages.value = res.data.totalPages;
    }
  } catch {
    toastStore.error(t('e_load_bonus_penalty_failed'));
  } finally {
    loading.value = false;
  }
};

let searchTimeout: any;
watch(search, () => {
  clearTimeout(searchTimeout);
  searchTimeout = setTimeout(() => {
    pageIndex.value = 1;
    fetchList();
  }, 400);
});

watch(selectedType, () => {
  pageIndex.value = 1;
  fetchList();
});

onMounted(async () => {
  await loadEmployees();
  fetchList();
});

// Modal: Add / Edit
const showModal = ref(false);
const isEditing = ref(false);
const editingId = ref<string | null>(null);
const formLoading = ref(false);

const now = new Date();
const formData = reactive<AddBonusPenaltyDto>({
  employeeId: '',
  type: BonusPenaltyType.Bonus,
  amount: 1000000,
  reason: '',
  date: now.toISOString().slice(0, 10),
  month: now.getMonth() + 1,
  year: now.getFullYear(),
});

const openAddModal = () => {
  isEditing.value = false;
  editingId.value = null;
  formData.employeeId = employees.value[0]?.id || '';
  formData.type = BonusPenaltyType.Bonus;
  formData.amount = 1000000;
  formData.reason = '';
  formData.date = new Date().toISOString().slice(0, 10);
  formData.month = new Date().getMonth() + 1;
  formData.year = new Date().getFullYear();
  showModal.value = true;
};

const openEditModal = (item: BonusPenaltyDto) => {
  isEditing.value = true;
  editingId.value = item.id;
  formData.employeeId = item.employeeId;
  formData.type = item.type;
  formData.amount = item.amount;
  formData.reason = item.reason;
  formData.date = item.date?.split('T')[0] || '';
  formData.month = item.month;
  formData.year = item.year;
  showModal.value = true;
};

const handleSubmit = async () => {
  if (!formData.employeeId || !formData.reason || !formData.amount) {
    toastStore.warning(t('e_enter_bonus_penalty_details'));
    return;
  }

  formLoading.value = true;
  try {
    if (isEditing.value && editingId.value) {
      const res = await salaryApi.updateBonusPenalty(editingId.value, formData);
      if (res.isSuccess) {
        toastStore.success(t('t_bonus_penalty_updated'));
        showModal.value = false;
        fetchList();
      }
    } else {
      const res = await salaryApi.addBonusPenalty(formData);
      if (res.isSuccess) {
        toastStore.success(t('t_bonus_penalty_added'));
        showModal.value = false;
        fetchList();
      }
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_save_failed');
    toastStore.error(msg);
  } finally {
    formLoading.value = false;
  }
};

// Delete
const showDeleteModal = ref(false);
const deleteTargetId = ref<string | null>(null);
const deleteLoading = ref(false);

const confirmDelete = (id: string) => {
  deleteTargetId.value = id;
  showDeleteModal.value = true;
};

const handleDelete = async () => {
  if (!deleteTargetId.value) return;
  deleteLoading.value = true;
  try {
    const res = await salaryApi.deleteBonusPenalty(deleteTargetId.value);
    if (res.isSuccess) {
      toastStore.success(t('t_bonus_penalty_deleted'));
      showDeleteModal.value = false;
      fetchList();
    }
  } catch {
    toastStore.error(t('e_delete_failed'));
  } finally {
    deleteLoading.value = false;
  }
};
</script>

<template>
  <div class="space-y-4 sm:space-y-5">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3 shrink-0">
      <div>
        <h1 class="text-lg sm:text-xl font-bold text-slate-800 tracking-tight flex items-center gap-2.5">
          <Award class="w-5 h-5 text-indigo-600" />
          {{ t('l_bonus_penalty_title') }}
        </h1>
        <p class="text-xs text-slate-500 hidden sm:block mt-0.5">
          {{ t('l_bonus_penalty_subtitle') }}
        </p>
      </div>

      <button
        v-if="authStore.canManagePayroll"
        @click="openAddModal"
        class="inline-flex items-center gap-2 px-3.5 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-xs sm:text-sm font-semibold shadow-md shadow-indigo-600/20 transition-colors"
      >
        <Plus class="w-4 h-4" />
        {{ t('b_add_decision') }}
      </button>
    </div>

    <!-- Filters & Toolbar -->
    <div class="bg-white p-3 sm:p-4 rounded-2xl border border-slate-200/80 shadow-xs flex flex-col sm:flex-row items-center justify-between gap-3 shrink-0">
      <div class="relative w-full sm:w-80">
        <Search class="w-4 h-4 text-slate-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
        <input
          v-model="search"
          type="text"
          :placeholder="t('l_search_bonus_penalty_placeholder')"
          class="w-full pl-10 pr-3.5 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm text-slate-800 placeholder-slate-400 focus:outline-hidden focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500"
        />
      </div>

      <div class="flex items-center gap-2.5">
        <select
          v-model="selectedType"
          class="py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm text-slate-700 font-semibold focus:outline-hidden focus:border-indigo-500"
        >
          <option :value="undefined">{{ t('l_all_forms') }}</option>
          <option :value="BonusPenaltyType.Bonus">{{ t('l_bonus_form') }}</option>
          <option :value="BonusPenaltyType.Penalty">{{ t('l_penalty_form') }}</option>
        </select>
      </div>
    </div>

    <!-- Table -->
    <div class="bg-white rounded-2xl border border-slate-200/80 shadow-xs overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse text-xs">
          <thead>
            <tr class="bg-slate-50/90 border-b border-slate-200/80 text-slate-500 font-semibold uppercase tracking-wider">
              <th class="py-3 px-4">{{ t('l_employee') }}</th>
              <th class="py-3 px-4">{{ t('l_action_type') }}</th>
              <th class="py-3 px-4">{{ t('l_amount') }}</th>
              <th class="py-3 px-4">{{ t('l_specific_reason') }}</th>
              <th class="py-3 px-4">{{ t('l_decision_date') }}</th>
              <th class="py-3 px-4">{{ t('l_applied_payroll_period') }}</th>
              <th class="py-3 px-4 text-right">{{ t('l_actions') }}</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr v-if="loading">
              <td colspan="7" class="py-10 text-center text-slate-400">{{ t('l_loading_bonus_penalty') }}</td>
            </tr>
            <tr v-else-if="list.length === 0">
              <td colspan="7" class="py-10 text-center text-slate-400">{{ t('l_no_bonus_penalty') }}</td>
            </tr>
            <tr
              v-for="item in list"
              :key="item.id"
              class="hover:bg-slate-50/60 transition-colors"
            >
              <td class="py-3 px-4 font-semibold text-slate-800 text-xs sm:text-sm">{{ item.employeeName }}</td>
              <td class="py-3 px-4">
                <StatusBadge type="bonusPenalty" :value="item.type" />
              </td>
              <td
                class="py-3 px-4 font-bold font-mono text-xs sm:text-sm"
                :class="item.type === BonusPenaltyType.Bonus ? 'text-emerald-600' : 'text-rose-600'"
              >
                {{ item.type === BonusPenaltyType.Bonus ? '+' : '-' }}{{ formatCurrency(item.amount) }}
              </td>
              <td class="py-3 px-4 text-slate-600 max-w-xs truncate">{{ item.reason }}</td>
              <td class="py-3 px-4 text-slate-600">{{ formatDate(item.date) }}</td>
              <td class="py-3 px-4 font-medium text-slate-700">{{ t('l_month_prefix') }} {{ item.month }}/{{ item.year }}</td>
              <td class="py-3 px-4 text-right">
                <div v-if="authStore.canManagePayroll" class="inline-flex items-center gap-1">
                  <button
                    @click="openEditModal(item)"
                    :title="t('b_edit')"
                    class="p-1.5 text-slate-500 hover:text-amber-600 hover:bg-amber-50 rounded-lg transition-colors"
                  >
                    <Edit class="w-4 h-4" />
                  </button>
                  <button
                    @click="confirmDelete(item.id)"
                    :title="t('b_delete')"
                    class="p-1.5 text-slate-500 hover:text-rose-600 hover:bg-rose-50 rounded-lg transition-colors"
                  >
                    <Trash2 class="w-4 h-4" />
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
        :totalPages="totalPages"
        @change-page="(p) => { pageIndex = p; fetchList(); }"
      />
    </div>

    <!-- Modal Form (Add / Edit) -->
    <BonusPenaltyFormModal
      v-model="showModal"
      :is-editing="isEditing"
      :form-data="formData"
      :employees="employees"
      :loading="formLoading"
      @submit="handleSubmit"
    />

    <!-- Delete Confirm -->
    <ConfirmModal
      v-model="showDeleteModal"
      :title="t('l_confirm_delete_bonus_penalty_title')"
      :message="t('l_confirm_delete_bonus_penalty_msg')"
      :loading="deleteLoading"
      @confirm="handleDelete"
    />
  </div>
</template>
