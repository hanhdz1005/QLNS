<script setup lang="ts">
import { useI18n } from '@/locales';
import { FileText, Search, Plus, AlertTriangle } from 'lucide-vue-next';

defineProps<{
  search: string;
  onlyExpiring: boolean;
  isAdmin: boolean;
}>();

const emit = defineEmits<{
  (e: 'update:search', val: string): void;
  (e: 'update:onlyExpiring', val: boolean): void;
  (e: 'openAdd'): void;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="space-y-3.5 shrink-0">
    <!-- Header Title & Buttons -->
    <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3">
      <div>
        <h1 class="text-lg sm:text-xl font-bold text-slate-800 tracking-tight flex items-center gap-2.5">
          <FileText class="w-5 h-5 text-indigo-600" />
          {{ t('n_contracts') }}
        </h1>
        <p class="text-xs text-slate-500 hidden sm:block mt-0.5">
          {{ t('l_contracts_subtitle') }}
        </p>
      </div>

      <div class="flex items-center gap-2.5">
        <button
          v-if="isAdmin"
          @click="emit('openAdd')"
          class="inline-flex items-center gap-2 px-3.5 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-xs sm:text-sm font-semibold shadow-md shadow-indigo-600/20 transition-colors"
        >
          <Plus class="w-4 h-4" />
          {{ t('b_add_contract') }}
        </button>
      </div>
    </div>

    <!-- Filters & Search Toolbar -->
    <div class="bg-white p-3 sm:p-4 rounded-2xl border border-slate-200/80 shadow-xs flex flex-col sm:flex-row items-center justify-between gap-3">
      <div class="relative w-full sm:w-80">
        <Search class="w-4 h-4 text-slate-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
        <input
          :value="search"
          @input="emit('update:search', ($event.target as HTMLInputElement).value)"
          type="text"
          :placeholder="t('l_search_contract_placeholder')"
          class="w-full pl-10 pr-3.5 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm text-slate-800 placeholder-slate-400 focus:outline-hidden focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
        />
      </div>

      <label class="flex items-center gap-2 text-xs sm:text-sm font-medium text-slate-700 cursor-pointer select-none">
        <input
          type="checkbox"
          :checked="onlyExpiring"
          @change="emit('update:onlyExpiring', ($event.target as HTMLInputElement).checked)"
          class="rounded-md border-slate-300 text-indigo-600 focus:ring-indigo-500 w-4 h-4"
        />
        <span class="flex items-center gap-1.5 text-amber-700 bg-amber-50 px-2.5 py-1 rounded-xl border border-amber-200/60 font-semibold text-xs">
          <AlertTriangle class="w-3.5 h-3.5 text-amber-500" />
          {{ t('l_expiring_only') }}
        </span>
      </label>
    </div>
  </div>
</template>
