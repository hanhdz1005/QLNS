<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useToastStore } from '@/stores/toast';
import { useI18n } from '@/locales';
import { lifecycleApi } from '@/api/lifecycle.api';
import type { EmployeeLifecycleDetailDto } from '@/types';
import LifecycleKpiHeader from './lifecycle/components/LifecycleKpiHeader.vue';
import LifecycleStageBreakdown from './lifecycle/components/LifecycleStageBreakdown.vue';
import LifecycleSymbolicCharts from './lifecycle/components/LifecycleSymbolicCharts.vue';
import LifecycleTimeline from './lifecycle/components/LifecycleTimeline.vue';
import { ArrowLeft, GitBranchPlus } from 'lucide-vue-next';

const route = useRoute();
const router = useRouter();
const toastStore = useToastStore();
const { t } = useI18n();

const now = new Date();
const selectedMonth = ref(now.getMonth() + 1);
const selectedYear = ref(now.getFullYear());
const loading = ref(false);
const detail = ref<EmployeeLifecycleDetailDto | null>(null);

const loadEmployeeLifecycle = async () => {
  const employeeId = route.params.id as string;
  if (!employeeId) return;

  loading.value = true;
  try {
    const res = await lifecycleApi.getEmployeeLifecycle(
      employeeId,
      selectedMonth.value,
      selectedYear.value
    );
    if (res.isSuccess && res.data) {
      detail.value = res.data;
    }
  } catch {
    toastStore.error(t('e_lifecycle_load_failed'));
  } finally {
    loading.value = false;
  }
};

watch(
  () => route.params.id,
  () => {
    loadEmployeeLifecycle();
  }
);

onMounted(() => {
  loadEmployeeLifecycle();
});
</script>

<template>
  <div class="space-y-4 sm:space-y-5">
    <!-- Top Navigation Bar -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
      <div class="flex items-center gap-3">
        <button
          @click="router.push('/lifecycle')"
          class="inline-flex items-center gap-2 px-3.5 py-2 bg-white hover:bg-slate-50 text-slate-700 border border-slate-200 rounded-xl text-xs sm:text-sm font-semibold shadow-2xs transition-colors"
        >
          <ArrowLeft class="w-4 h-4 text-slate-500" />
          {{ t('b_back_to_lifecycle_list') }}
        </button>

        <div>
          <h1 class="text-lg sm:text-xl font-bold text-slate-800 tracking-tight flex items-center gap-2">
            <GitBranchPlus class="w-5 h-5 text-indigo-600" />
            {{ t('l_lifecycle_page_title') }}
          </h1>
        </div>
      </div>
    </div>

    <div v-if="loading && !detail" class="bg-white rounded-2xl border border-slate-200/80 p-12 text-center text-sm text-slate-400">
      {{ t('l_loading_data') }}
    </div>

    <div v-else-if="detail" class="space-y-4 sm:space-y-5">
      <LifecycleKpiHeader
        :detail="detail"
        v-model:month="selectedMonth"
        v-model:year="selectedYear"
        @reload="loadEmployeeLifecycle"
      />

      <LifecycleStageBreakdown :detail="detail" />

      <LifecycleSymbolicCharts :detail="detail" />

      <LifecycleTimeline :milestones="detail.milestones" />
    </div>
  </div>
</template>
