<script setup lang="ts">
import type { LeaveBalanceDto } from '@/types';
import { useI18n } from '@/locales';
import { CalendarCheck, Clock, ShieldCheck } from 'lucide-vue-next';

defineProps<{
  balance: LeaveBalanceDto;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="grid grid-cols-1 sm:grid-cols-3 gap-4 sm:gap-5 shrink-0">
    <!-- Total days -->
    <div class="bg-white p-4 sm:p-5 rounded-2xl border border-slate-200/80 shadow-xs flex items-center gap-4">
      <div class="w-10 h-10 rounded-xl bg-indigo-50 text-indigo-600 flex items-center justify-center shrink-0 border border-indigo-100/60 shadow-2xs">
        <CalendarCheck class="w-5 h-5" />
      </div>
      <div>
        <span class="text-xs font-semibold text-slate-500 uppercase tracking-wider block">
          {{ t('l_total_leave_days') }} {{ balance.year }}
        </span>
        <div class="text-xl sm:text-2xl font-extrabold text-slate-800 mt-0.5">
          {{ balance.totalLeaveDays }} {{ t('l_days_unit') }}
        </div>
      </div>
    </div>

    <!-- Used days -->
    <div class="bg-white p-4 sm:p-5 rounded-2xl border border-slate-200/80 shadow-xs flex items-center gap-4">
      <div class="w-10 h-10 rounded-xl bg-amber-50 text-amber-600 flex items-center justify-center shrink-0 border border-amber-100/60 shadow-2xs">
        <Clock class="w-5 h-5" />
      </div>
      <div>
        <span class="text-xs font-semibold text-slate-500 uppercase tracking-wider block">
          {{ t('l_used_leave_days') }}
        </span>
        <div class="text-xl sm:text-2xl font-extrabold text-amber-600 mt-0.5">
          {{ balance.usedLeaveDays }} {{ t('l_days_unit') }}
        </div>
      </div>
    </div>

    <!-- Remaining days -->
    <div class="bg-white p-4 sm:p-5 rounded-2xl border border-slate-200/80 shadow-xs flex items-center gap-4">
      <div class="w-10 h-10 rounded-xl bg-emerald-50 text-emerald-600 flex items-center justify-center shrink-0 border border-emerald-100/60 shadow-2xs">
        <ShieldCheck class="w-5 h-5" />
      </div>
      <div>
        <span class="text-xs font-semibold text-slate-500 uppercase tracking-wider block">
          {{ t('l_remaining_leave_days') }}
        </span>
        <div class="text-xl sm:text-2xl font-extrabold text-emerald-600 mt-0.5">
          {{ balance.remainingLeaveDays }} {{ t('l_days_unit') }}
        </div>
      </div>
    </div>
  </div>
</template>
