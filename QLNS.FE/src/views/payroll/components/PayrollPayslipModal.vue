<script setup lang="ts">
import type { PayrollDto } from '@/types';
import { formatCurrency } from '@/utils/formatters';
import { useI18n } from '@/locales';
import BaseModal from '@/components/BaseModal.vue';

defineProps<{
  modelValue: boolean;
  payslip: PayrollDto | null;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', val: boolean): void;
}>();

const { t } = useI18n();
</script>

<template>
  <BaseModal
    :model-value="modelValue"
    @update:model-value="emit('update:modelValue', $event)"
    max-width="lg"
    :title="t('l_payslip_modal_title')"
  >
    <div v-if="payslip" class="space-y-4 text-xs">
      <!-- Employee Banner -->
      <div class="p-3.5 rounded-2xl bg-slate-50 border border-slate-200/80 flex items-center justify-between">
        <div>
          <h3 class="font-bold text-slate-800 text-sm">{{ payslip.employeeName }}</h3>
          <span class="text-slate-500 text-[11px]">{{ payslip.departmentName }}</span>
        </div>
        <div class="text-right">
          <span class="text-[11px] text-slate-400 block">{{ t('l_payment_period') }}</span>
          <span class="font-bold text-indigo-600 font-mono text-sm">{{ t('l_month_prefix') }} {{ payslip.month }}/{{ payslip.year }}</span>
        </div>
      </div>

      <!-- Itemized Breakdown -->
      <div class="space-y-2">
        <!-- Earnings Section -->
        <div class="p-3 rounded-xl border border-slate-100 bg-slate-50/50 space-y-2">
          <span class="font-bold text-slate-700 uppercase tracking-wider text-[11px] block border-b pb-1">
            {{ t('l_earnings_section') }}
          </span>
          <div class="flex justify-between"><span class="text-slate-500">{{ t('l_base_contract_salary') }}:</span> <span class="font-mono">{{ formatCurrency(payslip.basicSalary) }}</span></div>
          <div class="flex justify-between"><span class="text-slate-500">{{ t('l_actual_work_salary') }} ({{ payslip.actualWorkDays }}/{{ payslip.standardWorkingDays }} {{ t('l_days_unit') }}):</span> <span class="font-mono font-medium">{{ formatCurrency(payslip.actualBasicSalary) }}</span></div>
          <div class="flex justify-between"><span class="text-slate-500">{{ t('l_allowance') }}:</span> <span class="font-mono text-emerald-600">+{{ formatCurrency(payslip.allowance) }}</span></div>
          <div class="flex justify-between"><span class="text-slate-500">{{ t('l_overtime_pay_label') }} ({{ payslip.overtimeHours || 0 }}h):</span> <span class="font-mono text-emerald-600">+{{ formatCurrency(payslip.overtimePay) }}</span></div>
          <div class="flex justify-between"><span class="text-slate-500">{{ t('l_bonus') }}:</span> <span class="font-mono text-emerald-600">+{{ formatCurrency(payslip.totalBonus) }}</span></div>
          <div class="flex justify-between border-t pt-1 font-bold"><span class="text-slate-800">{{ t('l_gross_salary') }}:</span> <span class="font-mono text-indigo-700">{{ formatCurrency(payslip.grossSalary) }}</span></div>
        </div>

        <!-- Deductions Section -->
        <div class="p-3 rounded-xl border border-slate-100 bg-slate-50/50 space-y-2">
          <span class="font-bold text-slate-700 uppercase tracking-wider text-[11px] block border-b pb-1">
            {{ t('l_deductions_section') }}
          </span>
          <div class="flex justify-between"><span class="text-slate-500">{{ t('l_insurance_deduction') }}:</span> <span class="font-mono text-rose-600">-{{ formatCurrency(payslip.insurance) }}</span></div>
          <div class="flex justify-between"><span class="text-slate-500">{{ t('l_taxable_income') }}:</span> <span class="font-mono">{{ formatCurrency(payslip.taxableIncome) }}</span></div>
          <div class="flex justify-between"><span class="text-slate-500">{{ t('l_tax_deduction') }}:</span> <span class="font-mono text-rose-600">-{{ formatCurrency(payslip.tax) }}</span></div>
          <div class="flex justify-between"><span class="text-slate-500">{{ t('l_penalty') }}:</span> <span class="font-mono text-rose-600">-{{ formatCurrency(payslip.totalPenalty) }}</span></div>
        </div>

        <!-- Net Take-Home Pay -->
        <div class="p-4 rounded-2xl bg-emerald-50 border border-emerald-200 flex items-center justify-between">
          <div>
            <span class="text-[11px] font-bold text-emerald-800 uppercase tracking-wider">{{ t('l_take_home_pay') }}</span>
            <div class="text-[10px] text-emerald-600">{{ t('l_take_home_note') }}</div>
          </div>
          <span class="text-xl font-bold text-emerald-700 font-mono">
            {{ formatCurrency(payslip.netSalary) }}
          </span>
        </div>
      </div>
    </div>

    <template #footer>
      <button
        type="button"
        @click="emit('update:modelValue', false)"
        class="px-5 py-2 bg-slate-100 hover:bg-slate-200 text-slate-700 rounded-xl font-medium text-xs transition-colors"
      >
        {{ t('l_close_payslip') }}
      </button>
    </template>
  </BaseModal>
</template>
