using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BountyCalculator.ApiInteraction
{
    class AuthorizationManager : IAuthorizationManager, IClientManager
    {
        static AuthorizationManager()
        {
            Instance = new AuthorizationManager();
        }

        public static AuthorizationManager Instance { get; private set; }

        private const int REQUEST_TIMEOUT_MILLISECONDS = 10 * 1000;

        private readonly ConcurrentDictionary<string, ExpirationWrapper<RequestGuid>> _authRequests;
        private readonly ConcurrentDictionary<string, ApiClient> _apiClients;

        private AuthorizationManager()
        {
            _authRequests = new ConcurrentDictionary<string, ExpirationWrapper<RequestGuid>>();
            _apiClients = new ConcurrentDictionary<string, ApiClient>();
        }

        #region [ Request guid handling ]

        private void RequestExpiredHandler(object sender, EventArgs e)
        {
            if (! (sender is ExpirationWrapper<RequestGuid>))
            {
                /* Shouldn't happen */
                return;
            }

            ExpirationWrapper<RequestGuid> wrapper = (ExpirationWrapper<RequestGuid>)sender;
            if (!_authRequests.TryRemove(wrapper.ToString(), out wrapper))
            {
                /* Request was already removed, ignore */
                return;
            }
        }

        public RequestGuid GetRequestGuid(string guidString)
        {
            if (!_authRequests.TryGetValue(guidString, out ExpirationWrapper<RequestGuid> wrapper))
            {
                throw new KeyNotFoundException("Could not retrieve request guid. Guid either does not exist or has expired.");
            }

            return wrapper.Value;
        }

        public void AddRequestGuid(RequestGuid requestGuid)
        {
            ExpirationWrapper<RequestGuid> wrapper = new ExpirationWrapper<RequestGuid>(requestGuid, REQUEST_TIMEOUT_MILLISECONDS);
            wrapper.Expired += RequestExpiredHandler;
            wrapper.RestartTimer();
            if (!_authRequests.TryAdd(requestGuid.ToString(), wrapper))
            {
                throw new InvalidOperationException("Request already exists.");
            }
        }

        public RequestGuid RemoveRequestGuid(string guidString)
        {
            if (!_authRequests.TryRemove(guidString, out ExpirationWrapper<RequestGuid> wrapper))
            {
                throw new KeyNotFoundException("Request does not exist.");
            }

            wrapper.StopTimer();
            return wrapper.Value;
        }

        #endregion

        #region [ Client storage ]

        private void TokenExpiredHandler(object sender, EventArgs e)
        {
            if (! (sender is ApiClient))
            {
                /* Shouldn't happen */
                return;
            }

            ApiClient client = (ApiClient)sender;
            if (!_apiClients.TryRemove(client.Guid, out client))
            {
                /* Client was already removed, ignore */
                return;
            }
        }

        public ApiClient GetApi(string clientGuid)
        {
            if (!_apiClients.TryGetValue(clientGuid, out ApiClient client))
            {
                throw new KeyNotFoundException("Could not retrieve API client. Client either does not exist or has expired.");
            }

            return client;
        }

        public void AddApiClient(ApiClient client)
        {
            client.TokenExpired += TokenExpiredHandler;
            if (!_apiClients.TryAdd(client.Guid, client))
            {
                throw new InvalidOperationException("Client already exists.");
            }
        }

        public ApiClient RemoveApiClient(string clientGuid)
        {
            if (!_apiClients.TryRemove(clientGuid, out ApiClient client))
            {
                throw new KeyNotFoundException("Client does not exist.");
            }

            return client;
        }

        #endregion
    }
}
