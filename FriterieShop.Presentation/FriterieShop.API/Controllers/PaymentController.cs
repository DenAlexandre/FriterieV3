namespace FriterieShop.API.Controllers
{
    using FriterieShop.Application.DTOs;
    using FriterieShop.Application.DTOs.Payment;
    using FriterieShop.Application.Services.Contracts.Payment;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IPayPalPaymentService _payPalPaymentService;
        private readonly FrontendSettings _frontendSettings;

        public PaymentController(
            IPaymentMethodService paymentMethodService,
            IPayPalPaymentService payPalPaymentService,
            IOptions<FrontendSettings> frontendSettings)
        {
            _paymentMethodService = paymentMethodService;
            _payPalPaymentService = payPalPaymentService;
            _frontendSettings = frontendSettings.Value;
        }

        /// <summary>
        /// Get all payment methods
        /// </summary>
        /// <returns>The payment methods </returns>
        [HttpGet("methods")]
        public async Task<ActionResult<IEnumerable<GetPaymentMethod>>> GetPaymentMethods()
        {
            var paymentMethods = await _paymentMethodService.GetPaymentMethodsAsync();
            return !paymentMethods.Any() ? this.NotFound() : this.Ok(paymentMethods);
        }

        /// <summary>
        /// PayPal capture redirect destination (stub implementation)
        /// </summary>
        [HttpGet("paypal/capture")]
        public async Task<IActionResult> CapturePayPal([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return BadRequest("Missing token");
            var ok = await _payPalPaymentService.CaptureAsync(token);
            if (!ok) return BadRequest("Capture failed");

            return Redirect($"{_frontendSettings.BaseUrl.TrimEnd('/')}/payment-success");
        }
    }
}
