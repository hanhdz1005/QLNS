import { ref, onUnmounted, type Ref } from 'vue';
import {
  extractFaceFromVideo,
  calculateDescriptorDistance,
  type FaceExtractionResult,
} from '@/utils/faceAnalyzer';

export type ScanState = 'idle' | 'scanning' | 'locking' | 'verifying' | 'cooldown' | 'error';

export function useRealtimeFaceScan(
  videoRef: Ref<HTMLVideoElement | null>,
  isCameraConnected: Ref<boolean>,
  onFaceLocked: (extraction: FaceExtractionResult) => Promise<boolean>,
  options?: {
    requiredStableFrames?: number;
    intervalMs?: number;
    cooldownTotalSeconds?: number;
  }
) {
  const requiredFrames = options?.requiredStableFrames ?? 4;
  const intervalMs = options?.intervalMs ?? 280;
  const cooldownTotal = options?.cooldownTotalSeconds ?? 8;

  const autoScanEnabled = ref(true);
  const scanState = ref<ScanState>('idle');
  const scanProgress = ref(0);
  const cooldownSeconds = ref(0);

  let scanTimer: ReturnType<typeof setInterval> | null = null;
  let cooldownTimer: ReturnType<typeof setInterval> | null = null;
  let stableCount = 0;
  let prevDescriptor: number[] = [];
  let isProcessing = false;

  const clearTimers = () => {
    if (scanTimer) {
      clearInterval(scanTimer);
      scanTimer = null;
    }
    if (cooldownTimer) {
      clearInterval(cooldownTimer);
      cooldownTimer = null;
    }
  };

  const startCooldown = (seconds = cooldownTotal) => {
    if (cooldownTimer) clearInterval(cooldownTimer);
    scanState.value = 'cooldown';
    cooldownSeconds.value = seconds;
    scanProgress.value = 100;

    cooldownTimer = setInterval(() => {
      cooldownSeconds.value -= 1;
      if (cooldownSeconds.value <= 0) {
        if (cooldownTimer) clearInterval(cooldownTimer);
        cooldownTimer = null;
        resetProgress();
      }
    }, 1000);
  };

  const resetProgress = () => {
    if (cooldownTimer) {
      clearInterval(cooldownTimer);
      cooldownTimer = null;
    }
    cooldownSeconds.value = 0;
    stableCount = 0;
    prevDescriptor = [];
    scanProgress.value = 0;
    scanState.value = isCameraConnected.value && autoScanEnabled.value ? 'scanning' : 'idle';
  };

  const tickScan = async () => {
    if (
      !autoScanEnabled.value ||
      !isCameraConnected.value ||
      !videoRef.value ||
      isProcessing ||
      scanState.value === 'cooldown'
    ) {
      return;
    }

    if (videoRef.value.readyState < 2) return;

    const extracted = extractFaceFromVideo(videoRef.value);
    if (!extracted.valid || extracted.descriptor.length === 0) {
      stableCount = Math.max(0, stableCount - 1);
      scanProgress.value = Math.round((stableCount / requiredFrames) * 100);
      scanState.value = 'scanning';
      return;
    }

    // Kiểm tra độ ổn định giữa 2 frame liên tiếp để đảm bảo khuôn mặt giữ yên rõ nét
    const dist =
      prevDescriptor.length > 0
        ? calculateDescriptorDistance(prevDescriptor, extracted.descriptor)
        : 0;

    prevDescriptor = extracted.descriptor;

    if (dist <= 0.22) {
      stableCount += 1;
      scanState.value = 'locking';
    } else {
      stableCount = 1;
      scanState.value = 'scanning';
    }

    scanProgress.value = Math.min(100, Math.round((stableCount / requiredFrames) * 100));

    if (stableCount >= requiredFrames) {
      isProcessing = true;
      scanState.value = 'verifying';
      scanProgress.value = 100;
      stableCount = 0;

      try {
        const succeeded = await onFaceLocked(extracted);
        if (succeeded) {
          startCooldown(cooldownTotal);
        } else {
          startCooldown(2);
        }
      } finally {
        isProcessing = false;
      }
    }
  };

  const startAutoScanLoop = () => {
    if (scanTimer) clearInterval(scanTimer);
    resetProgress();
    scanTimer = setInterval(() => {
      void tickScan();
    }, intervalMs);
  };

  const stopAutoScanLoop = () => {
    clearTimers();
    scanState.value = 'idle';
    scanProgress.value = 0;
  };

  onUnmounted(() => {
    clearTimers();
  });

  return {
    autoScanEnabled,
    scanState,
    scanProgress,
    cooldownSeconds,
    startAutoScanLoop,
    stopAutoScanLoop,
    resetProgress,
  };
}
