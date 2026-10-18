<script setup lang="ts">
import type { EmployeeStatsDto } from '@/types';
import { useI18n } from '@/locales';
import { Building2 } from 'lucide-vue-next';

defineProps<{
  stats: EmployeeStatsDto[];
  totalEmployees: number;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="bg-white p-5 rounded-2xl border border-slate-200/80 shadow-xs">
    <div class="flex items-center justify-between mb-4">
      <div>
        <h2 class="text-sm sm:text-base font-bold text-slate-800">{{ t('l_staff_distribution') }}</h2>
        <p class="text-xs text-slate-500 mt-0.5">{{ t('l_staff_distribution_desc') }}</p>
      </div>
      <router-link to="/departments" class="text-xs font-semibold text-indigo-600 hover:text-indigo-800">
        {{ t('l_view_all') }} &rarr;
      </router-link>
    </div>

    <div v-if="stats.length === 0" class="text-center py-6 text-slate-400 text-xs">
      {{ t('l_no_dept_stats') }}
    </div>

    <div v-else class="space-y-3">
      <div v-for="dept in stats" :key="dept.departmentId" class="space-y-1.5">
        <div class="flex justify-between text-xs sm:text-sm">
          <span class="font-medium text-slate-700 flex items-center gap-2 truncate">
            <Building2 class="w-4 h-4 text-slate-400 shrink-0" />
            {{ dept.departmentName }}
          </span>
          <span class="text-slate-500 font-mono text-xs shrink-0">
            {{ dept.employeeCount }} {{ t('l_people_count_unit') }} ({{ totalEmployees > 0 ? Math.round((dept.employeeCount / totalEmployees) * 100) : 0 }}%)
          </span>
        </div>
        <div class="w-full h-2 bg-slate-100 rounded-full overflow-hidden">
          <div
            class="h-full bg-gradient-to-r from-indigo-500 to-indigo-600 rounded-full transition-all duration-500"
            :style="{ width: `${totalEmployees > 0 ? (dept.employeeCount / totalEmployees) * 100 : 0}%` }"
          />
        </div>
      </div>
    </div>
  </div>
</template>
