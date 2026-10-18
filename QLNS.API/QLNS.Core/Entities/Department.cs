using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNSCore.Entities
{
    public class Department : BaseEntity<Guid>
    {
        public string DepartmentName { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
         public DateTime CreateDate { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
