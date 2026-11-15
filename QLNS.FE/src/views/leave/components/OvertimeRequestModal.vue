<script setup lang="ts">
import { computed, watch } from 'vue';
import { OvertimeType, OvertimeCompensationType, type CreateOvertimeRequestDto } from '@/types';
import { useCompanySettingsStore } from '@/stores/companySettings';
import { useAuthStore } from '@/stores/auth';
import { useI18n } from '@/locales';
import BaseModal from '@/components/BaseModal.vue';
import { Flame, Clock, Info, CalendarPlus, Award, DollarSign, ShieldCheck, Settings } from 'lucide-vue-next';

const props = defineProps<{
  modelValue: boolean;
  otForm: CreateOvertimeRequestDto;
  loading: boolean;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', val: boolean): void;
  (e: 'submit'): void;
}>();

const { t } = useI18n();
const settingsStore = useCompanySettingsStore();
const authStore = useAuthStore();

const parseTimeToMinutes = (val?: string | null): number | null => {
  if (!val) return null;
  const parts = val.split(':').map(Number);
  if (parts.length < 2 || Number.isNaN(parts[0]) || Number.isNaN(parts[1])) return null;
  return (parts[0] ?? 0) * 60 + (parts[1] ?? 0);
};

watch(
  () => props.otForm.date,
  (newDate) => {
    if (!newDate) return;
    const dayOfWeek = new Date(newDate).getDay();
    const isWeekendDay = dayOfWeek === 0 || (dayOfWeek === 6 && settingsStore.settings.excludeSaturday);
    if (isWeekendDay) {
      props.otForm.otType = OvertimeType.Weekend;
    } else if (props.otForm.otType === OvertimeType.Weekend) {
      props.otForm.otType = OvertimeType.Weekday;
    }
  }
);

watch(
  () => [props.otForm.startTime, props.otForm.endTime],
  ([startStr, endStr]) => {
    const startMin = parseTimeToMinutes(startStr);
    const endMin = parseTimeToMinutes(endStr);
    if (startMin !== null && endMin !== null && endMin > startMin) {
      let hours = (endMin - startMin) / 60;
      if (startMin <= 12 * 60 && endMin >= 13 * 60 && hours >= 6) {
        hours -= 1;
      }
      props.otForm.registeredHours = Math.round(hours * 2) / 2;
    }
  }
);

const applyPreset = (start: string, end: string, hours: number, isWeekend = false) => {
  props.otForm.startTime = start;
  props.otForm.endTime = end;
  props.otForm.registeredHours = hours;
  if (isWeekend) {
    props.otForm.otType = OvertimeType.Weekend;
  }
};

const currentMultiplier = computed(() => {
  if (props.otForm.otType === OvertimeType.Holiday || props.otForm.otType === 3) {
    return settingsStore.settings.holidayMultiplier;
  }
  if (props.otForm.otType === OvertimeType.Weekend || props.otForm.otType === 2) {
    return settingsStore.settings.weekendMultiplier;
  }
  return settingsStore.settings.weekdayMultiplier;
});

const estimatedCompDays = computed(() =>
  ((props.otForm.registeredHours || 0) / (settingsStore.settings.hoursPerCompDay || 8)).toFixed(2)
);

const estimatedPaidHours = computed(() =>
  ((props.otForm.registeredHours || 0) * currentMultiplier.value).toFixed(1)
);
</script>

