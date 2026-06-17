using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Application.Interfaces
{
    public interface IAppDbContext
    {

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
