<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import { useToastStore } from '@/stores/toast';
import { useI18n } from '@/locales';
import { Lock, Mail, ArrowRight } from 'lucide-vue-next';

const router = useRouter();
const authStore = useAuthStore();
const toastStore = useToastStore();
const { t } = useI18n();

const email = ref('admin@gmail.com');
const password = ref('Admin@1234');
const loading = ref(false);

const handleLogin = async () => {
  if (!email.value || !password.value) {
    toastStore.warning(t('e_enter_email_and_password'));
    return;
  }

  loading.value = true;
  try {
    const success = await authStore.login({
      email: email.value,
      password: password.value,
    });

    if (success) {
      toastStore.success(t('t_login_success'));
      router.push('/');
    } else {
      toastStore.error(t('e_login_failed'));
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_invalid_credentials');
    toastStore.error(msg);
  } finally {
    loading.value = false;
  }
};
</script>

<template>
  <div
    class="min-h-screen bg-slate-900 flex items-center justify-center p-4 relative overflow-hidden font-sans"
  >
    <!-- Background glowing accents -->
    <div
      class="absolute -top-40 -left-40 w-96 h-96 bg-indigo-500/20 rounded-full blur-3xl pointer-events-none"
    />
    <div
      class="absolute -bottom-40 -right-40 w-96 h-96 bg-cyan-500/20 rounded-full blur-3xl pointer-events-none"
    />

    <div
      class="w-full max-w-md bg-white rounded-3xl shadow-2xl p-8 sm:p-10 relative z-10 border border-slate-100"
    >
      <!-- Logo & Heading -->
      <div class="text-center mb-8">
        <div
          class="inline-flex items-center justify-center w-14 h-14 rounded-2xl bg-gradient-to-tr from-indigo-600 via-indigo-500 to-cyan-400 text-white font-bold text-2xl shadow-lg shadow-indigo-500/30 mb-4"
        >
          HR
        </div>
        <h1 class="text-2xl font-bold text-slate-800 tracking-tight">{{ t('l_login_page_title') }}</h1>
        <p class="text-xs text-slate-500 mt-1">{{ t('l_login_page_subtitle') }}</p>
      </div>

      <!-- Login Form -->
      <form @submit.prevent="handleLogin" class="space-y-4">
        <div>
          <label class="block text-xs font-semibold text-slate-700 uppercase tracking-wider mb-1.5">
            {{ t('l_account_email') }}
          </label>
          <div class="relative">
            <Mail class="w-5 h-5 text-slate-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
            <input
              v-model="email"
              type="email"
              required
              placeholder="admin@gmail.com"
              class="w-full pl-11 pr-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm text-slate-800 placeholder-slate-400 focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 transition-all"
            />
          </div>
        </div>

        <div>
          <label class="block text-xs font-semibold text-slate-700 uppercase tracking-wider mb-1.5">
            {{ t('l_password') }}
          </label>
          <div class="relative">
            <Lock class="w-5 h-5 text-slate-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
            <input
              v-model="password"
              type="password"
              required
              placeholder="••••••••"
              class="w-full pl-11 pr-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm text-slate-800 placeholder-slate-400 focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 transition-all"
            />
          </div>
        </div>

        <button
          type="submit"
          :disabled="loading"
          class="w-full mt-2 py-3 px-4 bg-indigo-600 hover:bg-indigo-700 active:bg-indigo-800 text-white font-semibold text-sm rounded-xl shadow-lg shadow-indigo-600/30 hover:shadow-indigo-600/40 transition-all flex items-center justify-center gap-2 disabled:opacity-50"
        >
          <span v-if="loading">{{ t('b_authenticating') }}</span>
          <span v-else class="flex items-center gap-2">
            {{ t('b_login') }}
            <ArrowRight class="w-4 h-4" />
          </span>
        </button>
      </form>
    </div>
  </div>
</template>
