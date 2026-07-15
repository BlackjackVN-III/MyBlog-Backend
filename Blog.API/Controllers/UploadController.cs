using Blog.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Blog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IFileService _fileService;

        public UploadController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("image")]
        [Authorize] // Chỉ cho phép thành viên đã đăng nhập upload ảnh
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Không có tệp tin nào được gửi lên.");
            }

            // Giới hạn dung lượng tối đa 5MB
            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest("Dung lượng ảnh vượt quá giới hạn cho phép (tối đa 5MB).");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var imageUrl = await _fileService.SaveFileAsync(stream, file.FileName, allowedExtensions);
                    return Ok(new { Url = imageUrl });
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }
    }
}
