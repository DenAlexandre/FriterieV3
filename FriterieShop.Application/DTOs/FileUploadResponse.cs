namespace FriterieShop.Application.DTOs
{
    public record FileUploadResponse(bool Success = false, string Message = null!, string Url = null!);
}
