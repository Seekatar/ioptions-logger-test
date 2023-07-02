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
using static IOptionTest.AuthConstants;


namespace IOptionTest.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class AuthTestApiController : ControllerBase
    {
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
            return Ok(new Message() { Text = $"Hello from {MethodBase.GetCurrentMethod()!.Name}" });
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
        [Authorize(PolicyA)]
        [Authorize(PolicyB, AuthenticationSchemes = SchemeB)]
        public virtual ActionResult<Message> GetAuthAB()
        {
            return Ok(new Message() { Text = $"Hello from {MethodBase.GetCurrentMethod()!.Name}" });
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
            return Ok(new Message() { Text = $"Hello from {MethodBase.GetCurrentMethod()!.Name}" });
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
            return Ok(new Message() { Text = $"Hello from {MethodBase.GetCurrentMethod()!.Name}" });
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
            return Ok(new Message() { Text = $"Hello from {MethodBase.GetCurrentMethod()!.Name}" });
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
            return Ok(new Message() { Text = $"Hello from {MethodBase.GetCurrentMethod()!.Name}" });
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
            return Ok(new Message() { Text = $"Hello from {MethodBase.GetCurrentMethod()!.Name}" });
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
            return Ok(new Message() { Text = $"Hello from {MethodBase.GetCurrentMethod()!.Name}" });
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
            return Ok(new Message() { Text = $"Hello from {MethodBase.GetCurrentMethod()!.Name}" });
        }
    }
}
