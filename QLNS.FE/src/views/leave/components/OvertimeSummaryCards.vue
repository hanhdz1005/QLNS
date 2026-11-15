<script setup lang="ts">
import { computed } from 'vue';
import type { OvertimeMonthlySummaryDto } from '@/types';
import { useI18n } from '@/locales';
import { Clock, CalendarPlus, Award, DollarSign, AlertTriangle } from 'lucide-vue-next';

const props = defineProps<{
  summary: OvertimeMonthlySummaryDto | null;
}>();

const { t } = useI18n();

const totalHours = computed(() => props.summary?.totalApprovedHours ?? 0);
const limitHours = computed(() => props.summary?.monthlyLimitHours || 40);
const limitRatio = computed(() => Math.min(100, Math.round((totalHours.value / limitHours.value) * 100)));
const isNearOrOverLimit = computed(() => totalHours.value >= 32);
</script>

<template>
  <div class="space-y-3">
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-3.5 sm:gap-4">
      <!-- 1. Monthly Approved Hours vs 40h Cap -->
      <div class="bg-white p-4 sm:p-5 rounded-2xl border border-slate-200/80 shadow-xs flex flex-col justify-between">
        <div class="flex items-start justify-between gap-2">
          <div>
            <span class="text-xs font-medium text-slate-500">{{ t('l_ot_kpi_monthly_hours') }}</span>
            <div class="text-xl sm:text-2xl font-extrabold text-slate-800 mt-1 font-mono">
              {{ totalHours }}h
              <span class="text-xs font-normal text-slate-400">/ {{ limitHours }}h</span>
            </div>
          </div>
          <div class="w-10 h-10 rounded-xl bg-amber-50 text-amber-600 flex items-center justify-center shrink-0">
            <Clock class="w-5 h-5" />
          </div>
        </div>
        <div class="mt-3 space-y-1">
          <div class="w-full h-2 bg-slate-100 rounded-full overflow-hidden">
            <div
              :class="[
                'h-full rounded-full transition-all',
                totalHours >= limitHours
                  ? 'bg-rose-500'
                  : totalHours >= 30
                    ? 'bg-amber-500'
                    : 'bg-emerald-500',
              ]"
              :style="{ width: `${limitRatio}%` }"
            />
          </div>
          <div class="text-[11px] text-slate-400 flex justify-between">
            <span>{{ t('l_ot_kpi_limit_suffix') }}</span>
            <span class="font-semibold">{{ limitRatio }}%</span>
          </div>
        </div>
      </div>

      <!-- 2. Compensatory Leave (Comp-off) -->
      <div class="bg-white p-4 sm:p-5 rounded-2xl border border-slate-200/80 shadow-xs flex items-start justify-between gap-2">
        <div>
          <span class="text-xs font-medium text-slate-500">{{ t('l_ot_kpi_comp_leave') }}</span>
          <div class="text-xl sm:text-2xl font-extrabold text-emerald-700 mt-1 font-mono">
            {{ summary?.compensatoryHours ?? 0 }}h
          </div>
          <div class="text-xs font-semibold text-emerald-600 mt-1">
            +{{ summary?.compensatoryDaysAdded ?? 0 }} {{ t('l_days_unit') }}
          </div>
        </div>
        <div class="w-10 h-10 rounded-xl bg-emerald-50 text-emerald-600 flex items-center justify-center shrink-0">
          <CalendarPlus class="w-5 h-5" />
        </div>
      </div>

      <!-- 3. KPI & Attendance Recognition Only -->
      <div class="bg-white p-4 sm:p-5 rounded-2xl border border-slate-200/80 shadow-xs flex items-start justify-between gap-2">
        <div>
          <span class="text-xs font-medium text-slate-500">{{ t('l_ot_kpi_recognition') }}</span>
          <div class="text-xl sm:text-2xl font-extrabold text-indigo-700 mt-1 font-mono">
            {{ summary?.recognitionHours ?? 0 }}h
          </div>
          <div class="text-[11px] text-slate-400 mt-1">
            {{ t('l_ot_comp_recognition') }}
          </div>
        </div>
        <div class="w-10 h-10 rounded-xl bg-indigo-50 text-indigo-600 flex items-center justify-center shrink-0">
          <Award class="w-5 h-5" />
        </div>
      </div>

      <!-- 4. Paid OT Hours -->
      <div class="bg-white p-4 sm:p-5 rounded-2xl border border-slate-200/80 shadow-xs flex items-start justify-between gap-2">
        <div>
          <span class="text-xs font-medium text-slate-500">{{ t('l_ot_kpi_paid_hours') }}</span>
          <div class="text-xl sm:text-2xl font-extrabold text-slate-800 mt-1 font-mono">
            {{ summary?.paidHours ?? 0 }}h
          </div>
          <div class="text-[11px] text-amber-600 font-semibold mt-1">
            {{ t('s_pending') }}: {{ summary?.pendingCount ?? 0 }}
          </div>
        </div>
        <div class="w-10 h-10 rounded-xl bg-slate-100 text-slate-700 flex items-center justify-center shrink-0">
          <DollarSign class="w-5 h-5" />
        </div>
      </div>
    </div>

    <!-- Warning Banner if approaching or exceeding 40h/month -->
    <div
      v-if="isNearOrOverLimit"
      class="p-3.5 rounded-2xl bg-rose-50 border border-rose-200 text-rose-800 text-xs font-semibold flex items-center gap-2.5"
    >
      <AlertTriangle class="w-4 h-4 text-rose-600 shrink-0" />
      <span>{{ t('l_ot_over_limit_warning') }}</span>
    </div>
  </div>
</template>
