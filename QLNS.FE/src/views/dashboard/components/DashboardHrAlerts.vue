<script setup lang="ts">
import type { HrAlertsDto } from '@/types';
import { useI18n } from '@/locales';
import { AlertTriangle, Cake, Clock } from 'lucide-vue-next';

defineProps<{
  alerts: HrAlertsDto;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="bg-white p-5 rounded-2xl border border-slate-200/80 shadow-xs space-y-4">
    <div class="flex items-center justify-between border-b border-slate-100 pb-3">
      <h2 class="text-sm sm:text-base font-bold text-slate-800">{{ t('l_hr_alerts_title') }}</h2>
    </div>

    <div class="space-y-4">
      <!-- 1. Expiring Contracts -->
      <div class="space-y-2">
        <div class="flex items-center justify-between text-xs font-semibold text-amber-800 bg-amber-50 px-3 py-1.5 rounded-xl border border-amber-200/60">
          <span class="flex items-center gap-2">
            <AlertTriangle class="w-3.5 h-3.5 text-amber-600" />
            {{ t('l_expiring_contracts_alert') }}
          </span>
          <span class="text-xs bg-amber-200/70 text-amber-900 font-bold px-2 py-0.5 rounded-full">{{ alerts.expiringContracts.length }}</span>
        </div>

        <div v-if="alerts.expiringContracts.length === 0" class="text-center py-2 text-slate-400 text-xs">
          {{ t('l_no_expiring_contracts') }}
        </div>
        <div v-else class="space-y-1.5">
          <div
            v-for="c in alerts.expiringContracts"
            :key="c.contractId"
            class="p-2.5 sm:p-3 rounded-xl bg-slate-50/80 border border-slate-100 flex items-center justify-between text-xs"
          >
            <div>
              <div class="font-semibold text-slate-800 text-xs sm:text-sm">{{ c.employeeName }}</div>
              <div class="text-[11px] text-slate-400 mt-0.5">{{ t('l_contract_number_abbr') }}: {{ c.contractNumber }}</div>
            </div>
            <span class="text-rose-600 font-bold font-mono text-xs">
              {{ t('l_days_remaining') }} {{ c.daysRemaining }} {{ t('l_days_unit') }}
            </span>
          </div>
        </div>
      </div>

      <!-- 2. Upcoming Birthdays -->
      <div class="space-y-2">
        <div class="flex items-center justify-between text-xs font-semibold text-pink-800 bg-pink-50 px-3 py-1.5 rounded-xl border border-pink-200/60">
          <span class="flex items-center gap-2">
            <Cake class="w-3.5 h-3.5 text-pink-500" />
            {{ t('l_birthdays_alert') }}
          </span>
          <span class="text-xs bg-pink-200/70 text-pink-900 font-bold px-2 py-0.5 rounded-full">{{ alerts.upcomingBirthdays.length }}</span>
        </div>

        <div v-if="alerts.upcomingBirthdays.length === 0" class="text-center py-2 text-slate-400 text-xs">
          {{ t('l_no_birthdays_this_month') }}
        </div>
        <div v-else class="space-y-1.5">
          <div
            v-for="b in alerts.upcomingBirthdays"
            :key="b.employeeId"
            class="p-2.5 sm:p-3 rounded-xl bg-slate-50/80 border border-slate-100 flex items-center justify-between text-xs"
          >
            <div>
              <div class="font-semibold text-slate-800 text-xs sm:text-sm">{{ b.employeeName }}</div>
              <div class="text-[11px] text-slate-400 mt-0.5">{{ b.departmentName }}</div>
            </div>
            <span class="text-pink-600 font-bold font-mono text-xs">{{ t('l_day_prefix') }} {{ b.day }}</span>
          </div>
        </div>
      </div>

      <!-- 3. Probation Ending -->
      <div class="space-y-2">
        <div class="flex items-center justify-between text-xs font-semibold text-indigo-800 bg-indigo-50 px-3 py-1.5 rounded-xl border border-indigo-200/60">
          <span class="flex items-center gap-2">
            <Clock class="w-3.5 h-3.5 text-indigo-600" />
            {{ t('l_probation_ending_alert') }}
          </span>
          <span class="text-xs bg-indigo-200/70 text-indigo-900 font-bold px-2 py-0.5 rounded-full">{{ alerts.probationEndingEmployees.length }}</span>
        </div>

        <div v-if="alerts.probationEndingEmployees.length === 0" class="text-center py-2 text-slate-400 text-xs">
          {{ t('l_no_probation_ending') }}
        </div>
        <div v-else class="space-y-1.5">
          <div
            v-for="p in alerts.probationEndingEmployees"
            :key="p.employeeId"
            class="p-2.5 sm:p-3 rounded-xl bg-slate-50/80 border border-slate-100 flex items-center justify-between text-xs"
          >
            <div>
              <div class="font-semibold text-slate-800 text-xs sm:text-sm">{{ p.employeeName }}</div>
              <div class="text-[11px] text-slate-400 mt-0.5">{{ p.departmentName }}</div>
            </div>
            <span class="text-indigo-600 font-bold font-mono text-xs">
              {{ t('l_days_remaining') }} {{ p.daysRemaining }} {{ t('l_days_unit') }}
            </span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
