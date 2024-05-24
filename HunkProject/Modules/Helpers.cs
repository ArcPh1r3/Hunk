using HunkMod.Modules.Components;
using HunkMod.Modules.Survivors;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace HunkMod.Modules
{
    internal static class Helpers
    {
        internal const string agilePrefix = "<style=cIsUtility>Agile.</style> ";

        internal const string whiteItemHex = "00FF66";
        internal const string greenItemHex = "00FF66";
        internal const string redItemHex = "FF0033";
        internal const string yellowItemHex = "FFFF00";
        internal const string lunarItemHex = "0066FF";
        internal const string voidItemHex = "C678B4";
        internal const string colorSuffix = "</color>";

        internal static Color whiteItemColor = new Color(1f, 1f, 1f);
        internal static Color greenItemColor = new Color(0f, 1f, 102f / 255f);
        internal static Color redItemColor = new Color(1f, 0f, 51f / 255f);
        internal static Color yellowItemColor = new Color(1f, 1f, 0f);
        internal static Color lunarItemColor = new Color(0f, 102f / 255f, 1f);
        internal static Color voidItemColor = new Color(198f / 255f, 120f / 255f, 180f / 255f);
        internal static Color badColor = new Color(127f / 255f, 0f, 0f);

        internal static string ScepterDescription(string desc)
        {
            return "\n<color=#d299ff>SCEPTER: " + desc + "</color>";
        }

        public static void CreateAmmoPickup(Vector3 origin)
        {
            CreateAmmoPickup(origin, Quaternion.identity);
        }

        public static void CreateAmmoPickup(Vector3 origin, Quaternion rotation)
        {
            if (!Helpers.isHunkInPlay) return;

            if (RoR2Application.isInSinglePlayer)
            {
                if (Helpers.isLoomingDreadActive) return;
            }

            if (Modules.Config.originalAmmoPickups.Value)
            {
                NetworkServer.Spawn(GameObject.Instantiate(Hunk.instance.ammoPickupInteractable, origin, rotation));
            }
            else
            {
                float xSpread = Random.Range(-1.4f, 1.4f);
                float ySpread = Random.Range(-1.4f, 1.4f);

                PickupDropletController.CreatePickupDroplet(
                    PickupCatalog.FindPickupIndex(Hunk.ammoItem.itemIndex),
                    origin + Vector3.up,
                    Vector3.up * Random.Range(15f, 35f) + new Vector3(xSpread, 0f, ySpread));
            }
        }

        [Server]
        public static void CreateItemTakenOrb(Vector3 effectOrigin, GameObject targetObject, ItemIndex itemIndex)
        {
            if (!NetworkServer.active) return;

            GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/ItemTakenOrbEffect.prefab").WaitForCompletion();
            EffectData effectData = new EffectData
            {
                origin = effectOrigin,
                genericFloat = 0.75f,
                genericUInt = (uint)(itemIndex + 1)
            };
            effectData.SetNetworkedObjectReference(targetObject);
            EffectManager.SpawnEffect(effectPrefab, effectData, true);
        }

        public static bool isHunkInPlay
        {
            get
            {
                foreach (var player in PlayerCharacterMasterController.instances)
                {
                    if (player.networkUser.bodyIndexPreference == BodyCatalog.FindBodyIndex(Modules.Survivors.Hunk.bodyName))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public static bool isLoomingDreadActive
        {
            get
            {
                foreach (var player in PlayerCharacterMasterController.instances)
                {
                    if (player.networkUser.bodyIndexPreference == BodyCatalog.FindBodyIndex(Modules.Survivors.Hunk.bodyName))
                    {
                        if (player.body)
                        {
                            HunkController hunk = player.body.GetComponent<HunkController>();
                            if (hunk)
                            {
                                if (hunk.passive)
                                {
                                    if (hunk.passive.isFullArsenal) return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
        }

        public static bool isLocalUserHunk
        {
            get
            {
                var localPlayers = LocalUserManager.readOnlyLocalUsersList;
                foreach (LocalUser i in localPlayers)
                {
                    if (i.cachedBody.baseNameToken == Modules.Survivors.Hunk.bodyNameToken && i.cachedBody.hasAuthority) return true;
                }
                return false;
            }
        }

        public static int hunkCount
        {
            get
            {
                int i = 0;
                foreach (var player in PlayerCharacterMasterController.instances)
                {
                    if (player.networkUser.bodyIndexPreference == BodyCatalog.FindBodyIndex(Modules.Survivors.Hunk.bodyName))
                    {
                        i++;
                    }
                }
                return i;
            }
        }

        public static bool HunkHasWeapon(HunkWeaponDef weaponDef)
        {
            foreach (HunkWeaponTracker hunk in GameObject.FindObjectsOfType<HunkWeaponTracker>())
            {
                if (!hunk.ignoreFlag)
                {
                    foreach (HunkWeaponData i in hunk.weaponData)
                    {
                        if (i.weaponDef == weaponDef) return true;
                    }
                }
            }

            return false;
        }

        public static bool HunkHasWeapon(ItemDef itemDef)
        {
            foreach (HunkWeaponTracker hunk in GameObject.FindObjectsOfType<HunkWeaponTracker>())
            {
                if (!hunk.ignoreFlag)
                {
                    if (hunk.GetComponent<Inventory>().GetItemCount(itemDef) > 0) return true;
                }
            }

            return false;
        }

        public static bool HunkHasWeapon(HunkWeaponDef weaponDef, HunkController hunk)
        {
            return HunkHasWeapon(weaponDef, hunk.weaponTracker);
        }

        public static bool HunkHasWeapon(HunkWeaponDef weaponDef, HunkWeaponTracker hunk)
        {
            foreach (HunkWeaponData i in hunk.weaponData)
            {
                if (i.weaponDef == weaponDef) return true;
            }

            return false;
        }
    }
}