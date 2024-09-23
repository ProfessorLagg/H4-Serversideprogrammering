namespace API.Services {
    public interface IPageContentService {
        Task<string?> GetPageContentAsync(params string[] pathParts);
        string GetPageContent(params string[] pathParts);
    }
}
