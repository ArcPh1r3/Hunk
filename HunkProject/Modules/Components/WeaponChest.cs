using HarmonyLib;
using HunkMod.Modules.Weapons;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace HunkMod.Modules.Components
{
    internal class WeaponChest : NetworkBehaviour
    {
        public ChestBehavior chestBehavior;
        public PurchaseInteraction purchaseInteraction;
        public GenericDisplayNameProvider genericDisplayNameProvider;
        private int chestType;

        private void Awake()
        {
            System.Random random = new System.Random();
            chestType = random.Next(1, 5);

            chestBehavior = this.GetComponent<ChestBehavior>();
            if (chestBehavior != null)
            {
                //chestBehavior.dropPickup = PickupCatalog.FindPickupIndex(M19.instance.itemDef.itemIndex);
                //why not?
            }

            purchaseInteraction = this.GetComponent<PurchaseInteraction>();
            genericDisplayNameProvider = this.GetComponent<GenericDisplayNameProvider>();
            
            if (purchaseInteraction != null)
            {
                switch (chestType)
                {
                    case 1:
                        purchaseInteraction.costType = (CostTypeIndex)Survivors.Hunk.heartCostTypeIndex;
                        purchaseInteraction.cost = 1;
                        purchaseInteraction.displayNameToken = MainPlugin.developerPrefix + "_HUNK_HEARTCHEST_NAME";
                        purchaseInteraction.contextToken = MainPlugin.developerPrefix + "_HUNK_HEARTCHEST_CONTEXT";
                        if (genericDisplayNameProvider != null)
                        {
                            genericDisplayNameProvider.displayToken = MainPlugin.developerPrefix + "_HUNK_HEARTCHEST_NAME";
                        }
                        break;
                    case 2:
                        purchaseInteraction.costType = (CostTypeIndex)Survivors.Hunk.spadeCostTypeIndex;
                        purchaseInteraction.cost = 1;
                        purchaseInteraction.displayNameToken = MainPlugin.developerPrefix + "_HUNK_SPADECHEST_NAME";
                        purchaseInteraction.contextToken = MainPlugin.developerPrefix + "_HUNK_SPADECHEST_CONTEXT";
                        if (genericDisplayNameProvider != null)
                        {
                            genericDisplayNameProvider.displayToken = MainPlugin.developerPrefix + "_HUNK_SPADECHEST_NAME";
                        }
                        break;
                    case 3:
                        purchaseInteraction.costType = (CostTypeIndex)Survivors.Hunk.diamondCostTypeIndex;
                        purchaseInteraction.cost = 1;
                        purchaseInteraction.displayNameToken = MainPlugin.developerPrefix + "_HUNK_DIAMONDCHEST_NAME";
                        purchaseInteraction.contextToken = MainPlugin.developerPrefix + "_HUNK_DIAMONDCHEST_CONTEXT";
                        if (genericDisplayNameProvider != null)
                        {
                            genericDisplayNameProvider.displayToken = MainPlugin.developerPrefix + "_HUNK_DIAMONDCHEST_NAME";
                        }
                        break;
                    case 4:
                        purchaseInteraction.costType = (CostTypeIndex)Survivors.Hunk.clubCostTypeIndex;
                        purchaseInteraction.cost = 1;
                        purchaseInteraction.displayNameToken = MainPlugin.developerPrefix + "_HUNK_CLUBCHEST_NAME";
                        purchaseInteraction.contextToken = MainPlugin.developerPrefix + "_HUNK_CLUBCHEST_CONTEXT";
                        if (genericDisplayNameProvider != null)
                        {
                            genericDisplayNameProvider.displayToken = MainPlugin.developerPrefix + "_HUNK_CLUBCHEST_NAME";
                        }
                        break;
                    default:
                        purchaseInteraction.costType = CostTypeIndex.Money;
                        purchaseInteraction.cost = 0;
                        purchaseInteraction.displayNameToken = MainPlugin.developerPrefix + "_HUNK_CHEST_NAME";
                        purchaseInteraction.contextToken = MainPlugin.developerPrefix + "_HUNK_CHEST_CONTEXT";
                        if (genericDisplayNameProvider != null)
                        {
                            genericDisplayNameProvider.displayToken = MainPlugin.developerPrefix + "_HUNK_CHEST_NAME";
                        }
                        break;
                }


                purchaseInteraction.costType = (CostTypeIndex)Survivors.Hunk.heartCostTypeIndex;
                purchaseInteraction.cost = 1;
            }
        }
    }
}
