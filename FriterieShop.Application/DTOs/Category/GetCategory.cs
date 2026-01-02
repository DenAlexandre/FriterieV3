namespace FriterieShop.Application.DTOs.Category
{
    using FriterieShop.Application.DTOs.Product;

    public class GetCategory : CategoryBase
    {
        public Guid Id { get; set; }

        public ICollection<GetProduct>? Products { get; set; }
    }
}