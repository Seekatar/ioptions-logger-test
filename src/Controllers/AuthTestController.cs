/*
 * IConfiguration and IOption test
 *
 * Test API to show using IOptions and IConfiguration
 *
 * OpenAPI spec version: 1.0.0-oas3
 * Contact: myemail@bogus.com
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using IOptionTest;
using System.Threading.Tasks;
using IOptionTest.Models;
using static IOptionTest.Auth.AuthConstants;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace IOptionTest.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class AuthTestApiController : ControllerBase
    {
        private readonly ILogger<AuthTestApiController> _logger;
        private readonly IAuthenticationSchemeProvider _schemeProvider;

        private ActionResult<Message> OkMessage(string message, HttpContext context)
        {
            // Get the authentication schemes
            var schemes = _schemeProvider.GetAllSchemesAsync().Result;

            _logger.LogInformation("{method}", message);
            
            var name = context.User.FindFirst(ClaimTypes.Name)?.Value;
            if (name is null)
                _logger.LogInformation("    No Name claim");
            else
                _logger.LogInformation("    Name claim: {name}", name);

            // log out all the role claims for the user
            var roles = context.User.FindAll(ClaimTypes.Role);
            if (roles is null)
                _logger.LogInformation("    No Role claims");
            else
            {
                foreach (var role in roles)
                {
                    _logger.LogInformation("    Role: {role}", role.Value);
                }
            }
            
            #if list_all_schemes
            foreach (var scheme in schemes)
            {
                _logger.LogInformation("    {scheme}", scheme.Name);
            }
            #endif
            return Ok(new Message { Text = message });
        }

        public AuthTestApiController(ILogger<AuthTestApiController> logger, IAuthenticationSchemeProvider schemeProvider)
        {
            _logger = logger;
            _schemeProvider = schemeProvider;
        }

        /// <summary>
        /// Get output for test
        /// </summary>
        /// <response code="200">Ok</response>
        /// <response code="400">bad input parameter</response>
        [HttpGet]
        [Route("/api/auth/a")]
        [SwaggerOperation("GetAuthA")]
        [SwaggerResponse(statusCode: 200, type: typeof(Message), description: "Ok")]
        [Authorize(PolicyA)]
        public virtual ActionResult<Message> GetAuthA()
        {
            return OkMessage(MethodBase.GetCurrentMethod()!.Name, HttpContext);
        }

        /// <summary>
        /// Get output for test
        /// </summary>
        /// <response code="200">Ok</response>
        /// <response code="400">bad input parameter</response>
        [HttpGet]
        [Route("/api/auth/a-and-b")]
        [SwaggerOperation("GetAuthAB")]
        [SwaggerResponse(statusCode: 200, type: typeof(Message), description: "Ok")]
        [Authorize(PolicyA, AuthenticationSchemes = SchemeA)]
        [Authorize(PolicyB, AuthenticationSchemes = SchemeB)]
        public virtual ActionResult<Message> GetAuthAB()
        {
            return OkMessage(MethodBase.GetCurrentMethod()!.Name, HttpContext);
        }

        /// <summary>
        /// Get output for test
        /// </summary>
        /// <response code="200">Ok</response>
        /// <response code="400">bad input parameter</response>
        [HttpGet]
        [Route("/api/auth/anon")]
        [SwaggerOperation("GetAuthAnon")]
        [SwaggerResponse(statusCode: 200, type: typeof(Message), description: "Ok")]
        public virtual ActionResult<Message> GetAuthAnon()
        {
            return OkMessage(MethodBase.GetCurrentMethod()!.Name, HttpContext);
        }

        /// <summary>
        /// Get output for test
        /// </summary>
        /// <response code="200">Ok</response>
        /// <response code="400">bad input parameter</response>
        [HttpGet]
        [Route("/api/auth/a-or-b")]
        [SwaggerOperation("GetAuthAorB")]
        [SwaggerResponse(statusCode: 200, type: typeof(Message), description: "Ok")]
        [Authorize(PolicyAorB)]
        public virtual ActionResult<Message> GetAuthAorB()
        {
            return OkMessage(MethodBase.GetCurrentMethod()!.Name, HttpContext);
        }

        /// <summary>
        /// Get output for test
        /// </summary>
        /// <response code="200">Ok</response>
        /// <response code="400">bad input parameter</response>
        [HttpGet]
        [Route("/api/auth/b")]
        [SwaggerOperation("GetAuthB")]
        [SwaggerResponse(statusCode: 200, type: typeof(Message), description: "Ok")]
        [Authorize(PolicyB)]
        public virtual ActionResult<Message> GetAuthB()
        {
            return OkMessage(MethodBase.GetCurrentMethod()!.Name, HttpContext);
        }

        /// <summary>
        /// Get output for test
        /// </summary>
        /// <response code="200">Ok</response>
        /// <response code="400">bad input parameter</response>
        [HttpGet]
        [Route("/api/auth/b-scheme")]
        [SwaggerOperation("GetAuthBScheme")]
        [SwaggerResponse(statusCode: 200, type: typeof(Message), description: "Ok")]
        [Authorize(PolicyB, AuthenticationSchemes = SchemeB)]
        public virtual ActionResult<Message> GetAuthBScheme()
        {
            return OkMessage(MethodBase.GetCurrentMethod()!.Name, HttpContext);
        }

        /// <summary>
        /// Get output for test
        /// </summary>
        /// <response code="200">Ok</response>
        /// <response code="400">bad input parameter</response>
        [HttpGet]
        [Route("/api/auth/any")]
        [SwaggerOperation("GetAuthAnyRole")]
        [SwaggerResponse(statusCode: 200, type: typeof(Message), description: "Ok")]
        [Authorize(PolicyAnyRole)]
        public virtual ActionResult<Message> GetAuthAnyRole()
        {
            return OkMessage(MethodBase.GetCurrentMethod()!.Name, HttpContext);
        }

        /// <summary>
        /// Get output for test
        /// </summary>
        /// <response code="200">Ok</response>
        /// <response code="400">bad input parameter</response>
        [HttpGet]
        [Route("/api/auth/a-role-c")]
        [SwaggerOperation("GetAuthARoleC")]
        [SwaggerResponse(statusCode: 200, type: typeof(Message), description: "Ok")]
        [Authorize(PolicyUserAandRoleC)]
        public virtual ActionResult<Message> GetAuthARoleC()
        {
            return OkMessage(MethodBase.GetCurrentMethod()!.Name, HttpContext);
        }

        /// <summary>
        /// Get output for test
        /// </summary>
        /// <response code="200">Ok</response>
        /// <response code="400">bad input parameter</response>
        [HttpGet]
        [Route("/api/auth")]
        [SwaggerOperation("GetAuthNone")]
        [SwaggerResponse(statusCode: 200, type: typeof(Message), description: "Ok")]
        [Authorize]
        public virtual ActionResult<Message> GetAuthNone()
        {
            return OkMessage(MethodBase.GetCurrentMethod()!.Name, HttpContext);
        }
    }
}
