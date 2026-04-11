namespace MinimalAPIsMovieNew.Services
{
    public class LocalFileStorage(IWebHostEnvironment env,
        IHttpContextAccessor httpContextAccessor,IConfiguration configuration) : IFileStorage
    {
        Task IFileStorage.Delete(string? route, string container)
        {
            if (string.IsNullOrWhiteSpace(route))
            {
                return Task.CompletedTask;
            }
            var baseAppImagesFolder = configuration.GetValue<string>("directoriimages");
            if (baseAppImagesFolder is not null)
            {
                var fileName = Path.GetFileName(route);
                var fileDirectory = Path.Combine(baseAppImagesFolder, container, fileName);

                if (File.Exists(fileDirectory))
                {
                    File.Delete(fileDirectory);
                }
                
            }

            return Task.CompletedTask;

        }

        async Task<string> IFileStorage.Store(string container, IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var baseAppImagesFolder = configuration.GetValue<string>("directoriimages");
            string folder = Path.Combine(baseAppImagesFolder!, container);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string route = Path.Combine(folder, fileName);

            // save file
            using (var stream = new FileStream(route, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var content = ms.ToArray();
                await File.WriteAllBytesAsync(route, content);
            }

            var schema = httpContextAccessor.HttpContext!.Request.Scheme;
            var host = httpContextAccessor.HttpContext!.Request.Host;
            var url = $"{schema}://{host}";
            var urlFile = Path.Combine(url, container, fileName).Replace("\\", "/");
            return urlFile;
        }
    }
}
