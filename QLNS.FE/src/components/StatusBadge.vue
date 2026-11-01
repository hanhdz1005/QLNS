<script setup lang="ts">
import { computed } from 'vue';
import {
  getEmployeeStatusInfo,
  getAttendanceStatusInfo,
  getContractStatusInfo,
  getLeaveStatusInfo,
  getBonusPenaltyLabel,
} from '@/utils/formatters';

const props = defineProps<{
  type: 'employee' | 'attendance' | 'contract' | 'leave' | 'bonusPenalty';
  value: string | number;
}>();

const badgeInfo = computed(() => {
  switch (props.type) {
    case 'employee':
      return getEmployeeStatusInfo(props.value);
    case 'attendance':
      return getAttendanceStatusInfo(props.value);
    case 'contract':
      return getContractStatusInfo(props.value);
    case 'leave':
      return getLeaveStatusInfo(props.value);
    case 'bonusPenalty':
      return getBonusPenaltyLabel(props.value);
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
