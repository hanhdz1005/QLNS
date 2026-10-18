using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    public class ContractService : IContractService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ContractService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ContractDto>> GetAllAsync()
        {
            var items = await _uow.ContractRepository
                .Query()
                .Include(c => c.Employee)
                .OrderByDescending(c => c.StartDate)
                .ToListAsync();
            return _mapper.Map<IReadOnlyList<ContractDto>>(items);
        }

        public async Task<PagedResult<ContractDto>> GetFilterAsync(int pageIndex, int pageSize, string? search)
        {
            var query = _uow.ContractRepository
                .Query()
                .Include(c => c.Employee)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(c => c.ContractNumber.ToLower().Contains(search) ||
                                         c.EmployeeName.ToLower().Contains(search));
            }

            return await query
                .OrderByDescending(c => c.StartDate)
                .ToPagedResultAsync<Contract, ContractDto>(pageIndex, pageSize, _mapper);
        }

        public async Task<ContractDto> GetByIdAsync(Guid id)
        {
            var item = await _uow.ContractRepository
                .Query()
                .Include(c => c.Employee)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (item == null)
                throw new KeyNotFoundException("Contract not found");

            return _mapper.Map<ContractDto>(item);
        }

        public async Task<IEnumerable<ContractDto>> GetByEmployeeIdAsync(Guid employeeId)
        {
            var items = await _uow.ContractRepository.GetByEmployeeIdAsync(employeeId);
            return _mapper.Map<IReadOnlyList<ContractDto>>(items);
        }

        public async Task<IEnumerable<ContractDto>> GetExpiringContractsAsync(int withinDays)
        {
            var items = await _uow.ContractRepository.GetExpiringContractsAsync(withinDays);
            return _mapper.Map<IReadOnlyList<ContractDto>>(items);
        }

        public async Task<ContractDto> AddAsync(AddContractDto dto)
        {
            var employee = await _uow.EmployeeRepository.GetAsync(dto.EmployeeId);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            var entity = _mapper.Map<Contract>(dto);
            entity.EmployeeName = employee.FullName;
            await _uow.ContractRepository.AddAsync(entity);
            return _mapper.Map<ContractDto>(entity);
        }

        public async Task<ContractDto> UpdateAsync(Guid id, AddContractDto dto)
        {
            var exist = await _uow.ContractRepository.GetAsync(id);
            if (exist == null)
                throw new KeyNotFoundException("Contract not found");

            var employee = await _uow.EmployeeRepository.GetAsync(dto.EmployeeId);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            var entity = _mapper.Map<Contract>(dto);
            entity.Id = id;
            entity.EmployeeName = employee.FullName;
            await _uow.ContractRepository.UpdateAsync(id, entity);
            return _mapper.Map<ContractDto>(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var exist = await _uow.ContractRepository.GetAsync(id);
            if (exist == null)
                throw new KeyNotFoundException("Contract not found");

            await _uow.ContractRepository.DeleteAsync(id);
            return true;
        }
    }
}
