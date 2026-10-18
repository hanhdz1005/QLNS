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
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public EmployeeService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            var employees = await _uow.EmployeeRepository
                .Query()
                .Include(e => e.Department)
                .OrderBy(e => e.FullName)
                .ToListAsync();

            return _mapper.Map<IReadOnlyList<EmployeeDto>>(employees);
        }

        public async Task<PagedResult<EmployeeDto>> GetFilterAsync(int pageIndex, int pageSize, string? search)
        {
            var query = _uow.EmployeeRepository
                .Query()
                .Include(e => e.Department)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(e => e.FullName.ToLower().Contains(search) || e.Code.ToLower().Contains(search));
            }

            return await query
                .OrderBy(e => e.FullName)
                .ToPagedResultAsync<Employee, EmployeeDto>(pageIndex, pageSize, _mapper);
        }

        public async Task<EmployeeDto> GetByIdAsync(Guid id)
        {
            var item = await _uow.EmployeeRepository
                .Query()
                .Include(e => e.Department)
                .Include(e => e.Position)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (item == null)
                throw new KeyNotFoundException("Not found employee");

            return _mapper.Map<EmployeeDto>(item);
        }

        public async Task<EmployeeDto> AddAsync(AddEmployeeDto dto)
        {
            var department = await _uow.DepartmentRepository.GetAsync(dto.DepartmentId);
            if (department == null)
                throw new KeyNotFoundException("Department not found");

            var position = await _uow.PositionRepository.GetAsync(dto.PositionId);
            if (position == null)
                throw new KeyNotFoundException("Position not found");

            var entity = _mapper.Map<Employee>(dto);
            entity.DepartmentName = department.DepartmentName;
            entity.PositionName = position.PositionName;
            entity.CreateDate = DateTime.UtcNow;
            entity.Code = StringHelper.GenerateCode(dto.FullName);

            await _uow.EmployeeRepository.AddAsync(entity);
            return _mapper.Map<EmployeeDto>(entity);
        }

        public async Task<EmployeeDto> UpdateAsync(Guid id, AddEmployeeDto dto)
        {
            var exist = await _uow.EmployeeRepository.GetAsync(id);
            if (exist == null)
                throw new KeyNotFoundException("Not found employee");

            var department = await _uow.DepartmentRepository.GetAsync(dto.DepartmentId);
            if (department == null)
                throw new KeyNotFoundException("Department not found");

            var position = await _uow.PositionRepository.GetAsync(dto.PositionId);
            if (position == null)
                throw new KeyNotFoundException("Position not found");

            var entity = _mapper.Map<Employee>(dto);
            entity.DepartmentName = department.DepartmentName;
            entity.PositionName = position.PositionName;
            entity.CreateDate = exist.CreateDate;
            entity.Id = id;
            entity.Code = StringHelper.GenerateCode(dto.FullName);

            await _uow.EmployeeRepository.UpdateAsync(id, entity);
            return _mapper.Map<EmployeeDto>(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var exist = await _uow.EmployeeRepository.GetAsync(id);
            if (exist == null)
                throw new KeyNotFoundException("Not found employee");

            await _uow.EmployeeRepository.DeleteAsync(id);
            return true;
        }

        public async Task<EmployeeDto> GetProfileAsync(string userIdentifier)
        {
            var item = await _uow.EmployeeRepository.GetByIdentifierAsync(userIdentifier);
            if (item == null)
                throw new KeyNotFoundException("Không tìm thấy hồ sơ nhân viên");

            return _mapper.Map<EmployeeDto>(item);
        }
    }
}