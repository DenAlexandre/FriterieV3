namespace FriterieShop.Application.Services.Contracts
{
    using FriterieShop.Application.DTOs;

    public interface INewsletterService
    {
        Task<ServiceResponse> SubscribeAsync(string email);
    }
}
