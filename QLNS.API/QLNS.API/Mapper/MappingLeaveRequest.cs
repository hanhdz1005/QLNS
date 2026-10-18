using AutoMapper;
using QLNS.Core.Dto;
using QLNSCore.Entities;

namespace QLNS.API.Mapper
{
    public class MappingLeaveRequest : Profile
    {
        public MappingLeaveRequest()
        {
            CreateMap<LeaveRequest, LeaveRequestDto>().ReverseMap();
            CreateMap<LeaveRequest, AddLeaveRequestDto>().ReverseMap();
        }
    }
}
