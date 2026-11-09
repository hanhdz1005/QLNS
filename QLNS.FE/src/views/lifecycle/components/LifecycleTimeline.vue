<script setup lang="ts">
import { ref, computed } from 'vue';
import type { LifecycleMilestoneDto } from '@/types';
import { formatDate, formatCurrency } from '@/utils/formatters';
import { useI18n } from '@/locales';
import {
  GitCommitHorizontal,
  GraduationCap,
  Briefcase,
  Award,
  Sparkles,
  AlertTriangle,
  UserPlus,
} from 'lucide-vue-next';

const props = defineProps<{
  milestones: LifecycleMilestoneDto[];
}>();

const { t } = useI18n();

const selectedCategory = ref<string>('ALL');

const categories = computed(() => [
  { key: 'ALL', label: t('l_timeline_all') },
  { key: 'Onboarding', label: t('l_timeline_onboarding') },
  { key: 'Internship', label: t('l_timeline_internship') },
  { key: 'Probation', label: t('l_timeline_probation') },
  { key: 'Official', label: t('l_timeline_official') },
  { key: 'Bonus', label: t('l_timeline_bonus') },
  { key: 'Penalty', label: t('l_timeline_penalty') },
]);

const filteredMilestones = computed(() => {
  if (selectedCategory.value === 'ALL') return props.milestones;
  return props.milestones.filter((m) => m.category === selectedCategory.value);
});

const getCategoryMeta = (category: string) => {
  switch (category) {
    case 'Onboarding':
      return {
        label: t('l_timeline_onboarding'),
        icon: UserPlus,
        dotClass: 'bg-emerald-600 ring-emerald-100',
        badgeClass: 'bg-emerald-50 text-emerald-700 border-emerald-200',
      };
    case 'Internship':
      return {
        label: t('l_timeline_internship'),
        icon: GraduationCap,
        dotClass: 'bg-cyan-500 ring-cyan-100',
        badgeClass: 'bg-cyan-50 text-cyan-700 border-cyan-200',
      };
    case 'Probation':
      return {
        label: t('l_timeline_probation'),
        icon: Briefcase,
        dotClass: 'bg-amber-500 ring-amber-100',
        badgeClass: 'bg-amber-50 text-amber-700 border-amber-200',
      };
    case 'Official':
      return {
        label: t('l_timeline_official'),
        icon: Award,
        dotClass: 'bg-indigo-600 ring-indigo-100',
        badgeClass: 'bg-indigo-50 text-indigo-700 border-indigo-200',
      };
    case 'Bonus':
      return {
        label: t('l_timeline_bonus'),
        icon: Sparkles,
        dotClass: 'bg-violet-600 ring-violet-100',
        badgeClass: 'bg-violet-50 text-violet-700 border-violet-200',
      };
    case 'Penalty':
      return {
        label: t('l_timeline_penalty'),
        icon: AlertTriangle,
        dotClass: 'bg-rose-600 ring-rose-100',
        badgeClass: 'bg-rose-50 text-rose-700 border-rose-200',
      };
    default:
      return {
        label: category,
        icon: GitCommitHorizontal,
        dotClass: 'bg-slate-500 ring-slate-100',
        badgeClass: 'bg-slate-50 text-slate-700 border-slate-200',
      };
  }
};
</script>

<template>
  <div class="bg-white rounded-2xl border border-slate-200/80 p-5 shadow-xs space-y-4">
    <!-- Header & Category Filter -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3 pb-3 border-b border-slate-100">
      <h2 class="text-sm sm:text-base font-bold text-slate-800 flex items-center gap-2">
        <GitCommitHorizontal class="w-5 h-5 text-indigo-600" />
        {{ t('l_timeline_section_title') }}
      </h2>

      <div class="flex flex-wrap items-center gap-1.5">
        <button
          v-for="cat in categories"
          :key="cat.key"
          @click="selectedCategory = cat.key"
          :class="[
            'px-2.5 py-1 rounded-lg text-xs font-semibold transition-all',
            selectedCategory === cat.key
              ? 'bg-indigo-600 text-white shadow-2xs'
              : 'bg-slate-100 text-slate-600 hover:bg-slate-200',
          ]"
        >
          {{ cat.label }}
        </button>
      </div>
    </div>

    <!-- Timeline Vertical Track -->
    <div v-if="filteredMilestones.length === 0" class="py-10 text-center text-xs text-slate-400">
      {{ t('l_no_data') }}
    </div>

    <div v-else class="relative pl-6 space-y-4 before:absolute before:left-2.5 before:top-2 before:bottom-2 before:w-0.5 before:bg-slate-200">
      <div
        v-for="(ms, index) in filteredMilestones"
        :key="`${ms.codeOrRef}-${index}`"
        class="relative p-3.5 rounded-xl bg-slate-50/80 border border-slate-200/70 hover:bg-white hover:shadow-xs transition-all flex flex-col sm:flex-row sm:items-center justify-between gap-2"
      >
        <!-- Node Dot -->
        <div
          :class="[
            'w-3.5 h-3.5 rounded-full ring-4 absolute -left-[22px] top-4',
            getCategoryMeta(ms.category).dotClass,
          ]"
        />

        <div class="space-y-1">
          <div class="flex flex-wrap items-center gap-2">
            <span
              :class="[
                'inline-flex items-center gap-1 px-2 py-0.5 rounded-md text-[11px] font-bold border',
                getCategoryMeta(ms.category).badgeClass,
              ]"
            >
              <component :is="getCategoryMeta(ms.category).icon" class="w-3 h-3" />
              {{ getCategoryMeta(ms.category).label }}
            </span>

            <span class="text-xs font-mono font-bold text-slate-700">{{ ms.codeOrRef }}</span>
            <span class="text-xs text-slate-400">• {{ formatDate(ms.date) }}</span>
          </div>

          <p class="text-xs sm:text-sm text-slate-700 font-medium">
            {{ ms.description }}
          </p>
        </div>

        <div v-if="ms.amount && ms.amount > 0" class="text-right shrink-0">
          <span
            :class="[
              'font-mono font-bold text-xs sm:text-sm px-2.5 py-1 rounded-lg border',
              ms.category === 'Penalty'
                ? 'bg-rose-50 text-rose-700 border-rose-200'
                : 'bg-emerald-50 text-emerald-700 border-emerald-200',
            ]"
          >
            {{ ms.category === 'Penalty' ? '-' : '' }}{{ formatCurrency(ms.amount) }}
          </span>
        </div>
      </div>
    </div>
  </div>
</template>