<template>
  <BaseModal
    :model-value="modelValue"
    :title="t('l_ot_modal_title')"
    max-width="lg"
    @update:model-value="emit('update:modelValue', $event)"
  >
    <form @submit.prevent="emit('submit')" class="space-y-4">
      <!-- Info Banner -->
      <div class="p-3 rounded-xl bg-amber-50 border border-amber-200/80 flex items-start gap-2.5">
        <Info class="w-4 h-4 text-amber-600 shrink-0 mt-0.5" />
        <p class="text-xs text-amber-800 leading-relaxed">
          {{ t('l_ot_registered_hours_hint') }}
        </p>
      </div>

      <!-- Date & Day Type -->
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
        <div>
          <label class="block text-xs font-semibold text-slate-700 mb-1.5">
            {{ t('l_ot_date') }} <span class="text-rose-500">*</span>
          </label>
          <input
            v-model="otForm.date"
            type="date"
            required
            class="w-full px-3.5 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm text-slate-800 focus:outline-hidden focus:border-indigo-500"
          />
        </div>

        <div>
          <label class="block text-xs font-semibold text-slate-700 mb-1.5">
            {{ t('l_ot_type_label') }}
          </label>
          <select
            v-model="otForm.otType"
            class="w-full px-3.5 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm font-semibold text-slate-800 focus:outline-hidden focus:border-indigo-500"
          >
            <option :value="OvertimeType.Weekday">{{ t('l_ot_type_weekday') }}</option>
            <option :value="OvertimeType.Weekend">{{ t('l_ot_type_weekend') }}</option>
            <option :value="OvertimeType.Holiday">{{ t('l_ot_type_holiday') }}</option>
          </select>
        </div>
      </div>

      <!-- Quick Shift Presets -->
      <div>
        <span class="block text-[11px] font-semibold text-slate-500 mb-1.5">
          {{ t('l_ot_quick_presets') }}:
        </span>
        <div class="flex flex-wrap gap-1.5">
          <button
            type="button"
            @click="applyPreset('17:30', '19:30', 2)"
            class="px-2.5 py-1 rounded-lg text-xs font-medium bg-slate-100 hover:bg-indigo-50 hover:text-indigo-700 text-slate-700 border border-slate-200/80 transition-colors"
          >
            {{ t('b_preset_evening_2h') }}
          </button>
          <button
            type="button"
            @click="applyPreset('18:00', '21:00', 3)"
            class="px-2.5 py-1 rounded-lg text-xs font-medium bg-slate-100 hover:bg-indigo-50 hover:text-indigo-700 text-slate-700 border border-slate-200/80 transition-colors"
          >
            {{ t('b_preset_evening_3h') }}
          </button>
          <button
            type="button"
            @click="applyPreset('08:00', '12:00', 4, true)"
            class="px-2.5 py-1 rounded-lg text-xs font-medium bg-slate-100 hover:bg-indigo-50 hover:text-indigo-700 text-slate-700 border border-slate-200/80 transition-colors"
          >
            {{ t('b_preset_weekend_half') }}
          </button>
          <button
            type="button"
            @click="applyPreset('08:00', '17:00', 8, true)"
            class="px-2.5 py-1 rounded-lg text-xs font-medium bg-slate-100 hover:bg-indigo-50 hover:text-indigo-700 text-slate-700 border border-slate-200/80 transition-colors"
          >
            {{ t('b_preset_weekend_full') }}
          </button>
        </div>
      </div>

      <!-- Start Time, End Time & Registered Hours -->
      <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
        <div>
          <label class="block text-xs font-semibold text-slate-700 mb-1.5">
            {{ t('l_ot_start_time') }}
          </label>
          <input
            v-model="otForm.startTime"
            type="time"
            class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm font-mono text-slate-800 focus:outline-hidden focus:border-indigo-500"
          />
        </div>

        <div>
          <label class="block text-xs font-semibold text-slate-700 mb-1.5">
            {{ t('l_ot_end_time') }}
          </label>
          <input
            v-model="otForm.endTime"
            type="time"
            class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm font-mono text-slate-800 focus:outline-hidden focus:border-indigo-500"
          />
        </div>

        <div>
          <label class="block text-xs font-semibold text-slate-700 mb-1.5">
            {{ t('l_ot_registered_hours') }} <span class="text-rose-500">*</span>
          </label>
          <div class="relative">
            <Clock class="w-4 h-4 text-slate-400 absolute left-3 top-1/2 -translate-y-1/2" />
            <input
              v-model.number="otForm.registeredHours"
              type="number"
              step="0.5"
              min="0.5"
              max="16"
              required
              class="w-full pl-9 pr-12 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm font-mono font-bold text-indigo-700 focus:outline-hidden focus:border-indigo-500"
            />
            <span class="absolute right-3 top-1/2 -translate-y-1/2 text-xs font-semibold text-slate-400">
              {{ t('l_hours_unit') }}
            </span>
          </div>
        </div>
      </div>

      <!-- Company Policy Mode (Locked vs Employee-Selectable) -->
      <div>
        <div class="flex items-center justify-between mb-1.5">
          <label class="block text-xs font-semibold text-slate-700">
            {{ t('l_ot_compensation_label') }}
          </label>
          <router-link
            v-if="authStore.hasAnyRole(['Admin', 'HRManager'])"
            to="/settings"
            @click="emit('update:modelValue', false)"
            class="inline-flex items-center gap-1 text-[11px] font-semibold text-indigo-600 hover:text-indigo-800"
          >
            <Settings class="w-3 h-3" />
            {{ t('l_policy_configure_link') }}
          </router-link>
        </div>

        <!-- Locked by Company Policy -->
        <div
          v-if="!settingsStore.settings.allowEmployeeChooseOtMode"
          class="p-3 rounded-xl bg-indigo-50/60 border border-indigo-200/80 flex items-center justify-between gap-2"
        >
          <div class="flex items-center gap-2 text-xs">
            <ShieldCheck class="w-4 h-4 text-indigo-600 shrink-0" />
            <span class="text-slate-600">{{ t('l_policy_locked_by_company') }}:</span>
            <strong
              v-if="otForm.compensationType === OvertimeCompensationType.CompensatoryLeave"
              class="text-emerald-700"
            >
              {{ t('l_ot_comp_leave') }}
            </strong>
            <strong
              v-else-if="otForm.compensationType === OvertimeCompensationType.RecognitionOnly"
              class="text-indigo-700"
            >
              {{ t('l_ot_comp_recognition') }}
            </strong>
            <strong v-else class="text-amber-700">
              {{ t('l_ot_comp_paid') }}
            </strong>
          </div>
        </div>

        <!-- Employee-Selectable Cards (when enabled in Company Settings) -->
        <div v-else class="grid grid-cols-1 sm:grid-cols-3 gap-2.5">
          <button
            type="button"
            @click="otForm.compensationType = OvertimeCompensationType.CompensatoryLeave"
            :class="[
              'p-3 rounded-xl border text-left transition-all flex flex-col justify-between gap-1.5',
              otForm.compensationType === OvertimeCompensationType.CompensatoryLeave
                ? 'bg-emerald-50/70 border-emerald-500 ring-2 ring-emerald-500/20'
                : 'bg-slate-50/70 border-slate-200 hover:bg-slate-100',
            ]"
          >
            <div class="flex items-center gap-1.5 font-bold text-xs text-emerald-800">
              <CalendarPlus class="w-4 h-4 text-emerald-600 shrink-0" />
              <span>{{ t('l_ot_comp_leave') }}</span>
            </div>
            <p class="text-[11px] text-slate-500 leading-snug">{{ t('l_ot_comp_leave_desc') }}</p>
          </button>

          <button
            type="button"
            @click="otForm.compensationType = OvertimeCompensationType.RecognitionOnly"
            :class="[
              'p-3 rounded-xl border text-left transition-all flex flex-col justify-between gap-1.5',
              otForm.compensationType === OvertimeCompensationType.RecognitionOnly
                ? 'bg-indigo-50/70 border-indigo-500 ring-2 ring-indigo-500/20'
                : 'bg-slate-50/70 border-slate-200 hover:bg-slate-100',
            ]"
          >
            <div class="flex items-center gap-1.5 font-bold text-xs text-indigo-800">
              <Award class="w-4 h-4 text-indigo-600 shrink-0" />
              <span>{{ t('l_ot_comp_recognition') }}</span>
            </div>
            <p class="text-[11px] text-slate-500 leading-snug">{{ t('l_ot_comp_recognition_desc') }}</p>
          </button>

          <button
            type="button"
            @click="otForm.compensationType = OvertimeCompensationType.Paid"
            :class="[
              'p-3 rounded-xl border text-left transition-all flex flex-col justify-between gap-1.5',
              otForm.compensationType === OvertimeCompensationType.Paid
                ? 'bg-amber-50/70 border-amber-500 ring-2 ring-amber-500/20'
                : 'bg-slate-50/70 border-slate-200 hover:bg-slate-100',
            ]"
          >
            <div class="flex items-center gap-1.5 font-bold text-xs text-amber-800">
              <DollarSign class="w-4 h-4 text-amber-600 shrink-0" />
              <span>{{ t('l_ot_comp_paid') }}</span>
            </div>
            <p class="text-[11px] text-slate-500 leading-snug">{{ t('l_ot_comp_paid_desc') }}</p>
          </button>
        </div>
      </div>

      <!-- Benefit Preview -->
      <div class="p-3 rounded-xl bg-slate-50 border border-slate-200/80 flex items-center justify-between text-xs">
        <span class="font-semibold text-slate-600">{{ t('l_ot_benefit_preview') }}:</span>
        <span
          v-if="otForm.compensationType === OvertimeCompensationType.CompensatoryLeave"
          class="font-bold text-emerald-700"
        >
          +{{ estimatedCompDays }} {{ t('l_ot_benefit_comp_days') }}
        </span>
        <span
          v-else-if="otForm.compensationType === OvertimeCompensationType.RecognitionOnly"
          class="font-bold text-indigo-700"
        >
          {{ t('l_ot_benefit_recognition') }}
        </span>
        <span v-else class="font-bold text-amber-700">
          +{{ estimatedPaidHours }}h {{ t('l_ot_benefit_paid_equiv') }} ({{ currentMultiplier }}x)
        </span>
      </div>

      <!-- Reason -->
      <div>
        <label class="block text-xs font-semibold text-slate-700 mb-1.5">
          {{ t('l_ot_reason') }} <span class="text-rose-500">*</span>
        </label>
        <textarea
          v-model="otForm.reason"
          rows="2"
          required
          :placeholder="t('l_ot_reason_placeholder')"
          class="w-full px-3.5 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm text-slate-800 placeholder-slate-400 focus:outline-hidden focus:border-indigo-500"
        />
      </div>

      <!-- Actions -->
      <div class="flex items-center justify-end gap-2.5 pt-2 border-t border-slate-100">
        <button
          type="button"
          @click="emit('update:modelValue', false)"
          class="px-4 py-2 bg-slate-100 hover:bg-slate-200 text-slate-700 rounded-xl text-xs sm:text-sm font-semibold transition-colors"
        >
          {{ t('b_cancel') }}
        </button>
        <button
          type="submit"
          :disabled="loading"
          class="inline-flex items-center gap-2 px-4 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-xs sm:text-sm font-semibold shadow-md shadow-indigo-600/20 transition-colors disabled:opacity-50"
        >
          <Flame class="w-4 h-4" />
          {{ loading ? t('b_saving') : t('b_submit_ot_request') }}
        </button>
      </div>
    </form>
  </BaseModal>
</template>
