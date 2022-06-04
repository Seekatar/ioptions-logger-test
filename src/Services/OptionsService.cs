using Microsoft.Extensions.Options;
using OptionLoggerTest;
using OptionsLoggerTest.Interfaces;

namespace OptionsLoggerTest.Services
{
    public class OptionsService : IOptionsService
    {
        private readonly ILogger<OptionsService> _logger;
        private readonly IOptionsMonitor<MonitoredOptions> _monitored;
        private readonly IOptions<OneTimeOptions> _onetime;

        public OptionsService(ILogger<OptionsService> logger, IOptionsMonitor<MonitoredOptions> monitored, IOptions<OneTimeOptions> onetime)
        {
            _logger = logger;
            _monitored = monitored;
            _onetime = onetime;
        }

        public Task<MonitoredOptions> GetMonitoredOptions()
        {
            try
            {
                _logger.LogInformation("Monitored output {value}", _monitored.CurrentValue);
                return Task.FromResult(_monitored.CurrentValue);
            }
            catch (OptionsValidationException e)
            {
                _logger.LogError(e, "Ow!");
            }
            return Task.FromResult(new MonitoredOptions());
        }

        public Task<OneTimeOptions> GetOneTimeOptions()
        {
            try
            {
                _logger.LogInformation("Onetime output {value}", _onetime.Value);
                return Task.FromResult(_onetime.Value);
            }
            catch (OptionsValidationException e)
            {
                _logger.LogError(e, "Ow!");
            }
            return Task.FromResult(new OneTimeOptions());
        }
    }
}
