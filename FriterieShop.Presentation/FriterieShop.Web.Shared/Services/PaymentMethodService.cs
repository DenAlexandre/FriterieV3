namespace FriterieShop.Web.Shared.Services
{
    using FriterieShop.Web.Shared.Helper.Contracts;
    using FriterieShop.Web.Shared.Models;
    using FriterieShop.Web.Shared.Models.Payment;
    using FriterieShop.Web.Shared.Services.Contracts;

    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IHttpClientHelper _httpClientHelper;
        private readonly IApiCallHelper _apiCallHelper;

        public PaymentMethodService(IHttpClientHelper httpClientHelper, IApiCallHelper apiCallHelper)
        {
            _httpClientHelper = httpClientHelper;
            _apiCallHelper = apiCallHelper;
        }

        public async Task<IEnumerable<GetPaymentMethod>> GetPaymentMethods()
        {
            var client = _httpClientHelper.GetPublicClient();
            var currentApiCall = new ApiCall
                                     {
                                         Route = Constant.Payment.GetAll,
                                         Type = Constant.ApiCallType.Get,
                                         Client = client,
                                         Model = null!,
                                         Id = null!
                                     };

            var result = await _apiCallHelper.ApiCallTypeCall<Unit>(currentApiCall);

            return result.IsSuccessStatusCode
                       ? await this._apiCallHelper.GetServiceResponse<IEnumerable<GetPaymentMethod>>(result)
                       : [];
        }
    }
}
