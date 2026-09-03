using Blog.Application.Commands.Comment.CreateComment;
using Blog.Application.Commands.Comment.DeleteComment;
using Blog.Application.DTOs.Comment;
using Blog.Application.Queries.Comment.GetCommentsByPost;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Threading.Tasks;

namespace Blog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("interactive-policy")]
    public class CommentsController : ControllerBase
    {
        private readonly ISender _sender;

        public CommentsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetByPost([FromRoute] Guid postId)
        {
            var result = await _sender.Send(new GetCommentsByPostQuery(postId));
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateCommentRequestDto dto)
        {
            var result = await _sender.Send(new CreateCommentCommand(dto));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var success = await _sender.Send(new DeleteCommentCommand(id));
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
