namespace FriterieShop.Infrastructure.Repositories.Payment
{
    using FriterieShop.Domain.Contracts.Payment;
    using FriterieShop.Domain.Entities.Payment;
    using FriterieShop.Infrastructure.Data;

    using Microsoft.EntityFrameworkCore;

    public class CartRepository : ICart
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            this._context = context;
        }

        public async Task<int> SaveCheckoutHistory(IEnumerable<OrderItem> checkouts)
        {
            this._context.CheckoutOrderItems.AddRange(checkouts);
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderItem>> GetAllCheckoutHistory()
        {
            return await _context.CheckoutOrderItems.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<OrderItem>> GetCheckoutHistoryByUserId(string userId)
        {
            return await _context.CheckoutOrderItems
                       .Where(o => o.UserId == userId)
                       .ToListAsync();
        }
    }
}
