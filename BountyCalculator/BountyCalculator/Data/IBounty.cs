using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BountyCalculator.Data
{
    enum Location
    {
        None,
        EDZ,
        Titan,
        Nessus,
        Io,
        Mercury,
        Mars,
        TangledShore,
        DreamingCity,
        Moon,
        NotMoon,
        Any
    }

    enum Activity
    {
        Strike,
        Crucible,
        Gambit,
        NightmareHunt,
        Any
    }

    interface IBounty
    {
        string Name { get; set; }

        Location Location { get; set; }

        Activity Activity { get; set; }

        ITask Task { get; set; }
    }
}
