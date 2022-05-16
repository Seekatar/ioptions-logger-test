using IOptionTest;
using OptionsLoggerTest.Interfaces;

namespace OptionsLoggerTest.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public Task<Configuration> GetConfiguration()
        {
            throw new NotImplementedException();
        }
        public Task<Configuration> GetConfigurationSection()
        {
            throw new NotImplementedException();
        }
    }
}
