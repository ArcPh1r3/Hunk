using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace HunkMod.Modules.Misc
{
    internal class ItemCostTypeHelperSpade
    {
        public static bool IsAffordable(CostTypeDef costTypeDef, CostTypeDef.IsAffordableContext context)
        {
            CharacterBody cb = context.activator.GetComponent<CharacterBody>();
            if (!cb)
                return false;

            Inventory inv = cb.inventory;
            if (!inv)
                return false;

            int cost = context.cost;
            int itemCount = inv.GetItemCount(Modules.Survivors.Hunk.spadeKeycard);

            if (itemCount >= cost)
                return true;
            else
                return false;
        }

        public static void PayCost(CostTypeDef costTypeDef, CostTypeDef.PayCostContext context)
        {
        }
    }
}
