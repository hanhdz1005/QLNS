using AutoMapper;
using QLNS.Core.Dto;
using QLNSCore.Entities;

namespace QLNS.API.Mapper
{
    public class MappingAttendance : Profile
    {
        public MappingAttendance()
        {
            CreateMap<Attendance, AttendanceDto>().ReverseMap();
            CreateMap<Attendance, AddAttendanceDto>().ReverseMap();
        }
    }
}
