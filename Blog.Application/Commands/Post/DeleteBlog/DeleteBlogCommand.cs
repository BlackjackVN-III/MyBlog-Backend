using Blog.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Application.Commands.Post.DeleteBlog
{
    public record DeleteBlogCommand(Guid id) : IRequest;
    public class DeleteBlogCommandHandler : IRequest<DeleteBlogCommand>
    {
        private readonly IAppDbContext _context;
        private readonly IPostRepository _postRepository;

        public DeleteBlogCommandHandler(IAppDbContext context, IPostRepository postRepository)
        {
            _context = context;
            _postRepository = postRepository;
        }
        public async Task Handle(DeleteBlogCommand request, CancellationToken cancellationToken)
        {

            await _postRepository.DeleteBlogPostAsync(request.id);
            await _context.SaveChangesAsync(cancellationToken);
        }



    }
}

