import type { AttendanceStatus } from './enums';

export interface AttendanceDto {
  id: string;
  employeeId: string;
  employeeName: string;
  date: string;
  checkIn?: string;
  checkOut?: string;
  hoursWorked?: number;
  status: AttendanceStatus;
  faceSnapshotUrl?: string | null;
  similarityScore?: number | null;
}

export interface AddAttendanceDto {
  employeeId: string;
  date: string;
  checkIn?: string;
  checkOut?: string;
  status: AttendanceStatus;
}

export type CameraErrorCode =
  | 'CAMERA_PERMISSION_DENIED'
  | 'CAMERA_NOT_FOUND'
  | 'CAMERA_IN_USE'
  | 'CAMERA_CONNECT_FAILED';

export interface FaceRegisterDto {
  capturedImageBase64: string;
  faceDescriptor: number[];
}

export interface FaceRegisterResultDto {
  hasRegisteredFace: boolean;
  faceImageUrl?: string | null;
}

export interface FaceCheckInDto {
  capturedImageBase64: string;
  capturedFaceDescriptor: number[];
}

export interface FaceCheckInResultDto {
  success: boolean;
  actionType: string;
  message: string;
  errorCode?: string | null;
  employeeName: string;
  date: string;
  time: string;
  hoursWorked?: number;
  similarityScore?: number;
  snapshotUrl?: string | null;
}

export interface QrTokenDto {
  token: string;
  expiresInSeconds: number;
  generatedAt: string;
}

export interface QrCheckInResultDto {
  success: boolean;
  actionType: string;
  message: string;
  employeeName: string;
  date: string;
  time: string;
  hoursWorked?: number;
}
