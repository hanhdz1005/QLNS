import apiClient from './client';

export const exportApi = {
  exportEmployeeCsv: async (): Promise<Blob> => {
    const res = await apiClient.get('/Export/employee-csv', { responseType: 'blob' });
    return res.data;
  },

  exportSalaryCsv: async (month: number, year: number): Promise<Blob> => {
    const res = await apiClient.get('/Export/salary-csv', {
      params: { month, year },
      responseType: 'blob',
    });
    return res.data;
  },

  exportLeaveCsv: async (fromDate: string, toDate: string): Promise<Blob> => {
    const res = await apiClient.get('/Export/leave-csv', {
      params: { fromDate, toDate },
      responseType: 'blob',
    });
    return res.data;
  },

  exportDashboardJson: async (): Promise<Blob> => {
    const res = await apiClient.get('/Export/dashboard-json', { responseType: 'blob' });
    return res.data;
  },

  downloadBlob: (blob: Blob, filename: string) => {
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', filename);
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.URL.revokeObjectURL(url);
  },
};
