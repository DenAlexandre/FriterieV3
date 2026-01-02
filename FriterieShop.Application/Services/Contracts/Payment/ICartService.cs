namespace FriterieShop.Application.Services.Contracts.Payment
{
    using FriterieShop.Application.DTOs;
    using FriterieShop.Application.DTOs.Payment;

    public interface ICartService
    {
        Task<ServiceResponse> SaveCheckoutHistoryAsync(IEnumerable<CreateOrderItem> orderItems);

        Task<ServiceResponse> CheckoutAsync(Checkout checkout);

        Task<ServiceResponse> CheckoutAsync(Checkout checkout, string? userId);

        Task<IEnumerable<GetOrderItem>> GetOrderItemsAsync();

        Task<IEnumerable<GetOrderItem>> GetCheckoutHistoryByUserId(string userId);
    }
}
