using Blog.Application.DTOs.Tag;
using Blog.Application.Interfaces;
using Blog.Application.Mappings;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.Application.Queries.Tag.GetAllTags
{
    public record GetAllTagsQuery : IRequest<List<TagDto>>;

    public class GetAllTagsQueryHandler : IRequestHandler<GetAllTagsQuery, List<TagDto>>
    {
        private readonly ITagRepository _tagRepository;

        public GetAllTagsQueryHandler(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<List<TagDto>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
        {
            var tags = await _tagRepository.GetAllTagsAsync();
            return tags.Select(t => t.toTagDto()).ToList();
        }
    }
}
