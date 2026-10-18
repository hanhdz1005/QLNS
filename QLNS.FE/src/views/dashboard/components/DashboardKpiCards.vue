<script setup lang="ts">
import type { DashboardDto } from '@/types';
import { useI18n } from '@/locales';
import { Users, Building2, CalendarClock, CheckCircle2 } from 'lucide-vue-next';

defineProps<{
  data: DashboardDto;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 sm:gap-5 shrink-0">
    <!-- Total Employees -->
    <div class="bg-white p-5 rounded-2xl border border-slate-200/80 shadow-xs hover:shadow-md transition-all">
      <div class="flex items-center justify-between">
        <span class="text-xs font-semibold text-slate-500 uppercase tracking-wider">{{ t('l_total_employees') }}</span>
        <div class="w-10 h-10 rounded-xl bg-indigo-50 text-indigo-600 flex items-center justify-center">
          <Users class="w-5 h-5" />
        </div>
      </div>
      <div class="mt-2 flex items-baseline gap-2">
        <span class="text-2xl sm:text-3xl font-extrabold text-slate-800">{{ data.totalEmployees }}</span>
        <span class="text-xs text-emerald-600 font-semibold">({{ data.activeEmployees }} {{ t('l_working_staff_suffix') }})</span>
      </div>
      <div class="mt-2 text-xs text-slate-400 flex items-center gap-1.5">
        <span class="w-2 h-2 rounded-full bg-emerald-500"></span>
        {{ t('l_active_rate') }}:
        <span class="font-semibold text-slate-700">
          {{ data.totalEmployees > 0 ? Math.round((data.activeEmployees / data.totalEmployees) * 100) : 0 }}%
        </span>
      </div>
    </div>

    <!-- Total Departments -->
    <div class="bg-white p-5 rounded-2xl border border-slate-200/80 shadow-xs hover:shadow-md transition-all">
      <div class="flex items-center justify-between">
        <span class="text-xs font-semibold text-slate-500 uppercase tracking-wider">{{ t('l_total_departments') }}</span>
        <div class="w-10 h-10 rounded-xl bg-cyan-50 text-cyan-600 flex items-center justify-center">
          <Building2 class="w-5 h-5" />
        </div>
      </div>
      <div class="mt-2 flex items-baseline gap-2">
        <span class="text-2xl sm:text-3xl font-extrabold text-slate-800">{{ data.totalDepartments }}</span>
        <span class="text-xs text-slate-500 font-medium">{{ t('l_functional_depts') }}</span>
      </div>
      <router-link to="/departments" class="mt-2 text-xs text-indigo-600 hover:text-indigo-700 font-semibold inline-flex items-center gap-1">
        {{ t('l_manage_depts') }} &rarr;
      </router-link>
    </div>

    <!-- Pending Leave Requests -->
    <div class="bg-white p-5 rounded-2xl border border-slate-200/80 shadow-xs hover:shadow-md transition-all">
      <div class="flex items-center justify-between">
        <span class="text-xs font-semibold text-slate-500 uppercase tracking-wider">{{ t('l_pending_leave_requests') }}</span>
        <div class="w-10 h-10 rounded-xl bg-amber-50 text-amber-600 flex items-center justify-center">
          <CalendarClock class="w-5 h-5" />
        </div>
      </div>
      <div class="mt-2 flex items-baseline gap-2">
        <span class="text-2xl sm:text-3xl font-extrabold text-amber-600">{{ data.pendingLeaveRequests }}</span>
        <span class="text-xs text-slate-500 font-medium">{{ t('l_requests_count_unit') }}</span>
      </div>
      <router-link to="/leaves" class="mt-2 text-xs text-amber-600 hover:text-amber-700 font-semibold inline-flex items-center gap-1">
        {{ t('l_review_leave_requests') }} &rarr;
      </router-link>
    </div>

    <!-- Total Attendances This Month -->
    <div class="bg-white p-5 rounded-2xl border border-slate-200/80 shadow-xs hover:shadow-md transition-all">
      <div class="flex items-center justify-between">
        <span class="text-xs font-semibold text-slate-500 uppercase tracking-wider">{{ t('l_total_attendances_this_month') }}</span>
        <div class="w-10 h-10 rounded-xl bg-emerald-50 text-emerald-600 flex items-center justify-center">
          <CheckCircle2 class="w-5 h-5" />
        </div>
      </div>
      <div class="mt-2 flex items-baseline gap-2">
        <span class="text-2xl sm:text-3xl font-extrabold text-slate-800">{{ data.totalAttendancesThisMonth }}</span>
        <span class="text-xs text-slate-500 font-medium">{{ t('l_attendance_times_unit') }}</span>
      </div>
      <router-link to="/attendance" class="mt-2 text-xs text-emerald-600 hover:text-emerald-700 font-semibold inline-flex items-center gap-1">
        {{ t('l_view_attendance_logs') }} &rarr;
      </router-link>
    </div>
  </div>
</template>
