using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Core.Dto
{
    public class PositionDto
    {
        public Guid Id { get; set; }
        public string PositionName { get; set; }
        public decimal BaseSalary { get; set; }
        public string Description { get; set; }
    }
    public class AddPositionDto
    {
        [Required(ErrorMessage = "PositionName is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "PositionName must be between 2 and 100 characters")]
        public string PositionName { get; set; }

        [Required(ErrorMessage = "BaseSalary is required")]
        [Range(0, double.MaxValue, ErrorMessage = "BaseSalary must be non-negative")]
        public decimal BaseSalary { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }
    }
      
}
