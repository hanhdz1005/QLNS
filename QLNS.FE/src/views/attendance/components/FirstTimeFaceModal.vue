<script setup lang="ts">
import { ref, watch } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useToastStore } from '@/stores/toast';
import { useI18n } from '@/locales';
import { useCamera } from '@/composables/useCamera';
import { useRealtimeFaceScan } from '@/composables/useRealtimeFaceScan';
import { getFaceErrorLocaleKey, type FaceExtractionResult } from '@/utils/faceAnalyzer';
import { attendanceApi } from '@/api/attendance.api';
import {
  Camera,
  CameraOff,
  CheckCircle2,
  AlertTriangle,
  RefreshCw,
  ScanFace,
  LogOut,
  ShieldCheck,
} from 'lucide-vue-next';

const props = defineProps<{
  show: boolean;
  mandatory?: boolean;
}>();

const emit = defineEmits<{
  (e: 'close'): void;
  (e: 'registered'): void;
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
} = useCamera();

const capturedImage = ref('');
const capturedDescriptor = ref<number[]>([]);
const captureErrorCode = ref<string | null>(null);
const saving = ref(false);

const getErrorMessageByCode = (code: string | null) =>
  t(getFaceErrorLocaleKey(code, 'e_face_register_failed'));

const handleSaveFace = async () => {
  if (!capturedImage.value || capturedDescriptor.value.length === 0) return false;

  saving.value = true;
  captureErrorCode.value = null;
  try {
    const res = await attendanceApi.registerFace({
      capturedImageBase64: capturedImage.value,
      faceDescriptor: capturedDescriptor.value,
    });

    if (res.isSuccess) {
      authStore.markFaceRegistered(res.data?.faceImageUrl);
      toastStore.success(t('t_face_register_success'));
      stopAutoScanLoop();
      disconnectCamera();
      emit('registered');
      emit('close');
      return true;
    } else {
      captureErrorCode.value = res.errorCode || 'FACE_REGISTER_FAILED';
      toastStore.error(res.message || t('e_face_register_failed'));
      return false;
    }
  } catch (err: unknown) {
    const errData = (err as { response?: { data?: { errorCode?: string; message?: string } } })
      ?.response?.data;
    captureErrorCode.value = errData?.errorCode || 'FACE_REGISTER_FAILED';
    toastStore.error(errData?.message || t('e_face_register_failed'));
    return false;
  } finally {
    saving.value = false;
  }
};

const onFirstFaceLocked = async (extracted: FaceExtractionResult): Promise<boolean> => {
  capturedImage.value = extracted.imageBase64;
  capturedDescriptor.value = extracted.descriptor;
  return await handleSaveFace();
};

const {
  scanProgress,
  startAutoScanLoop,
  stopAutoScanLoop,
  resetProgress,
} = useRealtimeFaceScan(videoRef, isCameraConnected, onFirstFaceLocked, {
  requiredStableFrames: 5,
  intervalMs: 250,
  cooldownTotalSeconds: 4,
});

const handleConnectCamera = async () => {
  captureErrorCode.value = null;
  const { connected, errorCode } = await connectCamera();
  if (connected) {
    toastStore.info(t('t_camera_connected'));
    startAutoScanLoop();
  } else if (errorCode) {
    toastStore.error(getErrorMessageByCode(errorCode));
  }
};

const handleRetake = async () => {
  capturedImage.value = '';
  capturedDescriptor.value = [];
  captureErrorCode.value = null;
  if (!isCameraConnected.value) {
    await handleConnectCamera();
  } else {
    resetProgress();
  }
};

watch(
  () => props.show,
  async (val) => {
    if (val) {
      capturedImage.value = '';
      capturedDescriptor.value = [];
      captureErrorCode.value = null;
      await handleConnectCamera();
    } else {
      stopAutoScanLoop();
      disconnectCamera();
    }
  },
  { immediate: true }
);
</script>

