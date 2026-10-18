<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import { useI18n } from '@/locales';
import { reportApi } from '@/api/report.api';
import type { HrAlertsDto } from '@/types';
import {
  Menu,
  QrCode,
  Bell,
  ChevronDown,
  User,
  LogOut,
  AlertCircle,
  Cake,
  Clock,
  Languages,
} from 'lucide-vue-next';

defineEmits<{
  (e: 'toggleSidebar'): void;
  (e: 'openMobileSidebar'): void;
}>();

const route = useRoute();
const authStore = useAuthStore();
const { locale, toggleLocale, t } = useI18n();

const isUserMenuOpen = ref(false);
const isAlertsOpen = ref(false);

const hrAlerts = ref<HrAlertsDto>({
  expiringContracts: [],
  upcomingBirthdays: [],
  probationEndingEmployees: [],
});

const totalAlertsCount = computed(() => {
  return (
    hrAlerts.value.expiringContracts.length +
    hrAlerts.value.upcomingBirthdays.length +
    hrAlerts.value.probationEndingEmployees.length
  );
});

const fetchAlerts = async () => {
  if (authStore.isAdmin || authStore.isLeader) {
    try {
      const res = await reportApi.getHrAlerts();
      if (res.isSuccess && res.data) {
        hrAlerts.value = res.data;
      }
    } catch {
      // ignore in background polling
    }
  }
};

onMounted(() => {
  fetchAlerts();
});

const handleLogout = () => {
  authStore.logout();
};
</script>

