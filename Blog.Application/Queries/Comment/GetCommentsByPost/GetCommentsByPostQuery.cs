using Blog.Application.DTOs.Comment;
using Blog.Application.Interfaces;
using Blog.Application.Mappings;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.Application.Queries.Comment.GetCommentsByPost
{
    public record GetCommentsByPostQuery(Guid PostId) : IRequest<List<CommentDto>>;

    public class GetCommentsByPostQueryHandler : IRequestHandler<GetCommentsByPostQuery, List<CommentDto>>
    {
        private readonly ICommentRepository _commentRepository;

        public GetCommentsByPostQueryHandler(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<List<CommentDto>> Handle(GetCommentsByPostQuery request, CancellationToken cancellationToken)
        {
            var comments = await _commentRepository.GetCommentsByPostIdAsync(request.PostId);
            return comments.Select(c => c.toCommentDto()).ToList();
        }
    }
}
