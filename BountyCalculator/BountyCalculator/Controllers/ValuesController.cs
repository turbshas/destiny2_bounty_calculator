using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BountyCalculator.ApiInteraction;
using Microsoft.AspNetCore.Mvc;

namespace BountyCalculator.Controllers
{
    [Route("")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<bool>> Get()
        {
            ApiClient client = new ApiClient(AuthorizationManager.Instance);
            string authorizationUrl = await client.GetAuthorizationUrl();

            /* Redirect the client to the login/authorization portal */
            Response.Headers.Add("Location", authorizationUrl);
            Response.StatusCode = (int)HttpStatusCode.RedirectKeepVerb;
            return true;
        }

        [HttpGet]
        [Route("hello")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<string>> GetAfterAuthorization()
        {
            string requestGuidString = Request.Query.FirstOrDefault(pair => pair.Key.Equals("state")).Value;
            
            if (requestGuidString == null)
            {
                return BadRequest("Query string parameter 'state' is required.");
            }

            string authCode = Request.Query.FirstOrDefault(pair => pair.Key.Equals("code")).Value;
            if (authCode == null)
            {
                return BadRequest("Query string parameter 'code' is required.");
            }

            ApiClient client = new ApiClient(AuthorizationManager.Instance);
            bool result = await client.GetToken(requestGuidString, authCode);

            if (result)
            {
                return "You've just authorized this app";
            }
            else
            {
                return this.BadRequest();
            }
        }
    }
}
