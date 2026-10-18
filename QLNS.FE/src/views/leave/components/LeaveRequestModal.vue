<script setup lang="ts">
import type { AddLeaveRequestDto, EmployeeDto } from '@/types';
import { LeaveType } from '@/types';
import { useI18n } from '@/locales';
import BaseModal from '@/components/BaseModal.vue';

defineProps<{
  modelValue: boolean;
  requestForm: AddLeaveRequestDto;
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
    :title="t('l_create_leave_modal_title')"
  >
    <form @submit.prevent="emit('submit')" class="space-y-4 text-xs">
      <div>
        <label class="block font-semibold text-slate-700 mb-1">{{ t('l_applicant') }} *</label>
        <select
          v-model="requestForm.employeeId"
          required
          class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500"
        >
          <option v-for="e in employees" :key="e.id" :value="e.id">
            {{ e.fullName }} ({{ e.code }})
          </option>
        </select>
      </div>

      <div>
        <label class="block font-semibold text-slate-700 mb-1">{{ t('l_leave_type') }} *</label>
        <select
          v-model="requestForm.leaveType"
          class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500"
        >
          <option :value="LeaveType.AnnualLeave">{{ t('s_annual_leave') }}</option>
          <option :value="LeaveType.SickLeave">{{ t('s_sick_leave') }}</option>
          <option :value="LeaveType.SpecialLeave">{{ t('s_special_leave') }}</option>
          <option :value="LeaveType.MaternityLeave">{{ t('s_maternity_leave') }}</option>
          <option :value="LeaveType.UnpaidLeave">{{ t('s_unpaid_leave') }}</option>
        </select>
      </div>

      <div class="grid grid-cols-2 gap-3">
        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_leave_from_date') }} *</label>
          <input
            v-model="requestForm.fromDate"
            type="date"
            required
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500"
          />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_leave_to_date') }} *</label>
          <input
            v-model="requestForm.toDate"
            type="date"
            required
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500"
          />
        </div>
      </div>

      <div>
        <label class="block font-semibold text-slate-700 mb-1">{{ t('l_leave_reason') }} *</label>
        <textarea
          v-model="requestForm.reason"
          required
          rows="3"
          :placeholder="t('l_leave_reason_placeholder')"
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
        <span v-else>{{ t('b_confirm') }}</span>
      </button>
    </template>
  </BaseModal>
</template>
