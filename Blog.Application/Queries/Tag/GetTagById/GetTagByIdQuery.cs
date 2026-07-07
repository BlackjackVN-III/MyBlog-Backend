using Blog.Application.DTOs.Tag;
using Blog.Application.Interfaces;
using Blog.Application.Mappings;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.Application.Queries.Tag.GetTagById
{
    public record GetTagByIdQuery(Guid Id) : IRequest<TagDto>;

    public class GetTagByIdQueryHandler : IRequestHandler<GetTagByIdQuery, TagDto>
    {
        private readonly ITagRepository _tagRepository;

        public GetTagByIdQueryHandler(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<TagDto> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
        {
            var tag = await _tagRepository.GetTagByIdAsync(request.Id);
            if (tag == null)
            {
                throw new Exception("Không tìm thấy thẻ.");
            }

            return tag.toTagDto();
        }
    }
}
