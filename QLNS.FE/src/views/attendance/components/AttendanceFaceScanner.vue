<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useToastStore } from '@/stores/toast';
import { useI18n } from '@/locales';
import { useCamera } from '@/composables/useCamera';
import { useRealtimeFaceScan } from '@/composables/useRealtimeFaceScan';
import { getFaceErrorLocaleKey, type FaceExtractionResult } from '@/utils/faceAnalyzer';
import { attendanceApi } from '@/api/attendance.api';
import type { FaceCheckInResultDto } from '@/types';
import FaceScannerResultPanel from './FaceScannerResultPanel.vue';
import {
  Camera,
  CameraOff,
  ScanFace,
  RefreshCw,
  ShieldAlert,
  Sparkles,
} from 'lucide-vue-next';

const emit = defineEmits<{
  (e: 'recordUpdated'): void;
  (e: 'openUpdateFace'): void;
}>();

const authStore = useAuthStore();
const toastStore = useToastStore();
const { t } = useI18n();

const {
  videoRef,
  isCameraConnected,
  cameraLoading,
  cameraErrorCode,
  connectCamera,
  disconnectCamera,
  simulateCameraError,
} = useCamera();

const checkInLoading = ref(false);
const checkInResult = ref<FaceCheckInResultDto | null>(null);
const verificationErrorCode = ref<string | null>(null);
const lastSnapshotPreview = ref('');

const getErrorMessageByCode = (code: string | null) => t(getFaceErrorLocaleKey(code));

const submitExtractedFace = async (extracted: FaceExtractionResult): Promise<boolean> => {
  verificationErrorCode.value = null;
  lastSnapshotPreview.value = extracted.imageBase64;
  checkInLoading.value = true;

  try {
    const res = await attendanceApi.faceCheckIn({
      capturedImageBase64: extracted.imageBase64,
      capturedFaceDescriptor: extracted.descriptor,
    });

    if (res.isSuccess && res.data) {
      checkInResult.value = res.data;
      const msg =
        res.data.actionType === 'CheckIn'
          ? t('t_face_check_in_success')
          : t('t_face_check_out_success');
      toastStore.success(msg);
      emit('recordUpdated');
      return true;
    } else {
      verificationErrorCode.value = res.errorCode || 'FACE_MISMATCH';
      toastStore.error(getErrorMessageByCode(verificationErrorCode.value));
      return false;
    }
  } catch (err: unknown) {
    const errPayload = (
      err as {
        response?: {
          data?: { errorCode?: string; data?: FaceCheckInResultDto };
        };
      }
    )?.response?.data;

    const code = errPayload?.errorCode || errPayload?.data?.errorCode || 'FACE_MISMATCH';
    verificationErrorCode.value = code;
    if (errPayload?.data) {
      checkInResult.value = errPayload.data;
    }
    toastStore.error(getErrorMessageByCode(code));
    return false;
  } finally {
    checkInLoading.value = false;
  }
};

const {
  autoScanEnabled,
  scanState,
  scanProgress,
  cooldownSeconds,
  startAutoScanLoop,
  stopAutoScanLoop,
  resetProgress,
} = useRealtimeFaceScan(videoRef, isCameraConnected, submitExtractedFace, {
  requiredStableFrames: 4,
  intervalMs: 280,
  cooldownTotalSeconds: 8,
});

const handleConnectCamera = async () => {
  verificationErrorCode.value = null;
  const { connected, errorCode } = await connectCamera();
  if (connected) {
    toastStore.info(t('t_camera_connected'));
    startAutoScanLoop();
  } else if (errorCode) {
    toastStore.error(getErrorMessageByCode(errorCode));
  }
};

const handleDisconnectCamera = () => {
  stopAutoScanLoop();
  disconnectCamera();
};

const handleSimulateCameraError = async () => {
  stopAutoScanLoop();
  checkInResult.value = null;
  verificationErrorCode.value = null;
  await simulateCameraError('CAMERA_CONNECT_FAILED');
  toastStore.error(t('e_camera_connect_failed'));
};

onMounted(() => {
  void handleConnectCamera();
});
</script>

