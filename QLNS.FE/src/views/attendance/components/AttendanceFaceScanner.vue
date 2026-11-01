<script setup lang="ts">
import { ref } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useToastStore } from '@/stores/toast';
import { useI18n } from '@/locales';
import { useCamera } from '@/composables/useCamera';
import { extractFaceFromVideo, getFaceErrorLocaleKey } from '@/utils/faceAnalyzer';
import { attendanceApi } from '@/api/attendance.api';
import type { FaceCheckInResultDto } from '@/types';
import {
  Camera,
  CameraOff,
  AlertTriangle,
  ScanFace,
  RefreshCw,
  ShieldAlert,
  UserCheck,
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

// Step B2: Request Camera Connection
const handleConnectCamera = async () => {
  verificationErrorCode.value = null;
  checkInResult.value = null;
  const { connected, errorCode } = await connectCamera();
  if (connected) {
    toastStore.info(t('t_camera_connected'));
  } else if (errorCode) {
    toastStore.error(getErrorMessageByCode(errorCode));
  }
};

const handleSimulateCameraError = async () => {
  checkInResult.value = null;
  verificationErrorCode.value = null;
  await simulateCameraError('CAMERA_CONNECT_FAILED');
  toastStore.error(t('e_camera_connect_failed'));
};

// Step B3 & B4: Capture Face -> Send to BE -> Compare with DB -> Return Result or Error Code
const handleCaptureAndCheckIn = async () => {
  if (!videoRef.value || !isCameraConnected.value) {
    await simulateCameraError('CAMERA_CONNECT_FAILED');
    toastStore.error(t('e_camera_connect_failed'));
    return;
  }

  verificationErrorCode.value = null;
  checkInResult.value = null;

  const extracted = extractFaceFromVideo(videoRef.value);
  if (!extracted.valid) {
    verificationErrorCode.value = extracted.errorCode || 'FACE_NOT_DETECTED';
    toastStore.error(t('e_face_not_detected'));
    return;
  }

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
    } else {
      verificationErrorCode.value = res.errorCode || 'FACE_MISMATCH';
      toastStore.error(getErrorMessageByCode(verificationErrorCode.value));
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
  } finally {
    checkInLoading.value = false;
  }
};
</script>

