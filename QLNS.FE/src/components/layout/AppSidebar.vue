<script setup lang="ts">
import { computed } from 'vue';
import { useRoute } from 'vue-router';
import { useAuthStore, type AppRole } from '@/stores/auth';
import { useCompanySettingsStore } from '@/stores/companySettings';
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
  UserCircle,
  GitBranchPlus,
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
const settingsStore = useCompanySettingsStore();
const { t } = useI18n();

const allRoles: AppRole[] = [
  'Admin',
  'HRManager',
  'HRStaff',
  'Accountant',
  'DepartmentManager',
  'Employee',
];

const roleLabel = computed(() => {
  switch (authStore.normalizedRole) {
    case 'Admin':
      return t('s_role_admin');
    case 'HRManager':
      return t('s_role_hr_manager');
    case 'HRStaff':
      return t('s_role_hr_staff');
    case 'Accountant':
      return t('s_role_accountant');
    case 'DepartmentManager':
      return t('s_role_department_manager');
    default:
      return t('s_role_employee');
  }
});

const navigation = computed(() => {
  const items: Array<{
    name: string;
    path: string;
    icon: unknown;
    roles: AppRole[];
  }> = [
    { name: t('n_dashboard'), path: '/', icon: LayoutDashboard, roles: allRoles },
    {
      name: authStore.canViewAllLifecycles ? t('n_lifecycle') : t('n_my_lifecycle'),
      path: '/lifecycle',
      icon: GitBranchPlus,
      roles: allRoles,
    },
    {
      name: t('n_employees'),
      path: '/employees',
      icon: Users,
      roles: ['Admin', 'HRManager', 'HRStaff', 'Accountant', 'DepartmentManager'],
    },
    {
      name: t('n_departments'),
      path: '/departments',
      icon: Building2,
      roles: ['Admin', 'HRManager', 'HRStaff', 'Accountant'],
    },
    {
      name: t('n_positions'),
      path: '/positions',
      icon: Briefcase,
      roles: ['Admin', 'HRManager', 'HRStaff', 'Accountant'],
    },
    {
      name: t('n_contracts'),
      path: '/contracts',
      icon: FileText,
      roles: ['Admin', 'HRManager', 'HRStaff', 'Accountant'],
    },
    { name: t('n_attendance'), path: '/attendance', icon: CalendarCheck2, roles: allRoles },
    { name: t('n_leaves'), path: '/leaves', icon: CalendarOff, roles: allRoles },
    {
      name: authStore.canManagePayroll ? t('n_payroll') : t('n_my_payslip'),
      path: '/payroll',
      icon: DollarSign,
      roles: allRoles,
    },
    {
      name: t('n_salaries'),
      path: '/salaries',
      icon: Sliders,
      roles: ['Admin', 'HRManager', 'Accountant'],
    },
    {
      name: t('n_bonus_penalty'),
      path: '/bonus-penalty',
      icon: Award,
      roles: ['Admin', 'HRManager', 'HRStaff', 'Accountant'],
    },
    {
      name: t('n_reports'),
      path: '/reports',
      icon: BarChart3,
      roles: ['Admin', 'HRManager', 'HRStaff', 'Accountant', 'DepartmentManager'],
    },
    { name: t('n_profile'), path: '/profile', icon: UserCircle, roles: allRoles },
  ];

  return items.filter((item) => authStore.hasAnyRole(item.roles));
});

const isRouteActive = (itemPath: string) => {
  if (itemPath === '/lifecycle') {
    return route.path.startsWith('/lifecycle');
  }
  return route.path === itemPath;
};
</script>

