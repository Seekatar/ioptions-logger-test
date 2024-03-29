﻿using IOptionTest.Interfaces;
using IOptionTest.Models;

namespace IOptionTest.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _configuration;

        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task<Configuration> GetConfiguration()
        {
            if (_configuration is null) throw new Exception();

            return Task.FromResult(
                new Configuration
                {
                    FromAppSettings = _configuration["FromAppSettings"] ?? "",
                    FromDevelopmentSettings = _configuration["FromDevelopmentSettings"] ?? "",
                    FromEnvironment = _configuration["FromEnvironment"] ?? "",
                    FromSharedDevelopmentSettings = _configuration["FromSharedDevelopmentSettings"] ?? ""
                }
            );
        }
        public Task<Configuration> GetConfigurationSection()
        {
            var ret = _configuration.GetSection(Configuration.SectionName).Get<Configuration>();
            ret!.Name = "ConfigurationSection";
            return Task.FromResult(ret);
        }
    }
}
