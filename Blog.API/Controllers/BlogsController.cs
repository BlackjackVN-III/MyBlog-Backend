using Blog.Application.DTOs.Blog;
using Blog.Application.Interfaces;
using Blog.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Blog.Application.Commands.Post.CreateBlog;
using Blog.Application.Queries.GetBlog;

namespace Blog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        public readonly IPostRepository _postRepository;
        public readonly ISender _sender;
        public BlogsController(IPostRepository postRepository, ISender sender)
        {
             _postRepository = postRepository;
             _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _sender.Send(new GetAllBlogsQuery());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBlogRequestDto dto,[FromRoute] Guid id)
        {
            
            var result = await _sender.Send(new CreateBlogCommand(dto));
            return Ok(result);
        }

}
}
