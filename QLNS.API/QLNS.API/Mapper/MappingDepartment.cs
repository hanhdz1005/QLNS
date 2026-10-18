using AutoMapper;
using QLNS.Core.Dto;
using QLNSCore.Entities;

namespace QLNS.API.Mapper
{
    public class MappingDepartment : Profile
    {
        public MappingDepartment()
        {
            CreateMap<DepartmentDto, Department>().ReverseMap();
            CreateMap<Department, AddDepartmentDto>().ReverseMap();
        }
    }
}
