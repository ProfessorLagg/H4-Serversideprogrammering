namespace API.Services {
    public class PageContentService {
        private readonly IWebHostEnvironment _environment;
        public PageContentService(IWebHostEnvironment env) {
            this._environment = env;
        }
    }
}
