namespace FriterieShop.Application.Validations
{
    using FriterieShop.Application.DTOs;

    using FluentValidation;

    public interface IValidationService
    {
        Task<ServiceResponse> ValidateAsync<T>(T model, IValidator<T> validator);
    }
}
