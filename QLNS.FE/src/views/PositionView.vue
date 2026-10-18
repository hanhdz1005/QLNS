<script setup lang="ts">
import { ref, reactive, onMounted, watch } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useToastStore } from '@/stores/toast';
import { useI18n } from '@/locales';
import { positionApi } from '@/api/position.api';
import type { PositionDto, AddPositionDto } from '@/types';
import { formatCurrency } from '@/utils/formatters';
import BaseModal from '@/components/BaseModal.vue';
import BasePagination from '@/components/BasePagination.vue';
import ConfirmModal from '@/components/ConfirmModal.vue';
import { Briefcase, Search, Plus, Edit, Trash2 } from 'lucide-vue-next';

const authStore = useAuthStore();
const toastStore = useToastStore();
const { t } = useI18n();

const loading = ref(false);
const positions = ref<PositionDto[]>([]);
const pageIndex = ref(1);
const pageSize = ref(10);
const totalCount = ref(0);
const totalPages = ref(1);
const search = ref('');

const fetchPositions = async () => {
  loading.value = true;
  try {
    const res = await positionApi.getFilter(pageIndex.value, pageSize.value, search.value.trim());
    if (res.isSuccess && res.data) {
      positions.value = res.data.items || [];
      totalCount.value = res.data.totalCount;
      totalPages.value = res.data.totalPages;
    }
  } catch (error) {
    toastStore.error(t('e_load_positions_failed'));
  } finally {
    loading.value = false;
  }
};

let searchTimeout: any;
watch(search, () => {
  clearTimeout(searchTimeout);
  searchTimeout = setTimeout(() => {
    pageIndex.value = 1;
    fetchPositions();
  }, 400);
});

onMounted(() => {
  fetchPositions();
});

// Add / Edit Modal
const showModal = ref(false);
const isEditing = ref(false);
const editingId = ref<string | null>(null);
const formLoading = ref(false);

const formData = reactive<AddPositionDto>({
  positionName: '',
  baseSalary: 10000000,
  description: '',
});

const openAddModal = () => {
  isEditing.value = false;
  editingId.value = null;
  formData.positionName = '';
  formData.baseSalary = 10000000;
  formData.description = '';
  showModal.value = true;
};

const openEditModal = (p: PositionDto) => {
  isEditing.value = true;
  editingId.value = p.id;
  formData.positionName = p.positionName;
  formData.baseSalary = p.baseSalary;
  formData.description = p.description || '';
  showModal.value = true;
};

const handleSubmit = async () => {
  if (!formData.positionName.trim()) {
    toastStore.warning(t('e_enter_position_name'));
    return;
  }

  formLoading.value = true;
  try {
    if (isEditing.value && editingId.value) {
      const res = await positionApi.update(editingId.value, formData);
      if (res.isSuccess) {
        toastStore.success(t('t_position_updated'));
        showModal.value = false;
        fetchPositions();
      }
    } else {
      const res = await positionApi.add(formData);
      if (res.isSuccess) {
        toastStore.success(t('t_position_added'));
        showModal.value = false;
        fetchPositions();
      }
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_save_position_failed');
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
    const res = await positionApi.delete(deleteTargetId.value);
    if (res.isSuccess) {
      toastStore.success(t('t_position_deleted'));
      showDeleteModal.value = false;
      fetchPositions();
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_delete_position_failed');
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
          <Briefcase class="w-5 h-5 text-indigo-600" />
          {{ t('l_position_management_title') }}
        </h1>
        <p class="text-xs text-slate-500 hidden sm:block mt-0.5">
          {{ t('l_positions_subtitle') }}
        </p>
      </div>

      <button
        v-if="authStore.isAdmin"
        @click="openAddModal"
        class="inline-flex items-center gap-2 px-3.5 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-xs sm:text-sm font-semibold shadow-md shadow-indigo-600/20 transition-colors"
      >
        <Plus class="w-4 h-4" />
        {{ t('b_add_position') }}
      </button>
    </div>

    <!-- Search Toolbar -->
    <div class="bg-white p-3 sm:p-4 rounded-2xl border border-slate-200/80 shadow-xs flex items-center justify-between shrink-0">
      <div class="relative w-full max-w-sm">
        <Search class="w-4 h-4 text-slate-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
        <input
          v-model="search"
          type="text"
          :placeholder="t('l_search_position_placeholder')"
          class="w-full pl-10 pr-3.5 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm text-slate-800 placeholder-slate-400 focus:outline-hidden focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500"
        />
      </div>
    </div>

    <!-- Positions Table -->
    <div class="bg-white rounded-2xl border border-slate-200/80 shadow-xs overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse text-xs">
          <thead>
            <tr class="bg-slate-50/90 border-b border-slate-200/80 text-slate-500 font-semibold uppercase tracking-wider">
              <th class="py-3 px-4">{{ t('l_position') }}</th>
              <th class="py-3 px-4">{{ t('l_position_standard_salary') }}</th>
              <th class="py-3 px-4">{{ t('l_position_responsibility') }}</th>
              <th class="py-3 px-4 text-right">{{ t('l_actions') }}</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr v-if="loading">
              <td colspan="4" class="py-10 text-center text-slate-400">
                {{ t('l_loading_positions') }}
              </td>
            </tr>

            <tr v-else-if="positions.length === 0">
              <td colspan="4" class="py-10 text-center text-slate-400">
                {{ t('l_no_positions') }}
              </td>
            </tr>

            <tr
              v-for="p in positions"
              :key="p.id"
              class="hover:bg-slate-50/60 transition-colors"
            >
              <td class="py-3 px-4 font-semibold text-slate-800 text-xs sm:text-sm">
                {{ p.positionName }}
              </td>

              <td class="py-3 px-4 font-bold text-emerald-600 font-mono">
                {{ formatCurrency(p.baseSalary) }}
              </td>

              <td class="py-3 px-4 text-slate-500 max-w-md truncate">
                {{ p.description || t('l_no_description') }}
              </td>

              <td class="py-3 px-4 text-right">
                <div v-if="authStore.isAdmin" class="inline-flex items-center gap-1">
                  <button
                    @click="openEditModal(p)"
                    :title="t('b_edit')"
                    class="p-1.5 text-slate-500 hover:text-amber-600 hover:bg-amber-50 rounded-lg transition-colors"
                  >
                    <Edit class="w-4 h-4" />
                  </button>

                  <button
                    @click="confirmDelete(p.id)"
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
        @change-page="(p) => { pageIndex = p; fetchPositions(); }"
      />
    </div>

    <!-- Modal Form -->
    <BaseModal
      v-model="showModal"
      :title="isEditing ? t('l_edit_position_modal_title') : t('l_add_position_modal_title')"
      max-width="md"
    >
      <form @submit.prevent="handleSubmit" class="space-y-4 text-xs">
        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_position') }} *</label>
          <input
            v-model="formData.positionName"
            type="text"
            required
            :placeholder="t('l_position_name_placeholder')"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
          />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_position_standard_salary_vnd') }} *</label>
          <input
            v-model.number="formData.baseSalary"
            type="number"
            min="0"
            step="500000"
            required
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
          />
          <div class="text-[11px] text-slate-400 mt-1">
            {{ t('l_display_preview') }}: {{ formatCurrency(formData.baseSalary) }}
          </div>
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_job_description') }}</label>
          <textarea
            v-model="formData.description"
            rows="3"
            :placeholder="t('l_job_description_placeholder')"
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
      :title="t('l_confirm_delete_position_title')"
      :message="t('l_confirm_delete_position_msg')"
      :loading="deleteLoading"
      @confirm="handleDelete"
    />
  </div>
</template>
