export const face = {
  // Buttons
  b_connect_camera: 'Connect Camera',
  b_reconnect_camera: 'Retry Camera Connection',
  b_disconnect_camera: 'Disconnect Camera',
  b_capture_face: 'Capture Face & Check-in',
  b_capture_first_face: 'Capture Face Photo',
  b_retake_face: 'Retake Photo',
  b_confirm_register_face: 'Save Face to System',
  b_update_face: 'Update Face Data',
  b_open_face_scanner: 'Face Recognition Check-in',
  b_simulate_camera_error: 'Simulate Camera Error (Test Error Code)',

  // Labels
  l_face_onboarding_title: 'First-Time Face Recognition Setup (Step 1)',
  l_face_onboarding_subtitle:
    'This is your first login. Please capture your face so the system can store your facial features in the database.',
  l_face_scanner_title: 'Face Recognition Attendance (Face ID)',
  l_face_scanner_subtitle:
    'Connect camera, capture your face and send to Backend to verify against facial features stored in the Database',
  l_face_step_b1: 'B1: First-Time Enrollment',
  l_face_step_b2: 'B2: Connect Camera',
  l_face_step_b3: 'B3: Capture & Verify with BE DB',
  l_face_step_b4: 'B4: Result / Error Code',
  l_camera_ready_hint: 'Position your face inside the oval guide and look straight at the camera',
  l_camera_not_connected: 'Camera Not Connected',
  l_camera_not_connected_desc:
    'Click the Connect Camera button below to activate your webcam before capturing your face for attendance.',
  l_face_analyzing: 'Sending captured image & facial features to Backend for DB verification...',
  l_face_detected_valid: 'Successfully extracted 128 facial recognition features',
  l_similarity_score: 'DB Feature Match Score',
  l_error_code_label: 'Returned Error Code',
  l_face_guide_1: 'Look straight at the camera in a well-lit environment.',
  l_face_guide_2: 'Keep your face centered inside the alignment frame.',
  l_face_guide_3: 'The system extracts 128 facial landmarks and stores them in the Database for attendance verification.',

  // Status
  s_camera_connected: 'Camera Connected',
  s_camera_disconnected: 'Camera Disconnected',
  s_camera_error: 'Camera Connection Error',
  s_face_registered: 'Face Registered',
  s_face_not_registered: 'Face Not Registered',
  s_face_matched: 'Facial Features Matched',
  s_face_mismatched: 'Facial Features Mismatch',

  // Toasts
  t_camera_connected: 'Camera connected successfully!',
  t_face_register_success: 'First-time facial recognition registered successfully!',
  t_face_check_in_success: 'Face verified with DB — Check-IN successful!',
  t_face_check_out_success: 'Face verified with DB — Check-OUT successful!',

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
    'No valid face detected in the captured frame (Error Code: FACE_NOT_DETECTED).',
  e_face_not_registered:
    'Account has no registered face data in Database (Error Code: FACE_NOT_REGISTERED).',
  e_face_mismatch:
    'Captured face does not match facial features stored in Database (Error Code: FACE_MISMATCH).',
  e_face_register_failed: 'Failed to register face data to the system.',
};
