using OptionLoggerTest;

namespace OptionsLoggerTest.Interfaces
{
    public interface IOptionsService
    {
        Task<MonitoredOptions> GetMonitoredOptions();
        Task<OneTimeOptions> GetOneTimeOptions();
    }
}