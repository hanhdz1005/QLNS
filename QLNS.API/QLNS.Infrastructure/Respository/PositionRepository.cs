using QLNSCore.Entities;
using QLNS.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Infrastructure.Respository
{
    public class PositionRepository : GenericRepository<Position>, IPositionRepository
    {
        public PositionRepository(AppDbContext context) : base(context)
        {
        }
    }
}
