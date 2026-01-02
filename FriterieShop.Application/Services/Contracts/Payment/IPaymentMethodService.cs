namespace FriterieShop.Application.Services.Contracts.Payment
{
    using FriterieShop.Application.DTOs.Payment;

    public interface IPaymentMethodService
    {
        Task<IEnumerable<GetPaymentMethod>> GetPaymentMethodsAsync();
    }
}
