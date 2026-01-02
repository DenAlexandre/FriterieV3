namespace FriterieShop.Domain.Contracts.CategoryPersistence
{
    using FriterieShop.Domain.Entities;

    public interface ICategoryRepository
    {
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId);
    }
}