<template>
  <aside
    :class="[
      'fixed inset-y-0 left-0 z-50 flex flex-col transition-all duration-300 ease-in-out lg:static lg:translate-x-0',
      settingsStore.currentTheme.sidebarBgClass,
      isMobileOpen ? 'translate-x-0 w-64' : '-translate-x-full lg:translate-x-0',
      isOpen ? 'lg:w-64' : 'lg:w-20',
    ]"
  >
    <!-- Brand -->
    <div
      :class="[
        'h-16 flex items-center justify-between px-5 border-b',
        settingsStore.currentTheme.sidebarHeaderClass,
      ]"
    >
      <router-link to="/" class="flex items-center gap-3 overflow-hidden">
        <div
          :class="[
            'w-10 h-10 rounded-xl flex items-center justify-center text-white font-bold text-lg shadow-md shrink-0',
            settingsStore.currentTheme.brandBadgeClass,
          ]"
        >
          HR
        </div>
        <div v-show="isOpen" class="flex flex-col whitespace-nowrap overflow-hidden transition-all duration-200">
          <span class="font-bold text-white tracking-wide text-base">HRMS PRO</span>
          <span
            :class="[
              'text-[10px] font-medium tracking-wider uppercase',
              settingsStore.currentTheme.brandSubtitleClass,
            ]"
          >
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
      <div v-show="isOpen" class="px-3 pb-2 text-[10px] font-semibold opacity-60 uppercase tracking-wider">
        {{ t('n_menu_title') }}
      </div>

      <router-link
        v-for="item in navigation"
        :key="item.path"
        :to="item.path"
        :title="!isOpen ? item.name : undefined"
        :class="[
          'flex items-center gap-3 px-3.5 py-2.5 rounded-xl text-sm font-medium transition-all group',
          isRouteActive(item.path)
            ? settingsStore.currentTheme.sidebarActiveClass
            : settingsStore.currentTheme.sidebarHoverClass,
        ]"
        @click="emit('closeMobile')"
      >
        <component
          :is="item.icon"
          :class="[
            'w-5 h-5 shrink-0 transition-transform group-hover:scale-105',
            isRouteActive(item.path) ? 'text-white' : 'opacity-75 group-hover:opacity-100',
          ]"
        />
        <span v-show="isOpen" class="whitespace-nowrap transition-all duration-200">
          {{ item.name }}
        </span>
      </router-link>

      <!-- System Roles (Admin only) -->
      <div v-if="authStore.isAdmin" class="pt-4">
        <div v-show="isOpen" class="px-3 pb-2 text-[10px] font-semibold opacity-60 uppercase tracking-wider">
          {{ t('n_system_title') }}
        </div>
        <router-link
          to="/roles"
          :title="!isOpen ? t('n_roles') : undefined"
          :class="[
            'flex items-center gap-3 px-3.5 py-2.5 rounded-xl text-sm font-medium transition-all group',
            route.path === '/roles'
              ? settingsStore.currentTheme.sidebarActiveClass
              : settingsStore.currentTheme.sidebarHoverClass,
          ]"
          @click="emit('closeMobile')"
        >
          <ShieldAlert class="w-5 h-5 shrink-0 opacity-75 group-hover:opacity-100" />
          <span v-show="isOpen" class="whitespace-nowrap">
            {{ t('n_roles') }}
          </span>
        </router-link>
      </div>
    </div>

    <!-- Quick User Profile in Footer -->
    <div :class="['p-3 border-t', settingsStore.currentTheme.sidebarFooterClass]">
      <router-link
        to="/profile"
        class="flex items-center gap-3 p-2 rounded-xl hover:bg-white/10 transition-colors"
      >
        <div class="w-9 h-9 rounded-lg bg-white/15 text-white font-semibold flex items-center justify-center shrink-0 border border-white/20">
          {{ authStore.user?.displayName?.charAt(0) || 'U' }}
        </div>
        <div v-show="isOpen" class="flex flex-col min-w-0">
          <span class="text-sm font-medium text-white truncate">
            {{ authStore.user?.displayName || t('l_default_user') }}
          </span>
          <span class="text-[11px] opacity-70 truncate">
            {{ roleLabel }}
          </span>
        </div>
      </router-link>
    </div>
  </aside>
</template>
