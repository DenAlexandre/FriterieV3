namespace FriterieShop.Application.Services.Contracts
{
    using FriterieShop.Application.DTOs;
    using FriterieShop.Application.DTOs.Product.ProductVariant;

    public interface IProductVariantService
    {
        Task<IEnumerable<GetProductVariant>> GetByProductIdAsync(Guid productId);

        Task<ServiceResponse> AddAsync(CreateProductVariant variant);

        Task<ServiceResponse> UpdateAsync(UpdateProductVariant variant);

        Task<ServiceResponse> DeleteAsync(Guid variantId);
    }
}
