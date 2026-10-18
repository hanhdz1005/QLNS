using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Core.Dto
{
    public class DepartmentDto
    {
        public Guid Id { get; set; }
        public string DepartmentName { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class AddDepartmentDto
    {
        [Required(ErrorMessage = "DepartmentName is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "DepartmentName must be between 2 and 100 characters")]
        public string DepartmentName { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }
    }
}
