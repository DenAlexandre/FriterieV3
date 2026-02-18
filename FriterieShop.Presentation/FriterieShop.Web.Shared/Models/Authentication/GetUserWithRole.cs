namespace FriterieShop.Web.Shared.Models.Authentication
{
    public class GetUserWithRole
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
