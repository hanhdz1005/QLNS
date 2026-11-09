<script setup lang="ts">
import type { EmployeeLifecycleDetailDto } from '@/types';
import { formatDate, formatCurrency } from '@/utils/formatters';
import { useI18n } from '@/locales';
import {
  Calendar,
  TrendingUp,
  Clock,
  CheckCircle2,
  Award,
  FileText,
  Sparkles,
  Building2,
  Briefcase,
} from 'lucide-vue-next';

defineProps<{
  detail: EmployeeLifecycleDetailDto;
  month: number;
  year: number;
}>();

const emit = defineEmits<{
  (e: 'update:month', val: number): void;
  (e: 'update:year', val: number): void;
  (e: 'reload'): void;
}>();

const { t } = useI18n();

const getStageBadge = (stage: string) => {
  switch (stage) {
    case 'Internship':
      return { label: t('s_stage_internship'), cls: 'bg-cyan-500/20 text-cyan-200 border-cyan-400/40' };
    case 'Probation':
      return { label: t('s_stage_probation'), cls: 'bg-amber-500/20 text-amber-200 border-amber-400/40' };
    case 'Resigned':
      return { label: t('s_stage_resigned'), cls: 'bg-rose-500/20 text-rose-200 border-rose-400/40' };
    default:
      return { label: t('s_stage_official'), cls: 'bg-emerald-500/20 text-emerald-200 border-emerald-400/40' };
  }
};
</script>

