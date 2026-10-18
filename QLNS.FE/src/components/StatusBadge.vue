<script setup lang="ts">
import { computed } from 'vue';
import {
  getEmployeeStatusInfo,
  getAttendanceStatusInfo,
  getContractStatusInfo,
  getLeaveStatusInfo,
  getBonusPenaltyLabel,
} from '@/utils/formatters';
import type {
  EmployeeStatus,
  AttendanceStatus,
  ContractStatus,
  LeaveStatus,
  BonusPenaltyType,
} from '@/types';

const props = defineProps<{
  type: 'employee' | 'attendance' | 'contract' | 'leave' | 'bonusPenalty';
  value: number;
}>();

const badgeInfo = computed(() => {
  switch (props.type) {
    case 'employee':
      return getEmployeeStatusInfo(props.value as EmployeeStatus);
    case 'attendance':
      return getAttendanceStatusInfo(props.value as AttendanceStatus);
    case 'contract':
      return getContractStatusInfo(props.value as ContractStatus);
    case 'leave':
      return getLeaveStatusInfo(props.value as LeaveStatus);
    case 'bonusPenalty':
      return getBonusPenaltyLabel(props.value as BonusPenaltyType);
    default:
      return { text: '—', color: 'bg-slate-100 text-slate-600' };
  }
});
</script>

<template>
  <span
    :class="[
      'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border',
      badgeInfo.color,
    ]"
  >
    {{ badgeInfo.text }}
  </span>
</template>
