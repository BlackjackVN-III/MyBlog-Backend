using Blog.Application.Commands.Post.CreateBlog;
using Blog.Application.Commands.Post.DeleteBlog;
using Blog.Application.Commands.Post.UpdateBlog;
using Blog.Application.DTOs.Blog;
using Blog.Application.Queries;
using Blog.Application.Queries.GetBlog;
using Blog.Application.Queries.GetBlogById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Blog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("general-policy")]
    public class BlogsController : ControllerBase
    {
        public readonly ISender _sender;
        public BlogsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            var result = await _sender.Send(new GetAllBlogsQuery(query));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _sender.Send(new GetBlogByIdQuery(id));
            return Ok(result);
        }

        [Authorize] 
        [HttpPost]
        [EnableRateLimiting("interactive-policy")]
        public async Task<IActionResult> Create([FromBody] CreateBlogRequestDto dto)
        {

            var blogId  = await _sender.Send(new CreateBlogCommand(dto));
            return CreatedAtAction(nameof(GetById), new { id = blogId  }, blogId );
        }

        [Authorize] 
        [HttpPut("{id}")]
        [EnableRateLimiting("interactive-policy")]
        public async Task<IActionResult> UpdateBlog([FromBody] UpdateBlogRequestDto updateDto, [FromRoute] Guid id)
        {
            var result = await _sender.Send(new UpdateBlogCommand(updateDto, id));
            return Ok(result);
        }
        [Authorize] 
        [HttpDelete("{id}")]
        [EnableRateLimiting("interactive-policy")]
        public async Task<IActionResult> DeleteBlog([FromRoute] Guid id)
        {
            await _sender.Send(new DeleteBlogCommand(id));
            return NoContent();
        }
        
    }
}
