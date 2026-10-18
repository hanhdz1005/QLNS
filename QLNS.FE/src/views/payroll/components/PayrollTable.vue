<script setup lang="ts">
import type { PayrollDto } from '@/types';
import { formatCurrency } from '@/utils/formatters';
import { useI18n } from '@/locales';
import { FileText } from 'lucide-vue-next';

defineProps<{
  payrollList: PayrollDto[];
  loading: boolean;
}>();

const emit = defineEmits<{
  (e: 'viewPayslip', p: PayrollDto): void;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="overflow-x-auto">
    <table class="w-full text-left border-collapse text-xs">
      <thead>
        <tr class="bg-slate-50/90 border-b border-slate-200/80 text-slate-500 font-semibold uppercase tracking-wider">
          <th class="py-3 px-4">{{ t('l_full_name') }}</th>
          <th class="py-3 px-4">{{ t('l_department') }}</th>
          <th class="py-3 px-4">{{ t('l_basic_salary') }}</th>
          <th class="py-3 px-4">{{ t('l_actual_work_days') }}</th>
          <th class="py-3 px-4">{{ t('l_gross_salary') }}</th>
          <th class="py-3 px-4">{{ t('l_insurance_deduction') }}</th>
          <th class="py-3 px-4">{{ t('l_net_salary') }}</th>
          <th class="py-3 px-4 text-right">{{ t('l_actions') }}</th>
        </tr>
      </thead>
      <tbody class="divide-y divide-slate-100">
        <tr v-if="loading">
          <td colspan="8" class="py-10 text-center text-slate-400">
            {{ t('l_loading_data') }}
          </td>
        </tr>

        <tr v-else-if="payrollList.length === 0">
          <td colspan="8" class="py-10 text-center text-slate-400">
            {{ t('l_no_payroll_for_period') }}
          </td>
        </tr>

        <tr
          v-for="p in payrollList"
          :key="p.employeeId"
          class="hover:bg-slate-50/70 transition-colors"
        >
          <!-- Employee -->
          <td class="py-3 px-4 font-semibold text-slate-800 text-xs sm:text-sm">
            {{ p.employeeName }}
          </td>

          <!-- Department -->
          <td class="py-3 px-4 text-slate-600">
            {{ p.departmentName }}
          </td>

          <!-- Base Salary -->
          <td class="py-3 px-4 font-mono text-slate-700">
            {{ formatCurrency(p.basicSalary) }}
          </td>

          <!-- Work Days -->
          <td class="py-3 px-4 font-mono text-slate-700">
            <div>{{ p.actualWorkDays }}/{{ p.standardWorkingDays }} {{ t('l_days_unit') }}</div>
            <span v-if="p.paidLeaveDays > 0" class="text-indigo-600 block text-xs mt-0.5">
              +{{ p.paidLeaveDays }} {{ t('l_paid_leave_prefix') }}
            </span>
          </td>

          <!-- Gross -->
          <td class="py-3 px-4 font-mono font-semibold text-slate-900">
            {{ formatCurrency(p.grossSalary) }}
          </td>

          <!-- Deductions -->
          <td class="py-3 px-4">
            <div class="text-xs font-mono text-rose-600 font-semibold">
              -{{ formatCurrency((p.insurance || 0) + (p.tax || 0)) }}
            </div>
            <div class="text-[11px] text-slate-400 mt-0.5">
              BH: {{ formatCurrency(p.insurance) }} | Thuế: {{ formatCurrency(p.tax) }}
            </div>
          </td>

          <!-- Net -->
          <td class="py-3 px-4 font-mono font-bold text-emerald-600 text-xs sm:text-sm">
            {{ formatCurrency(p.netSalary) }}
          </td>

          <!-- Action -->
          <td class="py-3 px-4 text-right">
            <button
              @click="emit('viewPayslip', p)"
              :title="t('b_view_payslip')"
              class="inline-flex items-center gap-1.5 px-3 py-1.5 bg-indigo-50 hover:bg-indigo-100 text-indigo-700 font-semibold rounded-lg transition-colors text-xs"
            >
              <FileText class="w-3.5 h-3.5" />
              {{ t('b_view_payslip') }}
            </button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
