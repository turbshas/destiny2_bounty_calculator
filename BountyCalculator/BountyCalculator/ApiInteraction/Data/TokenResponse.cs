using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BountyCalculator.ApiInteraction.Data
{
    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /* Should always be "Bearer" */
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        /* Should always be null as this is a Public app, not Confidential */
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        /* Should always be null as this is a Public app, not Confidential */
        [JsonProperty("refresh_expires_in")]
        public int RefreshExpiresIn { get; set; }

        [JsonProperty("membership_id")]
        public string MembershipId { get; set; }
    }
}
