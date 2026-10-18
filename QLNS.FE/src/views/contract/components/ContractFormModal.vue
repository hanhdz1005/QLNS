<script setup lang="ts">
import { ref } from 'vue';
import { fileApi } from '@/api/file.api';
import { useToastStore } from '@/stores/toast';
import { useI18n } from '@/locales';
import type { AddContractDto, EmployeeDto } from '@/types';
import { ContractType, ContractStatus } from '@/types';
import BaseModal from '@/components/BaseModal.vue';

const props = defineProps<{
  modelValue: boolean;
  isEditing: boolean;
  formData: AddContractDto;
  employees: EmployeeDto[];
  loading: boolean;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', val: boolean): void;
  (e: 'submit'): void;
}>();

const toastStore = useToastStore();
const { t } = useI18n();
const uploadLoading = ref(false);

const handleFileUpload = async (e: Event) => {
  const target = e.target as HTMLInputElement;
  if (!target.files || target.files.length === 0) return;
  const file = target.files[0];
  if (!file) return;

  uploadLoading.value = true;
  try {
    const res = await fileApi.upload(file);
    if (res.isSuccess && res.data) {
      props.formData.attachmentUrl = res.data.url;
      toastStore.success(t('t_file_upload_success'));
    }
  } catch {
    toastStore.error(t('e_file_upload_failed'));
  } finally {
    uploadLoading.value = false;
  }
};
</script>

<template>
  <BaseModal
    :model-value="modelValue"
    @update:model-value="emit('update:modelValue', $event)"
    max-width="2xl"
    :title="isEditing ? t('l_edit_contract_modal_title') : t('l_add_contract_modal_title')"
  >
    <form @submit.prevent="emit('submit')" class="space-y-4 text-xs">
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_contract_employee') }} *</label>
          <select
            v-model="formData.employeeId"
            required
            :disabled="isEditing"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 disabled:bg-slate-100"
          >
            <option v-for="e in employees" :key="e.id" :value="e.id">
              {{ e.fullName }} ({{ e.code }})
            </option>
          </select>
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_contract_number') }} *</label>
          <input
            v-model="formData.contractNumber"
            type="text"
            required
            placeholder="HD-2026-001"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 font-mono"
          />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_contract_type') }} *</label>
          <select
            v-model="formData.contractType"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500"
          >
            <option :value="ContractType.Probation">{{ t('s_probation_contract') }}</option>
            <option :value="ContractType.DefiniteTerm">{{ t('s_definite_term_contract') }}</option>
            <option :value="ContractType.IndefiniteTerm">{{ t('s_indefinite_term_contract') }}</option>
            <option :value="ContractType.Internship">{{ t('s_internship_contract') }}</option>
            <option :value="ContractType.PartTime">{{ t('s_part_time_contract') }}</option>
          </select>
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_status') }}</label>
          <select
            v-model="formData.status"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500"
          >
            <option :value="ContractStatus.Active">{{ t('s_contract_active') }}</option>
            <option :value="ContractStatus.Expired">{{ t('s_contract_expired') }}</option>
            <option :value="ContractStatus.Terminated">{{ t('s_contract_terminated') }}</option>
          </select>
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_start_date') }} *</label>
          <input
            v-model="formData.startDate"
            type="date"
            required
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500"
          />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_end_date') }}</label>
          <input
            v-model="formData.endDate"
            type="date"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500"
          />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_sign_date') }} *</label>
          <input
            v-model="formData.signDate"
            type="date"
            required
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500"
          />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_contract_salary') }} *</label>
          <input
            v-model.number="formData.contractSalary"
            type="number"
            min="0"
            required
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 font-mono font-semibold"
          />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_insurance_salary') }} *</label>
          <input
            v-model.number="formData.insuranceSalary"
            type="number"
            min="0"
            required
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 font-mono"
          />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_upload_contract_scan') }}</label>
          <div class="flex items-center gap-2">
            <input
              type="file"
              @change="handleFileUpload"
              class="text-xs file:py-1.5 file:px-3 file:rounded-lg file:border-0 file:bg-indigo-50 file:text-indigo-700 file:font-semibold hover:file:bg-indigo-100"
            />
          </div>
        </div>
      </div>

      <div>
        <label class="block font-semibold text-slate-700 mb-1">{{ t('l_notes') }}</label>
        <textarea
          v-model="formData.notes"
          rows="2"
          :placeholder="t('l_notes_placeholder')"
          class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500"
        ></textarea>
      </div>
    </form>

    <template #footer>
      <button
        type="button"
        @click="emit('update:modelValue', false)"
        class="px-4 py-2 text-slate-600 hover:bg-slate-100 rounded-xl transition-colors font-medium text-xs"
      >
        {{ t('b_cancel') }}
      </button>
      <button
        type="button"
        :disabled="loading"
        @click="emit('submit')"
        class="px-5 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl font-semibold text-xs shadow-md shadow-indigo-600/20 transition-colors disabled:opacity-50"
      >
        <span v-if="loading">{{ t('b_saving') }}</span>
        <span v-else>{{ isEditing ? t('b_save') : t('b_save_new') }}</span>
      </button>
    </template>
  </BaseModal>
</template>
