namespace FriterieShop.Web.Shared.Models.Authentication
{
    public class UpdateUserModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }
}
