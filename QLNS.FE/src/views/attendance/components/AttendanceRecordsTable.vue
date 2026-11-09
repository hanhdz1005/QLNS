<script setup lang="ts">
import type { AttendanceDto } from '@/types';
import { formatDate } from '@/utils/formatters';
import { useI18n } from '@/locales';
import StatusBadge from '@/components/StatusBadge.vue';
import { Clock, Edit, Trash2, Repeat, Flame } from 'lucide-vue-next';

defineProps<{
  records: AttendanceDto[];
  loading: boolean;
  isAdmin: boolean;
}>();

const emit = defineEmits<{
  (e: 'edit', record: AttendanceDto): void;
  (e: 'delete', id: string): void;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="overflow-x-auto" tabindex="0" role="region" :aria-label="t('l_tab_attendance_records')">
    <table class="w-full text-left border-collapse text-xs">
      <thead>
        <tr class="bg-slate-50/90 border-b border-slate-200/80 text-slate-500 font-semibold uppercase tracking-wider">
          <th class="py-3 px-4">{{ t('l_full_name') }}</th>
          <th class="py-3 px-4">{{ t('l_attendance_date') }}</th>
          <th class="py-3 px-4">{{ t('l_first_check_in') }}</th>
          <th class="py-3 px-4">{{ t('l_last_check_out') }}</th>
          <th class="py-3 px-4">{{ t('l_scan_count') }}</th>
          <th class="py-3 px-4">{{ t('l_regular_hours_col') }}</th>
          <th class="py-3 px-4">{{ t('l_ot_hours_col') }}</th>
          <th class="py-3 px-4">{{ t('l_total_hours') }}</th>
          <th class="py-3 px-4">{{ t('l_status') }}</th>
          <th v-if="isAdmin" class="py-3 px-4 text-right">{{ t('l_actions') }}</th>
        </tr>
      </thead>
      <tbody class="divide-y divide-slate-100">
        <tr v-if="loading">
          <td :colspan="isAdmin ? 10 : 9" class="py-10 text-center text-slate-400">
            {{ t('l_loading_data') }}
          </td>
        </tr>

        <tr v-else-if="records.length === 0">
          <td :colspan="isAdmin ? 10 : 9" class="py-10 text-center text-slate-400">
            {{ t('l_no_data') }}
          </td>
        </tr>

        <tr
          v-for="att in records"
          :key="att.id"
          class="hover:bg-slate-50/70 transition-colors"
        >
          <!-- Employee -->
          <td class="py-3 px-4">
            <div class="flex items-center gap-2.5">
              <div class="w-8 h-8 rounded-xl bg-indigo-50 text-indigo-700 font-bold text-xs flex items-center justify-center shrink-0 border border-indigo-100">
                {{ att.employeeName?.charAt(0) || 'N' }}
              </div>
              <span class="font-semibold text-slate-800 text-xs sm:text-sm">{{ att.employeeName }}</span>
            </div>
          </td>

          <!-- Date -->
          <td class="py-3 px-4 text-slate-700 font-medium">
            {{ formatDate(att.date) }}
          </td>

          <!-- First Check In -->
          <td class="py-3 px-4 text-slate-700">
            <span v-if="att.firstCheckIn || att.checkIn" class="inline-flex items-center gap-1.5 font-mono text-emerald-600 font-semibold text-xs">
              <Clock class="w-3.5 h-3.5" /> {{ att.firstCheckIn || att.checkIn }}
            </span>
            <span v-else class="text-slate-400">—</span>
          </td>

          <!-- Last Check Out -->
          <td class="py-3 px-4 text-slate-700">
            <span v-if="att.lastCheckOut || att.checkOut" class="inline-flex items-center gap-1.5 font-mono text-blue-600 font-semibold text-xs">
              <Clock class="w-3.5 h-3.5" /> {{ att.lastCheckOut || att.checkOut }}
            </span>
            <span v-else class="text-slate-400">—</span>
          </td>

          <!-- Scan Count -->
          <td class="py-3 px-4">
            <span class="inline-flex items-center gap-1 px-2 py-0.5 rounded-lg bg-slate-100 text-slate-700 font-mono font-semibold text-[11px]">
              <Repeat class="w-3 h-3 text-slate-500" /> #{{ att.scanCount || 1 }}
            </span>
          </td>

          <!-- Regular Hours -->
          <td class="py-3 px-4 font-mono font-semibold text-slate-700">
            {{ att.regularHours !== undefined ? `${att.regularHours}h` : `${att.hoursWorked ?? 0}h` }}
          </td>

          <!-- Overtime Hours -->
          <td class="py-3 px-4 font-mono">
            <span
              v-if="att.overtimeHours && att.overtimeHours > 0"
              class="inline-flex items-center gap-1 px-2 py-0.5 rounded-lg bg-amber-50 text-amber-700 border border-amber-200 font-bold text-xs"
            >
              <Flame class="w-3 h-3 text-amber-500" /> +{{ att.overtimeHours }}h
            </span>
            <span v-else class="text-slate-400">0h</span>
          </td>

          <!-- Total Hours Worked -->
          <td class="py-3 px-4 font-mono font-bold text-indigo-700">
            {{ att.hoursWorked !== null && att.hoursWorked !== undefined ? `${att.hoursWorked}h` : '—' }}
          </td>

          <!-- Status -->
          <td class="py-3 px-4">
            <StatusBadge type="attendance" :value="att.status" />
          </td>

          <!-- Actions -->
          <td v-if="isAdmin" class="py-3 px-4 text-right">
            <div class="inline-flex items-center gap-1">
              <button
                @click="emit('edit', att)"
                :title="t('b_edit')"
                class="p-1.5 text-slate-500 hover:text-amber-600 hover:bg-amber-50 rounded-lg transition-colors"
              >
                <Edit class="w-4 h-4" />
              </button>
              <button
                @click="emit('delete', att.id)"
                :title="t('b_delete')"
                class="p-1.5 text-slate-500 hover:text-rose-600 hover:bg-rose-50 rounded-lg transition-colors"
              >
                <Trash2 class="w-4 h-4" />
              </button>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
