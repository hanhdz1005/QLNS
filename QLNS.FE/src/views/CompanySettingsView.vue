<script setup lang="ts">
import { ref, reactive, computed } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useToastStore } from '@/stores/toast';
import {
  useCompanySettingsStore,
  type CompanyPolicySettings,
} from '@/stores/companySettings';
import { OvertimeCompensationType } from '@/types';
import { useI18n } from '@/locales';
import {
  Settings,
  Flame,
  Clock,
  DollarSign,
  CheckCircle2,
  RotateCcw,
  Save,
  CalendarPlus,
  Award,
  ShieldCheck,
} from 'lucide-vue-next';

const authStore = useAuthStore();
const toastStore = useToastStore();
const settingsStore = useCompanySettingsStore();
const { t } = useI18n();

const canEditPolicy = computed(() =>
  authStore.hasAnyRole(['Admin', 'HRManager'])
);

const activeTab = ref<'ot' | 'attendance' | 'payroll'>('ot');

const form = reactive<CompanyPolicySettings>({
  ...settingsStore.settings,
});

const handleSavePolicy = () => {
  if (!canEditPolicy.value) return;
  settingsStore.updateSettings({ ...form });
  toastStore.success(t('t_settings_saved'));
};

const handleResetDefaults = () => {
  if (!canEditPolicy.value) return;
  settingsStore.resetDefaults();
  Object.assign(form, settingsStore.settings);
  toastStore.info(t('t_settings_reset'));
};
</script>

