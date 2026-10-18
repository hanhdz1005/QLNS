<script setup lang="ts">
import type { LeaveRequestDto } from '@/types';
import { formatDate, getLeaveTypeLabel } from '@/utils/formatters';
import { useI18n } from '@/locales';
import StatusBadge from '@/components/StatusBadge.vue';

defineProps<{
  requests: LeaveRequestDto[];
  loading: boolean;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="overflow-x-auto">
    <table class="w-full text-left border-collapse text-xs">
      <thead>
        <tr class="bg-slate-50/90 border-b border-slate-200/80 text-slate-500 font-semibold uppercase tracking-wider">
          <th class="py-3 px-4">{{ t('l_full_name') }}</th>
          <th class="py-3 px-4">{{ t('l_leave_type') }}</th>
          <th class="py-3 px-4">{{ t('l_leave_duration') }}</th>
          <th class="py-3 px-4">{{ t('l_leave_reason') }}</th>
          <th class="py-3 px-4">{{ t('l_created_date') }}</th>
          <th class="py-3 px-4 text-right">{{ t('l_status') }}</th>
        </tr>
      </thead>
      <tbody class="divide-y divide-slate-100">
        <tr v-if="loading">
          <td colspan="6" class="py-10 text-center text-slate-400">
            {{ t('l_loading_data') }}
          </td>
        </tr>

        <tr v-else-if="requests.length === 0">
          <td colspan="6" class="py-10 text-center text-slate-400">
            {{ t('l_no_data') }}
          </td>
        </tr>

        <tr
          v-for="r in requests"
          :key="r.id"
          class="hover:bg-slate-50/70 transition-colors"
        >
          <!-- Employee -->
          <td class="py-3 px-4">
            <div class="font-semibold text-slate-800 text-xs sm:text-sm">{{ r.employeeName }}</div>
          </td>

          <!-- Leave Type -->
          <td class="py-3 px-4 font-medium text-slate-700">
            <span class="inline-flex items-center px-2 py-0.5 rounded-md bg-indigo-50 text-indigo-700 text-xs font-semibold border border-indigo-100">
              {{ getLeaveTypeLabel(r.leaveType) }}
            </span>
          </td>

          <!-- Date Range -->
          <td class="py-3 px-4 text-slate-700">
            <div class="font-medium text-slate-800 text-xs">
              {{ formatDate(r.fromDate) }} → {{ formatDate(r.toDate) }}
            </div>
          </td>

          <!-- Reason -->
          <td class="py-3 px-4 text-slate-600 max-w-xs truncate">
            {{ r.reason }}
          </td>

          <!-- Created Date -->
          <td class="py-3 px-4 text-slate-500">
            {{ formatDate(r.createdDate) }}
          </td>

          <!-- Status -->
          <td class="py-3 px-4 text-right">
            <StatusBadge type="leave" :value="r.status" />
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
