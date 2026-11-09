<script setup lang="ts">
import type { CreateOvertimeRequestDto } from '@/types';
import { useI18n } from '@/locales';
import BaseModal from '@/components/BaseModal.vue';
import { Flame, Clock, Info } from 'lucide-vue-next';

defineProps<{
  modelValue: boolean;
  otForm: CreateOvertimeRequestDto;
  loading: boolean;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', val: boolean): void;
  (e: 'submit'): void;
}>();

const { t } = useI18n();
</script>

<template>
  <BaseModal
    :model-value="modelValue"
    :title="t('l_ot_modal_title')"
    @update:model-value="emit('update:modelValue', $event)"
  >
    <form @submit.prevent="emit('submit')" class="space-y-4">
      <!-- Info Banner about Exact Registered OT Hours -->
      <div class="p-3.5 rounded-xl bg-amber-50 border border-amber-200/80 flex items-start gap-2.5">
        <Info class="w-4 h-4 text-amber-600 shrink-0 mt-0.5" />
        <p class="text-xs text-amber-800 leading-relaxed">
          {{ t('l_ot_registered_hours_hint') }}
        </p>
      </div>

      <!-- OT Date -->
      <div>
        <label class="block text-xs font-semibold text-slate-700 mb-1.5">
          {{ t('l_ot_date') }} <span class="text-rose-500">*</span>
        </label>
        <input
          v-model="otForm.date"
          type="date"
          required
          class="w-full px-3.5 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm text-slate-800 focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500"
        />
      </div>

      <!-- Registered Hours -->
      <div>
        <label class="block text-xs font-semibold text-slate-700 mb-1.5">
          {{ t('l_ot_registered_hours') }} <span class="text-rose-500">*</span>
        </label>
        <div class="relative">
          <Clock class="w-4 h-4 text-slate-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
          <input
            v-model.number="otForm.registeredHours"
            type="number"
            step="0.5"
            min="0.5"
            max="12"
            required
            class="w-full pl-10 pr-16 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm font-mono font-bold text-indigo-700 focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500"
          />
          <span class="absolute right-3.5 top-1/2 -translate-y-1/2 text-xs font-semibold text-slate-400">
            {{ t('l_hours_unit') }}
          </span>
        </div>
        <p class="text-[11px] text-slate-500 mt-1">
          {{ t('l_ot_registered_hours_note') }}
        </p>
      </div>

      <!-- Reason -->
      <div>
        <label class="block text-xs font-semibold text-slate-700 mb-1.5">
          {{ t('l_ot_reason') }} <span class="text-rose-500">*</span>
        </label>
        <textarea
          v-model="otForm.reason"
          rows="3"
          required
          :placeholder="t('l_ot_reason_placeholder')"
          class="w-full px-3.5 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm text-slate-800 placeholder-slate-400 focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500"
        />
      </div>

      <!-- Actions -->
      <div class="flex items-center justify-end gap-2.5 pt-2 border-t border-slate-100">
        <button
          type="button"
          @click="emit('update:modelValue', false)"
          class="px-4 py-2 bg-slate-100 hover:bg-slate-200 text-slate-700 rounded-xl text-xs sm:text-sm font-semibold transition-colors"
        >
          {{ t('b_cancel') }}
        </button>
        <button
          type="submit"
          :disabled="loading"
          class="inline-flex items-center gap-2 px-4 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-xs sm:text-sm font-semibold shadow-md shadow-indigo-600/20 transition-colors disabled:opacity-50"
        >
          <Flame class="w-4 h-4" />
          {{ loading ? t('b_saving') : t('b_submit_ot_request') }}
        </button>
      </div>
    </form>
  </BaseModal>
</template>
