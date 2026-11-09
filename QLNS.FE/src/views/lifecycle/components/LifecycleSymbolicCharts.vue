<script setup lang="ts">
import { computed } from 'vue';
import type { EmployeeLifecycleDetailDto } from '@/types';
import { formatCurrency } from '@/utils/formatters';
import { useI18n } from '@/locales';
import { BarChart3, PieChart, TrendingUp, Clock } from 'lucide-vue-next';

const props = defineProps<{
  detail: EmployeeLifecycleDetailDto;
}>();

const { t } = useI18n();

// 1. Stage Duration Ratio Percentages
const stageRatio = computed(() => {
  const intern = Math.max(0, props.detail.internshipStage.totalMonths);
  const prob = Math.max(0, props.detail.probationStage.totalMonths);
  const official = Math.max(0, props.detail.officialStage.totalMonths);
  const sum = intern + prob + official || 1;
  return {
    internPct: Math.round((intern / sum) * 100),
    probPct: Math.round((prob / sum) * 100),
    officialPct: Math.max(0, 100 - Math.round((intern / sum) * 100) - Math.round((prob / sum) * 100)),
  };
});

// 2. Max Net Salary in 6-month series for scaling SVG bars & polyline
const maxSalary = computed(() => {
  const vals = props.detail.monthlySeries.map((m) => m.netSalary);
  return Math.max(...vals, props.detail.currentSalary, 10000000);
});

const incomePolylinePoints = computed(() => {
  const series = props.detail.monthlySeries;
  if (!series.length) return '';
  const width = 480;
  const height = 140;
  return series
    .map((item, idx) => {
      const x = series.length === 1 ? width / 2 : (idx / (series.length - 1)) * (width - 40) + 20;
      const ratio = Math.min(1, Math.max(0.08, item.netSalary / maxSalary.value));
      const y = height - ratio * (height - 25) - 10;
      return `${x},${y}`;
    })
    .join(' ');
});

// 3. Max Hours in 6-month series
const maxMonthlyHours = computed(() => {
  const vals = props.detail.monthlySeries.map((m) => m.regularHours + m.overtimeHours);
  return Math.max(...vals, 180);
});

// 4. SVG Donut stroke-dasharray for Punctuality Rate (circumference = 2 * PI * 42 ≈ 264)
const donutCircumference = 264;
const punctualityDash = computed(() => {
  const rate = Math.min(100, Math.max(0, props.detail.overallPunctualityRate));
  const filled = (rate / 100) * donutCircumference;
  return `${filled} ${donutCircumference}`;
});
</script>

