using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Core.Dto
{
    public class RegisterDto
    {
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterInfoDto
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Token { get; set; }
    }

    public class GetCurrentUserDto
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Role { get; set; }
        public bool HasRegisteredFace { get; set; }
        public string? FaceImageUrl { get; set; }
    }
}
