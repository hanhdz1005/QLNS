<script setup lang="ts">
import type { UpdateLeaveBalanceDto, EmployeeDto } from '@/types';
import { useI18n } from '@/locales';
import BaseModal from '@/components/BaseModal.vue';

defineProps<{
  modelValue: boolean;
  quotaForm: UpdateLeaveBalanceDto;
  employees: EmployeeDto[];
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
    :title="t('l_adjust_quota_modal_title')"
  >
    <form @submit.prevent="emit('submit')" class="space-y-4 text-xs">
      <div>
        <label class="block font-semibold text-slate-700 mb-1">{{ t('l_full_name') }} *</label>
        <select
          v-model="quotaForm.employeeId"
          required
          class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500"
        >
          <option v-for="e in employees" :key="e.id" :value="e.id">
            {{ e.fullName }} ({{ e.code }})
          </option>
        </select>
      </div>

      <div class="grid grid-cols-2 gap-3">
        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_applicable_year') }} *</label>
          <input
            v-model.number="quotaForm.year"
            type="number"
            min="2020"
            max="2035"
            required
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500"
          />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_total_leave_days') }} *</label>
          <input
            v-model.number="quotaForm.totalLeaveDays"
            type="number"
            min="1"
            max="30"
            required
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500"
          />
        </div>
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
        <span v-else>{{ t('b_save') }}</span>
      </button>
    </template>
  </BaseModal>
</template>
