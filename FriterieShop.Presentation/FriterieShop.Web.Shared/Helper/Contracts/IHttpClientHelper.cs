namespace FriterieShop.Web.Shared.Helper.Contracts
{
    public interface IHttpClientHelper
    {
        HttpClient GetPublicClient();

        Task<HttpClient> GetPrivateClientAsync();
    }
}
