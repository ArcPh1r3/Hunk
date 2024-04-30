using EntityStates.VoidRaidCrab;
using HunkMod.Modules.Components;
using HunkMod.Modules.Weapons;
using IL.RoR2.Items;
using R2API;
using RoR2;
using RoR2.Hologram;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace HunkMod.Modules
{
    public static class WeaponChest
    {
        internal static List<GameObject> interactables = new List<GameObject>();

        public static void AddInteractable(GameObject networkedObject)
        {
            interactables.Add(networkedObject);

            Debug.Log("Added " + networkedObject.name + " to Hunk interactable catalog");
        }

        public static GameObject interactableBodyModelPrefab;
        public static GameObject interactableModel => Assets.mainAssetBundle.LoadAsset<GameObject>("WeaponChest.prefab");

        public static CostTypeDef heartCostDef;
        public static int heartCostTypeIndex;

        public static string heartDisplayToken = MainPlugin.developerPrefix + "_HEARTWEAPONCHEST_DISPLAY";
        public static string heartContextToken = MainPlugin.developerPrefix + "_HEARTWEAPONCHEST_CONTEXT";
        public static string heartCostToken = MainPlugin.developerPrefix + "_HEARTCOSTTOKEN_NAME";

        public static void Initialize()
        {
            //CostTypeCatalog.modHelper.getAdditionalEntries += AddHeartCostType;
            CreateChest();
        }

        public static void CreateChest()
        {
            interactableBodyModelPrefab = interactableModel;
            interactableBodyModelPrefab.AddComponent<NetworkIdentity>();

            var purchaseInteraction = interactableBodyModelPrefab.AddComponent<PurchaseInteraction>();
            purchaseInteraction.displayNameToken = heartDisplayToken;
            purchaseInteraction.contextToken = heartContextToken;
            purchaseInteraction.costType = (CostTypeIndex)heartCostTypeIndex;
            purchaseInteraction.automaticallyScaleCostWithDifficulty = false;
            purchaseInteraction.cost = 1;
            purchaseInteraction.available = true;
            purchaseInteraction.setUnavailableOnTeleporterActivated = false;
            purchaseInteraction.isShrine = false;
            purchaseInteraction.isGoldShrine = false;

            //var pingInfoProvider = interactableBodyModelPrefab.AddComponent<PingInfoProvider>();
            //pingInfoProvider.pingIconOverride = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texHeartWeaponChest");

            var genericNameDisplay = interactableBodyModelPrefab.AddComponent<GenericDisplayNameProvider>();
            genericNameDisplay.displayToken = heartDisplayToken;

            var interactionComponent = interactableBodyModelPrefab.AddComponent<HeartChestInteractable>();
            interactionComponent.purchaseInteraction = purchaseInteraction;

            var entityLocator = interactableBodyModelPrefab.GetComponentInChildren<MeshCollider>().gameObject.AddComponent<EntityLocator>();
            entityLocator.entity = interactableBodyModelPrefab;

            var modelLoactor = interactableBodyModelPrefab.AddComponent<ModelLocator>();
            modelLoactor.modelTransform = interactableBodyModelPrefab.transform.Find("mdlWeaponChest");
            modelLoactor.modelBaseTransform = modelLoactor.modelTransform;
            modelLoactor.dontDetatchFromParent = true;
            modelLoactor.autoUpdateModelTransform = true;

            /*var highlightController = interactableBodyModelPrefab.GetComponent<Highlight>();
            highlightController.targetRenderer = interactableBodyModelPrefab.GetComponentInChildren<MeshRenderer>().Where(x => x.gameObject.name.Contains("WeaponChest")).First();
            highlightController.strength = 1;
            highlightController.highlightColor = Highlight.HighlightColor.interactive;*/

            var hologramController = interactableBodyModelPrefab.AddComponent<HologramProjector>();
            hologramController.hologramPivot = interactableBodyModelPrefab.transform.Find("HologramPivot");
            hologramController.displayDistance = 10;
            hologramController.disableHologramRotation = true;

            var childLocator = interactableBodyModelPrefab.AddComponent<ChildLocator>();
            childLocator.transformPairs = new ChildLocator.NameTransformPair[]
            {
                new ChildLocator.NameTransformPair()
                {
                    name = "FireworkOrigin",
                    transform = interactableBodyModelPrefab.transform.Find("FireworkEmitter")
                }
            };

            PrefabAPI.RegisterNetworkPrefab(interactableBodyModelPrefab);

            AddInteractable(interactableBodyModelPrefab);
        }

        private static void AddHeartCostType(List<CostTypeDef> list)
        {
            heartCostDef = new CostTypeDef();
            heartCostDef.costStringFormatToken = "";
            heartCostDef.isAffordable = new CostTypeDef.IsAffordableDelegate(Misc.ItemCostTypeHelperHeart.IsAffordable);
            heartCostDef.payCost = new CostTypeDef.PayCostDelegate(Misc.ItemCostTypeHelperHeart.PayCost);
            heartCostDef.colorIndex = ColorCatalog.ColorIndex.Blood;
            heartCostDef.saturateWorldStyledCostString = true;
            heartCostDef.darkenWorldStyledCostString = false;
            heartCostTypeIndex = CostTypeCatalog.costTypeDefs.Length + list.Count;
            list.Add(heartCostDef);
        }

        public class HeartChestInteractable : NetworkBehaviour
        {
            public CharacterBody lastActivator;
            public PurchaseInteraction purchaseInteraction;
            public Transform transform;
            public ItemDef weapon = M19.instance.itemDef;

            public void Start()
            {
                if (NetworkServer.active && Run.instance)
                {
                    purchaseInteraction.SetAvailableTrue();
                }
                purchaseInteraction.costType = (CostTypeIndex)heartCostTypeIndex;
                purchaseInteraction.onPurchase.AddListener(WeaponSpawn);
            }

            public void WeaponSpawn(Interactor interactor)
            {
                if (!interactor)
                    return;

                var body = interactor.GetComponent<CharacterBody>();
                if (body && body.master)
                {
                    if (NetworkServer.active)
                    {
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(weapon.itemIndex), this.transform.position, (Vector3.up * 20f) + (5 * Vector3.right * Mathf.Cos(2f * Mathf.PI / Run.instance.participatingPlayerCount)) + (5 * Vector3.forward * Mathf.Sin(2f * Mathf.PI / Run.instance.participatingPlayerCount)));

                        lastActivator = body;

                        purchaseInteraction.SetAvailable(false);
                    }
                }
            }
        }

        private static class ItemCostTypeHelperSpade
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
                int itemCount = inv.GetItemCount(RoR2Content.Items.Dagger);

                if (itemCount >= cost)
                    return true;
                else
                    return false;
            }

            public static void PayCost()
            {
            }
        }

        private static class ItemCostTypeHelperDiamond
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
                int itemCount = inv.GetItemCount(RoR2Content.Items.CritGlasses);

                if (itemCount >= cost)
                    return true;
                else
                    return false;
            }

            public static void PayCost()
            {
            }
        }

        private static class ItemCostTypeHelperClub
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
                int itemCount = inv.GetItemCount(RoR2Content.Items.Hoof);

                if (itemCount >= cost)
                    return true;
                else
                    return false;
            }

            public static void PayCost()
            {
            }
        }
    }
}