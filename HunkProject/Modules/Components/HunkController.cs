using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.Navigation;
using RoR2.Skills;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace HunkMod.Modules.Components
{
    public class HunkController : MonoBehaviour
    {
        public ushort syncedWeapon;
        public NetworkInstanceId netId;

        public bool spawnedATM = false;
        public HunkPassive passive;

        public float baseTurnSpeed;

        public bool isAiming;
        public HunkWeaponDef weaponDef;
        private HunkWeaponDef lastWeaponDef;

        public float chargeValue;
        public float lockOnTimer;
        public HurtBox targetHurtbox;

        public float snapOffset = 0.25f;
        public float flamethrowerLifetime;

        public bool immobilized;
        private bool _wasImmobilized;
        private GameObject shieldOverlay;

        private int counterCount;

        public HunkNotificationHandler notificationHandler;
        public CharacterBody characterBody { get; private set; }
        private ChildLocator childLocator;
        private CharacterModel characterModel;
        private Animator animator;
        private SkillLocator skillLocator;

        public GenericSkill knifeSkinSkillSlot;

        public int maxShellCount = 24;

        private int currentShell;
        private int currentSlug;
        private GameObject[] shellObjects;
        private GameObject[] slugObjects;

        public Action<HunkController> onWeaponUpdate;
        public static Action<int> onCounter;

        public int maxAmmo;
        public int ammo;

        private Transform cameraPivot;
        private SkinnedMeshRenderer weaponRenderer;
        private HunkWeaponTracker _weaponTracker;

        public GameObject crosshairPrefab;
        public ParticleSystem machineGunVFX;
        public float desiredYOffset;
        public float desiredZOffset;

        private GameObject heldWeaponInstance;
        public float reloadTimer;
        private WeaponNotificationQueue notificationQueue;
        private EntityStateMachine weaponStateMachine;
        public float defaultYOffset { get; private set; }
        private float yOffset;
        public float defaultZOffset { get; private set; }
        private float zOffset;
        public bool isRolling;
        public bool isReloading;
        private GameObject backWeaponInstance;
        private HunkWeaponDef backWeaponDef;
        private CameraRigController cameraController;
        public float ammoKillTimer = 0f;
        private ParticleSystem speedLines;
        private Animator dodgeFlash;
        private Animator counterFlash;
        private GameObject emptyCrosshair = Modules.Assets.LoadCrosshair("Bad");
        private bool isOut;
        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
        private GameObject flamethrowerEffectInstance;
        private GameObject flamethrowerEffectInstance2;
        private bool flameIsPlaying = true;
        private GameObject flamethrowerLight;
        private uint flamethrowerPlayID;
        private bool flameInit = false;
        private uint bgmPlayID;
        private bool shieldIsVoid;
        private bool _shieldIsVoid;
        private bool fancyShield = true;
        private float lastSprintMultiplier;
        private float sprintMultiplier;
        private float sprintStopwatch;
        private float baseSprintValue;

        public int mupQueuedShots = 2;

        public float iFrames;

        private void Awake()
        {
            this.characterBody = this.GetComponent<CharacterBody>();
            ModelLocator modelLocator = this.GetComponent<ModelLocator>();
            this.passive = this.GetComponent<HunkPassive>();
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
            this.defaultZOffset = 0f;
            this.desiredZOffset = this.defaultZOffset;
            this.zOffset = this.desiredZOffset;
            this.baseTurnSpeed = this.GetComponent<CharacterDirection>().turnSpeed;
            this.baseSprintValue = this.characterBody.sprintingSpeedMultiplier;

            this.flamethrowerEffectInstance = GameObject.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/DroneFlamethrowerEffect.prefab").WaitForCompletion());

            Destroy(this.flamethrowerEffectInstance.GetComponent<DestroyOnTimer>());
            Destroy(this.flamethrowerEffectInstance.GetComponent<ScaleParticleSystemDuration>());;
            Destroy(this.flamethrowerEffectInstance.GetComponent<DetachParticleOnDestroyAndEndEmission>());
            Destroy(this.flamethrowerEffectInstance.GetComponentInChildren<DynamicBone>());

            //this.flamethrowerEffectInstance.transform.Find("Donut").gameObject.SetActive(true);
            this.flamethrowerEffectInstance.transform.Find("Bone1/Bone2/Bone3/Bone4/FireForward").gameObject.SetActive(false);
            //this.flamethrowerEffectInstance.transform.Find("Donut").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOpaqueDustSpeckled.mat").WaitForCompletion();
            this.flamethrowerEffectInstance.transform.Find("FireForward (1)").gameObject.SetActive(true);

            foreach (ParticleSystem i in this.flamethrowerEffectInstance.GetComponentsInChildren<ParticleSystem>())
            {
                var main = i.main;
                main.loop = true;
                main.playOnAwake = false;
                i.Stop();
            }

            this.flamethrowerLight = this.flamethrowerEffectInstance.GetComponentInChildren<Light>().gameObject;

            this.flamethrowerEffectInstance.transform.parent = this.childLocator.FindChild("MuzzleSMG");
            this.flamethrowerEffectInstance.transform.localPosition = Vector3.zero;
            this.flamethrowerEffectInstance.transform.localRotation = Quaternion.identity;

            this.flamethrowerEffectInstance2 = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("FlamethrowerEffect"));
            this.flamethrowerEffectInstance2.transform.parent = this.childLocator.FindChild("MuzzleSMG");
            this.flamethrowerEffectInstance2.transform.localPosition = Vector3.zero;
            this.flamethrowerEffectInstance2.transform.localRotation = Quaternion.identity;
            this.flamethrowerEffectInstance2.transform.localScale = Vector3.one;

            foreach (ParticleSystem i in this.flamethrowerEffectInstance2.GetComponentsInChildren<ParticleSystem>())
            {
                var main = i.main;
                main.loop = true;
                main.playOnAwake = false;
                i.Stop();
            }

            this.flamethrowerEffectInstance.SetActive(false);
            this.flamethrowerEffectInstance2.SetActive(false);

            this.fancyShield = Modules.Config.shieldBubble.Value;
            this.shieldOverlay = this.childLocator.FindChild("ShieldOverlay").gameObject;
            this.shieldOverlay.GetComponent<MeshRenderer>().material = Modules.Assets.shieldMat;
            if (this.fancyShield) this.shieldOverlay.AddComponent<HunkShieldHandler>();

            if (MainPlugin.moreShrinesInstalled) this.gameObject.AddComponent<IncompatWarning>();

            this.Invoke("SetInventoryHook", 0.5f);
        }

        private void Start()
        {
            this.InitShells();

            if (this.characterBody)
            {
                CameraTargetParams ctp = this.characterBody.GetComponent<CameraTargetParams>();
                if (ctp)
                {
                    if (Modules.Config.overTheShoulderCamera.Value && !Modules.Config.overTheShoulderCamera2.Value)
                    {
                        ctp.cameraPivotTransform = this.childLocator.FindChild("CameraTracker");
                    }
                }
            }

            this.Invoke("Init", 0.3f);

            //SpawnChests();
            this.SpawnTerminal();
        }

        private void Init()
        {
            this.EquipWeapon(this.weaponTracker.equippedIndex);

            if (Modules.Config.customEscapeSequence.Value)
            {
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "moon" || UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "moon2") this.StartDialogue1();
            }
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
                    i.SetHunk(this);
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
            if (this.weaponDef == Modules.Weapons.SMG.instance.weaponDef)
            {
                if (this.characterBody.inventory && this.characterBody.inventory.GetItemCount(Modules.Weapons.SMG.extendedMag) > 0)
                {
                    this.maxAmmo = 60;
                }
            }

            this.CheckForNeedler();

            if (this.characterBody.inventory.GetItemCount(DLC1Content.Items.MissileVoid) > 0)
            {
                this.shieldIsVoid = true;
            }
            else this.shieldIsVoid = false;
        }

        public void ConsumeAmmo(int amount = 1)
        {
            this.reloadTimer = 2f;

            if (this.characterBody.HasBuff(RoR2Content.Buffs.NoCooldowns)) return;

            // fake ammo sync
            if (this.ammo <= this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].currentAmmo) this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].currentAmmo -= amount;

            this.ammo -= amount;

            if (this.ammo <= 0) this.ammo = 0;
            if (this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].currentAmmo <= 0) this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].currentAmmo = 0;

            if (this.weaponDef.animationSet == HunkWeaponDef.AnimationSet.Pistol) this.Invoke("BulletDrop", UnityEngine.Random.Range(0.5f, 0.6f));
        }

        public void ApplyBandolier()
        {
            if (this.ammo > this.maxAmmo) return;
            if (this.ammo - this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].currentAmmo >= this.maxAmmo && this.ammo != this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].currentAmmo) return;

            this.ammo += Mathf.CeilToInt(this.maxAmmo * 1f);
            if (this.ammo > this.maxAmmo) this.ammo = this.maxAmmo;
        }

        private void FixedUpdate()
        {
            this.reloadTimer -= Time.fixedDeltaTime;
            this.lockOnTimer -= Time.fixedDeltaTime;
            this.ammoKillTimer -= Time.fixedDeltaTime;
            this.iFrames -= Time.fixedDeltaTime;
            this.flamethrowerLifetime -= Time.fixedDeltaTime;

            if (this.characterBody)
            {
                if (this.immobilized)
                {
                    this.characterBody.moveSpeed = 0f;
                    this.animator.SetBool("isMoving", false);
                }

                if (this._wasImmobilized && !this.immobilized) this.characterBody.RecalculateStats();
                this._wasImmobilized = this.immobilized;

                this.HandleShield();
                this.HandleSprint();
            }

            if (NetworkServer.active)
            {
                if (this.characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
                {
                    if (this.iFrames <= 0f)
                    {
                        this.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                    }
                }
                else
                {
                    if (this.iFrames > 0f)
                    {
                        this.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
                    }
                }
            }

            if (this.flamethrowerLifetime <= 0f)
            {
                if (this.flameIsPlaying)
                {
                    this.flameIsPlaying = false;
                    foreach (ParticleSystem i in this.flamethrowerEffectInstance.GetComponentsInChildren<ParticleSystem>())
                    {
                        i.Stop();
                    }
                    foreach (ParticleSystem i in this.flamethrowerEffectInstance2.GetComponentsInChildren<ParticleSystem>())
                    {
                        i.Stop();
                    }
                    this.flamethrowerLight.SetActive(false);
                    AkSoundEngine.StopPlayingID(this.flamethrowerPlayID);

                    if (this.flameInit) Util.PlaySound("sfx_hunk_flamethrower_end", this.gameObject);
                    this.flameInit = true;
                }
            }
            else
            {
                if (this.weaponDef.nameToken.Contains("FLAME"))
                {
                    if (!this.flameIsPlaying)
                    {
                        this.flameIsPlaying = true;

                        if (!this.flamethrowerEffectInstance.activeSelf) this.flamethrowerEffectInstance.SetActive(true);
                        if (!this.flamethrowerEffectInstance2.activeSelf) this.flamethrowerEffectInstance2.SetActive(true);

                        foreach (ParticleSystem i in this.flamethrowerEffectInstance.GetComponentsInChildren<ParticleSystem>())
                        {
                            i.Play();
                        }
                        foreach (ParticleSystem i in this.flamethrowerEffectInstance2.GetComponentsInChildren<ParticleSystem>())
                        {
                            i.Play();
                        }
                        this.flamethrowerLight.SetActive(true);
                        this.flamethrowerPlayID = Util.PlaySound("sfx_hunk_flamethrower_loop", this.gameObject);
                    }
                }
            }

            if (this.animator)
            {
                this.animator.SetBool("isRolling", this.isRolling);
            }

            if (this.ammo <= 0)
            {
                if (!this.isOut) this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(this.characterBody, this.emptyCrosshair, CrosshairUtils.OverridePriority.Skill);
                this.isOut = true;
            }
            else
            {
                if (this.isOut) this.crosshairOverrideRequest.Dispose();
                this.isOut = false;
            }

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
            else
            {
                this.cameraController.fadeEndDistance = 2f;
                this.cameraController.fadeStartDistance = -5f;

                if (!this.dodgeFlash)
                {
                    this.dodgeFlash = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("DodgeFlash")).GetComponent<Animator>();
                    this.dodgeFlash.transform.parent = this.cameraController.hud.mainContainer.transform;
                    this.dodgeFlash.gameObject.SetActive(false);

                    RectTransform rect = this.dodgeFlash.GetComponent<RectTransform>();
                    rect.sizeDelta = Vector2.one;
                    rect.localPosition = Vector3.zero;
                }

                if (!this.counterFlash)
                {
                    this.counterFlash = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("CounterFlash")).GetComponent<Animator>();
                    this.counterFlash.transform.parent = this.cameraController.hud.mainContainer.transform;
                    this.counterFlash.gameObject.SetActive(false);

                    RectTransform rect = this.counterFlash.GetComponent<RectTransform>();
                    rect.sizeDelta = Vector2.one;
                    rect.localPosition = Vector3.zero;
                }

                if (!this.speedLines)
                {
                    this.speedLines = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("SpeedLines")).GetComponent<ParticleSystem>();
                    this.speedLines.transform.parent = this.cameraController.sceneCam.transform;
                    this.speedLines.transform.localPosition = new Vector3(0f, 0f, 5f);
                    this.speedLines.transform.localRotation = Quaternion.identity;
                    this.speedLines.transform.localScale = Vector3.one;
                    this.speedLines.gameObject.layer = 21;
                }
                else
                {
                    if (this.lockOnTimer > 1.2f)
                    {
                        if (!this.speedLines.isPlaying) this.speedLines.Play();
                    }
                    else
                    {
                        if (this.speedLines.isPlaying) this.speedLines.Stop();
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

            //this.yOffset = Mathf.Lerp(this.yOffset, this.desiredYOffset, 5f * Time.fixedDeltaTime);
            //this.zOffset = Mathf.Lerp(this.zOffset, this.desiredZOffset, 5f * Time.fixedDeltaTime);
            //this.cameraPivot.localPosition = new Vector3(0f, this.yOffset, this.zOffset);
        }

        public void TriggerDodge()
        {
            if (this.dodgeFlash)
            {
                this.dodgeFlash.gameObject.SetActive(false);
                this.dodgeFlash.gameObject.SetActive(true);
            }
        }

        public void TriggerCounter()
        {
            if (this.counterFlash)
            {
                this.counterFlash.gameObject.SetActive(false);
                this.counterFlash.gameObject.SetActive(true);
            }

            this.counterCount++;
            if (onCounter == null) return;
            onCounter(this.counterCount);
        }

        /*private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                this.gameObject.AddComponent<HunkDialogue>();
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                this.gameObject.AddComponent<HunkDialogue2>();
            }
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
            if (!this.weaponDef.allowAutoReload) return;
            if (!this.characterBody.hasAuthority) return;

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

        public void ServerDropWeapon(int index)
        {
            if (!NetworkServer.active) return;

            NetworkIdentity identity = this.GetComponent<NetworkIdentity>();
            if (!identity) return;

            new SyncGunDrop2(identity.netId, index).Send(NetworkDestination.Clients);
        }

        public void ClientDropWeapon(int index)
        {
            this.weaponTracker.DropWeapon(index);
        }

        public void ServerSetWeapon(int index)
        {
            if (!NetworkServer.active) return;

            this.weaponTracker.nextWeapon = index;
        }

        public void ServerGetAmmo(float multiplier = 1f)
        {
            NetworkIdentity identity = this.GetComponent<NetworkIdentity>();
            if (!identity) return;

            bool valid = false;
            int index = 0;

            while (!valid)
            {
                index = UnityEngine.Random.Range(0, this.weaponTracker.weaponData.Length);
                if (this.weaponTracker.weaponData[index].weaponDef.canPickUpAmmo) valid = true;
            }

            new SyncAmmoPickup(identity.netId, multiplier, index).Send(NetworkDestination.Clients);
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
            int magSize = this.weaponDef.magSize;

            /*if (this.characterBody.master)
            {
                if (this.characterBody.master.inventory)
                {
                    magSize *= 1 + this.characterBody.master.inventory.GetItemCount(RoR2Content.Items.LunarBadLuck);
                }
            }*/

            if (this.weaponDef == Modules.Weapons.SMG.instance.weaponDef)
            {
                if (this.characterBody.inventory && this.characterBody.inventory.GetItemCount(Modules.Weapons.SMG.extendedMag) > 0)
                {
                    magSize = 60;
                }
            }

            if (this.ammo + this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].totalAmmo >= magSize)
            {
                this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].currentAmmo = magSize;
                this.ammo = magSize;
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

        public void FinishRoundReload()
        {
            if (this.characterBody.master)
            {
                if (this.characterBody.master.inventory)
                {
                    this.ammo += this.characterBody.master.inventory.GetItemCount(RoR2Content.Items.SecondarySkillMagazine);
                }
            }
        }

        private void BulletDrop()
        {
            Util.PlaySound("sfx_hunk_bullet_drop", this.gameObject);
        }

        public void ReloadRound()
        {
            // ugh
            if (this.weaponDef.animationSet == HunkWeaponDef.AnimationSet.Pistol) Util.PlaySound("sfx_hunk_revolver_load", this.gameObject);
            else Util.PlaySound("sfx_hunk_shotgun_load", this.gameObject);

            this.ammo++;
            this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].currentAmmo++;
            this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].totalAmmo--;

            if (this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].totalAmmo <= 0) this.weaponTracker.weaponData[this.weaponTracker.equippedIndex].totalAmmo = 0;
        }

        public void PickUpWeapon(HunkWeaponDef newWeapon)
        {
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

            if (this.weaponDef == Modules.Weapons.SMG.instance.weaponDef)
            {
                if (this.weaponTracker.weaponData[index].weaponDef == Modules.Weapons.SMG.instance.weaponDef)
                {
                    if (this.characterBody.inventory && this.characterBody.inventory.GetItemCount(Modules.Weapons.SMG.extendedMag) > 0)
                    {
                        this.maxAmmo = 60;
                    }
                }
            }

            // model swap
            if (this.weaponDef.modelPrefab)
            {
                this.weaponRenderer.gameObject.SetActive(false);
                if (this.heldWeaponInstance) Destroy(this.heldWeaponInstance);

                GameObject modelPrefab = this.weaponDef.modelPrefab;
                // #swuff
                // if your current skin has an override just swap the prefab out here and done
                this.heldWeaponInstance = GameObject.Instantiate(modelPrefab);
                this.heldWeaponInstance.transform.parent = this.childLocator.FindChild("Weapon");
                this.heldWeaponInstance.transform.localPosition = Vector3.zero;
                this.heldWeaponInstance.transform.localRotation = Quaternion.identity;
                this.heldWeaponInstance.transform.localScale = Vector3.one;
            }
            else
            {
                // this just defaults to an smg if your wepaon doesn't have a model
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

            this.ToggleLayer("Body (Pistol2)", false);
            this.ToggleLayer("Gesture (Pistol2)", false);
            this.ToggleLayer("FullBody (Pistol2)", false);

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
                case HunkWeaponDef.AnimationSet.PistolAlt:
                    this.ToggleLayer("Body (Pistol2)", true);
                    this.ToggleLayer("Gesture (Pistol2)", true);
                    this.ToggleLayer("FullBody (Pistol2)", true);
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

        public void HandleBackWeapon()
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
                if (this.weaponTracker.lastEquippedIndex == this.weaponTracker.equippedIndex) return;

                this.backWeaponDef = this.weaponTracker.weaponData[this.weaponTracker.lastEquippedIndex].weaponDef;

                GameObject modelPrefab = this.backWeaponDef.modelPrefab;
                // #swuff
                // same thing here. replace with skin override
                this.backWeaponInstance = GameObject.Instantiate(modelPrefab);
                this.backWeaponInstance.transform.parent = this.childLocator.FindChild("BackWeapon");
                this.backWeaponInstance.transform.localPosition = new Vector3(5f, 0f, 0f);
                this.backWeaponInstance.transform.localRotation = Quaternion.Euler(new Vector3(345f, 90f, 15f));
                this.backWeaponInstance.transform.localScale = Vector3.one;

                DynamicBone fuckYou = this.childLocator.FindChild("BackWeapon").GetComponent<DynamicBone>();
                if (fuckYou) DestroyImmediate(fuckYou);

                Transform why = this.childLocator.FindChild("BackWeapon").parent;
                why.transform.localRotation = Quaternion.Euler(new Vector3(25f, 0f, 180f));

                DynamicBone hehe = this.childLocator.FindChild("BackWeapon").gameObject.AddComponent<DynamicBone>();
                hehe.m_Root = hehe.transform.parent;
                hehe.m_Damping = 0.005f;
                hehe.m_Elasticity = 0.01f;
                hehe.m_Stiffness = 0.5f;
                hehe.m_Inert = 0.824f;
                hehe.m_Gravity = Vector3.zero;
                hehe.m_DistantDisable = true;
                hehe.m_DistanceToObject = 200f;
                hehe.m_FreezeAxis = DynamicBone.FreezeAxis.Y;// | DynamicBone.FreezeAxis.Z;
                hehe.m_Exclusions = new List<Transform>();
                hehe.m_Exclusions.Add(this.backWeaponInstance.transform);
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

        public void StartBGM()
        {
            this.bgmPlayID = Util.PlaySound("bgm_hunk_loomingdread", this.gameObject);
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

            if (this.speedLines) Destroy(this.speedLines.gameObject);
            if (this.dodgeFlash) Destroy(this.dodgeFlash.gameObject);

            AkSoundEngine.StopPlayingID(this.flamethrowerPlayID);
            AkSoundEngine.StopPlayingID(this.bgmPlayID);
        }

        public void AddRandomAmmo(float multiplier = 1f)
        {
            if (this.passive.isFullArsenal) return;
            // TODO
            // change this to a weighted selection, so stronger weapons are less likely to get ammo

            // alien head
            if (this.characterBody && this.characterBody.inventory)
            {
                int alienHeadCount = this.characterBody.inventory.GetItemCount(RoR2Content.Items.AlienHead);
                if (alienHeadCount > 0)
                {
                    for (int i = 0; i < alienHeadCount; i++)
                    {
                        if (MainPlugin.greenAlienHeadInstalled)
                        {
                            multiplier *= 1.15f;
                        }
                        else
                        {
                            multiplier *= 1.25f;
                        }
                    }
                }
            }


            bool valid = false;
            int index = 0;

            while (!valid)
            {
                index = UnityEngine.Random.Range(0, this.weaponTracker.weaponData.Length);
                if (this.weaponTracker.weaponData[index].weaponDef.canPickUpAmmo) valid = true;
            }

            int amount = Mathf.CeilToInt(this.weaponTracker.weaponData[index].weaponDef.magSize * multiplier * this.weaponTracker.weaponData[index].weaponDef.magPickupMultiplier);

            this.weaponTracker.weaponData[index].totalAmmo += amount;

            GameObject effect = GameObject.Instantiate(Modules.Assets.ammoPickupEffectPrefab, this.characterBody.aimOrigin + (Vector3.up * 0.85f) + (this.characterBody.inputBank.aimDirection * 2f), Quaternion.identity);

            effect.GetComponentInChildren<RoR2.UI.LanguageTextMeshController>().token = "+" + amount + " " + this.weaponTracker.weaponData[index].weaponDef.ammoName;
        }

        public void AddAmmoFromIndex(int index, float multiplier = 1f)
        {
            if (this.passive.isFullArsenal) return;

            // alien head
            if (this.characterBody && this.characterBody.inventory)
            {
                float baseMult = 1.25f;
                if (MainPlugin.greenAlienHeadInstalled) baseMult = 1.1f;

                int alienHeadCount = this.characterBody.inventory.GetItemCount(RoR2Content.Items.AlienHead);
                if (alienHeadCount > 0)
                {
                    for (int i = 0; i < alienHeadCount; i++)
                    {
                        if (MainPlugin.greenAlienHeadInstalled)
                        {
                            multiplier *= baseMult;
                            baseMult *= 1.1f - 1f;
                        }
                        else
                        {
                            multiplier *= baseMult;
                            baseMult *= 1.25f - 1f;
                        }
                    }
                }
            }

            int magSize = this.weaponTracker.weaponData[index].weaponDef.magSize;

            if (this.weaponTracker.weaponData[index].weaponDef == Modules.Weapons.SMG.instance.weaponDef)
            {
                if (this.characterBody.inventory && this.characterBody.inventory.GetItemCount(Modules.Weapons.SMG.extendedMag) > 0)
                {
                    magSize = 90;
                }
            }

            int amount = Mathf.CeilToInt(magSize * multiplier * this.weaponTracker.weaponData[index].weaponDef.magPickupMultiplier);

            this.weaponTracker.weaponData[index].totalAmmo += amount;

            GameObject effect = GameObject.Instantiate(Modules.Assets.ammoPickupEffectPrefab, this.characterBody.aimOrigin + (Vector3.up * 0.85f) + (this.characterBody.inputBank.aimDirection * 2f), Quaternion.identity);

            effect.GetComponentInChildren<RoR2.UI.LanguageTextMeshController>().token = "+" + amount + " " + this.weaponTracker.weaponData[index].weaponDef.ammoName;
            Util.PlaySound("sfx_hunk_pickup", this.gameObject);
        }

        private void SpawnChests()
        {
            if (!NetworkServer.active) return;

            Xoroshiro128Plus rng = new Xoroshiro128Plus(Run.instance.seed);
            DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(Survivors.Hunk.chestInteractableCard, new DirectorPlacementRule { placementMode = DirectorPlacementRule.PlacementMode.Random }, rng));
        }

        private void SpawnTerminal()
        {
            if (!NetworkServer.active) return;
            if (!this.characterBody.isPlayerControlled) return;

            Xoroshiro128Plus rng = new Xoroshiro128Plus(Run.instance.seed);
            DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(Survivors.Hunk.terminalInteractableCard, new DirectorPlacementRule { placementMode = DirectorPlacementRule.PlacementMode.Random }, rng));
        }

        public void TrySpawnRocketLauncher()
        {
            this.Invoke("SpawnRocketLauncher", 10f);
        }

        public void SpawnRocketLauncher()
        {
            Util.PlaySound("sfx_hunk_weapon_case_drop", this.gameObject);

            if (!NetworkServer.active) return;

            System.Random random = new System.Random();
            NodeGraph groundNodes = SceneInfo.instance.groundNodes;
            List<NodeGraph.NodeIndex> nodeList = groundNodes.FindNodesInRange(transform.position, 3f, 16f, HullMask.Human);
            NodeGraph.NodeIndex randomNode = nodeList[random.Next(nodeList.Count)];
            if (randomNode != null)
            {
                Vector3 position;
                groundNodes.GetNodePosition(randomNode, out position);
                GameObject rocketLauncherChest = Instantiate(Survivors.Hunk.weaponCasePrefab, position, Quaternion.identity);
                
                WeaponChest weaponChest = GetComponent<WeaponChest>();
                if (weaponChest != null)
                {
                    weaponChest.chestType = 0;
                    weaponChest.itemDef = Weapons.RocketLauncher.instance.itemDef;
                }

                NetworkServer.Spawn(rocketLauncherChest);

                EffectManager.SpawnEffect(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SurvivorPod/PodGroundImpact.prefab").WaitForCompletion(),
    new EffectData
    {
        origin = position,
        rotation = Quaternion.identity,
        scale = 2f
    }, true);
            }
            //DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(Survivors.Hunk.terminalInteractableCard, new DirectorPlacementRule { placementMode = DirectorPlacementRule.PlacementMode.NearestNode }, rng));
        }

        public void StartDialogue1()
        {
            if (!this.characterBody.hasAuthority || !this.characterBody.isPlayerControlled) return;
            if (this.weaponTracker.wtfTheFuck) return;

            this.weaponTracker.wtfTheFuck = true;
            this.gameObject.AddComponent<HunkDialogue>();
        }

        public void StartDialogue2()
        {
            if (!this.characterBody.hasAuthority || !this.characterBody.isPlayerControlled) return;

            this.gameObject.AddComponent<HunkDialogue2>();
        }

        public SkillDef knifeSkin
        {
            get
            {
                return this.knifeSkinSkillSlot.skillDef;
            }
        }

        private void HandleShield()
        {
            if (!this.characterBody) return;
            if (!this.characterBody.healthComponent) return;
            if (!this.shieldOverlay) return;
            if (!this.fancyShield)
            {
                this.shieldOverlay.SetActive(false);
                return;
            }

            if (this.characterBody.healthComponent.shield > 0)
            {
                this.shieldOverlay.SetActive(this.fancyShield);
            }
            else
            {
                this.shieldOverlay.SetActive(false);
            }

            if (this._shieldIsVoid != this.shieldIsVoid)
            {
                if (this.shieldIsVoid) this.shieldOverlay.GetComponent<MeshRenderer>().material = Modules.Assets.voidShieldMat;
                else this.shieldOverlay.GetComponent<MeshRenderer>().material = Modules.Assets.shieldMat;
            }

            this._shieldIsVoid = this.shieldIsVoid;
        }

        private void HandleSprint()
        {
            if (!this.characterBody) return;

            if (this.characterBody.isSprinting)
            {
                this.sprintStopwatch += Time.fixedDeltaTime;

                if (this.sprintStopwatch >= 1.5f)
                {
                    float value = Mathf.Clamp(this.sprintStopwatch, 0f, 5f);
                    this.sprintMultiplier = Util.Remap(value, 1.5f, 5f, 1f, 1.25f);
                }
            }
            else
            {
                this.sprintStopwatch = 0f;
                this.sprintMultiplier = 1f;
            }

            this.characterBody.sprintingSpeedMultiplier = this.baseSprintValue * this.sprintMultiplier;
            if (this.lastSprintMultiplier != this.sprintMultiplier) this.characterBody.RecalculateStats();
            this.lastSprintMultiplier = this.sprintMultiplier;
        }
    }
}