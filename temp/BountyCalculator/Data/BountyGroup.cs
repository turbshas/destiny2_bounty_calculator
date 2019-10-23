using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BountyCalculator.Data
{
    class BountyGroup : IBounty
    {
        private Bounty _leader;
        private List<Bounty> _subBounties;

        public BountyGroup(Bounty leader)
        {
            _leader = leader;
            _subBounties = new List<Bounty>();
        }

        public BountyGroup(Bounty leader, Bounty subBounty)
        {
            _leader = leader;
            _subBounties = new List<Bounty> { subBounty };
        }

        public string Name
        {
            get
            {
                return _leader.Name;
            }
            set
            {
                _leader.Name = value;
            }
        }

        public Location Location
        {
            get
            {
                return _leader.Location;
            }
            set
            {
                _leader.Location = value;
            }
        }

        public Activity Activity
        {
            get
            {
                return _leader.Activity;
            }
            set
            {
                _leader.Activity = value;
            }
        }

        public ITask Task
        {
            get
            {
                return _leader.Task;
            }
            set
            {
                _leader.Task = value;
            }
        }

        public void AddBounty(Bounty bounty)
        {
            _subBounties.Add(bounty);
        }

        public override string ToString()
        {
            return _leader.ToString();
        }
    }
}
