using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HunkMod.Modules.Components
{
    public struct HunkWeaponData
    {
        public HunkWeaponDef weaponDef;
        public int totalAmmo;
        public int currentAmmo;
    };

    public struct HunkStoredWeaponData
    {
        public HunkWeaponDef weaponDef;
        public int totalAmmo;
        public int currentAmmo;
    };

    public class HunkWeaponTracker : MonoBehaviour
    {
        public HunkWeaponData[] weaponData = new HunkWeaponData[0];
        public HunkWeaponData[] storedWeaponData = new HunkWeaponData[0];
        public int equippedIndex = 0;

        public int missionStep;
        public int lastEquippedIndex = 1;
        public int nextWeapon;

        public bool wtfTheFuck = false;
        public bool ignoreFlag = false;

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

        public bool spawnedKeycardThisStage = false;
        public bool spawnedTerminalThisStage = false;
        public bool usedAmmoThisStage = false;

        private Inventory inventory;
        private HunkController _hunk;

        private bool hasAllKeycards
        {
            get
            {
                if (!this.inventory) return false;

                if (this.inventory.GetItemCount(Modules.Survivors.Hunk.spadeKeycard) <= 0) return false;
                if (this.inventory.GetItemCount(Modules.Survivors.Hunk.clubKeycard) <= 0) return false;
                if (this.inventory.GetItemCount(Modules.Survivors.Hunk.heartKeycard) <= 0) return false;
                if (this.inventory.GetItemCount(Modules.Survivors.Hunk.diamondKeycard) <= 0) return false;
                if (this.inventory.GetItemCount(Modules.Survivors.Hunk.wristband) <= 0) return false;

                return true;
            }
        }

        private void Awake()
        {
            this.inventory = this.GetComponent<Inventory>();

            this.Init();

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            this.CancelInvoke();
            if (NetworkServer.active) this.Invoke("SpawnKeycard", 60f);
        }

        private void OnDestroy()
        {
            if (this.inventory) this.inventory.onItemAddedClient -= this.Inventory_onItemAddedClient;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1)
        {
            this.spawnedKeycardThisStage = false;
            this.spawnedTerminalThisStage = false;
            this.usedAmmoThisStage = false;

            this.CancelInvoke();
            if (NetworkServer.active) this.Invoke("SpawnKeycard", UnityEngine.Random.Range(5f, 30f));
        }

        private void Start()
        {
            this.inventory.onItemAddedClient += this.Inventory_onItemAddedClient;
        }

        public void SetHunk(HunkController i)
        {
            this._hunk = i;
        }

        private void Inventory_onItemAddedClient(ItemIndex itemIndex)
        {
            if (itemIndex == Modules.Survivors.Hunk.gVirusSample.itemIndex) this.missionStep = 0;
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
            if (this.hunk.GetComponent<HunkPassive>().isFullArsenal)
            {
                if (RoR2Application.isInMultiPlayer) this.ignoreFlag = true;

                this.weaponData = new HunkWeaponData[]
                {
                    new HunkWeaponData
                    {
                        weaponDef = Modules.Weapons.SMG.instance.weaponDef,
                        totalAmmo = Modules.Weapons.SMG.instance.magSize * 12,
                        currentAmmo = Modules.Weapons.SMG.instance.magSize
                    },
                    new HunkWeaponData
                    {
                        weaponDef = Modules.Weapons.MUP.instance.weaponDef,
                        totalAmmo = Modules.Weapons.MUP.instance.magSize * 20,
                        currentAmmo = Modules.Weapons.MUP.instance.magSize
                    },
                    new HunkWeaponData
                    {
                        weaponDef = Modules.Weapons.Shotgun.instance.weaponDef,
                        totalAmmo = Modules.Weapons.Shotgun.instance.magSize * 8,
                        currentAmmo = Modules.Weapons.Shotgun.instance.magSize
                    },
                    new HunkWeaponData
                    {
                        weaponDef = Modules.Weapons.Slugger.instance.weaponDef,
                        totalAmmo = Modules.Weapons.Slugger.instance.magSize * 8,
                        currentAmmo = Modules.Weapons.Slugger.instance.magSize
                    },
                    new HunkWeaponData
                    {
                        weaponDef = Modules.Weapons.Magnum.instance.weaponDef,
                        totalAmmo = Modules.Weapons.Magnum.instance.magSize * 6,
                        currentAmmo = Modules.Weapons.Magnum.instance.magSize
                    },
                    new HunkWeaponData
                    {
                        weaponDef = Modules.Weapons.Revolver.instance.weaponDef,
                        totalAmmo = Modules.Weapons.Revolver.instance.magSize * 6,
                        currentAmmo = Modules.Weapons.Revolver.instance.magSize
                    },
                    new HunkWeaponData
                    {
                        weaponDef = Modules.Weapons.Flamethrower.instance.weaponDef,
                        totalAmmo = Modules.Weapons.Flamethrower.instance.magSize * 4,
                        currentAmmo = Modules.Weapons.Flamethrower.instance.magSize
                    },
                    new HunkWeaponData
                    {
                        weaponDef = Modules.Weapons.GrenadeLauncher.instance.weaponDef,
                        totalAmmo = Modules.Weapons.GrenadeLauncher.instance.magSize * 8,
                        currentAmmo = Modules.Weapons.GrenadeLauncher.instance.magSize
                    }
                };

                this.AddWeaponItem(Modules.Weapons.SMG.instance.weaponDef);
                this.AddWeaponItem(Modules.Weapons.MUP.instance.weaponDef);
                this.AddWeaponItem(Modules.Weapons.Shotgun.instance.weaponDef);
                this.AddWeaponItem(Modules.Weapons.Slugger.instance.weaponDef);
                this.AddWeaponItem(Modules.Weapons.Magnum.instance.weaponDef);
                this.AddWeaponItem(Modules.Weapons.Revolver.instance.weaponDef);
                this.AddWeaponItem(Modules.Weapons.Flamethrower.instance.weaponDef);
                this.AddWeaponItem(Modules.Weapons.GrenadeLauncher.instance.weaponDef);

                if (NetworkServer.active)
                {
                    this.inventory.GiveItem(Modules.Weapons.SMG.laserSight);
                    this.inventory.GiveItem(Modules.Weapons.SMG.extendedMag);
                    this.inventory.GiveItem(Modules.Weapons.MUP.gunStock);
                    this.inventory.GiveItem(Modules.Weapons.Magnum.longBarrel);
                    this.inventory.GiveItem(Modules.Weapons.Revolver.speedloader);
                }
            }
            else
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
                    },
                    new HunkWeaponData
                    {
                        weaponDef = Modules.Weapons.ScanGun.instance.weaponDef,
                        totalAmmo = 999,
                        currentAmmo = 999
                    }
                };

                this.AddWeaponItem(Modules.Weapons.SMG.instance.weaponDef);
                this.AddWeaponItem(Modules.Weapons.MUP.instance.weaponDef);
                this.AddWeaponItem(Modules.Weapons.ScanGun.instance.weaponDef);
            }
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

            bool hasStoredData = false;
            foreach (HunkWeaponData j in this.storedWeaponData)
            {
                if (weaponDef == j.weaponDef)
                {
                    hasStoredData = true;
                    this.RemoveStoredData(weaponDef);

                    this.weaponData[this.weaponData.Length - 1] = new HunkWeaponData
                    {
                        weaponDef = weaponDef,
                        totalAmmo = j.totalAmmo,
                        currentAmmo = j.currentAmmo
                    };
                }
            }

            if (!hasStoredData)
            {
                this.weaponData[this.weaponData.Length - 1] = new HunkWeaponData
                {
                    weaponDef = weaponDef,
                    totalAmmo = weaponDef.magSize * weaponDef.startingMags,
                    currentAmmo = weaponDef.magSize
                };
            }

            Util.PlaySound("sfx_hunk_weapon_get", this.hunk.gameObject);

            // redundant notification lmao
            //this.hunk.PickUpWeapon(weaponDef);

            // failsafe
            this.AddWeaponItem(weaponDef);
        }

        public void DropWeapon(int index)
        {
            if (index >= this.weaponData.Length) return;
            if (this.weaponData[index].weaponDef == null) return;

            // no repeats
            foreach (HunkWeaponData i in this.storedWeaponData)
            {
                if (i.weaponDef == this.weaponData[index].weaponDef) return;
            }

            HunkWeaponDef weaponDef = this.weaponData[index].weaponDef;
            int _current = this.weaponData[index].currentAmmo;
            int _total = this.weaponData[index].totalAmmo;

            // add an entry to the stored weapons
            // this is for keeping your ammo when you pick up a gun again

            Array.Resize(ref this.storedWeaponData, this.storedWeaponData.Length + 1);

            this.storedWeaponData[this.storedWeaponData.Length - 1] = new HunkWeaponData
            {
                weaponDef = weaponDef,
                totalAmmo = _total,
                currentAmmo = _current
            };

            for (int i = index; i < this.weaponData.Length - 1; i++)
            {
                // move elements downwards
                this.weaponData[i] = this.weaponData[i + 1];
            }
            // decrement array size
            Array.Resize(ref this.weaponData, this.weaponData.Length - 1);

            // create pickup now
            if (NetworkServer.active)
            {
                PickupDropletController.CreatePickupDroplet(
                    PickupCatalog.FindPickupIndex(weaponDef.itemDef.itemIndex),
                    this.hunk.characterBody.corePosition,
                    this.hunk.characterBody.inputBank.aimDirection * 15f);

                // remove it from the inventory
                this.inventory.RemoveItem(weaponDef.itemDef, 100);
            }

            // adjust your equipped index if it was decremented
            if (index < this.equippedIndex) this.equippedIndex--;

            // if you just dropped your last equipped weapon, find a new one- defaults to last in inventory
            if (this.lastEquippedIndex == index)
            {
                this.lastEquippedIndex = 0;
                for (int i = 0; i < this.weaponData.Length; i++)
                {
                    if (i != this.equippedIndex) this.lastEquippedIndex = i;
                }

                this.hunk.HandleBackWeapon();
            }
            else
            {
                if (index < this.lastEquippedIndex) this.lastEquippedIndex--;
            }
        }

        public void RemoveStoredData(HunkWeaponDef weaponDef)
        {
            int index = 0;
            for (int j = 0; j < this.storedWeaponData.Length; j++)
            {
                if (this.storedWeaponData[j].weaponDef && this.storedWeaponData[j].weaponDef == weaponDef) index = j;
            }

            for (int i = index; i < this.storedWeaponData.Length - 1; i++)
            {
                // move elements downwards
                this.storedWeaponData[i] = this.storedWeaponData[i + 1];
            }
            // decrement array size
            Array.Resize(ref this.storedWeaponData, this.storedWeaponData.Length - 1);
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.V)) this.SpawnKeycard();
        }

        private void AddWeaponItem(HunkWeaponDef weaponDef)
        {
            if (!NetworkServer.active) return;
            if (this.inventory.GetItemCount(weaponDef.itemDef) <= 0) this.inventory.GiveItem(weaponDef.itemDef);
        }

        private void SpawnKeycard()
        {
            if (this.spawnedKeycardThisStage)
            {
                this.CancelInvoke();
                return;
            }

            if (this.hasAllKeycards && !Modules.Config.permanentInfectionEvent.Value)
            {
                this.CancelInvoke();
                return;
            }

            if (this.SpawnKeycardHolder(Modules.Enemies.Parasite.characterSpawnCard))
            {
                this.spawnedKeycardThisStage = true;
                return;
            }
            else
            {
                this.Invoke("SpawnKeycard", 0.5f);
            }
        }

        private bool SpawnKeycardHolder(SpawnCard spawnCard)
        {
            // just spawn it on a random enemy idc
            Transform target = null;
            foreach (CharacterBody i in CharacterBody.readOnlyInstancesList)
            {
                if (i && i.teamComponent && i.teamComponent.teamIndex == TeamIndex.Monster && i.healthComponent.alive)
                {
                    if (!i.GetComponent<VirusHandler>() && !i.GetComponent<ParasiteController>())
                    {
                        target = i.transform;
                        break;
                    }
                }
            }

            //target = this.hunk.characterBody.transform;

            if (!target) return false;

            if (Modules.Config.globalInfectionSound.Value) Util.PlaySound("sfx_hunk_virus_spawn", this.gameObject);

            if (NetworkServer.active)
            {
                NetworkIdentity identity = this.GetComponent<NetworkIdentity>();
                if (identity)
                {
                    new SyncVirus(identity.netId, target.gameObject).Send(NetworkDestination.Clients);
                }

                /*target.gameObject.AddComponent<VirusHandler>();

                GameObject positionIndicator = GameObject.Instantiate(Modules.Assets.virusPositionIndicator);
                positionIndicator.transform.parent = target.transform;
                positionIndicator.transform.localPosition = Vector3.zero;
                positionIndicator.GetComponent<PositionIndicator>().targetTransform = target.GetComponent<CharacterBody>().mainHurtBox.transform;*/

                this.missionStep = 1;
            }

            return true;
        }
    }
}