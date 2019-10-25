using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BountyCalculator.ApiInteraction.Data
{
    class ApiResponse
    {
        /* JSON object containing the actual data returned by a request, if it succeeded. */
        public JObject Response { get; set; }

        public int ErrorCode { get; set; }

        public int ThrottleSeconds { get; set; }

        public string ErrorStatus { get; set; }

        public string Message { get; set; }

        /* Additional info for the message. */
        public JObject MessageData { get; set; }
    }
}
