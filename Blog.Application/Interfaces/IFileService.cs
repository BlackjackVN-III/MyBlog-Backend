using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Application.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(Stream fileStream, string fileName, string[] allowedExtensions);
        Task DeleteFileAsync(string fileUrl);
    }
}
