using Blog.Application.Interfaces;
using Blog.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        public readonly IPostRepository _postRepository;
        public BlogsController(IPostRepository postRepository)
        {
             _postRepository = postRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<BlogPost>> GetAll()
        {
            return await _postRepository.GetAllPostsAsync();
        }
}
}
