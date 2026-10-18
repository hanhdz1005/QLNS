<script setup lang="ts">
import type { AddAttendanceDto, EmployeeDto } from '@/types';
import { AttendanceStatus } from '@/types';
import { useI18n } from '@/locales';
import BaseModal from '@/components/BaseModal.vue';

defineProps<{
  modelValue: boolean;
  isEditing: boolean;
  formData: AddAttendanceDto;
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
    :title="isEditing ? t('l_edit_attendance_modal_title') : t('l_add_attendance_modal_title')"
  >
    <form @submit.prevent="emit('submit')" class="space-y-4 text-xs">
      <div>
        <label class="block font-semibold text-slate-700 mb-1">{{ t('l_full_name') }} *</label>
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
        <label class="block font-semibold text-slate-700 mb-1">{{ t('l_attendance_date') }} *</label>
        <input
          v-model="formData.date"
          type="date"
          required
          class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500"
        />
      </div>

      <div class="grid grid-cols-2 gap-3">
        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_check_in_time') }}</label>
          <input
            v-model="formData.checkIn"
            type="time"
            step="1"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500"
          />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_check_out_time') }}</label>
          <input
            v-model="formData.checkOut"
            type="time"
            step="1"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500"
          />
        </div>
      </div>

      <div>
        <label class="block font-semibold text-slate-700 mb-1">{{ t('l_status') }}</label>
        <select
          v-model="formData.status"
          class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500"
        >
          <option :value="AttendanceStatus.Present">{{ t('s_present') }}</option>
          <option :value="AttendanceStatus.Late">{{ t('s_late') }}</option>
          <option :value="AttendanceStatus.EarlyLeave">{{ t('s_early_leave') }}</option>
          <option :value="AttendanceStatus.WorkFromHome">{{ t('s_work_from_home') }}</option>
          <option :value="AttendanceStatus.Leave">{{ t('s_leave') }}</option>
          <option :value="AttendanceStatus.Absent">{{ t('s_absent') }}</option>
        </select>
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
