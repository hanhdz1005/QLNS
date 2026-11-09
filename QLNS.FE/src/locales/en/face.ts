export const face = {
  // Buttons
  b_connect_camera: 'Start Auto-Scan Camera',
  b_reconnect_camera: 'Retry Camera Connection',
  b_disconnect_camera: 'Stop Camera',
  b_capture_face: 'Scan Face Now',
  b_capture_first_face: 'Auto-Scan Face',
  b_retake_face: 'Rescan Face Sample',
  b_confirm_register_face: 'Save Face to System',
  b_update_face: 'Update Face Data',
  b_open_face_scanner: 'Real-Time Face Scanner',
  b_simulate_camera_error: 'Simulate Camera Error',
  b_scan_next_now: 'Scan Next Immediately',
  b_toggle_auto_scan: 'Real-time Auto-Scan Mode',

  // Labels
  l_face_onboarding_title: 'First-Time Real-Time Face Enrollment (Step 1)',
  l_face_onboarding_subtitle:
    'First-time login detected. Simply hold your face steady inside the oval frame—the system will automatically scan and enroll your biometric template for future check-ins.',
  l_face_scanner_title: 'Real-Time Automatic Face Attendance (Face ID)',
  l_face_scanner_subtitle:
    'Real-time automatic scanning — Counts First-In / Last-Out within standard office hours plus approved OT hours',
  l_face_step_b1: 'B1: Base Face Template',
  l_face_step_b2: 'B2: Real-Time Camera',
  l_face_step_b3: 'B3: Auto-Lock & Verify',
  l_face_step_b4: 'B4: First/Last & OT Result',
  l_camera_ready_hint: 'Hold your face steady inside the oval — Real-time auto-scanning active',
  l_camera_not_connected: 'Camera Paused',
  l_camera_not_connected_desc:
    'Click Start Camera below so the system can automatically recognize your face without manual capture.',
  l_face_analyzing: 'Face locked at 100% — Automatically verifying against Database...',
  l_face_locking_progress: 'Auto-scanning & locking stable face target',
  l_face_cooldown_active: 'Attendance recorded! Next auto-scan resumes in',
  l_face_detected_valid: 'Automatically extracted 128 facial recognition features',
  l_similarity_score: 'DB Match Score',
  l_error_code_label: 'Returned Error Code',
  l_face_guide_1: 'Look straight at the camera and hold steady for ~1 second.',
  l_face_guide_2: 'The system scans continuously in real time with no capture button needed.',
  l_face_guide_3: 'Only First-In and Last-Out scans are counted per day using office hours + approved OT.',
  l_first_check_in_label: 'First Scan (Check-In)',
  l_last_check_out_label: 'Last Scan (Check-Out)',
  l_regular_hours_label: 'Regular Office Hours',
  l_approved_ot_hours_label: 'Approved OT Hours',
  l_scan_count_label: 'Daily Scan Count',

  // Status
  s_camera_connected: 'Real-time Scanning',
  s_camera_disconnected: 'Camera Paused',
  s_camera_error: 'Camera Connection Error',
  s_face_registered: 'Template Enrolled',
  s_face_not_registered: 'Not Enrolled',
  s_face_matched: 'Face Matched',
  s_face_mismatched: 'Features Mismatch',
  s_auto_scan_on: 'Auto-Scan: ON',
  s_auto_scan_off: 'Auto-Scan: OFF',

  // Toasts
  t_camera_connected: 'Camera started — Real-time automatic face scanning is active!',
  t_face_register_success: 'First-time facial template auto-scanned and saved successfully!',
  t_face_check_in_success: 'Auto-recognized — First Check-IN of the day recorded!',
  t_face_check_out_success: 'Auto-recognized — Last Check-OUT of the day updated!',

  // Errors
  e_camera_permission_denied:
    'User denied camera access permission (Error Code: CAMERA_PERMISSION_DENIED).',
  e_camera_not_found:
    'No Camera/Webcam device found on your device (Error Code: CAMERA_NOT_FOUND).',
  e_camera_in_use:
    'Camera is currently in use by another application (Error Code: CAMERA_IN_USE).',
  e_camera_connect_failed:
    'Failed to connect to user camera device (Error Code: CAMERA_CONNECT_FAILED).',
  e_face_not_detected:
    'No stable face detected in the scanner frame (Error Code: FACE_NOT_DETECTED).',
  e_face_not_registered:
    'Account has no registered face data in Database (Error Code: FACE_NOT_REGISTERED).',
  e_face_mismatch:
    'Scanned face does not match facial features stored in Database (Error Code: FACE_MISMATCH).',
  e_face_register_failed: 'Failed to register face data to the system.',
};
