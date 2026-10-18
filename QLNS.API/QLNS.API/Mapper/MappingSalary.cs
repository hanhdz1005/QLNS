using AutoMapper;
using QLNS.Core.Dto;
using QLNSCore.Entities;

namespace QLNS.API.Mapper
{
    public class MappingSalary : Profile
    {
        public MappingSalary()
        {
            CreateMap<Salary, SalariesDto>().ReverseMap();
            CreateMap<Salary, AddSalariesDto>().ReverseMap();
        }
    }
}
