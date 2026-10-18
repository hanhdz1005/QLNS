<script setup lang="ts">
import type { PayrollGenerateDto } from '@/types';
import { useI18n } from '@/locales';
import BaseModal from '@/components/BaseModal.vue';

defineProps<{
  modelValue: boolean;
  generateForm: PayrollGenerateDto;
  loading: boolean;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', val: boolean): void;
  (e: 'submit'): void;
}>();

const { t } = useI18n();
</script>

<template>
  <BaseModal
    :model-value="modelValue"
    @update:model-value="emit('update:modelValue', $event)"
    max-width="md"
    :title="t('l_generate_payroll_modal_title')"
  >
    <form @submit.prevent="emit('submit')" class="space-y-4 text-xs">
      <div class="grid grid-cols-2 gap-3">
        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_payroll_period') }}</label>
          <select
            v-model.number="generateForm.month"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 font-semibold"
          >
            <option v-for="m in 12" :key="m" :value="m">{{ t('l_month_prefix') }} {{ m }}</option>
          </select>
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_year_prefix') }}</label>
          <select
            v-model.number="generateForm.year"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 font-semibold"
          >
            <option v-for="y in [2025, 2026, 2027]" :key="y" :value="y">{{ t('l_year_prefix') }} {{ y }}</option>
          </select>
        </div>
      </div>

      <div>
        <label class="block font-semibold text-slate-700 mb-1">{{ t('l_standard_working_days') }}</label>
        <input
          v-model.number="generateForm.standardWorkingDays"
          type="number"
          min="15"
          max="31"
          required
          class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 font-mono"
        />
      </div>

      <div class="space-y-2 pt-2 border-t border-slate-100">
        <label class="flex items-center gap-2 cursor-pointer select-none">
          <input
            v-model="generateForm.calculateAttendance"
            type="checkbox"
            class="rounded-md border-slate-300 text-indigo-600 focus:ring-indigo-500 w-4 h-4"
          />
          <span class="text-slate-700">{{ t('l_calc_attendance_option') }}</span>
        </label>

        <label class="flex items-center gap-2 cursor-pointer select-none">
          <input
            v-model="generateForm.includeInsurance"
            type="checkbox"
            class="rounded-md border-slate-300 text-indigo-600 focus:ring-indigo-500 w-4 h-4"
          />
          <span class="text-slate-700">{{ t('l_include_insurance_option') }}</span>
        </label>

        <label class="flex items-center gap-2 cursor-pointer select-none">
          <input
            v-model="generateForm.includeTax"
            type="checkbox"
            class="rounded-md border-slate-300 text-indigo-600 focus:ring-indigo-500 w-4 h-4"
          />
          <span class="text-slate-700">{{ t('l_include_tax_option') }}</span>
        </label>

        <label class="flex items-center gap-2 cursor-pointer select-none">
          <input
            v-model="generateForm.saveToDatabase"
            type="checkbox"
            class="rounded-md border-slate-300 text-indigo-600 focus:ring-indigo-500 w-4 h-4"
          />
          <span class="text-slate-700">{{ t('l_save_to_db_option') }}</span>
        </label>
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
        <span v-if="loading">{{ t('l_calculating_payroll') }}</span>
        <span v-else>{{ t('l_run_calculate') }}</span>
      </button>
    </template>
  </BaseModal>
</template>
