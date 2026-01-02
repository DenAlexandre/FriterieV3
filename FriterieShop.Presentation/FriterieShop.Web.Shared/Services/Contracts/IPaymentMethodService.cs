namespace FriterieShop.Web.Shared.Services.Contracts
{
    using FriterieShop.Web.Shared.Models.Payment;

    public interface IPaymentMethodService
    {
        Task<IEnumerable<GetPaymentMethod>> GetPaymentMethods();
    }
}
