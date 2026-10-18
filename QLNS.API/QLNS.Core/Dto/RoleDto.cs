using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Core.Dto
{
    public class RolesDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class AddRolesDto
    {
        public string Name { get; set; }
    }

    public class UserInfoDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Role { get; set; }
    }

    public class AssignRoleDto
    {
        public string Email { get; set; }
        public string Role { get; set; }
    }

    public class AssignClaimDto
    {
        public string Email { get; set; }
        public string ClaimValue { get; set; }
    }
}
