import { createRouter, createWebHistory } from 'vue-router';
import AppLayout from '@/layouts/AppLayout.vue';
import LoginView from '@/views/LoginView.vue';

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
          meta: { title: 'Tổng quan hệ thống' },
        },
        {
          path: 'employees',
          name: 'employees',
          component: () => import('@/views/EmployeeView.vue'),
          meta: { title: 'Quản lý Nhân sự' },
        },
        {
          path: 'departments',
          name: 'departments',
          component: () => import('@/views/DepartmentView.vue'),
          meta: { title: 'Quản lý Phòng ban' },
        },
        {
          path: 'positions',
          name: 'positions',
          component: () => import('@/views/PositionView.vue'),
          meta: { title: 'Quản lý Chức vụ' },
        },
        {
          path: 'contracts',
          name: 'contracts',
          component: () => import('@/views/ContractView.vue'),
          meta: { title: 'Hợp đồng Lao động' },
        },
        {
          path: 'attendance',
          name: 'attendance',
          component: () => import('@/views/AttendanceView.vue'),
          meta: { title: 'Chấm công & QR Kiosk' },
        },
        {
          path: 'leaves',
          name: 'leaves',
          component: () => import('@/views/LeaveManagementView.vue'),
          meta: { title: 'Quản lý Nghỉ phép' },
        },
        {
          path: 'payroll',
          name: 'payroll',
          component: () => import('@/views/PayrollView.vue'),
          meta: { title: 'Bảng tính lương' },
        },
        {
          path: 'salaries',
          name: 'salaries',
          component: () => import('@/views/SalaryStructureView.vue'),
          meta: { title: 'Mức lương cơ sở' },
        },
        {
          path: 'bonus-penalty',
          name: 'bonus-penalty',
          component: () => import('@/views/BonusPenaltyView.vue'),
          meta: { title: 'Khen thưởng & Kỷ luật' },
        },
        {
          path: 'reports',
          name: 'reports',
          component: () => import('@/views/ReportView.vue'),
          meta: { title: 'Báo cáo & Thống kê' },
        },
        {
          path: 'roles',
          name: 'roles',
          component: () => import('@/views/RoleView.vue'),
          meta: { requiresAdmin: true, title: 'Phân quyền & Tài khoản' },
        },
        {
          path: 'profile',
          name: 'profile',
          component: () => import('@/views/ProfileView.vue'),
          meta: { title: 'Hồ sơ cá nhân' },
        },
      ],
    },
    {
      path: '/:pathMatch(.*)*',
      redirect: '/',
    },
  ],
});

// Navigation Guard
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('access_token');
  const userInfo = localStorage.getItem('user_info');
  const user = userInfo ? JSON.parse(userInfo) : null;

  // Set document title
  document.title = to.meta.title ? `${to.meta.title} - HRMS Pro` : 'HRMS Pro - Quản Lý Nhân Sự';

  if (to.meta.guestOnly && token) {
    return next('/');
  }

  if (to.meta.requiresAuth !== false && !to.meta.guestOnly && !token) {
    return next('/login');
  }

  if (to.meta.requiresAdmin && user?.role?.toLowerCase() !== 'admin') {
    return next('/');
  }

  next();
});

export default router;
