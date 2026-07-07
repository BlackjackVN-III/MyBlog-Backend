using Blog.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.Application.Commands.Tag.DeleteTag
{
    public record DeleteTagCommand(Guid Id) : IRequest<bool>;

    public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, bool>
    {
        private readonly ITagRepository _tagRepository;
        private readonly IAppDbContext _context;

        public DeleteTagCommandHandler(ITagRepository tagRepository, IAppDbContext context)
        {
            _tagRepository = tagRepository;
            _context = context;
        }

        public async Task<bool> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
        {
            var result = await _tagRepository.DeleteTagAsync(request.Id);
            if (result == null)
            {
                return false;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
