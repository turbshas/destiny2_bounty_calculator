using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace BountyCalculator
{
    class ApiClient
    {
        private const string m_baseUri = "https://www.bungie.net";
        private const string m_authUri = "en/oauth/authorize";
 
        private const string m_redirectUri = "https://localhost:45932/";

        private const string m_apiKeyHeader = "X-API-Key";
        private const string m_apiKey = "b944b784b5b3426cac173a27ed20d40a";
        private const string m_oauthClientId = "30304";

        private readonly HttpClient _client;

        public ApiClient()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(m_baseUri),
            };
            _client.DefaultRequestHeaders.Add(m_apiKeyHeader, m_apiKey);
        }

        public string GetAuthorizationUrl()
        {
            Guid guidA = Guid.NewGuid();
            Guid guidB = Guid.NewGuid();
            string guidString = $"{guidA}_{guidB}";

            Dictionary<string, string> query = new Dictionary<string, string>
            {
                { "client_id", m_oauthClientId },
                { "response_type", "code" },
                { "state", guidString },
                { "redirect_uri", m_redirectUri },
            };
            HttpResponseMessage response = _client.GetAsync(QueryHelpers.AddQueryString(m_authUri, query)).Result;

            /* URI is encoded twice, so need to decode twice to get the query string */
            string firstDecode = WebUtility.UrlDecode(response.RequestMessage.RequestUri.ToString());
            string resultUri = WebUtility.UrlDecode(firstDecode);
            int queryIndex = resultUri.IndexOf('?');
            if (queryIndex < 0)
            {
                /* Shouldn't happen, TODO: create custom exception for this */
                throw new HttpRequestException("Unexpected error with authorization request - no query string returned.");
            }

            Dictionary<string, StringValues> queryValues = QueryHelpers.ParseNullableQuery(resultUri.Substring(queryIndex));
            if (queryValues == null)
            {
                /* Shouldn't happen, TODO: create custom exception for this */
                throw new HttpRequestException("Unexpected error with authorization request - no query string returned.");
            }

            string stateReturned = queryValues["state"];
            if (stateReturned != guidString)
            {
                throw new InvalidOperationException("Incorrect state returned by API.");
            }

            return response.RequestMessage.RequestUri.ToString();
        }
    }
}
