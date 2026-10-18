using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QLNS.Core.Common;
using QLNS.Core.Interfaces.IService;
using QLNS.Core.Dto;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Core.Services
{
    public class PositionService : IPositionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PositionService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PositionDto>> GetAllAsync()
        {
            var items = await _uow.PositionRepository
                .Query()
                .OrderBy(p => p.PositionName)
                .ToListAsync();
            return _mapper.Map<IReadOnlyList<PositionDto>>(items);
        }

        public async Task<PagedResult<PositionDto>> GetFilterAsync(int pageIndex, int pageSize, string? search)
        {
            var query = _uow.PositionRepository.Query().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(p => p.PositionName.ToLower().Contains(search));
            }

            return await query
                .OrderBy(p => p.PositionName)
                .ToPagedResultAsync<Position, PositionDto>(pageIndex, pageSize, _mapper);
        }

        public async Task<PositionDto> GetByIdAsync(Guid id)
        {
            var item = await _uow.PositionRepository.GetAsync(id);
            if (item == null)
                throw new KeyNotFoundException("Not found position");
            return _mapper.Map<PositionDto>(item);
        }

        public async Task<PositionDto> AddAsync(AddPositionDto dto)
        {
            var exist = await _uow.PositionRepository
                .CheckAsync(p => p.PositionName.Trim().ToLower() == dto.PositionName.ToLower());
            if (exist)
                throw new InvalidOperationException("Position already exist");

            var entity = _mapper.Map<Position>(dto);
            await _uow.PositionRepository.AddAsync(entity);
            return _mapper.Map<PositionDto>(entity);
        }

        public async Task<PositionDto> UpdateAsync(Guid id, AddPositionDto dto)
        {
            var exist = await _uow.PositionRepository.GetAsync(id);
            if (exist == null)
                throw new KeyNotFoundException("Not found position");

            var entity = _mapper.Map<Position>(dto);
            await _uow.PositionRepository.UpdateAsync(id, entity);
            return _mapper.Map<PositionDto>(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var exist = await _uow.PositionRepository.GetAsync(id);
            if (exist == null)
                throw new KeyNotFoundException("Not found position");
            await _uow.PositionRepository.DeleteAsync(id);
            return true;
        }
    }
}