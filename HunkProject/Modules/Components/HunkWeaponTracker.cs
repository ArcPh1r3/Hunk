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

        private HunkController hunk
        {
            get
            {
                if (this._hunk) return this._hunk;

                if (this.GetComponent<CharacterMaster>())
                {
                    if (this.GetComponent<CharacterMaster>().GetBody())
                    {
                        this._hunk = this.GetComponent<CharacterMaster>().GetBody().GetComponent<HunkController>();
                        return this._hunk;
                    }
                }

                return null;
            }
        }

        private Inventory inventory;
        private HunkController _hunk;

        private void Awake()
        {
            this.inventory = this.GetComponent<Inventory>();
            this.Init();
        }

        private void Start()
        {
            this.AddWeaponItem(Modules.Weapons.SMG.instance.weaponDef);
            this.AddWeaponItem(Modules.Weapons.MUP.instance.weaponDef);

            this.inventory.onItemAddedClient += this.Inventory_onItemAddedClient;
        }

        private void OnDestroy()
        {
            if (this.inventory) this.inventory.onItemAddedClient -= this.Inventory_onItemAddedClient;
        }

        private void Inventory_onItemAddedClient(ItemIndex itemIndex)
        {
            // hmm.. not the best
            foreach (HunkWeaponDef i in HunkWeaponCatalog.weaponDefs)
            {
                if (itemIndex == i.itemDef.itemIndex)
                {
                    this.AddWeapon(i);
                }
            }
        }

        private void Init()
        {
            this.weaponData = new HunkWeaponData[]
            {
                new HunkWeaponData
                {
                    weaponDef = Modules.Weapons.SMG.instance.weaponDef,
                    totalAmmo = Modules.Weapons.SMG.instance.magSize * 2,
                    currentAmmo = Modules.Weapons.SMG.instance.magSize
                },
                new HunkWeaponData
                {
                    weaponDef = Modules.Weapons.MUP.instance.weaponDef,
                    totalAmmo = Modules.Weapons.MUP.instance.magSize * 2,
                    currentAmmo = Modules.Weapons.MUP.instance.magSize
                }
            };
            /*this.weaponData = new HunkWeaponData[]
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
                    totalAmmo = Modules.Weapons.Shotgun.instance.magSize,
                    currentAmmo = Modules.Weapons.Shotgun.instance.magSize
                },
                new HunkWeaponData
                {
                    weaponDef = Modules.Weapons.Slugger.instance.weaponDef,
                    totalAmmo = Modules.Weapons.Slugger.instance.magSize,
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
                    totalAmmo = Modules.Weapons.Magnum.instance.magSize,
                    currentAmmo = Modules.Weapons.Magnum.instance.magSize
                },
                new HunkWeaponData
                {
                    weaponDef = Modules.Weapons.Revolver.instance.weaponDef,
                    totalAmmo = Modules.Weapons.Revolver.instance.magSize,
                    currentAmmo = Modules.Weapons.Revolver.instance.magSize
                },
                new HunkWeaponData
                {
                    weaponDef = Modules.Weapons.RocketLauncher.instance.weaponDef,
                    totalAmmo = Modules.Weapons.RocketLauncher.instance.magSize,
                    currentAmmo = Modules.Weapons.RocketLauncher.instance.magSize
                },
                new HunkWeaponData
                {
                    weaponDef = Modules.Weapons.MUP.instance.weaponDef,
                    totalAmmo = Modules.Weapons.MUP.instance.magSize * 3,
                    currentAmmo = Modules.Weapons.MUP.instance.magSize
                }
            };*/
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
            for (int i = 0; i < this.weaponData.Length; i++)
            {
                if (this.weaponData[i].weaponDef == weaponDef) return;
            }

            Array.Resize(ref this.weaponData, this.weaponData.Length + 1);

            this.weaponData[this.weaponData.Length - 1] = new HunkWeaponData
            {
                weaponDef = weaponDef,
                totalAmmo = 0,
                currentAmmo = weaponDef.magSize
            };

            // redundant notification lmao
            //this.hunk.PickUpWeapon(weaponDef);

            // failsafe
            this.AddWeaponItem(weaponDef);
        }

        private void AddWeaponItem(HunkWeaponDef weaponDef)
        {
            if (this.inventory.GetItemCount(weaponDef.itemDef) <= 0) this.inventory.GiveItem(weaponDef.itemDef);
        }
    }
}