using RoR2;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace HunkMod.Modules.Components
{
    internal class Terminal : NetworkBehaviour
    {
        public ItemDef itemDef;

        public ChestBehavior chestBehavior;
        public PurchaseInteraction purchaseInteraction;
        public GenericDisplayNameProvider genericDisplayNameProvider;

        private void Awake()
        {
            purchaseInteraction = this.GetComponent<PurchaseInteraction>();
            genericDisplayNameProvider = this.GetComponent<GenericDisplayNameProvider>();

            if (purchaseInteraction != null)
            {
                purchaseInteraction.costType = (CostTypeIndex)Survivors.Hunk.sampleCostTypeIndex;
                purchaseInteraction.cost = 1;
                purchaseInteraction.displayNameToken = MainPlugin.developerPrefix + "_HUNK_TERMINAL_NAME";
                purchaseInteraction.contextToken = MainPlugin.developerPrefix + "_HUNK_TERMINAL_CONTEXT";
                if (genericDisplayNameProvider != null)
                {
                    genericDisplayNameProvider.displayToken = MainPlugin.developerPrefix + "_HUNK_TERMINAL_NAME";
                }
            }
        }
    }
}