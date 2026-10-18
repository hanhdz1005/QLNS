using QLNS.Core.Common;
using QLNS.Core.Dto;
using QLNS.Core.Interfaces;
using QLNS.Core.Interfaces.IService;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLNS.Core.Services
{
    public class BonusPenaltyService : IBonusPenaltyService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public BonusPenaltyService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BonusPenaltyDto>> GetAllAsync()
        {
            var items = await _uow.BonusPenaltyRepository.GetAllAsync();
            return _mapper.Map<IReadOnlyList<BonusPenaltyDto>>(items);
        }

        public async Task<PagedResult<BonusPenaltyDto>> GetFilterAsync(int pageIndex, int pageSize, string? search, int? type)
        {
            var query = _uow.BonusPenaltyRepository.Query();

            if (type.HasValue)
            {
                var targetType = (BonusPenaltyType)type.Value;
                query = query.Where(bp => bp.Type == targetType);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(bp => bp.EmployeeName.ToLower().Contains(search) || bp.Reason.ToLower().Contains(search));
            }

            return await query
                .OrderByDescending(bp => bp.Date)
                .ToPagedResultAsync<BonusPenalty, BonusPenaltyDto>(pageIndex, pageSize, _mapper);
        }

        public async Task<BonusPenaltyDto> GetByIdAsync(Guid id)
        {
            var item = await _uow.BonusPenaltyRepository.GetByIdAsync(id);
            if (item == null)
                throw new KeyNotFoundException("Not found bonus/penalty record");
            return _mapper.Map<BonusPenaltyDto>(item);
        }

        public async Task<BonusPenaltyDto> AddAsync(AddBonusPenaltyDto dto)
        {
            var employee = await _uow.EmployeeRepository.GetAsync(dto.EmployeeId);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            var entity = _mapper.Map<BonusPenalty>(dto);
            entity.EmployeeName = employee.FullName;
            await _uow.BonusPenaltyRepository.AddAsync(entity);
            return _mapper.Map<BonusPenaltyDto>(entity);
        }

        public async Task<BonusPenaltyDto> UpdateAsync(Guid id, AddBonusPenaltyDto dto)
        {
            var exist = await _uow.BonusPenaltyRepository.GetAsync(id);
            if (exist == null)
                throw new KeyNotFoundException("Not found bonus/penalty record");

            var employee = await _uow.EmployeeRepository.GetAsync(dto.EmployeeId);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            var entity = _mapper.Map<BonusPenalty>(dto);
            entity.EmployeeName = employee.FullName;
            entity.Id = id;
            await _uow.BonusPenaltyRepository.UpdateAsync(id, entity);
            return _mapper.Map<BonusPenaltyDto>(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var exist = await _uow.BonusPenaltyRepository.GetAsync(id);
            if (exist == null)
                throw new KeyNotFoundException("Not found bonus/penalty record");
            await _uow.BonusPenaltyRepository.DeleteAsync(id);
            return true;
        }
    }
}
