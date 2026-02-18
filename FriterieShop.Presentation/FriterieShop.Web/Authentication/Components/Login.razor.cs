namespace FriterieShop.Web.Authentication.Components
{
    using System.Security.Claims;

    using FriterieShop.Web.Authentication.Providers;
    using FriterieShop.Web.Shared;
    using FriterieShop.Web.Shared.Models.Authentication;
    using FriterieShop.Web.Shared.Toast;

    using Microsoft.AspNetCore.Components;

    public partial class Login
    {
        private string _alertType = string.Empty;

        private string _message = string.Empty;

        [Parameter]
        public string Route { get; set; } = null!;

        public LoginUser User { get; set; } = new();

        private async Task LoginUser()
        {
            _message = string.Empty;
            _alertType = string.Empty;

            var result = await this.AuthenticationService.LoginUser(this.User);

            if (!result.Success)
            {
                _message = result.Message;
                _alertType = "danger";
                this.ToastService.ShowToast(ToastLevel.Error, result.Message, "Error", ToastIcon.Error);
                return;
            }

            string cookieValue = this.TokenService.FromToken(result.Token, result.RefreshToken);
            await this.TokenService.SetCookie(
                Constant.Cookie.Name,
                cookieValue,
                Constant.Cookie.Days,
                Constant.Cookie.Path);

            (this.AuthStateProvider as CustomAuthStateProvider)!.NotifyAuthenticationState();

            // Restore saved cart
            try
            {
                var authState = await this.AuthStateProvider.GetAuthenticationStateAsync();
                var userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    var savedCart = await this.CookieStorageService.GetFromLocalStorageAsync($"cart_{userId}");
                    if (!string.IsNullOrEmpty(savedCart))
                    {
                        await this.CookieStorageService.SetAsync(Constant.Cart.Name, savedCart, 30);
                        await this.CookieStorageService.RemoveFromLocalStorageAsync($"cart_{userId}");
                    }
                }
            }
            catch { }

            this.NavigationManager.NavigateTo(this.Route == null ? "/" : this.Route, true);
        }
    }
}