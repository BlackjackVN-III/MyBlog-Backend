using Blog.Application.Commands.User.ChangePassword;
using Blog.Application.Commands.User.UpdateAvatarUrl;
using Blog.Application.Commands.User.UpdateBio;
using Blog.Application.DTOs.User;
using Blog.Application.Interfaces;
using Blog.Application.Queries.User.GetUserProfile;
using MediatR;
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
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IFileService _fileService;

        public ProfileController(ISender sender, IFileService fileService)
        {
            _sender = sender;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var result = await _sender.Send(new GetUserProfileQuery());
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBio([FromBody] UpdateBioRequestDto dto)
        {
            var result = await _sender.Send(new UpdateBioCommand(dto));
            return Ok(result);
        }

        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Không có tệp tin nào được chọn.");
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest("Dung lượng ảnh vượt quá 5MB.");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

            using (var stream = file.OpenReadStream())
            {
                // 1. Tải ảnh trực tiếp lên Cloudinary
                var avatarUrl = await _fileService.SaveFileAsync(stream, file.FileName, allowedExtensions);

                // 2. Cập nhật URL ảnh vừa tạo vào Profile cá nhân
                var updatedProfile = await _sender.Send(new UpdateAvatarUrlCommand(avatarUrl));

                return Ok(updatedProfile);
            }
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto dto)
        {
            var result = await _sender.Send(new ChangePasswordCommand(dto));
            return Ok(new { Success = result, Message = "Đổi mật khẩu thành công." });
        }
    }
}
