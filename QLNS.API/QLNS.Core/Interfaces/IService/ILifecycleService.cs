using QLNS.Core.Dto;
using System;
using System.Threading.Tasks;

namespace QLNS.Core.Interfaces.IService
{
    public interface ILifecycleService
    {
        Task<CompanyLifecycleOverviewDto> GetCompanyOverviewAsync(int month, int year);
        Task<EmployeeLifecycleDetailDto> GetEmployeeLifecycleAsync(Guid employeeId, int month, int year);
        Task<EmployeeLifecycleDetailDto> GetMyLifecycleAsync(string userIdentifier, int month, int year);
    }
}
