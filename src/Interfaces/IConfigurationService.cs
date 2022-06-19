using IOptionTest.Models;

namespace IOptionTest.Interfaces
{
    public interface IConfigurationService
    {
        Task<Configuration> GetConfiguration();
        public Task<Configuration> GetConfigurationSection();
    }
}
