using OptionLoggerTest;
using OptionsLoggerTest.Interfaces;

namespace OptionsLoggerTest.Services
{
    public class OptionsService : IOptionsService
    {
        public Task<MonitoredOptions> GetMonitoredOptions()
        {
            throw new NotImplementedException();
        }

        public Task<OneTimeOptions> GetOneTimeOptions()
        {
            throw new NotImplementedException();
        }
    }
}
