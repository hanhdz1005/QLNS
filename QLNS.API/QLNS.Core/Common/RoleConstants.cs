namespace QLNS.Core.Common
{
    public static class RoleConstants
    {
        public const string Admin = "Admin";
        public const string Accountant = "Accountant";
        public const string Employee = "Employee";

        public const string AdminOrAccountant = "Admin, Accountant";
        public const string AllRoles = "Admin, Accountant, Employee";

        public static readonly string[] CoreRoles = { Admin, Accountant, Employee };
    }
}
