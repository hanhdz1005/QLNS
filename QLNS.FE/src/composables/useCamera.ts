import { ref, onUnmounted } from 'vue';
import type { CameraErrorCode } from '@/types';
import { attendanceApi } from '@/api/attendance.api';

export function useCamera() {
  const videoRef = ref<HTMLVideoElement | null>(null);
  const isCameraConnected = ref(false);
  const cameraLoading = ref(false);
  const cameraErrorCode = ref<CameraErrorCode | null>(null);
  let mediaStream: MediaStream | null = null;

  const mapMediaErrorToCode = (err: unknown): CameraErrorCode => {
    const errorName = (err as { name?: string })?.name || '';
    if (
      errorName === 'NotAllowedError' ||
      errorName === 'PermissionDeniedError' ||
      errorName === 'SecurityError'
    ) {
      return 'CAMERA_PERMISSION_DENIED';
    }
    if (
      errorName === 'NotFoundError' ||
      errorName === 'DevicesNotFoundError' ||
      errorName === 'OverconstrainedError'
    ) {
      return 'CAMERA_NOT_FOUND';
    }
    if (errorName === 'NotReadableError' || errorName === 'TrackStartError') {
      return 'CAMERA_IN_USE';
    }
    return 'CAMERA_CONNECT_FAILED';
  };

  const reportErrorToBackend = async (code: CameraErrorCode, detail?: string) => {
    try {
      await attendanceApi.reportCameraError(code, detail);
    } catch (apiErr: unknown) {
      // Backend returns 400 Bad Request with standardized errorCode in response payload
      const returnedCode = (
        apiErr as { response?: { data?: { errorCode?: CameraErrorCode } } }
      )?.response?.data?.errorCode;
      if (returnedCode) {
        cameraErrorCode.value = returnedCode;
      }
    }
  };

  const connectCamera = async (): Promise<{
    connected: boolean;
    errorCode: CameraErrorCode | null;
  }> => {
    cameraLoading.value = true;
    cameraErrorCode.value = null;

    if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
      const code: CameraErrorCode = 'CAMERA_NOT_FOUND';
      cameraErrorCode.value = code;
      await reportErrorToBackend(code, 'Browser does not support mediaDevices.getUserMedia');
      cameraLoading.value = false;
      return { connected: false, errorCode: code };
    }

    try {
      disconnectCamera();
      mediaStream = await navigator.mediaDevices.getUserMedia({
        video: {
          facingMode: 'user',
          width: { ideal: 640 },
          height: { ideal: 480 },
        },
        audio: false,
      });

      if (videoRef.value) {
        videoRef.value.srcObject = mediaStream;
        await videoRef.value.play();
      }

      isCameraConnected.value = true;
      return { connected: true, errorCode: null };
    } catch (err: unknown) {
      const code = mapMediaErrorToCode(err);
      cameraErrorCode.value = code;
      isCameraConnected.value = false;
      const msg = (err as { message?: string })?.message;
      await reportErrorToBackend(code, msg);
      return { connected: false, errorCode: code };
    } finally {
      cameraLoading.value = false;
    }
  };

  const disconnectCamera = () => {
    if (mediaStream) {
      mediaStream.getTracks().forEach((track) => track.stop());
      mediaStream = null;
    }
    if (videoRef.value) {
      videoRef.value.srcObject = null;
    }
    isCameraConnected.value = false;
  };

  const simulateCameraError = async (code: CameraErrorCode = 'CAMERA_CONNECT_FAILED') => {
    disconnectCamera();
    cameraErrorCode.value = code;
    await reportErrorToBackend(code, `Simulated camera error: ${code}`);
  };

  onUnmounted(() => {
    disconnectCamera();
  });

  return {
    videoRef,
    isCameraConnected,
    cameraLoading,
    cameraErrorCode,
    connectCamera,
    disconnectCamera,
    simulateCameraError,
  };
}
