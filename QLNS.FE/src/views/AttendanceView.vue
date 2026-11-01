<script setup lang="ts">
import { ref, reactive, onMounted, watch } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useToastStore } from '@/stores/toast';
import { useI18n } from '@/locales';
import { attendanceApi } from '@/api/attendance.api';
import { employeeApi } from '@/api/employee.api';
import type { AttendanceDto, AddAttendanceDto, EmployeeDto } from '@/types';
import { AttendanceStatus } from '@/types';
import { usePagination } from '@/composables/usePagination';
import AttendanceFaceScanner from './attendance/components/AttendanceFaceScanner.vue';
import FirstTimeFaceModal from './attendance/components/FirstTimeFaceModal.vue';
import AttendanceRecordsTable from './attendance/components/AttendanceRecordsTable.vue';
import MyAttendanceTable from './attendance/components/MyAttendanceTable.vue';
import AttendanceFormModal from './attendance/components/AttendanceFormModal.vue';
import BasePagination from '@/components/BasePagination.vue';
import ConfirmModal from '@/components/ConfirmModal.vue';
import { CalendarCheck2, ScanFace, Search, Plus, User } from 'lucide-vue-next';

const authStore = useAuthStore();
const toastStore = useToastStore();
const { t } = useI18n();

const activeTab = ref<'kiosk' | 'records' | 'my-attendance'>(
  authStore.isEmployee ? 'kiosk' : 'records'
);
const showUpdateFaceModal = ref(false);

// Pagination & search
const { pageIndex, pageSize, totalCount, totalPages, setPaginationData, onPageChange, resetPage } = usePagination(10);
const search = ref('');
const loading = ref(false);
const attendanceList = ref<AttendanceDto[]>([]);
const employees = ref<EmployeeDto[]>([]);

const fetchRecords = async () => {
  loading.value = true;
  try {
    const res = await attendanceApi.getFilter(pageIndex.value, pageSize.value, search.value.trim());
    if (res.isSuccess && res.data) {
      attendanceList.value = res.data.items || [];
      setPaginationData(res.data.totalCount, res.data.totalPages);
    }
  } catch {
    toastStore.error(t('e_load_failed'));
  } finally {
    loading.value = false;
  }
};

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

// My Attendance state
const myAttendance = ref<AttendanceDto[]>([]);
const myMonth = ref(new Date().getMonth() + 1);
const myYear = ref(new Date().getFullYear());
const myLoading = ref(false);

const fetchMyAttendance = async () => {
  myLoading.value = true;
  try {
    const res = await attendanceApi.getMyAttendance(myMonth.value, myYear.value);
    if (res.isSuccess && res.data) {
      myAttendance.value = res.data;
    }
  } catch (error) {
    console.error('Failed to load my attendance:', error);
  } finally {
    myLoading.value = false;
  }
};

// Manual Form Modal
const showModal = ref(false);
const isEditing = ref(false);
const editingId = ref<string | null>(null);
const formLoading = ref(false);

const formData = reactive<AddAttendanceDto>({
  employeeId: '',
  date: new Date().toISOString().slice(0, 10),
  checkIn: '08:30:00',
  checkOut: '17:30:00',
  status: AttendanceStatus.Present,
});

const openAddModal = () => {
  isEditing.value = false;
  editingId.value = null;
  formData.employeeId = employees.value[0]?.id || '';
  formData.date = new Date().toISOString().slice(0, 10);
  formData.checkIn = '08:30:00';
  formData.checkOut = '17:30:00';
  formData.status = AttendanceStatus.Present;
  showModal.value = true;
};

const openEditModal = (att: AttendanceDto) => {
  isEditing.value = true;
  editingId.value = att.id;
  formData.employeeId = att.employeeId;
  formData.date = att.date?.split('T')[0] || '';
  formData.checkIn = att.checkIn || undefined;
  formData.checkOut = att.checkOut || undefined;
  formData.status = att.status;
  showModal.value = true;
};

