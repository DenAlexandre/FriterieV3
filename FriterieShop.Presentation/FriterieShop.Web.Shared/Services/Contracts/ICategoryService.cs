namespace FriterieShop.Web.Shared.Services.Contracts
{
    using FriterieShop.Web.Shared.Models;
    using FriterieShop.Web.Shared.Models.Category;
    using FriterieShop.Web.Shared.Models.Product;

    public interface ICategoryService
    {
        Task<IEnumerable<GetCategory>> GetAllAsync();

        Task<GetCategory> GetByIdAsync(Guid id);

        Task<ServiceResponse> AddAsync(CreateCategory category);

        Task<ServiceResponse> UpdateAsync(UpdateCategory category);

        Task<ServiceResponse> DeleteAsync(Guid id);

        Task<IEnumerable<GetProduct>> GetProductsByCategoryAsync(Guid id);
    }
}
