using Blog.Application.Commands.Tag.CreateTag;
using Blog.Application.Commands.Tag.DeleteTag;
using Blog.Application.Commands.Tag.UpdateTag;
using Blog.Application.DTOs.Tag;
using Blog.Application.Queries.Tag.GetAllTags;
using Blog.Application.Queries.Tag.GetTagById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Blog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("general-policy")]
    public class TagsController : ControllerBase
    {
        private readonly ISender _sender;

        public TagsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _sender.Send(new GetAllTagsQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _sender.Send(new GetTagByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("interactive-policy")]
        public async Task<IActionResult> Create([FromBody] CreateTagRequestDto dto)
        {
            var result = await _sender.Send(new CreateTagCommand(dto));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("interactive-policy")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] CreateTagRequestDto dto)
        {
            var result = await _sender.Send(new UpdateTagCommand(id, dto));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("interactive-policy")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var success = await _sender.Send(new DeleteTagCommand(id));
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
