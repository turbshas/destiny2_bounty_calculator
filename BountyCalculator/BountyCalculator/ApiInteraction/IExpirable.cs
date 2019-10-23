using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BountyCalculator.ApiInteraction
{
    interface IExpirable
    {
        event EventHandler<EventArgs> Expired;

        bool IsExpired { get; }
        int ExpirationTime { get; }

        void RestartTimer();
        void StopTimer();
        void SetTimer(int timeInMilliseconds);
    }
}
