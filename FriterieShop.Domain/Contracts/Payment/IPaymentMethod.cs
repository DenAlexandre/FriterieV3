namespace FriterieShop.Domain.Contracts.Payment
{
    using FriterieShop.Domain.Entities.Payment;

    public interface IPaymentMethod
    {
        Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync();
    }
}
