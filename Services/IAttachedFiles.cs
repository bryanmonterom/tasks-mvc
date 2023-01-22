using TasksMVC.Models;

namespace TasksMVC.Services
{
    public interface IAttachedFiles
    {
        Task Delete(string path, string container);
        Task<FileResults[]> Save(string path, IEnumerable<IFormFile> files);

    }
}
