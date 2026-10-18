using Microsoft.EntityFrameworkCore;
using QLNS.Core.Interfaces;
using QLNSCore.Entities;
using System.Threading.Tasks;

namespace QLNS.Infrastructure.Respository
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Employee?> GetByIdentifierAsync(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return null;

            identifier = identifier.Trim();

            return await _context.Employee
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(e => e.Email == identifier || e.Code == identifier || e.Id.ToString() == identifier);
        }
    }
}
