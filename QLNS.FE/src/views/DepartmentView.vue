<script setup lang="ts">
import { ref, reactive, onMounted, watch } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useToastStore } from '@/stores/toast';
import { useI18n } from '@/locales';
import { departmentApi } from '@/api/department.api';
import type { DepartmentDto, AddDepartmentDto } from '@/types';
import { formatDate } from '@/utils/formatters';
import BaseModal from '@/components/BaseModal.vue';
import BasePagination from '@/components/BasePagination.vue';
import ConfirmModal from '@/components/ConfirmModal.vue';
import { Building2, Search, Plus, Edit, Trash2 } from 'lucide-vue-next';

const authStore = useAuthStore();
const toastStore = useToastStore();
const { t } = useI18n();

const loading = ref(false);
const departments = ref<DepartmentDto[]>([]);
const pageIndex = ref(1);
const pageSize = ref(10);
const totalCount = ref(0);
const totalPages = ref(1);
const search = ref('');

const fetchDepartments = async () => {
  loading.value = true;
  try {
    const res = await departmentApi.getFilter(pageIndex.value, pageSize.value, search.value.trim());
    if (res.isSuccess && res.data) {
      departments.value = res.data.items || [];
      totalCount.value = res.data.totalCount;
      totalPages.value = res.data.totalPages;
    }
  } catch (error) {
    toastStore.error(t('e_load_departments_failed'));
  } finally {
    loading.value = false;
  }
};

let searchTimeout: any;
watch(search, () => {
  clearTimeout(searchTimeout);
  searchTimeout = setTimeout(() => {
    pageIndex.value = 1;
    fetchDepartments();
  }, 400);
});

onMounted(() => {
  fetchDepartments();
});

// Add / Edit Modal
const showModal = ref(false);
const isEditing = ref(false);
const editingId = ref<string | null>(null);
const formLoading = ref(false);

const formData = reactive<AddDepartmentDto>({
  departmentName: '',
  description: '',
});

const openAddModal = () => {
  isEditing.value = false;
  editingId.value = null;
  formData.departmentName = '';
  formData.description = '';
  showModal.value = true;
};

const openEditModal = (d: DepartmentDto) => {
  isEditing.value = true;
  editingId.value = d.id;
  formData.departmentName = d.departmentName;
  formData.description = d.description || '';
  showModal.value = true;
};

