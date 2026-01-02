namespace FriterieShop.Application.Mapping
{
    using AutoMapper;

    using FriterieShop.Application.DTOs.Category;
    using FriterieShop.Application.DTOs.Payment;
    using FriterieShop.Application.DTOs.Product;
    using FriterieShop.Application.DTOs.Product.ProductVariant;
    using FriterieShop.Application.DTOs.UserIdentity;
    using FriterieShop.Domain.Entities;
    using FriterieShop.Domain.Entities.Identity;
    using FriterieShop.Domain.Entities.Payment;

    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            // CreateMap<Source, Destination>();
            this.CreateMap<CreateCategory, Category>();
            this.CreateMap<UpdateCategory, Category>();
            this.CreateMap<Category, GetCategory>();

            this.CreateMap<CreateProduct, Product>();
            this.CreateMap<UpdateProduct, Product>();
            this.CreateMap<Product, GetProduct>()
                .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants));

            this.CreateMap<Product, GetProductRecommendation>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));

            this.CreateMap<CreateProductVariant, ProductVariant>();
            this.CreateMap<UpdateProductVariant, ProductVariant>();
            this.CreateMap<ProductVariant, GetProductVariant>();

            this.CreateMap<CreateUser, AppUser>();
            this.CreateMap<LoginUser, AppUser>();

            this.CreateMap<PaymentMethod, GetPaymentMethod>();
            this.CreateMap<CreateOrderItem, OrderItem>();
        }
    }
}
