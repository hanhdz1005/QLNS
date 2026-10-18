using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QLNS.Core.Common;
using QLNS.Core.Dto;
using QLNS.Core.Interfaces.IService;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLNS.Core.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public DepartmentService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var departments = await _uow.DepartmentRepository
                .Query()
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();

            return _mapper.Map<IReadOnlyList<DepartmentDto>>(departments);
        }

        public async Task<PagedResult<DepartmentDto>> GetFilterAsync(int pageIndex, int pageSize, string? search)
        {
            var query = _uow.DepartmentRepository.Query();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(d => d.DepartmentName.ToLower().Contains(search));
            }

            return await query
                .OrderBy(d => d.DepartmentName)
                .ToPagedResultAsync<Department, DepartmentDto>(pageIndex, pageSize, _mapper);
        }

        public async Task<DepartmentDto> GetByIdAsync(Guid id)
        {
            var department = await _uow.DepartmentRepository.GetAsync(id);
            if (department == null)
                throw new KeyNotFoundException("Not found department");

            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task<DepartmentDto> AddAsync(AddDepartmentDto department)
        {
            var exist = await _uow.DepartmentRepository
                .CheckAsync(d => d.DepartmentName.Trim().ToLower() == department.DepartmentName.ToLower());
            if (exist)
                throw new InvalidOperationException("Department already exist");

            var entity = _mapper.Map<Department>(department);
            entity.Code = StringHelper.GenerateCode(department.DepartmentName);
            entity.CreateDate = DateTime.UtcNow;

            await _uow.DepartmentRepository.AddAsync(entity);

            return new DepartmentDto
            {
                Id = entity.Id,
                Code = entity.Code,
                DepartmentName = entity.DepartmentName,
                Description = entity.Description,
                CreateDate = entity.CreateDate
            };
        }

        public async Task<DepartmentDto> UpdateAsync(Guid id, AddDepartmentDto department)
        {
            var exist = await _uow.DepartmentRepository.GetAsync(id);
            if (exist == null)
                throw new KeyNotFoundException("Not found department");

            var entity = _mapper.Map<Department>(department);
            entity.Id = id;
            entity.Code = StringHelper.GenerateCode(department.DepartmentName);

            await _uow.DepartmentRepository.UpdateAsync(id, entity);

            return new DepartmentDto
            {
                Id = entity.Id,
                Code = entity.Code,
                DepartmentName = entity.DepartmentName,
                Description = entity.Description
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var exist = await _uow.DepartmentRepository.GetAsync(id);
            if (exist == null)
                throw new KeyNotFoundException("Not found department");

            await _uow.DepartmentRepository.DeleteAsync(id);
            return true;
        }
    }
}