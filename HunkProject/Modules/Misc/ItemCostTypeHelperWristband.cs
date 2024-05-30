﻿using RoR2;

namespace HunkMod.Modules.Misc
{
    internal class ItemCostTypeHelperWristband
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
            int itemCount = inv.GetItemCount(Modules.Survivors.Hunk.wristband);

            if (inv.GetItemCount(Modules.Survivors.Hunk.masterKeycard) > 0) return true;

            if (itemCount >= cost)
                return true;
            else
            {
                Modules.Components.HunkController hunk = cb.GetComponent<Modules.Components.HunkController>();
                if (hunk)
                {
                    hunk.notificationHandler.SoftInit("You need an ID Wristband to open this", UnityEngine.Color.red, 2f);
                }
                return false;
            }
        }

        public static void PayCost(CostTypeDef costTypeDef, CostTypeDef.PayCostContext context)
        {
        }
    }
}