import { reactive, computed } from 'vue';
import { defineStore } from 'pinia';
import { OvertimeCompensationType } from '@/types';
import type { LocaleSchema } from '@/locales/vi';

export type AppThemeId =
  | 'aurora_indigo'
  | 'warm_sand'
  | 'emerald_mist'
  | 'ocean_breeze'
  | 'classic_slate';

export interface ThemePreset {
  id: AppThemeId;
  nameKey: keyof LocaleSchema;
  descKey: keyof LocaleSchema;
  appBgClass: string;
  navbarBgClass: string;
  sidebarBgClass: string;
  sidebarHeaderClass: string;
  sidebarFooterClass: string;
  sidebarActiveClass: string;
  sidebarHoverClass: string;
  brandBadgeClass: string;
  brandSubtitleClass: string;
  previewSidebar: string;
  previewBg: string;
  previewAccent: string;
}

export interface CompanyPolicySettings {
  themeId: AppThemeId;
  // OT Policy
  defaultOtMode: OvertimeCompensationType;
  allowEmployeeChooseOtMode: boolean;
  allowApproverOverrideOtMode: boolean;
  maxMonthlyOtHours: number;
  hoursPerCompDay: number;
  weekdayMultiplier: number;
  weekendMultiplier: number;
  holidayMultiplier: number;
  // Working Hours & Attendance Policy
  morningStart: string;
  morningEnd: string;
  afternoonStart: string;
  afternoonEnd: string;
  gracePeriodTime: string;
  standardWorkingDays: number;
  excludeSaturday: boolean;
  // Payroll & Leave Policy
  defaultAnnualLeaveDays: number;
  includeOvertimePayInPayroll: boolean;
  includeInsuranceByDefault: boolean;
  includeTaxByDefault: boolean;
}

const STORAGE_KEY = 'qlns_company_policy_settings_v2';

export const THEME_PRESETS: ThemePreset[] = [
  {
    id: 'emerald_mist',
    nameKey: 'l_theme_emerald_mist_name',
    descKey: 'l_theme_emerald_mist_desc',
    appBgClass: 'bg-gradient-to-br from-emerald-100/70 via-teal-50/60 to-slate-50',
    navbarBgClass: 'bg-white/85 backdrop-blur-md border-b border-emerald-200/70',
    sidebarBgClass: 'bg-teal-950 text-teal-100',
    sidebarHeaderClass: 'border-teal-900/80 bg-teal-950/60',
    sidebarFooterClass: 'border-teal-900/80 bg-teal-950/40',
    sidebarActiveClass: 'bg-emerald-600 text-white shadow-md shadow-emerald-600/35',
    sidebarHoverClass: 'text-teal-200/80 hover:text-white hover:bg-teal-900/60',
    brandBadgeClass: 'bg-gradient-to-tr from-emerald-600 via-teal-500 to-cyan-400 shadow-emerald-500/25',
    brandSubtitleClass: 'text-emerald-300',
    previewSidebar: 'bg-teal-950',
    previewBg: 'bg-gradient-to-br from-emerald-100 via-teal-50 to-slate-50',
    previewAccent: 'bg-emerald-600',
  },
  {
    id: 'aurora_indigo',
    nameKey: 'l_theme_aurora_indigo_name',
    descKey: 'l_theme_aurora_indigo_desc',
    appBgClass: 'bg-gradient-to-br from-indigo-50/90 via-slate-50 to-sky-100/70',
    navbarBgClass: 'bg-white/85 backdrop-blur-md border-b border-indigo-100/90',
    sidebarBgClass: 'bg-indigo-950 text-indigo-100',
    sidebarHeaderClass: 'border-indigo-900/80 bg-indigo-950/60',
    sidebarFooterClass: 'border-indigo-900/80 bg-indigo-950/40',
    sidebarActiveClass: 'bg-indigo-600 text-white shadow-md shadow-indigo-600/35',
    sidebarHoverClass: 'text-indigo-200/80 hover:text-white hover:bg-indigo-900/60',
    brandBadgeClass: 'bg-gradient-to-tr from-indigo-600 via-indigo-500 to-cyan-400 shadow-indigo-500/25',
    brandSubtitleClass: 'text-indigo-300',
    previewSidebar: 'bg-indigo-950',
    previewBg: 'bg-gradient-to-br from-indigo-100 via-slate-50 to-sky-100',
    previewAccent: 'bg-indigo-600',
  },
  {
    id: 'warm_sand',
    nameKey: 'l_theme_warm_sand_name',
    descKey: 'l_theme_warm_sand_desc',
    appBgClass: 'bg-[#F5F2EB]',
    navbarBgClass: 'bg-[#FCFBF7]/90 backdrop-blur-md border-b border-stone-200/80',
    sidebarBgClass: 'bg-stone-900 text-stone-200',
    sidebarHeaderClass: 'border-stone-800 bg-stone-950/40',
    sidebarFooterClass: 'border-stone-800 bg-stone-950/30',
    sidebarActiveClass: 'bg-amber-600 text-white shadow-md shadow-amber-600/30',
    sidebarHoverClass: 'text-stone-300/80 hover:text-white hover:bg-stone-800/70',
    brandBadgeClass: 'bg-gradient-to-tr from-amber-600 via-orange-500 to-yellow-400 shadow-amber-500/25',
    brandSubtitleClass: 'text-amber-300',
    previewSidebar: 'bg-stone-900',
    previewBg: 'bg-[#EFECE4]',
    previewAccent: 'bg-amber-600',
  },
  {
    id: 'ocean_breeze',
    nameKey: 'l_theme_ocean_breeze_name',
    descKey: 'l_theme_ocean_breeze_desc',
    appBgClass: 'bg-gradient-to-br from-sky-100/75 via-cyan-50/40 to-slate-100/80',
    navbarBgClass: 'bg-white/85 backdrop-blur-md border-b border-sky-200/70',
    sidebarBgClass: 'bg-sky-950 text-sky-100',
    sidebarHeaderClass: 'border-sky-900/80 bg-sky-950/60',
    sidebarFooterClass: 'border-sky-900/80 bg-sky-950/40',
    sidebarActiveClass: 'bg-sky-600 text-white shadow-md shadow-sky-600/35',
    sidebarHoverClass: 'text-sky-200/80 hover:text-white hover:bg-sky-900/60',
    brandBadgeClass: 'bg-gradient-to-tr from-sky-600 via-cyan-500 to-teal-400 shadow-sky-500/25',
    brandSubtitleClass: 'text-sky-300',
    previewSidebar: 'bg-sky-950',
    previewBg: 'bg-gradient-to-br from-sky-100 via-cyan-50 to-slate-100',
    previewAccent: 'bg-sky-600',
  },
  {
    id: 'classic_slate',
    nameKey: 'l_theme_classic_slate_name',
    descKey: 'l_theme_classic_slate_desc',
    appBgClass: 'bg-slate-50',
    navbarBgClass: 'bg-white border-b border-slate-200/80',
    sidebarBgClass: 'bg-slate-900 text-slate-300',
    sidebarHeaderClass: 'border-slate-800 bg-slate-950/40',
    sidebarFooterClass: 'border-slate-800 bg-slate-950/20',
    sidebarActiveClass: 'bg-indigo-600 text-white shadow-md shadow-indigo-600/30',
    sidebarHoverClass: 'text-slate-400 hover:text-slate-100 hover:bg-slate-800/60',
    brandBadgeClass: 'bg-gradient-to-tr from-indigo-600 via-indigo-500 to-cyan-400 shadow-indigo-500/20',
    brandSubtitleClass: 'text-indigo-400',
    previewSidebar: 'bg-slate-900',
    previewBg: 'bg-slate-100',
    previewAccent: 'bg-indigo-600',
  },
];

