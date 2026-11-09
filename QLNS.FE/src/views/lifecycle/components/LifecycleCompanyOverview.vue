<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import type { CompanyLifecycleOverviewDto } from '@/types';
import { formatDate, formatCurrency } from '@/utils/formatters';
import { useI18n } from '@/locales';
import {
  Users,
  GraduationCap,
  Briefcase,
  Award,
  TrendingUp,
  Search,
  ArrowRight,
  Clock,
  Flame,
} from 'lucide-vue-next';

const props = defineProps<{
  overview: CompanyLifecycleOverviewDto;
  loading: boolean;
}>();

const router = useRouter();
const { t } = useI18n();

const search = ref('');
const stageFilter = ref<string>('ALL');

const filteredEmployees = computed(() => {
  const keyword = search.value.trim().toLowerCase();
  return (props.overview.employees || []).filter((emp) => {
    const matchStage = stageFilter.value === 'ALL' || emp.currentStage === stageFilter.value;
    const matchSearch =
      !keyword ||
      emp.fullName.toLowerCase().includes(keyword) ||
      emp.code.toLowerCase().includes(keyword) ||
      emp.departmentName.toLowerCase().includes(keyword);
    return matchStage && matchSearch;
  });
});

const getStageBadge = (stage: string) => {
  switch (stage) {
    case 'Internship':
      return { label: t('s_stage_internship'), cls: 'bg-cyan-50 text-cyan-700 border-cyan-200' };
    case 'Probation':
      return { label: t('s_stage_probation'), cls: 'bg-amber-50 text-amber-700 border-amber-200' };
    case 'Resigned':
      return { label: t('s_stage_resigned'), cls: 'bg-rose-50 text-rose-700 border-rose-200' };
    default:
      return { label: t('s_stage_official'), cls: 'bg-indigo-50 text-indigo-700 border-indigo-200' };
  }
};

const goToDetail = (employeeId: string) => {
  router.push(`/lifecycle/${employeeId}`);
};
</script>

