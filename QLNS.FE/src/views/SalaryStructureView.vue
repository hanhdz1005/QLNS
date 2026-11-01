<script setup lang="ts">
import { ref, reactive, onMounted, watch } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useToastStore } from '@/stores/toast';
import { useI18n } from '@/locales';
import { salaryApi } from '@/api/salary.api';
import { employeeApi } from '@/api/employee.api';
import type { SalariesDto, AddSalariesDto, EmployeeDto } from '@/types';
import { formatCurrency } from '@/utils/formatters';
import BasePagination from '@/components/BasePagination.vue';
import ConfirmModal from '@/components/ConfirmModal.vue';
import SalaryFormModal from '@/views/salary/components/SalaryFormModal.vue';
import { Sliders, Search, Plus, Edit, Trash2 } from 'lucide-vue-next';

const authStore = useAuthStore();
const toastStore = useToastStore();
const { t } = useI18n();

const loading = ref(false);
const salaries = ref<SalariesDto[]>([]);
const employees = ref<EmployeeDto[]>([]);
const pageIndex = ref(1);
const pageSize = ref(10);
const totalCount = ref(0);
const totalPages = ref(1);
const search = ref('');

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

const fetchSalaries = async () => {
  loading.value = true;
  try {
    const res = await salaryApi.getFilterSalaries(pageIndex.value, pageSize.value, search.value.trim());
    if (res.isSuccess && res.data) {
      salaries.value = res.data.items || [];
      totalCount.value = res.data.totalCount;
      totalPages.value = res.data.totalPages;
    }
  } catch {
    toastStore.error(t('e_load_salaries_failed'));
  } finally {
    loading.value = false;
  }
};

let searchTimeout: any;
watch(search, () => {
  clearTimeout(searchTimeout);
  searchTimeout = setTimeout(() => {
    pageIndex.value = 1;
    fetchSalaries();
  }, 400);
});

onMounted(async () => {
  await loadEmployees();
  fetchSalaries();
});

// Modal: Add / Edit
const showModal = ref(false);
const isEditing = ref(false);
const editingId = ref<string | null>(null);
const formLoading = ref(false);

const now = new Date();
const formData = reactive<AddSalariesDto>({
  employeeId: '',
  allowance: 1000000,
  bonus: 0,
  penalty: 0,
  actualWorkDays: 22,
  paidLeaveDays: 0,
  overtimePay: 0,
  insurance: 0,
  tax: 0,
  month: now.getMonth() + 1,
  year: now.getFullYear(),
});

const openAddModal = () => {
  isEditing.value = false;
  editingId.value = null;
  formData.employeeId = employees.value[0]?.id || '';
  formData.allowance = 1000000;
  formData.bonus = 0;
  formData.penalty = 0;
  formData.actualWorkDays = 22;
  formData.paidLeaveDays = 0;
  formData.overtimePay = 0;
  formData.insurance = 0;
  formData.tax = 0;
  formData.month = now.getMonth() + 1;
  formData.year = now.getFullYear();
  showModal.value = true;
};

const openEditModal = (s: SalariesDto) => {
  isEditing.value = true;
  editingId.value = s.id;
  formData.employeeId = s.employeeId;
  formData.allowance = s.allowance;
  formData.bonus = s.bonus;
  formData.penalty = s.penalty;
  formData.actualWorkDays = s.actualWorkDays;
  formData.paidLeaveDays = s.paidLeaveDays;
  formData.overtimePay = s.overtimePay;
  formData.insurance = s.insurance;
  formData.tax = s.tax;
  formData.month = s.month;
  formData.year = s.year;
  showModal.value = true;
};

