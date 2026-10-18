<script setup lang="ts">
import type { ContractDto } from '@/types';
import { formatDate, formatCurrency, getContractTypeLabel } from '@/utils/formatters';
import { useI18n } from '@/locales';
import StatusBadge from '@/components/StatusBadge.vue';
import { ExternalLink, Edit, Trash2 } from 'lucide-vue-next';

defineProps<{
  contracts: ContractDto[];
  loading: boolean;
  isAdmin: boolean;
}>();

const emit = defineEmits<{
  (e: 'edit', contract: ContractDto): void;
  (e: 'delete', id: string): void;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="overflow-x-auto">
    <table class="w-full text-left border-collapse text-xs">
      <thead>
        <tr class="bg-slate-50/90 border-b border-slate-200/80 text-slate-500 font-semibold uppercase tracking-wider">
          <th class="py-3 px-4">{{ t('l_contract_and_employee') }}</th>
          <th class="py-3 px-4">{{ t('l_contract_type') }}</th>
          <th class="py-3 px-4">{{ t('l_contract_duration') }}</th>
          <th class="py-3 px-4">{{ t('l_contract_salary') }}</th>
          <th class="py-3 px-4">{{ t('l_attachment') }}</th>
          <th class="py-3 px-4">{{ t('l_status') }}</th>
          <th v-if="isAdmin" class="py-3 px-4 text-right">{{ t('l_actions') }}</th>
        </tr>
      </thead>
      <tbody class="divide-y divide-slate-100">
        <tr v-if="loading">
          <td :colspan="isAdmin ? 7 : 6" class="py-10 text-center text-slate-400">
            {{ t('l_loading_data') }}
          </td>
        </tr>

        <tr v-else-if="contracts.length === 0">
          <td :colspan="isAdmin ? 7 : 6" class="py-10 text-center text-slate-400">
            {{ t('l_no_data') }}
          </td>
        </tr>

        <tr
          v-for="c in contracts"
          :key="c.id"
          class="hover:bg-slate-50/70 transition-colors"
        >
          <!-- Contract Number & Employee -->
          <td class="py-3 px-4">
            <div class="font-bold text-slate-800 font-mono text-xs sm:text-sm">{{ c.contractNumber }}</div>
            <div class="text-xs text-indigo-600 font-medium mt-0.5">{{ c.employeeName }}</div>
          </td>

          <!-- Contract Type -->
          <td class="py-3 px-4 text-slate-700">
            <span class="inline-flex items-center px-2 py-0.5 rounded-md bg-indigo-50 text-indigo-700 text-xs font-semibold border border-indigo-100">
              {{ getContractTypeLabel(c.contractType) }}
            </span>
          </td>

          <!-- Duration -->
          <td class="py-3 px-4 text-slate-700">
            <div class="text-xs">{{ formatDate(c.startDate) }} → {{ c.endDate ? formatDate(c.endDate) : t('l_indefinite') }}</div>
            <div class="text-[11px] text-slate-400 mt-0.5">{{ t('l_sign_date') }}: {{ formatDate(c.signDate) }}</div>
          </td>

          <!-- Salary -->
          <td class="py-3 px-4">
            <div class="font-bold text-slate-800 font-mono text-xs sm:text-sm">{{ formatCurrency(c.contractSalary) }}</div>
            <div class="text-[11px] text-slate-400 font-mono mt-0.5">BHXH: {{ formatCurrency(c.insuranceSalary) }}</div>
          </td>

          <!-- Attachment -->
          <td class="py-3 px-4">
            <a
              v-if="c.attachmentUrl"
              :href="c.attachmentUrl"
              target="_blank"
              class="inline-flex items-center gap-1.5 text-indigo-600 hover:text-indigo-800 hover:underline font-medium text-xs"
            >
              <ExternalLink class="w-3.5 h-3.5" /> {{ t('b_download_scan') }}
            </a>
            <span v-else class="text-slate-400">—</span>
          </td>

          <!-- Status -->
          <td class="py-3 px-4">
            <StatusBadge type="contract" :value="c.status" />
          </td>

          <!-- Actions -->
          <td v-if="isAdmin" class="py-3 px-4 text-right">
            <div class="inline-flex items-center gap-1">
              <button
                @click="emit('edit', c)"
                :title="t('b_edit')"
                class="p-1.5 text-slate-500 hover:text-amber-600 hover:bg-amber-50 rounded-lg transition-colors"
              >
                <Edit class="w-4 h-4" />
              </button>
              <button
                @click="emit('delete', c.id)"
                :title="t('b_delete')"
                class="p-1.5 text-slate-500 hover:text-rose-600 hover:bg-rose-50 rounded-lg transition-colors"
              >
                <Trash2 class="w-4 h-4" />
              </button>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
