using BountyCalculator.ApiInteraction.Data;
using BountyCalculator.Data;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BountyCalculator.ApiInteraction
{
    class ApiClient
    {
        private const string BASE_URI = "https://www.bungie.net";
        private const string AUTH_URI = "en/oauth/authorize";
        private const string TOKEN_URI = "platform/app/oauth/token";
 
        private const string REDIRECT_URI = "https://localhost:44300/hello";

        private const string API_KEY_HEADER = "X-API-Key";
        private const string API_KEY = "b944b784b5b3426cac173a27ed20d40a";
        private const string AUTH_HEADER_KEY = "Authorization";
        private const string OAUTH_CLIENT_ID = "30304";

        private readonly IAuthorizationManager _authMgr;
        private readonly HttpClient _client;
        private ExpirationWrapper<string> _token;
        private string _membershipId;

        public ApiClient(IAuthorizationManager authMgr)
        {
            _authMgr = authMgr;

            _client = new HttpClient
            {
                BaseAddress = new Uri(BASE_URI),
            };
            _client.DefaultRequestHeaders.Add(API_KEY_HEADER, API_KEY);
        }

        #region [ Authorization & Token Retrieval ]

        public async Task<string> GetAuthorizationUrl()
        {
            RequestGuid guid = new RequestGuid();
            string guidString = guid.ToString();

            Dictionary<string, string> query = new Dictionary<string, string>
            {
                { "client_id", OAUTH_CLIENT_ID },
                { "response_type", "code" },
                { "state", guidString },
                { "redirect_uri", REDIRECT_URI },
            };
            HttpResponseMessage response = await _client.GetAsync(QueryHelpers.AddQueryString(AUTH_URI, query));

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

            /* Request for authorization URL was successful */
            _authMgr.AddRequestGuid(guid);
            return response.RequestMessage.RequestUri.ToString();
        }

        public bool VerifyAuthRequest(string requestGuidString)
        {
            RequestGuid guid = _authMgr.GetRequestGuid(requestGuidString);
            if (guid != null && guid.ToString() == requestGuidString)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //TODO: throw exceptions?
        public async Task<bool> GetToken(string requestGuidString, string authCode)
        {
            if (!VerifyAuthRequest(requestGuidString))
            {
                return false;
            }

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, TOKEN_URI);

            Dictionary<string, string> values = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", authCode },
            };
            FormUrlEncodedContent content = new FormUrlEncodedContent(values);
            message.Content = content;

            HttpResponseMessage response = await _client.SendAsync(message);
            string responseString = await response.Content.ReadAsStringAsync();
            if (responseString == null)
            {
                return false;
            }

            TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseString);
            if (tokenResponse == null || tokenResponse.TokenType != "Bearer")
            {
                return false;
            }

            _token = new ExpirationWrapper<string>(tokenResponse.AccessToken, tokenResponse.ExpiresIn * 1000);
            _membershipId = tokenResponse.MembershipId;

            _client.DefaultRequestHeaders.Add(AUTH_HEADER_KEY, $"{tokenResponse.TokenType} {_token.Value}");

            //TODO: store api clients in authorization manager, return request guid to client?
            return true;
        }

        #endregion

        private async Task<T> MakeRequest<T>(HttpMethod method, string uri, HttpContent body)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, uri);
            if (body != null)
            {
                request.Content = body;
            }

            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                //TODO: throw exception
            }

            string responseString = await response.Content.ReadAsStringAsync();
        }
    }
}
