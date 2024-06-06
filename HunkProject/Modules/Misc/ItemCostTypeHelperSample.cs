using RoR2;

namespace HunkMod.Modules.Misc
{
    internal class ItemCostTypeHelperSample
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
            int itemCount = inv.GetItemCount(Modules.Survivors.Hunk.gVirusSample) + inv.GetItemCount(Modules.Survivors.Hunk.tVirusSample) + +inv.GetItemCount(Modules.Survivors.Hunk.cVirusSample);

            if (itemCount >= cost)
                return true;
            else
                return false;
        }

        public static void PayCost(CostTypeDef costTypeDef, CostTypeDef.PayCostContext context)
        {
            if (context.activatorMaster.inventory.GetItemCount(Modules.Survivors.Hunk.gVirusSample) > 0)
            {
                context.activatorMaster.inventory.RemoveItem(Modules.Survivors.Hunk.gVirusSample);
                Modules.Helpers.CreateItemTakenOrb(context.activatorBody.corePosition, context.purchasedObject, Modules.Survivors.Hunk.gVirusSample.itemIndex);
                return;
            }

            if (context.activatorMaster.inventory.GetItemCount(Modules.Survivors.Hunk.tVirusSample) > 0)
            {
                context.activatorMaster.inventory.RemoveItem(Modules.Survivors.Hunk.tVirusSample);
                Modules.Helpers.CreateItemTakenOrb(context.activatorBody.corePosition, context.purchasedObject, Modules.Survivors.Hunk.tVirusSample.itemIndex);
                return;
            }

            if (context.activatorMaster.inventory.GetItemCount(Modules.Survivors.Hunk.cVirusSample) > 0)
            {
                context.activatorMaster.inventory.RemoveItem(Modules.Survivors.Hunk.cVirusSample);
                Modules.Helpers.CreateItemTakenOrb(context.activatorBody.corePosition, context.purchasedObject, Modules.Survivors.Hunk.cVirusSample.itemIndex);
                return;
            }
        }
    }
}