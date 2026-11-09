<script setup lang="ts">
import { useI18n } from '@/locales';
import { getFaceErrorLocaleKey } from '@/utils/faceAnalyzer';
import type { FaceCheckInResultDto } from '@/types';
import { AlertTriangle, UserCheck, Clock, RefreshCw, ShieldCheck } from 'lucide-vue-next';

defineProps<{
  cameraErrorCode: string | null;
  verificationErrorCode: string | null;
  checkInResult: FaceCheckInResultDto | null;
  lastSnapshotPreview: string;
  cooldownSeconds: number;
}>();

const emit = defineEmits<{
  (e: 'scanNextNow'): void;
}>();

const { t } = useI18n();
const getErrorMessageByCode = (code: string | null) => t(getFaceErrorLocaleKey(code));
</script>

<template>
  <div class="space-y-4">
    <!-- Cooldown Banner after Auto-Scan -->
    <div
      v-if="cooldownSeconds > 0 && checkInResult?.success"
      class="p-3.5 rounded-2xl bg-indigo-50 border border-indigo-200 flex items-center justify-between gap-3"
    >
      <div class="flex items-center gap-2.5 text-xs font-semibold text-indigo-900">
        <Clock class="w-4 h-4 text-indigo-600 shrink-0 animate-pulse" />
        <span>{{ t('l_face_cooldown_active') }} {{ cooldownSeconds }}s</span>
      </div>
      <button
        type="button"
        class="px-3 py-1.5 rounded-xl bg-indigo-600 hover:bg-indigo-700 text-white text-xs font-bold inline-flex items-center gap-1.5 transition-colors shrink-0"
        @click="emit('scanNextNow')"
      >
        <RefreshCw class="w-3.5 h-3.5" />
        {{ t('b_scan_next_now') }}
      </button>
    </div>

    <!-- Error Code Card -->
    <div
      v-if="cameraErrorCode || verificationErrorCode"
      class="p-4 rounded-2xl bg-rose-50 border border-rose-200 space-y-2.5"
    >
      <div class="flex items-center gap-2 text-rose-800 font-bold text-xs">
        <AlertTriangle class="w-4 h-4 text-rose-600 shrink-0" />
        <span>{{ t('l_error_code_label') }}</span>
      </div>
      <div class="px-3 py-1.5 rounded-xl bg-rose-900 text-rose-100 font-mono text-xs font-bold tracking-wide">
        {{ cameraErrorCode || verificationErrorCode }}
      </div>
      <p class="text-xs text-rose-700 leading-relaxed">
        {{ getErrorMessageByCode(cameraErrorCode || verificationErrorCode) }}
      </p>
      <p
        v-if="checkInResult && checkInResult.similarityScore !== undefined"
        class="text-xs font-semibold text-rose-800"
      >
        {{ t('l_similarity_score') }}: {{ checkInResult.similarityScore }}%
      </p>
    </div>

    <!-- Success Verification Card (First-In / Last-Out + Regular & Approved OT) -->
    <div
      v-if="checkInResult && checkInResult.success"
      class="p-4 rounded-2xl bg-emerald-50 border border-emerald-200 space-y-3"
    >
      <div class="flex items-center justify-between">
        <div class="flex items-center gap-2 text-emerald-800 font-bold text-sm">
          <UserCheck class="w-5 h-5 text-emerald-600" />
          <span>{{ t('l_attendance_success') }}</span>
        </div>
        <span class="px-2.5 py-0.5 rounded-full bg-emerald-200/80 text-emerald-900 text-xs font-bold">
          {{ t('l_similarity_score') }}: {{ checkInResult.similarityScore }}%
        </span>
      </div>

      <div class="space-y-1.5 text-xs text-emerald-900">
        <p class="text-sm font-bold text-emerald-950">
          {{ checkInResult.employeeName }}
        </p>
        <div class="grid grid-cols-2 gap-2 pt-1">
          <div class="p-2 rounded-xl bg-white/80 border border-emerald-200/70">
            <span class="text-[11px] text-slate-500 block">{{ t('l_first_check_in_label') }}</span>
            <strong class="font-mono text-emerald-800">{{ checkInResult.firstCheckIn || checkInResult.time }}</strong>
          </div>
          <div class="p-2 rounded-xl bg-white/80 border border-emerald-200/70">
            <span class="text-[11px] text-slate-500 block">{{ t('l_last_check_out_label') }}</span>
            <strong class="font-mono text-indigo-800">{{ checkInResult.lastCheckOut || '--:--:--' }}</strong>
          </div>
          <div class="p-2 rounded-xl bg-white/80 border border-emerald-200/70">
            <span class="text-[11px] text-slate-500 block">{{ t('l_regular_hours_label') }}</span>
            <strong class="font-mono text-slate-800">{{ checkInResult.regularHours ?? 0 }}h / 8h</strong>
          </div>
          <div class="p-2 rounded-xl bg-white/80 border border-emerald-200/70">
            <span class="text-[11px] text-slate-500 block">{{ t('l_approved_ot_hours_label') }}</span>
            <strong class="font-mono text-amber-700">+{{ checkInResult.overtimeHours ?? 0 }}h</strong>
          </div>
        </div>
        <div class="pt-1 flex items-center justify-between text-xs font-semibold border-t border-emerald-200/70">
          <span>{{ t('l_scan_count_label') }}: #{{ checkInResult.scanCount ?? 1 }}</span>
          <span>{{ t('l_total_hours') }}: {{ checkInResult.hoursWorked ?? 0 }}h</span>
        </div>
      </div>
    </div>

    <!-- Guide Card when waiting for scan -->
    <div
      v-if="!checkInResult && !cameraErrorCode && !verificationErrorCode"
      class="p-4 rounded-2xl bg-slate-50 border border-slate-200/80 space-y-2"
    >
      <div class="flex items-center gap-2 text-xs font-bold text-slate-800">
        <ShieldCheck class="w-4 h-4 text-indigo-600" />
        <span>{{ t('b_toggle_auto_scan') }}</span>
      </div>
      <ul class="text-xs text-slate-600 space-y-1.5 list-disc list-inside">
        <li>{{ t('l_face_guide_1') }}</li>
        <li>{{ t('l_face_guide_2') }}</li>
        <li>{{ t('l_face_guide_3') }}</li>
      </ul>
    </div>

    <!-- Snapshot Comparison Preview -->
    <div
      v-if="lastSnapshotPreview"
      class="p-3.5 rounded-2xl bg-slate-50 border border-slate-200/80 flex items-center gap-3.5"
    >
      <img
        :src="lastSnapshotPreview"
        alt="Snapshot"
        class="w-16 h-16 rounded-xl object-cover border border-slate-300 -scale-x-100 shrink-0"
      />
      <div class="text-xs space-y-1">
        <p class="font-bold text-slate-800">{{ t('l_face_step_b3') }}</p>
        <p class="text-slate-500">{{ t('l_face_detected_valid') }}</p>
      </div>
    </div>
  </div>
</template>
