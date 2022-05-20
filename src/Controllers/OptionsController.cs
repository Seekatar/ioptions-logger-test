/*
 * IConfiguration and IOption test
 *
 * Test API to show using IOptions and IConfiguration
 *
 * OpenAPI spec version: 1.0.0-oas3
 * Contact: myemail@aisreview.com
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;
using IO.Swagger.Attributes;

using Microsoft.AspNetCore.Authorization;
using IOptionTest;
using System.Threading.Tasks;
using OptionsLoggerTest.Interfaces;
using OptionLoggerTest;
using Microsoft.Extensions.Options;

namespace IOptionTest.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [ApiController]
    public class OptionsApiController : ControllerBase
    {
        private readonly IOptionsService _optionsService;
        private readonly IOptionsSnapshot<SnapshotOptions> _snapshot;
        private readonly ILogger<OptionsApiController> _logger;

        public OptionsApiController(IOptionsSnapshot<SnapshotOptions> snapshot,
                                    IOptionsService optionsService,
                                    ILogger<OptionsApiController> logger)
        {
            _optionsService = optionsService;
            _snapshot = snapshot;
            _logger = logger;
        }

        /// <summary>
        /// Get IOptions values using monitor
        /// </summary>
        /// <response code="200">Ok</response>
        /// <response code="400">bad input parameter</response>
        [HttpGet]
        [Route("/api/options/monitored")]
        [ValidateModelState]
        [SwaggerOperation("GetIMonitoredOptions")]
        [SwaggerResponse(statusCode: 200, type: typeof(Configuration), description: "Ok")]
        public virtual async Task<ActionResult<MonitoredOptions>> GetIMonitoredOptions()
        {
            return Ok(await _optionsService.GetMonitoredOptions());
        }

        /// <summary>
        /// Get IOptions values
        /// </summary>
        /// <response code="200">Ok</response>
        /// <response code="400">bad input parameter</response>
        [HttpGet]
        [Route("/api/options")]
        [ValidateModelState]
        [SwaggerOperation("GetIOptions")]
        [SwaggerResponse(statusCode: 200, type: typeof(Configuration), description: "Ok")]
        public virtual async Task<ActionResult<OneTimeOptions>> GetIOptions()
        {
            return Ok(await _optionsService.GetOneTimeOptions());
        }

        /// <summary>
        /// Get IOptions values using snapshot
        /// </summary>
        /// <response code="200">Ok</response>
        /// <response code="400">bad input parameter</response>
        [HttpGet]
        [Route("/api/options/snapshot")]
        [ValidateModelState]
        [SwaggerOperation("GetISnapshotOptions")]
        [SwaggerResponse(statusCode: 200, type: typeof(Configuration), description: "Ok")]
        public virtual ActionResult<SnapshotOptions> GetISnapshotOptions()
        {
            try
            {
                return Ok(_snapshot.Value);
            }
            catch (OptionsValidationException e)
            {
                _logger.LogError(e, "Ow!");
            }
            return Ok(new SnapshotOptions());
        }
    }
}
