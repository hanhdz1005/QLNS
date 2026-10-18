<script setup lang="ts">
import { computed } from 'vue';
import { useRoute } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import { useI18n } from '@/locales';
import {
  LayoutDashboard,
  Users,
  Building2,
  Briefcase,
  FileText,
  CalendarCheck2,
  CalendarOff,
  DollarSign,
  Award,
  BarChart3,
  ShieldAlert,
  Sliders,
  X,
} from 'lucide-vue-next';

defineProps<{
  isOpen: boolean;
  isMobileOpen: boolean;
}>();

const emit = defineEmits<{
  (e: 'closeMobile'): void;
}>();

const route = useRoute();
const authStore = useAuthStore();
const { t } = useI18n();

const navigation = computed(() => [
  { name: t('n_dashboard'), path: '/', icon: LayoutDashboard },
  { name: t('n_employees'), path: '/employees', icon: Users },
  { name: t('n_departments'), path: '/departments', icon: Building2 },
  { name: t('n_positions'), path: '/positions', icon: Briefcase },
  { name: t('n_contracts'), path: '/contracts', icon: FileText },
  { name: t('n_attendance'), path: '/attendance', icon: CalendarCheck2 },
  { name: t('n_leaves'), path: '/leaves', icon: CalendarOff },
  { name: t('n_payroll'), path: '/payroll', icon: DollarSign },
  { name: t('n_salaries'), path: '/salaries', icon: Sliders },
  { name: t('n_bonus_penalty'), path: '/bonus-penalty', icon: Award },
  { name: t('n_reports'), path: '/reports', icon: BarChart3 },
]);
</script>

<template>
  <aside
    :class="[
      'fixed inset-y-0 left-0 z-50 flex flex-col bg-slate-900 text-slate-300 transition-all duration-300 ease-in-out lg:static lg:translate-x-0',
      isMobileOpen ? 'translate-x-0 w-64' : '-translate-x-full lg:translate-x-0',
      isOpen ? 'lg:w-64' : 'lg:w-20',
    ]"
  >
    <!-- Brand -->
    <div class="h-16 flex items-center justify-between px-5 border-b border-slate-800 bg-slate-950/40">
      <router-link to="/" class="flex items-center gap-3 overflow-hidden">
        <div class="w-10 h-10 rounded-xl bg-gradient-to-tr from-indigo-600 via-indigo-500 to-cyan-400 flex items-center justify-center text-white font-bold text-lg shadow-md shadow-indigo-500/20 shrink-0">
          HR
        </div>
        <div v-show="isOpen" class="flex flex-col whitespace-nowrap overflow-hidden transition-all duration-200">
          <span class="font-bold text-white tracking-wide text-base">HRMS PRO</span>
          <span class="text-[10px] text-indigo-400 font-medium tracking-wider uppercase">
            {{ t('n_sub_title') }}
          </span>
        </div>
      </router-link>

      <button
        @click="emit('closeMobile')"
        class="lg:hidden text-slate-400 hover:text-white p-1"
      >
        <X class="w-5 h-5" />
      </button>
    </div>

    <!-- Navigation Items -->
    <div class="flex-1 overflow-y-auto px-3 py-4 space-y-1">
      <div v-show="isOpen" class="px-3 pb-2 text-[10px] font-semibold text-slate-500 uppercase tracking-wider">
        {{ t('n_menu_title') }}
      </div>

      <router-link
        v-for="item in navigation"
        :key="item.path"
        :to="item.path"
        :title="!isOpen ? item.name : undefined"
        :class="[
          'flex items-center gap-3 px-3.5 py-2.5 rounded-xl text-sm font-medium transition-all group',
          route.path === item.path
            ? 'bg-indigo-600 text-white shadow-md shadow-indigo-600/30'
            : 'text-slate-400 hover:text-slate-100 hover:bg-slate-800/60',
        ]"
        @click="emit('closeMobile')"
      >
        <component
          :is="item.icon"
          :class="[
            'w-5 h-5 shrink-0 transition-transform group-hover:scale-105',
            route.path === item.path ? 'text-white' : 'text-slate-400 group-hover:text-indigo-400',
          ]"
        />
        <span v-show="isOpen" class="whitespace-nowrap transition-all duration-200">
          {{ item.name }}
        </span>
      </router-link>

      <!-- System Roles (Admin only) -->
      <div v-if="authStore.isAdmin" class="pt-4">
        <div v-show="isOpen" class="px-3 pb-2 text-[10px] font-semibold text-slate-500 uppercase tracking-wider">
          {{ t('n_system_title') }}
        </div>
        <router-link
          to="/roles"
          :title="!isOpen ? t('n_roles') : undefined"
          :class="[
            'flex items-center gap-3 px-3.5 py-2.5 rounded-xl text-sm font-medium transition-all group',
            route.path === '/roles'
              ? 'bg-indigo-600 text-white shadow-md shadow-indigo-600/30'
              : 'text-slate-400 hover:text-slate-100 hover:bg-slate-800/60',
          ]"
          @click="emit('closeMobile')"
        >
          <ShieldAlert class="w-5 h-5 shrink-0 text-slate-400 group-hover:text-indigo-400" />
          <span v-show="isOpen" class="whitespace-nowrap">
            {{ t('n_roles') }}
          </span>
        </router-link>
      </div>
    </div>

    <!-- Quick User Profile in Footer -->
    <div class="p-3 border-t border-slate-800 bg-slate-950/20">
      <router-link
        to="/profile"
        class="flex items-center gap-3 p-2 rounded-xl hover:bg-slate-800/60 transition-colors"
      >
        <div class="w-9 h-9 rounded-lg bg-indigo-500/20 text-indigo-400 font-semibold flex items-center justify-center shrink-0 border border-indigo-500/30">
          {{ authStore.user?.displayName?.charAt(0) || 'U' }}
        </div>
        <div v-show="isOpen" class="flex flex-col min-w-0">
          <span class="text-sm font-medium text-slate-200 truncate">
            {{ authStore.user?.displayName || t('l_default_user') }}
          </span>
          <span class="text-[11px] text-slate-500 truncate">
            {{ t('l_role') }}: {{ authStore.user?.role || 'User' }}
          </span>
        </div>
      </router-link>
    </div>
  </aside>
</template>
