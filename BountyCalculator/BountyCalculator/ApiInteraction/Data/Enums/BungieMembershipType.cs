using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BountyCalculator.ApiInteraction.Data.Enums
{
    enum BungieMembershipType
    {
        None = 0,
        TigerXbox,
        TigerPsn,
        TigerSteam,
        TigerBlizzard,
        TigerStadia,

        TigerDemon = 10,
        BungieNext = 254,
        All = -1, /* Only allowed for searches */
    }
}