const handleSubmit = async () => {
  if (!formData.employeeId) {
    toastStore.warning(t('e_select_employee'));
    return;
  }

  formLoading.value = true;
  try {
    if (isEditing.value && editingId.value) {
      const res = await salaryApi.updateSalary(editingId.value, formData);
      if (res.isSuccess) {
        toastStore.success(t('t_salary_updated'));
        showModal.value = false;
        fetchSalaries();
      }
    } else {
      const res = await salaryApi.addSalary(formData);
      if (res.isSuccess) {
        toastStore.success(t('t_salary_added'));
        showModal.value = false;
        fetchSalaries();
      }
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_save_salary_failed');
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
    const res = await salaryApi.deleteSalary(deleteTargetId.value);
    if (res.isSuccess) {
      toastStore.success(t('t_salary_deleted'));
      showDeleteModal.value = false;
      fetchSalaries();
    }
  } catch {
    toastStore.error(t('e_delete_salary_failed'));
  } finally {
    deleteLoading.value = false;
  }
};
</script>

<template>
  <div class="space-y-4 sm:space-y-5">
    <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3 shrink-0">
      <div>
        <h1 class="text-lg sm:text-xl font-bold text-slate-800 tracking-tight flex items-center gap-2.5">
          <Sliders class="w-5 h-5 text-indigo-600" />
          {{ t('l_salary_structure_title') }}
        </h1>
        <p class="text-xs text-slate-500 hidden sm:block mt-0.5">
          {{ t('l_salary_structure_subtitle') }}
        </p>
      </div>

      <button
        v-if="authStore.canManagePayroll"
        @click="openAddModal"
        class="inline-flex items-center gap-2 px-3.5 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-xs sm:text-sm font-semibold shadow-md shadow-indigo-600/20 transition-colors"
      >
        <Plus class="w-4 h-4" />
        {{ t('b_add_new_record') }}
      </button>
    </div>

    <!-- Search Toolbar -->
    <div class="bg-white p-3 sm:p-4 rounded-2xl border border-slate-200/80 shadow-xs flex items-center justify-between shrink-0">
      <div class="relative w-full max-w-sm">
        <Search class="w-4 h-4 text-slate-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
        <input
          v-model="search"
          type="text"
          :placeholder="t('l_search_employee_placeholder')"
          class="w-full pl-10 pr-3.5 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm text-slate-800 placeholder-slate-400 focus:outline-hidden focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500"
        />
      </div>
    </div>

    <!-- Salaries Table -->
    <div class="bg-white rounded-2xl border border-slate-200/80 shadow-xs overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse text-xs">
          <thead>
            <tr class="bg-slate-50/90 border-b border-slate-200/80 text-slate-500 font-semibold uppercase tracking-wider">
              <th class="py-3 px-4">{{ t('l_employee') }}</th>
              <th class="py-3 px-4">{{ t('l_payroll_period') }}</th>
              <th class="py-3 px-4">{{ t('l_basic_salary') }}</th>
              <th class="py-3 px-4">{{ t('l_work_days') }}</th>
              <th class="py-3 px-4">{{ t('l_allowance') }}</th>
              <th class="py-3 px-4">{{ t('l_gross_salary') }}</th>
              <th class="py-3 px-4 text-right">{{ t('l_total_take_home') }}</th>
              <th class="py-3 px-4 text-right">{{ t('l_actions') }}</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr v-if="loading">
              <td colspan="8" class="py-10 text-center text-slate-400">{{ t('l_loading_salaries') }}</td>
            </tr>
            <tr v-else-if="salaries.length === 0">
              <td colspan="8" class="py-10 text-center text-slate-400">{{ t('l_no_salaries') }}</td>
            </tr>
            <tr
              v-for="s in salaries"
              :key="s.id"
              class="hover:bg-slate-50/60 transition-colors"
            >
              <td class="py-3 px-4 font-semibold text-slate-800 text-xs sm:text-sm">{{ s.employeeName }}</td>
              <td class="py-3 px-4 font-medium text-indigo-700">{{ t('l_month_prefix') }} {{ s.month }}/{{ s.year }}</td>
              <td class="py-3 px-4 font-mono text-slate-700">{{ formatCurrency(s.basicSalary) }}</td>
              <td class="py-3 px-4 text-slate-600">{{ s.actualWorkDays }} {{ t('l_days_unit') }}</td>
              <td class="py-3 px-4 text-emerald-600 font-medium font-mono">+{{ formatCurrency(s.allowance) }}</td>
              <td class="py-3 px-4 text-slate-700 font-semibold font-mono">{{ formatCurrency(s.grossSalary) }}</td>
              <td class="py-3 px-4 text-right font-bold text-emerald-600 font-mono text-xs sm:text-sm">{{ formatCurrency(s.totalSalary) }}</td>
              <td class="py-3 px-4 text-right">
                <div v-if="authStore.canManagePayroll" class="inline-flex items-center gap-1">
                  <button
                    @click="openEditModal(s)"
                    :title="t('b_edit')"
                    class="p-1.5 text-slate-500 hover:text-amber-600 hover:bg-amber-50 rounded-lg transition-colors"
                  >
                    <Edit class="w-4 h-4" />
                  </button>
                  <button
                    @click="confirmDelete(s.id)"
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
        @change-page="(p) => { pageIndex = p; fetchSalaries(); }"
      />
    </div>

    <!-- Modal Form (Add / Edit) -->
    <SalaryFormModal
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
      :title="t('l_confirm_delete_salary_title')"
      :message="t('l_confirm_delete_salary_msg')"
      :loading="deleteLoading"
      @confirm="handleDelete"
    />
  </div>
</template>
