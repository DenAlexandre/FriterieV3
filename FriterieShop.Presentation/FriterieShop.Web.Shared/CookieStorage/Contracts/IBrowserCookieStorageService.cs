namespace FriterieShop.Web.Shared.CookieStorage.Contracts
{
    public interface IBrowserCookieStorageService
    {
        Task SetAsync(string name, string value, int days, string path = "/");

        Task<string?> GetAsync(string name);

        Task RemoveAsync(string name);

        Task SaveToLocalStorageAsync(string key, string value);

        Task<string?> GetFromLocalStorageAsync(string key);

        Task RemoveFromLocalStorageAsync(string key);
    }
}
