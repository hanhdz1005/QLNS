<script setup lang="ts">
import { useI18n } from '@/locales';
import type { AddBonusPenaltyDto, EmployeeDto } from '@/types';
import { BonusPenaltyType } from '@/types';
import BaseModal from '@/components/BaseModal.vue';

defineProps<{
  modelValue: boolean;
  isEditing: boolean;
  formData: AddBonusPenaltyDto;
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
    :title="isEditing ? t('l_edit_bonus_penalty_modal_title') : t('l_add_bonus_penalty_modal_title')"
    max-width="md"
  >
    <form @submit.prevent="emit('submit')" class="space-y-4 text-xs">
      <div>
        <label class="block font-semibold text-slate-700 mb-1">{{ t('l_employee') }} *</label>
        <select v-model="formData.employeeId" required class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl">
          <option v-for="e in employees" :key="e.id" :value="e.id">{{ e.fullName }} ({{ e.code }})</option>
        </select>
      </div>

      <div class="grid grid-cols-2 gap-4">
        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_action_type') }} *</label>
          <select v-model.number="formData.type" required class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl">
            <option :value="BonusPenaltyType.Bonus">{{ t('l_bonus_form') }}</option>
            <option :value="BonusPenaltyType.Penalty">{{ t('l_penalty_form') }}</option>
          </select>
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_amount_vnd') }} *</label>
          <input v-model.number="formData.amount" type="number" min="0" step="100000" required class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl" />
        </div>
      </div>

      <div class="grid grid-cols-3 gap-3">
        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_decision_date') }}</label>
          <input v-model="formData.date" type="date" required class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl" />
        </div>
        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_applied_month') }}</label>
          <select v-model.number="formData.month" class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl">
            <option v-for="m in 12" :key="m" :value="m">{{ t('l_month_prefix') }} {{ m }}</option>
          </select>
        </div>
        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_year_prefix') }}</label>
          <input v-model.number="formData.year" type="number" class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl" />
        </div>
      </div>

      <div>
        <label class="block font-semibold text-slate-700 mb-1">{{ t('l_specific_reason') }} *</label>
        <textarea
          v-model="formData.reason"
          rows="3"
          required
          :placeholder="t('l_decision_reason_placeholder')"
          class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl"
        />
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
        <span v-else>{{ isEditing ? t('b_save') : t('b_add_decision') }}</span>
      </button>
    </template>
  </BaseModal>
</template>
