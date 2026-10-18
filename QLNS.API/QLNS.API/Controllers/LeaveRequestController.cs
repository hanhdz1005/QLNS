using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLNS.Core.Interfaces.IService;
using QLNS.Core.Dto;
using QLNSCore.Entities;
using Microsoft.AspNetCore.Authorization;

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

        [Authorize(Roles ="User, Admin, Leader")]
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

        [Authorize(Roles ="User, Admin, Leader")]
        [HttpGet("filter")]
        public async Task<ActionResult> GetFilter(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 5,
            [FromQuery] string? search = null)
        {
            var data = await _service.GetFilterAsync(pageIndex, pageSize, search);
            return Ok(Result<PagedResult<LeaveRequestDto>>.Success(data));
        }

        [Authorize(Roles ="User, Admin, Leader")]
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
        [Authorize(Roles ="User, Admin, Leader")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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