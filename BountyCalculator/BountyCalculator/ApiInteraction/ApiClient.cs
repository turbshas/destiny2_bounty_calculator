using BountyCalculator.ApiInteraction.Data;
using BountyCalculator.Data;
using BountyCalculator.Exceptions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        /* Request URIs require a trailing slash, for some goddamn reason. */
        private const string BASE_URI = "https://www.bungie.net";
        private const string AUTH_URI = "en/oauth/authorize/";
        private const string TOKEN_URI = "platform/app/oauth/token/";
        private const string TOKEN_TYPE = "Bearer";
 
        private const string REDIRECT_URI = "https://localhost:44300/hello";

        private const string API_KEY_HEADER = "X-API-Key";
        private const string API_KEY = "b944b784b5b3426cac173a27ed20d40a";
        private const string AUTH_HEADER_KEY = "Authorization";
        private const string OAUTH_CLIENT_ID = "30304";

        private readonly IAuthorizationManager _authMgr;
        private readonly IClientManager _clientMgr;
        private readonly HttpClient _client;

        private string _guid;
        private ExpirationWrapper<string> _token;
        private string _membershipId;

        private string AuthHeaderValue
        {
            get
            {
                if (_token == null)
                {
                    throw new InvalidOperationException("Token not initialized for API interaction.");
                }

                if (_token.IsExpired)
                {
                    throw new InvalidOperationException("Token has expired.");
                }

                return $"{TOKEN_TYPE} {_token.Value}";
            }
        }

        public ApiClient(IAuthorizationManager authMgr, IClientManager clientMgr)
        {
            _authMgr = authMgr;
            _clientMgr = clientMgr;

            _client = new HttpClient
            {
                BaseAddress = new Uri(BASE_URI),
            };
            _client.DefaultRequestHeaders.Add(API_KEY_HEADER, API_KEY);
        }

        public string Guid
        {
            get
            {
                if (_guid == null)
                {
                    throw new InvalidOperationException("Client not initialized for API interaction.");
                }

                return _guid;
            }
        }

        public event EventHandler<EventArgs> TokenExpired;

        private void TokenExpiredHandler(object sender, EventArgs e)
        {
            TokenExpired?.Invoke(this, EventArgs.Empty);
        }

        #region [ Authorization & Token Retrieval ]

        public async Task<string> GetAuthorizationUrlAsync()
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
                throw new InvalidQueryException("No query string returned by authorization request.");
            }

            Dictionary<string, StringValues> queryValues = QueryHelpers.ParseNullableQuery(resultUri.Substring(queryIndex));
            if (queryValues == null)
            {
                throw new InvalidQueryException("Invalid query string returned by authorization request.");
            }

            string stateReturned = queryValues["state"];
            if (stateReturned != guidString)
            {
                throw new InvalidOperationException("Incorrect state returned by authorization request.");
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

        public async Task<string> GetTokenAsync(string requestGuidString, string authCode)
        {
            if (_token == null || !_token.IsExpired)
            {
                throw new InvalidOperationException("A valid token has already been retrieved.");
            }

            if (!VerifyAuthRequest(requestGuidString))
            {
                throw new ArgumentException("Invalid request guid.");
            }

            _guid = requestGuidString;

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, TOKEN_URI);

            Dictionary<string, string> values = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", authCode },
                { "client_id", OAUTH_CLIENT_ID },
                { "redirect_uri", REDIRECT_URI },
            };
            FormUrlEncodedContent content = new FormUrlEncodedContent(values);
            message.Content = content;

            HttpResponseMessage response = await _client.SendAsync(message);
            string responseString = await response.Content.ReadAsStringAsync();
            if (responseString == null)
            {
                throw new InvalidResponseException("Empty response returned by token request.");
            }

            TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseString);
            if (tokenResponse == null || tokenResponse.TokenType != TOKEN_TYPE)
            {
                throw new InvalidResponseException("Invalid response returned by token request.");
            }

            _token = new ExpirationWrapper<string>(tokenResponse.AccessToken, tokenResponse.ExpiresIn * 1000);
            _token.Expired += TokenExpiredHandler;
            _membershipId = tokenResponse.MembershipId;

            _clientMgr.AddApiClient(this);
            return requestGuidString;
        }

        #endregion

        private async Task<ApiResponse> MakeRequestAsync(HttpMethod method, string uri, HttpContent body)
        {
            if (!uri.EndsWith('/'))
            {
                /* Make sure request URIs have a trailing slash. */
                uri += "/";
            }

            HttpRequestMessage request = new HttpRequestMessage(method, uri);
            if (body != null)
            {
                request.Content = body;
            }

            request.Headers.Add(AUTH_HEADER_KEY, AuthHeaderValue);

            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new InvalidResponseException($"Error returned by request to {uri}: {response.StatusCode}");
            }

            string responseString = await response.Content.ReadAsStringAsync();
            if (responseString == null)
            {
                return new ApiResponse();
            }

            return JsonConvert.DeserializeObject<ApiResponse>(responseString) ?? new ApiResponse();
        }

        private Task<ApiResponse> GetAsync(string uri, HttpContent body = null)
        {
            return MakeRequestAsync(HttpMethod.Get, uri, body);
        }

        private Task<ApiResponse> PostAsync(string uri, HttpContent body)
        {
            return MakeRequestAsync(HttpMethod.Post, uri, body);
        }

        public async Task<string> Test()
        {
            Dictionary<string, string> query = new Dictionary<string, string>
            {
                { "components", "100" },
            };
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, QueryHelpers.AddQueryString($"Platform/Destiny2/3/Profile/{_membershipId}/", query));

            request.Headers.Add(AUTH_HEADER_KEY, AuthHeaderValue);

            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.Content == null)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
