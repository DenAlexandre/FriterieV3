namespace FriterieShop.Application.Services.Contracts
{
    using FriterieShop.Application.DTOs.Product;

    public interface IProductRecommendationService
    {
        Task<IEnumerable<GetProductRecommendation>> GetRecommendationsForProductAsync(Guid productId);
    }
}