<template>
  <div class="space-y-4">
    <!-- Section Title + Stage Ratio Bar -->
    <div class="bg-white rounded-2xl border border-slate-200/80 p-5 shadow-xs space-y-3">
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-2">
        <h3 class="text-sm font-bold text-slate-800 flex items-center gap-2">
          <BarChart3 class="w-4 h-4 text-indigo-600" />
          {{ t('l_chart_stage_ratio_title') }}
        </h3>
        <div class="flex flex-wrap items-center gap-4 text-xs">
          <span class="inline-flex items-center gap-1.5 font-medium text-slate-600">
            <span class="w-3 h-3 rounded-xs bg-cyan-500 inline-block" />
            {{ t('s_stage_internship') }}: <strong>{{ detail.internshipStage.totalMonths }} {{ t('l_months_abbr') }} ({{ stageRatio.internPct }}%)</strong>
          </span>
          <span class="inline-flex items-center gap-1.5 font-medium text-slate-600">
            <span class="w-3 h-3 rounded-xs bg-amber-500 inline-block" />
            {{ t('s_stage_probation') }}: <strong>{{ detail.probationStage.totalMonths }} {{ t('l_months_abbr') }} ({{ stageRatio.probPct }}%)</strong>
          </span>
          <span class="inline-flex items-center gap-1.5 font-medium text-slate-600">
            <span class="w-3 h-3 rounded-xs bg-indigo-600 inline-block" />
            {{ t('s_stage_official') }}: <strong>{{ detail.officialStage.totalMonths }} {{ t('l_months_abbr') }} ({{ stageRatio.officialPct }}%)</strong>
          </span>
        </div>
      </div>

      <!-- Segmented Progress Bar -->
      <div class="h-4 w-full rounded-full bg-slate-100 overflow-hidden flex p-0.5 gap-0.5">
        <div
          v-if="stageRatio.internPct > 0"
          class="h-full bg-cyan-500 first:rounded-l-full last:rounded-r-full transition-all duration-500"
          :style="{ width: `${stageRatio.internPct}%` }"
        />
        <div
          v-if="stageRatio.probPct > 0"
          class="h-full bg-amber-500 first:rounded-l-full last:rounded-r-full transition-all duration-500"
          :style="{ width: `${stageRatio.probPct}%` }"
        />
        <div
          v-if="stageRatio.officialPct > 0"
          class="h-full bg-indigo-600 first:rounded-l-full last:rounded-r-full transition-all duration-500"
          :style="{ width: `${stageRatio.officialPct}%` }"
        />
      </div>
    </div>

    <!-- 3 Symbolic Charts Grid -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-4">
      <!-- Chart 1: 6-Month Net Income & Tenure SVG Trend -->
      <div class="bg-white rounded-2xl border border-slate-200/80 p-5 shadow-xs flex flex-col justify-between space-y-4">
        <div class="flex items-center justify-between">
          <h3 class="text-xs sm:text-sm font-bold text-slate-800 flex items-center gap-2">
            <TrendingUp class="w-4 h-4 text-emerald-600" />
            {{ t('l_chart_monthly_income_title') }}
          </h3>
        </div>

        <!-- Symbolic SVG Line + Area Chart -->
        <div class="w-full">
          <svg viewBox="0 0 480 150" class="w-full h-36 overflow-visible">
            <!-- Horizontal Grid Lines -->
            <line x1="10" y1="25" x2="470" y2="25" stroke="#f1f5f9" stroke-width="1" />
            <line x1="10" y1="75" x2="470" y2="75" stroke="#f1f5f9" stroke-width="1" />
            <line x1="10" y1="125" x2="470" y2="125" stroke="#e2e8f0" stroke-width="1" />

            <!-- Line Path -->
            <polyline
              fill="none"
              stroke="#4f46e5"
              stroke-width="3"
              stroke-linecap="round"
              stroke-linejoin="round"
              :points="incomePolylinePoints"
            />

            <!-- Data Nodes -->
            <g v-for="(m, idx) in detail.monthlySeries" :key="m.label">
              <circle
                :cx="detail.monthlySeries.length === 1 ? 240 : (idx / (detail.monthlySeries.length - 1)) * 440 + 20"
                :cy="140 - Math.min(1, Math.max(0.08, m.netSalary / maxSalary)) * 115 - 10"
                r="5"
                class="fill-white stroke-indigo-600 stroke-2"
              />
            </g>
          </svg>

          <!-- Month Labels & Values -->
          <div class="grid grid-cols-6 gap-1 pt-2 border-t border-slate-100 text-center">
            <div v-for="m in detail.monthlySeries" :key="m.label" class="space-y-0.5">
              <div class="text-[10px] font-bold text-slate-600">{{ m.label }}</div>
              <div class="text-[10px] font-mono text-indigo-600 font-semibold truncate" :title="formatCurrency(m.netSalary)">
                {{ (m.netSalary / 1000000).toFixed(1) }}M
              </div>
              <div class="text-[9px] text-slate-400">{{ m.cumulativeTenureMonths }} {{ t('l_months_abbr') }}</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Chart 2: Monthly Office Hours vs Approved OT Hours Stacked Columns -->
      <div class="bg-white rounded-2xl border border-slate-200/80 p-5 shadow-xs flex flex-col justify-between space-y-4">
        <div class="flex items-center justify-between">
          <h3 class="text-xs sm:text-sm font-bold text-slate-800 flex items-center gap-2">
            <Clock class="w-4 h-4 text-amber-500" />
            {{ t('l_chart_monthly_hours_title') }}
          </h3>
        </div>

        <div class="h-40 flex items-end justify-between gap-2 pt-4 px-2 border-b border-slate-100">
          <div
            v-for="m in detail.monthlySeries"
            :key="m.label"
            class="flex-1 flex flex-col items-center gap-1.5 h-full justify-end group"
          >
            <div class="text-[10px] font-mono font-bold text-slate-600">
              {{ m.regularHours }}h
              <span v-if="m.overtimeHours > 0" class="text-amber-600 block text-[9px]">+{{ m.overtimeHours }}h OT</span>
            </div>

            <!-- Stacked Bar -->
            <div class="w-full max-w-[30px] flex flex-col justify-end rounded-t-lg overflow-hidden bg-slate-100 h-24">
              <div
                v-if="m.overtimeHours > 0"
                class="w-full bg-amber-500 transition-all duration-500"
                :style="{ height: `${Math.max(8, (m.overtimeHours / maxMonthlyHours) * 100)}%` }"
                :title="`OT: ${m.overtimeHours}h`"
              />
              <div
                class="w-full bg-indigo-600 transition-all duration-500"
                :style="{ height: `${Math.max(6, (m.regularHours / maxMonthlyHours) * 100)}%` }"
                :title="`${t('l_regular_hours_col')}: ${m.regularHours}h`"
              />
            </div>

            <span class="text-[10px] font-semibold text-slate-500 pb-1">{{ m.label }}</span>
          </div>
        </div>

        <div class="flex items-center justify-center gap-4 text-[11px] text-slate-600">
          <span class="inline-flex items-center gap-1.5">
            <span class="w-2.5 h-2.5 rounded-xs bg-indigo-600 inline-block" />
            {{ t('l_regular_hours_col') }}
          </span>
          <span class="inline-flex items-center gap-1.5">
            <span class="w-2.5 h-2.5 rounded-xs bg-amber-500 inline-block" />
            {{ t('l_ot_hours_col') }}
          </span>
        </div>
      </div>

      <!-- Chart 3: Punctuality & Leave SVG Donut -->
      <div class="bg-white rounded-2xl border border-slate-200/80 p-5 shadow-xs flex flex-col justify-between space-y-4">
        <h3 class="text-xs sm:text-sm font-bold text-slate-800 flex items-center gap-2">
          <PieChart class="w-4 h-4 text-cyan-600" />
          {{ t('l_chart_punctuality_donut_title') }}
        </h3>

        <div class="flex items-center justify-around gap-4 my-auto">
          <div class="relative w-28 h-28 flex items-center justify-center shrink-0">
            <svg viewBox="0 0 100 100" class="w-full h-full -rotate-90">
              <circle cx="50" cy="50" r="42" fill="none" stroke="#f1f5f9" stroke-width="10" />
              <circle
                cx="50"
                cy="50"
                r="42"
                fill="none"
                stroke="#0891b2"
                stroke-width="10"
                stroke-linecap="round"
                :stroke-dasharray="punctualityDash"
              />
            </svg>
            <div class="absolute inset-0 flex flex-col items-center justify-center">
              <span class="text-base font-extrabold text-slate-800 font-mono">{{ detail.overallPunctualityRate }}%</span>
              <span class="text-[10px] text-slate-400">{{ t('s_present') }}</span>
            </div>
          </div>

          <div class="space-y-2 text-xs flex-1">
            <div class="flex items-center justify-between p-2 rounded-xl bg-emerald-50/70 border border-emerald-100">
              <span class="text-slate-600">{{ t('s_present') }}</span>
              <span class="font-mono font-bold text-emerald-700">{{ detail.onTimeDays }} {{ t('l_days_abbr') }}</span>
            </div>
            <div class="flex items-center justify-between p-2 rounded-xl bg-amber-50/70 border border-amber-100">
              <span class="text-slate-600">{{ t('s_late') }}</span>
              <span class="font-mono font-bold text-amber-700">{{ detail.lateDays }} {{ t('l_days_abbr') }}</span>
            </div>
            <div class="flex items-center justify-between p-2 rounded-xl bg-indigo-50/70 border border-indigo-100">
              <span class="text-slate-600">{{ t('s_on_leave') }}</span>
              <span class="font-mono font-bold text-indigo-700">
                {{ detail.leaveDaysUsed }} / {{ detail.leaveDaysUsed + detail.leaveDaysRemaining }} {{ t('l_days_abbr') }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
