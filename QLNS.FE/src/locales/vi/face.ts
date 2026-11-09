export const face = {
  // Buttons
  b_connect_camera: 'Bật Camera Quét Tự Động',
  b_reconnect_camera: 'Thử kết nối lại Camera',
  b_disconnect_camera: 'Tắt Camera',
  b_capture_face: 'Quét Khuôn Mặt Ngay',
  b_capture_first_face: 'Quét Khuôn Mặt Tự Động',
  b_retake_face: 'Quét lại mẫu khuôn mặt',
  b_confirm_register_face: 'Lưu Khuôn Mặt Vào Hệ Thống',
  b_update_face: 'Cập nhật lại khuôn mặt',
  b_open_face_scanner: 'Quét Khuôn Mặt Real-Time',
  b_simulate_camera_error: 'Mô phỏng Lỗi Camera',
  b_scan_next_now: 'Quét lượt tiếp theo ngay',
  b_toggle_auto_scan: 'Chế độ tự động quét Real-time',

  // Labels
  l_face_onboarding_title: 'Quét Khuôn Mặt Lần Đầu Tự Động (B1)',
  l_face_onboarding_subtitle:
    'Tài khoản đăng nhập lần đầu. Chỉ cần giữ khuôn mặt trong khung ngắm, hệ thống sẽ tự động quét real-time và lưu mẫu nhận diện gốc cho các lần chấm công sau.',
  l_face_scanner_title: 'Chấm Công Khuôn Mặt Tự Động Real-Time (Face ID)',
  l_face_scanner_subtitle:
    'Tự động quét khuôn mặt thời gian thực — Chỉ tính lần quét Đầu/Cuối trong ngày theo giờ hành chính & cộng giờ OT đã duyệt',
  l_face_step_b1: 'B1: Mẫu mặt lần đầu',
  l_face_step_b2: 'B2: Camera Real-time',
  l_face_step_b3: 'B3: Tự động khóa & Đối sánh',
  l_face_step_b4: 'B4: Kết quả Đầu/Cuối & OT',
  l_camera_ready_hint: 'Giữ yên khuôn mặt trong khung elip — Hệ thống đang tự động quét Real-time',
  l_camera_not_connected: 'Camera đang tạm ngắt',
  l_camera_not_connected_desc:
    'Nhấn nút Bật Camera bên dưới để hệ thống tự động nhận diện khuôn mặt mà không cần bấm chụp.',
  l_face_analyzing: 'Đã khóa khuôn mặt 100% — Đang tự động đối sánh với Database...',
  l_face_locking_progress: 'Đang tự động quét & khóa khuôn mặt ổn định',
  l_face_cooldown_active: 'Đã ghi nhận chấm công! Tự động mở quét tiếp sau',
  l_face_detected_valid: 'Đã tự động trích xuất thành công 128 đặc điểm nhận diện khuôn mặt',
  l_similarity_score: 'Độ khớp DB',
  l_error_code_label: 'Mã lỗi trả về (Error Code)',
  l_face_guide_1: 'Nhìn thẳng vào camera, giữ khuôn mặt ổn định khoảng 1 giây.',
  l_face_guide_2: 'Hệ thống tự động quét liên tục mà không cần nhấn nút chụp.',
  l_face_guide_3: 'Chỉ tính lần quét ĐẦU (Vào) và lần quét CUỐI (Ra) trong ngày theo giờ hành chính + OT đã duyệt.',
  l_first_check_in_label: 'Lần quét đầu (Giờ vào)',
  l_last_check_out_label: 'Lần quét cuối (Giờ ra)',
  l_regular_hours_label: 'Giờ công hành chính',
  l_approved_ot_hours_label: 'Giờ OT đã duyệt',
  l_scan_count_label: 'Số lần quét trong ngày',

  // Status
  s_camera_connected: 'Đang quét Real-time',
  s_camera_disconnected: 'Tạm dừng Camera',
  s_camera_error: 'Lỗi kết nối Camera',
  s_face_registered: 'Đã có mẫu gốc',
  s_face_not_registered: 'Chưa có mẫu gốc',
  s_face_matched: 'Khớp khuôn mặt',
  s_face_mismatched: 'Không khớp đặc điểm',
  s_auto_scan_on: 'Tự động quét: BẬT',
  s_auto_scan_off: 'Tự động quét: TẮT',

  // Toasts
  t_camera_connected: 'Đã bật Camera — Hệ thống bắt đầu tự động quét khuôn mặt Real-time!',
  t_face_register_success: 'Tự động quét và lưu mẫu khuôn mặt lần đầu thành công!',
  t_face_check_in_success: 'Tự động nhận diện thành công — Đã ghi nhận lần quét ĐẦU TIÊN trong ngày!',
  t_face_check_out_success: 'Tự động nhận diện thành công — Đã cập nhật lần quét CUỐI CÙNG trong ngày!',

  // Errors
  e_camera_permission_denied:
    'Người dùng từ chối cấp quyền truy cập Camera (Error Code: CAMERA_PERMISSION_DENIED).',
  e_camera_not_found:
    'Không tìm thấy thiết bị Camera/Webcam trên thiết bị của bạn (Error Code: CAMERA_NOT_FOUND).',
  e_camera_in_use:
    'Camera đang bị ứng dụng khác sử dụng, không thể truy cập (Error Code: CAMERA_IN_USE).',
  e_camera_connect_failed:
    'Không thể kết nối với thiết bị Camera của người dùng (Error Code: CAMERA_CONNECT_FAILED).',
  e_face_not_detected:
    'Chưa phát hiện khuôn mặt ổn định trong khung ngắm (Error Code: FACE_NOT_DETECTED).',
  e_face_not_registered:
    'Tài khoản chưa có dữ liệu khuôn mặt trong Database (Error Code: FACE_NOT_REGISTERED).',
  e_face_mismatch:
    'Khuôn mặt không khớp với đặc điểm nhận diện gốc trong Database (Error Code: FACE_MISMATCH).',
  e_face_register_failed: 'Không thể đăng ký dữ liệu khuôn mặt vào hệ thống.',
};
