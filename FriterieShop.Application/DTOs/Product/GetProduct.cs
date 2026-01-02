namespace FriterieShop.Application.DTOs.Product
{
    using System.ComponentModel.DataAnnotations;

    using FriterieShop.Application.DTOs.Category;
    using FriterieShop.Application.DTOs.Product.ProductVariant;

    public class GetProduct : ProductBase
    {
        [Required]
        public Guid Id { get; set; }

        public GetCategory? Category { get; set; }

        public DateTime CreatedOn { get; set; }

        public IEnumerable<GetProductVariant> Variants { get; set; } = Array.Empty<GetProductVariant>();
    }
}
