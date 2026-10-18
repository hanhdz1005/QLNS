<script setup lang="ts">
import { ref } from 'vue';
import { useI18n } from '@/locales';
import BaseModal from '@/components/BaseModal.vue';

const props = defineProps<{
  modelValue: boolean;
  loading: boolean;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void;
  (e: 'submit', roleName: string): void;
}>();

const { t } = useI18n();
const roleName = ref('');

const handleSubmit = () => {
  emit('submit', roleName.value);
};

const handleClose = () => {
  roleName.value = '';
  emit('update:modelValue', false);
};
</script>

<template>
  <BaseModal
    :model-value="modelValue"
    @update:model-value="(val) => emit('update:modelValue', val)"
    :title="t('l_create_role_modal_title')"
    max-width="sm"
  >
    <div class="space-y-4 text-xs">
      <div>
        <label class="block font-semibold text-slate-700 mb-1">{{ t('l_role_name') }} *</label>
        <input
          v-model="roleName"
          type="text"
          required
          :placeholder="t('l_role_name_placeholder')"
          class="w-full py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl"
        />
      </div>
    </div>
    <template #footer>
      <button
        type="button"
        @click="handleClose"
        class="px-4 py-2 text-slate-600 hover:bg-slate-100 rounded-xl font-medium text-xs"
      >
        {{ t('b_cancel') }}
      </button>
      <button
        type="button"
        :disabled="loading"
        @click="handleSubmit"
        class="px-5 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl font-semibold text-xs disabled:opacity-50"
      >
        <span v-if="loading">{{ t('b_saving') }}</span>
        <span v-else>{{ t('b_create_role') }}</span>
      </button>
    </template>
  </BaseModal>
</template>
