namespace FriterieShop.Infrastructure.Repositories.CategoryPersistence
{
    using FriterieShop.Domain.Contracts.CategoryPersistence;
    using FriterieShop.Domain.Entities;
    using FriterieShop.Infrastructure.Data;

    using Microsoft.EntityFrameworkCore;

    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId)
        {
            var products = await _context
                               .Products
                               .Include(x => x.Category)
                               .Where(p => p.CategoryId == categoryId)
                               .AsNoTracking()
                               .ToListAsync();

            return products.Count > 0 ? products : [];
        }
    }
}
