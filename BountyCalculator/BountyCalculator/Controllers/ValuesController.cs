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
        [ProducesResponseType((int)HttpStatusCode.RedirectKeepVerb)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> Get()
        {
            ApiClient client = new ApiClient(AuthorizationManager.Instance, AuthorizationManager.Instance);
            string authorizationUrl = await client.GetAuthorizationUrlAsync();

            return RedirectPreserveMethod(authorizationUrl);
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

            ApiClient client = new ApiClient(AuthorizationManager.Instance, AuthorizationManager.Instance);
            string clientGuid = await client.GetTokenAsync(requestGuidString, authCode);
            return await client.Test();

            return $"You've just authorized this app! Client guid: {clientGuid}";
        }
    }
}
