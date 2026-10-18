<script setup lang="ts">
import type { EmployeeDto } from '@/types';
import { formatDate, getGenderLabel } from '@/utils/formatters';
import { useI18n } from '@/locales';
import BaseModal from '@/components/BaseModal.vue';
import StatusBadge from '@/components/StatusBadge.vue';

defineProps<{
  modelValue: boolean;
  employee: EmployeeDto | null;
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
    max-width="2xl"
    :title="t('l_employee_detail_modal_title')"
  >
    <div v-if="employee" class="space-y-6 text-xs">
      <!-- Profile Banner -->
      <div class="flex items-center gap-4 p-4 rounded-2xl bg-indigo-50/70 border border-indigo-100">
        <div class="w-14 h-14 rounded-2xl bg-indigo-600 text-white font-bold text-xl flex items-center justify-center shrink-0 shadow-xs">
          {{ employee.fullName.charAt(0) }}
        </div>
        <div>
          <h2 class="text-base font-bold text-slate-800">{{ employee.fullName }}</h2>
          <div class="flex items-center gap-2 mt-0.5">
            <span class="font-mono text-indigo-700 font-semibold">{{ employee.code }}</span>
            <span class="text-slate-300">•</span>
            <StatusBadge type="employee" :value="employee.status" />
          </div>
          <div class="text-slate-500 mt-1">
            {{ employee.positionName }} — {{ employee.departmentName }}
          </div>
        </div>
      </div>

      <!-- 4 Grid Sections -->
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <!-- General Info -->
        <div class="p-3.5 rounded-xl border border-slate-100 bg-slate-50/50 space-y-2">
          <span class="font-bold text-slate-700 uppercase tracking-wider text-[11px] block border-b border-slate-200/60 pb-1">
            {{ t('l_tab_general') }}
          </span>
          <div><span class="text-slate-400">{{ t('l_gender') }}:</span> {{ getGenderLabel(employee.gender) }}</div>
          <div><span class="text-slate-400">{{ t('l_date_of_birth') }}:</span> {{ formatDate(employee.dateOfBirth) }}</div>
          <div><span class="text-slate-400">{{ t('l_email') }}:</span> {{ employee.email }}</div>
          <div><span class="text-slate-400">{{ t('l_phone_number') }}:</span> {{ employee.phoneNumber }}</div>
          <div><span class="text-slate-400">{{ t('l_address') }}:</span> {{ employee.address || '—' }}</div>
          <div><span class="text-slate-400">{{ t('l_join_date') }}:</span> {{ formatDate(employee.joinDate) }}</div>
          <div><span class="text-slate-400">{{ t('l_manager') }}:</span> {{ employee.managerName || '—' }}</div>
        </div>

        <!-- Identity, Tax & Social Insurance -->
        <div class="p-3.5 rounded-xl border border-slate-100 bg-slate-50/50 space-y-2">
          <span class="font-bold text-slate-700 uppercase tracking-wider text-[11px] block border-b border-slate-200/60 pb-1">
            {{ t('l_tab_identity') }}
          </span>
          <div><span class="text-slate-400">{{ t('l_citizen_id') }}:</span> {{ employee.citizenId || '—' }}</div>
          <div><span class="text-slate-400">{{ t('l_citizen_id_issue_date') }}:</span> {{ employee.citizenIdIssueDate ? formatDate(employee.citizenIdIssueDate) : '—' }}</div>
          <div><span class="text-slate-400">{{ t('l_citizen_id_issue_place') }}:</span> {{ employee.citizenIdIssuePlace || '—' }}</div>
          <div><span class="text-slate-400">{{ t('l_tax_code') }}:</span> {{ employee.taxCode || '—' }}</div>
          <div><span class="text-slate-400">{{ t('l_social_insurance_number') }}:</span> {{ employee.socialInsuranceNumber || '—' }}</div>
          <div><span class="text-slate-400">{{ t('l_dependents_count') }}:</span> {{ employee.dependentsCount }} {{ t('l_people_count_unit') }}</div>
        </div>

        <!-- Banking Information -->
        <div class="p-3.5 rounded-xl border border-slate-100 bg-slate-50/50 space-y-2">
          <span class="font-bold text-slate-700 uppercase tracking-wider text-[11px] block border-b border-slate-200/60 pb-1">
            {{ t('l_tab_bank') }}
          </span>
          <div><span class="text-slate-400">{{ t('l_bank_account_number') }}:</span> {{ employee.bankAccountNumber || '—' }}</div>
          <div><span class="text-slate-400">{{ t('l_bank_name') }}:</span> {{ employee.bankName || '—' }}</div>
          <div><span class="text-slate-400">{{ t('l_bank_branch') }}:</span> {{ employee.bankBranch || '—' }}</div>
        </div>

        <!-- Emergency Contact -->
        <div class="p-3.5 rounded-xl border border-slate-100 bg-slate-50/50 space-y-2">
          <span class="font-bold text-slate-700 uppercase tracking-wider text-[11px] block border-b border-slate-200/60 pb-1">
            {{ t('l_tab_emergency') }}
          </span>
          <div><span class="text-slate-400">{{ t('l_emergency_contact_name') }}:</span> {{ employee.emergencyContactName || '—' }}</div>
          <div><span class="text-slate-400">{{ t('l_emergency_contact_phone') }}:</span> {{ employee.emergencyContactPhone || '—' }}</div>
          <div><span class="text-slate-400">{{ t('l_emergency_contact_relationship') }}:</span> {{ employee.emergencyContactRelationship || '—' }}</div>
        </div>
      </div>
    </div>

    <template #footer>
      <button
        type="button"
        @click="emit('update:modelValue', false)"
        class="px-5 py-2 bg-slate-100 hover:bg-slate-200 text-slate-700 rounded-xl font-medium text-xs transition-colors"
      >
        {{ t('b_close') }}
      </button>
    </template>
  </BaseModal>
</template>
