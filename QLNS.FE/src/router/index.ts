import { createRouter, createWebHistory } from 'vue-router';
import AppLayout from '@/layouts/AppLayout.vue';
import LoginView from '@/views/LoginView.vue';
import { normalizeAppRole, type AppRole } from '@/stores/auth';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: LoginView,
      meta: { guestOnly: true, title: 'Đăng nhập hệ thống' },
    },
    {
      path: '/',
      component: AppLayout,
      meta: { requiresAuth: true },
      children: [
        {
          path: '',
          name: 'dashboard',
          component: () => import('@/views/DashboardView.vue'),
          meta: {
            roles: ['Admin', 'Accountant', 'Employee'] as AppRole[],
            title: 'Tổng quan hệ thống',
          },
        },
        {
          path: 'employees',
          name: 'employees',
          component: () => import('@/views/EmployeeView.vue'),
          meta: {
            roles: ['Admin', 'Accountant'] as AppRole[],
            title: 'Quản lý Nhân sự',
          },
        },
        {
          path: 'departments',
          name: 'departments',
          component: () => import('@/views/DepartmentView.vue'),
          meta: {
            roles: ['Admin', 'Accountant'] as AppRole[],
            title: 'Quản lý Phòng ban',
          },
        },
        {
          path: 'positions',
          name: 'positions',
          component: () => import('@/views/PositionView.vue'),
          meta: {
            roles: ['Admin', 'Accountant'] as AppRole[],
            title: 'Quản lý Chức vụ',
          },
        },
        {
          path: 'contracts',
          name: 'contracts',
          component: () => import('@/views/ContractView.vue'),
          meta: {
            roles: ['Admin', 'Accountant'] as AppRole[],
            title: 'Hợp đồng Lao động',
          },
        },
        {
          path: 'attendance',
          name: 'attendance',
          component: () => import('@/views/AttendanceView.vue'),
          meta: {
            roles: ['Admin', 'Accountant', 'Employee'] as AppRole[],
            title: 'Chấm công Face ID',
          },
        },
        {
          path: 'leaves',
          name: 'leaves',
          component: () => import('@/views/LeaveManagementView.vue'),
          meta: {
            roles: ['Admin', 'Accountant', 'Employee'] as AppRole[],
            title: 'Quản lý Nghỉ phép',
          },
        },
        {
          path: 'payroll',
          name: 'payroll',
          component: () => import('@/views/PayrollView.vue'),
          meta: {
            roles: ['Admin', 'Accountant', 'Employee'] as AppRole[],
            title: 'Bảng tính lương & Phiếu lương',
          },
        },
        {
          path: 'salaries',
          name: 'salaries',
          component: () => import('@/views/SalaryStructureView.vue'),
          meta: {
            roles: ['Admin', 'Accountant'] as AppRole[],
            title: 'Mức lương cơ sở',
          },
        },
        {
          path: 'bonus-penalty',
          name: 'bonus-penalty',
          component: () => import('@/views/BonusPenaltyView.vue'),
          meta: {
            roles: ['Admin', 'Accountant'] as AppRole[],
            title: 'Khen thưởng & Kỷ luật',
          },
        },
        {
          path: 'reports',
          name: 'reports',
          component: () => import('@/views/ReportView.vue'),
          meta: {
            roles: ['Admin', 'Accountant'] as AppRole[],
            title: 'Báo cáo & Thống kê',
          },
        },
        {
          path: 'roles',
          name: 'roles',
          component: () => import('@/views/RoleView.vue'),
          meta: {
            roles: ['Admin'] as AppRole[],
            title: 'Phân quyền & Tài khoản',
          },
        },
        {
          path: 'profile',
          name: 'profile',
          component: () => import('@/views/ProfileView.vue'),
          meta: {
            roles: ['Admin', 'Accountant', 'Employee'] as AppRole[],
            title: 'Hồ sơ cá nhân',
          },
        },
      ],
    },
    {
      path: '/:pathMatch(.*)*',
      redirect: '/',
    },
  ],
});

router.beforeEach((to, _from, next) => {
  const token = localStorage.getItem('access_token');
  const userInfo = localStorage.getItem('user_info');
  const user = userInfo ? JSON.parse(userInfo) : null;

  document.title = to.meta.title ? `${to.meta.title} - HRMS Pro` : 'HRMS Pro - Quản Lý Nhân Sự';

  if (to.meta.guestOnly && token) {
    return next('/');
  }

  if (to.meta.requiresAuth !== false && !to.meta.guestOnly && !token) {
    return next('/login');
  }

  const allowedRoles = to.meta.roles as AppRole[] | undefined;
  if (allowedRoles && allowedRoles.length > 0) {
    const currentRole = normalizeAppRole(user?.role);
    if (!allowedRoles.includes(currentRole)) {
      return next('/');
    }
  }

  next();
});

export default router;
