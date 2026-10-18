export interface ApiResponse<T> {
  isSuccess: boolean;
  data: T;
  message?: string;
  errors?: string[];
}

export interface PagedResult<T> {
  pageIndex: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  items: T[];
}

export interface ListItems<T> {
  items: T[];
}

export interface FileUploadResult {
  fileName: string;
  originalName: string;
  size: number;
  url: string;
}
