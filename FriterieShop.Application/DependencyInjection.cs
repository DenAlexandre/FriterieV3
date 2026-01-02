namespace FriterieShop.Application
{
    using FriterieShop.Application.Mapping;
    using FriterieShop.Application.Options;
    using FriterieShop.Application.Services;
    using FriterieShop.Application.Services.Authentication;
    using FriterieShop.Application.Services.Contracts;
    using FriterieShop.Application.Services.Contracts.Authentication;
    using FriterieShop.Application.Services.Contracts.Payment;
    using FriterieShop.Application.Services.Payment;
    using FriterieShop.Application.Validations;
    using FriterieShop.Application.Validations.Authentication;

    using FluentValidation;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingConfig>());
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductVariantService, ProductVariantService>();
            services.AddScoped<IProductRecommendationService, ProductRecommendationService>();
            services.AddScoped<IMetricsService, MetricsService>();

            services.Configure<RecommendationOptions>(configuration.GetSection(RecommendationOptions.SectionName));

            services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();
            services.AddScoped<IValidationService, ValidationService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IPaymentMethodService, PaymentMethodService>();

            services.AddScoped<INewsletterService, NewsletterService>();

            return services;
        }
    }
}
