namespace FriterieShop.Application.Services.Payment
{
    using AutoMapper;

    using FriterieShop.Application.DTOs.Payment;
    using FriterieShop.Application.Services.Contracts.Payment;
    using FriterieShop.Domain.Contracts.Payment;

    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IPaymentMethod _paymentMethod;
        private readonly IMapper _mapper;

        public PaymentMethodService(IPaymentMethod paymentMethod, IMapper mapper)
        {
            this._paymentMethod = paymentMethod;
            this._mapper = mapper;
        }

        public async Task<IEnumerable<GetPaymentMethod>> GetPaymentMethodsAsync()
        {
            var methods = await this._paymentMethod.GetPaymentMethodsAsync();

            if (methods == null || !methods.Any())
            {
                return [];
            }

            return this._mapper.Map<IEnumerable<GetPaymentMethod>>(methods);
        }
    }
}
