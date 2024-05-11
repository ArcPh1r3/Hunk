using HunkMod.Modules.Weapons;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HunkMod.Modules.Components
{
    internal class WeaponChest : NetworkBehaviour
    {
        public HunkGunPickup gunPickup;
        //public ChestBehavior chestBehavior;
        public PurchaseInteraction purchaseInteraction;
        public PingInfoProvider pingInfoProvider;
        public GenericDisplayNameProvider genericDisplayNameProvider;
        public HunkWeaponDef weaponDef;
        public int chestType;

        private void InitPickup()
        {
            this.weaponDef = Modules.Weapons.MUP.instance.weaponDef;
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            bool fuck = false;
            if (sceneName == "goldshores") fuck = true;
            if (sceneName == "mysteryspace") fuck = true;
            if (sceneName == "moon2") fuck = true;

                // im just gonna hard code this rn okay.
                // just get it working and move on. it can be cleaned up later.
            if (Modules.Helpers.HunkHasWeapon(Modules.Weapons.Shotgun.instance.weaponDef) || Modules.Helpers.HunkHasWeapon(Modules.Weapons.Slugger.instance.weaponDef))
            {
                if (Modules.Helpers.HunkHasWeapon(Modules.Weapons.Magnum.instance.weaponDef) || Modules.Helpers.HunkHasWeapon(Modules.Weapons.Revolver.instance.weaponDef))
                {
                    if (Modules.Helpers.HunkHasWeapon(Modules.Weapons.Flamethrower.instance.weaponDef))
                    {
                        // start giving the rest of the unowned weapons
                        List<HunkWeaponDef> weaponPool = new List<HunkWeaponDef>();
                        weaponPool.Add(Modules.Weapons.Shotgun.instance.weaponDef);
                        weaponPool.Add(Modules.Weapons.Slugger.instance.weaponDef);
                        weaponPool.Add(Modules.Weapons.Magnum.instance.weaponDef);
                        weaponPool.Add(Modules.Weapons.Revolver.instance.weaponDef);
                        weaponPool.Add(Modules.Weapons.Flamethrower.instance.weaponDef);
                        weaponPool.Add(Modules.Weapons.GrenadeLauncher.instance.weaponDef);

                        bool foundWeapon = false;
                        foreach (HunkWeaponDef i in weaponPool)
                        {
                            if (!Modules.Helpers.HunkHasWeapon(i) && !Modules.Survivors.Hunk.spawnedWeaponList.Contains(i))
                            {
                                Modules.Survivors.Hunk.spawnedWeaponList.Add(i);
                                weaponDef = i;
                                foundWeapon = true;
                                break;
                            }
                        }

                        if (!foundWeapon && !fuck)
                        {
                            // destroy this if no more weanpos are vailbnelne
                            Destroy(this.gameObject);
                        }
                    }
                    else
                    {
                        if (MainPlugin.badaBingBadaBoom) weaponDef = Modules.Weapons.Flamethrower.instance.weaponDef;
                        else weaponDef = Modules.Weapons.GrenadeLauncher.instance.weaponDef;

                        MainPlugin.badaBingBadaBoom = !MainPlugin.badaBingBadaBoom;
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

            if (sceneName == "goldshores") weaponDef = Modules.Weapons.GoldenGun.instance.weaponDef;
            if (sceneName == "mysteryspace") weaponDef = Modules.Weapons.BlueRose.instance.weaponDef;
            if (sceneName == "moon2") weaponDef = RocketLauncher.instance.weaponDef;

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

            if (weaponDef.nameToken.Contains("SLUGGER"))
            {
                var h = GetComponent<Highlight>();
                Transform gunTransform = h.targetRenderer.transform.parent.parent.Find("WeaponHolder");
                gunTransform.localPosition = new Vector3(0.6f, 2.98f, -0.16f);
            }

            if (purchaseInteraction != null)
            {
                purchaseInteraction.displayNameToken = string.Format("{0}{1}", Language.GetStringFormatted(MainPlugin.developerPrefix + "_HUNK_CHEST_NAME"), Language.GetStringFormatted(weaponDef.nameToken));
                purchaseInteraction.contextToken = MainPlugin.developerPrefix + "_HUNK_CHEST_CONTEXT";

                if (sceneName == "moon2") purchaseInteraction.costType = CostTypeIndex.None;
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
            pingInfoProvider = this.GetComponent<PingInfoProvider>();

            var h = GetComponent<Highlight>();
            if (h.targetRenderer.transform.parent.Find("Hinge")) h.targetRenderer.transform.parent.Find("Hinge/Lock/OnLight").gameObject.SetActive(false);

            if (Run.instance.stageClearCount <= 0 || UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "goldshores") chestType = 2;
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "moon2") chestType = 0;


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
                        if (pingInfoProvider != null)
                            pingInfoProvider.pingIconOverride = Assets.mainAssetBundle.LoadAsset<Sprite>("texIconHeart");
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
                        if (pingInfoProvider != null)
                            pingInfoProvider.pingIconOverride = Assets.mainAssetBundle.LoadAsset<Sprite>("texIconSpade");
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
                        if (pingInfoProvider != null)
                            pingInfoProvider.pingIconOverride = Assets.mainAssetBundle.LoadAsset<Sprite>("texIconDiamond");
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
                        if (pingInfoProvider != null)
                            pingInfoProvider.pingIconOverride = Assets.mainAssetBundle.LoadAsset<Sprite>("texIconClub");
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