<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import QRCode from 'qrcode';
import { attendanceApi } from '@/api/attendance.api';
import { useToastStore } from '@/stores/toast';
import { useI18n } from '@/locales';
import type { QrCheckInResultDto } from '@/types';
import { QrCode, RefreshCw, CheckCircle2, Clock } from 'lucide-vue-next';

const emit = defineEmits<{
  (e: 'recordUpdated'): void;
}>();

const toastStore = useToastStore();
const { t } = useI18n();
const qrCanvas = ref<HTMLCanvasElement | null>(null);
const currentQrToken = ref('');
const qrExpiresIn = ref(30);
const qrCountdown = ref(30);
const qrLoading = ref(false);
let qrTimer: ReturnType<typeof setInterval> | null = null;

const checkInResult = ref<QrCheckInResultDto | null>(null);
const checkInLoading = ref(false);

const generateQr = async () => {
  qrLoading.value = true;
  try {
    const res = await attendanceApi.getQrToken();
    if (res.isSuccess && res.data) {
      currentQrToken.value = res.data.token;
      qrExpiresIn.value = res.data.expiresInSeconds || 30;
      qrCountdown.value = res.data.expiresInSeconds || 30;

      if (qrCanvas.value) {
        await QRCode.toCanvas(qrCanvas.value, res.data.token, {
          width: 180,
          margin: 2,
          color: {
            dark: '#1e1b4b',
            light: '#ffffff',
          },
        });
      }
    }
  } catch (error) {
    console.error('Failed to generate QR token:', error);
  } finally {
    qrLoading.value = false;
  }
};

const startQrPolling = () => {
  generateQr();
  if (qrTimer) clearInterval(qrTimer);
  qrTimer = setInterval(() => {
    if (qrCountdown.value > 1) {
      qrCountdown.value--;
    } else {
      generateQr();
    }
  }, 1000);
};

const stopQrPolling = () => {
  if (qrTimer) {
    clearInterval(qrTimer);
    qrTimer = null;
  }
};

const handleScanQrNow = async () => {
  if (!currentQrToken.value) return;
  checkInLoading.value = true;
  try {
    const res = await attendanceApi.qrCheckIn(currentQrToken.value);
    if (res.isSuccess && res.data) {
      checkInResult.value = res.data;
      const successMsg = res.data.actionType === 'CheckIn'
        ? t('t_check_in_success')
        : t('t_check_out_success');
      toastStore.success(successMsg);
      emit('recordUpdated');
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_qr_scan_failed');
    toastStore.error(msg);
  } finally {
    checkInLoading.value = false;
  }
};

onMounted(() => {
  startQrPolling();
});

onUnmounted(() => {
  stopQrPolling();
});
</script>

<template>
  <div class="max-w-md mx-auto bg-white rounded-2xl border border-slate-200/80 shadow-xs p-4 sm:p-5 text-center space-y-3 shrink-0">
    <div class="space-y-0.5">
      <div class="inline-flex p-2 rounded-xl bg-indigo-50 text-indigo-600 mb-1">
        <QrCode class="w-6 h-6" />
      </div>
      <h2 class="text-base sm:text-lg font-bold text-slate-800">{{ t('l_tab_attendance_kiosk') }}</h2>
      <p class="text-xs text-slate-500 max-w-sm mx-auto">
        {{ t('l_qr_kiosk_subtitle') }}
      </p>
    </div>

    <!-- QR Canvas Display -->
    <div class="flex flex-col items-center justify-center p-3 bg-slate-50 rounded-xl border border-slate-200/60">
      <div class="relative bg-white p-2 rounded-xl shadow-xs border border-slate-100">
        <canvas ref="qrCanvas" class="rounded-lg"></canvas>
        <div v-if="qrLoading" class="absolute inset-0 bg-white/80 backdrop-blur-2xs rounded-lg flex items-center justify-center">
          <RefreshCw class="w-5 h-5 text-indigo-600 animate-spin" />
        </div>
      </div>

      <!-- Countdown Bar -->
      <div class="mt-2 flex items-center gap-1.5 text-xs font-medium text-slate-600">
        <Clock class="w-3.5 h-3.5 text-indigo-600 animate-pulse" />
        <span>{{ t('l_auto_refresh_in') }} <strong class="text-indigo-600 font-bold font-mono">{{ qrCountdown }}s</strong></span>
      </div>
    </div>

    <!-- Simulation Check-in Button -->
    <div class="pt-0.5">
      <button
        @click="handleScanQrNow"
        :disabled="checkInLoading || qrLoading"
        class="w-full sm:w-auto px-4 py-2 bg-gradient-to-r from-indigo-600 to-indigo-700 hover:from-indigo-700 hover:to-indigo-800 text-white font-semibold text-xs rounded-xl shadow-xs shadow-indigo-600/20 transition-all disabled:opacity-50 inline-flex items-center justify-center gap-1.5"
      >
        <CheckCircle2 class="w-3.5 h-3.5" />
        <span v-if="checkInLoading">{{ t('l_verifying_qr') }}</span>
        <span v-else>{{ t('b_scan_qr') }}</span>
      </button>
    </div>

    <!-- Success Result Banner -->
    <div
      v-if="checkInResult"
      class="p-2.5 rounded-xl bg-emerald-50 border border-emerald-200 text-left space-y-1 text-xs"
    >
      <div class="flex items-center gap-1.5 text-emerald-800 font-bold text-xs">
        <CheckCircle2 class="w-3.5 h-3.5 text-emerald-600" />
        {{ t('l_attendance_success') }}: {{ checkInResult.employeeName }}
      </div>
      <div class="text-[11px] text-emerald-700 flex flex-wrap gap-x-3 gap-y-0.5">
        <span>{{ t('l_action_type') }}: <strong>{{ checkInResult.actionType === 'CheckIn' ? t('l_check_in_action') : t('l_check_out_action') }}</strong></span>
        <span>{{ t('l_time') }}: <strong>{{ checkInResult.time }} ({{ checkInResult.date }})</strong></span>
        <span v-if="checkInResult.hoursWorked">{{ t('l_total_hours') }}: <strong>{{ checkInResult.hoursWorked }}h</strong></span>
      </div>
    </div>
  </div>
</template>
