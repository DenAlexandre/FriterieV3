namespace FriterieShop.Web.Shared.Models.Category
{
    using FriterieShop.Web.Shared.Models.Product;

    public class GetCategory : CategoryBase
    {
        public Guid Id { get; set; }
        
        public ICollection<GetProduct>? Products { get; set; }
    }
}