<template>
  <div class="w-full max-w-4xl mx-auto bg-white rounded-3xl border border-slate-200/80 shadow-xs p-5 sm:p-6 space-y-5">
    <!-- Top Header & 4-Step Flow Indicators -->
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

      <button
        type="button"
        class="inline-flex items-center gap-1.5 px-3.5 py-2 rounded-xl border border-slate-200 bg-slate-50 hover:bg-slate-100 text-xs font-semibold text-slate-700 transition-colors self-start md:self-auto"
        @click="emit('openUpdateFace')"
      >
        <RefreshCw class="w-3.5 h-3.5 text-indigo-600" />
        {{ t('b_update_face') }}
      </button>
    </div>

    <!-- Step Status Strip (B1 -> B4) -->
    <div class="grid grid-cols-2 sm:grid-cols-4 gap-2.5 text-xs">
      <div class="p-2.5 rounded-xl border border-slate-200/80 bg-slate-50 flex items-center justify-between">
        <span class="font-semibold text-slate-700">{{ t('l_face_step_b1') }}</span>
        <span
          :class="[
            'px-2 py-0.5 rounded-full text-[10px] font-bold',
            authStore.user?.hasRegisteredFace
              ? 'bg-emerald-100 text-emerald-700'
              : 'bg-amber-100 text-amber-700',
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
            isCameraConnected
              ? 'bg-emerald-100 text-emerald-700'
              : cameraErrorCode
                ? 'bg-rose-100 text-rose-700'
                : 'bg-slate-200 text-slate-700',
          ]"
        >
          {{
            isCameraConnected
              ? t('s_camera_connected')
              : cameraErrorCode
                ? t('s_camera_error')
                : t('s_camera_disconnected')
          }}
        </span>
      </div>

      <div class="p-2.5 rounded-xl border border-slate-200/80 bg-slate-50 flex items-center justify-between">
        <span class="font-semibold text-slate-700">{{ t('l_face_step_b3') }}</span>
        <span class="px-2 py-0.5 rounded-full bg-indigo-100 text-indigo-700 text-[10px] font-bold">
          {{ lastSnapshotPreview ? t('s_face_registered') : t('s_camera_disconnected') }}
        </span>
      </div>

      <div class="p-2.5 rounded-xl border border-slate-200/80 bg-slate-50 flex items-center justify-between">
        <span class="font-semibold text-slate-700">{{ t('l_face_step_b4') }}</span>
        <span
          :class="[
            'px-2 py-0.5 rounded-full text-[10px] font-bold',
            checkInResult?.success
              ? 'bg-emerald-100 text-emerald-700'
              : cameraErrorCode || verificationErrorCode
                ? 'bg-rose-100 text-rose-700'
                : 'bg-slate-200 text-slate-600',
          ]"
        >
          {{
            checkInResult?.success
              ? t('s_face_matched')
              : cameraErrorCode || verificationErrorCode
                ? cameraErrorCode || verificationErrorCode
                : t('s_camera_disconnected')
          }}
        </span>
      </div>
    </div>

    <!-- Main Camera & Verification Area -->
    <div class="grid grid-cols-1 lg:grid-cols-12 gap-5 items-start">
      <!-- Camera Viewport (B2 & B3) -->
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

          <!-- Face Alignment Guide -->
          <div
            v-if="isCameraConnected"
            class="absolute inset-0 pointer-events-none flex flex-col items-center justify-center"
          >
            <div
              class="w-44 h-56 rounded-full border-2 border-dashed border-emerald-400/90 shadow-[0_0_30px_rgba(16,185,129,0.3)]"
            />
            <span
              class="mt-2.5 px-3 py-1 rounded-full bg-slate-900/80 text-[11px] font-medium text-emerald-300"
            >
              {{ checkInLoading ? t('l_face_analyzing') : t('l_camera_ready_hint') }}
            </span>
          </div>

          <!-- Not Connected Placeholder -->
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

        <!-- Camera Action Buttons -->
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
              @click="disconnectCamera"
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

          <button
            type="button"
            :disabled="!isCameraConnected || checkInLoading"
            class="inline-flex items-center gap-2 px-5 py-2.5 rounded-xl bg-gradient-to-r from-emerald-600 to-teal-600 hover:from-emerald-700 hover:to-teal-700 text-white text-xs font-bold shadow-md shadow-emerald-600/20 transition-all disabled:opacity-40"
            @click="handleCaptureAndCheckIn"
          >
            <ScanFace class="w-4 h-4" />
            {{ checkInLoading ? t('l_face_analyzing') : t('b_capture_face') }}
          </button>
        </div>
      </div>

      <!-- Result / Error Code Panel (B4) -->
      <div class="lg:col-span-5 space-y-4">
        <!-- Error Code Card (Camera Error or Face Mismatch) -->
        <div
          v-if="cameraErrorCode || verificationErrorCode"
          class="p-4 rounded-2xl bg-rose-50 border border-rose-200 space-y-2.5"
        >
          <div class="flex items-center gap-2 text-rose-800 font-bold text-xs">
            <AlertTriangle class="w-4 h-4 text-rose-600 shrink-0" />
            <span>{{ t('l_error_code_label') }}</span>
          </div>
          <div class="px-3 py-1.5 rounded-xl bg-rose-900 text-rose-100 font-mono text-xs font-bold tracking-wide">
            {{ cameraErrorCode || verificationErrorCode }}
          </div>
          <p class="text-xs text-rose-700 leading-relaxed">
            {{ getErrorMessageByCode(cameraErrorCode || verificationErrorCode) }}
          </p>
          <p
            v-if="checkInResult && checkInResult.similarityScore !== undefined"
            class="text-xs font-semibold text-rose-800"
          >
            {{ t('l_similarity_score') }}: {{ checkInResult.similarityScore }}%
          </p>
        </div>

        <!-- Success Verification Card -->
        <div
          v-if="checkInResult && checkInResult.success"
          class="p-4 rounded-2xl bg-emerald-50 border border-emerald-200 space-y-3"
        >
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-2 text-emerald-800 font-bold text-sm">
              <UserCheck class="w-5 h-5 text-emerald-600" />
              <span>{{ t('l_attendance_success') }}</span>
            </div>
            <span class="px-2.5 py-0.5 rounded-full bg-emerald-200/80 text-emerald-900 text-xs font-bold">
              {{ t('l_similarity_score') }}: {{ checkInResult.similarityScore }}%
            </span>
          </div>

          <div class="space-y-1.5 text-xs text-emerald-900">
            <p>
              <strong>{{ checkInResult.employeeName }}</strong>
            </p>
            <p>
              {{ t('l_action_type') }}:
              <strong>
                {{
                  checkInResult.actionType === 'CheckIn'
                    ? t('l_check_in_action')
                    : t('l_check_out_action')
                }}
              </strong>
            </p>
            <p>
              {{ t('l_time') }}: <strong>{{ checkInResult.time }} ({{ checkInResult.date }})</strong>
            </p>
            <p v-if="checkInResult.hoursWorked">
              {{ t('l_total_hours') }}: <strong>{{ checkInResult.hoursWorked }}h</strong>
            </p>
          </div>
        </div>

        <!-- Snapshot Comparison Preview -->
        <div
          v-if="lastSnapshotPreview"
          class="p-3.5 rounded-2xl bg-slate-50 border border-slate-200/80 flex items-center gap-3.5"
        >
          <img
            :src="lastSnapshotPreview"
            alt="Snapshot"
            class="w-16 h-16 rounded-xl object-cover border border-slate-300 -scale-x-100 shrink-0"
          />
          <div class="text-xs space-y-1">
            <p class="font-bold text-slate-800">{{ t('l_face_step_b3') }}</p>
            <p class="text-slate-500">{{ t('l_face_detected_valid') }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
