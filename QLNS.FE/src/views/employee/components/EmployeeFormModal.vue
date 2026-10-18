<script setup lang="ts">
import { ref } from 'vue';
import type { AddEmployeeDto, DepartmentDto, PositionDto, EmployeeDto } from '@/types';
import { Gender, EmployeeStatus } from '@/types';
import { useI18n } from '@/locales';
import BaseModal from '@/components/BaseModal.vue';

const props = defineProps<{
  modelValue: boolean;
  isEditing: boolean;
  editingId: string | null;
  formData: AddEmployeeDto;
  departments: DepartmentDto[];
  positions: PositionDto[];
  employees: EmployeeDto[];
  loading: boolean;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', val: boolean): void;
  (e: 'submit'): void;
}>();

const { t } = useI18n();
const formTab = ref<'general' | 'identity' | 'bank' | 'emergency'>('general');
</script>

<template>
  <BaseModal
    :model-value="modelValue"
    @update:model-value="emit('update:modelValue', $event)"
    max-width="3xl"
    :title="isEditing ? t('l_edit_employee_modal_title') : t('l_add_employee_modal_title')"
  >
    <!-- Sub-tabs for Form fields -->
    <div class="flex items-center border-b border-slate-200 mb-5 gap-2 text-xs font-semibold">
      <button
        type="button"
        @click="formTab = 'general'"
        :class="[
          'pb-2 px-3 border-b-2 transition-colors',
          formTab === 'general' ? 'border-indigo-600 text-indigo-600' : 'border-transparent text-slate-500 hover:text-slate-700',
        ]"
      >
        1. {{ t('l_tab_general') }}
      </button>
      <button
        type="button"
        @click="formTab = 'identity'"
        :class="[
          'pb-2 px-3 border-b-2 transition-colors',
          formTab === 'identity' ? 'border-indigo-600 text-indigo-600' : 'border-transparent text-slate-500 hover:text-slate-700',
        ]"
      >
        2. {{ t('l_tab_identity') }}
      </button>
      <button
        type="button"
        @click="formTab = 'bank'"
        :class="[
          'pb-2 px-3 border-b-2 transition-colors',
          formTab === 'bank' ? 'border-indigo-600 text-indigo-600' : 'border-transparent text-slate-500 hover:text-slate-700',
        ]"
      >
        3. {{ t('l_tab_bank') }}
      </button>
      <button
        type="button"
        @click="formTab = 'emergency'"
        :class="[
          'pb-2 px-3 border-b-2 transition-colors',
          formTab === 'emergency' ? 'border-indigo-600 text-indigo-600' : 'border-transparent text-slate-500 hover:text-slate-700',
        ]"
      >
        4. {{ t('l_tab_emergency') }}
      </button>
    </div>

    <form @submit.prevent="emit('submit')" class="space-y-4 text-xs">
      <!-- Tab 1: General Info -->
      <div v-show="formTab === 'general'" class="space-y-4">
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div>
            <label class="block font-semibold text-slate-700 mb-1">{{ t('l_full_name') }} *</label>
            <input
              v-model="formData.fullName"
              type="text"
              required
              class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
            />
          </div>

          <div>
            <label class="block font-semibold text-slate-700 mb-1">{{ t('l_email') }} *</label>
            <input
              v-model="formData.email"
              type="email"
              required
              class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
            />
          </div>

          <div>
            <label class="block font-semibold text-slate-700 mb-1">{{ t('l_phone_number') }} *</label>
            <input
              v-model="formData.phoneNumber"
              type="text"
              required
              class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
            />
          </div>

          <div>
            <label class="block font-semibold text-slate-700 mb-1">{{ t('l_date_of_birth') }} *</label>
            <input
              v-model="formData.dateOfBirth"
              type="date"
              required
              class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
            />
          </div>

          <div>
            <label class="block font-semibold text-slate-700 mb-1">{{ t('l_gender') }}</label>
            <select
              v-model="formData.gender"
              class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
            >
              <option :value="Gender.Male">{{ t('s_male') }}</option>
              <option :value="Gender.Female">{{ t('s_female') }}</option>
              <option :value="Gender.Other">{{ t('s_other') }}</option>
            </select>
          </div>

          <div>
            <label class="block font-semibold text-slate-700 mb-1">{{ t('l_status') }}</label>
            <select
              v-model="formData.status"
              class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
            >
              <option :value="EmployeeStatus.Active">{{ t('s_active') }}</option>
              <option :value="EmployeeStatus.Probation">{{ t('s_probation') }}</option>
              <option :value="EmployeeStatus.Inactive">{{ t('s_inactive') }}</option>
              <option :value="EmployeeStatus.Resigned">{{ t('s_resigned') }}</option>
            </select>
          </div>

          <div>
            <label class="block font-semibold text-slate-700 mb-1">{{ t('l_department') }} *</label>
            <select
              v-model="formData.departmentId"
              required
              class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
            >
              <option v-for="d in departments" :key="d.id" :value="d.id">{{ d.departmentName }}</option>
            </select>
          </div>

          <div>
            <label class="block font-semibold text-slate-700 mb-1">{{ t('l_position') }} *</label>
            <select
              v-model="formData.positionId"
              required
              class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
            >
              <option v-for="p in positions" :key="p.id" :value="p.id">{{ p.positionName }}</option>
            </select>
          </div>

          <div>
            <label class="block font-semibold text-slate-700 mb-1">{{ t('l_join_date') }} *</label>
            <input
              v-model="formData.joinDate"
              type="date"
              required
              class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
            />
          </div>

          <div>
            <label class="block font-semibold text-slate-700 mb-1">{{ t('l_manager') }}</label>
            <select
              v-model="formData.managerId"
              class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
            >
              <option :value="undefined">{{ t('l_no_manager') }}</option>
              <option v-for="e in employees.filter(emp => emp.id !== editingId)" :key="e.id" :value="e.id">
                {{ e.fullName }} ({{ e.code }})
              </option>
            </select>
          </div>
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_address') }}</label>
          <input
            v-model="formData.address"
            type="text"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
          />
        </div>
      </div>

      <!-- Tab 2: Identity & Tax -->
      <div v-show="formTab === 'identity'" class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_citizen_id') }}</label>
          <input
            v-model="formData.citizenId"
            type="text"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
          />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_citizen_id_issue_date') }}</label>
          <input
            v-model="formData.citizenIdIssueDate"
            type="date"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
          />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_citizen_id_issue_place') }}</label>
          <input
            v-model="formData.citizenIdIssuePlace"
            type="text"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
          />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_tax_code') }}</label>
          <input
            v-model="formData.taxCode"
            type="text"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
          />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_social_insurance_number') }}</label>
          <input
            v-model="formData.socialInsuranceNumber"
            type="text"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
          />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_dependents_count') }}</label>
          <input
            v-model.number="formData.dependentsCount"
            type="number"
            min="0"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
          />
        </div>
      </div>

      <!-- Tab 3: Bank -->
      <div v-show="formTab === 'bank'" class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_bank_account_number') }}</label>
          <input
            v-model="formData.bankAccountNumber"
            type="text"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
          />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_bank_name') }}</label>
          <input
            v-model="formData.bankName"
            type="text"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
          />
        </div>

        <div class="sm:col-span-2">
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_bank_branch') }}</label>
          <input
            v-model="formData.bankBranch"
            type="text"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
          />
        </div>
      </div>

      <!-- Tab 4: Emergency Contact -->
      <div v-show="formTab === 'emergency'" class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_emergency_contact_name') }}</label>
          <input
            v-model="formData.emergencyContactName"
            type="text"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
          />
        </div>

        <div>
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_emergency_contact_phone') }}</label>
          <input
            v-model="formData.emergencyContactPhone"
            type="text"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
          />
        </div>

        <div class="sm:col-span-2">
          <label class="block font-semibold text-slate-700 mb-1">{{ t('l_emergency_contact_relationship') }}</label>
          <input
            v-model="formData.emergencyContactRelationship"
            type="text"
            class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl focus:border-indigo-500 focus:bg-white"
          />
        </div>
      </div>
    </form>

    <template #footer>
      <button
        type="button"
        @click="emit('update:modelValue', false)"
        class="px-4 py-2 text-slate-600 hover:bg-slate-100 rounded-xl transition-colors font-medium text-xs"
      >
        {{ t('b_cancel') }}
      </button>
      <button
        type="button"
        :disabled="loading"
        @click="emit('submit')"
        class="px-5 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl font-semibold text-xs shadow-md shadow-indigo-600/20 transition-colors disabled:opacity-50"
      >
        <span v-if="loading">{{ t('b_saving') }}</span>
        <span v-else>{{ isEditing ? t('b_save') : t('b_save_new') }}</span>
      </button>
    </template>
  </BaseModal>
</template>
