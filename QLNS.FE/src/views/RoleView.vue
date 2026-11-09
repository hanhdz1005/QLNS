<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { useToastStore } from '@/stores/toast';
import { useI18n } from '@/locales';
import { roleApi } from '@/api/role.api';
import type { IdentityRole, UserInfoDto, AssignRoleDto, AssignClaimDto } from '@/types';
import ConfirmModal from '@/components/ConfirmModal.vue';
import RoleCreateModal from '@/views/role/components/RoleCreateModal.vue';
import RoleAssignModal from '@/views/role/components/RoleAssignModal.vue';
import RoleClaimModal from '@/views/role/components/RoleClaimModal.vue';
import {
  ShieldAlert,
  Plus,
  Key,
  Trash2,
  Shield,
  Users,
} from 'lucide-vue-next';

const toastStore = useToastStore();
const { t } = useI18n();

const coreRoles = ['Admin', 'HRManager', 'HRStaff', 'Accountant', 'DepartmentManager', 'Employee'];

const roles = ref<IdentityRole[]>([]);
const users = ref<UserInfoDto[]>([]);
const loading = ref(false);

const loadData = async () => {
  loading.value = true;
  try {
    const [rolesRes, usersRes] = await Promise.all([
      roleApi.getAllRoles(),
      roleApi.getAllUsers(),
    ]);

    if (rolesRes.isSuccess && rolesRes.data?.items) {
      roles.value = rolesRes.data.items;
    }
    if (usersRes.isSuccess && usersRes.data?.items) {
      users.value = usersRes.data.items;
    }
  } catch {
    toastStore.error(t('e_load_roles_failed'));
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  loadData();
});

const getRoleBadgeInfo = (roleName?: string) => {
  switch (roleName) {
    case 'Admin':
      return {
        label: t('s_role_admin'),
        desc: t('l_full_access_badge'),
        badgeClass: 'bg-indigo-50 text-indigo-700 border-indigo-200',
        pillClass: 'bg-indigo-100 text-indigo-700',
      };
    case 'HRManager':
      return {
        label: t('s_role_hr_manager'),
        desc: t('l_hr_manager_access_badge'),
        badgeClass: 'bg-violet-50 text-violet-700 border-violet-200',
        pillClass: 'bg-violet-100 text-violet-700',
      };
    case 'HRStaff':
      return {
        label: t('s_role_hr_staff'),
        desc: t('l_hr_staff_access_badge'),
        badgeClass: 'bg-cyan-50 text-cyan-700 border-cyan-200',
        pillClass: 'bg-cyan-100 text-cyan-700',
      };
    case 'Accountant':
      return {
        label: t('s_role_accountant'),
        desc: t('l_accountant_access_badge'),
        badgeClass: 'bg-emerald-50 text-emerald-700 border-emerald-200',
        pillClass: 'bg-emerald-100 text-emerald-700',
      };
    case 'DepartmentManager':
      return {
        label: t('s_role_department_manager'),
        desc: t('l_dept_manager_access_badge'),
        badgeClass: 'bg-amber-50 text-amber-700 border-amber-200',
        pillClass: 'bg-amber-100 text-amber-700',
      };
    default:
      return {
        label: t('s_role_employee'),
        desc: t('l_employee_access_badge'),
        badgeClass: 'bg-sky-50 text-sky-700 border-sky-200',
        pillClass: 'bg-sky-100 text-sky-700',
      };
  }
};

// Create Role Modal
const showCreateRoleModal = ref(false);
const createRoleLoading = ref(false);

