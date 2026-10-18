using QLNSCore.Entities;
using System.Threading.Tasks;

namespace QLNS.Core.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<Employee?> GetByIdentifierAsync(string identifier);
    }
}
