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
        [Authorize(Policy = AuthConstants.PolicyA)]
        public virtual ActionResult<Message> GetAuthA()
        {
            return Ok(new Message() { _Message = "Hello World" });
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
        [Authorize(Policy = AuthConstants.PolicyA)]
        [Authorize(Policy = AuthConstants.PolicyB, AuthenticationSchemes = AuthConstants.SchemeB)]
        public virtual ActionResult<Message> GetAuthAB()
        {
            return Ok(new Message() { _Message = "Hello World" });
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
            return Ok(new Message() { _Message = "Hello World" });
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
        [Authorize(Policy = PolicyAorB)]
        public virtual ActionResult<Message> GetAuthAorB()
        { 
            return Ok(new Message() { _Message = "Hello World" });
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
        [Authorize(Policy = PolicyB)]
        public virtual ActionResult<Message> GetAuthB()
        {
            return Ok(new Message() { _Message = "Hello World" });
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
        [Authorize(Policy = PolicyB, AuthenticationSchemes = SchemeB)]
        public virtual ActionResult<Message> GetAuthBScheme()
        {
            return Ok(new Message() { _Message = "Hello World" });
        }

        /// <summary>
        /// Get output for test
        /// </summary>
        /// <response code="200">Ok</response>
        /// <response code="400">bad input parameter</response>
        [HttpGet]
        [Route("/api/auth/c")]
        [SwaggerOperation("GetAuthC")]
        [SwaggerResponse(statusCode: 200, type: typeof(Message), description: "Ok")]
        [Authorize(Policy = AuthConstants.PolicyC)]
        public virtual ActionResult<Message> GetAuthC()
        {
            return Ok(new Message() { _Message = "Hello World" });
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
            return Ok(new Message() { _Message = "Hello World" });
        }
    }
}
