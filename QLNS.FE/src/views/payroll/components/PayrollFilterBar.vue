<script setup lang="ts">
import { useI18n } from '@/locales';
import { Calculator, Download } from 'lucide-vue-next';

defineProps<{
  month: number;
  year: number;
  isAdmin: boolean;
}>();

const emit = defineEmits<{
  (e: 'update:month', val: number): void;
  (e: 'update:year', val: number): void;
  (e: 'openGenerate'): void;
  (e: 'exportCsv'): void;
  (e: 'reload'): void;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="bg-white p-3 sm:p-4 rounded-2xl border border-slate-200/80 shadow-xs flex flex-wrap items-center justify-between gap-3 shrink-0">
    <div class="flex items-center gap-2.5">
      <span class="text-xs sm:text-sm font-semibold text-slate-700">{{ t('l_payroll_period') }}:</span>
      <select
        :value="month"
        @change="emit('update:month', Number(($event.target as HTMLSelectElement).value)); emit('reload')"
        class="py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm text-slate-800 font-semibold focus:outline-hidden focus:border-indigo-500"
      >
        <option v-for="m in 12" :key="m" :value="m">{{ t('l_month_prefix') }} {{ m }}</option>
      </select>

      <select
        :value="year"
        @change="emit('update:year', Number(($event.target as HTMLSelectElement).value)); emit('reload')"
        class="py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm text-slate-800 font-semibold focus:outline-hidden focus:border-indigo-500"
      >
        <option v-for="y in [2025, 2026, 2027]" :key="y" :value="y">{{ t('l_year_prefix') }} {{ y }}</option>
      </select>
    </div>

    <div class="flex items-center gap-2.5">
      <button
        @click="emit('exportCsv')"
        class="inline-flex items-center gap-2 px-3.5 py-2 bg-white hover:bg-slate-50 text-slate-700 border border-slate-200 rounded-xl text-xs sm:text-sm font-semibold shadow-2xs transition-colors"
      >
        <Download class="w-4 h-4 text-slate-500" />
        {{ t('l_export_salary_csv') }}
      </button>

      <button
        v-if="isAdmin"
        @click="emit('openGenerate')"
        class="inline-flex items-center gap-2 px-3.5 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-xs sm:text-sm font-semibold shadow-md shadow-indigo-600/20 transition-colors"
      >
        <Calculator class="w-4 h-4" />
        {{ t('b_generate_payroll') }}
      </button>
    </div>
  </div>
</template>
