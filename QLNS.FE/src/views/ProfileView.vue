<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useToastStore } from '@/stores/toast';
import { useI18n } from '@/locales';
import { employeeApi } from '@/api/employee.api';
import { leaveApi } from '@/api/leave.api';
import { fileApi } from '@/api/file.api';
import type { EmployeeDto, LeaveBalanceDto } from '@/types';
import { formatDate, getGenderLabel } from '@/utils/formatters';
import StatusBadge from '@/components/StatusBadge.vue';
import {
  User,
  Mail,
  Building2,
  Briefcase,
  Calendar,
  CreditCard,
  HeartHandshake,
  ShieldCheck,
  Upload,
  CalendarCheck2,
  DollarSign,
} from 'lucide-vue-next';

const authStore = useAuthStore();
const toastStore = useToastStore();
const { t } = useI18n();

const loading = ref(true);
const profile = ref<EmployeeDto | null>(null);
const leaveBalance = ref<LeaveBalanceDto | null>(null);
const avatarUrl = ref<string | null>(null);
const uploadingAvatar = ref(false);

const loadProfile = async () => {
  loading.value = true;
  try {
    const [empRes, leaveRes] = await Promise.allSettled([
      employeeApi.getProfileMe(),
      leaveApi.getMyBalance(),
    ]);

    if (empRes.status === 'fulfilled' && empRes.value.isSuccess) {
      profile.value = empRes.value.data;
    }
    if (leaveRes.status === 'fulfilled' && leaveRes.value.isSuccess) {
      leaveBalance.value = leaveRes.value.data;
    }
  } catch (error) {
    console.error('Failed to load profile:', error);
  } finally {
    loading.value = false;
  }
};

const handleAvatarUpload = async (event: Event) => {
  const target = event.target as HTMLInputElement;
  if (!target.files || target.files.length === 0) return;

  const file = target.files[0];
  if (!file) return;

  uploadingAvatar.value = true;
  try {
    const res = await fileApi.upload(file, 'avatars');
    if (res.isSuccess && res.data?.url) {
      avatarUrl.value = res.data.url;
      toastStore.success(t('t_avatar_updated'));
    }
  } catch {
    toastStore.error(t('e_file_upload_failed'));
  } finally {
    uploadingAvatar.value = false;
  }
};

onMounted(() => {
  loadProfile();
});
</script>