const handleSubmit = async () => {
  if (!formData.departmentName.trim()) {
    toastStore.warning(t('e_enter_department_name'));
    return;
  }

  formLoading.value = true;
  try {
    if (isEditing.value && editingId.value) {
      const res = await departmentApi.update(editingId.value, formData);
      if (res.isSuccess) {
        toastStore.success(t('t_department_updated'));
        showModal.value = false;
        fetchDepartments();
      }
    } else {
      const res = await departmentApi.add(formData);
      if (res.isSuccess) {
        toastStore.success(t('t_department_added'));
        showModal.value = false;
        fetchDepartments();
      }
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_save_department_failed');
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
    const res = await departmentApi.delete(deleteTargetId.value);
    if (res.isSuccess) {
      toastStore.success(t('t_department_deleted'));
      showDeleteModal.value = false;
      fetchDepartments();
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_delete_department_failed');
    toastStore.error(msg);
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
          <Building2 class="w-5 h-5 text-indigo-600" />
          {{ t('l_department_management_title') }}
        </h1>
        <p class="text-xs text-slate-500 hidden sm:block mt-0.5">
          {{ t('l_departments_subtitle') }}
        </p>
      </div>

      <button
        v-if="authStore.isAdmin"
        @click="openAddModal"
        class="inline-flex items-center gap-2 px-3.5 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-xs sm:text-sm font-semibold shadow-md shadow-indigo-600/20 transition-colors"
      >
        <Plus class="w-4 h-4" />
        {{ t('b_add_department') }}
      </button>
    </div>

    <!-- Search Toolbar -->
    <div class="bg-white p-3 sm:p-4 rounded-2xl border border-slate-200/80 shadow-xs flex items-center justify-between shrink-0">
      <div class="relative w-full max-w-sm">
        <Search class="w-4 h-4 text-slate-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
        <input
          v-model="search"
          type="text"
          :placeholder="t('l_search_department_placeholder')"
          class="w-full pl-10 pr-3.5 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm text-slate-800 placeholder-slate-400 focus:outline-hidden focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500"
        />
      </div>
    </div>

    <!-- Department Table -->
    <div class="bg-white rounded-2xl border border-slate-200/80 shadow-xs overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse text-xs">
          <thead>
            <tr class="bg-slate-50/90 border-b border-slate-200/80 text-slate-500 font-semibold uppercase tracking-wider">
              <th class="py-3 px-4">{{ t('l_department_name') }}</th>
              <th class="py-3 px-4">{{ t('l_department_code') }}</th>
              <th class="py-3 px-4">{{ t('l_department_description') }}</th>
              <th class="py-3 px-4">{{ t('l_established_date') }}</th>
              <th class="py-3 px-4 text-right">{{ t('l_actions') }}</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr v-if="loading">
              <td colspan="5" class="py-10 text-center text-slate-400">
                {{ t('l_loading_departments') }}
              </td>
            </tr>

            <tr v-else-if="departments.length === 0">
              <td colspan="5" class="py-10 text-center text-slate-400">
                {{ t('l_no_departments') }}
              </td>
            </tr>

            <tr
              v-for="dept in departments"
              :key="dept.id"
              class="hover:bg-slate-50/60 transition-colors"
            >
              <td class="py-3 px-4 font-semibold text-slate-800 text-xs sm:text-sm">
                {{ dept.departmentName }}
              </td>

              <td class="py-3 px-4 font-mono text-indigo-600 font-medium">
                {{ dept.code || '—' }}
              </td>

              <td class="py-3 px-4 text-slate-500 max-w-md truncate">
                {{ dept.description || t('l_no_description') }}
              </td>

              <td class="py-3 px-4 text-slate-500">
                {{ formatDate(dept.createDate) }}
              </td>

              <td class="py-3 px-4 text-right">
                <div v-if="authStore.isAdmin" class="inline-flex items-center gap-1">
                  <button
                    @click="openEditModal(dept)"
                    :title="t('b_edit')"
                    class="p-1.5 text-slate-500 hover:text-amber-600 hover:bg-amber-50 rounded-lg transition-colors"
                  >
                    <Edit class="w-4 h-4" />
                  </button>

                  <button
                    @click="confirmDelete(dept.id)"
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
        :total-pages="totalPages"
        @change-page="(p) => { pageIndex = p; fetchDepartments(); }"
      />
    </div>

    <!-- Modal Form -->
    <BaseModal
      v-model="showModal"
      :title="isEditing ? t('l_edit_department_modal_title') : t('l_add_department_modal_title')"
      max-width="md"
    >
      <form @submit.prevent="handleSubmit" class="space-y-4 text-xs">
        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_department_name') }} *</label>
          <input
            v-model="formData.departmentName"
            type="text"
            required
            :placeholder="t('l_department_name_placeholder')"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
          />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_department_function_desc') }}</label>
          <textarea
            v-model="formData.description"
            rows="3"
            :placeholder="t('l_department_function_placeholder')"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
          />
        </div>
      </form>

      <template #footer>
        <button
          type="button"
          @click="showModal = false"
          class="px-4 py-2 text-slate-600 hover:bg-slate-100 rounded-xl font-medium text-xs transition-colors"
        >
          {{ t('b_cancel') }}
        </button>
        <button
          type="button"
          :disabled="formLoading"
          @click="handleSubmit"
          class="px-5 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl font-semibold text-xs shadow-md shadow-indigo-600/20 transition-colors disabled:opacity-50"
        >
          <span v-if="formLoading">{{ t('b_saving') }}</span>
          <span v-else>{{ isEditing ? t('b_save') : t('b_save_new') }}</span>
        </button>
      </template>
    </BaseModal>

    <!-- Delete Confirm -->
    <ConfirmModal
      v-model="showDeleteModal"
      :title="t('l_confirm_delete_department_title')"
      :message="t('l_confirm_delete_department_msg')"
      :loading="deleteLoading"
      @confirm="handleDelete"
    />
  </div>
</template>
