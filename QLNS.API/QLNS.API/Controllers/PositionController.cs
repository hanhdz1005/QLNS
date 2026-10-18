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
    public class PositionController : ControllerBase
    {
        private readonly IPositionService _service;

        public PositionController(IPositionService service)
        {
            _service = service;
        }

        [Authorize(Roles ="User, Admin, Leader")]
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var items = await _service.GetAllAsync();
            var data = new ListItems<PositionDto>
            {
                Items = items.ToList()
            };
            return Ok(Result<ListItems<PositionDto>>.Success(data));
        }

        [Authorize(Roles ="User, Admin, Leader")]
        [HttpGet("filter")]
        public async Task<ActionResult> GetFilter(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 5,
            [FromQuery] string? search = null)
        {
            var data = await _service.GetFilterAsync(pageIndex, pageSize, search);
            return Ok(Result<PagedResult<PositionDto>>.Success(data));
        }

        [Authorize(Roles ="User, Admin, Leader")]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            try
            {
                var data = await _service.GetByIdAsync(id);
                return Ok(Result<PositionDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Add(AddPositionDto dto)
        {
            try
            {
                var data = await _service.AddAsync(dto);
                return Ok(Result<PositionDto>.Success(data));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(Result<string>.Failure(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, Result<string>.Failure(ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, AddPositionDto dto)
        {
            try
            {
                var data = await _service.UpdateAsync(id, dto);
                return Ok(Result<PositionDto>.Success(data));
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