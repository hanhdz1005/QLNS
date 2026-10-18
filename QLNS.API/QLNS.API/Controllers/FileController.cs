using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using QLNS.Core.Dto;
using QLNSCore.Entities;
using System;
using System.IO;
using System.Threading.Tasks;

namespace QLNS.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        public FileController()
        {
            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);
        }

        [HttpPost("upload")]
        public async Task<ActionResult> Upload(IFormFile file, [FromQuery] string? folder = null)
        {
            if (file == null || file.Length == 0)
                return BadRequest(Result<bool>.Failure("No file provided"));

            var targetFolder = string.IsNullOrWhiteSpace(folder)
                ? _uploadPath
                : Path.Combine(_uploadPath, folder);

            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(targetFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var relativePath = string.IsNullOrWhiteSpace(folder)
                ? $"/uploads/{fileName}"
                : $"/uploads/{folder}/{fileName}";

            return Ok(Result<FileUploadResult>.Success(new FileUploadResult
            {
                FileName = fileName,
                OriginalName = file.FileName,
                Size = file.Length,
                Url = relativePath
            }));
        }

        [HttpPost("upload-multiple")]
        public async Task<ActionResult> UploadMultiple([FromForm] IFormFileCollection files, [FromQuery] string? folder = null)
        {
            if (files == null || files.Count == 0)
                return BadRequest(Result<bool>.Failure("No files provided"));

            var targetFolder = string.IsNullOrWhiteSpace(folder)
                ? _uploadPath
                : Path.Combine(_uploadPath, folder);

            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            var results = new System.Collections.Generic.List<FileUploadResult>();

            foreach (var file in files)
            {
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(targetFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                results.Add(new FileUploadResult
                {
                    FileName = fileName,
                    OriginalName = file.FileName,
                    Size = file.Length,
                    Url = $"/uploads/{(string.IsNullOrWhiteSpace(folder) ? "" : folder + "/")}{fileName}"
                });
            }

            return Ok(Result<ListItems<FileUploadResult>>.Success(new ListItems<FileUploadResult> { Items = results }));
        }

        [HttpGet("download/{fileName}")]
        public ActionResult Download(string fileName)
        {
            var filePath = Path.Combine(_uploadPath, fileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound(Result<string>.Failure("File not found"));

            var provider = new FileExtensionContentTypeProvider();
            provider.TryGetContentType(filePath, out var contentType);
            contentType ??= "application/octet-stream";

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, contentType, fileName);
        }

        [HttpDelete("{fileName}")]
        public ActionResult Delete(string fileName)
        {
            var filePath = Path.Combine(_uploadPath, fileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound(Result<string>.Failure("File not found"));

            System.IO.File.Delete(filePath);
            return Ok(Result<bool>.Success(true));
        }
    }

    
}
