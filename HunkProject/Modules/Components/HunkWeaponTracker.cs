﻿using System;
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
                    totalAmmo = Modules.Weapons.SMG.instance.magSize * 3,
                    currentAmmo = Modules.Weapons.SMG.instance.magSize
                },
                new HunkWeaponData
                {
                    weaponDef = Modules.Weapons.SMG.instance.weaponDef,
                    totalAmmo = Modules.Weapons.SMG.instance.magSize * 3,
                    currentAmmo = 12
                }
            };
        }

        public void AddWeapon(HunkWeaponDef weaponDef)
        {
            Array.Resize(ref this.weaponData, this.weaponData.Length + 1);

            this.weaponData[this.weaponData.Length - 1] = new HunkWeaponData
            {
                weaponDef = weaponDef,
                totalAmmo = weaponDef.magSize * 2,
                currentAmmo = weaponDef.magSize
            };
        }
    }
}