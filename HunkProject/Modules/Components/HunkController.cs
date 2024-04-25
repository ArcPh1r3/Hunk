using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HunkMod.Modules.Components
{
    public class HunkController : MonoBehaviour
    {
        public ushort syncedWeapon;
        public NetworkInstanceId netId;

        public bool isAiming;
        public HunkWeaponDef weaponDef;
        private HunkWeaponDef lastWeaponDef;

        public float chargeValue;
        public float lockOnTimer;
        public HurtBox targetHurtbox;

        private CharacterBody characterBody;
        private ChildLocator childLocator;
        private CharacterModel characterModel;
        private Animator animator;
        private SkillLocator skillLocator;

        public int maxShellCount = 24;

        private int currentShell;
        private int currentSlug;
        private GameObject[] shellObjects;
        private GameObject[] slugObjects;

        public Action<HunkController> onWeaponUpdate;

        public int maxAmmo;
        public int ammo;

        private Transform cameraPivot;
        private SkinnedMeshRenderer weaponRenderer;
        private HunkWeaponTracker _weaponTracker;

        public GameObject crosshairPrefab;
        public ParticleSystem machineGunVFX;
        public float desiredYOffset;

        private GameObject heldWeaponInstance;
        public float reloadTimer;
        private WeaponNotificationQueue notificationQueue;
        private EntityStateMachine weaponStateMachine;
        public float defaultYOffset { get; private set; }
        private float yOffset;
        public bool isRolling;
        public bool isReloading;
        private GameObject backWeaponInstance;
        private HunkWeaponDef backWeaponDef;
        private CameraRigController cameraController;
        public float ammoKillTimer = 0f;

        private void Awake()
        {
            this.characterBody = this.GetComponent<CharacterBody>();
            ModelLocator modelLocator = this.GetComponent<ModelLocator>();
            this.childLocator = modelLocator.modelBaseTransform.GetComponentInChildren<ChildLocator>();
            this.animator = modelLocator.modelBaseTransform.GetComponentInChildren<Animator>();
            this.characterModel = modelLocator.modelBaseTransform.GetComponentInChildren<CharacterModel>();
            this.skillLocator = this.GetComponent<SkillLocator>();
            this.machineGunVFX = this.childLocator.FindChild("MachineGunVFX").gameObject.GetComponent<ParticleSystem>();
            this.weaponRenderer = this.childLocator.FindChild("WeaponModel").GetComponent<SkinnedMeshRenderer>();
            this.weaponStateMachine = EntityStateMachine.FindByCustomName(this.gameObject, "Weapon");
            this.cameraPivot = modelLocator.modelBaseTransform.Find("CameraPivot").transform;
            this.defaultYOffset = 1.59f;
            this.desiredYOffset = this.defaultYOffset;
            this.yOffset = this.desiredYOffset;

            this.Invoke("SetInventoryHook", 0.5f);
        }

        private void Start()
        {
            this.InitShells();

            this.EquipWeapon(this.weaponTracker.equippedIndex);
        }

        private void SetInventoryHook()
        {
            if (this.characterBody && this.characterBody.master && this.characterBody.master.inventory)
            {
                this.characterBody.master.inventory.onInventoryChanged += this.Inventory_onInventoryChanged;
            }

            this.CheckForNeedler();
        }

        public HunkWeaponTracker weaponTracker
        {
            get
            {
                if (this._weaponTracker) return this._weaponTracker;
                if (this.characterBody && this.characterBody.master)
                {
                    HunkWeaponTracker i = this.characterBody.master.GetComponent<HunkWeaponTracker>();
                    if (!i) i = this.characterBody.master.gameObject.AddComponent<HunkWeaponTracker>();
                    this._weaponTracker = i;
                    return i;
                }
                return null;
            }
        }

        private void CheckForNeedler()
        {
            /*if (this.characterBody && this.characterBody.master && this.characterBody.master.inventory)
            {
                HunkWeaponDef desiredWeapon = this.pistolWeaponDef;

                if (this.characterBody.master.inventory.GetItemCount(RoR2Content.Items.TitanGoldDuringTP) > 0)
                {
                    if (this.defaultWeaponDef == DriverWeaponCatalog.Pistol || this.defaultWeaponDef == DriverWeaponCatalog.PyriteGun) desiredWeapon = DriverWeaponCatalog.PyriteGun;
                }

                if (this.characterBody.master.inventory.GetItemCount(RoR2Content.Items.LunarPrimaryReplacement) > 0)
                {
                    desiredWeapon = DriverWeaponCatalog.Needler;
                }

                if (this.maxWeaponTimer <= 0f && desiredWeapon != this.defaultWeaponDef)
                {
                    this.defaultWeaponDef = desiredWeapon;
                    this.PickUpWeapon(this.defaultWeaponDef);
                }
            }*/
        }

        public void SwapToLastWeapon()
        {
            this.weaponTracker.SwapToLastWeapon();
            this.EquipWeapon(this.weaponTracker.equippedIndex);
        }

        public void CycleWeapon()
        {
            this.weaponTracker.CycleWeapon();
            this.EquipWeapon(this.weaponTracker.equippedIndex);
        }

        public void SwapToWeapon(int index)
        {
            this.weaponTracker.SwapToWeapon(index);
            this.EquipWeapon(this.weaponTracker.equippedIndex);
        }

        private void Inventory_onInventoryChanged()
        {
            this.CheckForNeedler();
        }

        public void ConsumeAmmo()
        {
            this.reloadTimer = 2f;

            // fake ammo sync
            if (this.ammo <= this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].currentAmmo) this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].currentAmmo--;

            this.ammo--;

            if (this.ammo <= 0) this.ammo = 0;
            if (this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].currentAmmo <= 0) this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].currentAmmo = 0;
        }

        public void ApplyBandolier()
        {
            if (this.ammo - this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].currentAmmo >= this.maxAmmo && this.ammo != this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].currentAmmo) return;

            this.ammo += Mathf.CeilToInt(this.maxAmmo * 0.5f);
            if (this.ammo > this.maxAmmo) this.ammo = this.maxAmmo;
        }

        private void FixedUpdate()
        {
            this.reloadTimer -= Time.fixedDeltaTime;
            this.lockOnTimer -= Time.fixedDeltaTime;
            this.ammoKillTimer -= Time.fixedDeltaTime;

            if (!this.cameraController)
            {
                if (this.characterBody && this.characterBody.master)
                {
                    if (this.characterBody.master.playerCharacterMasterController)
                    {
                        if (this.characterBody.master.playerCharacterMasterController.networkUser)
                        {
                            this.cameraController = this.characterBody.master.playerCharacterMasterController.networkUser.cameraRigController;
                        }
                    }
                }
            }

            if (this.reloadTimer <= 0f && this.ammo < this.maxAmmo)
            {
                this.TryReload();
            }

            if (this.heldWeaponInstance)
            {
                if (this.characterModel && this.characterModel.invisibilityCount > 0) this.heldWeaponInstance.SetActive(false);
                else this.heldWeaponInstance.SetActive(true);
            }

            if (this.backWeaponInstance)
            {
                if (this.characterModel && this.characterModel.invisibilityCount > 0) this.backWeaponInstance.SetActive(false);
                else this.backWeaponInstance.SetActive(true);
            }

            if (this.lockOnTimer > 0f)
            {
                //this.TryLockOn();
            }

            this.yOffset = Mathf.Lerp(this.yOffset, this.desiredYOffset, 5f * Time.fixedDeltaTime);
            this.cameraPivot.localPosition = new Vector3(0f, this.yOffset, 0f);
        }

        /*private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V)) this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].totalAmmo += this.maxAmmo;
        }*/

        private void TryLockOn()
        {
            if (this.cameraController)
            {
                if (this.targetHurtbox)
                {
                    Vector3 targetVector = (this.targetHurtbox.healthComponent.transform.position - this.cameraPivot.position).normalized;
                    Vector3 lookVector = Vector3.Lerp(this.characterBody.inputBank.aimDirection, targetVector, 12f * Time.fixedDeltaTime);

                    ((RoR2.CameraModes.CameraModePlayerBasic.InstanceData)this.cameraController.cameraMode.camToRawInstanceData[this.cameraController]).SetPitchYawFromLookVector(lookVector);
                }
                else
                {

                }
            }
        }

        private void TryReload()
        {
            if (this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].totalAmmo > 0)
            {
                if (this.ammo == 0)
                {
                    this.weaponStateMachine.SetInterruptState(new SkillStates.Hunk.Reload
                    {
                        interruptPriority = EntityStates.InterruptPriority.Skill
                    }, EntityStates.InterruptPriority.Any);
                }
                else
                {
                    this.weaponStateMachine.SetInterruptState(new SkillStates.Hunk.Reload
                    {
                        interruptPriority = EntityStates.InterruptPriority.Any
                    }, EntityStates.InterruptPriority.Any);
                }
            }
        }

        public void ServerGetStoredWeapon(HunkWeaponDef newWeapon, float ammo, HunkController hunkController)
        {
            NetworkIdentity identity = hunkController.gameObject.GetComponent<NetworkIdentity>();
            if (!identity) return;

            new SyncStoredWeapon(identity.netId, newWeapon.index, ammo).Send(NetworkDestination.Clients);
        }

        public void ServerGetAmmo()
        {
            NetworkIdentity identity = this.GetComponent<NetworkIdentity>();
            if (!identity) return;

            new SyncAmmoPickup(identity.netId).Send(NetworkDestination.Clients);
        }

        public void ServerPickUpWeapon(HunkWeaponDef newWeapon, bool cutAmmo, HunkController hunkController, bool isAmmoBox = false)
        {
            NetworkIdentity identity = hunkController.gameObject.GetComponent<NetworkIdentity>();
            if (!identity) return;

            new SyncWeapon(identity.netId, newWeapon.index, cutAmmo, isAmmoBox).Send(NetworkDestination.Clients);
        }
        
        public void FinishReload()
        {
            int diff = this.maxAmmo - this.ammo;

            if (this.ammo + this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].totalAmmo >= this.weaponDef.magSize)
            {
                this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].currentAmmo = this.weaponDef.magSize;
                this.ammo = this.weaponDef.magSize;
            }
            else
            {
                this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].currentAmmo = this.ammo + this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].totalAmmo;
                this.ammo = this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].currentAmmo;
            }

            if (this.characterBody.master)
            {
                if (this.characterBody.master.inventory)
                {
                    this.ammo += this.characterBody.master.inventory.GetItemCount(RoR2Content.Items.SecondarySkillMagazine);
                }
            }

            this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].totalAmmo -= diff;
            if (this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].totalAmmo <= 0) this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].totalAmmo = 0;
        }

        public void PickUpWeapon(HunkWeaponDef newWeapon)
        {
            this.weaponDef = newWeapon;

            this.weaponTracker.AddWeapon(newWeapon);

            this.TryPickupNotification();

            if (this.onWeaponUpdate == null) return;
            this.onWeaponUpdate(this);
        }

        private void TryPickupNotification(bool force = false)
        {
            // attempt to add the component if it's not there
            if (!this.notificationQueue && this.characterBody.master)
            {
                this.notificationQueue = this.characterBody.master.GetComponent<WeaponNotificationQueue>();
               // if (!this.notificationQueue) this.notificationQueue = this.characterBody.master.gameObject.AddComponent<WeaponNotificationQueue>();
            }

            if (this.notificationQueue)
            {
                if (force)
                {
                    WeaponNotificationQueue.PushWeaponNotification(this.characterBody.master, this.weaponDef.index);
                    this.lastWeaponDef = this.weaponDef;
                    return;
                }

                if (this.weaponDef != this.lastWeaponDef)
                {
                    WeaponNotificationQueue.PushWeaponNotification(this.characterBody.master, this.weaponDef.index);
                }
                this.lastWeaponDef = this.weaponDef;
            }
        }

        public void EquipWeapon(int index)
        {
            this.weaponTracker.equippedIndex = index;

            this.weaponDef = this.weaponTracker.weaponData[index].weaponDef;

            // ammo
            this.maxAmmo = this.weaponDef.magSize;
            this.ammo = this.weaponTracker.weaponData[index].currentAmmo;

            // model swap
            if (this.weaponDef.modelPrefab)
            {
                this.weaponRenderer.gameObject.SetActive(false);
                if (this.heldWeaponInstance) Destroy(this.heldWeaponInstance);

                this.heldWeaponInstance = GameObject.Instantiate(this.weaponDef.modelPrefab);
                this.heldWeaponInstance.transform.parent = this.childLocator.FindChild("Weapon");
                this.heldWeaponInstance.transform.localPosition = Vector3.zero;
                this.heldWeaponInstance.transform.localRotation = Quaternion.identity;
                this.heldWeaponInstance.transform.localScale = Vector3.one;
            }
            else
            {
                this.weaponRenderer.gameObject.SetActive(true);
                if (this.heldWeaponInstance) Destroy(this.heldWeaponInstance);
            }

            // crosshair
            this.crosshairPrefab = this.weaponDef.crosshairPrefab;
            this.characterBody._defaultCrosshairPrefab = this.crosshairPrefab;

            // animator layer
            this.ToggleLayer("Body (Pistol)", false);
            this.ToggleLayer("Gesture (Pistol)", false);
            this.ToggleLayer("FullBody (Pistol)", false);

            this.ToggleLayer("Body (Rocket)", false);
            this.ToggleLayer("Gesture (Rocket)", false);
            this.ToggleLayer("FullBody (Rocket)", false);

            switch (this.weaponDef.animationSet)
            {
                case HunkWeaponDef.AnimationSet.Pistol:
                    this.ToggleLayer("Body (Pistol)", true);
                    this.ToggleLayer("Gesture (Pistol)", true);
                    this.ToggleLayer("FullBody (Pistol)", true);
                    break;
                case HunkWeaponDef.AnimationSet.Rocket:
                    this.ToggleLayer("Body (Rocket)", true);
                    this.ToggleLayer("Gesture (Rocket)", true);
                    this.ToggleLayer("FullBody (Rocket)", true);
                    break;
            }

            this.HandleBackWeapon();

            if (this.onWeaponUpdate == null) return;
            this.onWeaponUpdate(this);
        }

        private void HandleBackWeapon()
        {
            if (this.backWeaponInstance)
            {
                if (this.backWeaponDef && this.backWeaponDef == this.weaponTracker.weaponData[this.weaponTracker.lastEquippedIndex].weaponDef)
                {
                    return;
                }
                Destroy(this.backWeaponInstance);
            }
            if (this.weaponTracker.weaponData[this.weaponTracker.lastEquippedIndex].weaponDef.storedOnBack)
            {
                this.backWeaponDef = this.weaponTracker.weaponData[this.weaponTracker.lastEquippedIndex].weaponDef;
                this.backWeaponInstance = GameObject.Instantiate(this.backWeaponDef.modelPrefab);
                this.backWeaponInstance.transform.parent = this.childLocator.FindChild("BackWeapon");
                this.backWeaponInstance.transform.localPosition = Vector3.zero;
                this.backWeaponInstance.transform.localRotation = Quaternion.identity;
                this.backWeaponInstance.transform.localScale = Vector3.one;
            }
        }

        private void ToggleLayer(string layerName, bool toEnable)
        {
            if (!this.animator) return;
            if (layerName == "") return;

            if (toEnable) this.animator.SetLayerWeight(this.animator.GetLayerIndex(layerName), 1f);
            else this.animator.SetLayerWeight(this.animator.GetLayerIndex(layerName), 0f);
        }

        private void InitShells()
        {
            this.currentShell = 0;

            this.shellObjects = new GameObject[this.maxShellCount + 1];

            GameObject desiredShell = Assets.shotgunShell;

            for (int i = 0; i < this.maxShellCount; i++)
            {
                this.shellObjects[i] = GameObject.Instantiate(desiredShell, this.childLocator.FindChild("MuzzleShell"), false);
                this.shellObjects[i].transform.localScale = Vector3.one * 0.75f;
                this.shellObjects[i].SetActive(false);
                this.shellObjects[i].GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;

                this.shellObjects[i].layer = LayerIndex.ragdoll.intVal;
                this.shellObjects[i].transform.GetChild(0).gameObject.layer = LayerIndex.ragdoll.intVal;
            }

            this.currentSlug = 0;

            this.slugObjects = new GameObject[this.maxShellCount + 1];

            desiredShell = Assets.shotgunSlug;

            for (int i = 0; i < this.maxShellCount; i++)
            {
                this.slugObjects[i] = GameObject.Instantiate(desiredShell, this.childLocator.FindChild("MuzzleShell"), false);
                this.slugObjects[i].transform.localScale = Vector3.one * 0.85f;
                this.slugObjects[i].SetActive(false);
                this.slugObjects[i].GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;

                this.slugObjects[i].layer = LayerIndex.ragdoll.intVal;
                this.slugObjects[i].transform.GetChild(0).gameObject.layer = LayerIndex.ragdoll.intVal;
            }
        }

        public void DropShell(Vector3 force)
        {
            if (this.shellObjects == null) return;

            if (this.shellObjects[this.currentShell] == null) return;

            Transform origin = this.childLocator.FindChild("MuzzleShell");

            this.shellObjects[this.currentShell].SetActive(false);

            this.shellObjects[this.currentShell].transform.position = origin.position;
            this.shellObjects[this.currentShell].transform.SetParent(null);
            this.shellObjects[this.currentShell].transform.localScale = Vector3.one * 0.75f;

            this.shellObjects[this.currentShell].SetActive(true);

            Rigidbody rb = this.shellObjects[this.currentShell].gameObject.GetComponent<Rigidbody>();
            if (rb) rb.velocity = force;

            this.currentShell++;
            if (this.currentShell >= this.maxShellCount) this.currentShell = 0;
        }

        public void DropSlug(Vector3 force)
        {
            if (this.slugObjects == null) return;

            if (this.slugObjects[this.currentSlug] == null) return;

            Transform origin = this.childLocator.FindChild("MuzzleShell");

            this.slugObjects[this.currentSlug].SetActive(false);

            this.slugObjects[this.currentSlug].transform.position = origin.position;
            this.slugObjects[this.currentSlug].transform.SetParent(null);
            this.slugObjects[this.currentSlug].transform.localScale = Vector3.one * 0.85f;

            this.slugObjects[this.currentSlug].SetActive(true);

            Rigidbody rb = this.slugObjects[this.currentSlug].gameObject.GetComponent<Rigidbody>();
            if (rb) rb.velocity = force;

            this.currentSlug++;
            if (this.currentSlug >= this.maxShellCount) this.currentSlug = 0;
        }

        private void OnDestroy()
        {
            if (this.shellObjects != null && this.shellObjects.Length > 0)
            {
                for (int i = 0; i < this.shellObjects.Length; i++)
                {
                    if (this.shellObjects[i]) Destroy(this.shellObjects[i]);
                }
            }

            if (this.slugObjects != null && this.slugObjects.Length > 0)
            {
                for (int i = 0; i < this.slugObjects.Length; i++)
                {
                    if (this.slugObjects[i]) Destroy(this.slugObjects[i]);
                }
            }

            if (this.characterBody && this.characterBody.master && this.characterBody.master.inventory)
            {
                this.characterBody.master.inventory.onInventoryChanged -= this.Inventory_onInventoryChanged;
            }
        }

        public void AddRandomAmmo(float multiplier = 1f)
        {
            // TODO
            // change this to a weighted selection, so stronger weapons are less likely to get ammo

            int index = UnityEngine.Random.Range(0, this.weaponTracker.weaponData.Length);
            int amount = Mathf.CeilToInt(this.weaponTracker.weaponData[index].weaponDef.magSize * multiplier);

            this.weaponTracker.weaponData[index].totalAmmo += amount;

            GameObject effect = GameObject.Instantiate(Modules.Assets.ammoPickupEffectPrefab, this.characterBody.aimOrigin + (Vector3.up * 0.85f) + (this.characterBody.inputBank.aimDirection * 2f), Quaternion.identity);

            effect.GetComponentInChildren<RoR2.UI.LanguageTextMeshController>().token = "+" + amount + " " + this.weaponTracker.weaponData[index].weaponDef.ammoName;
        }

        public void AddAmmoFromIndex(int index)
        {
            int amount = Mathf.CeilToInt(this.weaponTracker.weaponData[index].weaponDef.magSize);

            this.weaponTracker.weaponData[index].totalAmmo += amount;

            GameObject effect = GameObject.Instantiate(Modules.Assets.ammoPickupEffectPrefab, this.characterBody.aimOrigin + (Vector3.up * 0.85f) + (this.characterBody.inputBank.aimDirection * 2f), Quaternion.identity);

            effect.GetComponentInChildren<RoR2.UI.LanguageTextMeshController>().token = "+" + amount + " " + this.weaponTracker.weaponData[index].weaponDef.ammoName;
            Util.PlaySound("sfx_hunk_pickup", this.gameObject);
        }
    }
}