<template>
  <Teleport to="body">
    <div
      v-if="show"
      class="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/40 p-4"
    >
      <div
        class="bg-white rounded-3xl shadow-2xl border border-slate-200 w-full max-w-2xl overflow-hidden"
      >
        <!-- Header -->
        <div
          class="bg-gradient-to-r from-indigo-600 via-indigo-700 to-slate-900 px-6 py-5 text-white flex items-center justify-between"
        >
          <div class="flex items-center gap-3">
            <div class="w-11 h-11 rounded-2xl bg-white/15 flex items-center justify-center">
              <ScanFace class="w-6 h-6 text-indigo-200" />
            </div>
            <div>
              <h3 class="text-lg font-bold">{{ t('l_face_onboarding_title') }}</h3>
              <p class="text-xs text-indigo-200 mt-0.5">{{ t('l_face_step_b1') }}</p>
            </div>
          </div>

          <button
            v-if="!mandatory"
            type="button"
            class="px-3 py-1.5 rounded-xl bg-white/10 hover:bg-white/20 text-xs font-medium transition-colors"
            @click="emit('close')"
          >
            {{ t('b_close') }}
          </button>
        </div>

        <!-- Body -->
        <div class="p-6 space-y-5">
          <p class="text-sm text-slate-600 leading-relaxed">
            {{ t('l_face_onboarding_subtitle') }}
          </p>

          <!-- Error Code Banner -->
          <div
            v-if="cameraErrorCode || captureErrorCode"
            class="p-4 rounded-2xl bg-rose-50 border border-rose-200 flex items-start gap-3"
          >
            <AlertTriangle class="w-5 h-5 text-rose-600 shrink-0 mt-0.5" />
            <div class="text-xs text-rose-800 space-y-1">
              <p class="font-bold">
                {{ t('l_error_code_label') }}:
                <span class="px-2 py-0.5 rounded bg-rose-200/70 font-mono text-rose-900">
                  {{ cameraErrorCode || captureErrorCode }}
                </span>
              </p>
              <p>{{ getErrorMessageByCode(cameraErrorCode || captureErrorCode) }}</p>
            </div>
          </div>

          <!-- Camera Viewport & Auto-Scan Progress -->
          <div class="grid grid-cols-1 md:grid-cols-12 gap-5 items-center">
            <div class="md:col-span-7">
              <div
                class="relative aspect-4/3 rounded-2xl bg-slate-900 overflow-hidden border-2 border-indigo-500/40 shadow-inner flex items-center justify-center"
              >
                <video
                  v-show="isCameraConnected && !capturedImage"
                  ref="videoRef"
                  autoplay
                  playsinline
                  muted
                  class="w-full h-full object-cover -scale-x-100"
                />

                <img
                  v-if="capturedImage"
                  :src="capturedImage"
                  alt="Captured Face"
                  class="w-full h-full object-cover -scale-x-100"
                />

                <!-- Oval Face Alignment & Auto-Lock Progress -->
                <div
                  v-if="isCameraConnected && !capturedImage"
                  class="absolute inset-0 pointer-events-none flex flex-col items-center justify-center"
                >
                  <div
                    class="w-40 h-52 rounded-full border-2 border-dashed border-emerald-400/90 shadow-[0_0_25px_rgba(16,185,129,0.35)]"
                  />
                  <div class="mt-2.5 w-48 bg-slate-900/80 px-3 py-1.5 rounded-xl border border-slate-700 space-y-1">
                    <div class="flex justify-between text-[11px] font-medium text-emerald-300">
                      <span>{{ t('l_face_locking_progress') }}</span>
                      <span class="font-mono">{{ scanProgress }}%</span>
                    </div>
                    <div class="w-full h-1.5 bg-slate-700 rounded-full overflow-hidden">
                      <div
                        class="h-full bg-emerald-400 transition-all duration-200"
                        :style="{ width: `${scanProgress}%` }"
                      />
                    </div>
                  </div>
                </div>

                <div
                  v-if="!isCameraConnected && !capturedImage"
                  class="flex flex-col items-center justify-center text-center p-6 text-slate-400"
                >
                  <CameraOff class="w-10 h-10 text-slate-500 mb-2" />
                  <p class="text-xs font-semibold text-slate-300">
                    {{ t('l_camera_not_connected') }}
                  </p>
                </div>
              </div>
            </div>

            <!-- Instructions & Status -->
            <div class="md:col-span-5 space-y-3">
              <div class="p-3.5 rounded-2xl bg-slate-50 border border-slate-200/80 space-y-2">
                <div class="flex items-center gap-2 text-xs font-bold text-slate-800">
                  <ShieldCheck class="w-4 h-4 text-indigo-600" />
                  <span>{{ t('l_face_step_b1') }}</span>
                </div>
                <ul class="text-xs text-slate-600 space-y-1.5 list-disc list-inside">
                  <li>{{ t('l_face_guide_1') }}</li>
                  <li>{{ t('l_face_guide_2') }}</li>
                  <li>{{ t('l_face_guide_3') }}</li>
                </ul>
              </div>

              <div
                v-if="capturedDescriptor.length > 0"
                class="p-3 rounded-2xl bg-emerald-50 border border-emerald-200 flex items-center gap-2.5 text-xs font-semibold text-emerald-800"
              >
                <CheckCircle2 class="w-4 h-4 text-emerald-600 shrink-0" />
                <span>{{ t('l_face_detected_valid') }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Footer Actions -->
        <div
          class="px-6 py-4 bg-slate-50 border-t border-slate-200/80 flex flex-wrap items-center justify-between gap-3"
        >
          <button
            v-if="mandatory"
            type="button"
            class="inline-flex items-center gap-2 px-4 py-2.5 rounded-xl border border-slate-300 text-slate-700 hover:bg-slate-100 text-xs font-semibold transition-colors"
            @click="authStore.logout()"
          >
            <LogOut class="w-4 h-4" />
            {{ t('b_logout') }}
          </button>
          <div v-else />

          <div class="flex items-center gap-2.5">
            <button
              v-if="!isCameraConnected && !capturedImage"
              type="button"
              :disabled="cameraLoading"
              class="inline-flex items-center gap-2 px-4 py-2.5 rounded-xl bg-indigo-600 hover:bg-indigo-700 text-white text-xs font-semibold shadow-sm transition-all disabled:opacity-50"
              @click="handleConnectCamera"
            >
              <Camera class="w-4 h-4" />
              {{ cameraErrorCode ? t('b_reconnect_camera') : t('b_connect_camera') }}
            </button>

            <template v-if="capturedImage">
              <button
                type="button"
                :disabled="saving"
                class="inline-flex items-center gap-1.5 px-4 py-2.5 rounded-xl border border-slate-300 bg-white hover:bg-slate-50 text-slate-700 text-xs font-semibold transition-colors"
                @click="handleRetake"
              >
                <RefreshCw class="w-3.5 h-3.5" />
                {{ t('b_retake_face') }}
              </button>

              <button
                type="button"
                :disabled="saving"
                class="inline-flex items-center gap-2 px-5 py-2.5 rounded-xl bg-emerald-600 hover:bg-emerald-700 text-white text-xs font-semibold shadow-md shadow-emerald-600/20 transition-all disabled:opacity-50"
                @click="handleSaveFace"
              >
                <CheckCircle2 class="w-4 h-4" />
                {{ saving ? t('b_saving') : t('b_confirm_register_face') }}
              </button>
            </template>
          </div>
        </div>
      </div>
    </div>
  </Teleport>
</template>
