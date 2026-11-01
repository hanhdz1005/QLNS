<script setup lang="ts">
import { useAuthStore } from '@/stores/auth';
import { useI18n } from '@/locales';
import { ShieldCheck, ScanFace, PlusCircle, Download } from 'lucide-vue-next';

defineProps<{
  exportLoading: boolean;
}>();

const emit = defineEmits<{
  (e: 'exportEmployees'): void;
}>();

const authStore = useAuthStore();
const { t } = useI18n();
</script>

<template>
  <div class="bg-gradient-to-r from-indigo-700 via-indigo-600 to-sky-600 rounded-2xl p-5 sm:p-6 text-white shadow-md shadow-indigo-500/10 flex flex-col md:flex-row items-start md:items-center justify-between gap-4 relative overflow-hidden shrink-0">
    <div class="relative z-10">
      <div class="flex items-center gap-2.5">
        <div class="inline-flex items-center gap-1.5 px-2.5 py-1 bg-white/15 backdrop-blur-md rounded-full text-xs font-medium text-white">
          <ShieldCheck class="w-3.5 h-3.5" />
          {{ t('l_enterprise_hrms') }}
        </div>
        <h1 class="text-xl sm:text-2xl font-bold tracking-tight">
          {{ t('l_greeting') }}, {{ authStore.user?.displayName || t('l_default_user') }}!
        </h1>
      </div>
      <p class="text-indigo-100 text-xs sm:text-sm mt-1 max-w-xl hidden sm:block">
        {{ t('l_welcome_desc') }}
      </p>
    </div>

    <!-- Quick Actions -->
    <div class="relative z-10 flex flex-wrap items-center gap-2.5">
      <router-link
        to="/attendance"
        class="inline-flex items-center gap-2 px-3.5 py-2 sm:px-4 sm:py-2.5 bg-white text-indigo-700 hover:bg-indigo-50 font-semibold text-xs sm:text-sm rounded-xl shadow-xs transition-all"
      >
        <ScanFace class="w-4 h-4" />
        {{ t('b_open_face_scanner') }}
      </router-link>

      <router-link
        v-if="authStore.isAdmin"
        to="/employees"
        class="inline-flex items-center gap-2 px-3.5 py-2 sm:px-4 sm:py-2.5 bg-indigo-800/80 hover:bg-indigo-800 text-white font-semibold text-xs sm:text-sm rounded-xl border border-indigo-400/30 transition-all"
      >
        <PlusCircle class="w-4 h-4" />
        {{ t('b_add_employee') }}
      </router-link>

      <button
        v-if="authStore.canManagePayroll"
        @click="emit('exportEmployees')"
        :disabled="exportLoading"
        class="inline-flex items-center gap-2 px-3.5 py-2 sm:px-4 sm:py-2.5 bg-white/10 hover:bg-white/20 text-white font-semibold text-xs sm:text-sm rounded-xl border border-white/20 transition-all"
      >
        <Download class="w-4 h-4" />
        {{ t('b_export_csv_staff') }}
      </button>
    </div>
  </div>
</template>
