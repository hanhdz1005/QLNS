<script setup lang="ts">
import { useI18n } from '@/locales';
import type { AssignClaimDto } from '@/types';
import BaseModal from '@/components/BaseModal.vue';

defineProps<{
  modelValue: boolean;
  form: AssignClaimDto;
  loading: boolean;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void;
  (e: 'submit'): void;
}>();

const { t } = useI18n();
</script>

<template>
  <BaseModal
    :model-value="modelValue"
    @update:model-value="(val) => emit('update:modelValue', val)"
    :title="t('l_assign_claim_modal_title')"
    max-width="sm"
  >
    <div class="space-y-4 text-xs">
      <div>
        <label class="block font-semibold text-slate-700 mb-1">{{ t('l_account_email') }}</label>
        <input :value="form.email" disabled class="w-full py-2 px-3 bg-slate-100 border border-slate-200 rounded-xl text-slate-500" />
      </div>
      <div>
        <label class="block font-semibold text-slate-700 mb-1">{{ t('l_claim_value') }} *</label>
        <select v-model="form.claimValue" class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl">
          <option value="FULL.ACCESS">{{ t('l_claim_full_access') }}</option>
          <option value="ASSET.VIEW">{{ t('l_claim_asset_view') }}</option>
          <option value="ASSET.CREATE">{{ t('l_claim_asset_create') }}</option>
          <option value="ASSET.EDIT">{{ t('l_claim_asset_edit') }}</option>
          <option value="ASSET.DELETE">{{ t('l_claim_asset_delete') }}</option>
        </select>
      </div>
    </div>
    <template #footer>
      <button
        type="button"
        @click="emit('update:modelValue', false)"
        class="px-4 py-2 text-slate-600 hover:bg-slate-100 rounded-xl font-medium text-xs"
      >
        {{ t('b_cancel') }}
      </button>
      <button
        type="button"
        :disabled="loading"
        @click="emit('submit')"
        class="px-5 py-2 bg-amber-600 hover:bg-amber-700 text-white rounded-xl font-semibold text-xs disabled:opacity-50"
      >
        <span v-if="loading">{{ t('b_saving') }}</span>
        <span v-else>{{ t('b_grant_claim_btn') }}</span>
      </button>
    </template>
  </BaseModal>
</template>
