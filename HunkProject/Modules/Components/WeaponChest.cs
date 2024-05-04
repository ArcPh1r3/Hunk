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
        public HunkGunPickup gunPickup;
        //public ChestBehavior chestBehavior;
        public PurchaseInteraction purchaseInteraction;
        public GenericDisplayNameProvider genericDisplayNameProvider;
        private int chestType;

        private void InitPickup()
        {
            HunkWeaponDef weaponDef = Modules.Weapons.MUP.instance.weaponDef;

            // im just gonna hard code this rn okay.
            // just get it working and move on. it can be cleaned up later.
            if (Modules.Helpers.HunkHasWeapon(Modules.Weapons.Shotgun.instance.weaponDef) || Modules.Helpers.HunkHasWeapon(Modules.Weapons.Slugger.instance.weaponDef))
            {
                if (Modules.Helpers.HunkHasWeapon(Modules.Weapons.Magnum.instance.weaponDef) || Modules.Helpers.HunkHasWeapon(Modules.Weapons.Revolver.instance.weaponDef))
                {
                    // start giving the rest of the unowned weapons
                    List<HunkWeaponDef> weaponPool = new List<HunkWeaponDef>();
                    weaponPool.Add(Modules.Weapons.Shotgun.instance.weaponDef);
                    weaponPool.Add(Modules.Weapons.Slugger.instance.weaponDef);
                    weaponPool.Add(Modules.Weapons.Magnum.instance.weaponDef);
                    weaponPool.Add(Modules.Weapons.Revolver.instance.weaponDef);

                    bool foundWeapon = false;
                    foreach (HunkWeaponDef i in weaponPool)
                    {
                        if (!Modules.Helpers.HunkHasWeapon(i))
                        {
                            weaponDef = i;
                            foundWeapon = true;
                            break;
                        }
                    }

                    if (!foundWeapon)
                    {
                        // destroy this if no more weanpos are vailbnelne
                        //Destroy(this.gameObject);
                    }
                }
                else
                {
                    if (MainPlugin.badaBingBadaBoom) weaponDef = Modules.Weapons.Magnum.instance.weaponDef;
                    else weaponDef = Modules.Weapons.Revolver.instance.weaponDef;

                    MainPlugin.badaBingBadaBoom = !MainPlugin.badaBingBadaBoom;
                }
            }
            else
            {
                if (MainPlugin.badaBingBadaBoom) weaponDef = Modules.Weapons.Shotgun.instance.weaponDef;
                else weaponDef = Modules.Weapons.Slugger.instance.weaponDef;

                MainPlugin.badaBingBadaBoom = !MainPlugin.badaBingBadaBoom;
            }

            gunPickup.weaponDef = weaponDef;

            // offset the shotgun a little
            // i hate how horrible this is but it works
            if (weaponDef.nameToken.Contains("SHOTGUN"))
            {
                var h = GetComponent<Highlight>();
                Transform gunTransform = h.targetRenderer.transform.parent.parent.Find("WeaponHolder");
                gunTransform.localPosition = new Vector3(-0.5f, 2.98f, 0.87f);
                gunTransform.localScale = Vector3.one * 1.25f;
            }

            gunPickup.Init();
        }

        private void Awake()
        {
            gunPickup = GetComponentInChildren<HunkGunPickup>();
            gunPickup.GetComponent<GenericPickupController>().enabled = false;

            // weapondef and shit
            Invoke("InitPickup", 1f); // delay it to give time for the player to spawn in, just as a safety measure

            System.Random random = new System.Random();
            chestType = random.Next(1, 5);

            /*chestBehavior = this.GetComponent<ChestBehavior>();
            if (chestBehavior != null)
            {
                //chestBehavior.dropPickup = PickupCatalog.FindPickupIndex(M19.instance.itemDef.itemIndex);
                //why not?
            }*/

            purchaseInteraction = this.GetComponent<PurchaseInteraction>();
            genericDisplayNameProvider = this.GetComponent<GenericDisplayNameProvider>();

            var h = GetComponent<Highlight>();
            h.targetRenderer.transform.parent.Find("Hinge/Lock/OnLight").gameObject.SetActive(false);

            if (Run.instance.stageClearCount <= 0) chestType = 2;

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
                        h.targetRenderer.transform.parent.Find("Hinge/Lock/Spade").gameObject.SetActive(false);
                        h.targetRenderer.transform.parent.Find("Hinge/Lock/Club").gameObject.SetActive(false);
                        h.targetRenderer.transform.parent.Find("Hinge/Lock/Heart").gameObject.SetActive(true);
                        h.targetRenderer.transform.parent.Find("Hinge/Lock/Diamond").gameObject.SetActive(false);
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
                        h.targetRenderer.transform.parent.Find("Hinge/Lock/Spade").gameObject.SetActive(true);
                        h.targetRenderer.transform.parent.Find("Hinge/Lock/Club").gameObject.SetActive(false);
                        h.targetRenderer.transform.parent.Find("Hinge/Lock/Heart").gameObject.SetActive(false);
                        h.targetRenderer.transform.parent.Find("Hinge/Lock/Diamond").gameObject.SetActive(false);
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
                        h.targetRenderer.transform.parent.Find("Hinge/Lock/Spade").gameObject.SetActive(false);
                        h.targetRenderer.transform.parent.Find("Hinge/Lock/Club").gameObject.SetActive(false);
                        h.targetRenderer.transform.parent.Find("Hinge/Lock/Heart").gameObject.SetActive(false);
                        h.targetRenderer.transform.parent.Find("Hinge/Lock/Diamond").gameObject.SetActive(true);
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
                        h.targetRenderer.transform.parent.Find("Hinge/Lock/Spade").gameObject.SetActive(false);
                        h.targetRenderer.transform.parent.Find("Hinge/Lock/Club").gameObject.SetActive(true);
                        h.targetRenderer.transform.parent.Find("Hinge/Lock/Heart").gameObject.SetActive(false);
                        h.targetRenderer.transform.parent.Find("Hinge/Lock/Diamond").gameObject.SetActive(false);
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
                        h.targetRenderer.transform.parent.Find("Hinge/Lock").gameObject.SetActive(false);
                        break;
                }
            }
        }
    }
}