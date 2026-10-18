<script setup lang="ts">
import type { EmployeeDto } from '@/types';
import { formatDate } from '@/utils/formatters';
import { useI18n } from '@/locales';
import StatusBadge from '@/components/StatusBadge.vue';
import { Phone, Mail, Eye, Edit, Trash2 } from 'lucide-vue-next';

defineProps<{
  employees: EmployeeDto[];
  loading: boolean;
  isAdmin: boolean;
}>();

const emit = defineEmits<{
  (e: 'viewDetail', emp: EmployeeDto): void;
  (e: 'edit', emp: EmployeeDto): void;
  (e: 'delete', id: string): void;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="overflow-x-auto">
    <table class="w-full text-left border-collapse text-xs">
      <thead>
        <tr class="bg-slate-50/90 border-b border-slate-200/80 text-slate-500 font-semibold uppercase tracking-wider">
          <th class="py-3 px-4">{{ t('l_full_name') }}</th>
          <th class="py-3 px-4">{{ t('l_department') }}</th>
          <th class="py-3 px-4">{{ t('l_position') }}</th>
          <th class="py-3 px-4">{{ t('l_contact') }}</th>
          <th class="py-3 px-4">{{ t('l_join_date') }}</th>
          <th class="py-3 px-4">{{ t('l_status') }}</th>
          <th class="py-3 px-4 text-right">{{ t('l_actions') }}</th>
        </tr>
      </thead>
      <tbody class="divide-y divide-slate-100">
        <tr v-if="loading">
          <td colspan="7" class="py-10 text-center text-slate-400">
            {{ t('l_loading_data') }}
          </td>
        </tr>

        <tr v-else-if="employees.length === 0">
          <td colspan="7" class="py-10 text-center text-slate-400">
            {{ t('l_no_data') }}
          </td>
        </tr>

        <tr
          v-for="emp in employees"
          :key="emp.id"
          class="hover:bg-slate-50/70 transition-colors"
        >
          <!-- Employee Info -->
          <td class="py-3 px-4">
            <div class="flex items-center gap-3">
              <div class="w-8 h-8 rounded-xl bg-indigo-50 text-indigo-700 font-bold flex items-center justify-center shrink-0 border border-indigo-100 text-xs shadow-2xs">
                {{ emp.fullName.charAt(0) }}
              </div>
              <div>
                <div class="font-semibold text-slate-800 text-xs sm:text-sm leading-tight">{{ emp.fullName }}</div>
                <div class="text-[11px] text-indigo-600 font-mono font-medium">{{ emp.code }}</div>
              </div>
            </div>
          </td>

          <!-- Department -->
          <td class="py-3 px-4 text-slate-700 font-medium">
            {{ emp.departmentName || '—' }}
          </td>

          <!-- Position -->
          <td class="py-3 px-4 text-slate-700 font-medium">
            {{ emp.positionName || '—' }}
          </td>

          <!-- Contact -->
          <td class="py-3 px-4">
            <div class="space-y-0.5 text-xs">
              <div class="flex items-center gap-1.5 text-slate-600">
                <Phone class="w-3 h-3 text-slate-400" />
                <span>{{ emp.phoneNumber }}</span>
              </div>
              <div class="flex items-center gap-1.5 text-slate-500 text-[11px]">
                <Mail class="w-3 h-3 text-slate-400" />
                <span>{{ emp.email }}</span>
              </div>
            </div>
          </td>

          <!-- Join Date -->
          <td class="py-3 px-4 text-slate-600">
            {{ formatDate(emp.joinDate) }}
          </td>

          <!-- Status -->
          <td class="py-3 px-4">
            <StatusBadge type="employee" :value="emp.status" />
          </td>

          <!-- Actions -->
          <td class="py-3 px-4 text-right">
            <div class="inline-flex items-center gap-1">
              <button
                @click="emit('viewDetail', emp)"
                :title="t('b_view_detail')"
                class="p-1.5 text-slate-500 hover:text-indigo-600 hover:bg-indigo-50 rounded-lg transition-colors"
              >
                <Eye class="w-4 h-4" />
              </button>

              <button
                v-if="isAdmin"
                @click="emit('edit', emp)"
                :title="t('b_edit')"
                class="p-1.5 text-slate-500 hover:text-amber-600 hover:bg-amber-50 rounded-lg transition-colors"
              >
                <Edit class="w-4 h-4" />
              </button>

              <button
                v-if="isAdmin"
                @click="emit('delete', emp.id)"
                :title="t('b_delete')"
                class="p-1.5 text-slate-500 hover:text-rose-600 hover:bg-rose-50 rounded-lg transition-colors"
              >
                <Trash2 class="w-4 h-4" />
              </button>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
