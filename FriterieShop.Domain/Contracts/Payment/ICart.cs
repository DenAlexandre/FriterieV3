namespace FriterieShop.Domain.Contracts.Payment
{
    using FriterieShop.Domain.Entities.Payment;

    public interface ICart
    {
        Task<int> SaveCheckoutHistory(IEnumerable<OrderItem> checkouts);

        Task<IEnumerable<OrderItem>> GetAllCheckoutHistory();

        Task<IEnumerable<OrderItem>> GetCheckoutHistoryByUserId(string userId);
    }
}
