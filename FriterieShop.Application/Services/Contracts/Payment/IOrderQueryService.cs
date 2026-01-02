namespace FriterieShop.Application.Services.Contracts.Payment
{
    using FriterieShop.Application.DTOs.Payment;

    public interface IOrderQueryService
    {
        Task<IEnumerable<GetOrder>> GetOrdersForUserAsync(string userId);

        Task<IEnumerable<GetOrder>> GetAllAsync();
    }
}