<template>
  <div class="space-y-4 sm:space-y-5">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-xl sm:text-2xl font-bold text-slate-800 tracking-tight flex items-center gap-2">
          <User class="w-6 h-6 text-indigo-600" />
          {{ t('l_profile_page_title') }}
        </h1>
        <p class="text-xs sm:text-sm text-slate-500 mt-1">
          {{ t('l_profile_page_subtitle') }}
        </p>
      </div>
    </div>

    <div v-if="loading" class="flex items-center justify-center p-12 text-slate-400 text-xs sm:text-sm bg-white rounded-2xl border border-slate-200/80 shadow-xs">
      {{ t('l_loading_profile') }}
    </div>

    <div v-else-if="!profile" class="flex items-center justify-center p-12 text-slate-400 text-xs sm:text-sm bg-white rounded-2xl border border-slate-200/80 shadow-xs">
      {{ t('l_unlinked_profile') }}
    </div>

    <div v-else class="space-y-4 sm:space-y-5">
      <!-- Main Profile Banner Card -->
      <div class="bg-white rounded-2xl p-5 sm:p-6 border border-slate-200/80 shadow-xs flex flex-col sm:flex-row items-center sm:items-start gap-5">
        <!-- Avatar with Upload -->
        <div class="relative group shrink-0">
          <div class="w-16 h-16 sm:w-20 sm:h-20 rounded-2xl bg-indigo-600 text-white font-extrabold text-2xl flex items-center justify-center shadow-xs overflow-hidden border-2 border-white">
            <img v-if="avatarUrl" :src="avatarUrl" class="w-full h-full object-cover" />
            <span v-else>{{ profile.fullName.charAt(0) }}</span>
          </div>
          <label class="absolute -bottom-1 -right-1 p-1.5 bg-slate-900 text-white rounded-xl cursor-pointer hover:bg-indigo-600 transition-colors shadow-xs">
            <Upload class="w-3.5 h-3.5" />
            <input type="file" accept="image/*" class="hidden" @change="handleAvatarUpload" />
          </label>
        </div>

        <!-- Info Header -->
        <div class="flex-1 text-center sm:text-left space-y-1 min-w-0">
          <div class="flex flex-col sm:flex-row sm:items-center gap-2.5">
            <h2 class="text-lg sm:text-xl font-bold text-slate-800 truncate">{{ profile.fullName }}</h2>
            <StatusBadge type="employee" :value="profile.status" />
          </div>
          <div class="text-xs sm:text-sm text-indigo-700 font-mono font-semibold">{{ t('l_code_abbr') }}: {{ profile.code }}</div>
          <div class="text-xs sm:text-sm text-slate-500 flex flex-wrap items-center justify-center sm:justify-start gap-3 pt-1">
            <span class="flex items-center gap-1.5"><Building2 class="w-4 h-4 text-slate-400" /> {{ profile.departmentName }}</span>
            <span>•</span>
            <span class="flex items-center gap-1.5"><Briefcase class="w-4 h-4 text-slate-400" /> {{ profile.positionName }}</span>
            <span>•</span>
            <span class="flex items-center gap-1.5"><Calendar class="w-4 h-4 text-slate-400" /> {{ t('l_join_date_prefix') }}: {{ formatDate(profile.joinDate) }}</span>
          </div>
        </div>

        <!-- Quick Links -->
        <div class="flex sm:flex-col gap-2 shrink-0">
          <router-link
            to="/attendance"
            class="px-3.5 py-2 bg-indigo-50 hover:bg-indigo-100 text-indigo-700 font-semibold text-xs sm:text-sm rounded-xl flex items-center gap-1.5 transition-colors"
          >
            <CalendarCheck2 class="w-4 h-4" />
            {{ t('b_view_attendance') }}
          </router-link>
          <router-link
            to="/payroll"
            class="px-3.5 py-2 bg-emerald-50 hover:bg-emerald-100 text-emerald-700 font-semibold text-xs sm:text-sm rounded-xl flex items-center gap-1.5 transition-colors"
          >
            <DollarSign class="w-4 h-4" />
            {{ t('b_view_my_payslip') }}
          </router-link>
        </div>
      </div>

      <!-- Quick Leave Balance Banner -->
      <div v-if="leaveBalance" class="bg-gradient-to-r from-indigo-500 to-indigo-700 text-white p-4 sm:p-5 rounded-2xl shadow-xs flex flex-col sm:flex-row items-center justify-between gap-4">
        <div class="flex items-center gap-4 text-center sm:text-left">
          <div>
            <span class="text-xs text-indigo-100 uppercase tracking-wider font-semibold block">{{ t('l_leave_quota_year') }} {{ leaveBalance.year }}</span>
            <div class="text-xl sm:text-2xl font-black leading-tight mt-0.5">{{ leaveBalance.remainingLeaveDays }} {{ t('l_remaining_days_suffix') }}</div>
          </div>
          <div class="text-xs sm:text-sm text-indigo-100 hidden sm:block border-l border-indigo-400/50 pl-4">
            {{ t('l_total_days_text') }} {{ leaveBalance.totalLeaveDays }} {{ t('l_days_unit') }} — {{ t('l_used_days_text') }}: {{ leaveBalance.usedLeaveDays }} {{ t('l_days_unit') }}
          </div>
        </div>
        <router-link
          to="/leaves"
          class="px-4 py-2 bg-white text-indigo-700 hover:bg-indigo-50 font-bold text-xs sm:text-sm rounded-xl shadow-xs transition-colors shrink-0"
        >
          {{ t('b_create_leave_request') }} &rarr;
        </router-link>
      </div>

      <!-- Detail Information Sections (4 columns) -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 sm:gap-5">
        <!-- 1. Contact Info -->
        <div class="bg-white rounded-2xl p-5 border border-slate-200/80 shadow-xs space-y-3 text-xs sm:text-sm">
          <h3 class="font-bold text-slate-800 text-xs sm:text-sm uppercase tracking-wider border-b border-slate-100 pb-2.5 flex items-center gap-2 text-indigo-700">
            <Mail class="w-4 h-4 text-indigo-600" />
            {{ t('l_contact_info_section') }}
          </h3>
          <div class="space-y-2">
            <div class="flex justify-between items-center"><span class="text-slate-400">{{ t('l_email') }}:</span> <span class="font-medium text-slate-800 truncate max-w-[140px]">{{ profile.email }}</span></div>
            <div class="flex justify-between items-center"><span class="text-slate-400">{{ t('l_phone_number') }}:</span> <span class="font-medium text-slate-800">{{ profile.phoneNumber }}</span></div>
            <div class="flex justify-between items-center"><span class="text-slate-400">{{ t('l_gender') }}:</span> <span class="font-medium text-slate-800">{{ getGenderLabel(profile.gender) }}</span></div>
            <div class="flex justify-between items-center"><span class="text-slate-400">{{ t('l_date_of_birth') }}:</span> <span class="font-medium text-slate-800">{{ formatDate(profile.dateOfBirth) }}</span></div>
            <div class="flex justify-between items-center"><span class="text-slate-400">{{ t('l_address') }}:</span> <span class="font-medium text-slate-800 text-right truncate max-w-[130px]">{{ profile.address || '—' }}</span></div>
          </div>
        </div>

        <!-- 2. Legal / Identity -->
        <div class="bg-white rounded-2xl p-5 border border-slate-200/80 shadow-xs space-y-3 text-xs sm:text-sm">
          <h3 class="font-bold text-slate-800 text-xs sm:text-sm uppercase tracking-wider border-b border-slate-100 pb-2.5 flex items-center gap-2 text-indigo-700">
            <ShieldCheck class="w-4 h-4 text-indigo-600" />
            {{ t('l_legal_identity_section') }}
          </h3>
          <div class="space-y-2">
            <div class="flex justify-between items-center"><span class="text-slate-400">{{ t('l_citizen_id') }}:</span> <span class="font-medium text-slate-800">{{ profile.citizenId || '—' }}</span></div>
            <div class="flex justify-between items-center"><span class="text-slate-400">{{ t('l_citizen_id_issue_date') }}:</span> <span class="font-medium text-slate-800">{{ formatDate(profile.citizenIdIssueDate) }}</span></div>
            <div class="flex justify-between items-center"><span class="text-slate-400">{{ t('l_citizen_id_issue_place') }}:</span> <span class="font-medium text-slate-800 truncate max-w-[120px]">{{ profile.citizenIdIssuePlace || '—' }}</span></div>
            <div class="flex justify-between items-center"><span class="text-slate-400">{{ t('l_tax_code') }}:</span> <span class="font-medium text-slate-800">{{ profile.taxCode || '—' }}</span></div>
            <div class="flex justify-between items-center"><span class="text-slate-400">{{ t('l_social_insurance_number') }}:</span> <span class="font-medium text-slate-800">{{ profile.socialInsuranceNumber || '—' }}</span></div>
            <div class="flex justify-between items-center"><span class="text-slate-400">{{ t('l_dependents_count') }}:</span> <span class="font-medium text-slate-800">{{ profile.dependentsCount }} {{ t('l_people_count_unit') }}</span></div>
          </div>
        </div>

        <!-- 3. Banking -->
        <div class="bg-white rounded-2xl p-5 border border-slate-200/80 shadow-xs space-y-3 text-xs sm:text-sm">
          <h3 class="font-bold text-slate-800 text-xs sm:text-sm uppercase tracking-wider border-b border-slate-100 pb-2.5 flex items-center gap-2 text-indigo-700">
            <CreditCard class="w-4 h-4 text-indigo-600" />
            {{ t('l_bank_account_section') }}
          </h3>
          <div class="space-y-2">
            <div class="flex justify-between items-center"><span class="text-slate-400">{{ t('l_bank_account_number') }}:</span> <span class="font-mono font-bold text-slate-800">{{ profile.bankAccountNumber || '—' }}</span></div>
            <div class="flex justify-between items-center"><span class="text-slate-400">{{ t('l_bank_name') }}:</span> <span class="font-medium text-slate-800 truncate max-w-[130px]">{{ profile.bankName || '—' }}</span></div>
            <div class="flex justify-between items-center"><span class="text-slate-400">{{ t('l_bank_branch') }}:</span> <span class="font-medium text-slate-800 truncate max-w-[130px]">{{ profile.bankBranch || '—' }}</span></div>
          </div>
        </div>

        <!-- 4. Emergency -->
        <div class="bg-white rounded-2xl p-5 border border-slate-200/80 shadow-xs space-y-3 text-xs sm:text-sm">
          <h3 class="font-bold text-slate-800 text-xs sm:text-sm uppercase tracking-wider border-b border-slate-100 pb-2.5 flex items-center gap-2 text-indigo-700">
            <HeartHandshake class="w-4 h-4 text-indigo-600" />
            {{ t('l_emergency_contact_section') }}
          </h3>
          <div class="space-y-2">
            <div class="flex justify-between items-center"><span class="text-slate-400">{{ t('l_full_name') }}:</span> <span class="font-medium text-slate-800">{{ profile.emergencyContactName || '—' }}</span></div>
            <div class="flex justify-between items-center"><span class="text-slate-400">{{ t('l_phone_number') }}:</span> <span class="font-medium text-slate-800">{{ profile.emergencyContactPhone || '—' }}</span></div>
            <div class="flex justify-between items-center"><span class="text-slate-400">{{ t('l_emergency_contact_relationship') }}:</span> <span class="font-medium text-slate-800">{{ profile.emergencyContactRelationship || '—' }}</span></div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
