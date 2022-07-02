using IOptionTest.Options;

namespace IOptionTest.Interfaces
{
    public interface IOptionsService
    {
        Task<MonitoredOptions> GetMonitoredOptions();
        Task<OneTimeOptions> GetOneTimeOptions();
    }
}