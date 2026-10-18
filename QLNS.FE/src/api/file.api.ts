import apiClient from './client';
import type { ApiResponse, ListItems, FileUploadResult } from '@/types';

export const fileApi = {
  upload: async (file: File, folder?: string): Promise<ApiResponse<FileUploadResult>> => {
    const formData = new FormData();
    formData.append('file', file);
    const res = await apiClient.post<ApiResponse<FileUploadResult>>('/File/upload', formData, {
      params: folder ? { folder } : {},
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return res.data;
  },

  uploadMultiple: async (files: FileList | File[], folder?: string): Promise<ApiResponse<ListItems<FileUploadResult>>> => {
    const formData = new FormData();
    Array.from(files).forEach((f) => formData.append('files', f));
    const res = await apiClient.post<ApiResponse<ListItems<FileUploadResult>>>('/File/upload-multiple', formData, {
      params: folder ? { folder } : {},
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return res.data;
  },

  deleteFile: async (fileName: string): Promise<ApiResponse<boolean>> => {
    const res = await apiClient.delete<ApiResponse<boolean>>(`/File/${encodeURIComponent(fileName)}`);
    return res.data;
  },
};
