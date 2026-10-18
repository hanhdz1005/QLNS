<script setup lang="ts">
import type { DepartmentDto } from '@/types';
import { useI18n } from '@/locales';
import { Users, Search, Plus, Download } from 'lucide-vue-next';

defineProps<{
  departments: DepartmentDto[];
  selectedDepartment: string;
  search: string;
  isAdmin: boolean;
}>();

const emit = defineEmits<{
  (e: 'update:search', val: string): void;
  (e: 'update:selectedDepartment', val: string): void;
  (e: 'openAdd'): void;
  (e: 'exportCsv'): void;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="space-y-3.5 shrink-0">
    <!-- Header Title & Action Buttons -->
    <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3">
      <div>
        <h1 class="text-lg sm:text-xl font-bold text-slate-800 tracking-tight flex items-center gap-2.5">
          <Users class="w-5 h-5 text-indigo-600" />
          {{ t('n_employees') }}
        </h1>
        <p class="text-xs text-slate-500 hidden sm:block mt-0.5">
          {{ t('l_staff_distribution') }}
        </p>
      </div>

      <div class="flex items-center gap-2.5">
        <button
          @click="emit('exportCsv')"
          class="inline-flex items-center gap-2 px-3.5 py-2 bg-white hover:bg-slate-50 text-slate-700 border border-slate-200 rounded-xl text-xs sm:text-sm font-semibold shadow-2xs transition-colors"
        >
          <Download class="w-4 h-4 text-slate-500" />
          {{ t('b_export_csv') }}
        </button>

        <button
          v-if="isAdmin"
          @click="emit('openAdd')"
          class="inline-flex items-center gap-2 px-3.5 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-xs sm:text-sm font-semibold shadow-md shadow-indigo-600/20 transition-colors"
        >
          <Plus class="w-4 h-4" />
          {{ t('b_add_new_employee') }}
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
          :placeholder="t('l_search_placeholder')"
          class="w-full pl-10 pr-3.5 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm text-slate-800 placeholder-slate-400 focus:outline-hidden focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
        />
      </div>

      <div class="flex items-center gap-2.5 w-full sm:w-auto">
        <select
          :value="selectedDepartment"
          @change="emit('update:selectedDepartment', ($event.target as HTMLSelectElement).value)"
          class="py-2 px-3 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm text-slate-700 focus:outline-hidden focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500"
        >
          <option value="">{{ t('l_all_departments') }}</option>
          <option v-for="d in departments" :key="d.id" :value="d.id">
            {{ d.departmentName }}
          </option>
        </select>
      </div>
    </div>
  </div>
</template>
