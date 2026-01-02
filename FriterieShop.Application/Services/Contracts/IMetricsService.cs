namespace FriterieShop.Application.Services.Contracts
{
    using FriterieShop.Application.DTOs.Analytics;

    public interface IMetricsService
    {
        Task<MetricsSeriesDto> GetSalesAsync(DateTime fromUtc, DateTime toUtc, MetricsGranularity granularity);

        Task<MetricsSeriesDto> GetTrafficAsync(DateTime fromUtc, DateTime toUtc, MetricsGranularity granularity);
    }
}