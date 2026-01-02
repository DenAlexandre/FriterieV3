namespace FriterieShop.Domain.Contracts.Authentication
{
    using FriterieShop.Domain.Entities.Identity;

    public interface IAppRoleManager
    {
        Task<string?> GetUserRoleAsync(string userEmail);

        Task<bool> AddUserToRoleAsync(AppUser user, string roleName);
    }
}
