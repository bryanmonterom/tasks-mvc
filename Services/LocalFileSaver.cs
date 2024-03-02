using TasksMVC.Models;

namespace TasksMVC.Services
{
    public class LocalFileSaver : IAttachedFiles
    {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;

        public LocalFileSaver(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }
        public Task Delete(string path, string container)
        {
            if (string.IsNullOrWhiteSpace(path)) { 
            
                return Task.CompletedTask;
            }

            var fileName = Path.GetFileName(path);
            var directory = Path.Combine(env.WebRootPath, container, fileName);

            if (File.Exists(directory)) { 
                File.Delete(directory);
            }

            return Task.CompletedTask;
        }

        public async Task<FileResults[]> Save(string container, IEnumerable<IFormFile> files)
        {
            var tasks = files.Select(async file =>
            {
                var fileNameOriginal = Path.GetFileName(file.FileName);
                var extension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";
                string folder = Path.Combine(env.WebRootPath, container);

                if (!Directory.Exists(folder)) { 
                
                    Directory.CreateDirectory(folder);
                }

                string path = Path.Combine(folder, fileName);
                using (var ms = new MemoryStream()) { 
                
                    await file.CopyToAsync(ms);
                    var content = ms.ToArray();
                    await File.WriteAllBytesAsync(path, content);
                }

                var url = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
                var urlFile = Path.Combine(url, container, fileName).Replace("\\","/");
                return new FileResults() {Title= fileNameOriginal, URL = urlFile };
            });

            var results = await Task.WhenAll(tasks);
            return results;

        }
    }
}
