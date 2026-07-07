using Blog.Application.DTOs.Tag;
using Blog.Application.Interfaces;
using Blog.Application.Mappings;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.Application.Commands.Tag.UpdateTag
{
    public record UpdateTagCommand(Guid Id, CreateTagRequestDto Dto) : IRequest<TagDto>;

    public class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand, TagDto>
    {
        private readonly ITagRepository _tagRepository;
        private readonly IAppDbContext _context;

        public UpdateTagCommandHandler(ITagRepository tagRepository, IAppDbContext context)
        {
            _tagRepository = tagRepository;
            _context = context;
        }

        public async Task<TagDto> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
        {
            var tag = request.Dto.toTagFromCreateDto();

            var result = await _tagRepository.UpdateTagAsync(tag, request.Id);
            if (result == null)
            {
                throw new Exception("Không tìm thấy thẻ cần cập nhật.");
            }

            await _context.SaveChangesAsync(cancellationToken);

            return result.toTagDto();
        }
    }
}
