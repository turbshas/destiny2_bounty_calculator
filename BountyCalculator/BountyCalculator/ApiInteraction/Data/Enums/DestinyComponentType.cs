using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BountyCalculator.ApiInteraction.Data.Enums
{
    enum DestinyComponentType
    {
        None = 0,

        Profiles = 100,
        VendorReceipts,
        ProfileInventories,
        ProfileCurrencies,
        ProfleProgression,
        PlatformSilver,

        Characters = 200,
        CharacterInventories,
        CharacterProgressions,
        CharacterRenderData,
        CharacterActivities,
        CharacterEquipment,

        ItemInstances = 300,
        ItemObjectives,
        ItemPerks,
        ItemRenderData,
        ItemStats,
        ItemSockets,
        ItemTalentGrids,
        ItemCommonData,
        ItemPlugStates,

        Vendors = 400,
        VendorCategories,
        VendorSales,
        
        Kiosks = 500,
        CurrencyLookups = 600,
        PresentationNodes = 700,
        Collectibles = 800,
        Records = 900,
        Transitory = 1000,
    }
}
