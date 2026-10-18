using AutoMapper;
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
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [Authorize(Roles ="User, Admin, Leader")]
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var departments = await _departmentService.GetAllAsync();
            var data = new ListItems<DepartmentDto>
            {
                Items = departments.ToList()
            };
            return Ok(Result<ListItems<DepartmentDto>>.Success(data));
        }

        [Authorize(Roles ="User, Admin, Leader")]
        [HttpGet("filter")]
        public async Task<ActionResult> GetFilter(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 5,
            [FromQuery] string? search = null)
        {
            var data = await _departmentService.GetFilterAsync(pageIndex, pageSize, search);
            return Ok(Result<PagedResult<DepartmentDto>>.Success(data));
        }

        [Authorize(Roles ="User, Admin, Leader")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartmentById(Guid id)
        {
            try
            {
                var data = await _departmentService.GetByIdAsync(id);
                return Ok(Result<DepartmentDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddDepartment(AddDepartmentDto department)
        {
            try
            {
                var data = await _departmentService.AddAsync(department);
                return Ok(Result<DepartmentDto>.Success(data));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(Result<bool>.Failure(ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateById(Guid id, AddDepartmentDto department)
        {
            try
            {
                var data = await _departmentService.UpdateAsync(id, department);
                return Ok(Result<DepartmentDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<bool>.Failure(ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteById(Guid id)
        {
            try
            {
                await _departmentService.DeleteAsync(id);
                return Ok(Result<bool>.Success(true));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<bool>.Failure(ex.Message));
            }
        }
    }
}