<script setup lang="ts">
import type { PayrollDto } from '@/types';
import { formatCurrency } from '@/utils/formatters';
import { useI18n } from '@/locales';
import { Calendar } from 'lucide-vue-next';

defineProps<{
  myPayslip: PayrollDto | null;
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
  <div class="space-y-4 max-w-xl mx-auto text-xs">
    <!-- Month / Year Selector -->
    <div class="bg-white p-3 sm:p-4 rounded-2xl border border-slate-200/80 shadow-xs flex items-center justify-between gap-3 shrink-0">
      <div class="flex items-center gap-2">
        <Calendar class="w-4 h-4 text-indigo-600" />
        <h3 class="font-bold text-slate-800 text-xs sm:text-sm">
          {{ t('l_tab_my_payslip') }}
        </h3>
      </div>

      <div class="flex items-center gap-2.5">
        <select
          :value="month"
          @change="emit('update:month', Number(($event.target as HTMLSelectElement).value)); emit('reload')"
          class="py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm text-slate-700 font-semibold focus:outline-hidden focus:border-indigo-500"
        >
          <option v-for="m in 12" :key="m" :value="m">{{ t('l_month_prefix') }} {{ m }}</option>
        </select>

        <select
          :value="year"
          @change="emit('update:year', Number(($event.target as HTMLSelectElement).value)); emit('reload')"
          class="py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm text-slate-700 font-semibold focus:outline-hidden focus:border-indigo-500"
        >
          <option v-for="y in [2025, 2026, 2027]" :key="y" :value="y">{{ t('l_year_prefix') }} {{ y }}</option>
        </select>
      </div>
    </div>

    <!-- Payslip Content Card -->
    <div v-if="loading" class="bg-white p-8 text-center text-slate-400 rounded-2xl border border-slate-200/80">
      {{ t('l_loading_my_payslip') }}
    </div>

    <div v-else-if="!myPayslip" class="bg-white p-8 text-center text-slate-400 rounded-2xl border border-slate-200/80">
      {{ t('l_no_my_payslip') }}
    </div>

    <div v-else class="bg-white rounded-2xl border border-slate-200/80 shadow-xs p-5 sm:p-6 space-y-4 text-xs">
      <!-- Header -->
      <div class="flex items-center justify-between border-b border-slate-100 pb-3">
        <div>
          <span class="text-[10px] font-bold text-slate-400 uppercase tracking-wider block">
            {{ t('l_payslip_staff_title') }}
          </span>
          <h2 class="text-base font-bold text-slate-800">{{ myPayslip.employeeName }}</h2>
          <span class="text-slate-500 text-[11px]">{{ myPayslip.departmentName }}</span>
        </div>
        <div class="text-right">
          <span class="text-[10px] text-slate-400 block">{{ t('l_payment_period') }}</span>
          <span class="font-bold text-indigo-600 font-mono text-sm sm:text-base">
            {{ t('l_month_prefix') }} {{ myPayslip.month }}/{{ myPayslip.year }}
          </span>
        </div>
      </div>

      <!-- Itemized Breakdown -->
      <div class="space-y-3">
        <!-- Earnings -->
        <div class="space-y-1.5">
          <span class="font-bold text-slate-700 uppercase tracking-wider text-[10px] block border-b pb-0.5">
            {{ t('l_earnings_section') }}
          </span>
          <div class="flex justify-between"><span class="text-slate-500">{{ t('l_base_contract_salary') }}:</span> <span class="font-mono">{{ formatCurrency(myPayslip.basicSalary) }}</span></div>
          <div class="flex justify-between"><span class="text-slate-500">{{ t('l_actual_work_salary') }} ({{ myPayslip.actualWorkDays }}/{{ myPayslip.standardWorkingDays }} {{ t('l_days_unit') }}):</span> <span class="font-mono font-medium">{{ formatCurrency(myPayslip.actualBasicSalary) }}</span></div>
          <div class="flex justify-between"><span class="text-slate-500">{{ t('l_allowance') }}:</span> <span class="font-mono text-emerald-600">+{{ formatCurrency(myPayslip.allowance) }}</span></div>
          <div class="flex justify-between"><span class="text-slate-500">{{ t('l_overtime_pay_label') }} ({{ myPayslip.overtimeHours || 0 }}h):</span> <span class="font-mono text-emerald-600">+{{ formatCurrency(myPayslip.overtimePay) }}</span></div>
          <div class="flex justify-between"><span class="text-slate-500">{{ t('l_bonus') }}:</span> <span class="font-mono text-emerald-600">+{{ formatCurrency(myPayslip.totalBonus) }}</span></div>
          <div class="flex justify-between border-t pt-1 font-bold"><span class="text-slate-800">{{ t('l_gross_salary') }}:</span> <span class="font-mono text-indigo-700">{{ formatCurrency(myPayslip.grossSalary) }}</span></div>
        </div>

        <!-- Deductions -->
        <div class="space-y-1.5">
          <span class="font-bold text-slate-700 uppercase tracking-wider text-[10px] block border-b pb-0.5">
            {{ t('l_deductions_section') }}
          </span>
          <div class="flex justify-between"><span class="text-slate-500">{{ t('l_insurance_deduction') }}:</span> <span class="font-mono text-rose-600">-{{ formatCurrency(myPayslip.insurance) }}</span></div>
          <div class="flex justify-between"><span class="text-slate-500">{{ t('l_tax_deduction') }}:</span> <span class="font-mono text-rose-600">-{{ formatCurrency(myPayslip.tax) }}</span></div>
          <div class="flex justify-between"><span class="text-slate-500">{{ t('l_penalty') }}:</span> <span class="font-mono text-rose-600">-{{ formatCurrency(myPayslip.totalPenalty) }}</span></div>
        </div>

        <!-- Take Home -->
        <div class="p-3 rounded-xl bg-emerald-50 border border-emerald-200 flex items-center justify-between">
          <div>
            <span class="text-[11px] font-bold text-emerald-800 uppercase tracking-wider block">
              {{ t('l_take_home_pay') }}
            </span>
            <div class="text-[10px] text-emerald-600">
              {{ t('l_take_home_note') }}
            </div>
          </div>
          <span class="text-lg font-bold text-emerald-700 font-mono">
            {{ formatCurrency(myPayslip.netSalary) }}
          </span>
        </div>
      </div>
    </div>
  </div>
</template>
