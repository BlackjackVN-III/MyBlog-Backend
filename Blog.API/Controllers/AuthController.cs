using Blog.Application.Commands.Auth.Login;
using Blog.Application.Commands.Auth.Register;
using Blog.Application.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;

        public AuthController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            var result = await _sender.Send(new RegisterCommand(dto));
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var result = await _sender.Send(new LoginCommand(dto));
            return Ok(result);
        }
    }
}
