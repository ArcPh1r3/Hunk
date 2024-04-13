using System;
using System.Collections.Generic;
using UnityEngine;

namespace HunkMod
{
    public static class HunkWeaponCatalog
    {
        public static Dictionary<string, HunkWeaponDef> weaponDrops = new Dictionary<string, HunkWeaponDef>();
        public static HunkWeaponDef[] weaponDefs = new HunkWeaponDef[0];

        public static void AddWeapon(HunkWeaponDef weaponDef)
        {
            Array.Resize(ref weaponDefs, weaponDefs.Length + 1);

            int index = weaponDefs.Length - 1;
            weaponDef.index = (ushort)index;

            weaponDefs[index] = weaponDef;
            weaponDef.index = (ushort)index;

            // heheheha
            weaponDef.pickupPrefab = Modules.Assets.CreatePickupObject(weaponDef);

            // set default icon
            /*if (!weaponDef.icon)
            {
                switch (weaponDef.tier)
                {
                    case DriverWeaponTier.Common:
                        weaponDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Texture>("texGenericWeaponGrey");
                        break;
                    case DriverWeaponTier.Uncommon:
                        weaponDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Texture>("texGenericWeaponGreen");
                        break;
                    case DriverWeaponTier.Legendary:
                        weaponDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Texture>("texGenericWeaponRed");
                        break;
                    case DriverWeaponTier.Unique:
                        weaponDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Texture>("texGenericWeaponYellow");
                        break;
                    case DriverWeaponTier.Lunar:
                        weaponDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Texture>("texGenericWeaponBlue");
                        break;
                    case DriverWeaponTier.Void:
                        weaponDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Texture>("texGenericWeaponPurple");
                        break;
                }
            }*/

            Debug.Log("Added " + weaponDef.nameToken + " to catalog with index: " + weaponDef.index);
        }

        public static void AddWeaponDrop(string bodyName, HunkWeaponDef weaponDef, bool autoComplete = true)
        {
            if (autoComplete)
            {
                if (!bodyName.Contains("Body")) bodyName += "Body";
                if (!bodyName.Contains("(Clone)")) bodyName += "(Clone)";
            }

            weaponDrops.Add(bodyName, weaponDef);
        }

        public static HunkWeaponDef GetWeaponFromIndex(int index)
        {
            return weaponDefs[index];
        }
    }
}