<template>
  <div class="w-full max-w-4xl mx-auto bg-white rounded-3xl border border-slate-200/80 shadow-xs p-5 sm:p-6 space-y-5">
    <!-- Top Header -->
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-4 pb-4 border-b border-slate-100">
      <div class="flex items-center gap-3">
        <div class="w-11 h-11 rounded-2xl bg-indigo-50 text-indigo-600 flex items-center justify-center shrink-0">
          <ScanFace class="w-6 h-6" />
        </div>
        <div>
          <h2 class="text-base sm:text-lg font-bold text-slate-800">{{ t('l_face_scanner_title') }}</h2>
          <p class="text-xs text-slate-500">{{ t('l_face_scanner_subtitle') }}</p>
        </div>
      </div>

      <div class="flex items-center gap-2 self-start md:self-auto">
        <button
          type="button"
          :class="[
            'inline-flex items-center gap-1.5 px-3 py-2 rounded-xl border text-xs font-semibold transition-colors',
            autoScanEnabled
              ? 'border-emerald-200 bg-emerald-50 text-emerald-700'
              : 'border-slate-200 bg-slate-50 text-slate-600',
          ]"
          @click="autoScanEnabled = !autoScanEnabled"
        >
          <Sparkles class="w-3.5 h-3.5" />
          {{ autoScanEnabled ? t('s_auto_scan_on') : t('s_auto_scan_off') }}
        </button>

        <button
          type="button"
          class="inline-flex items-center gap-1.5 px-3.5 py-2 rounded-xl border border-slate-200 bg-slate-50 hover:bg-slate-100 text-xs font-semibold text-slate-700 transition-colors"
          @click="emit('openUpdateFace')"
        >
          <RefreshCw class="w-3.5 h-3.5 text-indigo-600" />
          {{ t('b_update_face') }}
        </button>
      </div>
    </div>

    <!-- Step Status Strip (B1 -> B4) -->
    <div class="grid grid-cols-2 sm:grid-cols-4 gap-2.5 text-xs">
      <div class="p-2.5 rounded-xl border border-slate-200/80 bg-slate-50 flex items-center justify-between">
        <span class="font-semibold text-slate-700">{{ t('l_face_step_b1') }}</span>
        <span
          :class="[
            'px-2 py-0.5 rounded-full text-[10px] font-bold',
            authStore.user?.hasRegisteredFace ? 'bg-emerald-100 text-emerald-700' : 'bg-amber-100 text-amber-700',
          ]"
        >
          {{ authStore.user?.hasRegisteredFace ? t('s_face_registered') : t('s_face_not_registered') }}
        </span>
      </div>

      <div class="p-2.5 rounded-xl border border-slate-200/80 bg-slate-50 flex items-center justify-between">
        <span class="font-semibold text-slate-700">{{ t('l_face_step_b2') }}</span>
        <span
          :class="[
            'px-2 py-0.5 rounded-full text-[10px] font-bold',
            isCameraConnected ? 'bg-emerald-100 text-emerald-700' : cameraErrorCode ? 'bg-rose-100 text-rose-700' : 'bg-slate-200 text-slate-700',
          ]"
        >
          {{ isCameraConnected ? t('s_camera_connected') : cameraErrorCode ? t('s_camera_error') : t('s_camera_disconnected') }}
        </span>
      </div>

      <div class="p-2.5 rounded-xl border border-slate-200/80 bg-slate-50 flex items-center justify-between">
        <span class="font-semibold text-slate-700">{{ t('l_face_step_b3') }}</span>
        <span class="px-2 py-0.5 rounded-full bg-indigo-100 text-indigo-700 text-[10px] font-bold font-mono">
          {{ scanProgress }}%
        </span>
      </div>

      <div class="p-2.5 rounded-xl border border-slate-200/80 bg-slate-50 flex items-center justify-between">
        <span class="font-semibold text-slate-700">{{ t('l_face_step_b4') }}</span>
        <span
          :class="[
            'px-2 py-0.5 rounded-full text-[10px] font-bold',
            checkInResult?.success ? 'bg-emerald-100 text-emerald-700' : cameraErrorCode || verificationErrorCode ? 'bg-rose-100 text-rose-700' : 'bg-slate-200 text-slate-600',
          ]"
        >
          {{ checkInResult?.success ? t('s_face_matched') : cameraErrorCode || verificationErrorCode ? cameraErrorCode || verificationErrorCode : t('s_camera_disconnected') }}
        </span>
      </div>
    </div>

    <!-- Main Camera & Real-Time Result Panel -->
    <div class="grid grid-cols-1 lg:grid-cols-12 gap-5 items-start">
      <div class="lg:col-span-7 space-y-3">
        <div
          class="relative aspect-4/3 rounded-2xl bg-slate-900 overflow-hidden border-2 border-indigo-500/30 shadow-inner flex items-center justify-center"
        >
          <video
            v-show="isCameraConnected"
            ref="videoRef"
            autoplay
            playsinline
            muted
            class="w-full h-full object-cover -scale-x-100"
          />

          <!-- Real-Time Radar & Stability Progress Overlay -->
          <div
            v-if="isCameraConnected"
            class="absolute inset-0 pointer-events-none flex flex-col items-center justify-center"
          >
            <div
              :class="[
                'w-44 h-56 rounded-full border-2 transition-all duration-200',
                scanState === 'locking' || scanState === 'verifying'
                  ? 'border-emerald-400 shadow-[0_0_35px_rgba(16,185,129,0.55)] scale-105'
                  : 'border-dashed border-indigo-400/80 shadow-[0_0_20px_rgba(99,102,241,0.3)]',
              ]"
            />
            <div class="mt-3 w-56 bg-slate-900/85 backdrop-blur-xs px-3 py-2 rounded-2xl border border-slate-700 space-y-1.5">
              <div class="flex items-center justify-between text-[11px] font-semibold text-emerald-300">
                <span>{{ checkInLoading ? t('l_face_analyzing') : t('l_face_locking_progress') }}</span>
                <span class="font-mono">{{ scanProgress }}%</span>
              </div>
              <div class="w-full h-1.5 bg-slate-700 rounded-full overflow-hidden">
                <div
                  class="h-full bg-gradient-to-r from-indigo-500 to-emerald-400 transition-all duration-200"
                  :style="{ width: `${scanProgress}%` }"
                />
              </div>
            </div>
          </div>

          <div
            v-if="!isCameraConnected"
            class="flex flex-col items-center justify-center text-center p-6 max-w-sm space-y-2"
          >
            <div class="w-12 h-12 rounded-2xl bg-slate-800 flex items-center justify-center text-slate-400">
              <CameraOff class="w-6 h-6" />
            </div>
            <p class="text-sm font-bold text-white">{{ t('l_camera_not_connected') }}</p>
            <p class="text-xs text-slate-400 leading-relaxed">{{ t('l_camera_not_connected_desc') }}</p>
          </div>
        </div>

        <!-- Camera Controls -->
        <div class="flex flex-wrap items-center justify-between gap-2.5">
          <div class="flex flex-wrap items-center gap-2">
            <button
              v-if="!isCameraConnected"
              type="button"
              :disabled="cameraLoading"
              class="inline-flex items-center gap-2 px-4 py-2.5 rounded-xl bg-indigo-600 hover:bg-indigo-700 text-white text-xs font-semibold shadow-sm transition-all disabled:opacity-50"
              @click="handleConnectCamera"
            >
              <Camera class="w-4 h-4" />
              {{ cameraErrorCode ? t('b_reconnect_camera') : t('b_connect_camera') }}
            </button>

            <button
              v-else
              type="button"
              class="inline-flex items-center gap-1.5 px-3.5 py-2.5 rounded-xl border border-slate-300 bg-white hover:bg-slate-50 text-slate-700 text-xs font-semibold transition-colors"
              @click="handleDisconnectCamera"
            >
              <CameraOff class="w-4 h-4 text-rose-500" />
              {{ t('b_disconnect_camera') }}
            </button>

            <button
              type="button"
              class="inline-flex items-center gap-1.5 px-3 py-2.5 rounded-xl border border-rose-200 bg-rose-50 hover:bg-rose-100 text-rose-700 text-xs font-semibold transition-colors"
              @click="handleSimulateCameraError"
            >
              <ShieldAlert class="w-3.5 h-3.5" />
              {{ t('b_simulate_camera_error') }}
            </button>
          </div>
        </div>
      </div>

      <!-- Result / Error Code Panel (B4) -->
      <div class="lg:col-span-5">
        <FaceScannerResultPanel
          :camera-error-code="cameraErrorCode"
          :verification-error-code="verificationErrorCode"
          :check-in-result="checkInResult"
          :last-snapshot-preview="lastSnapshotPreview"
          :cooldown-seconds="cooldownSeconds"
          @scan-next-now="resetProgress"
        />
      </div>
    </div>
  </div>
</template>
