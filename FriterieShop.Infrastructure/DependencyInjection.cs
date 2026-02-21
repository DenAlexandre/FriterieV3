namespace FriterieShop.Infrastructure
{
    using FriterieShop.Application.DTOs;
    using FriterieShop.Application.DTOs.Payment;
    using FriterieShop.Application.Services.Contracts.Logging;
    using FriterieShop.Application.Services.Contracts.Payment;
    using FriterieShop.Application.Services.Payment;
    using FriterieShop.Domain.Contracts;
    using FriterieShop.Domain.Contracts.Authentication;
    using FriterieShop.Domain.Contracts.CategoryPersistence;
    using FriterieShop.Domain.Contracts.Newsletters;
    using FriterieShop.Domain.Contracts.Payment;
    using FriterieShop.Domain.Entities.Identity;
    using FriterieShop.Infrastructure.Data;
    using FriterieShop.Infrastructure.ExceptionsMiddleware;
    using FriterieShop.Infrastructure.Repositories;
    using FriterieShop.Infrastructure.Repositories.Authentication;
    using FriterieShop.Infrastructure.Repositories.CategoryPersistence;
    using FriterieShop.Infrastructure.Repositories.Newsletters;
    using FriterieShop.Infrastructure.Repositories.Payment;
    using FriterieShop.Infrastructure.Services;

    using EntityFramework.Exceptions.PostgreSQL;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;

    public static class DependencyInjection
    {
        /// <summary>
        /// Convertit une URI PostgreSQL (postgres://user:pass@host:port/db?params) en format ADO.NET
        /// nécessaire pour Npgsql. Supporte Render (fromDatabase) et Neon (channel_binding).
        /// </summary>
        private static string NormalizeConnectionString(string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return connectionString ?? string.Empty;

            if (!connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
                !connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
                return connectionString;

            var uri = new Uri(connectionString);
            var userInfo = uri.UserInfo.Split(':');
            var username = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : "";
            var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
            var host = uri.Host;
            var port = uri.Port > 0 ? uri.Port : 5432;
            var database = uri.AbsolutePath.TrimStart('/');

            var result = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";

            // Mapper les query parameters URI vers les paramètres Npgsql
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            if (query["channel_binding"] is string cb)
                result += $";Channel Binding={cb}";

            return result;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = NormalizeConnectionString(config.GetConnectionString("DefaultConnection"));

            services.AddDbContext<AppDbContext>(
                opt => opt
                    .UseNpgsql(
                        connectionString,
                        npgsqlOptions =>
                            {
                                npgsqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                                npgsqlOptions.EnableRetryOnFailure();
                            })
                    .UseExceptionProcessor()
                    .ConfigureWarnings(w => w.Log(RelationalEventId.PendingModelChangesWarning))
            );

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));

            services.AddDefaultIdentity<AppUser>(
                opt =>
                    {
                        opt.SignIn.RequireConfirmedEmail = true;
                        opt.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
                        opt.Password.RequireDigit = true;
                        opt.Password.RequireNonAlphanumeric = true;
                        opt.Password.RequiredLength = 8;
                        opt.Password.RequireLowercase = true;
                        opt.Password.RequireUppercase = true;
                        opt.Password.RequiredUniqueChars = 1;
                    })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            services.AddAuthentication(opt =>
                {
                    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(opt =>
                {
                    opt.SaveToken = true;
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                        ValidateIssuerSigningKey = true,
                        ValidAudience = config["JWT:Audience"],
                        ValidIssuer = config["JWT:Issuer"],
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config["JWT:Key"]!)),
                    };
                });

            services.AddScoped<IAppUserManager, AppUserManager>();
            services.AddScoped<IAppTokenManager, AppTokenManager>();
            services.AddScoped<IAppRoleManager, AppRoleManager>();

            services.AddScoped<IPaymentMethod, PaymentMethodRepository>();
            services.AddScoped<IPaymentService, StripePaymentService>();
            services.AddScoped<IPayPalPaymentService, PayPalPaymentService>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderTrackingService, OrderTrackingService>();
            services.AddScoped<IOrderQueryService, OrderQueryService>();
            services.AddScoped<INewsletterSubscriberRepository, NewsletterSubscriberRepository>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<ICart, CartRepository>();

            // Product recommendations
            services.AddScoped<IProductRecommendationRepository, ProductRecommendationRepository>();

            // Add memory cache for recommendations
            services.AddMemoryCache();

            Stripe.StripeConfiguration.ApiKey = config["Stripe:SecretKey"];

            services.Configure<FrontendSettings>(config.GetSection("Frontend"));
            services.Configure<EmailSettings>(config.GetSection("EmailSettings"));
            services.Configure<BankTransferSettings>(config.GetSection("BankTransfer"));
            services.AddTransient<IEmailService, EmailService>();

            return services;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            return app;
        }
    }
}
