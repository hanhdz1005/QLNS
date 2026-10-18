<script setup lang="ts">
import { useI18n } from '@/locales';
import { ChevronLeft, ChevronRight } from 'lucide-vue-next';

defineProps<{
  pageIndex: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}>();

const emit = defineEmits<{
  (e: 'change-page', page: number): void;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="flex items-center justify-between px-4 py-3 bg-white border-t border-slate-100 shrink-0">
    <div class="text-xs text-slate-500">
      {{ t('l_showing') }}
      <span class="font-semibold text-slate-700">
        {{ totalCount === 0 ? 0 : (pageIndex - 1) * pageSize + 1 }}
      </span>
      {{ t('l_to') }}
      <span class="font-semibold text-slate-700">
        {{ Math.min(pageIndex * pageSize, totalCount) }}
      </span>
      {{ t('l_of_total') }}
      <span class="font-semibold text-slate-700">{{ totalCount }}</span> {{ t('l_records_suffix') }}
    </div>

    <div class="flex items-center gap-2">
      <button
        :disabled="pageIndex <= 1"
        @click="emit('change-page', pageIndex - 1)"
        class="inline-flex items-center justify-center p-1.5 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 disabled:opacity-40 disabled:cursor-not-allowed transition-colors"
      >
        <ChevronLeft class="w-4 h-4" />
      </button>

      <span class="text-xs font-medium text-slate-600 px-1.5">
        {{ t('l_page_prefix') }} {{ pageIndex }} / {{ Math.max(totalPages, 1) }}
      </span>

      <button
        :disabled="pageIndex >= totalPages || totalPages === 0"
        @click="emit('change-page', pageIndex + 1)"
        class="inline-flex items-center justify-center p-1.5 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 disabled:opacity-40 disabled:cursor-not-allowed transition-colors"
      >
        <ChevronRight class="w-4 h-4" />
      </button>
    </div>
  </div>
</template>
