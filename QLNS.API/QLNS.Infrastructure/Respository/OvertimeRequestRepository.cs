using QLNS.Core.Interfaces;
using QLNSCore.Entities;

namespace QLNS.Infrastructure.Respository
{
    public class OvertimeRequestRepository : GenericRepository<OvertimeRequest>, IOvertimeRequestRepository
    {
        public OvertimeRequestRepository(AppDbContext context) : base(context)
        {
        }
    }
}
