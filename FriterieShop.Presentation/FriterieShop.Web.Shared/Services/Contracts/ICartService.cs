namespace FriterieShop.Web.Shared.Services.Contracts
{
    using FriterieShop.Web.Shared.Models;
    using FriterieShop.Web.Shared.Models.Payment;

    public interface ICartService
    {
        Task<ServiceResponse> Checkout(Checkout checkout);

        Task<ServiceResponse> SaveCheckoutHistory(IEnumerable<CreateOrderItem> orderItems);

        Task<IEnumerable<GetOrderItem>> GetOrderItemsAsync();

        Task<IEnumerable<GetOrderItem>> GetCheckoutHistoryByUserId();
    }
}
