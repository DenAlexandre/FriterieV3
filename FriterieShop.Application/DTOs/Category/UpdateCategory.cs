namespace FriterieShop.Application.DTOs.Category
{
    using System.ComponentModel.DataAnnotations;

    public class UpdateCategory : CategoryBase
    {
        [Required]
        public Guid Id { get; set; }
    }
}