<template>
  <div class="space-y-4">
    <!-- Employee Profile Banner + Month/Year Period Selector -->
    <div class="bg-gradient-to-r from-slate-900 via-indigo-950 to-slate-900 rounded-2xl p-5 text-white shadow-lg border border-slate-800 flex flex-col lg:flex-row lg:items-center justify-between gap-4">
      <div class="flex items-center gap-4">
        <div class="w-16 h-16 rounded-2xl bg-indigo-500/30 border-2 border-indigo-400/40 flex items-center justify-center text-2xl font-extrabold text-white shrink-0 overflow-hidden">
          <img
            v-if="detail.faceImageUrl"
            :src="detail.faceImageUrl"
            :alt="detail.fullName"
            class="w-full h-full object-cover"
          />
          <span v-else>{{ detail.fullName?.charAt(0) || 'N' }}</span>
        </div>

        <div class="space-y-1">
          <div class="flex flex-wrap items-center gap-2">
            <h2 class="text-lg sm:text-xl font-bold tracking-tight">{{ detail.fullName }}</h2>
            <span class="px-2.5 py-0.5 rounded-lg bg-white/10 text-indigo-200 font-mono text-xs font-semibold">
              {{ detail.code }}
            </span>
            <span :class="['px-2.5 py-0.5 rounded-full text-xs font-bold border', getStageBadge(detail.currentStage).cls]">
              {{ getStageBadge(detail.currentStage).label }}
            </span>
          </div>

          <div class="flex flex-wrap items-center gap-x-4 gap-y-1 text-xs text-slate-300">
            <span class="inline-flex items-center gap-1.5">
              <Building2 class="w-3.5 h-3.5 text-indigo-400" />
              {{ detail.departmentName }}
            </span>
            <span class="inline-flex items-center gap-1.5">
              <Briefcase class="w-3.5 h-3.5 text-cyan-400" />
              {{ detail.positionName }}
            </span>
            <span class="inline-flex items-center gap-1.5">
              <Calendar class="w-3.5 h-3.5 text-emerald-400" />
              {{ formatDate(detail.joinDate) }}
            </span>
          </div>
        </div>
      </div>

      <!-- Auto-update Month/Year Controls -->
      <div class="flex flex-wrap items-center gap-2.5 bg-slate-800/80 p-2.5 rounded-xl border border-slate-700/80">
        <div class="flex items-center gap-1.5 px-2 text-xs text-indigo-300 font-semibold">
          <Sparkles class="w-4 h-4 text-amber-400" />
          <span>{{ t('l_as_of_period') }}:</span>
        </div>

        <select
          :value="month"
          @change="emit('update:month', Number(($event.target as HTMLSelectElement).value)); emit('reload')"
          class="py-1.5 px-2.5 bg-slate-900 border border-slate-700 rounded-lg text-xs text-white focus:outline-hidden focus:border-indigo-400"
        >
          <option v-for="m in 12" :key="m" :value="m">{{ t('l_month_prefix') }} {{ m }}</option>
        </select>

        <select
          :value="year"
          @change="emit('update:year', Number(($event.target as HTMLSelectElement).value)); emit('reload')"
          class="py-1.5 px-2.5 bg-slate-900 border border-slate-700 rounded-lg text-xs text-white focus:outline-hidden focus:border-indigo-400"
        >
          <option v-for="y in [2025, 2026, 2027]" :key="y" :value="y">{{ t('l_year_prefix') }} {{ y }}</option>
        </select>
      </div>
    </div>

    <!-- 6 KPI Summary Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-3.5">
      <!-- 1. Cumulative Tenure -->
      <div class="bg-white p-4 rounded-2xl border border-slate-200/80 shadow-xs space-y-1.5">
        <div class="flex items-center justify-between text-xs text-slate-500 font-medium">
          <span>{{ t('l_total_tenure_kpi') }}</span>
          <Calendar class="w-4 h-4 text-indigo-600" />
        </div>
        <div class="text-xl font-extrabold text-slate-800 font-mono">
          {{ detail.totalTenureMonths }} {{ t('l_months_abbr') }}
        </div>
        <div class="text-[11px] text-slate-400">
          {{ detail.totalTenureDays }} {{ t('l_days_abbr') }}
        </div>
      </div>

      <!-- 2. Income Growth -->
      <div class="bg-white p-4 rounded-2xl border border-slate-200/80 shadow-xs space-y-1.5">
        <div class="flex items-center justify-between text-xs text-slate-500 font-medium">
          <span>{{ t('l_salary_growth_kpi') }}</span>
          <TrendingUp class="w-4 h-4 text-emerald-600" />
        </div>
        <div class="text-xl font-extrabold text-emerald-600 font-mono">
          +{{ detail.salaryGrowthPercent }}%
        </div>
        <div class="text-[11px] text-slate-500 truncate" :title="`${formatCurrency(detail.initialSalary)} -> ${formatCurrency(detail.currentSalary)}`">
          {{ formatCurrency(detail.currentSalary) }}
        </div>
      </div>

      <!-- 3. Monthly Hours (Office + OT) -->
      <div class="bg-white p-4 rounded-2xl border border-slate-200/80 shadow-xs space-y-1.5">
        <div class="flex items-center justify-between text-xs text-slate-500 font-medium">
          <span>{{ t('l_monthly_hours_kpi') }}</span>
          <Clock class="w-4 h-4 text-blue-600" />
        </div>
        <div class="text-xl font-extrabold text-slate-800 font-mono">
          {{ detail.currentMonthRegularHours }}h
          <span class="text-sm text-amber-600 font-bold">+{{ detail.currentMonthOvertimeHours }}h OT</span>
        </div>
        <div class="text-[11px] text-slate-400">
          {{ detail.currentMonthWorkDays }} {{ t('l_days_abbr') }} ({{ detail.asOfMonth }}/{{ detail.asOfYear }})
        </div>
      </div>

      <!-- 4. Punctuality Rate -->
      <div class="bg-white p-4 rounded-2xl border border-slate-200/80 shadow-xs space-y-1.5">
        <div class="flex items-center justify-between text-xs text-slate-500 font-medium">
          <span>{{ t('l_punctuality_kpi') }}</span>
          <CheckCircle2 class="w-4 h-4 text-cyan-600" />
        </div>
        <div class="text-xl font-extrabold text-cyan-700 font-mono">
          {{ detail.overallPunctualityRate }}%
        </div>
        <div class="text-[11px] text-slate-400">
          {{ t('l_on_time_vs_late_days') }}: {{ detail.onTimeDays }} / {{ detail.lateDays }}
        </div>
      </div>

      <!-- 5. Rewards & Discipline -->
      <div class="bg-white p-4 rounded-2xl border border-slate-200/80 shadow-xs space-y-1.5">
        <div class="flex items-center justify-between text-xs text-slate-500 font-medium">
          <span>{{ t('l_rewards_discipline_kpi') }}</span>
          <Award class="w-4 h-4 text-amber-500" />
        </div>
        <div class="text-xl font-extrabold text-slate-800 font-mono">
          <span class="text-emerald-600">+{{ detail.totalBonusCount }}</span>
          <span class="text-slate-300 mx-1">/</span>
          <span class="text-rose-600">-{{ detail.totalPenaltyCount }}</span>
        </div>
        <div class="text-[11px] text-slate-400 truncate">
          {{ formatCurrency(detail.totalBonusAmount - detail.totalPenaltyAmount) }}
        </div>
      </div>

      <!-- 6. Contracts & Leave Quota -->
      <div class="bg-white p-4 rounded-2xl border border-slate-200/80 shadow-xs space-y-1.5">
        <div class="flex items-center justify-between text-xs text-slate-500 font-medium">
          <span>{{ t('l_contracts_leave_kpi') }}</span>
          <FileText class="w-4 h-4 text-violet-600" />
        </div>
        <div class="text-xl font-extrabold text-violet-700 font-mono">
          {{ detail.totalContractsCount }} HĐ
        </div>
        <div class="text-[11px] text-slate-400">
          {{ detail.leaveDaysUsed }}/{{ detail.leaveDaysUsed + detail.leaveDaysRemaining }} {{ t('l_days_abbr') }}
        </div>
      </div>
    </div>
  </div>
</template>
