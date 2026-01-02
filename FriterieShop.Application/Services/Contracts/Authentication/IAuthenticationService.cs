namespace FriterieShop.Application.Services.Contracts.Authentication
{
    using FriterieShop.Application.DTOs;
    using FriterieShop.Application.DTOs.UserIdentity;

    public interface IAuthenticationService
    {
        Task<ServiceResponse> CreateUser(CreateUser user);

        Task<LoginResponse> LoginUser(LoginUser user);

        Task<LoginResponse> ReviveToken(string refreshToken);

        Task<ServiceResponse> ChangePassword(ChangePassword changePasswordDto, string userId);

        Task<ServiceResponse> ConfirmEmail(string email, string token);

        Task<ServiceResponse> UpdateProfile(string userId, UpdateProfile dto);
    }
}
