<script setup lang="ts">
import type { AttendanceDto } from '@/types';
import { formatDate } from '@/utils/formatters';
import { useI18n } from '@/locales';
import StatusBadge from '@/components/StatusBadge.vue';
import { Calendar, Clock } from 'lucide-vue-next';

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
        <h3 class="font-bold text-slate-800 text-xs sm:text-sm">
          {{ t('l_my_attendance_history') }}
        </h3>
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
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse text-xs">
          <thead>
            <tr class="bg-slate-50/90 border-b border-slate-200/80 text-slate-500 font-semibold uppercase tracking-wider">
              <th class="py-3 px-4">{{ t('l_attendance_date') }}</th>
              <th class="py-3 px-4">{{ t('l_check_in_time') }}</th>
              <th class="py-3 px-4">{{ t('l_check_out_time') }}</th>
              <th class="py-3 px-4">{{ t('l_total_hours') }}</th>
              <th class="py-3 px-4">{{ t('l_status') }}</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr v-if="loading">
              <td colspan="5" class="py-10 text-center text-slate-400">
                {{ t('l_loading_data') }}
              </td>
            </tr>

            <tr v-else-if="myAttendance.length === 0">
              <td colspan="5" class="py-10 text-center text-slate-400">
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
                <span v-if="att.checkIn" class="inline-flex items-center gap-1.5 font-mono text-emerald-600 font-semibold text-xs">
                  <Clock class="w-3.5 h-3.5" /> {{ att.checkIn }}
                </span>
                <span v-else class="text-slate-400">—</span>
              </td>
              <td class="py-3 px-4">
                <span v-if="att.checkOut" class="inline-flex items-center gap-1.5 font-mono text-blue-600 font-semibold text-xs">
                  <Clock class="w-3.5 h-3.5" /> {{ att.checkOut }}
                </span>
                <span v-else class="text-slate-400">—</span>
              </td>
              <td class="py-3 px-4 font-mono font-semibold text-slate-800">
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
