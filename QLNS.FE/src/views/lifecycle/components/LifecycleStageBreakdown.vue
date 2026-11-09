<script setup lang="ts">
import { computed } from 'vue';
import type { EmployeeLifecycleDetailDto, LifecycleStageDurationDto } from '@/types';
import { formatDate, formatCurrency } from '@/utils/formatters';
import { useI18n } from '@/locales';
import { GraduationCap, Briefcase, Award, CheckCircle2, Sparkles, Clock } from 'lucide-vue-next';

const props = defineProps<{
  detail: EmployeeLifecycleDetailDto;
}>();

const { t } = useI18n();

const stages = computed(() => [
  {
    key: 'Internship',
    title: t('l_stage_internship_title'),
    badge: t('s_stage_internship'),
    icon: GraduationCap,
    data: props.detail.internshipStage,
    accentClass: 'border-cyan-200 bg-cyan-50/40',
    iconBgClass: 'bg-cyan-100 text-cyan-700',
    badgeClass: 'bg-cyan-100 text-cyan-800 border-cyan-200',
    barClass: 'bg-cyan-500',
  },
  {
    key: 'Probation',
    title: t('l_stage_probation_title'),
    badge: t('s_stage_probation'),
    icon: Briefcase,
    data: props.detail.probationStage,
    accentClass: 'border-amber-200 bg-amber-50/40',
    iconBgClass: 'bg-amber-100 text-amber-700',
    badgeClass: 'bg-amber-100 text-amber-800 border-amber-200',
    barClass: 'bg-amber-500',
  },
  {
    key: 'Official',
    title: t('l_stage_official_title'),
    badge: t('s_stage_official'),
    icon: Award,
    data: props.detail.officialStage,
    accentClass: 'border-indigo-200 bg-indigo-50/40',
    iconBgClass: 'bg-indigo-100 text-indigo-700',
    badgeClass: 'bg-indigo-100 text-indigo-800 border-indigo-200',
    barClass: 'bg-indigo-600',
  },
]);

const formatPeriodRange = (stage: LifecycleStageDurationDto) => {
  if (!stage.hasCompletedOrActive || !stage.startDate) {
    return t('l_stage_not_applicable');
  }
  const start = formatDate(stage.startDate);
  const end = stage.isCurrent || !stage.endDate ? t('l_until_now') : formatDate(stage.endDate);
  return `${start} → ${end}`;
};
</script>

<template>
  <div class="bg-white rounded-2xl border border-slate-200/80 p-5 shadow-xs space-y-4">
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-2 pb-3 border-b border-slate-100">
      <div>
        <h2 class="text-sm sm:text-base font-bold text-slate-800 flex items-center gap-2">
          <Sparkles class="w-4 h-4 text-indigo-600" />
          {{ t('l_three_stages_section_title') }}
        </h2>
        <p class="text-xs text-slate-500 mt-0.5">
          {{ t('l_three_stages_section_desc') }}
        </p>
      </div>
      <span class="inline-flex items-center gap-1.5 px-3 py-1 rounded-full bg-indigo-50 text-indigo-700 border border-indigo-200 text-xs font-semibold shrink-0">
        <Clock class="w-3.5 h-3.5" />
        {{ t('l_monthly_auto_updated_badge') }} ({{ detail.asOfMonth }}/{{ detail.asOfYear }})
      </span>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
      <div
        v-for="stg in stages"
        :key="stg.key"
        :class="[
          'rounded-2xl border p-4 transition-all relative overflow-hidden flex flex-col justify-between gap-3',
          stg.data.isCurrent
            ? 'ring-2 ring-indigo-500/40 shadow-sm ' + stg.accentClass
            : stg.data.hasCompletedOrActive
              ? 'border-slate-200 bg-slate-50/60'
              : 'border-slate-100 bg-slate-50/30 opacity-70',
        ]"
      >
        <!-- Top Header -->
        <div class="flex items-start justify-between gap-2">
          <div class="flex items-center gap-2.5">
            <div :class="['w-10 h-10 rounded-xl flex items-center justify-center shrink-0', stg.iconBgClass]">
              <component :is="stg.icon" class="w-5 h-5" />
            </div>
            <div>
              <h3 class="text-xs sm:text-sm font-bold text-slate-800">{{ stg.title }}</h3>
              <span class="text-[11px] text-slate-500">{{ formatPeriodRange(stg.data) }}</span>
            </div>
          </div>

          <span
            v-if="stg.data.isCurrent"
            class="px-2.5 py-0.5 rounded-full text-[11px] font-bold bg-emerald-100 text-emerald-700 border border-emerald-200 shrink-0"
          >
            {{ t('l_stage_current_active') }}
          </span>
          <span
            v-else-if="stg.data.hasCompletedOrActive"
            class="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-[11px] font-semibold bg-slate-200/80 text-slate-700 shrink-0"
          >
            <CheckCircle2 class="w-3 h-3 text-emerald-600" />
            {{ t('l_stage_completed') }}
          </span>
        </div>

        <!-- Duration Big Metric -->
        <div class="p-3 rounded-xl bg-white/90 border border-slate-200/60 flex items-center justify-between">
          <div>
            <div class="text-[11px] text-slate-500 font-medium">{{ t('l_stage_duration_label') }}</div>
            <div class="text-lg sm:text-xl font-extrabold text-slate-800 font-mono mt-0.5">
              {{ stg.data.totalMonths }} {{ t('l_months_abbr') }}
              <span class="text-xs font-normal text-slate-500">({{ stg.data.totalDays }} {{ t('l_days_abbr') }})</span>
            </div>
          </div>

          <div class="text-right">
            <div class="text-[11px] text-slate-500 font-medium">{{ t('l_stage_contracts_signed') }}</div>
            <div class="text-sm font-bold text-indigo-700 font-mono mt-0.5">
              {{ stg.data.contractCount }}
            </div>
          </div>
        </div>

        <!-- Stage Salary Footer -->
        <div class="flex items-center justify-between text-xs pt-1">
          <span class="text-slate-500">{{ t('l_stage_salary_level') }}:</span>
          <span class="font-mono font-bold text-slate-800">
            {{ stg.data.representativeSalary > 0 ? formatCurrency(stg.data.representativeSalary) : '—' }}
          </span>
        </div>
      </div>
    </div>
  </div>
</template>
