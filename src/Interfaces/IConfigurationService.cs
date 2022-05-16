using IOptionTest;

namespace OptionsLoggerTest.Interfaces
{
    public interface IConfigurationService
    {
        Task<Configuration> GetConfiguration();
        public Task<Configuration> GetConfigurationSection();
    }
}
