using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNSCore.Entities
{
    public class BaseEntity<T>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
