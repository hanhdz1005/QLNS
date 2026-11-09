<script setup lang="ts">
import type { AttendanceDto } from '@/types';
import { formatDate } from '@/utils/formatters';
import { useI18n } from '@/locales';
import StatusBadge from '@/components/StatusBadge.vue';
import { Calendar, Clock, Repeat, Flame } from 'lucide-vue-next';

defineProps<{
  myAttendance: AttendanceDto[];
  loading: boolean;
  month: number;
  year: number;
}>();

const emit = defineEmits<{
  (e: 'update:month', val: number): void;
  (e: 'update:year', val: number): void;
  (e: 'reload'): void;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="space-y-4 sm:space-y-5">
    <!-- Month/Year Toolbar -->
    <div class="bg-white p-3 sm:p-4 rounded-2xl border border-slate-200/80 shadow-xs flex flex-wrap items-center justify-between gap-3 shrink-0">
      <div class="flex items-center gap-2">
        <Calendar class="w-4 h-4 text-indigo-600" />
        <div>
          <h3 class="font-bold text-slate-800 text-xs sm:text-sm">
            {{ t('l_my_attendance_history') }}
          </h3>
          <p class="text-[11px] text-slate-500">{{ t('l_office_hours_rule_note') }}</p>
        </div>
      </div>

      <div class="flex items-center gap-2.5">
        <select
          :value="month"
          @change="emit('update:month', Number(($event.target as HTMLSelectElement).value)); emit('reload')"
          class="py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm text-slate-700 focus:outline-hidden focus:border-indigo-500"
        >
          <option v-for="m in 12" :key="m" :value="m">{{ t('l_month_prefix') }} {{ m }}</option>
        </select>

        <select
          :value="year"
          @change="emit('update:year', Number(($event.target as HTMLSelectElement).value)); emit('reload')"
          class="py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm text-slate-700 focus:outline-hidden focus:border-indigo-500"
        >
          <option v-for="y in [2025, 2026, 2027]" :key="y" :value="y">{{ t('l_year_prefix') }} {{ y }}</option>
        </select>
      </div>
    </div>

    <!-- Attendance Table -->
    <div class="bg-white rounded-2xl border border-slate-200/80 shadow-xs overflow-hidden">
      <div class="overflow-x-auto" tabindex="0" role="region" :aria-label="t('l_my_attendance_history')">
        <table class="w-full text-left border-collapse text-xs">
          <thead>
            <tr class="bg-slate-50/90 border-b border-slate-200/80 text-slate-500 font-semibold uppercase tracking-wider">
              <th class="py-3 px-4">{{ t('l_attendance_date') }}</th>
              <th class="py-3 px-4">{{ t('l_first_check_in') }}</th>
              <th class="py-3 px-4">{{ t('l_last_check_out') }}</th>
              <th class="py-3 px-4">{{ t('l_scan_count') }}</th>
              <th class="py-3 px-4">{{ t('l_regular_hours_col') }}</th>
              <th class="py-3 px-4">{{ t('l_ot_hours_col') }}</th>
              <th class="py-3 px-4">{{ t('l_total_hours') }}</th>
              <th class="py-3 px-4">{{ t('l_status') }}</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr v-if="loading">
              <td colspan="8" class="py-10 text-center text-slate-400">
                {{ t('l_loading_data') }}
              </td>
            </tr>

            <tr v-else-if="myAttendance.length === 0">
              <td colspan="8" class="py-10 text-center text-slate-400">
                {{ t('l_no_attendance_data_for_month') }}
              </td>
            </tr>

            <tr
              v-for="att in myAttendance"
              :key="att.id"
              class="hover:bg-slate-50/70 transition-colors"
            >
              <td class="py-3 px-4 font-semibold text-slate-800 text-xs sm:text-sm">
                {{ formatDate(att.date) }}
              </td>
              <td class="py-3 px-4">
                <span v-if="att.firstCheckIn || att.checkIn" class="inline-flex items-center gap-1.5 font-mono text-emerald-600 font-semibold text-xs">
                  <Clock class="w-3.5 h-3.5" /> {{ att.firstCheckIn || att.checkIn }}
                </span>
                <span v-else class="text-slate-400">—</span>
              </td>
              <td class="py-3 px-4">
                <span v-if="att.lastCheckOut || att.checkOut" class="inline-flex items-center gap-1.5 font-mono text-blue-600 font-semibold text-xs">
                  <Clock class="w-3.5 h-3.5" /> {{ att.lastCheckOut || att.checkOut }}
                </span>
                <span v-else class="text-slate-400">—</span>
              </td>
              <td class="py-3 px-4">
                <span class="inline-flex items-center gap-1 px-2 py-0.5 rounded-lg bg-slate-100 text-slate-700 font-mono font-semibold text-[11px]">
                  <Repeat class="w-3 h-3 text-slate-500" /> #{{ att.scanCount || 1 }}
                </span>
              </td>
              <td class="py-3 px-4 font-mono font-semibold text-slate-700">
                {{ att.regularHours !== undefined ? `${att.regularHours}h` : `${att.hoursWorked ?? 0}h` }}
              </td>
              <td class="py-3 px-4 font-mono">
                <span
                  v-if="att.overtimeHours && att.overtimeHours > 0"
                  class="inline-flex items-center gap-1 px-2 py-0.5 rounded-lg bg-amber-50 text-amber-700 border border-amber-200 font-bold text-xs"
                >
                  <Flame class="w-3 h-3 text-amber-500" /> +{{ att.overtimeHours }}h
                </span>
                <span v-else class="text-slate-400">0h</span>
              </td>
              <td class="py-3 px-4 font-mono font-bold text-indigo-700">
                {{ att.hoursWorked !== null && att.hoursWorked !== undefined ? `${att.hoursWorked} ${t('l_hours_unit')}` : '—' }}
              </td>
              <td class="py-3 px-4">
                <StatusBadge type="attendance" :value="att.status" />
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
