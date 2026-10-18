<script setup lang="ts">
import type { LeaveRequestDto } from '@/types';
import { formatDate, getLeaveTypeLabel } from '@/utils/formatters';
import { useI18n } from '@/locales';
import { CheckCircle2, XCircle } from 'lucide-vue-next';

defineProps<{
  pendingApprovals: LeaveRequestDto[];
  loading: boolean;
}>();

const emit = defineEmits<{
  (e: 'approve', id: string): void;
  (e: 'reject', id: string): void;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="overflow-x-auto">
    <table class="w-full text-left border-collapse text-xs">
      <thead>
        <tr class="bg-slate-50/90 border-b border-slate-200/80 text-slate-500 font-semibold uppercase tracking-wider">
          <th class="py-3 px-4">{{ t('l_applicant') }}</th>
          <th class="py-3 px-4">{{ t('l_leave_type') }}</th>
          <th class="py-3 px-4">{{ t('l_leave_duration') }}</th>
          <th class="py-3 px-4">{{ t('l_leave_reason') }}</th>
          <th class="py-3 px-4 text-right">{{ t('l_actions') }}</th>
        </tr>
      </thead>
      <tbody class="divide-y divide-slate-100">
        <tr v-if="loading">
          <td colspan="5" class="py-10 text-center text-slate-400">
            {{ t('l_loading_data') }}
          </td>
        </tr>

        <tr v-else-if="pendingApprovals.length === 0">
          <td colspan="5" class="py-10 text-center text-slate-400">
            {{ t('l_no_pending_approvals') }}
          </td>
        </tr>

        <tr
          v-for="r in pendingApprovals"
          :key="r.id"
          class="hover:bg-slate-50/70 transition-colors"
        >
          <!-- Employee -->
          <td class="py-3 px-4">
            <div class="font-semibold text-slate-800 text-xs sm:text-sm">{{ r.employeeName }}</div>
            <div class="text-[11px] text-slate-400 mt-0.5">{{ t('l_sent_date') }}: {{ formatDate(r.createdDate) }}</div>
          </td>

          <!-- Type -->
          <td class="py-3 px-4 font-medium text-slate-700">
            <span class="inline-flex items-center px-2 py-0.5 rounded-md bg-indigo-50 text-indigo-700 text-xs font-semibold border border-indigo-100">
              {{ getLeaveTypeLabel(r.leaveType) }}
            </span>
          </td>

          <!-- Duration -->
          <td class="py-3 px-4 text-slate-700 font-medium">
            <div class="text-xs">{{ formatDate(r.fromDate) }} → {{ formatDate(r.toDate) }}</div>
          </td>

          <!-- Reason -->
          <td class="py-3 px-4 text-slate-600 max-w-sm truncate">
            {{ r.reason }}
          </td>

          <!-- Actions -->
          <td class="py-3 px-4 text-right">
            <div class="inline-flex items-center gap-2">
              <button
                @click="emit('approve', r.id)"
                class="inline-flex items-center gap-1.5 px-3 py-1.5 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg font-semibold text-xs shadow-xs transition-colors"
              >
                <CheckCircle2 class="w-4 h-4" />
                {{ t('b_approve') }}
              </button>

              <button
                @click="emit('reject', r.id)"
                class="inline-flex items-center gap-1.5 px-3 py-1.5 bg-rose-50 hover:bg-rose-100 text-rose-700 border border-rose-200 rounded-lg font-semibold text-xs transition-colors"
              >
                <XCircle class="w-4 h-4" />
                {{ t('b_reject') }}
              </button>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
