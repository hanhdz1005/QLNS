<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useToastStore } from '@/stores/toast';
import { useI18n } from '@/locales';
import { lifecycleApi } from '@/api/lifecycle.api';
import type { CompanyLifecycleOverviewDto, EmployeeLifecycleDetailDto } from '@/types';
import LifecycleCompanyOverview from './lifecycle/components/LifecycleCompanyOverview.vue';
import LifecycleKpiHeader from './lifecycle/components/LifecycleKpiHeader.vue';
import LifecycleStageBreakdown from './lifecycle/components/LifecycleStageBreakdown.vue';
import LifecycleSymbolicCharts from './lifecycle/components/LifecycleSymbolicCharts.vue';
import LifecycleTimeline from './lifecycle/components/LifecycleTimeline.vue';
import { GitBranchPlus, Users, User, Sparkles } from 'lucide-vue-next';

const authStore = useAuthStore();
const toastStore = useToastStore();
const { t } = useI18n();

const now = new Date();
const selectedMonth = ref(now.getMonth() + 1);
const selectedYear = ref(now.getFullYear());

const viewMode = ref<'company' | 'personal'>(
  authStore.canViewAllLifecycles ? 'company' : 'personal'
);

const loading = ref(false);
const overviewData = ref<CompanyLifecycleOverviewDto>({
  asOfMonth: selectedMonth.value,
  asOfYear: selectedYear.value,
  totalEmployees: 0,
  internshipCount: 0,
  probationCount: 0,
  officialCount: 0,
  resignedCount: 0,
  averageTenureMonths: 0,
  retentionRate: 100,
  employees: [],
});

const myLifecycle = ref<EmployeeLifecycleDetailDto | null>(null);

const loadCompanyOverview = async () => {
  if (!authStore.canViewAllLifecycles) return;
  loading.value = true;
  try {
    const res = await lifecycleApi.getOverview(selectedMonth.value, selectedYear.value);
    if (res.isSuccess && res.data) {
      overviewData.value = res.data;
    }
  } catch {
    toastStore.error(t('e_lifecycle_load_failed'));
  } finally {
    loading.value = false;
  }
};

const loadMyLifecycle = async () => {
  loading.value = true;
  try {
    const res = await lifecycleApi.getMyLifecycle(selectedMonth.value, selectedYear.value);
    if (res.isSuccess && res.data) {
      myLifecycle.value = res.data;
    }
  } catch {
    toastStore.error(t('e_lifecycle_load_failed'));
  } finally {
    loading.value = false;
  }
};

const handleReload = () => {
  if (viewMode.value === 'company' && authStore.canViewAllLifecycles) {
    loadCompanyOverview();
  } else {
    loadMyLifecycle();
  }
};

const switchMode = (mode: 'company' | 'personal') => {
  viewMode.value = mode;
  handleReload();
};

onMounted(() => {
  handleReload();
});
</script>

<template>
  <div class="space-y-4 sm:space-y-5">
    <!-- Top Page Header -->
    <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3">
      <div>
        <h1 class="text-lg sm:text-xl font-bold text-slate-800 tracking-tight flex items-center gap-2.5">
          <GitBranchPlus class="w-5 h-5 text-indigo-600" />
          {{ authStore.canViewAllLifecycles && viewMode === 'company' ? t('l_lifecycle_page_title') : t('l_my_lifecycle_title') }}
        </h1>
        <p class="text-xs text-slate-500 mt-0.5">
          {{ authStore.canViewAllLifecycles && viewMode === 'company' ? t('l_lifecycle_page_subtitle') : t('l_my_lifecycle_subtitle') }}
        </p>
      </div>

      <div class="flex flex-wrap items-center gap-2.5">
        <!-- Period Selector when in Company Overview Mode -->
        <div
          v-if="authStore.canViewAllLifecycles && viewMode === 'company'"
          class="flex items-center gap-2 bg-white px-3 py-1.5 rounded-xl border border-slate-200/80 shadow-2xs text-xs"
        >
          <Sparkles class="w-3.5 h-3.5 text-indigo-600" />
          <span class="text-slate-500 font-medium">{{ t('l_as_of_period') }}:</span>
          <select
            v-model.number="selectedMonth"
            @change="loadCompanyOverview"
            class="bg-slate-50 border border-slate-200 rounded-lg px-2 py-1 text-slate-700 font-semibold focus:outline-hidden"
          >
            <option v-for="m in 12" :key="m" :value="m">{{ t('l_month_prefix') }} {{ m }}</option>
          </select>
          <select
            v-model.number="selectedYear"
            @change="loadCompanyOverview"
            class="bg-slate-50 border border-slate-200 rounded-lg px-2 py-1 text-slate-700 font-semibold focus:outline-hidden"
          >
            <option v-for="y in [2025, 2026, 2027]" :key="y" :value="y">{{ t('l_year_prefix') }} {{ y }}</option>
          </select>
        </div>

        <!-- Mode Switcher for Admin / HR / DepartmentManager -->
        <div
          v-if="authStore.canViewAllLifecycles"
          class="flex items-center p-1 bg-slate-200/80 rounded-xl text-xs font-semibold"
        >
          <button
            @click="switchMode('company')"
            :class="[
              'flex items-center gap-1.5 px-3 py-1.5 rounded-lg transition-all',
              viewMode === 'company' ? 'bg-white text-indigo-700 shadow-2xs' : 'text-slate-600 hover:text-slate-900',
            ]"
          >
            <Users class="w-3.5 h-3.5" />
            {{ t('b_view_company_lifecycle') }}
          </button>
          <button
            @click="switchMode('personal')"
            :class="[
              'flex items-center gap-1.5 px-3 py-1.5 rounded-lg transition-all',
              viewMode === 'personal' ? 'bg-white text-indigo-700 shadow-2xs' : 'text-slate-600 hover:text-slate-900',
            ]"
          >
            <User class="w-3.5 h-3.5" />
            {{ t('b_view_my_lifecycle') }}
          </button>
        </div>
      </div>
    </div>

    <!-- Company Overview Mode (Admin / HRManager / HRStaff / DepartmentManager) -->
    <LifecycleCompanyOverview
      v-if="authStore.canViewAllLifecycles && viewMode === 'company'"
      :overview="overviewData"
      :loading="loading"
    />

    <!-- Personal Lifecycle Mode (Default for Employee/Accountant, or toggled) -->
    <div v-else-if="myLifecycle" class="space-y-4 sm:space-y-5">
      <LifecycleKpiHeader
        :detail="myLifecycle"
        v-model:month="selectedMonth"
        v-model:year="selectedYear"
        @reload="loadMyLifecycle"
      />

      <LifecycleStageBreakdown :detail="myLifecycle" />

      <LifecycleSymbolicCharts :detail="myLifecycle" />

      <LifecycleTimeline :milestones="myLifecycle.milestones" />
    </div>

    <div
      v-else-if="loading"
      class="bg-white rounded-2xl border border-slate-200/80 p-12 text-center text-sm text-slate-400"
    >
      {{ t('l_loading_data') }}
    </div>
  </div>
</template>
