namespace FriterieShop.Infrastructure.Services
{
    using FriterieShop.Application.Services.Contracts.Logging;

    using Microsoft.Extensions.Logging;

    public class LoggerAdapter<T> : IAppLogger<T>
    {
        private readonly ILogger<T> _logger;

        public LoggerAdapter(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }

        public void LogError(Exception exception, string message)
        {
            _logger.LogError(exception, message);
        }
    }
}
