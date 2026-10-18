<script setup lang="ts">
import { formatCurrency } from '@/utils/formatters';
import { useI18n } from '@/locales';
import { DollarSign, TrendingUp, Receipt, Percent } from 'lucide-vue-next';

defineProps<{
  totalGrossSum: number;
  totalNetSum: number;
  totalTaxSum: number;
  totalInsuranceSum: number;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 sm:gap-5 shrink-0">
    <!-- Total Gross -->
    <div class="bg-white p-4 sm:p-5 rounded-2xl border border-slate-200/80 shadow-xs flex items-center gap-4">
      <div class="w-10 h-10 rounded-xl bg-indigo-50 text-indigo-600 flex items-center justify-center shrink-0 border border-indigo-100/60 shadow-2xs">
        <DollarSign class="w-5 h-5" />
      </div>
      <div>
        <span class="text-xs font-semibold text-slate-500 uppercase tracking-wider block">
          {{ t('l_total_gross_fund') }}
        </span>
        <div class="text-sm sm:text-base font-bold text-slate-800 font-mono mt-0.5">{{ formatCurrency(totalGrossSum) }}</div>
      </div>
    </div>

    <!-- Total Net -->
    <div class="bg-white p-4 sm:p-5 rounded-2xl border border-slate-200/80 shadow-xs flex items-center gap-4">
      <div class="w-10 h-10 rounded-xl bg-emerald-50 text-emerald-600 flex items-center justify-center shrink-0 border border-emerald-100/60 shadow-2xs">
        <TrendingUp class="w-5 h-5" />
      </div>
      <div>
        <span class="text-xs font-semibold text-slate-500 uppercase tracking-wider block">
          {{ t('l_net_employee_payout') }}
        </span>
        <div class="text-sm sm:text-base font-bold text-emerald-600 font-mono mt-0.5">{{ formatCurrency(totalNetSum) }}</div>
      </div>
    </div>

    <!-- Total Tax -->
    <div class="bg-white p-4 sm:p-5 rounded-2xl border border-slate-200/80 shadow-xs flex items-center gap-4">
      <div class="w-10 h-10 rounded-xl bg-amber-50 text-amber-600 flex items-center justify-center shrink-0 border border-amber-100/60 shadow-2xs">
        <Receipt class="w-5 h-5" />
      </div>
      <div>
        <span class="text-xs font-semibold text-slate-500 uppercase tracking-wider block">
          {{ t('l_total_tax_deductions') }}
        </span>
        <div class="text-sm sm:text-base font-bold text-amber-600 font-mono mt-0.5">{{ formatCurrency(totalTaxSum) }}</div>
      </div>
    </div>

    <!-- Total Insurance -->
    <div class="bg-white p-4 sm:p-5 rounded-2xl border border-slate-200/80 shadow-xs flex items-center gap-4">
      <div class="w-10 h-10 rounded-xl bg-blue-50 text-blue-600 flex items-center justify-center shrink-0 border border-blue-100/60 shadow-2xs">
        <Percent class="w-5 h-5" />
      </div>
      <div>
        <span class="text-xs font-semibold text-slate-500 uppercase tracking-wider block">
          {{ t('l_total_insurance_deductions') }}
        </span>
        <div class="text-sm sm:text-base font-bold text-blue-600 font-mono mt-0.5">{{ formatCurrency(totalInsuranceSum) }}</div>
      </div>
    </div>
  </div>
</template>
