namespace FriterieShop.Infrastructure.Repositories.Payment
{
    using FriterieShop.Domain.Contracts.Payment;
    using FriterieShop.Domain.Entities.Payment;
    using FriterieShop.Infrastructure.Data;

    using Microsoft.EntityFrameworkCore;

    public class PaymentMethodRepository : IPaymentMethod
    {
        private readonly AppDbContext _context;

        public PaymentMethodRepository(AppDbContext context)
        {
            this._context = context;
        }

        public async Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync()
        {
            return await this._context.PaymentMethods.AsNoTracking().ToListAsync();
        }
    }
}
