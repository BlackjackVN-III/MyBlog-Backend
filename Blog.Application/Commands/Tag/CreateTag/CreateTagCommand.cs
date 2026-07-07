using Blog.Application.DTOs.Tag;
using Blog.Application.Interfaces;
using Blog.Application.Mappings;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.Application.Commands.Tag.CreateTag
{
    public record CreateTagCommand(CreateTagRequestDto Dto) : IRequest<TagDto>;

    public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, TagDto>
    {
        private readonly ITagRepository _tagRepository;
        private readonly IAppDbContext _context;

        public CreateTagCommandHandler(ITagRepository tagRepository, IAppDbContext context)
        {
            _tagRepository = tagRepository;
            _context = context;
        }

        public async Task<TagDto> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            var tag = request.Dto.toTagFromCreateDto();
            tag.Id = Guid.NewGuid();

            await _tagRepository.CreateTagAsync(tag);
            await _context.SaveChangesAsync(cancellationToken);

            return tag.toTagDto();
        }
    }
}