const handleSubmit = async () => {
  if (!formData.employeeId || !formData.date) {
    toastStore.warning(t('e_required_fields'));
    return;
  }

  formLoading.value = true;
  try {
    if (isEditing.value && editingId.value) {
      const res = await attendanceApi.update(editingId.value, formData);
      if (res.isSuccess) {
        toastStore.success(t('t_attendance_updated'));
        showModal.value = false;
        fetchRecords();
      }
    } else {
      const res = await attendanceApi.add(formData);
      if (res.isSuccess) {
        toastStore.success(t('t_attendance_added'));
        showModal.value = false;
        fetchRecords();
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
    const res = await attendanceApi.delete(deleteTargetId.value);
    if (res.isSuccess) {
      toastStore.success(t('t_attendance_deleted'));
      showDeleteModal.value = false;
      fetchRecords();
    }
  } catch {
    toastStore.error(t('e_delete_failed'));
  } finally {
    deleteLoading.value = false;
  }
};

let searchTimeout: ReturnType<typeof setTimeout> | null = null;
watch(search, () => {
  if (searchTimeout) clearTimeout(searchTimeout);
  searchTimeout = setTimeout(() => {
    resetPage();
    fetchRecords();
  }, 400);
});

watch(activeTab, (tab) => {
  if (tab === 'records') fetchRecords();
  if (tab === 'my-attendance') fetchMyAttendance();
});

onMounted(async () => {
  if (authStore.canManagePayroll) {
    if (authStore.isAdmin) {
      await loadEmployees();
    }
    fetchRecords();
  }
});
</script>

<template>
  <div class="space-y-4 sm:space-y-5">
    <!-- Header Page Actions -->
    <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3 shrink-0">
      <div>
        <h1 class="text-lg sm:text-xl font-bold text-slate-800 tracking-tight flex items-center gap-2.5">
          <CalendarCheck2 class="w-5 h-5 text-indigo-600" />
          {{ t('n_attendance') }}
        </h1>
        <p class="text-xs text-slate-500 hidden sm:block mt-0.5">
          {{ t('l_attendance_subtitle') }}
        </p>
      </div>

      <!-- Navigation Tabs -->
      <div class="flex items-center p-1 bg-slate-200/80 rounded-xl text-xs sm:text-sm font-semibold">
        <button
          v-if="authStore.canManagePayroll"
          @click="activeTab = 'records'"
          :class="[
            'flex items-center gap-2 px-3.5 py-1.5 rounded-lg transition-all',
            activeTab === 'records' ? 'bg-white text-indigo-700 shadow-xs' : 'text-slate-600 hover:text-slate-900',
          ]"
        >
          <CalendarCheck2 class="w-4 h-4" />
          {{ t('l_tab_attendance_records') }}
        </button>

        <button
          @click="activeTab = 'kiosk'"
          :class="[
            'flex items-center gap-2 px-3.5 py-1.5 rounded-lg transition-all',
            activeTab === 'kiosk' ? 'bg-white text-indigo-700 shadow-xs' : 'text-slate-600 hover:text-slate-900',
          ]"
        >
          <ScanFace class="w-4 h-4" />
          {{ t('b_open_face_scanner') }}
        </button>

        <button
          @click="activeTab = 'my-attendance'"
          :class="[
            'flex items-center gap-2 px-3.5 py-1.5 rounded-lg transition-all',
            activeTab === 'my-attendance' ? 'bg-white text-indigo-700 shadow-xs' : 'text-slate-600 hover:text-slate-900',
          ]"
        >
          <User class="w-4 h-4" />
          {{ t('l_tab_my_attendance') }}
        </button>
      </div>
    </div>

    <!-- Tab 1: Records Table -->
    <div v-if="activeTab === 'records'" class="space-y-4 sm:space-y-5">
      <!-- Search & Add Bar -->
      <div class="bg-white p-3 sm:p-4 rounded-2xl border border-slate-200/80 shadow-xs flex flex-col sm:flex-row items-center justify-between gap-3 shrink-0">
        <div class="relative w-full sm:w-80">
          <Search class="w-4 h-4 text-slate-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
          <input
            v-model="search"
            type="text"
            :placeholder="t('l_search_placeholder')"
            class="w-full pl-10 pr-3.5 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm text-slate-800 placeholder-slate-400 focus:outline-hidden focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
          />
        </div>

        <button
          v-if="authStore.isAdmin"
          @click="openAddModal"
          class="inline-flex items-center gap-2 px-3.5 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-xs sm:text-sm font-semibold shadow-md shadow-indigo-600/20 transition-colors w-full sm:w-auto justify-center"
        >
          <Plus class="w-4 h-4" />
          {{ t('b_manual_attendance') }}
        </button>
      </div>

      <div class="bg-white rounded-2xl border border-slate-200/80 shadow-xs overflow-hidden">
        <AttendanceRecordsTable
          :records="attendanceList"
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
          @change-page="(p) => { onPageChange(p); fetchRecords(); }"
        />
      </div>
    </div>

    <!-- Tab 2: Face Recognition Attendance -->
    <div v-else-if="activeTab === 'kiosk'" class="py-2 flex items-center justify-center">
      <AttendanceFaceScanner
        @record-updated="() => { if (authStore.canManagePayroll) fetchRecords(); }"
        @open-update-face="showUpdateFaceModal = true"
      />
    </div>

    <!-- Tab 3: My Attendance -->
    <div v-else-if="activeTab === 'my-attendance'">
      <MyAttendanceTable
        :my-attendance="myAttendance"
        :loading="myLoading"
        v-model:month="myMonth"
        v-model:year="myYear"
        @reload="fetchMyAttendance"
      />
    </div>

    <!-- Modals -->
    <FirstTimeFaceModal
      :show="showUpdateFaceModal"
      :mandatory="false"
      @close="showUpdateFaceModal = false"
    />

    <AttendanceFormModal
      v-model="showModal"
      :is-editing="isEditing"
      :form-data="formData"
      :employees="employees"
      :loading="formLoading"
      @submit="handleSubmit"
    />

    <ConfirmModal
      v-model="showDeleteModal"
      :title="t('l_confirm_delete_attendance_title')"
      :message="t('l_confirm_delete_attendance_msg')"
      :loading="deleteLoading"
      @confirm="handleDelete"
    />
  </div>
</template>
