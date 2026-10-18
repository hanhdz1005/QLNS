using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNSCore.Entities
{
    public class Position : BaseEntity<Guid>
    {
        public string PositionName { get; set; }
        public decimal BaseSalary { get; set; }
        public string Description { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
