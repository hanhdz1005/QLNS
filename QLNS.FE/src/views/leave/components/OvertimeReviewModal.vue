<script setup lang="ts">
import { reactive, watch } from 'vue';
import {
  OvertimeType,
  OvertimeCompensationType,
  type OvertimeRequestDto,
  type ReviewOvertimeRequestDto,
} from '@/types';
import { useI18n } from '@/locales';
import { useCompanySettingsStore } from '@/stores/companySettings';
import { formatDate, formatTime } from '@/utils/formatters';
import BaseModal from '@/components/BaseModal.vue';
import { CheckCircle2, XCircle, ScanFace, Clock } from 'lucide-vue-next';

const props = defineProps<{
  modelValue: boolean;
  request: OvertimeRequestDto | null;
  loading: boolean;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', val: boolean): void;
  (e: 'approve', payload: ReviewOvertimeRequestDto): void;
  (e: 'reject', payload: ReviewOvertimeRequestDto): void;
}>();

const { t } = useI18n();
const settingsStore = useCompanySettingsStore();

const reviewForm = reactive<ReviewOvertimeRequestDto>({
  approvedHours: 2,
  compensationType: OvertimeCompensationType.CompensatoryLeave,
  otType: OvertimeType.Weekday,
  approverNote: '',
});

const normalizeCompType = (val?: OvertimeCompensationType | number | string) => {
  if (val === OvertimeCompensationType.RecognitionOnly || val === 2 || val === '2') {
    return OvertimeCompensationType.RecognitionOnly;
  }
  if (val === OvertimeCompensationType.Paid || val === 3 || val === '3') {
    return OvertimeCompensationType.Paid;
  }
  return OvertimeCompensationType.CompensatoryLeave;
};

const normalizeOtType = (val?: OvertimeType | number | string) => {
  if (val === OvertimeType.Weekend || val === 2 || val === '2') return OvertimeType.Weekend;
  if (val === OvertimeType.Holiday || val === 3 || val === '3') return OvertimeType.Holiday;
  return OvertimeType.Weekday;
};

watch(
  () => props.request,
  (req) => {
    if (!req) return;
    reviewForm.approvedHours = req.registeredHours;
    reviewForm.compensationType = normalizeCompType(req.compensationType);
    reviewForm.otType = normalizeOtType(req.otType);
    reviewForm.approverNote = req.approverNote || '';
  },
  { immediate: true }
);
</script>

