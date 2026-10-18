using AutoMapper;
using QLNS.Core.Dto;
using QLNSCore.Entities;

namespace QLNS.API.Mapper
{
    public class MappingPosition : Profile
    {
        public MappingPosition()
        {
            CreateMap<Position, PositionDto>().ReverseMap();
            CreateMap<Position, AddPositionDto>().ReverseMap();
        }
    }
}
