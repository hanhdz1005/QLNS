<script setup lang="ts">
import { useI18n } from '@/locales';
import type { AddSalariesDto, EmployeeDto } from '@/types';
import BaseModal from '@/components/BaseModal.vue';

defineProps<{
  modelValue: boolean;
  isEditing: boolean;
  formData: AddSalariesDto;
  employees: EmployeeDto[];
  loading: boolean;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void;
  (e: 'submit'): void;
}>();

const { t } = useI18n();
</script>

<template>
  <BaseModal
    :model-value="modelValue"
    @update:model-value="(val) => emit('update:modelValue', val)"
    :title="isEditing ? t('l_edit_salary_modal_title') : t('l_add_salary_modal_title')"
    max-width="lg"
  >
    <form @submit.prevent="emit('submit')" class="space-y-4 text-xs">
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div class="sm:col-span-2">
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_employee') }} *</label>
          <select v-model="formData.employeeId" required class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl">
            <option v-for="e in employees" :key="e.id" :value="e.id">{{ e.fullName }} ({{ e.code }})</option>
          </select>
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_month_prefix') }} *</label>
          <select v-model.number="formData.month" required class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl">
            <option v-for="m in 12" :key="m" :value="m">{{ t('l_month_prefix') }} {{ m }}</option>
          </select>
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_year_prefix') }} *</label>
          <input v-model.number="formData.year" type="number" required class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl" />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_actual_work_days') }}</label>
          <input v-model.number="formData.actualWorkDays" type="number" step="0.5" class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl" />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_paid_leave_days') }}</label>
          <input v-model.number="formData.paidLeaveDays" type="number" step="0.5" class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl" />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_allowance_vnd') }}</label>
          <input v-model.number="formData.allowance" type="number" step="100000" class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl" />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_overtime_pay_vnd') }}</label>
          <input v-model.number="formData.overtimePay" type="number" step="100000" class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl" />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_bonus_vnd') }}</label>
          <input v-model.number="formData.bonus" type="number" step="100000" class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl" />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_penalty_vnd') }}</label>
          <input v-model.number="formData.penalty" type="number" step="100000" class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl" />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_insurance_vnd') }}</label>
          <input v-model.number="formData.insurance" type="number" step="50000" class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl" />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_tax_vnd') }}</label>
          <input v-model.number="formData.tax" type="number" step="50000" class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl" />
        </div>
      </div>
    </form>

    <template #footer>
      <button
        type="button"
        @click="emit('update:modelValue', false)"
        class="px-4 py-2 text-slate-600 hover:bg-slate-100 rounded-xl font-medium text-xs transition-colors"
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