<template>
  <BaseModal
    :model-value="modelValue"
    :title="t('l_ot_review_modal_title')"
    max-width="lg"
    @update:model-value="emit('update:modelValue', $event)"
  >
    <div v-if="request" class="space-y-4 text-xs sm:text-sm">
      <!-- Employee & Request Summary -->
      <div class="p-3.5 rounded-xl bg-slate-50 border border-slate-200/80 space-y-2">
        <div class="flex items-center justify-between">
          <div>
            <span class="font-bold text-slate-800">{{ request.employeeName }}</span>
            <span class="ml-2 text-xs font-mono text-slate-500">{{ request.employeeCode }}</span>
          </div>
          <span class="text-xs font-semibold text-indigo-700 bg-indigo-50 px-2.5 py-1 rounded-lg">
            {{ formatDate(request.date) }}
          </span>
        </div>

        <div class="text-xs text-slate-600 flex flex-wrap items-center gap-3">
          <span v-if="request.startTime && request.endTime" class="font-mono font-semibold text-slate-700">
            {{ formatTime(request.startTime) }} &rarr; {{ formatTime(request.endTime) }}
          </span>
          <span>{{ t('l_ot_reason') }}: <strong class="text-slate-800">{{ request.reason }}</strong></span>
        </div>

        <!-- Actual Face Attendance Verification -->
        <div class="pt-2 border-t border-slate-200/70 flex items-center justify-between">
          <span class="text-xs font-semibold text-slate-600 flex items-center gap-1.5">
            <ScanFace class="w-4 h-4 text-indigo-600" />
            {{ t('l_ot_actual_attendance_col') }}:
          </span>
          <span
            v-if="request.actualCheckIn || request.actualCheckOut"
            class="px-2.5 py-1 rounded-lg bg-emerald-50 text-emerald-700 border border-emerald-200 font-mono text-xs font-bold"
          >
            {{ formatTime(request.actualCheckIn) }} &rarr; {{ formatTime(request.actualCheckOut) }}
          </span>
          <span
            v-else
            class="px-2.5 py-1 rounded-lg bg-amber-50 text-amber-700 border border-amber-200 text-xs font-medium"
          >
            {{ t('l_ot_no_attendance_yet') }}
          </span>
        </div>
      </div>

      <!-- Adjust Approved Hours & Day Type -->
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
        <div>
          <label class="block text-xs font-semibold text-slate-700 mb-1.5">
            {{ t('l_ot_approved_hours_input') }}
          </label>
          <div class="relative">
            <Clock class="w-4 h-4 text-slate-400 absolute left-3 top-1/2 -translate-y-1/2" />
            <input
              v-model.number="reviewForm.approvedHours"
              type="number"
              step="0.5"
              min="0.5"
              max="16"
              class="w-full pl-9 pr-12 py-2 bg-slate-50 border border-slate-200 rounded-xl font-mono font-bold text-indigo-700"
            />
            <span class="absolute right-3 top-1/2 -translate-y-1/2 text-xs text-slate-400">
              {{ t('l_hours_unit') }}
            </span>
          </div>
        </div>

        <div>
          <label class="block text-xs font-semibold text-slate-700 mb-1.5">
            {{ t('l_ot_type_label') }}
          </label>
          <select
            v-model="reviewForm.otType"
            class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl font-semibold text-slate-800"
          >
            <option :value="OvertimeType.Weekday">{{ t('l_ot_type_weekday') }}</option>
            <option :value="OvertimeType.Weekend">{{ t('l_ot_type_weekend') }}</option>
            <option :value="OvertimeType.Holiday">{{ t('l_ot_type_holiday') }}</option>
          </select>
        </div>
      </div>

      <!-- Finalize Compensation / Recognition Mode -->
      <div>
        <label class="block text-xs font-semibold text-slate-700 mb-1.5">
          {{ t('l_ot_compensation_label') }}
        </label>
        <select
          v-model="reviewForm.compensationType"
          :disabled="!settingsStore.settings.allowApproverOverrideOtMode"
          class="w-full px-3.5 py-2.5 bg-slate-50 border border-slate-200 rounded-xl font-semibold text-slate-800 disabled:opacity-70"
        >
          <option :value="OvertimeCompensationType.CompensatoryLeave">{{ t('l_ot_comp_leave') }}</option>
          <option :value="OvertimeCompensationType.RecognitionOnly">{{ t('l_ot_comp_recognition') }}</option>
          <option :value="OvertimeCompensationType.Paid">{{ t('l_ot_comp_paid') }}</option>
        </select>
      </div>

      <!-- Approver Note -->
      <div>
        <label class="block text-xs font-semibold text-slate-700 mb-1.5">
          {{ t('l_ot_approver_note') }}
        </label>
        <textarea
          v-model="reviewForm.approverNote"
          rows="2"
          :placeholder="t('l_ot_approver_note_placeholder')"
          class="w-full px-3.5 py-2 bg-slate-50 border border-slate-200 rounded-xl text-slate-800 placeholder-slate-400"
        />
      </div>

      <!-- Actions -->
      <div class="flex flex-wrap items-center justify-end gap-2 pt-3 border-t border-slate-100">
        <button
          type="button"
          @click="emit('update:modelValue', false)"
          class="px-3.5 py-2 bg-slate-100 hover:bg-slate-200 text-slate-700 rounded-xl text-xs font-semibold transition-colors"
        >
          {{ t('b_cancel') }}
        </button>
        <button
          type="button"
          :disabled="loading"
          @click="emit('reject', reviewForm)"
          class="inline-flex items-center gap-1.5 px-3.5 py-2 bg-rose-50 hover:bg-rose-100 text-rose-700 border border-rose-200 rounded-xl text-xs font-semibold transition-colors disabled:opacity-50"
        >
          <XCircle class="w-4 h-4" />
          {{ t('b_reject_ot') }}
        </button>
        <button
          type="button"
          :disabled="loading"
          @click="emit('approve', reviewForm)"
          class="inline-flex items-center gap-1.5 px-4 py-2 bg-emerald-600 hover:bg-emerald-700 text-white rounded-xl text-xs font-semibold shadow-xs transition-colors disabled:opacity-50"
        >
          <CheckCircle2 class="w-4 h-4" />
          {{ t('b_approve_ot') }}
        </button>
      </div>
    </div>
  </BaseModal>
</template>
