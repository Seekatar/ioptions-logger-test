/*
 * IConfiguration and IOption test
 *
 * Test API to show using IOptions and IConfiguration
 *
 * OpenAPI spec version: 1.0.0-oas3
 * Contact: myemail@bogus.com
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using IOptionTest.Interfaces;
using IOptionTest.Models;
using IOptionTest.Attributes;

namespace IOptionTest.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [ApiController]
    public class ConfigurationApiController : ControllerBase
    {
        private readonly IConfigurationService _configurationService;

        public ConfigurationApiController(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        /// <summary>
        /// Get IConfiguration values
        /// </summary>
        /// <response code="200">Configuration</response>
        /// <response code="400">bad input parameter</response>
        [HttpGet]
        [Route("/api/config")]
        [ValidateModelState]
        [SwaggerOperation("GetIConfiguration")]
        [SwaggerResponse(statusCode: 200, type: typeof(Configuration), description: "Configuration")]
        public virtual async Task<ActionResult<Configuration>> GetIConfiguration()
        {
            return Ok(await _configurationService.GetConfiguration());
        }

        /// <summary>
        /// Get IConfiguration values in a section
        /// </summary>
        /// <response code="200">Configuration</response>
        /// <response code="400">bad input parameter</response>
        [HttpGet]
        [Route("/api/config/section")]
        [ValidateModelState]
        [SwaggerOperation("GetIConfigurationSection")]
        [SwaggerResponse(statusCode: 200, type: typeof(Configuration), description: "Configuration")]
        public virtual async Task<IActionResult> GetIConfigurationSection()
        {
            return Ok(await _configurationService.GetConfigurationSection());
        }
    }
}
