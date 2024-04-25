using RoR2;
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

        private Inventory inventory;

        private void Awake()
        {
            this.inventory = this.GetComponent<Inventory>();
            this.Init();
        }

        private void Start()
        {
            this.AddWeaponItem(Modules.Weapons.SMG.instance.weaponDef);
            this.AddWeaponItem(Modules.Weapons.Shotgun.instance.weaponDef);
            this.AddWeaponItem(Modules.Weapons.Slugger.instance.weaponDef);
            this.AddWeaponItem(Modules.Weapons.M19.instance.weaponDef);
            this.AddWeaponItem(Modules.Weapons.Magnum.instance.weaponDef);
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
                },
                new HunkWeaponData
                {
                    weaponDef = Modules.Weapons.M19.instance.weaponDef,
                    totalAmmo = Modules.Weapons.M19.instance.magSize * 3,
                    currentAmmo = Modules.Weapons.M19.instance.magSize
                },
                new HunkWeaponData
                {
                    weaponDef = Modules.Weapons.Magnum.instance.weaponDef,
                    totalAmmo = Modules.Weapons.Magnum.instance.magSize * 3,
                    currentAmmo = Modules.Weapons.Magnum.instance.magSize
                },
                new HunkWeaponData
                {
                    weaponDef = Modules.Weapons.ATM.instance.weaponDef,
                    totalAmmo = Modules.Weapons.ATM.instance.magSize * 3,
                    currentAmmo = Modules.Weapons.ATM.instance.magSize
                },
                new HunkWeaponData
                {
                    weaponDef = Modules.Weapons.RocketLauncher.instance.weaponDef,
                    totalAmmo = Modules.Weapons.RocketLauncher.instance.magSize * 3,
                    currentAmmo = Modules.Weapons.RocketLauncher.instance.magSize
                }
            };
        }

        public void SwapToLastWeapon()
        {
            int penis = this.equippedIndex;
            this.equippedIndex = this.lastEquippedIndex;
            this.lastEquippedIndex = penis;
        }

        public void CycleWeapon()
        {
            this.lastEquippedIndex = this.equippedIndex;
            this.equippedIndex++;
            if (this.equippedIndex >= this.weaponData.Length) this.equippedIndex = 0;
        }

        public void SwapToWeapon(int index)
        {
            this.lastEquippedIndex = this.equippedIndex;
            this.equippedIndex = index;
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

            this.AddWeaponItem(weaponDef);
        }

        private void AddWeaponItem(HunkWeaponDef weaponDef)
        {
            if (this.inventory.GetItemCount(weaponDef.itemDef) <= 0) this.inventory.GiveItem(weaponDef.itemDef);
        }
    }
}