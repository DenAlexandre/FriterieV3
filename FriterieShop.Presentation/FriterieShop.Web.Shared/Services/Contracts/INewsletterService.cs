namespace FriterieShop.Web.Shared.Services.Contracts
{
    using FriterieShop.Web.Shared.Models;
    using FriterieShop.Web.Shared.Models.Newsletter;

    public interface INewsletterService
    {
        Task<ServiceResponse> SubscribeAsync(SubscribeRequest request);
    }
}