const handleCreateRole = async (newRoleName: string) => {
  if (!newRoleName.trim()) {
    toastStore.warning(t('e_enter_role_name'));
    return;
  }

  createRoleLoading.value = true;
  try {
    const res = await roleApi.createRole(newRoleName.trim());
    if (res.isSuccess) {
      toastStore.success(t('t_role_created'));
      showCreateRoleModal.value = false;
      loadData();
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_create_role_failed');
    toastStore.error(msg);
  } finally {
    createRoleLoading.value = false;
  }
};

// Delete Role
const showDeleteRoleModal = ref(false);
const deleteRoleName = ref('');
const deleteRoleLoading = ref(false);

const confirmDeleteRole = (name: string) => {
  deleteRoleName.value = name;
  showDeleteRoleModal.value = true;
};

const handleDeleteRole = async () => {
  deleteRoleLoading.value = true;
  try {
    const res = await roleApi.deleteRole(deleteRoleName.value);
    if (res.isSuccess) {
      toastStore.success(t('t_role_deleted'));
      showDeleteRoleModal.value = false;
      loadData();
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_delete_role_failed');
    toastStore.error(msg);
  } finally {
    deleteRoleLoading.value = false;
  }
};

// Assign Role Modal
const showAssignRoleModal = ref(false);
const assignRoleLoading = ref(false);
const assignRoleForm = reactive<AssignRoleDto>({
  email: '',
  role: 'Employee',
});

const openAssignRole = (u: UserInfoDto) => {
  assignRoleForm.email = u.email;
  assignRoleForm.role = u.role || 'Employee';
  showAssignRoleModal.value = true;
};

const handleAssignRole = async () => {
  assignRoleLoading.value = true;
  try {
    const res = await roleApi.assignRole(assignRoleForm);
    if (res.isSuccess) {
      toastStore.success(t('t_role_assigned'));
      showAssignRoleModal.value = false;
      loadData();
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_assign_role_failed');
    toastStore.error(msg);
  } finally {
    assignRoleLoading.value = false;
  }
};

// Assign Claim Modal
const showClaimModal = ref(false);
const claimLoading = ref(false);
const claimForm = reactive<AssignClaimDto>({
  email: '',
  claimValue: 'FULL.ACCESS',
});

const openAssignClaim = (u: UserInfoDto) => {
  claimForm.email = u.email;
  claimForm.claimValue = 'FULL.ACCESS';
  showClaimModal.value = true;
};

const handleAssignClaim = async () => {
  claimLoading.value = true;
  try {
    const res = await roleApi.assignClaim(claimForm);
    if (res.isSuccess) {
      toastStore.success(t('t_claim_assigned'));
      showClaimModal.value = false;
    }
  } catch (error: any) {
    const msg = error.response?.data?.message || t('e_assign_claim_failed');
    toastStore.error(msg);
  } finally {
    claimLoading.value = false;
  }
};
</script>

<template>
  <div class="space-y-4 sm:space-y-5">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3">
      <div>
        <h1 class="text-xl sm:text-2xl font-bold text-slate-800 tracking-tight flex items-center gap-2">
          <ShieldAlert class="w-6 h-6 text-indigo-600" />
          {{ t('l_roles_page_title') }}
        </h1>
        <p class="text-xs sm:text-sm text-slate-500 mt-1">
          {{ t('l_roles_page_subtitle') }}
        </p>
      </div>

      <button
        @click="showCreateRoleModal = true"
        class="inline-flex items-center gap-2 px-4 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-xs sm:text-sm font-semibold shadow-xs shadow-indigo-600/20 transition-colors"
      >
        <Plus class="w-4 h-4" />
        {{ t('b_create_role') }}
      </button>
    </div>

    <!-- 2 Columns: Roles List & Users Table -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-4 sm:gap-5 items-start">
      <!-- Left: Roles Cards -->
      <div class="bg-white rounded-2xl p-5 border border-slate-200/80 shadow-xs space-y-4">
        <h2 class="text-sm font-bold text-slate-800 flex items-center gap-2 pb-3 border-b border-slate-100">
          <Shield class="w-4 h-4 text-indigo-600" />
          {{ t('l_roles_list_title') }}
        </h2>

        <div class="space-y-2.5">
          <div
            v-for="r in roles"
            :key="r.id"
            class="p-3 rounded-xl bg-slate-50 border border-slate-100 flex items-center justify-between"
          >
            <div class="flex flex-col gap-1">
              <div class="flex flex-wrap items-center gap-2">
                <span class="font-bold text-slate-800 text-xs sm:text-sm">{{ r.name }}</span>
                <span :class="['text-[11px] font-semibold px-2 py-0.5 rounded-lg', getRoleBadgeInfo(r.name).pillClass]">
                  {{ getRoleBadgeInfo(r.name).label }}
                </span>
              </div>
              <span class="text-[11px] text-slate-500">{{ getRoleBadgeInfo(r.name).desc }}</span>
            </div>

            <button
              v-if="!coreRoles.includes(r.name)"
              @click="confirmDeleteRole(r.name)"
              class="p-1.5 text-slate-400 hover:text-rose-600 rounded-lg transition-colors"
              :title="t('l_delete_this_role')"
            >
              <Trash2 class="w-4 h-4" />
            </button>
          </div>
        </div>
      </div>

      <!-- Right: Users Accounts Table -->
      <div class="lg:col-span-2 bg-white rounded-2xl border border-slate-200/80 shadow-xs overflow-hidden">
        <div class="p-4 sm:p-5 border-b border-slate-100 flex items-center justify-between">
          <h2 class="text-sm font-bold text-slate-800 flex items-center gap-2">
            <Users class="w-4 h-4 text-indigo-600" />
            {{ t('l_users_accounts_title') }}
          </h2>
          <span class="text-xs sm:text-sm text-slate-500 font-medium">{{ users.length }} {{ t('l_accounts_unit') }}</span>
        </div>

        <div class="overflow-x-auto" tabindex="0" role="region" :aria-label="t('l_users_accounts_title')">
          <table class="w-full text-left border-collapse text-xs sm:text-sm">
            <thead class="bg-slate-50/90 border-b border-slate-200/80 text-slate-500 font-semibold uppercase tracking-wider text-[11px]">
              <tr>
                <th class="py-3 px-4">{{ t('l_user_name') }}</th>
                <th class="py-3 px-4">{{ t('l_display_name') }}</th>
                <th class="py-3 px-4">{{ t('l_email') }}</th>
                <th class="py-3 px-4">{{ t('l_primary_role') }}</th>
                <th class="py-3 px-4 text-right">{{ t('l_permissions_col') }}</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-100">
              <tr v-if="loading">
                <td colspan="5" class="py-12 text-center text-slate-400 text-xs sm:text-sm">{{ t('l_loading_accounts') }}</td>
              </tr>
              <tr v-else-if="users.length === 0">
                <td colspan="5" class="py-12 text-center text-slate-400 text-xs sm:text-sm">{{ t('l_no_accounts') }}</td>
              </tr>
              <tr
                v-for="u in users"
                :key="u.id"
                class="hover:bg-slate-50/60 transition-colors"
              >
                <td class="py-3 px-4 font-mono font-semibold text-slate-800">{{ u.userName }}</td>
                <td class="py-3 px-4 text-slate-700 font-medium">{{ u.displayName }}</td>
                <td class="py-3 px-4 text-slate-500">{{ u.email }}</td>
                <td class="py-3 px-4">
                  <span
                    :class="[
                      'px-2.5 py-1 rounded-lg text-xs font-semibold border',
                      getRoleBadgeInfo(u.role).badgeClass,
                    ]"
                  >
                    {{ getRoleBadgeInfo(u.role).label }}
                  </span>
                </td>
                <td class="py-3 px-4 text-right">
                  <div class="inline-flex items-center gap-1.5">
                    <button
                      @click="openAssignRole(u)"
                      class="px-2.5 py-1.5 bg-indigo-50 hover:bg-indigo-100 text-indigo-700 border border-indigo-200 rounded-lg font-medium text-xs transition-colors"
                    >
                      {{ t('b_change_role') }}
                    </button>
                    <button
                      @click="openAssignClaim(u)"
                      class="px-2.5 py-1.5 bg-amber-50 hover:bg-amber-100 text-amber-700 border border-amber-200 rounded-lg font-medium text-xs transition-colors flex items-center gap-1"
                    >
                      <Key class="w-3.5 h-3.5" />
                      {{ t('b_grant_claim') }}
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Modals -->
    <RoleCreateModal
      v-model="showCreateRoleModal"
      :loading="createRoleLoading"
      @submit="handleCreateRole"
    />

    <RoleAssignModal
      v-model="showAssignRoleModal"
      :form="assignRoleForm"
      :roles="roles"
      :loading="assignRoleLoading"
      @submit="handleAssignRole"
    />

    <RoleClaimModal
      v-model="showClaimModal"
      :form="claimForm"
      :loading="claimLoading"
      @submit="handleAssignClaim"
    />

    <!-- Delete Role Confirm -->
    <ConfirmModal
      v-model="showDeleteRoleModal"
      :title="t('l_confirm_delete_role_title')"
      :message="t('l_confirm_delete_role_msg')"
      :loading="deleteRoleLoading"
      @confirm="handleDeleteRole"
    />
  </div>
</template>
