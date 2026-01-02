namespace FriterieShop.Application.Services.Contracts.Payment
{
    using FriterieShop.Application.DTOs;
    using FriterieShop.Application.DTOs.Payment;
    using FriterieShop.Domain.Entities;

    public interface IPayPalPaymentService
    {
        Task<ServiceResponse> Pay(decimal totalAmount, IEnumerable<Product> cartProducts, IEnumerable<ProcessCart> carts);

        Task<bool> CaptureAsync(string orderId);
    }
}
