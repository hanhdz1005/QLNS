using AutoMapper;
using QLNS.Core.Dto;
using QLNSCore.Entities;

namespace QLNS.API.Mapper
{
    public class MappingEmployee : Profile
    {
        public MappingEmployee()
        {
            CreateMap<Employee, EmployeeDto>().ReverseMap();
            CreateMap<Employee, AddEmployeeDto>().ReverseMap();
        }
    }
}
