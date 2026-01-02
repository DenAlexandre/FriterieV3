namespace FriterieShop.Web.Shared.Services.Contracts
{
  using FriterieShop.Web.Shared.Models.Product;

    public interface IProductRecommendationService
    {
        Task<IEnumerable<GetProductRecommendation>> GetRecommendationsAsync(Guid productId);
    }
}
