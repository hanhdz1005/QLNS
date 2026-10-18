<script setup lang="ts">
import { ref, reactive, onMounted, watch } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useToastStore } from '@/stores/toast';
import { useI18n } from '@/locales';
import { contractApi } from '@/api/contract.api';
import { employeeApi } from '@/api/employee.api';
import type { ContractDto, AddContractDto, EmployeeDto } from '@/types';
import { ContractType, ContractStatus } from '@/types';
import { usePagination } from '@/composables/usePagination';
import ContractFilterBar from './contract/components/ContractFilterBar.vue';
import ContractTable from './contract/components/ContractTable.vue';
import ContractFormModal from './contract/components/ContractFormModal.vue';
import BasePagination from '@/components/BasePagination.vue';
import ConfirmModal from '@/components/ConfirmModal.vue';

const authStore = useAuthStore();
const toastStore = useToastStore();
const { t } = useI18n();

const { pageIndex, pageSize, totalCount, totalPages, setPaginationData, onPageChange, resetPage } = usePagination(10);
const loading = ref(false);
const contracts = ref<ContractDto[]>([]);
const employees = ref<EmployeeDto[]>([]);
const search = ref('');
const onlyExpiring = ref(false);

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

const fetchContracts = async () => {
  loading.value = true;
  try {
    if (onlyExpiring.value) {
      const res = await contractApi.getExpiring(30);
      if (res.isSuccess && res.data) {
        contracts.value = res.data;
        setPaginationData(res.data.length, 1);
      }
    } else {
      const res = await contractApi.getFilter(pageIndex.value, pageSize.value, search.value.trim());
      if (res.isSuccess && res.data) {
        contracts.value = res.data.items || [];
        setPaginationData(res.data.totalCount, res.data.totalPages);
      }
    }
  } catch {
    toastStore.error(t('e_load_failed'));
  } finally {
    loading.value = false;
  }
};

let searchTimeout: ReturnType<typeof setTimeout> | null = null;
watch(search, () => {
  if (searchTimeout) clearTimeout(searchTimeout);
  searchTimeout = setTimeout(() => {
    resetPage();
    fetchContracts();
  }, 400);
});

watch(onlyExpiring, () => {
  resetPage();
  fetchContracts();
});

onMounted(async () => {
  await loadEmployees();
  fetchContracts();
});

// Modal: Add / Edit
const showModal = ref(false);
const isEditing = ref(false);
const editingId = ref<string | null>(null);
const formLoading = ref(false);

const formData = reactive<AddContractDto>({
  employeeId: '',
  contractNumber: '',
  contractType: ContractType.DefiniteTerm,
  startDate: new Date().toISOString().slice(0, 10),
  endDate: undefined,
  signDate: new Date().toISOString().slice(0, 10),
  contractSalary: 15000000,
  insuranceSalary: 5000000,
  attachmentUrl: '',
  status: ContractStatus.Active,
  notes: '',
});

const openAddModal = () => {
  isEditing.value = false;
  editingId.value = null;
  formData.employeeId = employees.value[0]?.id || '';
  formData.contractNumber = `HD-${Date.now().toString().slice(-6)}`;
  formData.contractType = ContractType.DefiniteTerm;
  formData.startDate = new Date().toISOString().slice(0, 10);
  formData.endDate = undefined;
  formData.signDate = new Date().toISOString().slice(0, 10);
  formData.contractSalary = 15000000;
  formData.insuranceSalary = 5000000;
  formData.attachmentUrl = '';
  formData.status = ContractStatus.Active;
  formData.notes = '';
  showModal.value = true;
};

const openEditModal = (c: ContractDto) => {
  isEditing.value = true;
  editingId.value = c.id;
  formData.employeeId = c.employeeId;
  formData.contractNumber = c.contractNumber;
  formData.contractType = c.contractType;
  formData.startDate = c.startDate?.split('T')[0] || '';
  formData.endDate = c.endDate ? c.endDate.split('T')[0] : undefined;
  formData.signDate = c.signDate?.split('T')[0] || '';
  formData.contractSalary = c.contractSalary;
  formData.insuranceSalary = c.insuranceSalary;
  formData.attachmentUrl = c.attachmentUrl || '';
  formData.status = c.status;
  formData.notes = c.notes || '';
  showModal.value = true;
};

const handleSubmit = async () => {
  if (!formData.employeeId || !formData.contractNumber || !formData.startDate || !formData.signDate) {
    toastStore.warning(t('e_required_fields'));
    return;
  }

  formLoading.value = true;
  try {
    if (isEditing.value && editingId.value) {
      const res = await contractApi.update(editingId.value, formData);
      if (res.isSuccess) {
        toastStore.success(t('t_contract_updated'));
        showModal.value = false;
        fetchContracts();
      }
    } else {
      const res = await contractApi.add(formData);
      if (res.isSuccess) {
        toastStore.success(t('t_contract_added'));
        showModal.value = false;
        fetchContracts();
      }
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_save_failed');
    toastStore.error(msg);
  } finally {
    formLoading.value = false;
  }
};

// Modal: Delete
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
    const res = await contractApi.delete(deleteTargetId.value);
    if (res.isSuccess) {
      toastStore.success(t('t_contract_deleted'));
      showDeleteModal.value = false;
      fetchContracts();
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
    <!-- Filter Toolbar -->
    <ContractFilterBar
      v-model:search="search"
      v-model:only-expiring="onlyExpiring"
      :is-admin="authStore.isAdmin"
      @open-add="openAddModal"
    />

    <!-- Main Table Container -->
    <div class="bg-white rounded-2xl border border-slate-200/80 shadow-xs overflow-hidden">
      <ContractTable
        :contracts="contracts"
        :loading="loading"
        :is-admin="authStore.isAdmin"
        @edit="openEditModal"
        @delete="confirmDelete"
      />

      <BasePagination
        :page-index="pageIndex"
        :page-size="pageSize"
        :total-count="totalCount"
        :total-pages="totalPages"
        @change-page="(p) => { onPageChange(p); fetchContracts(); }"
      />
    </div>

    <!-- Modals -->
    <ContractFormModal
      v-model="showModal"
      :is-editing="isEditing"
      :form-data="formData"
      :employees="employees"
      :loading="formLoading"
      @submit="handleSubmit"
    />

    <ConfirmModal
      v-model="showDeleteModal"
      :title="t('l_confirm_delete_contract_title')"
      :message="t('l_confirm_delete_contract_msg')"
      :loading="deleteLoading"
      @confirm="handleDelete"
    />
  </div>
</template>
