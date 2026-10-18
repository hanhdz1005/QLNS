using AutoMapper;
using QLNS.Core.Dto;
using QLNS.Core.Dto;
using QLNSCore.Entities;

namespace QLNS.API.Mapper
{
    public class MappingBonusPenalty : Profile
    {
        public MappingBonusPenalty()
        {
            CreateMap<BonusPenalty, BonusPenaltyDto>().ReverseMap();
            CreateMap<BonusPenalty, AddBonusPenaltyDto>().ReverseMap();
        }
    }
}
