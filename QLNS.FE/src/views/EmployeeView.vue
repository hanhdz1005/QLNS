<script setup lang="ts">
import { ref, reactive, onMounted, computed, watch } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useToastStore } from '@/stores/toast';
import { employeeApi } from '@/api/employee.api';
import { departmentApi } from '@/api/department.api';
import { positionApi } from '@/api/position.api';
import { exportApi } from '@/api/export.api';
import type { EmployeeDto, AddEmployeeDto, DepartmentDto, PositionDto } from '@/types';
import { Gender, EmployeeStatus } from '@/types';
import { usePagination } from '@/composables/usePagination';
import EmployeeFilterBar from './employee/components/EmployeeFilterBar.vue';
import EmployeeTable from './employee/components/EmployeeTable.vue';
import EmployeeFormModal from './employee/components/EmployeeFormModal.vue';
import EmployeeDetailModal from './employee/components/EmployeeDetailModal.vue';
import BasePagination from '@/components/BasePagination.vue';
import ConfirmModal from '@/components/ConfirmModal.vue';
import { useI18n } from '@/locales';

const authStore = useAuthStore();
const toastStore = useToastStore();
const { t } = useI18n();

const { pageIndex, pageSize, totalCount, totalPages, setPaginationData, onPageChange, resetPage } = usePagination(10);

const loading = ref(false);
const employees = ref<EmployeeDto[]>([]);
const departments = ref<DepartmentDto[]>([]);
const positions = ref<PositionDto[]>([]);
const search = ref('');
const selectedDepartment = ref<string>('');

const loadDepartmentsAndPositions = async () => {
  try {
    const [deptRes, posRes] = await Promise.all([
      departmentApi.getAll(),
      positionApi.getAll(),
    ]);
    if (deptRes.isSuccess && deptRes.data?.items) {
      departments.value = deptRes.data.items;
    }
    if (posRes.isSuccess && posRes.data?.items) {
      positions.value = posRes.data.items;
    }
  } catch (error) {
    console.error('Failed to load depts/positions:', error);
  }
};

const fetchEmployees = async () => {
  loading.value = true;
  try {
    const res = await employeeApi.getFilter(pageIndex.value, pageSize.value, search.value.trim());
    if (res.isSuccess && res.data) {
      employees.value = res.data.items || [];
      setPaginationData(res.data.totalCount, res.data.totalPages);
    }
  } catch {
    toastStore.error(t('e_load_failed'));
  } finally {
    loading.value = false;
  }
};

const filteredEmployees = computed(() => {
  if (!selectedDepartment.value) return employees.value;
  return employees.value.filter((e) => e.departmentId === selectedDepartment.value);
});

let searchTimeout: ReturnType<typeof setTimeout> | null = null;
watch(search, () => {
  if (searchTimeout) clearTimeout(searchTimeout);
  searchTimeout = setTimeout(() => {
    resetPage();
    fetchEmployees();
  }, 400);
});

onMounted(async () => {
  await loadDepartmentsAndPositions();
  await fetchEmployees();
});

// Modal: Add / Edit
const showFormModal = ref(false);
const isEditing = ref(false);
const editingId = ref<string | null>(null);
const formLoading = ref(false);

const formData = reactive<AddEmployeeDto>({
  fullName: '',
  dateOfBirth: '',
  gender: Gender.Male,
  phoneNumber: '',
  email: '',
  address: '',
  departmentId: '',
  positionId: '',
  joinDate: new Date().toISOString().slice(0, 10),
  status: EmployeeStatus.Active,
  citizenId: '',
  citizenIdIssueDate: '',
  citizenIdIssuePlace: '',
  taxCode: '',
  socialInsuranceNumber: '',
  dependentsCount: 0,
  bankAccountNumber: '',
  bankName: '',
  bankBranch: '',
  emergencyContactName: '',
  emergencyContactPhone: '',
  emergencyContactRelationship: '',
  managerId: undefined,
});

const openAddModal = () => {
  isEditing.value = false;
  editingId.value = null;
  Object.assign(formData, {
    fullName: '',
    dateOfBirth: '2000-01-01',
    gender: Gender.Male,
    phoneNumber: '',
    email: '',
    address: '',
    departmentId: departments.value[0]?.id || '',
    positionId: positions.value[0]?.id || '',
    joinDate: new Date().toISOString().slice(0, 10),
    status: EmployeeStatus.Active,
    citizenId: '',
    citizenIdIssueDate: '',
    citizenIdIssuePlace: '',
    taxCode: '',
    socialInsuranceNumber: '',
    dependentsCount: 0,
    bankAccountNumber: '',
    bankName: '',
    bankBranch: '',
    emergencyContactName: '',
    emergencyContactPhone: '',
    emergencyContactRelationship: '',
    managerId: undefined,
  });
  showFormModal.value = true;
};

