using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLNS.Core.Dto;
using QLNS.Core.Interfaces.IService;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLNS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        [Authorize(Roles = "Admin, Leader")]
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var items = await _contractService.GetAllAsync();
            var data = new ListItems<ContractDto>
            {
                Items = items.ToList()
            };
            return Ok(Result<ListItems<ContractDto>>.Success(data));
        }

        [Authorize(Roles = "Admin, Leader")]
        [HttpGet("filter")]
        public async Task<ActionResult> GetFilter(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var data = await _contractService.GetFilterAsync(pageIndex, pageSize, search);
            return Ok(Result<PagedResult<ContractDto>>.Success(data));
        }

        [Authorize(Roles = "Admin, Leader")]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            try
            {
                var data = await _contractService.GetByIdAsync(id);
                return Ok(Result<ContractDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
        }

        [Authorize(Roles = "User, Admin, Leader")]
        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult> GetByEmployeeId(Guid employeeId)
        {
            var items = await _contractService.GetByEmployeeIdAsync(employeeId);
            return Ok(Result<IEnumerable<ContractDto>>.Success(items));
        }

        [Authorize(Roles = "Admin, Leader")]
        [HttpGet("expiring")]
        public async Task<ActionResult> GetExpiring([FromQuery] int days = 30)
        {
            var items = await _contractService.GetExpiringContractsAsync(days);
            return Ok(Result<IEnumerable<ContractDto>>.Success(items));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] AddContractDto dto)
        {
            try
            {
                var data = await _contractService.AddAsync(dto);
                return Ok(Result<ContractDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, Result<string>.Failure(ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] AddContractDto dto)
        {
            try
            {
                var data = await _contractService.UpdateAsync(id, dto);
                return Ok(Result<ContractDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, Result<string>.Failure(ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _contractService.DeleteAsync(id);
                return Ok(Result<bool>.Success(true));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<bool>.Failure(ex.Message));
            }
        }
    }
}
