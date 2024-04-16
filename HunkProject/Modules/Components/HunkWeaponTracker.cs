using System;
using UnityEngine;

namespace HunkMod.Modules.Components
{
    public struct HunkWeaponData
    {
        public HunkWeaponDef weaponDef;
        public int totalAmmo;
        public int currentAmmo;
    };

    public class HunkWeaponTracker : MonoBehaviour
    {
        public HunkWeaponData[] weaponData = new HunkWeaponData[0];
        public int equippedIndex = 0;

        public int lastEquippedIndex = 1;

        private void Awake()
        {
            this.Init();
        }

        private void Init()
        {
            this.weaponData = new HunkWeaponData[]
            {
                new HunkWeaponData
                {
                    weaponDef = Modules.Weapons.SMG.instance.weaponDef,
                    totalAmmo = Modules.Weapons.SMG.instance.magSize * 6,
                    currentAmmo = Modules.Weapons.SMG.instance.magSize
                },
                new HunkWeaponData
                {
                    weaponDef = Modules.Weapons.Shotgun.instance.weaponDef,
                    totalAmmo = Modules.Weapons.Shotgun.instance.magSize * 3,
                    currentAmmo = Modules.Weapons.Shotgun.instance.magSize
                },
                new HunkWeaponData
                {
                    weaponDef = Modules.Weapons.Slugger.instance.weaponDef,
                    totalAmmo = Modules.Weapons.Slugger.instance.magSize * 3,
                    currentAmmo = Modules.Weapons.Slugger.instance.magSize
                }
            };
        }

        public void SwapToLastWeapon()
        {
            // temporary until radial is in
            this.lastEquippedIndex = this.equippedIndex;
            this.equippedIndex++;
            if (this.equippedIndex >= this.weaponData.Length) this.equippedIndex = 0;
            return;

            int penis = this.equippedIndex;
            this.equippedIndex = this.lastEquippedIndex;
            this.lastEquippedIndex = penis;
        }

        public void AddWeapon(HunkWeaponDef weaponDef)
        {
            Array.Resize(ref this.weaponData, this.weaponData.Length + 1);

            this.weaponData[this.weaponData.Length - 1] = new HunkWeaponData
            {
                weaponDef = weaponDef,
                totalAmmo = 0,
                currentAmmo = weaponDef.magSize
            };
        }
    }
}