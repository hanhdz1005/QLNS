using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLNS.API.Extension;
using QLNS.Core.Common;
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
    public class LeaveRequestController : ControllerBase
    {
        private readonly ILeaveRequestService _service;

        public LeaveRequestController(ILeaveRequestService service)
        {
            _service = service;
        }

        [Authorize(Roles = RoleConstants.AdminOrAccountant)]
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var items = await _service.GetAllAsync();
            var data = new ListItems<LeaveRequestDto>
            {
                Items = items.ToList()
            };
            return Ok(Result<ListItems<LeaveRequestDto>>.Success(data));
        }

        [Authorize(Roles = RoleConstants.AllRoles)]
        [HttpGet("filter")]
        public async Task<ActionResult> GetFilter(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 5,
            [FromQuery] string? search = null,
            [FromQuery] bool myOnly = false)
        {
            var isEmployeeOnly = User.IsInRole(RoleConstants.Employee)
                && !User.IsInRole(RoleConstants.Admin)
                && !User.IsInRole(RoleConstants.Accountant);

            var filterIdentifier = (myOnly || isEmployeeOnly) ? User.GetUserIdentifier() : null;
            var data = await _service.GetFilterAsync(pageIndex, pageSize, search, filterIdentifier);
            return Ok(Result<PagedResult<LeaveRequestDto>>.Success(data));
        }

        [Authorize(Roles = RoleConstants.AllRoles)]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            try
            {
                var data = await _service.GetByIdAsync(id);
                return Ok(Result<LeaveRequestDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.AllRoles)]
        public async Task<ActionResult> Add(AddLeaveRequestDto dto)
        {
            try
            {
                var data = await _service.AddAsync(dto);
                return Ok(Result<LeaveRequestDto>.Success(data));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(Result<string>.Failure(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { message = innerMessage, fullError = ex.ToString() });
            }
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, AddLeaveRequestDto dto)
        {
            try
            {
                var data = await _service.UpdateAsync(id, dto);
                return Ok(Result<LeaveRequestDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok(Result<bool>.Success(true));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
        }
    }
}
