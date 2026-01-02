namespace FriterieShop.Web.Shared.Services.Contracts
{
    using FriterieShop.Web.Shared.Models.Analytics;

    public interface IMetricsClient
    {
        Task<MetricsSeriesModel?> GetSalesAsync(MetricsFilterModel filter);

        Task<MetricsSeriesModel?> GetTrafficAsync(MetricsFilterModel filter);
    }
}