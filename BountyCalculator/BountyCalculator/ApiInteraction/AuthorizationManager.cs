using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BountyCalculator.ApiInteraction
{
    public class AuthorizationManager : IAuthorizationManager
    {
        static AuthorizationManager()
        {
            Instance = new AuthorizationManager();
        }

        public static AuthorizationManager Instance { get; private set; }

        private const int REQUEST_TIMEOUT_MILLISECONDS = 10 * 1000;

        private readonly ConcurrentDictionary<string, ExpirationWrapper<RequestGuid>> _authRequests;

        private AuthorizationManager()
        {
            _authRequests = new ConcurrentDictionary<string, ExpirationWrapper<RequestGuid>>();
        }

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
            if (_authRequests.TryGetValue(guidString, out ExpirationWrapper<RequestGuid> wrapper))
            {
                return wrapper.Value;
            }

            return null;
        }

        public void AddRequestGuid(RequestGuid requestGuid)
        {
            ExpirationWrapper<RequestGuid> wrapper = new ExpirationWrapper<RequestGuid>(requestGuid, REQUEST_TIMEOUT_MILLISECONDS);
            wrapper.Expired += RequestExpiredHandler;
            wrapper.RestartTimer();
            if (!_authRequests.TryAdd(requestGuid.ToString(), wrapper))
            {
                //TODO: create custom exception for this
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
    }
}
