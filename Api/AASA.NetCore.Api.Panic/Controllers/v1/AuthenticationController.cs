using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using IdentityModel.Client;
using AASA.NetCore.Lib.Helper.Models;
using AASA.NetCore.Api.Panic.Models;

namespace AASA.NetCore.v1.Panic.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Produces("application/json")]

    public class AuthenticationController : ControllerBase
    {
        // GET: api/Authentication/5
        /// <summary>
        /// To get the Bearer Token
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// {
        /// 	"username":"your username",
        /// 	"password":"your password",
        /// 	"client_secret":"your secret",
        /// 	"client_id":"your id"
        /// }
        /// <br /> 
        /// Note:
        /// This token only lasts a certain time period, after expiry you will have to request a new token.
        /// </remarks>
        /// <param name="value"></param>
        /// <returns>The access_token (Bearer Token)</returns>
        /// <response code="200">
        /// Sample Response
        /// <br />
        /// {
        /// <br />
        ///  "access_token": "access token string",
        ///  <br />
        ///  "expires_in": (Time frame),
        ///  <br />
        ///  "token_type": "Bearer",
        ///  <br />
        ///  "scope": "(example)"
        ///  <br />
        ///}
        /// </response>
        [HttpPost]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public async Task<IActionResult> GetAuthToken([FromBody] Authenticate value)
        {
            var client = new HttpClient();

            
            var response = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = $"{ApplicationSettings.KeyCloakBaseUrl}/auth/realms/{ApplicationSettings.KeyCloakRealm}/protocol/openid-connect/token",

                ClientId = value.client_id,
                ClientSecret = value.client_secret,
                Scope = ApplicationSettings.KeyCloakScope, // Client Scope - Mapper - Audience
                // GrantType = "password",
                UserName = value.username,
                Password = value.password
            });

            if (response.IsError)
            {
                return BadRequest("Failed to authenticate " + response.ErrorDescription + response.Error);
            }

            try
            {
                ///dynamic d = JObject.Parse(response.Raw);
                var d = JsonConvert.DeserializeObject<AuthenticationResult>(response.Raw);
                return Ok(d);
            }
            catch (Exception)
            {

            }

            return Ok(response.Json);
        }

        // POST: api/Authentication
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Test()
        {
            return Ok("{ \"Service\": \"Running\", \"Status\":\"Healthy\" }");
        }
    }
}
