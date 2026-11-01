using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLNS.Core.Common;
using QLNS.Core.Dto;
using QLNS.Core.Interfaces.IService;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLNS.API.Controllers
{
    [Authorize(Roles = RoleConstants.AdminOrAccountant)]
    [Route("api/[controller]")]
    [ApiController]
    public class BonusPenaltyController : ControllerBase
    {
        private readonly IBonusPenaltyService _service;

        public BonusPenaltyController(IBonusPenaltyService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var items = await _service.GetAllAsync();
            var data = new ListItems<BonusPenaltyDto>
            {
                Items = (IReadOnlyList<BonusPenaltyDto>)items
            };
            return Ok(Result<ListItems<BonusPenaltyDto>>.Success(data));
        }

        [HttpGet("filter")]
        public async Task<ActionResult> GetFilter(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 5,
            [FromQuery] string? search = null,
            [FromQuery] int? type = null)
        {
            var data = await _service.GetFilterAsync(pageIndex, pageSize, search, type);
            return Ok(Result<PagedResult<BonusPenaltyDto>>.Success(data));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            try
            {
                var data = await _service.GetByIdAsync(id);
                return Ok(Result<BonusPenaltyDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Add(AddBonusPenaltyDto dto)
        {
            try
            {
                var data = await _service.AddAsync(dto);
                return Ok(Result<BonusPenaltyDto>.Success(data));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(Result<bool>.Failure(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, AddBonusPenaltyDto dto)
        {
            try
            {
                var data = await _service.UpdateAsync(id, dto);
                return Ok(Result<BonusPenaltyDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<bool>.Failure(ex.Message));
            }
        }

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
                return NotFound(Result<bool>.Failure(ex.Message));
            }
        }
    }
}
