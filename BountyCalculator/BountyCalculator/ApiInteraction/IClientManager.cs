using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BountyCalculator.ApiInteraction
{
    interface IClientManager
    {
        ApiClient GetApi(string clientGuid);
        void AddApiClient(ApiClient client);
        ApiClient RemoveApiClient(string clientGuid);
    }
}