<template>
  <div class="space-y-4 sm:space-y-5">
    <!-- Company Lifecycle KPI Cards -->
    <div class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-6 gap-3.5">
      <div class="bg-white p-4 rounded-2xl border border-slate-200/80 shadow-xs space-y-1">
        <div class="flex items-center justify-between text-xs text-slate-500">
          <span>{{ t('n_employees') }}</span>
          <Users class="w-4 h-4 text-slate-600" />
        </div>
        <div class="text-xl font-extrabold text-slate-800 font-mono">{{ overview.totalEmployees }}</div>
      </div>

      <div class="bg-white p-4 rounded-2xl border border-cyan-200/80 shadow-xs space-y-1">
        <div class="flex items-center justify-between text-xs text-cyan-700 font-medium">
          <span>{{ t('s_stage_internship') }}</span>
          <GraduationCap class="w-4 h-4 text-cyan-600" />
        </div>
        <div class="text-xl font-extrabold text-cyan-700 font-mono">{{ overview.internshipCount }}</div>
      </div>

      <div class="bg-white p-4 rounded-2xl border border-amber-200/80 shadow-xs space-y-1">
        <div class="flex items-center justify-between text-xs text-amber-700 font-medium">
          <span>{{ t('s_stage_probation') }}</span>
          <Briefcase class="w-4 h-4 text-amber-600" />
        </div>
        <div class="text-xl font-extrabold text-amber-700 font-mono">{{ overview.probationCount }}</div>
      </div>

      <div class="bg-white p-4 rounded-2xl border border-indigo-200/80 shadow-xs space-y-1">
        <div class="flex items-center justify-between text-xs text-indigo-700 font-medium">
          <span>{{ t('s_stage_official') }}</span>
          <Award class="w-4 h-4 text-indigo-600" />
        </div>
        <div class="text-xl font-extrabold text-indigo-700 font-mono">{{ overview.officialCount }}</div>
      </div>

      <div class="bg-white p-4 rounded-2xl border border-slate-200/80 shadow-xs space-y-1">
        <div class="flex items-center justify-between text-xs text-slate-500">
          <span>{{ t('l_company_avg_tenure') }}</span>
          <Clock class="w-4 h-4 text-indigo-600" />
        </div>
        <div class="text-xl font-extrabold text-slate-800 font-mono">
          {{ overview.averageTenureMonths }} {{ t('l_months_abbr') }}
        </div>
      </div>

      <div class="bg-white p-4 rounded-2xl border border-emerald-200/80 shadow-xs space-y-1">
        <div class="flex items-center justify-between text-xs text-emerald-700 font-medium">
          <span>{{ t('l_company_retention_rate') }}</span>
          <TrendingUp class="w-4 h-4 text-emerald-600" />
        </div>
        <div class="text-xl font-extrabold text-emerald-600 font-mono">{{ overview.retentionRate }}%</div>
      </div>
    </div>

    <!-- Search & Stage Filter Toolbar -->
    <div class="bg-white p-4 rounded-2xl border border-slate-200/80 shadow-xs flex flex-col sm:flex-row items-center justify-between gap-3">
      <div class="flex flex-wrap items-center gap-1.5 w-full sm:w-auto">
        <button
          v-for="st in [
            { key: 'ALL', label: t('l_all_stages') },
            { key: 'Internship', label: t('s_stage_internship') },
            { key: 'Probation', label: t('s_stage_probation') },
            { key: 'Official', label: t('s_stage_official') },
            { key: 'Resigned', label: t('s_stage_resigned') },
          ]"
          :key="st.key"
          @click="stageFilter = st.key"
          :class="[
            'px-3 py-1.5 rounded-xl text-xs font-semibold transition-all',
            stageFilter === st.key
              ? 'bg-indigo-600 text-white shadow-2xs'
              : 'bg-slate-100 text-slate-600 hover:bg-slate-200',
          ]"
        >
          {{ st.label }}
        </button>
      </div>

      <div class="relative w-full sm:w-80">
        <Search class="w-4 h-4 text-slate-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
        <input
          v-model="search"
          type="text"
          :placeholder="t('l_search_placeholder')"
          class="w-full pl-10 pr-3.5 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs sm:text-sm text-slate-800 placeholder-slate-400 focus:outline-hidden focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500"
        />
      </div>
    </div>

    <!-- Employee Lifecycle Directory Table -->
    <div class="bg-white rounded-2xl border border-slate-200/80 shadow-xs overflow-hidden">
      <div class="p-4 border-b border-slate-100 flex items-center justify-between">
        <h3 class="text-xs sm:text-sm font-bold text-slate-800">
          {{ t('l_employee_lifecycle_list_title') }}
        </h3>
        <span class="text-xs text-slate-500 font-medium">
          {{ filteredEmployees.length }} / {{ overview.totalEmployees }}
        </span>
      </div>

      <div class="overflow-x-auto" tabindex="0" role="region" :aria-label="t('l_employee_lifecycle_list_title')">
        <table class="w-full text-left border-collapse text-xs">
          <thead>
            <tr class="bg-slate-50/90 border-b border-slate-200/80 text-slate-500 font-semibold uppercase tracking-wider">
              <th class="py-3 px-4">{{ t('l_full_name') }}</th>
              <th class="py-3 px-4">{{ t('l_department') }}</th>
              <th class="py-3 px-4">{{ t('l_stage_current_active') }}</th>
              <th class="py-3 px-4">{{ t('s_stage_internship') }}</th>
              <th class="py-3 px-4">{{ t('s_stage_probation') }}</th>
              <th class="py-3 px-4">{{ t('s_stage_official') }}</th>
              <th class="py-3 px-4">{{ t('l_total_tenure_kpi') }}</th>
              <th class="py-3 px-4">{{ t('l_monthly_hours_kpi') }}</th>
              <th class="py-3 px-4">{{ t('l_stage_salary_level') }}</th>
              <th class="py-3 px-4 text-right">{{ t('l_actions') }}</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100">
            <tr v-if="loading">
              <td colspan="10" class="py-10 text-center text-slate-400">{{ t('l_loading_data') }}</td>
            </tr>
            <tr v-else-if="filteredEmployees.length === 0">
              <td colspan="10" class="py-10 text-center text-slate-400">{{ t('l_no_data') }}</td>
            </tr>
            <tr
              v-for="emp in filteredEmployees"
              :key="emp.employeeId"
              @click="goToDetail(emp.employeeId)"
              class="hover:bg-indigo-50/40 cursor-pointer transition-colors group"
            >
              <td class="py-3 px-4">
                <div class="flex items-center gap-2.5">
                  <div class="w-8 h-8 rounded-xl bg-indigo-50 text-indigo-700 font-bold text-xs flex items-center justify-center shrink-0 border border-indigo-100">
                    {{ emp.fullName?.charAt(0) || 'N' }}
                  </div>
                  <div>
                    <div class="font-bold text-slate-800 group-hover:text-indigo-600 transition-colors">
                      {{ emp.fullName }}
                    </div>
                    <div class="text-[11px] font-mono text-slate-400">{{ emp.code }} • {{ formatDate(emp.joinDate) }}</div>
                  </div>
                </div>
              </td>
              <td class="py-3 px-4">
                <div class="font-medium text-slate-700">{{ emp.departmentName }}</div>
                <div class="text-[11px] text-slate-400">{{ emp.positionName }}</div>
              </td>
              <td class="py-3 px-4">
                <span :class="['px-2.5 py-1 rounded-lg text-xs font-bold border', getStageBadge(emp.currentStage).cls]">
                  {{ getStageBadge(emp.currentStage).label }}
                </span>
              </td>
              <td class="py-3 px-4 font-mono text-cyan-700 font-semibold">
                {{ emp.internshipMonths }} {{ t('l_months_abbr') }}
              </td>
              <td class="py-3 px-4 font-mono text-amber-700 font-semibold">
                {{ emp.probationMonths }} {{ t('l_months_abbr') }}
              </td>
              <td class="py-3 px-4 font-mono text-indigo-700 font-semibold">
                {{ emp.officialMonths }} {{ t('l_months_abbr') }}
              </td>
              <td class="py-3 px-4 font-mono font-extrabold text-slate-800">
                {{ emp.totalTenureMonths }} {{ t('l_months_abbr') }}
              </td>
              <td class="py-3 px-4 font-mono">
                <span class="font-semibold text-slate-700">{{ emp.monthlyRegularHours }}h</span>
                <span v-if="emp.monthlyOvertimeHours > 0" class="ml-1.5 inline-flex items-center gap-0.5 text-amber-600 font-bold">
                  <Flame class="w-3 h-3" /> +{{ emp.monthlyOvertimeHours }}h
                </span>
              </td>
              <td class="py-3 px-4 font-mono font-semibold text-slate-700">
                {{ formatCurrency(emp.currentSalary) }}
              </td>
              <td class="py-3 px-4 text-right" @click.stop>
                <button
                  @click="goToDetail(emp.employeeId)"
                  class="inline-flex items-center gap-1.5 px-3 py-1.5 bg-indigo-50 hover:bg-indigo-600 text-indigo-700 hover:text-white rounded-xl text-xs font-semibold transition-all"
                >
                  {{ t('b_view_lifecycle_detail') }}
                  <ArrowRight class="w-3.5 h-3.5" />
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
