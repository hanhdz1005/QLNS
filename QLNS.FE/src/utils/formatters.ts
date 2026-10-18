import {
  Gender,
  EmployeeStatus,
  AttendanceStatus,
  ContractType,
  ContractStatus,
  LeaveType,
  LeaveStatus,
  BonusPenaltyType,
} from '@/types';
import { useI18n } from '@/locales';

const getT = () => {
  const { t } = useI18n();
  return t;
};

// Format currency in Vietnamese Dong (VNĐ)
export const formatCurrency = (val?: number | null): string => {
  if (val === undefined || val === null) return '0 ₫';
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND',
  }).format(val);
};

// Format standard Date (DD/MM/YYYY)
export const formatDate = (val?: string | null): string => {
  if (!val) return '—';
  try {
    const datePart = val.split('T')[0];
    if (datePart) {
      const parts = datePart.split('-');
      if (parts.length === 3 && parts[0] && parts[1] && parts[2]) {
        return `${parts[2]}/${parts[1]}/${parts[0]}`;
      }
    }
    const d = new Date(val);
    return isNaN(d.getTime()) ? val : d.toLocaleDateString('vi-VN');
  } catch {
    return val;
  }
};

// Format DateTime (HH:mm - DD/MM/YYYY)
export const formatDateTime = (val?: string | null): string => {
  if (!val) return '—';
  try {
    const d = new Date(val);
    return isNaN(d.getTime())
      ? val
      : `${d.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })} ${d.toLocaleDateString('vi-VN')}`;
  } catch {
    return val;
  }
};

// Enum label helpers
export const getGenderLabel = (g?: Gender): string => {
  const t = getT();
  switch (g) {
    case Gender.Male:
      return t('s_male');
    case Gender.Female:
      return t('s_female');
    case Gender.Other:
      return t('s_other');
    default:
      return '—';
  }
};

export const getEmployeeStatusInfo = (s?: EmployeeStatus): { text: string; color: string } => {
  const t = getT();
  switch (s) {
    case EmployeeStatus.Active:
      return { text: t('s_active'), color: 'bg-emerald-50 text-emerald-700 border-emerald-200' };
    case EmployeeStatus.Inactive:
      return { text: t('s_inactive'), color: 'bg-amber-50 text-amber-700 border-amber-200' };
    case EmployeeStatus.Resigned:
      return { text: t('s_resigned'), color: 'bg-rose-50 text-rose-700 border-rose-200' };
    case EmployeeStatus.Probation:
      return { text: t('s_probation'), color: 'bg-blue-50 text-blue-700 border-blue-200' };
    default:
      return { text: t('s_unknown'), color: 'bg-slate-50 text-slate-700 border-slate-200' };
  }
};

export const getAttendanceStatusInfo = (s?: AttendanceStatus): { text: string; color: string } => {
  const t = getT();
  switch (s) {
    case AttendanceStatus.Present:
      return { text: t('s_present'), color: 'bg-emerald-50 text-emerald-700 border-emerald-200' };
    case AttendanceStatus.Absent:
      return { text: t('s_absent'), color: 'bg-rose-50 text-rose-700 border-rose-200' };
    case AttendanceStatus.Late:
      return { text: t('s_late'), color: 'bg-amber-50 text-amber-700 border-amber-200' };
    case AttendanceStatus.EarlyLeave:
      return { text: t('s_early_leave'), color: 'bg-orange-50 text-orange-700 border-orange-200' };
    case AttendanceStatus.WorkFromHome:
      return { text: t('s_work_from_home'), color: 'bg-indigo-50 text-indigo-700 border-indigo-200' };
    case AttendanceStatus.Leave:
      return { text: t('s_leave'), color: 'bg-purple-50 text-purple-700 border-purple-200' };
    default:
      return { text: t('s_unknown'), color: 'bg-slate-50 text-slate-700 border-slate-200' };
  }
};

export const getContractTypeLabel = (ct?: ContractType): string => {
  const t = getT();
  switch (ct) {
    case ContractType.Probation:
      return t('s_probation_contract');
    case ContractType.DefiniteTerm:
      return t('s_definite_term_contract');
    case ContractType.IndefiniteTerm:
      return t('s_indefinite_term_contract');
    case ContractType.Internship:
      return t('s_internship_contract');
    case ContractType.PartTime:
      return t('s_part_time_contract');
    default:
      return '—';
  }
};

export const getContractStatusInfo = (s?: ContractStatus): { text: string; color: string } => {
  const t = getT();
  switch (s) {
    case ContractStatus.Active:
      return { text: t('s_contract_active'), color: 'bg-emerald-50 text-emerald-700 border-emerald-200' };
    case ContractStatus.Expired:
      return { text: t('s_contract_expired'), color: 'bg-amber-50 text-amber-700 border-amber-200' };
    case ContractStatus.Terminated:
      return { text: t('s_contract_terminated'), color: 'bg-rose-50 text-rose-700 border-rose-200' };
    default:
      return { text: '—', color: 'bg-slate-50 text-slate-700 border-slate-200' };
  }
};

export const getLeaveTypeLabel = (lt?: LeaveType): string => {
  const t = getT();
  switch (lt) {
    case LeaveType.AnnualLeave:
      return t('s_annual_leave');
    case LeaveType.SickLeave:
      return t('s_sick_leave');
    case LeaveType.MaternityLeave:
      return t('s_maternity_leave');
    case LeaveType.SpecialLeave:
      return t('s_special_leave');
    case LeaveType.UnpaidLeave:
      return t('s_unpaid_leave');
    default:
      return '—';
  }
};

export const getLeaveStatusInfo = (s?: LeaveStatus): { text: string; color: string } => {
  const t = getT();
  switch (s) {
    case LeaveStatus.Pending:
      return { text: t('s_pending'), color: 'bg-amber-50 text-amber-700 border-amber-200' };
    case LeaveStatus.Approved:
      return { text: t('s_approved'), color: 'bg-emerald-50 text-emerald-700 border-emerald-200' };
    case LeaveStatus.Rejected:
      return { text: t('s_rejected'), color: 'bg-rose-50 text-rose-700 border-rose-200' };
    case LeaveStatus.Cancelled:
      return { text: t('s_cancelled'), color: 'bg-slate-50 text-slate-700 border-slate-200' };
    default:
      return { text: '—', color: 'bg-slate-50 text-slate-700 border-slate-200' };
  }
};

export const getBonusPenaltyLabel = (bpt?: BonusPenaltyType): { text: string; color: string } => {
  const t = getT();
  if (bpt === BonusPenaltyType.Bonus) {
    return { text: t('s_bonus'), color: 'bg-emerald-50 text-emerald-700 border-emerald-200' };
  }
  return { text: t('s_penalty'), color: 'bg-rose-50 text-rose-700 border-rose-200' };
};
