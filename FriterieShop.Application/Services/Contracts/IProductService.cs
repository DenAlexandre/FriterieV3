namespace FriterieShop.Application.Services.Contracts
{
    using FriterieShop.Application.DTOs;
    using FriterieShop.Application.DTOs.Product;

    public interface IProductService
    {
        Task<IEnumerable<GetProduct>> GetAllAsync();

        Task<GetProduct?> GetByIdAsync(Guid id);

        Task<ServiceResponse> AddAsync(CreateProduct product);

        Task<ServiceResponse> UpdateAsync(UpdateProduct product);

        Task<ServiceResponse> DeleteAsync(Guid id);
    }
}
