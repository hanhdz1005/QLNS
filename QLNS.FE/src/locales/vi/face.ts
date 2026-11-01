export const face = {
  // Buttons
  b_connect_camera: 'Kết nối Camera',
  b_reconnect_camera: 'Thử kết nối lại Camera',
  b_disconnect_camera: 'Ngắt kết nối Camera',
  b_capture_face: 'Chụp Khuôn Mặt & Chấm Công',
  b_capture_first_face: 'Chụp Ảnh Khuôn Mặt',
  b_retake_face: 'Chụp lại ảnh khác',
  b_confirm_register_face: 'Lưu Khuôn Mặt Vào Hệ Thống',
  b_update_face: 'Cập nhật lại khuôn mặt',
  b_open_face_scanner: 'Chấm Công Khuôn Mặt',
  b_simulate_camera_error: 'Mô phỏng Lỗi Camera (Test Error Code)',

  // Labels
  l_face_onboarding_title: 'Thiết Lập Nhận Diện Khuôn Mặt Lần Đầu (B1)',
  l_face_onboarding_subtitle:
    'Tài khoản của bạn đăng nhập lần đầu tiên. Vui lòng chụp khuôn mặt để hệ thống lưu trữ đặc điểm nhận diện vào cơ sở dữ liệu.',
  l_face_scanner_title: 'Chấm Công Nhận Diện Khuôn Mặt (Face ID)',
  l_face_scanner_subtitle:
    'Kết nối Camera, chụp khuôn mặt trực tiếp và gửi về Backend đối sánh với đặc điểm nhận diện trong Database',
  l_face_step_b1: 'B1: Chụp mặt lần đầu',
  l_face_step_b2: 'B2: Kết nối Camera',
  l_face_step_b3: 'B3: Chụp & Gửi BE đối sánh DB',
  l_face_step_b4: 'B4: Kết quả / Error Code',
  l_camera_ready_hint: 'Đặt khuôn mặt nằm gọn trong khung elip định vị và nhìn thẳng vào camera',
  l_camera_not_connected: 'Chưa kết nối thiết bị Camera',
  l_camera_not_connected_desc:
    'Nhấn nút Kết nối Camera bên dưới để bật máy ảnh trước khi thực hiện chụp khuôn mặt chấm công.',
  l_face_analyzing: 'Đang gửi ảnh & đặc điểm khuôn mặt về Backend kiểm tra với Database...',
  l_face_detected_valid: 'Đã trích xuất thành công 128 đặc điểm nhận diện khuôn mặt',
  l_similarity_score: 'Độ khớp đặc điểm DB',
  l_error_code_label: 'Mã lỗi trả về (Error Code)',
  l_face_guide_1: 'Nhìn thẳng vào camera, giữ khuôn mặt đủ sáng và rõ nét.',
  l_face_guide_2: 'Đảm bảo khuôn mặt nằm ở trung tâm khung định vị.',
  l_face_guide_3: 'Hệ thống sẽ trích xuất 128 điểm đặc trưng khuôn mặt lưu vào Database để đối chiếu khi chấm công.',

  // Status
  s_camera_connected: 'Đã kết nối Camera',
  s_camera_disconnected: 'Chưa kết nối Camera',
  s_camera_error: 'Lỗi kết nối Camera',
  s_face_registered: 'Đã đăng ký khuôn mặt',
  s_face_not_registered: 'Chưa đăng ký khuôn mặt',
  s_face_matched: 'Khớp đặc điểm nhận diện',
  s_face_mismatched: 'Không khớp đặc điểm',

  // Toasts
  t_camera_connected: 'Kết nối thiết bị Camera thành công!',
  t_face_register_success: 'Đăng ký nhận diện khuôn mặt lần đầu thành công!',
  t_face_check_in_success: 'Xác thực khuôn mặt khớp với DB — Điểm danh VÀO thành công!',
  t_face_check_out_success: 'Xác thực khuôn mặt khớp với DB — Điểm danh RA thành công!',

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
    'Không phát hiện được khuôn mặt hợp lệ trong khung hình chụp (Error Code: FACE_NOT_DETECTED).',
  e_face_not_registered:
    'Tài khoản chưa có dữ liệu khuôn mặt trong Database (Error Code: FACE_NOT_REGISTERED).',
  e_face_mismatch:
    'Khuôn mặt chụp không khớp với đặc điểm nhận diện trong Database (Error Code: FACE_MISMATCH).',
  e_face_register_failed: 'Không thể đăng ký dữ liệu khuôn mặt vào hệ thống.',
};
