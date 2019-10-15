using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BountyCalculator.Data
{
    class Bounty : IBounty
    {
        public string Name { get; set; }

        public Location Location { get; set; }

        public Activity Activity { get; set; }

        public ITask Task { get; set; }

        public override string ToString()
        {
            return $"{Task} in {Activity} on {Location}";
        }
    }
}
