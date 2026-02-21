namespace FriterieShop.Infrastructure.Services
{
    using FriterieShop.Application.DTOs;
    using FriterieShop.Application.DTOs.Payment;
    using FriterieShop.Application.Services.Contracts.Payment;
    using FriterieShop.Domain.Entities;

    using Microsoft.Extensions.Options;

    using Stripe.Checkout;

    public class StripePaymentService : IPaymentService
    {
        private readonly FrontendSettings _frontendSettings;

        public StripePaymentService(IOptions<FrontendSettings> frontendSettings)
        {
            _frontendSettings = frontendSettings.Value;
        }

        public async Task<ServiceResponse> Pay(decimal totalAmount, IEnumerable<Product> cartProducts, IEnumerable<ProcessCart> carts)
        {
            try
            {
                var lineItems = new List<SessionLineItemOptions>();

                foreach (var item in cartProducts)
                {
                    var pQuantity = carts.FirstOrDefault(_ => _.ProductId == item.Id);

                    lineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "eur",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Name,
                                Description = item.Description
                            },
                            UnitAmount = (long)(item.Price * 100),
                        },

                        Quantity = pQuantity!.Quantity,
                    });
                }

                var opt = new SessionCreateOptions
                {
                    PaymentMethodTypes = ["card"],
                    LineItems = lineItems,
                    Mode = "payment",
                    SuccessUrl = $"{_frontendSettings.BaseUrl.TrimEnd('/')}/payment-success",
                    CancelUrl = $"{_frontendSettings.BaseUrl.TrimEnd('/')}/payment-cancel",
                };

                var service = new SessionService();
                var session = await service.CreateAsync(opt);

                return new ServiceResponse(true, session.Url);
            }
            catch (Exception ex)
            {
                return new ServiceResponse(false, ex.Message);
            }
        }
    }
}
