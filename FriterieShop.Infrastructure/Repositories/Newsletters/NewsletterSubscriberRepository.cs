namespace FriterieShop.Infrastructure.Repositories.Newsletters
{
    using System;
    using System.Linq;

    using FriterieShop.Domain.Contracts.Newsletters;
    using FriterieShop.Domain.Entities;
    using FriterieShop.Infrastructure.Data;

    using Microsoft.EntityFrameworkCore;

    public class NewsletterSubscriberRepository(AppDbContext context) : INewsletterSubscriberRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<NewsletterSubscriber>> GetByDateRangeAsync(DateTime fromUtc, DateTime toUtc)
        {
            return await _context.NewsletterSubscribers
                .AsNoTracking()
                .Where(s => s.CreatedOn >= fromUtc && s.CreatedOn <= toUtc)
                .OrderBy(s => s.CreatedOn)
                .ToListAsync();
        }
    }
}