<template>
  <div class="space-y-4">
    <!-- Top Header -->
    <div class="bg-white/95 backdrop-blur-xs rounded-2xl border border-slate-200/80 p-4 sm:p-5 shadow-2xs flex flex-col sm:flex-row sm:items-center justify-between gap-3">
      <div class="flex items-start gap-3">
        <div class="w-10 h-10 rounded-xl bg-indigo-600 text-white flex items-center justify-center shrink-0 shadow-sm shadow-indigo-500/25">
          <Settings class="w-5 h-5" />
        </div>
        <div>
          <h1 class="text-base sm:text-lg font-bold text-slate-900">
            {{ t('l_settings_page_title') }}
          </h1>
          <p class="text-xs text-slate-500 mt-0.5">
            {{ t('l_settings_page_subtitle') }}
          </p>
        </div>
      </div>

      <div v-if="canEditPolicy" class="flex items-center gap-2 shrink-0">
        <button
          type="button"
          @click="handleResetDefaults"
          class="inline-flex items-center gap-1.5 px-3 py-2 rounded-xl text-xs font-semibold text-slate-700 bg-slate-100 hover:bg-slate-200 transition-colors"
        >
          <RotateCcw class="w-3.5 h-3.5" />
          {{ t('b_reset_default_settings') }}
        </button>
        <button
          type="button"
          @click="handleSavePolicy"
          class="inline-flex items-center gap-1.5 px-4 py-2 rounded-xl text-xs font-semibold text-white bg-indigo-600 hover:bg-indigo-700 shadow-sm shadow-indigo-600/20 transition-colors"
        >
          <Save class="w-3.5 h-3.5" />
          {{ t('b_save_settings') }}
        </button>
      </div>
    </div>

    <!-- Read-only notice for non-admin/HR -->
    <div
      v-if="!canEditPolicy"
      class="p-3.5 rounded-xl bg-amber-50 border border-amber-200/80 flex items-center gap-2.5 text-xs text-amber-800"
    >
      <ShieldCheck class="w-4 h-4 text-amber-600 shrink-0" />
      <span>{{ t('l_readonly_policy_notice') }}</span>
    </div>

    <!-- Navigation Tabs -->
    <div class="flex flex-wrap items-center gap-2 bg-white/90 p-1.5 rounded-2xl border border-slate-200/80 shadow-2xs w-fit">
      <button
        type="button"
        @click="activeTab = 'ot'"
        :class="[
          'inline-flex items-center gap-2 px-3.5 py-2 rounded-xl text-xs font-bold transition-all',
          activeTab === 'ot'
            ? 'bg-indigo-600 text-white shadow-xs'
            : 'text-slate-600 hover:bg-slate-100',
        ]"
      >
        <Flame class="w-4 h-4" />
        {{ t('l_settings_tab_ot') }}
      </button>

      <button
        type="button"
        @click="activeTab = 'attendance'"
        :class="[
          'inline-flex items-center gap-2 px-3.5 py-2 rounded-xl text-xs font-bold transition-all',
          activeTab === 'attendance'
            ? 'bg-indigo-600 text-white shadow-xs'
            : 'text-slate-600 hover:bg-slate-100',
        ]"
      >
        <Clock class="w-4 h-4" />
        {{ t('l_settings_tab_attendance') }}
      </button>

      <button
        type="button"
        @click="activeTab = 'payroll'"
        :class="[
          'inline-flex items-center gap-2 px-3.5 py-2 rounded-xl text-xs font-bold transition-all',
          activeTab === 'payroll'
            ? 'bg-indigo-600 text-white shadow-xs'
            : 'text-slate-600 hover:bg-slate-100',
        ]"
      >
        <DollarSign class="w-4 h-4" />
        {{ t('l_settings_tab_payroll') }}
      </button>
    </div>

    <!-- TAB 1: OVERTIME (OT) POLICY -->
    <div v-if="activeTab === 'ot'" class="bg-white rounded-2xl border border-slate-200/80 p-5 shadow-2xs space-y-5">
      <div>
        <h2 class="text-sm sm:text-base font-bold text-slate-900">{{ t('l_policy_ot_section_title') }}</h2>
        <p class="text-xs text-slate-500 mt-0.5">{{ t('l_policy_ot_section_desc') }}</p>
      </div>

      <!-- Default OT Mode Cards -->
      <div>
        <label class="block text-xs font-bold text-slate-700 mb-2">
          {{ t('l_policy_default_ot_mode') }}
        </label>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-3">
          <!-- Mode 1: Compensatory Leave -->
          <button
            type="button"
            :disabled="!canEditPolicy"
            @click="form.defaultOtMode = OvertimeCompensationType.CompensatoryLeave"
            :class="[
              'p-4 rounded-xl border text-left transition-all flex flex-col justify-between gap-2 disabled:opacity-75',
              form.defaultOtMode === OvertimeCompensationType.CompensatoryLeave
                ? 'bg-emerald-50/80 border-emerald-500 ring-2 ring-emerald-500/20'
                : 'bg-slate-50/60 border-slate-200 hover:bg-slate-100',
            ]"
          >
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-2 font-bold text-xs sm:text-sm text-emerald-800">
                <CalendarPlus class="w-4 h-4 text-emerald-600" />
                <span>{{ t('l_ot_comp_leave') }}</span>
              </div>
              <CheckCircle2
                v-if="form.defaultOtMode === OvertimeCompensationType.CompensatoryLeave"
                class="w-4 h-4 text-emerald-600"
              />
            </div>
            <p class="text-xs text-slate-600 leading-relaxed">{{ t('l_ot_comp_leave_desc') }}</p>
          </button>

          <!-- Mode 2: Recognition Only -->
          <button
            type="button"
            :disabled="!canEditPolicy"
            @click="form.defaultOtMode = OvertimeCompensationType.RecognitionOnly"
            :class="[
              'p-4 rounded-xl border text-left transition-all flex flex-col justify-between gap-2 disabled:opacity-75',
              form.defaultOtMode === OvertimeCompensationType.RecognitionOnly
                ? 'bg-indigo-50/80 border-indigo-500 ring-2 ring-indigo-500/20'
                : 'bg-slate-50/60 border-slate-200 hover:bg-slate-100',
            ]"
          >
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-2 font-bold text-xs sm:text-sm text-indigo-800">
                <Award class="w-4 h-4 text-indigo-600" />
                <span>{{ t('l_ot_comp_recognition') }}</span>
              </div>
              <CheckCircle2
                v-if="form.defaultOtMode === OvertimeCompensationType.RecognitionOnly"
                class="w-4 h-4 text-indigo-600"
              />
            </div>
            <p class="text-xs text-slate-600 leading-relaxed">{{ t('l_ot_comp_recognition_desc') }}</p>
          </button>

          <!-- Mode 3: Paid OT -->
          <button
            type="button"
            :disabled="!canEditPolicy"
            @click="form.defaultOtMode = OvertimeCompensationType.Paid"
            :class="[
              'p-4 rounded-xl border text-left transition-all flex flex-col justify-between gap-2 disabled:opacity-75',
              form.defaultOtMode === OvertimeCompensationType.Paid
                ? 'bg-amber-50/80 border-amber-500 ring-2 ring-amber-500/20'
                : 'bg-slate-50/60 border-slate-200 hover:bg-slate-100',
            ]"
          >
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-2 font-bold text-xs sm:text-sm text-amber-800">
                <DollarSign class="w-4 h-4 text-amber-600" />
                <span>{{ t('l_ot_comp_paid') }}</span>
              </div>
              <CheckCircle2
                v-if="form.defaultOtMode === OvertimeCompensationType.Paid"
                class="w-4 h-4 text-amber-600"
              />
            </div>
            <p class="text-xs text-slate-600 leading-relaxed">{{ t('l_ot_comp_paid_desc') }}</p>
          </button>
        </div>
      </div>

      <!-- Policy Lock Toggles -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4 pt-2 border-t border-slate-100">
        <label class="flex items-start gap-3 p-3.5 rounded-xl bg-slate-50 border border-slate-200/80 cursor-pointer">
          <input
            v-model="form.allowEmployeeChooseOtMode"
            :disabled="!canEditPolicy"
            type="checkbox"
            class="mt-0.5 w-4 h-4 rounded-sm text-indigo-600 focus:ring-indigo-500"
          />
          <div>
            <span class="block text-xs font-bold text-slate-800">
              {{ t('l_policy_allow_employee_ot_choice') }}
            </span>
            <span class="block text-[11px] text-slate-500 mt-0.5 leading-relaxed">
              {{ t('l_policy_allow_employee_ot_choice_desc') }}
            </span>
          </div>
        </label>

        <label class="flex items-start gap-3 p-3.5 rounded-xl bg-slate-50 border border-slate-200/80 cursor-pointer">
          <input
            v-model="form.allowApproverOverrideOtMode"
            :disabled="!canEditPolicy"
            type="checkbox"
            class="mt-0.5 w-4 h-4 rounded-sm text-indigo-600 focus:ring-indigo-500"
          />
          <div>
            <span class="block text-xs font-bold text-slate-800">
              {{ t('l_policy_allow_approver_ot_override') }}
            </span>
            <span class="block text-[11px] text-slate-500 mt-0.5 leading-relaxed">
              {{ t('l_policy_allow_approver_ot_override_desc') }}
            </span>
          </div>
        </label>
      </div>

      <!-- Numeric Rules & Multipliers -->
      <div class="grid grid-cols-2 sm:grid-cols-5 gap-3 pt-2 border-t border-slate-100">
        <div>
          <label class="block text-[11px] font-semibold text-slate-600 mb-1">
            {{ t('l_policy_max_monthly_ot') }}
          </label>
          <input
            v-model.number="form.maxMonthlyOtHours"
            :disabled="!canEditPolicy"
            type="number"
            min="10"
            max="100"
            class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs font-mono font-bold text-slate-800"
          />
        </div>
        <div>
          <label class="block text-[11px] font-semibold text-slate-600 mb-1">
            {{ t('l_policy_hours_per_comp_day') }}
          </label>
          <input
            v-model.number="form.hoursPerCompDay"
            :disabled="!canEditPolicy"
            type="number"
            min="4"
            max="12"
            class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs font-mono font-bold text-emerald-700"
          />
        </div>
        <div>
          <label class="block text-[11px] font-semibold text-slate-600 mb-1">
            {{ t('l_policy_weekday_multiplier') }}
          </label>
          <input
            v-model.number="form.weekdayMultiplier"
            :disabled="!canEditPolicy"
            type="number"
            step="0.1"
            min="1"
            class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs font-mono font-bold text-indigo-700"
          />
        </div>
        <div>
          <label class="block text-[11px] font-semibold text-slate-600 mb-1">
            {{ t('l_policy_weekend_multiplier') }}
          </label>
          <input
            v-model.number="form.weekendMultiplier"
            :disabled="!canEditPolicy"
            type="number"
            step="0.1"
            min="1"
            class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs font-mono font-bold text-amber-700"
          />
        </div>
        <div>
          <label class="block text-[11px] font-semibold text-slate-600 mb-1">
            {{ t('l_policy_holiday_multiplier') }}
          </label>
          <input
            v-model.number="form.holidayMultiplier"
            :disabled="!canEditPolicy"
            type="number"
            step="0.1"
            min="1"
            class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs font-mono font-bold text-rose-700"
          />
        </div>
      </div>
    </div>

    <!-- TAB 2: WORKING HOURS & ATTENDANCE -->
    <div v-else-if="activeTab === 'attendance'" class="bg-white rounded-2xl border border-slate-200/80 p-5 shadow-2xs space-y-4">
      <h2 class="text-sm sm:text-base font-bold text-slate-900">{{ t('l_policy_attendance_section_title') }}</h2>

      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <div>
          <label class="block text-xs font-semibold text-slate-700 mb-1.5">{{ t('l_policy_morning_shift') }}</label>
          <div class="flex items-center gap-2">
            <input v-model="form.morningStart" :disabled="!canEditPolicy" type="time" class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs font-mono" />
            <span class="text-slate-400">-</span>
            <input v-model="form.morningEnd" :disabled="!canEditPolicy" type="time" class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs font-mono" />
          </div>
        </div>

        <div>
          <label class="block text-xs font-semibold text-slate-700 mb-1.5">{{ t('l_policy_afternoon_shift') }}</label>
          <div class="flex items-center gap-2">
            <input v-model="form.afternoonStart" :disabled="!canEditPolicy" type="time" class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs font-mono" />
            <span class="text-slate-400">-</span>
            <input v-model="form.afternoonEnd" :disabled="!canEditPolicy" type="time" class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs font-mono" />
          </div>
        </div>

        <div>
          <label class="block text-xs font-semibold text-slate-700 mb-1.5">{{ t('l_policy_grace_period') }}</label>
          <input v-model="form.gracePeriodTime" :disabled="!canEditPolicy" type="time" class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs font-mono font-bold text-amber-700" />
        </div>

        <div>
          <label class="block text-xs font-semibold text-slate-700 mb-1.5">{{ t('l_policy_standard_working_days') }}</label>
          <input v-model.number="form.standardWorkingDays" :disabled="!canEditPolicy" type="number" min="20" max="26" class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs font-mono font-bold text-indigo-700" />
        </div>
      </div>

      <label class="flex items-center gap-3 p-3.5 rounded-xl bg-slate-50 border border-slate-200/80 cursor-pointer">
        <input v-model="form.excludeSaturday" :disabled="!canEditPolicy" type="checkbox" class="w-4 h-4 rounded-sm text-indigo-600" />
        <span class="text-xs font-semibold text-slate-800">{{ t('l_policy_exclude_saturday') }}</span>
      </label>
    </div>

    <!-- TAB 4: PAYROLL & LEAVE POLICY -->
    <div v-else class="bg-white rounded-2xl border border-slate-200/80 p-5 shadow-2xs space-y-4">
      <h2 class="text-sm sm:text-base font-bold text-slate-900">{{ t('l_policy_payroll_section_title') }}</h2>

      <div class="max-w-xs">
        <label class="block text-xs font-semibold text-slate-700 mb-1.5">{{ t('l_policy_default_annual_leave') }}</label>
        <input v-model.number="form.defaultAnnualLeaveDays" :disabled="!canEditPolicy" type="number" min="12" max="30" class="w-full px-3.5 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs font-mono font-bold text-indigo-700" />
      </div>

      <div class="grid grid-cols-1 md:grid-cols-3 gap-3 pt-2">
        <label class="flex items-center gap-3 p-3.5 rounded-xl bg-slate-50 border border-slate-200/80 cursor-pointer">
          <input v-model="form.includeOvertimePayInPayroll" :disabled="!canEditPolicy" type="checkbox" class="w-4 h-4 rounded-sm text-indigo-600" />
          <span class="text-xs font-semibold text-slate-800">{{ t('l_policy_payroll_include_ot') }}</span>
        </label>

        <label class="flex items-center gap-3 p-3.5 rounded-xl bg-slate-50 border border-slate-200/80 cursor-pointer">
          <input v-model="form.includeInsuranceByDefault" :disabled="!canEditPolicy" type="checkbox" class="w-4 h-4 rounded-sm text-indigo-600" />
          <span class="text-xs font-semibold text-slate-800">{{ t('l_policy_payroll_include_insurance') }}</span>
        </label>

        <label class="flex items-center gap-3 p-3.5 rounded-xl bg-slate-50 border border-slate-200/80 cursor-pointer">
          <input v-model="form.includeTaxByDefault" :disabled="!canEditPolicy" type="checkbox" class="w-4 h-4 rounded-sm text-indigo-600" />
          <span class="text-xs font-semibold text-slate-800">{{ t('l_policy_payroll_include_tax') }}</span>
        </label>
      </div>
    </div>
  </div>
</template>
