using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BountyCalculator.Data
{
    enum Ability
    {
        None,
        Class,
        Melee,
        Grenade,
        Super,
    }

    enum Element
    {
        Arc,
        Solar,
        Void,
        Any
    }

    enum Enemy
    {
        Fallen,
        Hive,
        Vex,
        Cabal,
        Taken,
        Scorn,
        Any
    }

    enum Weapon
    {
        AutoRifle,
        FusionRifle,
        GrenadeLauncher,
        HandCannon,
        LMG,
        PulseRifle,
        RocketLauncher,
        ScoutRifle,
        Shotgun,
        SMG,
        SniperRifle,
        Sword,
        TraceRifle,
        Any
    }

    enum WeaponSlot
    {
        Kinetic,
        Energy,
        Power,
        Any
    }

    class KillsTaskInfo
    {
        public Ability Ability { get; set; }

        public Element Element { get; set; }

        public Enemy Enemy { get; set; }

        public bool IsMultikills { get; set; }

        public bool IsPrecision { get; set; }

        public Weapon Weapon { get; set; }

        public WeaponSlot WeaponSlot { get; set; }
    }
}