<template>
  <header class="h-13 sm:h-14 bg-white border-b border-slate-200/80 shrink-0 z-30 flex items-center justify-between px-3 sm:px-5 shadow-2xs">
    <div class="flex items-center gap-3">
      <button
        @click="$emit('openMobileSidebar')"
        class="lg:hidden p-2 rounded-lg text-slate-500 hover:bg-slate-100 transition-colors"
      >
        <Menu class="w-5 h-5" />
      </button>
      <button
        @click="$emit('toggleSidebar')"
        class="hidden lg:flex p-2 rounded-lg text-slate-500 hover:bg-slate-100 transition-colors"
        :title="t('l_toggle_menu')"
      >
        <Menu class="w-5 h-5" />
      </button>

      <!-- Page Title -->
      <div class="flex items-center gap-2">
        <span class="text-sm font-semibold text-slate-800 hidden sm:inline">
          {{ route.meta.title ? String(route.meta.title) : t('l_system_name') }}
        </span>
      </div>
    </div>

    <!-- Header Right Actions -->
    <div class="flex items-center gap-2.5">
      <!-- Language Toggle Switcher -->
      <button
        @click="toggleLocale"
        class="inline-flex items-center gap-1.5 px-2.5 py-1 text-xs font-semibold text-slate-700 bg-slate-100 hover:bg-slate-200 rounded-lg transition-colors border border-slate-200"
        :title="locale === 'vi' ? 'Switch to English' : 'Chuyển sang Tiếng Việt'"
      >
        <Languages class="w-3.5 h-3.5 text-indigo-600" />
        <span>{{ locale === 'vi' ? 'VI' : 'EN' }}</span>
      </button>

      <router-link
        to="/attendance"
        class="hidden sm:inline-flex items-center gap-2 px-3 py-1.5 text-xs font-medium text-indigo-700 bg-indigo-50 hover:bg-indigo-100 border border-indigo-200 rounded-lg transition-colors"
      >
        <QrCode class="w-4 h-4" />
        {{ t('b_scan_qr') }}
      </router-link>

      <!-- HR Alerts Dropdown (Admin/Leader) -->
      <div v-if="authStore.isAdmin || authStore.isLeader" class="relative">
        <button
          @click="isAlertsOpen = !isAlertsOpen; isUserMenuOpen = false"
          class="relative p-2 rounded-xl text-slate-500 hover:bg-slate-100 transition-colors"
          :title="t('l_hr_alerts_title')"
        >
          <Bell class="w-5 h-5" />
          <span
            v-if="totalAlertsCount > 0"
            class="absolute top-1.5 right-1.5 w-4 h-4 rounded-full bg-rose-500 text-white text-[10px] font-bold flex items-center justify-center ring-2 ring-white"
          >
            {{ totalAlertsCount }}
          </span>
        </button>

        <!-- Alerts Menu Popover -->
        <div
          v-if="isAlertsOpen"
          class="absolute right-0 mt-2 w-80 sm:w-96 bg-white rounded-2xl shadow-xl border border-slate-100 py-3 z-50 animate-fade-in divide-y divide-slate-100"
        >
          <div class="px-4 pb-2 flex items-center justify-between">
            <h4 class="text-xs font-semibold text-slate-800 uppercase tracking-wider">
              {{ t('l_hr_alerts_title') }} ({{ totalAlertsCount }})
            </h4>
          </div>

          <div class="max-h-80 overflow-y-auto px-2 py-2 space-y-2 text-xs">
            <div v-if="totalAlertsCount === 0" class="py-6 text-center text-slate-400">
              {{ t('l_no_alerts') }}
            </div>

            <!-- Expiring contracts -->
            <div v-if="hrAlerts.expiringContracts.length > 0" class="space-y-1">
              <div class="px-2 py-1 text-[11px] font-semibold text-amber-600 flex items-center gap-1.5 bg-amber-50 rounded-lg">
                <AlertCircle class="w-3.5 h-3.5" /> {{ t('l_expiring_contracts_alert') }}
              </div>
              <div
                v-for="c in hrAlerts.expiringContracts"
                :key="c.contractId"
                class="px-2 py-1.5 rounded-lg hover:bg-slate-50 transition-colors flex justify-between items-center"
              >
                <div>
                  <span class="font-medium text-slate-800">{{ c.employeeName }}</span>
                  <span class="text-[10px] text-slate-400 block">{{ t('l_contract_number') }}: {{ c.contractNumber }}</span>
                </div>
                <span class="text-rose-600 font-semibold text-[11px]">
                  {{ t('l_days_remaining') }} {{ c.daysRemaining }} {{ t('l_days_unit') }}
                </span>
              </div>
            </div>

            <!-- Birthdays -->
            <div v-if="hrAlerts.upcomingBirthdays.length > 0" class="space-y-1">
              <div class="px-2 py-1 text-[11px] font-semibold text-pink-600 flex items-center gap-1.5 bg-pink-50 rounded-lg">
                <Cake class="w-3.5 h-3.5" /> {{ t('l_birthdays_alert') }}
              </div>
              <div
                v-for="b in hrAlerts.upcomingBirthdays"
                :key="b.employeeId"
                class="px-2 py-1.5 rounded-lg hover:bg-slate-50 transition-colors flex justify-between items-center"
              >
                <div>
                  <span class="font-medium text-slate-800">{{ b.employeeName }}</span>
                  <span class="text-[10px] text-slate-400 block">{{ b.departmentName }}</span>
                </div>
                <span class="text-pink-600 font-semibold text-[11px]">
                  {{ t('l_day_prefix') }} {{ b.day }}
                </span>
              </div>
            </div>

            <!-- Probation endings -->
            <div v-if="hrAlerts.probationEndingEmployees.length > 0" class="space-y-1">
              <div class="px-2 py-1 text-[11px] font-semibold text-indigo-600 flex items-center gap-1.5 bg-indigo-50 rounded-lg">
                <Clock class="w-3.5 h-3.5" /> {{ t('l_probation_ending_alert') }}
              </div>
              <div
                v-for="p in hrAlerts.probationEndingEmployees"
                :key="p.employeeId"
                class="px-2 py-1.5 rounded-lg hover:bg-slate-50 transition-colors flex justify-between items-center"
              >
                <div>
                  <span class="font-medium text-slate-800">{{ p.employeeName }}</span>
                  <span class="text-[10px] text-slate-400 block">{{ p.departmentName }}</span>
                </div>
                <span class="text-indigo-600 font-semibold text-[11px]">
                  {{ t('l_days_remaining') }} {{ p.daysRemaining }} {{ t('l_days_unit') }}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- User Menu -->
      <div class="relative">
        <button
          @click="isUserMenuOpen = !isUserMenuOpen; isAlertsOpen = false"
          class="flex items-center gap-2 p-1.5 rounded-xl hover:bg-slate-100 transition-colors"
        >
          <div class="w-8 h-8 rounded-lg bg-indigo-600 text-white font-semibold flex items-center justify-center text-xs shadow-xs">
            {{ authStore.user?.displayName?.charAt(0) || 'A' }}
          </div>
          <div class="hidden md:flex flex-col text-left">
            <span class="text-xs font-semibold text-slate-700 truncate max-w-[120px]">
              {{ authStore.user?.displayName || t('l_account') }}
            </span>
            <span class="text-[10px] text-slate-400">
              {{ authStore.user?.role || 'User' }}
            </span>
          </div>
          <ChevronDown class="w-3.5 h-3.5 text-slate-400" />
        </button>

        <!-- Dropdown Menu -->
        <div
          v-if="isUserMenuOpen"
          class="absolute right-0 mt-2 w-56 bg-white rounded-2xl shadow-xl border border-slate-100 py-1.5 z-50 animate-fade-in"
        >
          <div class="px-4 py-2.5 border-b border-slate-100 mb-1">
            <div class="text-xs font-semibold text-slate-800">{{ authStore.user?.displayName }}</div>
            <div class="text-[11px] text-slate-400 truncate">{{ t('l_role') }}: {{ authStore.user?.role }}</div>
          </div>

          <router-link
            to="/profile"
            @click="isUserMenuOpen = false"
            class="flex items-center gap-2.5 px-4 py-2 text-xs text-slate-700 hover:bg-slate-50 transition-colors"
          >
            <User class="w-4 h-4 text-slate-400" />
            {{ t('l_my_profile_and_attendance') }}
          </router-link>

          <button
            @click="handleLogout"
            class="w-full flex items-center gap-2.5 px-4 py-2 text-xs text-rose-600 hover:bg-rose-50 transition-colors text-left"
          >
            <LogOut class="w-4 h-4 text-rose-500" />
            {{ t('b_logout') }}
          </button>
        </div>
      </div>
    </div>
  </header>
</template>