export const getDefaultCompanySettings = (): CompanyPolicySettings => ({
  themeId: 'classic_slate',
  defaultOtMode: OvertimeCompensationType.CompensatoryLeave,
  allowEmployeeChooseOtMode: false,
  allowApproverOverrideOtMode: true,
  maxMonthlyOtHours: 40,
  hoursPerCompDay: 8,
  weekdayMultiplier: 1.5,
  weekendMultiplier: 2.0,
  holidayMultiplier: 3.0,
  morningStart: '08:00',
  morningEnd: '12:00',
  afternoonStart: '13:00',
  afternoonEnd: '17:30',
  gracePeriodTime: '08:30',
  standardWorkingDays: 22,
  excludeSaturday: true,
  defaultAnnualLeaveDays: 12,
  includeOvertimePayInPayroll: true,
  includeInsuranceByDefault: true,
  includeTaxByDefault: true,
});

export const useCompanySettingsStore = defineStore('companySettings', () => {
  const loadInitialSettings = (): CompanyPolicySettings => {
    const defaults = getDefaultCompanySettings();
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (!raw) return defaults;
      const parsed = JSON.parse(raw) as Partial<CompanyPolicySettings>;
      return { ...defaults, ...parsed, themeId: 'classic_slate' };
    } catch {
      return defaults;
    }
  };

  const settings = reactive<CompanyPolicySettings>(loadInitialSettings());

  const currentTheme = computed<ThemePreset>(() => {
    return (
      THEME_PRESETS.find((t) => t.id === 'classic_slate') ||
      THEME_PRESETS[0]!
    );
  });

  const persist = () => {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(settings));
  };

  const updateSettings = (partial: Partial<CompanyPolicySettings>) => {
    Object.assign(settings, partial);
    persist();
  };

  const setTheme = (themeId: AppThemeId) => {
    settings.themeId = themeId;
    persist();
  };

  const resetDefaults = () => {
    Object.assign(settings, getDefaultCompanySettings());
    persist();
  };

  return {
    settings,
    currentTheme,
    updateSettings,
    setTheme,
    resetDefaults,
  };
});
