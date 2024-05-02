using RoR2;
using System;
using System.Collections;
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

        public bool hasSpawnedSpadeKeycard = false;
        public bool hasSpawnedClubKeycard = false;
        public bool hasSpawnedHeartKeycard = false;
        public bool hasSpawnedDiamondKeycard = false;
        private bool spawnedKeycardThisStage = false;
        private int attempts = 0;

        private Inventory inventory;
        private HunkController _hunk;

        private void Awake()
        {
            this.inventory = this.GetComponent<Inventory>();
            this.Init();

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private void OnDestroy()
        {
            if (this.inventory) this.inventory.onItemAddedClient -= this.Inventory_onItemAddedClient;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1)
        {
            this.spawnedKeycardThisStage = false;
            this.attempts = 0;

            this.CancelInvoke();
            this.InvokeRepeating("TrySpawnKeycard", 10f, 10f);
        }

        private void Start()
        {
            this.AddWeaponItem(Modules.Weapons.SMG.instance.weaponDef);
            this.AddWeaponItem(Modules.Weapons.MUP.instance.weaponDef);
            //this.AddWeaponItem(Modules.Weapons.Shotgun.instance.weaponDef);
            //this.AddWeaponItem(Modules.Weapons.Slugger.instance.weaponDef);
            //this.AddWeaponItem(Modules.Weapons.Magnum.instance.weaponDef);
            //this.AddWeaponItem(Modules.Weapons.Revolver.instance.weaponDef);

            //this.inventory.GiveItem(Modules.Survivors.Hunk.clubKeycard);
            //this.inventory.GiveItem(Modules.Survivors.Hunk.diamondKeycard);
            //this.inventory.GiveItem(Modules.Survivors.Hunk.heartKeycard);
            //this.inventory.GiveItem(Modules.Survivors.Hunk.spadeKeycard);

            this.inventory.onItemAddedClient += this.Inventory_onItemAddedClient;
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
                    weaponDef = Modules.Weapons.Magnum.instance.weaponDef,
                    totalAmmo = Modules.Weapons.Magnum.instance.magSize,
                    currentAmmo = Modules.Weapons.Magnum.instance.magSize
                },
                new HunkWeaponData
                {
                    weaponDef = Modules.Weapons.Revolver.instance.weaponDef,
                    totalAmmo = Modules.Weapons.Revolver.instance.magSize,
                    currentAmmo = Modules.Weapons.Revolver.instance.magSize
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

            Util.PlaySound("sfx_hunk_pickup", this.hunk.gameObject);

            // redundant notification lmao
            //this.hunk.PickUpWeapon(weaponDef);

            // failsafe
            this.AddWeaponItem(weaponDef);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V)) this.TrySpawnKeycard();
        }

        private void AddWeaponItem(HunkWeaponDef weaponDef)
        {
            if (this.inventory.GetItemCount(weaponDef.itemDef) <= 0) this.inventory.GiveItem(weaponDef.itemDef);
        }

        private void TrySpawnKeycard()
        {
            if (this.spawnedKeycardThisStage)
            {
                this.CancelInvoke();
                return;
            }

            if (this.hasSpawnedSpadeKeycard
                && this.hasSpawnedClubKeycard
                && this.hasSpawnedHeartKeycard
                && this.hasSpawnedDiamondKeycard)
            {
                this.CancelInvoke();
                return;
            }

            float rng = UnityEngine.Random.value;
            float chance = 0.02f;

            this.attempts++; // near guaranteed after 2 minutes
            if (this.attempts >= 12) chance = 0.9f;

            if (!this.hasSpawnedSpadeKeycard)
            {
                if (rng <= chance)
                {
                    if (this.SpawnKeycardHolder(Modules.Enemies.Parasite.spadeSpawnCard)) this.hasSpawnedSpadeKeycard = true;
                }
                return;
            }

            // only spades on stage 1
            if (Run.instance.stageClearCount <= 0) return;

            if (!this.hasSpawnedClubKeycard)
            {
                if (rng <= chance)
                {
                    if (this.SpawnKeycardHolder(Modules.Enemies.Parasite.clubSpawnCard)) this.hasSpawnedClubKeycard = true;
                }
                return;
            }

            if (!this.hasSpawnedHeartKeycard)
            {
                if (rng <= chance)
                {
                    if (this.SpawnKeycardHolder(Modules.Enemies.Parasite.heartSpawnCard)) this.hasSpawnedHeartKeycard = true;
                }
                return;
            }

            if (!this.hasSpawnedDiamondKeycard)
            {
                if (rng <= chance)
                {
                    if (this.SpawnKeycardHolder(Modules.Enemies.Parasite.diamondSpawnCard)) this.hasSpawnedDiamondKeycard = true;
                }
                return;
            }
        }

        private bool SpawnKeycardHolder(SpawnCard spawnCard)
        {
            this.spawnedKeycardThisStage = true;

            // just spawn it on a random enemy idc
            Transform target = null;
            foreach (CharacterBody i in CharacterBody.readOnlyInstancesList)
            {
                if (i && i.teamComponent && i.teamComponent.teamIndex != TeamIndex.Player)
                {
                    target = i.transform;
                    break;
                }
            }

            if (!target) return false;

            if (NetworkServer.active)
            {
                /*DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.Direct,
                    minDistance = 0f,
                    maxDistance = 0f,
                    position = target.position + (Vector3.up * 8f)
                }, Run.instance.runRNG);

                GameObject spawnedBody = DirectorCore.instance.TrySpawnObject(directorSpawnRequest);*/

                var summon = new MasterSummon();
                summon.position = target.position + (Vector3.up * 8);
                summon.masterPrefab = spawnCard.prefab;
                summon.summonerBodyObject = target.gameObject;
                var master = summon.Perform();
            }

            return true;
        }
        // ummm
        /*PickupDropletController.CreatePickupDroplet(
            PickupCatalog.FindPickupIndex(itemDef.itemIndex),
            this.hunk.characterBody.corePosition,
            this.hunk.characterBody.inputBank.aimDirection * 10f);*/
    }
}