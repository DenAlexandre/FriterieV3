namespace FriterieShop.Web.Shared.Services.Contracts
{
    using FriterieShop.Web.Shared.Models;

    using Microsoft.AspNetCore.Components.Forms;

    public interface IFileUploadService
    {
        Task<FileUploadResponse> UploadFileAsync(IBrowserFile file);
    }
}
