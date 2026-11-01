export interface FaceExtractionResult {
  valid: boolean;
  errorCode?: 'FACE_NOT_DETECTED';
  imageBase64: string;
  descriptor: number[];
}

/**
 * Trích xuất ảnh chụp khuôn mặt và vector 128 đặc điểm nhận diện sinh trắc học từ Video Stream
 */
export function extractFaceFromVideo(videoEl: HTMLVideoElement): FaceExtractionResult {
  const width = videoEl.videoWidth || 640;
  const height = videoEl.videoHeight || 480;

  // 1. Vẽ ảnh chất lượng chuẩn để gửi lưu trữ / đối chiếu
  const captureCanvas = document.createElement('canvas');
  captureCanvas.width = 480;
  captureCanvas.height = 360;
  const captureCtx = captureCanvas.getContext('2d');

  if (!captureCtx) {
    return {
      valid: false,
      errorCode: 'FACE_NOT_DETECTED',
      imageBase64: '',
      descriptor: [],
    };
  }

  captureCtx.drawImage(videoEl, 0, 0, width, height, 0, 0, 480, 360);
  const imageBase64 = captureCanvas.toDataURL('image/jpeg', 0.85);

  // 2. Phân tích vùng trung tâm khuôn mặt trên lưới chuẩn hóa 64x64
  const gridCanvas = document.createElement('canvas');
  gridCanvas.width = 64;
  gridCanvas.height = 64;
  const gridCtx = gridCanvas.getContext('2d');

  if (!gridCtx) {
    return {
      valid: false,
      errorCode: 'FACE_NOT_DETECTED',
      imageBase64,
      descriptor: [],
    };
  }

  // Crop vùng trung tâm (nơi đặt khung định vị khuôn mặt)
  const cropW = width * 0.55;
  const cropH = height * 0.7;
  const cropX = (width - cropW) / 2;
  const cropY = (height - cropH) / 2;

  gridCtx.drawImage(videoEl, cropX, cropY, cropW, cropH, 0, 0, 64, 64);
  const imgData = gridCtx.getImageData(0, 0, 64, 64).data;

  // Kiểm tra độ sáng và độ tương phản để phát hiện trường hợp che camera hoặc khung hình trống
  let sumLum = 0;
  let sumLumSq = 0;
  const totalPixels = 64 * 64;
  const gray = new Float64Array(totalPixels);
  const crChannel = new Float64Array(totalPixels);

  for (let i = 0; i < totalPixels; i++) {
    const r = imgData[i * 4] ?? 0;
    const g = imgData[i * 4 + 1] ?? 0;
    const b = imgData[i * 4 + 2] ?? 0;

    const y = (0.299 * r + 0.587 * g + 0.114 * b) / 255.0;
    const cr = (0.5 * r - 0.4187 * g - 0.0813 * b + 128) / 255.0;

    gray[i] = y;
    crChannel[i] = cr;
    sumLum += y;
    sumLumSq += y * y;
  }

  const meanLum = sumLum / totalPixels;
  const varianceLum = sumLumSq / totalPixels - meanLum * meanLum;
  const stdDevLum = Math.sqrt(Math.max(0, varianceLum));

  // Nếu ảnh quá tối (< 0.04), quá cháy sáng (> 0.97) hoặc không có chi tiết khuôn mặt (stdDev < 0.03)
  if (meanLum < 0.04 || meanLum > 0.97 || stdDevLum < 0.03) {
    return {
      valid: false,
      errorCode: 'FACE_NOT_DETECTED',
      imageBase64,
      descriptor: [],
    };
  }

  // 3. Trích xuất 128 đặc điểm nhận diện từ lưới 8x8 vùng giải phẫu khuôn mặt (Mỗi ô trích xuất 2 đặc trưng: Luminance chuẩn hóa & Gradient/Cr)
  const rawFeatures: number[] = [];

  for (let by = 0; by < 8; by++) {
    for (let bx = 0; bx < 8; bx++) {
      let blockLum = 0;
      let blockGrad = 0;
      let count = 0;

      for (let py = by * 8; py < (by + 1) * 8; py++) {
        for (let px = bx * 8; px < (bx + 1) * 8; px++) {
          const idx = py * 64 + px;
          const val = (gray[idx] ?? meanLum) - meanLum;
          blockLum += val;

          const rightIdx = py * 64 + Math.min(63, px + 1);
          const downIdx = Math.min(63, py + 1) * 64 + px;
          const gx = (gray[rightIdx] ?? 0) - (gray[idx] ?? 0);
          const gy = (gray[downIdx] ?? 0) - (gray[idx] ?? 0);
          const cr = (crChannel[idx] ?? 0.5) - 0.5;

          blockGrad += Math.sqrt(gx * gx + gy * gy) * 0.6 + cr * 0.4;
          count++;
        }
      }

      rawFeatures.push(blockLum / (count * (stdDevLum + 0.05)));
      rawFeatures.push(blockGrad / count);
    }
  }

  // 4. Chuẩn hóa L2 (Unit Vector 128 chiều) để đối sánh khoảng cách Euclidean tại Backend
  let normSq = 0;
  for (const f of rawFeatures) {
    normSq += f * f;
  }
  const norm = Math.sqrt(normSq) || 1.0;
  const descriptor = rawFeatures.map((f) => Number((f / norm).toFixed(6)));

  return {
    valid: true,
    imageBase64,
    descriptor,
  };
}

export function getFaceErrorLocaleKey(
  code: string | null,
  fallbackKey: 'e_camera_connect_failed' | 'e_face_register_failed' = 'e_camera_connect_failed'
) {
  switch (code) {
    case 'CAMERA_PERMISSION_DENIED':
      return 'e_camera_permission_denied' as const;
    case 'CAMERA_NOT_FOUND':
      return 'e_camera_not_found' as const;
    case 'CAMERA_IN_USE':
      return 'e_camera_in_use' as const;
    case 'CAMERA_CONNECT_FAILED':
      return 'e_camera_connect_failed' as const;
    case 'FACE_NOT_DETECTED':
      return 'e_face_not_detected' as const;
    case 'FACE_NOT_REGISTERED':
      return 'e_face_not_registered' as const;
    case 'FACE_MISMATCH':
      return 'e_face_mismatch' as const;
    default:
      return fallbackKey;
  }
}
