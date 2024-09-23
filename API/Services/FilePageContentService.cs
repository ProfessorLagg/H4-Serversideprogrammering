using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

namespace API.Services {
    public class FilePageContentService : IPageContentService {
        const string PagesSubPath = "Pages";
        private readonly IWebHostEnvironment _environment;
        private IFileProvider FileProvider { get { return _environment.ContentRootFileProvider; } }
        public FilePageContentService(IWebHostEnvironment env) {
            this._environment = env;

        }
        public async Task<string?> GetPageContentAsync(params string[] pathParts) {
            if (pathParts.IsNullOrEmpty()) { return null; }

            string[] parts = new string[pathParts.Length + 1];
            parts[0] = PagesSubPath;
            pathParts.CopyTo(parts, 1);
            string path = Path.Join(parts);
            if (!path.EndsWith(".html")) { path += ".html"; }
            IFileInfo pageFile = FileProvider.GetFileInfo(path);
            if (!pageFile.Exists || pageFile.IsDirectory) { return null; }
            using (Stream readStream = pageFile.CreateReadStream()) {
                using (StreamReader streamReader = new StreamReader(readStream)) {
                    return await streamReader.ReadToEndAsync();
                }
            }
        }
        public string? GetPageContent(params string[] pathParts) {
            Task<string?> promise = this.GetPageContentAsync(pathParts);
            promise.Wait();
            return promise.Result;
        }
    }
}
