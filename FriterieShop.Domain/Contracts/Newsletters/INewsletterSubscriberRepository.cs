namespace FriterieShop.Domain.Contracts.Newsletters
{
    using FriterieShop.Domain.Entities;

    public interface INewsletterSubscriberRepository
    {
        Task<List<NewsletterSubscriber>> GetByDateRangeAsync(DateTime fromUtc, DateTime toUtc);
    }
}