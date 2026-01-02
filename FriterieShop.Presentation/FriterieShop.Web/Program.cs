namespace FriterieShop.Web
{
    using FriterieShop.Web.Authentication.Providers;
    using FriterieShop.Web.Shared;
    using FriterieShop.Web.Shared.CookieStorage;
    using FriterieShop.Web.Shared.CookieStorage.Contracts;
    using FriterieShop.Web.Shared.Helper;
    using FriterieShop.Web.Shared.Helper.Contracts;
    using FriterieShop.Web.Shared.Services;
    using FriterieShop.Web.Shared.Services.Contracts;

    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddSingleton<IBrowserCookieStorageService, BrowserCookieStorageService>();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IHttpClientHelper, HttpClientHelper>();
            builder.Services.AddScoped<IApiCallHelper, ApiCallHelper>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            builder.Services.AddScoped<RefreshTokenHandler>();

            var apiBase = new Uri(new Uri(builder.HostEnvironment.BaseAddress), "api/");

#if DEBUG
            builder.Services.AddHttpClient(
                Constant.ApiClient.Name,
                opt =>
                    {
                        opt.BaseAddress = new Uri("https://localhost:7094/api/");
                    }).AddHttpMessageHandler<RefreshTokenHandler>();


            //builder.Services.AddHttpClient(
            // Constant.ApiClient.Name,
            // client => { client.BaseAddress = apiBase; }
            // ).AddHttpMessageHandler<RefreshTokenHandler>();
#else


            builder.Services.AddHttpClient(
                Constant.ApiClient.Name,
                client => { client.BaseAddress = apiBase; }
            ).AddHttpMessageHandler<RefreshTokenHandler>();
#endif
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
            builder.Services.AddScoped<IFileUploadService, FileUploadService>();
            builder.Services.AddScoped<IProductVariantService, ProductVariantService>();
            builder.Services.AddScoped<IProductRecommendationService, ProductRecommendationService>();
            builder.Services.AddSingleton<IToastService, ToastService>();
            builder.Services.AddScoped<INewsletterService, NewsletterService>();
            builder.Services.AddScoped<IMetricsClient, MetricsClient>();

            await builder.Build().RunAsync();
        }
    }
}
