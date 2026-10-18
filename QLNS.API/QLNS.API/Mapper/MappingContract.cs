using AutoMapper;
using QLNS.Core.Dto;
using QLNSCore.Entities;

namespace QLNS.API.Mapper
{
    public class MappingContract : Profile
    {
        public MappingContract()
        {
            CreateMap<Contract, ContractDto>().ReverseMap();
            CreateMap<Contract, AddContractDto>().ReverseMap();
            CreateMap<LeaveBalance, LeaveBalanceDto>().ReverseMap();
            CreateMap<LeaveBalance, UpdateLeaveBalanceDto>().ReverseMap();
        }
    }
}
