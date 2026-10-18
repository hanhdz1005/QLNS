<script setup lang="ts">
import { useI18n } from '@/locales';
import { AlertTriangle } from 'lucide-vue-next';
import BaseModal from './BaseModal.vue';

defineProps<{
  modelValue: boolean;
  title?: string;
  message?: string;
  confirmText?: string;
  cancelText?: string;
  loading?: boolean;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void;
  (e: 'confirm'): void;
}>();

const { t } = useI18n();
</script>

<template>
  <BaseModal
    :model-value="modelValue"
    @update:model-value="emit('update:modelValue', $event)"
    max-width="md"
    :title="title || t('l_confirm_action_title')"
  >
    <div class="flex items-start gap-4">
      <div class="w-10 h-10 rounded-xl bg-rose-50 text-rose-600 flex items-center justify-center shrink-0 border border-rose-100">
        <AlertTriangle class="w-5 h-5" />
      </div>
      <div>
        <h4 class="text-sm font-semibold text-slate-800">
          {{ title || t('l_confirm_default_question') }}
        </h4>
        <p class="text-sm text-slate-500 mt-1 leading-relaxed">
          {{ message || t('l_confirm_default_notice') }}
        </p>
      </div>
    </div>

    <template #footer>
      <button
        type="button"
        @click="emit('update:modelValue', false)"
        class="px-4 py-2 text-sm font-medium text-slate-600 hover:bg-slate-100 rounded-xl transition-colors"
      >
        {{ cancelText || t('b_cancel') }}
      </button>
      <button
        type="button"
        :disabled="loading"
        @click="emit('confirm')"
        class="px-4 py-2 text-sm font-medium text-white bg-rose-600 hover:bg-rose-700 disabled:opacity-50 rounded-xl transition-colors shadow-xs"
      >
        <span v-if="loading">{{ t('l_processing') }}</span>
        <span v-else>{{ confirmText || t('l_confirm_delete_default') }}</span>
      </button>
    </template>
  </BaseModal>
</template>
