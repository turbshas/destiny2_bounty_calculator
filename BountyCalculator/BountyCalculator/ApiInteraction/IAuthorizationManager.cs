using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BountyCalculator.ApiInteraction
{
    interface IAuthorizationManager
    {
        RequestGuid GetRequestGuid(string guidString);
        void AddRequestGuid(RequestGuid requestGuid);
        RequestGuid RemoveRequestGuid(string guidString);
    }
}
