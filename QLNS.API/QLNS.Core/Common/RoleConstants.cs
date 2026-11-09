namespace QLNS.Core.Common
{
    public static class RoleConstants
    {
        public const string Admin = "Admin";
        public const string HRManager = "HRManager";
        public const string HRStaff = "HRStaff";
        public const string Accountant = "Accountant";
        public const string DepartmentManager = "DepartmentManager";
        public const string Employee = "Employee";

        // Nhóm quyền tổng hợp theo 6 tầng doanh nghiệp
        public const string AdminOrHRManager = "Admin, HRManager";
        public const string AdminOrHR = "Admin, HRManager, HRStaff";
        public const string AdminOrAccountant = "Admin, HRManager, HRStaff, Accountant";
        public const string PayrollRoles = "Admin, HRManager, Accountant";
        public const string LifecycleViewAllRoles = "Admin, HRManager, HRStaff, DepartmentManager";
        public const string ApproverRoles = "Admin, HRManager, DepartmentManager";
        public const string AttendanceViewAllRoles = "Admin, HRManager, HRStaff, Accountant, DepartmentManager";
        public const string AllRoles = "Admin, HRManager, HRStaff, Accountant, DepartmentManager, Employee";

        public static readonly string[] CoreRoles =
        {
            Admin,
            HRManager,
            HRStaff,
            Accountant,
            DepartmentManager,
            Employee
        };
    }
}