const openEditModal = (emp: EmployeeDto) => {
  isEditing.value = true;
  editingId.value = emp.id;
  Object.assign(formData, {
    fullName: emp.fullName,
    dateOfBirth: emp.dateOfBirth?.split('T')[0] || '',
    gender: emp.gender,
    phoneNumber: emp.phoneNumber,
    email: emp.email,
    address: emp.address,
    departmentId: emp.departmentId,
    positionId: emp.positionId,
    joinDate: emp.joinDate?.split('T')[0] || '',
    status: emp.status,
    citizenId: emp.citizenId || '',
    citizenIdIssueDate: emp.citizenIdIssueDate?.split('T')[0] || '',
    citizenIdIssuePlace: emp.citizenIdIssuePlace || '',
    taxCode: emp.taxCode || '',
    socialInsuranceNumber: emp.socialInsuranceNumber || '',
    dependentsCount: emp.dependentsCount || 0,
    bankAccountNumber: emp.bankAccountNumber || '',
    bankName: emp.bankName || '',
    bankBranch: emp.bankBranch || '',
    emergencyContactName: emp.emergencyContactName || '',
    emergencyContactPhone: emp.emergencyContactPhone || '',
    emergencyContactRelationship: emp.emergencyContactRelationship || '',
    managerId: emp.managerId || undefined,
  });
  showFormModal.value = true;
};

const handleSubmit = async () => {
  if (!formData.fullName || !formData.email || !formData.phoneNumber) {
    toastStore.warning(t('e_required_fields'));
    return;
  }

  formLoading.value = true;
  try {
    const payload: AddEmployeeDto = {
      ...formData,
      citizenIdIssueDate: formData.citizenIdIssueDate || undefined,
      managerId: formData.managerId ? formData.managerId : undefined,
    };

    if (isEditing.value && editingId.value) {
      const res = await employeeApi.update(editingId.value, payload);
      if (res.isSuccess) {
        toastStore.success(t('t_employee_updated'));
        showFormModal.value = false;
        fetchEmployees();
      }
    } else {
      const res = await employeeApi.add(payload);
      if (res.isSuccess) {
        toastStore.success(t('t_employee_added'));
        showFormModal.value = false;
        fetchEmployees();
      }
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_save_failed');
    toastStore.error(msg);
  } finally {
    formLoading.value = false;
  }
};

// Modal: Detail & Delete
const showDetailModal = ref(false);
const detailEmployee = ref<EmployeeDto | null>(null);

const openDetail = (emp: EmployeeDto) => {
  detailEmployee.value = emp;
  showDetailModal.value = true;
};

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
    const res = await employeeApi.delete(deleteTargetId.value);
    if (res.isSuccess) {
      toastStore.success(t('t_employee_deleted'));
      showDeleteModal.value = false;
      fetchEmployees();
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_delete_failed');
    toastStore.error(msg);
  } finally {
    deleteLoading.value = false;
  }
};

const exportCsv = async () => {
  try {
    const blob = await exportApi.exportEmployeeCsv();
    exportApi.downloadBlob(blob, `Danh_sach_nhan_vien_${new Date().toISOString().slice(0, 10)}.csv`);
    toastStore.success(t('t_export_success'));
  } catch {
    toastStore.error(t('e_export_failed'));
  }
};
</script>

<template>
  <div class="space-y-4 sm:space-y-5">
    <!-- Filter Toolbar -->
    <EmployeeFilterBar
      :departments="departments"
      v-model:search="search"
      v-model:selected-department="selectedDepartment"
      :is-admin="authStore.isAdmin"
      @open-add="openAddModal"
      @export-csv="exportCsv"
    />

    <!-- Main Table Container -->
    <div class="bg-white rounded-2xl border border-slate-200/80 shadow-xs overflow-hidden">
      <EmployeeTable
        :employees="filteredEmployees"
        :loading="loading"
        :is-admin="authStore.isAdmin"
        @view-detail="openDetail"
        @edit="openEditModal"
        @delete="confirmDelete"
      />

      <BasePagination
        :page-index="pageIndex"
        :page-size="pageSize"
        :total-count="totalCount"
        :total-pages="totalPages"
        @change-page="(p) => { onPageChange(p); fetchEmployees(); }"
      />
    </div>

    <!-- Modals -->
    <EmployeeFormModal
      v-model="showFormModal"
      :is-editing="isEditing"
      :editing-id="editingId"
      :form-data="formData"
      :departments="departments"
      :positions="positions"
      :employees="employees"
      :loading="formLoading"
      @submit="handleSubmit"
    />

    <EmployeeDetailModal
      v-model="showDetailModal"
      :employee="detailEmployee"
    />

    <ConfirmModal
      v-model="showDeleteModal"
      :title="t('l_confirm_delete_employee_title')"
      :message="t('l_confirm_delete_employee_msg')"
      :loading="deleteLoading"
      @confirm="handleDelete"
    />
  </div>
</template>
