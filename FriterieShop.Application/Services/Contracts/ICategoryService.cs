namespace FriterieShop.Application.Services.Contracts
{
    using FriterieShop.Application.DTOs;
    using FriterieShop.Application.DTOs.Category;
    using FriterieShop.Application.DTOs.Product;

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
