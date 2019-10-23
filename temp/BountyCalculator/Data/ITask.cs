using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BountyCalculator.Data
{
    enum TaskType
    {
        Patrols,
        Adventures,
        Strikes,
        CrucibleMatches,
        GambitMatches,
        Chests,
        Materials,
        PublicEvents,
        Kills,
    }

    interface ITask
    {
        TaskType TaskType { get; set; }

        int Amount { get; set; }

        KillsTaskInfo TaskInfo { get; set; }
    }
}
