using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;
using RoR2.CharacterAI;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using RoR2.UI;
using System.Linq;
using HunkMod.Modules.Components;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine.SceneManagement;
using RoR2.Orbs;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;

namespace HunkMod.Modules.Survivors
{
    internal class Hunk
    {
        internal static Hunk instance;

        internal static GameObject characterPrefab;
        internal static GameObject displayPrefab;

        internal static GameObject umbraMaster;

        internal static ConfigEntry<bool> forceUnlock;
        internal static ConfigEntry<bool> characterEnabled;

        internal float pityMultiplier = 1f;

        public static Color characterColor = new Color(127f / 255f, 0f, 0f);

        public const string bodyName = "RobHunkBody";

        public static int bodyRendererIndex; // use this to store the rendererinfo index containing our character's body
                                             // keep it last in the rendererinfos because teleporter particles for some reason require this. hopoo pls

        // item display stuffs
        internal static ItemDisplayRuleSet itemDisplayRuleSet;
        internal static List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules;

        internal static UnlockableDef characterUnlockableDef;
        internal static UnlockableDef masteryUnlockableDef;
        internal static UnlockableDef cqcUnlockableDef;
        internal static UnlockableDef lightweightUnlockableDef;
        internal static UnlockableDef earlySupporterUnlockableDef;
        internal static UnlockableDef completionUnlockableDef;

        // skill overrides
        internal static SkillDef reloadSkillDef;
        internal static SkillDef counterSkillDef;
        internal static SkillDef scepterDodgeSkillDef;
        internal static SkillDef scepterCounterSkillDef;

        internal static SkillDef confirmSkillDef;
        internal static SkillDef cancelSkillDef;

        internal static EntityStates.SerializableEntityStateType airDodgeState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.AirDodge));

        internal static string bodyNameToken;

        internal static GameObject spawnPodPrefab;
        internal static GameObject podPanelPrefab;
        internal static GameObject podContentPrefab;
        internal static Material miliMat;

        public static List<HunkWeaponDef> defaultWeaponPool = new List<HunkWeaponDef>();
        public static List<ItemDef> spawnedWeaponList = new List<ItemDef>();
        public static List<GameObject> virusObjectiveObjects = new List<GameObject>();
        public static List<GameObject> virusObjectiveObjects2 = new List<GameObject>();
        public static List<CostTypeDef> spawnedCostTypeList = new List<CostTypeDef>();

        public static string stageBlacklist = "arena,artifactworld,bazaar,limbo,moon,moon2,outro,voidoutro,voidraid,voidstage";
        public static List<string> blacklistedStageNames = new List<string>();

        public HealthBarStyle infectedHealthBarStyle;
        public HealthBarStyle tInfectedHealthBarStyle;

        public static InteractableSpawnCard chestInteractableCard;
        internal static GameObject weaponChestPrefab;

        public static InteractableSpawnCard caseInteractableCard;
        internal static GameObject weaponCasePrefab;

        public static CostTypeDef heartCostDef;
        public static int heartCostTypeIndex;

        public static CostTypeDef spadeCostDef;
        public static int spadeCostTypeIndex;

        public static CostTypeDef clubCostDef;
        public static int clubCostTypeIndex;

        public static CostTypeDef diamondCostDef;
        public static int diamondCostTypeIndex;

        public static CostTypeDef starCostDef;
        public static int starCostTypeIndex;

        public static CostTypeDef wristbandCostDef;
        public static int wristbandCostTypeIndex;

        public static InteractableSpawnCard terminalInteractableCard;
        internal static GameObject terminalPrefab;

        public static CostTypeDef sampleCostDef;
        public static int sampleCostTypeIndex;

        internal GameObject ammoPickupInteractable;
        internal GameObject ammoPickupInteractableSmall;

        internal static ItemDef spadeKeycard;
        internal static ItemDef clubKeycard;
        internal static ItemDef heartKeycard;
        internal static ItemDef diamondKeycard;
        internal static ItemDef wristband;
        internal static ItemDef goldKeycard;
        internal static ItemDef masterKeycard;
        internal static ItemDef gVirusSample;
        internal static ItemDef gVirus;
        internal static ItemDef gVirus2;
        internal static ItemDef gVirusFinal;
        internal static ItemDef tVirusSample;
        internal static ItemDef tVirus;
        internal static ItemDef tVirusRevival;
        internal static ItemDef ammoItem;

        // orb
        internal static GameObject ammoOrb;

        // knife skins
        public static Dictionary<SkillDef, GameObject> knifeSkins = new Dictionary<SkillDef, GameObject>();

        // knife skilldefs
        public static SkillDef defaultKnifeDef;
        public static SkillDef hiddenKnifeDef;
        public static SkillDef infiniteKnifeDef;
        internal static UnlockableDef infiniteKnifeUnlockableDef;
        public static SkillDef bloodyKnifeDef;
        //internal static UnlockableDef bloodyKnifeUnlockableDef;
        public static SkillDef weskerKnifeDef;
        internal static UnlockableDef weskerKnifeUnlockableDef;
        public static SkillDef macheteKnifeDef;
        internal static UnlockableDef macheteKnifeUnlockableDef;
        public static SkillDef re4KnifeDef;
        internal static UnlockableDef re4KnifeUnlockableDef;

        internal static BuffDef immobilizedBuff;
        internal static BuffDef infectedBuff;
        internal static BuffDef infectedBuff2;
        internal static BuffDef mangledBuff;

        public static int requiredKills = 0;

        public static List<GolemLaser> golemLasers = new List<GolemLaser>();

        internal void CreateCharacter()
        {
            instance = this;

            characterEnabled = Modules.Config.CharacterEnableConfig("Hunk");

            if (characterEnabled.Value)
            {
                forceUnlock = Modules.Config.ForceUnlockConfig("Hunk");

                //if (!forceUnlock.Value) characterUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.DriverUnlockAchievement>();
                masteryUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.HunkMonsoonAchievement>();
                cqcUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.HunkCQCAchievement>();
                lightweightUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.HunkLightweightAchievement>();
                earlySupporterUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.HunkSupporterAchievement>();
                completionUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.HunkCompletionAchievement>();
                infiniteKnifeUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.HunkInfiniteKnifeAchievement>();
                //bloodyKnifeUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.HunkCompletionAchievement>();
                weskerKnifeUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.HunkWeskerKnifeAchievement>();
                macheteKnifeUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.HunkMacheteAchievement>();
                re4KnifeUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.HunkLeonKnifeAchievement>();

                CreateKeycards();
                CreateAmmoInteractable();
                CreateBarrelAmmoInteractable();
                CreateChest();
                CreateCase();
                CreateTerminal();
                CreatePod();
                CreateHealthBarStyle();
                CreateOrb();

                characterPrefab = CreateBodyPrefab(true);

                displayPrefab = Modules.Prefabs.CreateDisplayPrefab("HunkDisplay", characterPrefab);
                ChildLocator childLocator = displayPrefab.GetComponentInChildren<ChildLocator>();
                childLocator.FindChild("KnifeModel").gameObject.SetActive(false);
                childLocator.FindChild("HiddenKnifeModel").gameObject.SetActive(false);

                Modules.Prefabs.RegisterNewSurvivor(characterPrefab, displayPrefab, "HUNK");
                //if (forceUnlock.Value) Modules.Prefabs.RegisterNewSurvivor(characterPrefab, displayPrefab, "DRIVER");
                //else Modules.Prefabs.RegisterNewSurvivor(characterPrefab, displayPrefab, "DRIVER", characterUnlockableDef);

                umbraMaster = CreateMaster(characterPrefab, "RobHunkMonsterMaster");
                
                immobilizedBuff = Modules.Buffs.AddNewBuff("buffHunkImmobilized", null, Color.white, false, false, true);
                infectedBuff = Modules.Buffs.AddNewBuff("buffHunkInfected", null, Color.yellow, false, false, true);
                infectedBuff2 = Modules.Buffs.AddNewBuff("buffHunkInfected2", null, Color.blue, false, false, true);
                mangledBuff = Modules.Buffs.AddNewBuff("buffHunkMangled", Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Bandit2/bdSuperBleed.asset").WaitForCompletion().iconSprite, Color.red, true, false);

                AddVirusDisplayRules();
                CreateKnifeSkins();
            }

            Hook();
        }

        private static GameObject CreateBodyPrefab(bool isPlayer)
        {
            bodyNameToken = MainPlugin.developerPrefix + "_HUNK_BODY_NAME";

            #region Body
            GameObject newPrefab = Modules.Prefabs.CreatePrefab("RobHunkBody", "mdlHunk", new BodyInfo
            {
                armor = Config.baseArmor.Value,
                armorGrowth = Config.armorGrowth.Value,
                bodyName = "RobHunkBody",
                bodyNameToken = bodyNameToken,
                bodyColor = characterColor,
                characterPortrait = Modules.Assets.LoadCharacterIcon("Hunk"),
                crosshair = Modules.Assets.LoadCrosshair("Standard"),
                damage = Config.baseDamage.Value,
                healthGrowth = Config.healthGrowth.Value,
                healthRegen = Config.baseRegen.Value,
                jumpCount = 1,
                maxHealth = Config.baseHealth.Value,
                subtitleNameToken = MainPlugin.developerPrefix + "_HUNK_BODY_SUBTITLE",
                podPrefab = spawnPodPrefab, //RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),
                moveSpeed = Config.baseMovementSpeed.Value,
                acceleration = 60f,
                jumpPower = 15f,
                attackSpeed = 1f,
                crit = Config.baseCrit.Value
            });

            ChildLocator childLocator = newPrefab.GetComponentInChildren<ChildLocator>();

            childLocator.gameObject.AddComponent<Modules.Components.HunkAnimationEvents>();

            CharacterBody body = newPrefab.GetComponent<CharacterBody>();
            //body.preferredInitialStateType = new EntityStates.SerializableEntityStateType(typeof(SpawnState));
            //body.bodyFlags = CharacterBody.BodyFlags.IgnoreFallDamage;
            //body.bodyFlags |= CharacterBody.BodyFlags.SprintAnyDirection;
            //body.sprintingSpeedMultiplier = 1.75f;
            body.hideCrosshair = true;
            body.overrideCoreTransform = childLocator.FindChild("Chest");

            SfxLocator sfx = newPrefab.GetComponent<SfxLocator>();
            //sfx.barkSound = "";
            //sfx.landingSound = "sfx_hunk_land";
            sfx.deathSound = "sfx_hunk_death";
            //sfx.fallDamageSound = "";

            FootstepHandler footstep = newPrefab.GetComponentInChildren<FootstepHandler>();
            //footstep.footstepDustPrefab = Resources.Load<GameObject>("Prefabs/GenericHugeFootstepDust");
            footstep.baseFootstepString = "sfx_hunk_footstep";
            footstep.sprintFootstepOverrideString = "sfx_hunk_sprint";

            //KinematicCharacterMotor characterController = newPrefab.GetComponent<KinematicCharacterMotor>();
            //characterController.CapsuleRadius = 4f;
            //characterController.CapsuleHeight = 9f;

            //CharacterDirection direction = newPrefab.GetComponent<CharacterDirection>();
            //direction.turnSpeed = 135f;

            //Interactor interactor = newPrefab.GetComponent<Interactor>();
            //interactor.maxInteractionDistance = 8f;

            newPrefab.GetComponent<CameraTargetParams>().cameraParams = Modules.CameraParams.CreateCameraParamsWithData(HunkCameraParams.DEFAULT);

            //newPrefab.GetComponent<CharacterDirection>().turnSpeed = 720f;

            newPrefab.GetComponent<EntityStateMachine>().mainStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.MainState));

            // this is for aiming
            EntityStateMachine stateMachine = newPrefab.AddComponent<EntityStateMachine>();
            stateMachine.customName = "Aim";
            stateMachine.initialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
            stateMachine.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));

            //var state = isPlayer ? typeof(EntityStates.SpawnTeleporterState) : typeof(SpawnState);
            //newPrefab.GetComponent<EntityStateMachine>().initialStateType = new EntityStates.SerializableEntityStateType(state);

            // schizophrenia
            newPrefab.GetComponent<CharacterDeathBehavior>().deathState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.FuckMyAss));

            newPrefab.AddComponent<Modules.Components.HunkController>();
            if (Modules.Config.overTheShoulderCamera.Value) newPrefab.AddComponent<Modules.Components.HunkCameraAdjuster>();
            #endregion

            #region Model
            bodyRendererIndex = 0;

            Modules.Prefabs.SetupCharacterModel(newPrefab, new CustomRendererInfo[] {
                new CustomRendererInfo
                {
                    childName = "Model01",
                    material = Modules.Assets.CreateMaterial("matHunk01", 0f, Color.black, 1f)
                },
                new CustomRendererInfo
                {
                    childName = "Model02",
                    material = Modules.Assets.CreateMaterial("matHunk02", 0f, Color.black, 1f)
                },
                new CustomRendererInfo
                {
                    childName = "Model03",
                    material = Modules.Assets.CreateMaterial("matHunk03", 0f, Color.black, 1f)
                },
                new CustomRendererInfo
                {
                    childName = "Model04",
                    material = Modules.Assets.CreateMaterial("matHunk04", 0f, Color.black, 1f)
                },
                new CustomRendererInfo
                {
                    childName = "Model05",
                    material = Modules.Assets.CreateMaterial("matHunk05", 0f, Color.black, 1f)
                },
                new CustomRendererInfo
                {
                    childName = "Model06",
                    material = Modules.Assets.CreateMaterial("matHunk06", 1f, Color.white, 1f)
                },
                new CustomRendererInfo
                {
                    childName = "KnifeModel",
                    material = Modules.Assets.CreateMaterial("matKnife", 0f, Color.black, 1f)
                },
                new CustomRendererInfo
                {
                    childName = "HiddenKnifeModel",
                    material = Modules.Assets.CreateMaterial("matInfiniteKnife", 0f, Color.black, 1f),
                    ignoreOverlays = true
                },
                new CustomRendererInfo
                {
                    childName = "WeaponModel",
                    material = Modules.Assets.CreateMaterial("matSMG"),
                    ignoreOverlays = true
                } }, bodyRendererIndex);
            #endregion

            childLocator.FindChild("KnifeModel").gameObject.SetActive(false);
            childLocator.FindChild("HiddenKnifeModel").gameObject.SetActive(false);

            CreateHitboxes(newPrefab);
            SetupHurtboxes(newPrefab);
            CreateSkills(newPrefab);
            CreateSkins(newPrefab);
            InitializeItemDisplays(newPrefab);

            return newPrefab;
        }

        private static void SetupHurtboxes(GameObject bodyPrefab)
        {
            HurtBoxGroup hurtboxGroup = bodyPrefab.GetComponentInChildren<HurtBoxGroup>();
            List<HurtBox> hurtboxes = new List<HurtBox>();

            hurtboxes.Add(bodyPrefab.GetComponentInChildren<ChildLocator>().FindChild("MainHurtbox").GetComponent<HurtBox>());

            HealthComponent healthComponent = bodyPrefab.GetComponent<HealthComponent>();

            foreach (Collider i in bodyPrefab.GetComponent<ModelLocator>().modelTransform.GetComponentsInChildren<Collider>())
            {
                if (i.gameObject.name != "MainHurtbox")
                {
                    HurtBox hurtbox = i.gameObject.AddComponent<HurtBox>();
                    hurtbox.gameObject.layer = LayerIndex.entityPrecise.intVal;
                    hurtbox.healthComponent = healthComponent;
                    hurtbox.isBullseye = false;
                    hurtbox.damageModifier = HurtBox.DamageModifier.Normal;
                    hurtbox.hurtBoxGroup = hurtboxGroup;

                    hurtboxes.Add(hurtbox);
                }
            }

            hurtboxGroup.hurtBoxes = hurtboxes.ToArray();
        }

        private static GameObject CreateMaster(GameObject bodyPrefab, string masterName)
        {
            GameObject newMaster = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterMasters/LemurianMaster"), masterName, true);
            newMaster.GetComponent<CharacterMaster>().bodyPrefab = bodyPrefab;

            #region AI
            foreach (AISkillDriver ai in newMaster.GetComponentsInChildren<AISkillDriver>())
            {
                MainPlugin.DestroyImmediate(ai);
            }

            newMaster.GetComponent<BaseAI>().fullVision = true;

            AISkillDriver revengeDriver = newMaster.AddComponent<AISkillDriver>();
            revengeDriver.customName = "Revenge";
            revengeDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            revengeDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            revengeDriver.activationRequiresAimConfirmation = true;
            revengeDriver.activationRequiresTargetLoS = false;
            revengeDriver.selectionRequiresTargetLoS = true;
            revengeDriver.maxDistance = 24f;
            revengeDriver.minDistance = 0f;
            revengeDriver.requireSkillReady = true;
            revengeDriver.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            revengeDriver.ignoreNodeGraph = true;
            revengeDriver.moveInputScale = 1f;
            revengeDriver.driverUpdateTimerOverride = 2.5f;
            revengeDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            revengeDriver.minTargetHealthFraction = Mathf.NegativeInfinity;
            revengeDriver.maxTargetHealthFraction = Mathf.Infinity;
            revengeDriver.minUserHealthFraction = Mathf.NegativeInfinity;
            revengeDriver.maxUserHealthFraction = 0.5f;
            revengeDriver.skillSlot = SkillSlot.Utility;

            AISkillDriver grabDriver = newMaster.AddComponent<AISkillDriver>();
            grabDriver.customName = "Grab";
            grabDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            grabDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            grabDriver.activationRequiresAimConfirmation = true;
            grabDriver.activationRequiresTargetLoS = false;
            grabDriver.selectionRequiresTargetLoS = true;
            grabDriver.maxDistance = 8f;
            grabDriver.minDistance = 0f;
            grabDriver.requireSkillReady = true;
            grabDriver.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            grabDriver.ignoreNodeGraph = true;
            grabDriver.moveInputScale = 1f;
            grabDriver.driverUpdateTimerOverride = 0.5f;
            grabDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            grabDriver.minTargetHealthFraction = Mathf.NegativeInfinity;
            grabDriver.maxTargetHealthFraction = Mathf.Infinity;
            grabDriver.minUserHealthFraction = Mathf.NegativeInfinity;
            grabDriver.maxUserHealthFraction = Mathf.Infinity;
            grabDriver.skillSlot = SkillSlot.Primary;

            AISkillDriver stompDriver = newMaster.AddComponent<AISkillDriver>();
            stompDriver.customName = "Stomp";
            stompDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            stompDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            stompDriver.activationRequiresAimConfirmation = true;
            stompDriver.activationRequiresTargetLoS = false;
            stompDriver.selectionRequiresTargetLoS = true;
            stompDriver.maxDistance = 32f;
            stompDriver.minDistance = 0f;
            stompDriver.requireSkillReady = true;
            stompDriver.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            stompDriver.ignoreNodeGraph = true;
            stompDriver.moveInputScale = 0.4f;
            stompDriver.driverUpdateTimerOverride = 0.5f;
            stompDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            stompDriver.minTargetHealthFraction = Mathf.NegativeInfinity;
            stompDriver.maxTargetHealthFraction = Mathf.Infinity;
            stompDriver.minUserHealthFraction = Mathf.NegativeInfinity;
            stompDriver.maxUserHealthFraction = Mathf.Infinity;
            stompDriver.skillSlot = SkillSlot.Secondary;

            AISkillDriver followCloseDriver = newMaster.AddComponent<AISkillDriver>();
            followCloseDriver.customName = "ChaseClose";
            followCloseDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            followCloseDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            followCloseDriver.activationRequiresAimConfirmation = false;
            followCloseDriver.activationRequiresTargetLoS = false;
            followCloseDriver.maxDistance = 32f;
            followCloseDriver.minDistance = 0f;
            followCloseDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            followCloseDriver.ignoreNodeGraph = false;
            followCloseDriver.moveInputScale = 1f;
            followCloseDriver.driverUpdateTimerOverride = -1f;
            followCloseDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            followCloseDriver.minTargetHealthFraction = Mathf.NegativeInfinity;
            followCloseDriver.maxTargetHealthFraction = Mathf.Infinity;
            followCloseDriver.minUserHealthFraction = Mathf.NegativeInfinity;
            followCloseDriver.maxUserHealthFraction = Mathf.Infinity;
            followCloseDriver.skillSlot = SkillSlot.None;

            AISkillDriver followDriver = newMaster.AddComponent<AISkillDriver>();
            followDriver.customName = "Chase";
            followDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            followDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            followDriver.activationRequiresAimConfirmation = false;
            followDriver.activationRequiresTargetLoS = false;
            followDriver.maxDistance = Mathf.Infinity;
            followDriver.minDistance = 0f;
            followDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            followDriver.ignoreNodeGraph = false;
            followDriver.moveInputScale = 1f;
            followDriver.driverUpdateTimerOverride = -1f;
            followDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            followDriver.minTargetHealthFraction = Mathf.NegativeInfinity;
            followDriver.maxTargetHealthFraction = Mathf.Infinity;
            followDriver.minUserHealthFraction = Mathf.NegativeInfinity;
            followDriver.maxUserHealthFraction = Mathf.Infinity;
            followDriver.skillSlot = SkillSlot.None;
            followDriver.shouldSprint = true;
            #endregion

            Modules.Prefabs.masterPrefabs.Add(newMaster);

            return newMaster;
        }

        private static void CreateHitboxes(GameObject prefab)
        {
            ChildLocator childLocator = prefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;

            Transform hitboxTransform = childLocator.FindChild("KnifeHitbox");
            Modules.Prefabs.SetupHitbox(model, new Transform[]
                {
                    hitboxTransform
                }, "Knife");
        }

        private static void CreateSkills(GameObject prefab)
        {
            HunkPassive passive = prefab.AddComponent<HunkPassive>();
            HunkController hunk = prefab.GetComponent<HunkController>();

            string prefix = MainPlugin.developerPrefix;
            SkillLocator skillLocator = prefab.GetComponent<SkillLocator>();

            Modules.Skills.CreateSkillFamilies(prefab);

            skillLocator.passiveSkill.enabled = true;
            skillLocator.passiveSkill.skillNameToken = prefix + "_HUNK_BODY_PPASSIVE_NAME";
            skillLocator.passiveSkill.skillDescriptionToken = prefix + "_HUNK_BODY_PPASSIVE_DESCRIPTION";
            skillLocator.passiveSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texOSPIcon");
            skillLocator.passiveSkill.keywordToken = MainPlugin.developerPrefix + "_HUNK_KEYWORD_GVIRUS";

            Hunk.reloadSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HUNK_BODY_PRIMARY_RELOAD_NAME",
                skillNameToken = prefix + "_HUNK_BODY_PRIMARY_RELOAD_NAME",
                skillDescriptionToken = prefix + "_HUNK_BODY_RELOAD_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texConfirmIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Reload)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Hunk.confirmSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HUNK_BODY_CONFIRM_NAME",
                skillNameToken = prefix + "_HUNK_BODY_CONFIRM_NAME",
                skillDescriptionToken = prefix + "_HUNK_BODY_CONFIRM_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texConfirmIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "fuck",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0,
            });

            Hunk.cancelSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HUNK_BODY_CANCEL_NAME",
                skillNameToken = prefix + "_HUNK_BODY_CANCEL_NAME",
                skillDescriptionToken = prefix + "_HUNK_BODY_CANCEL_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texCancelIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "fuck",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0,
            });

            #region Passive
            passive.rummagePassive = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HUNK_BODY_PASSIVE_NAME",
                skillNameToken = prefix + "_HUNK_BODY_PASSIVE_NAME",
                skillDescriptionToken = prefix + "_HUNK_BODY_PASSIVE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPassiveIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 2,
                stockToConsume = 1
            });

            passive.fullArsenalPassive = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HUNK_BODY_PASSIVE2_NAME",
                skillNameToken = prefix + "_HUNK_BODY_PASSIVE2_NAME",
                skillDescriptionToken = prefix + "_HUNK_BODY_PASSIVE2_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPassive2Icon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 2,
                stockToConsume = 1
            });

            Modules.Skills.AddPassiveSkills(passive.passiveSkillSlot.skillFamily, new SkillDef[]{
                    passive.rummagePassive,
                    passive.fullArsenalPassive
                });

            Modules.Skills.AddUnlockablesToFamily(passive.passiveSkillSlot.skillFamily,
            null, completionUnlockableDef);
            #endregion

            #region Primary
            SkillDef knife = Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.SwingKnife)),
                "Weapon",
                prefix + "_HUNK_BODY_PRIMARY_KNIFE_NAME",
                prefix + "_HUNK_BODY_PRIMARY_KNIFE_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKnifeIcon"), false);
            knife.interruptPriority = EntityStates.InterruptPriority.Skill;
            knife.keywordTokens = new string[]
            {
                MainPlugin.developerPrefix + "_HUNK_KEYWORD_LOOTING",
                MainPlugin.developerPrefix + "_HUNK_KEYWORD_MANGLED"
            };

            /*SkillDef knifeAlt = Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.SwingAltKnife)),
    "Weapon",
    prefix + "_HUNK_BODY_PRIMARY_KNIFEALT_NAME",
    prefix + "_HUNK_BODY_PRIMARY_KNIFEALT_DESCRIPTION",
    Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKnifeIcon"), false);
            knifeAlt.interruptPriority = EntityStates.InterruptPriority.Skill;
            knifeAlt.keywordTokens = new string[]
            {
                MainPlugin.developerPrefix + "_HUNK_KEYWORD_LOOTING"
            };*/

            counterSkillDef = Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Counter.Lunge)),
    "Weapon",
    prefix + "_HUNK_BODY_PRIMARY_KNIFE_NAME",
    prefix + "_HUNK_BODY_PRIMARY_KNIFE_DESCRIPTION",
    Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKnifeIcon"), false);
            counterSkillDef.interruptPriority = EntityStates.InterruptPriority.PrioritySkill;

            scepterCounterSkillDef = Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Counter.UroLunge)),
"Weapon",
prefix + "_HUNK_BODY_PRIMARY_KNIFE_NAME",
prefix + "_HUNK_BODY_PRIMARY_KNIFE_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKnifeIcon"), false);
            counterSkillDef.interruptPriority = EntityStates.InterruptPriority.PrioritySkill;

            Modules.Skills.AddPrimarySkills(prefab,
                knife);
            #endregion

            #region Secondary
            SkillDef aimSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HUNK_BODY_SECONDARY_AIM_NAME",
                skillNameToken = prefix + "_HUNK_BODY_SECONDARY_AIM_NAME",
                skillDescriptionToken = prefix + "_HUNK_BODY_SECONDARY_AIM_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texAimIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.SteadyAim)),
                activationStateMachineName = "Aim",
                baseMaxStock = 1,
                baseRechargeInterval = 0.4f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0,
            });

            Modules.Skills.AddSecondarySkills(prefab, aimSkillDef);
            #endregion

            #region Utility
            SkillDef dodgeSkillDef = Modules.Skills.CreateAwesomeSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HUNK_BODY_UTILITY_DODGE_NAME",
                skillNameToken = prefix + "_HUNK_BODY_UTILITY_DODGE_NAME",
                skillDescriptionToken = prefix + "_HUNK_BODY_UTILITY_DODGE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texDodgeIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Step)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 2, //1
                baseRechargeInterval = 6f, //4
                beginSkillCooldownOnSkillEnd = true, //false
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = true, //false
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 99, //1
                requiredStock = 1,
                stockToConsume = 1
            });

            dodgeSkillDef.keywordTokens = new string[]
            {
                MainPlugin.developerPrefix + "_HUNK_KEYWORD_PERFECTDODGE",
                MainPlugin.developerPrefix + "_HUNK_KEYWORD_COUNTER"
            };

            scepterDodgeSkillDef = Modules.Skills.CreateAwesomeSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HUNK_BODY_UTILITY_DODGE_SCEPTER_NAME",
                skillNameToken = prefix + "_HUNK_BODY_UTILITY_DODGE_SCEPTER_NAME",
                skillDescriptionToken = prefix + "_HUNK_BODY_UTILITY_DODGE_SCEPTER_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texDodgeScepterIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Urostep)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 3, //1
                baseRechargeInterval = 5f, //4
                beginSkillCooldownOnSkillEnd = true, //false
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = true, //false
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 99, //1
                requiredStock = 1,
                stockToConsume = 1
            });

            Modules.Skills.AddUtilitySkills(prefab, dodgeSkillDef);
            #endregion

            #region Special
            SkillDef swapSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HUNK_BODY_SPECIAL_SWAP_NAME",
                skillNameToken = prefix + "_HUNK_BODY_SPECIAL_SWAP_NAME",
                skillDescriptionToken = prefix + "_HUNK_BODY_SPECIAL_SWAP_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSwapIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.SwapWeapon)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0
            });

            Modules.Skills.AddSpecialSkills(prefab, swapSkillDef);
            #endregion

            #region KnifeSkins
            defaultKnifeDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HUNK_BODY_KNIFE_DEFAULT_NAME",
                skillNameToken = prefix + "_HUNK_BODY_KNIFE_DEFAULT_NAME",
                skillDescriptionToken = prefix + "_HUNK_BODY_KNIFE_DEFAULT_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKnifeDefault"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 0,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 0,
                requiredStock = 0,
                stockToConsume = 0
            });

            hiddenKnifeDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HUNK_BODY_KNIFE_HIDDEN_NAME",
                skillNameToken = prefix + "_HUNK_BODY_KNIFE_HIDDEN_NAME",
                skillDescriptionToken = prefix + "_HUNK_BODY_KNIFE_HIDDEN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKnifeHidden"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 0,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 0,
                requiredStock = 0,
                stockToConsume = 0
            });

            bloodyKnifeDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HUNK_BODY_KNIFE_BLOODY_NAME",
                skillNameToken = prefix + "_HUNK_BODY_KNIFE_BLOODY_NAME",
                skillDescriptionToken = prefix + "_HUNK_BODY_KNIFE_BLOODY_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKnifeBloody"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 0,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 0,
                requiredStock = 0,
                stockToConsume = 0
            });

            infiniteKnifeDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HUNK_BODY_KNIFE_INFINITE_NAME",
                skillNameToken = prefix + "_HUNK_BODY_KNIFE_INFINITE_NAME",
                skillDescriptionToken = prefix + "_HUNK_BODY_KNIFE_INFINITE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKnifeInfinite"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 0,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 0,
                requiredStock = 0,
                stockToConsume = 0
            });

            weskerKnifeDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HUNK_BODY_KNIFE_WESKER_NAME",
                skillNameToken = prefix + "_HUNK_BODY_KNIFE_WESKER_NAME",
                skillDescriptionToken = prefix + "_HUNK_BODY_KNIFE_WESKER_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKnifeWesker"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 0,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 0,
                requiredStock = 0,
                stockToConsume = 0
            });

            macheteKnifeDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HUNK_BODY_KNIFE_MACHETE_NAME",
                skillNameToken = prefix + "_HUNK_BODY_KNIFE_MACHETE_NAME",
                skillDescriptionToken = prefix + "_HUNK_BODY_KNIFE_MACHETE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKnifeMachete"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 0,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 0,
                requiredStock = 0,
                stockToConsume = 0
            });

            re4KnifeDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HUNK_BODY_KNIFE_RE4_NAME",
                skillNameToken = prefix + "_HUNK_BODY_KNIFE_RE4_NAME",
                skillDescriptionToken = prefix + "_HUNK_BODY_KNIFE_RE4_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKnifeLeon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 0,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 0,
                requiredStock = 0,
                stockToConsume = 0
            });

            Modules.Skills.AddKnifeSkins(hunk.knifeSkinSkillSlot.skillFamily, new SkillDef[]{
                    defaultKnifeDef,
                    bloodyKnifeDef,
                    hiddenKnifeDef,
                    infiniteKnifeDef,
                    weskerKnifeDef,
                    macheteKnifeDef,
                    re4KnifeDef
                });

            Modules.Skills.AddUnlockablesToFamily(hunk.knifeSkinSkillSlot.skillFamily,
            null, 
            masteryUnlockableDef,
            cqcUnlockableDef,
            infiniteKnifeUnlockableDef,
            weskerKnifeUnlockableDef,
            macheteKnifeUnlockableDef,
            re4KnifeUnlockableDef);
            #endregion


            if (MainPlugin.scepterInstalled) InitializeScepterSkills();
        }

        private static void InitializeScepterSkills()
        {
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterDodgeSkillDef, bodyName, SkillSlot.Utility, 0);
        }

        private static void CreateSkins(GameObject prefab)
        {
            GameObject model = prefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = characterModel.mainSkinnedMeshRenderer;

            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(MainPlugin.developerPrefix + "_HUNK_BODY_DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRenderers,
                mainRenderer,
                model);

            defaultSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model01").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("0000_Resident Evil 7_ 1")
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model02").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("0001_Resident Evil 7_ 2")
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model03").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("0002_Resident Evil 7_ 3")
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model04").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("0003_Resident Evil 7_ 4")
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model05").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("0004_Resident Evil 7_ 1")
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model06").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("0005_Resident Evil 7_ 2")
                }
            };

            skins.Add(defaultSkin);
            #endregion

            #region MasterySkin
            SkinDef masterySkin = Modules.Skins.CreateSkinDef(MainPlugin.developerPrefix + "_HUNK_BODY_TOFU_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texTofuSkin"),
                SkinRendererInfos(defaultRenderers,
                new Material[]
                {
                    Modules.Assets.CreateMaterial("matTofu", 0f, Color.black, 1f),
                    Modules.Assets.CreateMaterial("matBeret", 0f, Color.black, 1f)
                }),
                mainRenderer,
                model,
                masteryUnlockableDef);

            masterySkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model01").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshTofu")
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model02").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshTofuHat")
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model03").GetComponent<SkinnedMeshRenderer>(),
                    mesh = null
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model04").GetComponent<SkinnedMeshRenderer>(),
                    mesh = null
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model05").GetComponent<SkinnedMeshRenderer>(),
                    mesh = null
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model06").GetComponent<SkinnedMeshRenderer>(),
                    mesh = null
                }
            };

            skins.Add(masterySkin);
            #endregion

            #region LightweightSkin
            SkinDef lightweightSkin = Modules.Skins.CreateSkinDef(MainPlugin.developerPrefix + "_HUNK_BODY_LIGHTWEIGHT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texLightweightSkin"),
                defaultRenderers,
                mainRenderer,
                model,
                lightweightUnlockableDef);

            lightweightSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model01").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshHunkB01")
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model02").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshHunkB02")
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model03").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshHunkB03")
                }
            };

            skins.Add(lightweightSkin);
            #endregion

            #region CommandoSkin
            SkinDef commandoSkin = Modules.Skins.CreateSkinDef(MainPlugin.developerPrefix + "_HUNK_BODY_COMMANDO_SKIN_NAME",
                Addressables.LoadAssetAsync<SkinDef>("RoR2/Base/Commando/skinCommandoDefault.asset").WaitForCompletion().icon,
                SkinRendererInfos(defaultRenderers,
                new Material[]
                {
                    Modules.Assets.commandoMat
                }),
                mainRenderer,
                model);

            commandoSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model01").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshCommando")
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model02").GetComponent<SkinnedMeshRenderer>(),
                    mesh = null
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model03").GetComponent<SkinnedMeshRenderer>(),
                    mesh = null
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model04").GetComponent<SkinnedMeshRenderer>(),
                    mesh = null
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model05").GetComponent<SkinnedMeshRenderer>(),
                    mesh = null
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model06").GetComponent<SkinnedMeshRenderer>(),
                    mesh = null
                }
            };

            if (Modules.Config.cursed.Value) skins.Add(commandoSkin);
            #endregion

            #region WeskerSkin
            SkinDef weskerSkin = Modules.Skins.CreateSkinDef(MainPlugin.developerPrefix + "_HUNK_BODY_WESKER_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texWeskerSkin"),
                defaultRenderers,
                mainRenderer,
                model);

            weskerSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model01").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("Albert.001_mesh.001")
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model02").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("Albert.001_mesh.002")
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model03").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("Albert.001_mesh.003")
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model04").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("Albert.001_mesh.004")
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model05").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("Albert.001_mesh.005")
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model06").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("Albert.001_mesh")
                }
            };

            CharacterModel.RendererInfo[] weskerInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(weskerInfos, 0);

            Material hairMat = Modules.Assets.CreateMaterial("matWesker04", 0f, Color.black, 1f);
            hairMat.EnableKeyword("_EnableCutout");
            hairMat.EnableKeyword("CUTOUT");
            hairMat.SetShaderPassEnabled("Cutout", true);
            hairMat.SetShaderPassEnabled("CUTOUT", true);
            hairMat.SetShaderPassEnabled("_EnableCutout", true);

            weskerSkin.rendererInfos = weskerInfos;
            weskerSkin.rendererInfos[0].defaultMaterial = Modules.Assets.CreateMaterial("matWesker05", 0f, Color.black, 1f);
            weskerSkin.rendererInfos[1].defaultMaterial = Modules.Assets.CreateMaterial("matWesker02", 0f, Color.black, 1f);
            weskerSkin.rendererInfos[2].defaultMaterial = Modules.Assets.CreateMaterial("matWesker06", 0f, Color.black, 1f);
            weskerSkin.rendererInfos[3].defaultMaterial = Modules.Assets.CreateMaterial("matWesker01", 0f, Color.black, 1f);
            weskerSkin.rendererInfos[4].defaultMaterial = hairMat;
            weskerSkin.rendererInfos[5].defaultMaterial = Modules.Assets.CreateMaterial("matWesker03", 0f, Color.black, 1f);

            if (Modules.Config.cursed.Value) skins.Add(weskerSkin);
            #endregion

            #region DoomSkin
            SkinDef doomSkin = Modules.Skins.CreateSkinDef(MainPlugin.developerPrefix + "_HUNK_BODY_DOOM_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texDoomSkin"),
                defaultRenderers,
                mainRenderer,
                model);

            doomSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model01").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshSlayerAccessories")
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model02").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshSlayerHelmet")
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model03").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshSlayerLegs")
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model04").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshSlayerTorso")
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model05").GetComponent<SkinnedMeshRenderer>(),
                    mesh = null
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model06").GetComponent<SkinnedMeshRenderer>(),
                    mesh = null
                }
            };

            CharacterModel.RendererInfo[] doomInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(doomInfos, 0);

            doomSkin.rendererInfos = doomInfos;
            doomSkin.rendererInfos[0].defaultMaterial = Modules.Assets.CreateMaterial("matDoom03");
            doomSkin.rendererInfos[1].defaultMaterial = Modules.Assets.CreateMaterial("matDoom04", 0f, Color.black, 1f);
            doomSkin.rendererInfos[2].defaultMaterial = Modules.Assets.CreateMaterial("matDoom02", 0f, Color.black, 1f);
            doomSkin.rendererInfos[3].defaultMaterial = Modules.Assets.CreateMaterial("matDoom01", 0f, Color.black, 1f);

            if (Modules.Config.cursed.Value) skins.Add(doomSkin);
            #endregion

            #region MinecraftSkin
            SkinDef minecraftSkin = Modules.Skins.CreateSkinDef(MainPlugin.developerPrefix + "_HUNK_BODY_MINECRAFT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMinecraftSkin"),
                defaultRenderers,
                mainRenderer,
                model);

            minecraftSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model01").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshMinecraftBase")
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model02").GetComponent<SkinnedMeshRenderer>(),
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshMinecraftOuter")
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model03").GetComponent<SkinnedMeshRenderer>(),
                    mesh = null
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model04").GetComponent<SkinnedMeshRenderer>(),
                    mesh = null
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model05").GetComponent<SkinnedMeshRenderer>(),
                    mesh = null
                },
                new SkinDef.MeshReplacement
                {
                    renderer = childLocator.FindChild("Model06").GetComponent<SkinnedMeshRenderer>(),
                    mesh = null
                }
            };

            CharacterModel.RendererInfo[] minecraftInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(minecraftInfos, 0);

            Material minecraftMat = Modules.Assets.CreateMaterial("matHunkMinecraft");
            minecraftMat.EnableKeyword("_EnableCutout");
            minecraftMat.EnableKeyword("CUTOUT");
            minecraftMat.SetShaderPassEnabled("Cutout", true);
            minecraftMat.SetShaderPassEnabled("CUTOUT", true);
            minecraftMat.SetShaderPassEnabled("_EnableCutout", true);

            minecraftSkin.rendererInfos = minecraftInfos;
            minecraftSkin.rendererInfos[0].defaultMaterial = minecraftMat;
            minecraftSkin.rendererInfos[1].defaultMaterial = minecraftMat;

            if (Modules.Config.cursed.Value) skins.Add(minecraftSkin);
            #endregion

            #region SuperSkin
            SkinDef superSkin = Modules.Skins.CreateSkinDef(MainPlugin.developerPrefix + "_HUNK_BODY_SUPER_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texSuperSkin"),
                defaultRenderers,
                mainRenderer,
                model,
                earlySupporterUnlockableDef);

            CharacterModel.RendererInfo[] superInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(superInfos, 0);

            superSkin.rendererInfos = superInfos;
            superSkin.rendererInfos[5].defaultMaterial = Modules.Assets.CreateMaterial("matHunk06", 10f, Color.white, 1f);

            superSkin.meshReplacements = defaultSkin.meshReplacements;

            if (!MainPlugin.unlockAllInstalled) skins.Add(superSkin);
            #endregion

            skinController.skins = skins.ToArray();
        }

        private static void CreateKnifeSkins()
        {
            Hunk.knifeSkins.Add(defaultKnifeDef, Modules.Assets.LoadKnife("CombatKnife"));
            //Hunk.knifeSkins.Add(hiddenKnifeDef, null);
            Hunk.knifeSkins.Add(infiniteKnifeDef, Modules.Assets.LoadKnife("InfiniteKnife"));
            Hunk.knifeSkins.Add(bloodyKnifeDef, Modules.Assets.LoadKnife("BloodyKnife"));
            Hunk.knifeSkins.Add(weskerKnifeDef, Modules.Assets.LoadKnife("WeskerKnife"));
            Hunk.knifeSkins.Add(macheteKnifeDef, Modules.Assets.LoadKnife("Machete"));
            Hunk.knifeSkins.Add(re4KnifeDef, Modules.Assets.LoadKnife("LeonKnife"));
        }

        private static void InitializeItemDisplays(GameObject prefab)
        {
            CharacterModel characterModel = prefab.GetComponentInChildren<CharacterModel>();

            if (itemDisplayRuleSet == null)
            {
                itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
                itemDisplayRuleSet.name = "idrs" + bodyName;
            }

            characterModel.itemDisplayRuleSet = itemDisplayRuleSet;
            characterModel.itemDisplayRuleSet.keyAssetRuleGroups = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<CharacterModel>().itemDisplayRuleSet.keyAssetRuleGroups;// itemDisplayRuleSet;
            itemDisplayRules = itemDisplayRuleSet.keyAssetRuleGroups.ToList();
        }

        internal static void AddVirusDisplayRules()
        {
            #region Golem
            ItemDisplayRuleSet idrs = Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Golem/idrsGolem.asset").WaitForCompletion();
            List<ItemDisplayRuleSet.KeyAssetRuleGroup> displayRules = idrs.keyAssetRuleGroups.ToList();

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
childName = "Chest",
localPos = new Vector3(0.69285F, 0.71388F, 0.44213F),
localAngles = new Vector3(344.0977F, 20.83211F, 306.3226F),
localScale = new Vector3(0.5279F, 0.5279F, 0.5279F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus2,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
childName = "Chest",
localPos = new Vector3(-0.21733F, 0.56729F, -0.37827F),
localAngles = new Vector3(353.4179F, 192.6143F, 292.5146F),
localScale = new Vector3(0.42724F, 0.61751F, 0.45384F),
                            limbMask = LimbFlags.None
                        }
        }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirusFinal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
childName = "Eye",
localPos = new Vector3(0F, -0.09772F, 0.18259F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.53765F, 0.535F, 0.42023F),
                            limbMask = LimbFlags.None
                        }
        }
                }
            });

            idrs.keyAssetRuleGroups = displayRules.ToArray();
            #endregion

            #region Lemurian
            idrs = Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Lemurian/idrsLemurian.asset").WaitForCompletion();
            displayRules = idrs.keyAssetRuleGroups.ToList();

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
childName = "Chest",
localPos = new Vector3(1.5837F, 1.20071F, -0.38966F),
localAngles = new Vector3(310.2687F, 89.30039F, 96.96844F),
localScale = new Vector3(2.13088F, 2.57479F, 3.24198F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus2,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
childName = "Hip",
localPos = new Vector3(1.63195F, 0.73422F, -0.36539F),
localAngles = new Vector3(355.8275F, 114.2429F, 174.8757F),
localScale = new Vector3(1.56833F, 1.89505F, 2.3861F),
                            limbMask = LimbFlags.None
                        }
        }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirusFinal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
childName = "Head",
localPos = new Vector3(-0.00001F, 1.97309F, 0.04859F),
localAngles = new Vector3(271.4205F, 0F, 0F),
localScale = new Vector3(2.13088F, 2.41184F, 4.34497F),
                            limbMask = LimbFlags.None
                        }
        }
                }
            });

            idrs.keyAssetRuleGroups = displayRules.ToArray();
            #endregion

            #region LesserWisp
            idrs = Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Wisp/idrsWisp.asset").WaitForCompletion();
            displayRules = idrs.keyAssetRuleGroups.ToList();

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
childName = "Head",
localPos = new Vector3(-0.55618F, -0.05854F, 0.18798F),
localAngles = new Vector3(337.501F, 297.2888F, 162.3405F),
localScale = new Vector3(0.36728F, 0.36728F, 0.36728F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus2,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
childName = "Head",
localPos = new Vector3(-0.23583F, 0.07614F, 0.63837F),
localAngles = new Vector3(327.4835F, 345.5454F, 356.0591F),
localScale = new Vector3(0.42641F, 0.5279F, 0.5279F),
                            limbMask = LimbFlags.None
                        }
        }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirusFinal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
childName = "Head",
localPos = new Vector3(0F, 0.32753F, -0.06664F),
localAngles = new Vector3(275.6953F, 359.2037F, 181.43F),
localScale = new Vector3(0.53158F, 0.69005F, 0.69005F),
                            limbMask = LimbFlags.None
                        }
        }
                }
            });

            idrs.keyAssetRuleGroups = displayRules.ToArray();
            #endregion

            #region Beetle
            idrs = Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Beetle/idrsBeetle.asset").WaitForCompletion();
            displayRules = idrs.keyAssetRuleGroups.ToList();

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(-0.35141F, 0.49907F, 0.26666F),
localAngles = new Vector3(297.3339F, 281.542F, 259.467F),
localScale = new Vector3(0.37442F, 0.43177F, 0.30541F)
                        }
                    }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus2,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Chest",
localPos = new Vector3(-0.34449F, 0.58535F, 0.28165F),
localAngles = new Vector3(295.1681F, 339.4774F, 155.8602F),
localScale = new Vector3(0.38573F, 0.36756F, 0.33815F)
                        }
        }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirusFinal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0.0008F, 0.04397F, -0.07806F),
localAngles = new Vector3(346.3586F, 180F, 1.87543F),
localScale = new Vector3(0.37442F, 0.43177F, 0.5829F)
                        }
        }
                }
            });
            idrs.keyAssetRuleGroups = displayRules.ToArray();
            #endregion

            #region Blind Pest
            idrs = Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/DLC1/FlyingVermin/idrsFlyingVermin.asset").WaitForCompletion();
            displayRules = idrs.keyAssetRuleGroups.ToList();

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Body",
localPos = new Vector3(-0.35141F, 1.01498F, 0.30014F),
localAngles = new Vector3(298.9637F, 306.8926F, 2.61667F),
localScale = new Vector3(0.68332F, 0.78798F, 0.55737F)
                        }
                    }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus2,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Body",
localPos = new Vector3(0.39853F, 0.89548F, -0.39389F),
localAngles = new Vector3(294.165F, 162.7658F, 83.1927F),
localScale = new Vector3(0.51132F, 0.6617F, 0.70897F)
                        }
        }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirusFinal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Body",
localPos = new Vector3(-0.00494F, 0.2474F, 0.84682F),
localAngles = new Vector3(13.39955F, 354.3815F, 356.81F),
localScale = new Vector3(0.65352F, 0.75362F, 1.01741F)
                        }
        }
                }
            });
            idrs.keyAssetRuleGroups = displayRules.ToArray();
            #endregion

            #region Alloy Vulture
            idrs = Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Vulture/idrsVulture.asset").WaitForCompletion();
            displayRules = idrs.keyAssetRuleGroups.ToList();

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Chest",
localPos = new Vector3(-2.45199F, 1.05421F, -0.86517F),
localAngles = new Vector3(12.60156F, 205.7276F, 116.2795F),
localScale = new Vector3(1.64841F, 2.07425F, 1.46721F)
                        }
                    }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus2,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "FootL",
localPos = new Vector3(0.23025F, 0.54567F, -0.75685F),
localAngles = new Vector3(315.5203F, 161.7386F, 214.9006F),
localScale = new Vector3(0.49248F, 0.56073F, 0.51586F)
                        }
        }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirusFinal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(-0.03198F, 0.54857F, -0.44969F),
localAngles = new Vector3(306.4828F, 185.0415F, 174.3479F),
localScale = new Vector3(2.27331F, 2.91669F, 3.9376F)
                        }
        }
                }
            });
            idrs.keyAssetRuleGroups = displayRules.ToArray();
            #endregion

            #region Beetle Guard
            idrs = Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Beetle/idrsBeetleGuard.asset").WaitForCompletion();
            displayRules = idrs.keyAssetRuleGroups.ToList();

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "HandR",
localPos = new Vector3(-0.06254F, 0.60391F, -0.66027F),
localAngles = new Vector3(357.0576F, 201.3958F, 263.8221F),
localScale = new Vector3(0.60461F, 0.69721F, 0.60435F)
                        }
                    }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus2,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0.60587F, 0.39132F, -0.47052F),
localAngles = new Vector3(313.6237F, 75.64559F, 37.47511F),
localScale = new Vector3(1.01838F, 0.97041F, 0.89277F)
                        }
        }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirusFinal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(-0.10479F, 1.55146F, -0.10945F),
localAngles = new Vector3(292.9561F, 357.9843F, 176.711F),
localScale = new Vector3(1.44881F, 1.8349F, 1.29496F)
                        }
        }
                }
            });
            idrs.keyAssetRuleGroups = displayRules.ToArray();
            #endregion

            #region Bison
            idrs = Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Bison/idrsBison.asset").WaitForCompletion();
            displayRules = idrs.keyAssetRuleGroups.ToList();

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(-0.02246F, 0.64255F, 0.33553F),
localAngles = new Vector3(299.4639F, 319.8605F, 172.6846F),
localScale = new Vector3(0.18002F, 0.20759F, 0.22654F)
                        }
                    }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus2,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(-0.01497F, 0.65113F, -0.37822F),
localAngles = new Vector3(306.1044F, 230.9469F, 327.8887F),
localScale = new Vector3(0.16618F, 0.21809F, 0.20064F)
                        }
        }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirusFinal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(-0.09919F, 0.67496F, 0.06389F),
localAngles = new Vector3(291.3203F, 317.8623F, 79.44216F),
localScale = new Vector3(0.33353F, 0.38461F, 0.58257F)
                        }
        }
                }
            });
            idrs.keyAssetRuleGroups = displayRules.ToArray();
            #endregion

            #region Templar
            idrs = Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/ClayBruiser/idrsClayBruiser.asset").WaitForCompletion();
            displayRules = idrs.keyAssetRuleGroups.ToList();

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Muzzle",
localPos = new Vector3(0F, -0.06654F, -0.20133F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.25402F, 0.32167F, 0.25402F)
                        }
                    }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus2,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(-0.48537F, 0.03016F, 0.28992F),
localAngles = new Vector3(331.2194F, 294.6687F, 181.9398F),
localScale = new Vector3(0.38573F, 0.36756F, 0.33815F)
                        }
        }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirusFinal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0.00825F, 0.2264F, 0.13584F),
localAngles = new Vector3(297.4628F, 67.79795F, 230.3572F),
localScale = new Vector3(0.44669F, 0.51511F, 0.69541F)
                        }
        }
                }
            });
            idrs.keyAssetRuleGroups = displayRules.ToArray();
            #endregion

            #region Elder Lemurian
            idrs = Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/LemurianBruiser/idrsLemurianBruiser.asset").WaitForCompletion();
            displayRules = idrs.keyAssetRuleGroups.ToList();

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Chest",
localPos = new Vector3(-0.73046F, 0.22604F, -2.22095F),
localAngles = new Vector3(337.0166F, 195.3143F, 307.628F),
localScale = new Vector3(2.20569F, 2.54354F, 1.79915F)
                        }
                    }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus2,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Chest",
localPos = new Vector3(1.72582F, 2.05372F, 0.47658F),
localAngles = new Vector3(303.0192F, 67.60723F, 195.18F),
localScale = new Vector3(1.83411F, 1.90506F, 3.21729F)
                        }
        }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirusFinal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0.07601F, 2.12676F, 1.07836F),
localAngles = new Vector3(276.5894F, 16.4315F, 163.9101F),
localScale = new Vector3(2.40804F, 2.77688F, 5.01033F)
                        }
        }
                }
            });
            idrs.keyAssetRuleGroups = displayRules.ToArray();
            #endregion

            #region Greater Wisp
            idrs = Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/GreaterWisp/idrsGreaterWisp.asset").WaitForCompletion();
            displayRules = idrs.keyAssetRuleGroups.ToList();

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "MaskBase",
localPos = new Vector3(0.47566F, 0.81408F, 0.21343F),
localAngles = new Vector3(334.4195F, 12.17842F, 308.6803F),
localScale = new Vector3(0.63072F, 0.71507F, 0.71507F)
                        }
                    }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus2,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "MuzzleLeft",
localPos = new Vector3(0.44733F, 0.10951F, 0.0809F),
localAngles = new Vector3(0.87499F, 20.25935F, 358.3208F),
localScale = new Vector3(0.25384F, 0.31895F, 0.39405F)
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "MuzzleRight",
localPos = new Vector3(0.08772F, -0.12623F, -0.27867F),
localAngles = new Vector3(0.87499F, 20.25935F, 358.3208F),
localScale = new Vector3(0.25384F, 0.31895F, 0.39405F)
                        }
        }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirusFinal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "MaskBase",
localPos = new Vector3(-0.10331F, 0.0761F, 0.5305F),
localAngles = new Vector3(13.67617F, 1.18147F, 30.66892F),
localScale = new Vector3(0.78518F, 0.90545F, 0.92726F)
                        }
        }
                }
            });
            idrs.keyAssetRuleGroups = displayRules.ToArray();
            #endregion

            #region Parent
            idrs = Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Parent/idrsParent.asset").WaitForCompletion();
            displayRules = idrs.keyAssetRuleGroups.ToList();

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Chest",
localPos = new Vector3(64.86555F, 2.47273F, -90.4606F),
localAngles = new Vector3(339.4435F, 116.334F, 268.6689F),
localScale = new Vector3(73.79612F, 66.57604F, 101.7756F)
                        }
                    }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus2,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Pelvis",
localPos = new Vector3(58.78731F, 31.02789F, 53.63195F),
localAngles = new Vector3(359.8232F, 53.31361F, 89.13486F),
localScale = new Vector3(50.37483F, 73.41547F, 94.4957F)
                        }
        }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirusFinal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(13.39281F, 41.68724F, 1.13535F),
localAngles = new Vector3(315.7136F, 69.70868F, 18.78091F),
localScale = new Vector3(74.23115F, 138.2175F, 115.5637F)
                        }
        }
                }
            });
            idrs.keyAssetRuleGroups = displayRules.ToArray();
            #endregion

            #region Imp
            idrs = Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Imp/idrsImp.asset").WaitForCompletion();
            displayRules = idrs.keyAssetRuleGroups.ToList();

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Chest",
localPos = new Vector3(-0.09919F, 0.39778F, 0.06803F),
localAngles = new Vector3(299.956F, 296.3019F, 63.28477F),
localScale = new Vector3(0.20132F, 0.23216F, 0.16421F)
                        }
                    }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirus2,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Chest",
localPos = new Vector3(0.02974F, 0.16718F, 0.21999F),
localAngles = new Vector3(350.9952F, 5.38308F, 359.3495F),
localScale = new Vector3(0.25758F, 0.29703F, 0.401F)
                        }
        }
                }
            });

            displayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirusFinal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.VirusEye,
                            limbMask = LimbFlags.None,
childName = "Chest",
localPos = new Vector3(0.0008F, 0.04397F, -0.07806F),
localAngles = new Vector3(346.3586F, 180F, 1.87543F),
localScale = new Vector3(0.37442F, 0.43177F, 0.5829F)
                        }
        }
                }
            });
            idrs.keyAssetRuleGroups = displayRules.ToArray();
            #endregion
        }

        internal static void SetItemDisplays()
        {
            // uhh
            Modules.ItemDisplays.PopulateDisplays();

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.spadeKeycard,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.SpadeKeycard,
                            limbMask = LimbFlags.None,
childName = "Pelvis",
localPos = new Vector3(18.28741F, -5.00451F, -6.3516F),
localAngles = new Vector3(55.81532F, 102.1367F, 182.2121F),
localScale = new Vector3(19.82461F, 22.86115F, 30.86304F)
                        }
        }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.clubKeycard,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
{
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.ClubKeycard,
                            limbMask = LimbFlags.None,
childName = "Pelvis",
localPos = new Vector3(15.68854F, -3.62925F, -8.9425F),
localAngles = new Vector3(54.34012F, 118.4513F, 195.6197F),
localScale = new Vector3(19.82461F, 22.86115F, 30.86304F)
                        }
}
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.heartKeycard,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
{
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.HeartKeycard,
                            limbMask = LimbFlags.None,
childName = "Pelvis",
localPos = new Vector3(18.9826F, -4.52743F, -4.17952F),
localAngles = new Vector3(37.62438F, 61.28591F, 139.0769F),
localScale = new Vector3(19.82461F, 22.86115F, 30.86304F)
                        }
}
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.diamondKeycard,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
{
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.DiamondKeycard,
                            limbMask = LimbFlags.None,
childName = "Pelvis",
localPos = new Vector3(19.26896F, -5.61374F, -2.02333F),
localAngles = new Vector3(56.2317F, 120.4924F, 171.5673F),
localScale = new Vector3(19.82461F, 22.86115F, 30.86304F)
                        }
}
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.goldKeycard,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
{
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.StarKeycard,
                            limbMask = LimbFlags.None,
childName = "Pelvis",
localPos = new Vector3(19.87744F, -3.52511F, -7.59921F),
localAngles = new Vector3(54.63135F, 152.5819F, 198.216F),
localScale = new Vector3(19.82461F, 22.86115F, 30.86304F)
                        }
}
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.masterKeycard,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
{
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.MasterKeycard,
                            limbMask = LimbFlags.None,
childName = "Pelvis",
localPos = new Vector3(21.71092F, -6.29003F, -7.494F),
localAngles = new Vector3(35.30389F, 123.9322F, 198.5352F),
localScale = new Vector3(22.80404F, 26.29696F, 35.5014F)
                        }
}
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.wristband,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
{
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.Wristband,
                            limbMask = LimbFlags.None,
childName = "LowerArmL",
localPos = new Vector3(1.6174F, 20.56634F, -1.37473F),
localAngles = new Vector3(331.4508F, 260.9691F, 0F),
localScale = new Vector3(9.34054F, 9.34054F, 9.34054F)
                        }
}
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.gVirusSample,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
{
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.GVirusSample,
                            limbMask = LimbFlags.None,
childName = "Stomach",
localPos = new Vector3(-4.27445F, 14.75704F, -16.96162F),
localAngles = new Vector3(0F, 0F, 46.0078F),
localScale = new Vector3(15.77204F, 15.77204F, 15.77204F)
                        }
}
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = Hunk.tVirusSample,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
{
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.TVirusSample,
                            limbMask = LimbFlags.None,
childName = "Stomach",
localPos = new Vector3(-4.03512F, 20.82401F, -16.96177F),
localAngles = new Vector3(0F, 0F, 46.0078F),
localScale = new Vector3(15.77204F, 15.77204F, 15.77204F)
                        }
}
                }
            });

            //if (!Modules.Config.enableItemDisplays) return;

            ReplaceItemDisplay(RoR2Content.Items.SecondarySkillMagazine, new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayDoubleMag"),
                    limbMask = LimbFlags.None,
childName = "HandR",
localPos = new Vector3(0.00888F, -0.03648F, -0.20898F),
localAngles = new Vector3(39.35415F, 348.9445F, 164.0792F),
localScale = new Vector3(0.06F, 0.06F, 0.06F)
                }
            });

            ReplaceItemDisplay(RoR2Content.Items.CritGlasses, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayGlasses"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(-0.00025F, 6.63616F, 13.36325F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(20.20473F, 21.7767F, 19.04527F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.BleedOnHit, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayTriTip"),
                    limbMask = LimbFlags.None,
childName = "Chest",
localPos = new Vector3(-19.87867F, 16.71131F, -12.58668F),
localAngles = new Vector3(20.06403F, 44.21899F, 305.6593F),
localScale = new Vector3(26.10918F, 26.1091F, 26.10918F)
                }
});

            ReplaceItemDisplay(DLC1Content.Items.BleedOnHitVoid, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayTriTipVoid"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(-5.34769F, 16.4988F, -8.31364F),
localAngles = new Vector3(342.8535F, 243.5605F, 253.8596F),
localScale = new Vector3(15.58726F, 15.58725F, 15.58725F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.ArmorReductionOnHit, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayWarhammer"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0.0006F, 0.25054F, 0.04672F),
localAngles = new Vector3(314.7648F, 358.1459F, 0.48047F),
localScale = new Vector3(0.30902F, 0.09537F, 0.30934F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.AttackSpeedOnCrit, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayWolfPelt"),
                    limbMask = LimbFlags.None,
childName = "Chest",
localPos = new Vector3(0.04472F, 26.87948F, -11.56151F),
localAngles = new Vector3(21.51934F, 178.8835F, 359.2931F),
localScale = new Vector3(23.00528F, 23.00528F, 23.00528F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.Behemoth, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayBehemoth"),
                    limbMask = LimbFlags.None,
childName = "Chest",
localPos = new Vector3(7.21928F, 30.64078F, -11.64861F),
localAngles = new Vector3(330.8196F, 60.65011F, 22.1571F),
localScale = new Vector3(7.26354F, 7.26354F, 7.26354F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.Bandolier, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayBandolier"),
                    limbMask = LimbFlags.None,
childName = "Stomach",
localPos = new Vector3(0.04472F, 26.87948F, -11.56151F),
localAngles = new Vector3(21.51934F, 178.8835F, 359.2931F),
localScale = new Vector3(23.00528F, 23.00528F, 23.00528F)
                }
});

            ReplaceItemDisplay(DLC1Content.Items.CritGlassesVoid, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayGlassesVoid"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(-0.00061F, 4.03076F, 13.69691F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(19.83379F, 28.175F, 28.175F)
                }
});

            ReplaceItemDisplay(DLC1Content.Items.LunarSun, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplaySunHeadNeck"),
                    limbMask = LimbFlags.None,
childName = "Chest",
localPos = new Vector3(-0.02605F, 0.38179F, -0.0112F),
localAngles = new Vector3(-0.00001F, 262.1551F, 0.00001F),
localScale = new Vector3(1.76594F, 1.84475F, 1.84475F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.LimbMask,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplaySunHead"),
                    limbMask = LimbFlags.Head,
childName = "Head",
localPos = new Vector3(0F, 0.10143F, -0.01147F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.90836F, 0.90836F, 0.90836F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplaySunHead"),
                    limbMask = LimbFlags.Head,
childName = "Head",
localPos = new Vector3(0F, 0.10143F, -0.01147F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.90836F, 0.90836F, 0.90836F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.GhostOnKill, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayMask"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0.0029F, 0.15924F, 0.07032F),
localAngles = new Vector3(355.7367F, 0.15F, 0F),
localScale = new Vector3(0.6F, 0.6F, 0.6F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.GoldOnHit, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayBoneCrown"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0F, 0.15159F, -0.0146F),
localAngles = new Vector3(8.52676F, 0F, 0F),
localScale = new Vector3(0.90509F, 0.90509F, 0.90509F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.JumpBoost, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayWaxBird"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0F, -0.228F, -0.108F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.79857F, 0.79857F, 0.79857F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.KillEliteFrenzy, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayBrainstalk"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0F, 0.12823F, 0.035F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.17982F, 0.17982F, 0.17982F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.LunarPrimaryReplacement, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayBirdEye"),
                    limbMask = LimbFlags.None,
childName = "HandL",
localPos = new Vector3(0F, 0.18736F, 0.08896F),
localAngles = new Vector3(306.9798F, 180F, 180F),
localScale = new Vector3(0.31302F, 0.31302F, 0.31302F)
                }
});

            ReplaceItemDisplay(DLC1Content.Items.FragileDamageBonus, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayDelicateWatch"),
                    limbMask = LimbFlags.None,
childName = "HandL",
localPos = new Vector3(0.001145094f, -0.01941454f, 0.001435831f),
localAngles = new Vector3(84.24088f, 213.1651f, 131.5774f),
localScale = new Vector3(0.5f, 0.5f, 0.5f)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.BarrierOnOverHeal, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayAegis"),
                    limbMask = LimbFlags.None,
childName = "LowerArmL",
localPos = new Vector3(0.01781F, 0.11702F, 0.01516F),
localAngles = new Vector3(90F, 270F, 0F),
localScale = new Vector3(0.3F, 0.3F, 0.3F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.SprintArmor, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayBuckler"),
                    limbMask = LimbFlags.None,
childName = "LowerArmL",
localPos = new Vector3(-0.012F, 0.171F, -0.027F),
localAngles = new Vector3(0F, 90F, 0F),
localScale = new Vector3(0.3F, 0.3F, 0.3F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.ArmorPlate, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayRepulsionArmorPlate"),
                    limbMask = LimbFlags.None,
childName = "CalfR",
localPos = new Vector3(-0.02573F, 0.22602F, 0.0361F),
localAngles = new Vector3(90F, 180F, 0F),
localScale = new Vector3(-0.2958F, 0.2958F, 0.29581F)
                }
});

            ReplaceItemDisplay(DLC1Content.Items.CritDamage, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayLaserSight"),
                    limbMask = LimbFlags.None,
childName = "HandR",
localPos = new Vector3(-0.01876F, 0.26245F, 0.11694F),
localAngles = new Vector3(0F, 0F, 270F),
localScale = new Vector3(0.05261F, 0.05261F, 0.05261F)
                }
});

            itemDisplayRuleSet.keyAssetRuleGroups = itemDisplayRules.ToArray();
            //itemDisplayRuleSet.GenerateRuntimeValues();
        }

        internal static void ReplaceItemDisplay(Object keyAsset, ItemDisplayRule[] newDisplayRules)
        {
            ItemDisplayRuleSet.KeyAssetRuleGroup[] cock = itemDisplayRules.ToArray();
            for (int i = 0; i < cock.Length; i++)
            {
                if (cock[i].keyAsset == keyAsset)
                {
                    // replace the item display rule
                    cock[i].displayRuleGroup.rules = newDisplayRules;
                }
            }
            itemDisplayRules = cock.ToList();
        }

        private static CharacterModel.RendererInfo[] SkinRendererInfos(CharacterModel.RendererInfo[] defaultRenderers, Material[] materials)
        {
            CharacterModel.RendererInfo[] newRendererInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(newRendererInfos, 0);

            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i])
                {
                    newRendererInfos[i].defaultMaterial = materials[i];
                }
            }

            return newRendererInfos;
        }

        private static void AddHeartCostType(List<CostTypeDef> list)
        {
            heartCostDef = new CostTypeDef();
            heartCostDef.costStringFormatToken = MainPlugin.developerPrefix + "_HEARTCOST";
            heartCostDef.isAffordable = new CostTypeDef.IsAffordableDelegate(Misc.ItemCostTypeHelperHeart.IsAffordable);
            heartCostDef.payCost = new CostTypeDef.PayCostDelegate(Misc.ItemCostTypeHelperHeart.PayCost);
            heartCostDef.colorIndex = ColorCatalog.ColorIndex.Blood;
            heartCostDef.saturateWorldStyledCostString = true;
            heartCostDef.darkenWorldStyledCostString = false;
            heartCostTypeIndex = CostTypeCatalog.costTypeDefs.Length + list.Count;
            list.Add(heartCostDef);
        }

        private static void AddClubCostType(List<CostTypeDef> list)
        {
            clubCostDef = new CostTypeDef();
            clubCostDef.costStringFormatToken = MainPlugin.developerPrefix + "_CLUBCOST";
            clubCostDef.isAffordable = new CostTypeDef.IsAffordableDelegate(Misc.ItemCostTypeHelperClub.IsAffordable);
            clubCostDef.payCost = new CostTypeDef.PayCostDelegate(Misc.ItemCostTypeHelperClub.PayCost);
            clubCostDef.colorIndex = ColorCatalog.ColorIndex.Blood;
            clubCostDef.saturateWorldStyledCostString = true;
            clubCostDef.darkenWorldStyledCostString = false;
            clubCostTypeIndex = CostTypeCatalog.costTypeDefs.Length + list.Count;
            list.Add(clubCostDef);
        }

        private static void AddSpadeCostType(List<CostTypeDef> list)
        {
            spadeCostDef = new CostTypeDef();
            spadeCostDef.costStringFormatToken = MainPlugin.developerPrefix + "_SPADECOST";
            spadeCostDef.isAffordable = new CostTypeDef.IsAffordableDelegate(Misc.ItemCostTypeHelperSpade.IsAffordable);
            spadeCostDef.payCost = new CostTypeDef.PayCostDelegate(Misc.ItemCostTypeHelperSpade.PayCost);
            spadeCostDef.colorIndex = ColorCatalog.ColorIndex.Blood;
            spadeCostDef.saturateWorldStyledCostString = true;
            spadeCostDef.darkenWorldStyledCostString = false;
            spadeCostTypeIndex = CostTypeCatalog.costTypeDefs.Length + list.Count;
            list.Add(spadeCostDef);
        }

        private static void AddDiamondCostType(List<CostTypeDef> list)
        {
            diamondCostDef = new CostTypeDef();
            diamondCostDef.costStringFormatToken = MainPlugin.developerPrefix + "_DIAMONDCOST";
            diamondCostDef.isAffordable = new CostTypeDef.IsAffordableDelegate(Misc.ItemCostTypeHelperDiamond.IsAffordable);
            diamondCostDef.payCost = new CostTypeDef.PayCostDelegate(Misc.ItemCostTypeHelperDiamond.PayCost);
            diamondCostDef.colorIndex = ColorCatalog.ColorIndex.Blood;
            diamondCostDef.saturateWorldStyledCostString = true;
            diamondCostDef.darkenWorldStyledCostString = false;
            diamondCostTypeIndex = CostTypeCatalog.costTypeDefs.Length + list.Count;
            list.Add(diamondCostDef);
        }

        private static void AddStarCostType(List<CostTypeDef> list)
        {
            starCostDef = new CostTypeDef();
            starCostDef.costStringFormatToken = MainPlugin.developerPrefix + "_STARCOST";
            starCostDef.isAffordable = new CostTypeDef.IsAffordableDelegate(Misc.ItemCostTypeHelperStar.IsAffordable);
            starCostDef.payCost = new CostTypeDef.PayCostDelegate(Misc.ItemCostTypeHelperStar.PayCost);
            starCostDef.colorIndex = ColorCatalog.ColorIndex.Blood;
            starCostDef.saturateWorldStyledCostString = true;
            starCostDef.darkenWorldStyledCostString = false;
            starCostTypeIndex = CostTypeCatalog.costTypeDefs.Length + list.Count;
            list.Add(starCostDef);
        }

        private static void AddWristbandCostType(List<CostTypeDef> list)
        {
            wristbandCostDef = new CostTypeDef();
            wristbandCostDef.costStringFormatToken = MainPlugin.developerPrefix + "_WRISTBANDCOST";
            wristbandCostDef.isAffordable = new CostTypeDef.IsAffordableDelegate(Misc.ItemCostTypeHelperWristband.IsAffordable);
            wristbandCostDef.payCost = new CostTypeDef.PayCostDelegate(Misc.ItemCostTypeHelperWristband.PayCost);
            wristbandCostDef.colorIndex = ColorCatalog.ColorIndex.Blood;
            wristbandCostDef.saturateWorldStyledCostString = true;
            wristbandCostDef.darkenWorldStyledCostString = false;
            wristbandCostTypeIndex = CostTypeCatalog.costTypeDefs.Length + list.Count;
            list.Add(wristbandCostDef);
        }

        private static void AddSampleCostType(List<CostTypeDef> list)
        {
            sampleCostDef = new CostTypeDef();
            sampleCostDef.costStringFormatToken = MainPlugin.developerPrefix + "_SAMPLECOST";
            sampleCostDef.isAffordable = new CostTypeDef.IsAffordableDelegate(Misc.ItemCostTypeHelperSample.IsAffordable);
            sampleCostDef.payCost = new CostTypeDef.PayCostDelegate(Misc.ItemCostTypeHelperSample.PayCost);
            sampleCostDef.colorIndex = ColorCatalog.ColorIndex.BossItem;
            sampleCostDef.saturateWorldStyledCostString = true;
            sampleCostDef.darkenWorldStyledCostString = false;
            sampleCostTypeIndex = CostTypeCatalog.costTypeDefs.Length + list.Count;
            list.Add(sampleCostDef);
        }

        public void CreateWeaponPools()
        {
            defaultWeaponPool.Add(Weapons.ATM.instance.weaponDef);
            defaultWeaponPool.Add(Weapons.M19.instance.weaponDef);
            defaultWeaponPool.Add(Weapons.Magnum.instance.weaponDef);
            defaultWeaponPool.Add(Weapons.MUP.instance.weaponDef);
            defaultWeaponPool.Add(Weapons.RocketLauncher.instance.weaponDef);
            defaultWeaponPool.Add(Weapons.Shotgun.instance.weaponDef);
            defaultWeaponPool.Add(Weapons.Slugger.instance.weaponDef);
            defaultWeaponPool.Add(Weapons.SMG.instance.weaponDef);
        }

        private void CreateChest()
        {
            GameObject displayCaseModel = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlDisplayCase"));

            weaponChestPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Chest2/Chest2.prefab").WaitForCompletion().InstantiateClone("HunkChest", true);
            //weaponChestPrefab.GetComponent<Highlight>().targetRenderer.material = miliMat;
            weaponChestPrefab.GetComponent<Highlight>().targetRenderer.enabled = false;
            weaponChestPrefab.GetComponent<Highlight>().targetRenderer.GetComponent<SkinnedMeshRenderer>().sharedMesh = null;
            weaponChestPrefab.AddComponent<Components.WeaponChest>();


            displayCaseModel.transform.parent = weaponChestPrefab.GetComponent<Highlight>().targetRenderer.transform;
            displayCaseModel.transform.localPosition = new Vector3(0f, 0f, -2.1f);
            displayCaseModel.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
            displayCaseModel.transform.localScale = Vector3.one * 1.3f;
            displayCaseModel.transform.Find("Pivot/Model/SM_Weapon_Case_low.001").gameObject.AddComponent<EntityLocator>().entity = weaponChestPrefab;
            //weaponChestPrefab.GetComponent<ModelLocator>().modelTransform = displayCaseModel.transform;

            Modules.Assets.ConvertAllRenderersToHopooShader(displayCaseModel.transform.Find("Pivot/Model/SM_Weapon_Case_low.001").gameObject);
            Modules.Assets.ConvertAllRenderersToHopooShader(displayCaseModel.transform.Find("Pivot/Model/Hinge/SM_Weapon_Case_low.002").gameObject);
            Modules.Assets.ConvertAllRenderersToHopooShader(displayCaseModel.transform.Find("Pivot/Model/Hinge/Lock").gameObject);
            displayCaseModel.transform.Find("Pivot/Model/Hinge/SM_Weapon_Case_low").gameObject.GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VendingMachine/matVendingMachineGlass.mat").WaitForCompletion();

            weaponChestPrefab.GetComponent<Highlight>().targetRenderer = displayCaseModel.transform.Find("Pivot/Model/Hinge/SM_Weapon_Case_low.002").gameObject.GetComponent<MeshRenderer>();

            weaponChestPrefab.transform.Find("HologramPivot").gameObject.SetActive(false);

            weaponChestPrefab.AddComponent<PingInfoProvider>();

            // nasty!
            GameObject pickupPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/QuestVolatileBattery/QuestVolatileBatteryWorldPickup.prefab").WaitForCompletion().InstantiateClone("HunkGunPickup", false);
            MainPlugin.Destroy(pickupPrefab.GetComponent<AwakeEvent>());
            MainPlugin.DestroyImmediate(pickupPrefab.GetComponent<NetworkParent>());
            pickupPrefab.AddComponent<HunkGunPickup>();

            Transform pickupDisplayTransform = pickupPrefab.transform.Find("PickupDisplay");
            //displayCaseModel.transform.Find("Pivot/WeaponHolder").gameObject.AddComponent<NetworkIdentity>();
            pickupPrefab.transform.parent = displayCaseModel.transform.Find("Pivot/WeaponHolder");
            pickupPrefab.transform.localPosition = Vector3.zero;
            pickupPrefab.transform.localRotation = Quaternion.identity;
            pickupDisplayTransform.localPosition = Vector3.zero;
            pickupDisplayTransform.localRotation = Quaternion.identity;
            //pickupDisplayTransform.localScale = Vector3.one * 1f;
            //displayCaseModel.transform.Find("Pivot/WeaponHolder").localScale = Vector3.one * 0.6f;

            chestInteractableCard = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            chestInteractableCard.name = "iscHunkChest";
            chestInteractableCard.prefab = weaponChestPrefab;
            chestInteractableCard.sendOverNetwork = true;
            chestInteractableCard.hullSize = HullClassification.Human;
            chestInteractableCard.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            chestInteractableCard.requiredFlags = RoR2.Navigation.NodeFlags.None;
            chestInteractableCard.forbiddenFlags = RoR2.Navigation.NodeFlags.None;

            chestInteractableCard.directorCreditCost = 0;

            chestInteractableCard.occupyPosition = true;
            chestInteractableCard.orientToFloor = true;
            chestInteractableCard.skipSpawnWhenSacrificeArtifactEnabled = false;
            //chestInteractableCard.maxSpawnsPerStage = 2;
        }

        private void CreateCase()
        {
            GameObject caseModel = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlWeaponCase"));

            weaponCasePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Chest2/Chest2.prefab").WaitForCompletion().InstantiateClone("HunkChest2", true);
            weaponCasePrefab.GetComponent<Highlight>().targetRenderer.enabled = false;
            weaponCasePrefab.GetComponent<Highlight>().targetRenderer.GetComponent<SkinnedMeshRenderer>().sharedMesh = null;
            weaponCasePrefab.AddComponent<Components.WeaponChest>();

            caseModel.transform.parent = weaponCasePrefab.GetComponent<Highlight>().targetRenderer.transform;
            caseModel.transform.localPosition = new Vector3(0f, 0f, -1.25f);
            caseModel.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
            caseModel.transform.localScale = Vector3.one * 1.3f;
            caseModel.transform.Find("Model/CaseLower_low").gameObject.AddComponent<EntityLocator>().entity = weaponCasePrefab;

            Modules.Assets.ConvertAllRenderersToHopooShader(caseModel);

            weaponCasePrefab.GetComponent<Highlight>().targetRenderer = caseModel.transform.Find("Model/CaseUpped_low").gameObject.GetComponent<MeshRenderer>();

            weaponCasePrefab.transform.Find("HologramPivot").gameObject.SetActive(false);

            weaponCasePrefab.AddComponent<PingInfoProvider>();

            // nasty!
            GameObject pickupPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/QuestVolatileBattery/QuestVolatileBatteryWorldPickup.prefab").WaitForCompletion().InstantiateClone("HunkGunPickup", false);
            MainPlugin.Destroy(pickupPrefab.GetComponent<AwakeEvent>());
            MainPlugin.DestroyImmediate(pickupPrefab.GetComponent<NetworkParent>());
            pickupPrefab.AddComponent<HunkGunPickup>();

            Transform pickupDisplayTransform = pickupPrefab.transform.Find("PickupDisplay");
            //caseModel.transform.Find("Model/WeaponHolder").gameObject.AddComponent<NetworkIdentity>();
            pickupPrefab.transform.parent = caseModel.transform.Find("Model/WeaponHolder");
            pickupPrefab.transform.localPosition = Vector3.zero;
            pickupPrefab.transform.localRotation = Quaternion.identity;
            pickupDisplayTransform.localPosition = Vector3.zero;
            pickupDisplayTransform.localRotation = Quaternion.identity;
            //pickupDisplayTransform.localScale = Vector3.one * 0.06f;
            //caseModel.transform.Find("Model/WeaponHolder").localScale = Vector3.one * 0.6f;

            caseInteractableCard = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            caseInteractableCard.name = "iscHunkChest2";
            caseInteractableCard.prefab = weaponCasePrefab;
            caseInteractableCard.sendOverNetwork = true;
            caseInteractableCard.hullSize = HullClassification.Human;
            caseInteractableCard.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            caseInteractableCard.requiredFlags = RoR2.Navigation.NodeFlags.None;
            caseInteractableCard.forbiddenFlags = RoR2.Navigation.NodeFlags.None;

            caseInteractableCard.directorCreditCost = 0;

            caseInteractableCard.occupyPosition = true;
            caseInteractableCard.orientToFloor = true;
            caseInteractableCard.skipSpawnWhenSacrificeArtifactEnabled = false;
            //chestInteractableCard.maxSpawnsPerStage = 2;
        }

        private void CreateTerminal()
        {
            GameObject terminalModel = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlTerminal"));

            terminalPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Chest2/Chest2.prefab").WaitForCompletion().InstantiateClone("HunkTerminal", true);
            //terminalPrefab.GetComponent<Highlight>().targetRenderer.material = miliMat;
            terminalPrefab.GetComponent<Highlight>().targetRenderer.enabled = false;
            terminalPrefab.GetComponent<Highlight>().targetRenderer.GetComponent<SkinnedMeshRenderer>().sharedMesh = null;
            terminalPrefab.AddComponent<Components.Terminal>();

            terminalModel.transform.parent = terminalPrefab.GetComponent<Highlight>().targetRenderer.transform;
            terminalModel.transform.localPosition = new Vector3(0f, 0.25f, -3f);
            terminalModel.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
            terminalModel.transform.localScale = new Vector3(2.8f, 1.8f, 2.8f);
            Modules.Assets.ConvertAllRenderersToHopooShader(terminalModel.gameObject);
            //terminalPrefab.GetComponent<ModelLocator>().modelTransform = terminalModel.transform;
            //^ this fixes the highlight bug but breaks the entire chest! fun!

            terminalPrefab.AddComponent<PingInfoProvider>().pingIconOverride = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texIconTerminal");

            GameObject beam = GameObject.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/arena/ArenaMissionController.prefab").WaitForCompletion().transform.Find("NullSafeZone (1)/BuiltInEffects/WardOn").gameObject);
            beam.SetActive(true);
            beam.transform.parent = terminalModel.transform;
            beam.transform.localPosition = new Vector3(0f, 0f, 0f);
            beam.transform.localRotation = Quaternion.identity;

            Material beamMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Grandparent/matGrandParentSunGlow.mat").WaitForCompletion());
            beamMat.SetColor("_TintColor", Color.red);
            beamMat.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampGolem.png").WaitForCompletion());

            beam.transform.Find("Beam").gameObject.GetComponent<ParticleSystemRenderer>().material = beamMat;


            terminalPrefab.GetComponent<Highlight>().targetRenderer = terminalModel.transform.Find("Model").gameObject.GetComponent<MeshRenderer>();
            terminalModel.transform.Find("Model").gameObject.AddComponent<EntityLocator>().entity = terminalPrefab;

            terminalPrefab.transform.Find("HologramPivot").transform.localPosition = new Vector3(0f, 2f, -1f);
            terminalPrefab.transform.Find("HologramPivot").transform.localScale = Vector3.one * 0.5f;

            terminalInteractableCard = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            terminalInteractableCard.name = "iscHunkTerminal";
            terminalInteractableCard.prefab = terminalPrefab;
            terminalInteractableCard.sendOverNetwork = true;
            terminalInteractableCard.hullSize = HullClassification.Human;
            terminalInteractableCard.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            terminalInteractableCard.requiredFlags = RoR2.Navigation.NodeFlags.None;
            terminalInteractableCard.forbiddenFlags = RoR2.Navigation.NodeFlags.None;

            terminalInteractableCard.directorCreditCost = 0;

            terminalInteractableCard.occupyPosition = true;
            terminalInteractableCard.orientToFloor = true;
            terminalInteractableCard.skipSpawnWhenSacrificeArtifactEnabled = false;
            //chestInteractableCard.maxSpawnsPerStage = 2;
        }

        private void CreatePod()
        {
            miliMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/TrimSheets/matTrimSheetMetalMilitaryEmission.mat").WaitForCompletion();

            podPanelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SurvivorPod/SurvivorPodBatteryPanel.prefab").WaitForCompletion().InstantiateClone("HunkPanel", true);
            podPanelPrefab.GetComponent<Highlight>().targetRenderer.material = miliMat;

            podContentPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/QuestVolatileBattery/QuestVolatileBatteryWorldPickup.prefab").WaitForCompletion().InstantiateClone("HunkContents", true);
            MainPlugin.Destroy(podContentPrefab.GetComponent<AwakeEvent>());
            podContentPrefab.AddComponent<HunkGunPickup>();//.weaponDef = Modules.Weapons.M19._weaponDef;

            Transform pickupDisplayTransform = podContentPrefab.transform.Find("PickupDisplay");
            pickupDisplayTransform.localPosition = new Vector3(-0.2f, 0.32f, -0.1f);
            pickupDisplayTransform.localRotation = Quaternion.Euler(new Vector3(0f, 120f, 90f));
            pickupDisplayTransform.localScale = Vector3.one * 0.35f;

            GameObject pissPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/QuestVolatileBattery/QuestVolatileBatteryWorldPickup.prefab").WaitForCompletion();

            spawnPodPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SurvivorPod/SurvivorPod.prefab").WaitForCompletion().InstantiateClone("HunkPod", true);
            Transform modelTransform = spawnPodPrefab.GetComponent<ModelLocator>().modelTransform;
            InstantiatePrefabBehavior[] ipb;
            ipb = spawnPodPrefab.GetComponents<InstantiatePrefabBehavior>();
            foreach (InstantiatePrefabBehavior prefab in ipb)
            {
                if (prefab.prefab == Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SurvivorPod/SurvivorPodBatteryPanel.prefab").WaitForCompletion())
                    prefab.prefab = podPanelPrefab;

                if (prefab.prefab == pissPrefab)
                    prefab.prefab = podContentPrefab;
            }
            modelTransform.Find("EscapePodArmature/Base/Door/EscapePodDoorMesh").GetComponent<MeshRenderer>().material = miliMat;
            modelTransform.Find("EscapePodArmature/Base/ReleaseExhaustFX/Door,Physics").GetComponent<MeshRenderer>().material = miliMat;
            modelTransform.Find("EscapePodArmature/Base/EscapePodMesh").GetComponent<MeshRenderer>().material = miliMat;
            modelTransform.Find("EscapePodArmature/Base/RotatingPanel/EscapePodMesh.002").GetComponent<MeshRenderer>().material = miliMat;
        }

        private void CreateHealthBarStyle()
        {
            infectedHealthBarStyle = HealthBarStyle.Instantiate(Addressables.LoadAssetAsync<HealthBarStyle>("RoR2/Base/Common/CombatHealthBar.asset").WaitForCompletion());
            infectedHealthBarStyle.name = "InfectedHealthBar";
            infectedHealthBarStyle.trailingOverHealthBarStyle.baseColor = new Color(1f, 42f / 255f, 107f / 255f);

            infectedHealthBarStyle.trailingUnderHealthBarStyle.sprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texWhite");
            infectedHealthBarStyle.trailingUnderHealthBarStyle.baseColor = new Color(1f, 1f, 0f);

            tInfectedHealthBarStyle = HealthBarStyle.Instantiate(Addressables.LoadAssetAsync<HealthBarStyle>("RoR2/Base/Common/CombatHealthBar.asset").WaitForCompletion());
            tInfectedHealthBarStyle.name = "TInfectedHealthBar";
            tInfectedHealthBarStyle.trailingOverHealthBarStyle.baseColor = new Color(28f / 255f, 69f / 255f, 1f);

            tInfectedHealthBarStyle.trailingUnderHealthBarStyle.sprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texWhite");
            tInfectedHealthBarStyle.trailingUnderHealthBarStyle.baseColor = new Color(1f, 1f, 0f);
        }

        private static void CreateOrb()
        {
            ammoOrb = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/InfusionOrbEffect"), "HunkAmmoOrbEffect", true);
            if (!ammoOrb.GetComponent<NetworkIdentity>()) ammoOrb.AddComponent<NetworkIdentity>();

            TrailRenderer trail = ammoOrb.transform.Find("TrailParent").Find("Trail").GetComponent<TrailRenderer>();
            trail.widthMultiplier = 0.25f;
            trail.material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Grandparent/matGrandparentTeleportOutBoom.mat").WaitForCompletion();

            ammoOrb.transform.Find("VFX").Find("Core").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Loader/matOmniRing2Loader.mat").WaitForCompletion();
            ammoOrb.transform.Find("VFX").localScale = Vector3.one * 0.8f;

            ammoOrb.transform.Find("VFX").Find("Core").localScale = Vector3.one * 2.5f;

            ammoOrb.transform.Find("VFX").Find("PulseGlow").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Grandparent/matGrandParentSunGlow.mat").WaitForCompletion();

            OrbEffect orb = ammoOrb.GetComponent<OrbEffect>();
            orb.startVelocity1 = new Vector3(0f, 0f, 0f);
            orb.startVelocity2 = new Vector3(1f, 1f, 1f);

            orb.startEffect = Modules.Assets.ammoSpawnEffect;
            orb.endEffect = Modules.Assets.ammoPickupSparkle;
            orb.onArrival = new UnityEngine.Events.UnityEvent();

            Modules.Assets.AddNewEffectDef(ammoOrb);
        }

        private void CreateKeycards()
        {
            spadeKeycard = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/ArtifactKey/ArtifactKey.asset").WaitForCompletion());
            spadeKeycard.name = "SpadeKeycard";
            spadeKeycard.nameToken = "ROB_HUNK_SPADE_KEYCARD_NAME";
            spadeKeycard.descriptionToken = "ROB_HUNK_KEYCARD_DESC";
            spadeKeycard.pickupToken = "ROB_HUNK_KEYCARD_DESC";
            spadeKeycard.loreToken = "ROB_HUNK_KEYCARD_DESC";
            spadeKeycard.canRemove = false;
            spadeKeycard.hidden = false;
            spadeKeycard.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKeycardSpadeIcon");
            spadeKeycard.requiredExpansion = null;
            spadeKeycard.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotDuplicate,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };
            spadeKeycard.unlockableDef = null;

            spadeKeycard.pickupModelPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlKeycardSpade");
            Modules.Assets.ConvertAllRenderersToHopooShader(spadeKeycard.pickupModelPrefab);

            clubKeycard = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/ArtifactKey/ArtifactKey.asset").WaitForCompletion());
            clubKeycard.name = "ClubKeycard";
            clubKeycard.nameToken = "ROB_HUNK_CLUB_KEYCARD_NAME";
            clubKeycard.descriptionToken = "ROB_HUNK_KEYCARD_DESC";
            clubKeycard.pickupToken = "ROB_HUNK_KEYCARD_DESC";
            clubKeycard.loreToken = "ROB_HUNK_KEYCARD_DESC";
            clubKeycard.canRemove = false;
            clubKeycard.hidden = false;
            clubKeycard.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKeycardClubIcon");
            clubKeycard.requiredExpansion = null;
            clubKeycard.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotDuplicate,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };
            clubKeycard.unlockableDef = null;

            clubKeycard.pickupModelPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlKeycardClub");
            Modules.Assets.ConvertAllRenderersToHopooShader(clubKeycard.pickupModelPrefab);

            heartKeycard = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/ArtifactKey/ArtifactKey.asset").WaitForCompletion());
            heartKeycard.name = "HeartKeycard";
            heartKeycard.nameToken = "ROB_HUNK_HEART_KEYCARD_NAME";
            heartKeycard.descriptionToken = "ROB_HUNK_KEYCARD_DESC";
            heartKeycard.pickupToken = "ROB_HUNK_KEYCARD_DESC";
            heartKeycard.loreToken = "ROB_HUNK_KEYCARD_DESC";
            heartKeycard.canRemove = false;
            heartKeycard.hidden = false;
            heartKeycard.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKeycardHeartIcon");
            heartKeycard.requiredExpansion = null;
            heartKeycard.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotDuplicate,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };
            heartKeycard.unlockableDef = null;

            heartKeycard.pickupModelPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlKeycardHeart");
            Modules.Assets.ConvertAllRenderersToHopooShader(heartKeycard.pickupModelPrefab);

            diamondKeycard = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/ArtifactKey/ArtifactKey.asset").WaitForCompletion());
            diamondKeycard.name = "DiamondKeycard";
            diamondKeycard.nameToken = "ROB_HUNK_DIAMOND_KEYCARD_NAME";
            diamondKeycard.descriptionToken = "ROB_HUNK_KEYCARD_DESC";
            diamondKeycard.pickupToken = "ROB_HUNK_KEYCARD_DESC";
            diamondKeycard.loreToken = "ROB_HUNK_KEYCARD_DESC";
            diamondKeycard.canRemove = false;
            diamondKeycard.hidden = false;
            diamondKeycard.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKeycardDiamondIcon");
            diamondKeycard.requiredExpansion = null;
            diamondKeycard.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotDuplicate,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };
            diamondKeycard.unlockableDef = null;

            diamondKeycard.pickupModelPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlKeycardDiamond");
            Modules.Assets.ConvertAllRenderersToHopooShader(diamondKeycard.pickupModelPrefab);

            wristband = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/ArtifactKey/ArtifactKey.asset").WaitForCompletion());
            wristband.name = "IDWristband";
            wristband.nameToken = "ROB_HUNK_WRISTBAND_NAME";
            wristband.descriptionToken = "ROB_HUNK_WRISTBAND_DESC";
            wristband.pickupToken = "ROB_HUNK_WRISTBAND_DESC";
            wristband.loreToken = "ROB_HUNK_WRISTBAND_DESC";
            wristband.canRemove = false;
            wristband.hidden = false;
            wristband.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texWristbandIcon");
            wristband.requiredExpansion = null;
            wristband.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotDuplicate,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };
            wristband.unlockableDef = null;

            wristband.pickupModelPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlWristband");
            Modules.Assets.ConvertAllRenderersToHopooShader(wristband.pickupModelPrefab);

            goldKeycard = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/ArtifactKey/ArtifactKey.asset").WaitForCompletion());
            goldKeycard.name = "goldKeycard";
            goldKeycard.nameToken = "ROB_HUNK_GOLD_KEYCARD_NAME";
            goldKeycard.descriptionToken = "ROB_HUNK_KEYCARD_DESC";
            goldKeycard.pickupToken = "ROB_HUNK_KEYCARD_DESC";
            goldKeycard.loreToken = "ROB_HUNK_KEYCARD_DESC";
            goldKeycard.canRemove = false;
            goldKeycard.hidden = false;
            goldKeycard.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKeycardGoldIcon");
            goldKeycard.requiredExpansion = null;
            goldKeycard.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotDuplicate,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };
            goldKeycard.unlockableDef = null;

            goldKeycard.pickupModelPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlKeycardGold");
            Modules.Assets.ConvertAllRenderersToHopooShader(goldKeycard.pickupModelPrefab);

            masterKeycard = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/ArtifactKey/ArtifactKey.asset").WaitForCompletion());
            masterKeycard.name = "masterKeycard";
            masterKeycard.nameToken = "ROB_HUNK_MASTER_KEYCARD_NAME";
            masterKeycard.descriptionToken = "ROB_HUNK_KEYCARD_MASTER_DESC";
            masterKeycard.pickupToken = "ROB_HUNK_KEYCARD_MASTER_DESC";
            masterKeycard.loreToken = "ROB_HUNK_KEYCARD_MASTER_DESC";
            masterKeycard.canRemove = false;
            masterKeycard.hidden = false;
            masterKeycard.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKeycardMasterIcon");
            masterKeycard.requiredExpansion = null;
            masterKeycard.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotDuplicate,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };
            masterKeycard.unlockableDef = null;

            masterKeycard.pickupModelPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlKeycardMaster");
            Modules.Assets.ConvertAllRenderersToHopooShader(masterKeycard.pickupModelPrefab);

            gVirusSample = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/ArtifactKey/ArtifactKey.asset").WaitForCompletion());
            gVirusSample.name = "GVirusSample";
            gVirusSample.nameToken = "ROB_HUNK_G_VIRUS_SAMPLE_NAME";
            gVirusSample.descriptionToken = "ROB_HUNK_G_VIRUS_SAMPLE_DESC";
            gVirusSample.pickupToken = "ROB_HUNK_G_VIRUS_SAMPLE_DESC";
            gVirusSample.loreToken = "ROB_HUNK_G_VIRUS_SAMPLE_DESC";
            gVirusSample.canRemove = false;
            gVirusSample.hidden = false;
            gVirusSample.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texVirusSampleIcon");
            gVirusSample.requiredExpansion = null;
            gVirusSample.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotDuplicate,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };
            gVirusSample.unlockableDef = null;

            gVirusSample.pickupModelPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlGVirusSample");
            Modules.Assets.ConvertAllRenderersToHopooShader(gVirusSample.pickupModelPrefab);
            gVirusSample.pickupModelPrefab.transform.Find("Model/Glass").GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/HealingPotion/matHealingPotionGlass.mat").WaitForCompletion();
            gVirusSample.pickupModelPrefab.transform.Find("Model/Glass2").GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/HealingPotion/matHealingPotionGlass.mat").WaitForCompletion();
            //gVirusSample.pickupModelPrefab.transform.Find("Model/Liquid").GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/EliteVoid/matVoidInfestorEyes.mat").WaitForCompletion();

            tVirusSample = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/ArtifactKey/ArtifactKey.asset").WaitForCompletion());
            tVirusSample.name = "TVirusSample";
            tVirusSample.nameToken = "ROB_HUNK_T_VIRUS_SAMPLE_NAME";
            tVirusSample.descriptionToken = "ROB_HUNK_T_VIRUS_SAMPLE_DESC";
            tVirusSample.pickupToken = "ROB_HUNK_T_VIRUS_SAMPLE_DESC";
            tVirusSample.loreToken = "ROB_HUNK_T_VIRUS_SAMPLE_DESC";
            tVirusSample.canRemove = false;
            tVirusSample.hidden = false;
            tVirusSample.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texTVirusSampleIcon");
            tVirusSample.requiredExpansion = null;
            tVirusSample.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotDuplicate,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };
            tVirusSample.unlockableDef = null;

            tVirusSample.pickupModelPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlTVirusSample");
            Modules.Assets.ConvertAllRenderersToHopooShader(tVirusSample.pickupModelPrefab);
            tVirusSample.pickupModelPrefab.transform.Find("Model/Glass").GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/HealingPotion/matHealingPotionGlass.mat").WaitForCompletion();
            tVirusSample.pickupModelPrefab.transform.Find("Model/Glass2").GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/HealingPotion/matHealingPotionGlass.mat").WaitForCompletion();
            //gVirusSample.pickupModelPrefab.transform.Find("Model/Liquid").GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/EliteVoid/matVoidInfestorEyes.mat").WaitForCompletion();

            tVirus = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/DrizzlePlayerHelper/DrizzlePlayerHelper.asset").WaitForCompletion());
            tVirus.name = "TVirus";
            tVirus.nameToken = "ROB_HUNK_T_VIRUS_NAME";
            tVirus.descriptionToken = "ROB_HUNK_T_VIRUS_DESC";
            tVirus.pickupToken = "ROB_HUNK_T_VIRUS_DESC";
            tVirus.loreToken = "ROB_HUNK_T_VIRUS_DESC";
            tVirus.canRemove = false;
            tVirus.hidden = true;
            tVirus.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKeycardDiamondIcon");
            tVirus.requiredExpansion = null;
            tVirus.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotDuplicate,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };
            tVirus.unlockableDef = null;

            tVirusRevival = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/DrizzlePlayerHelper/DrizzlePlayerHelper.asset").WaitForCompletion());
            tVirusRevival.name = "TVirusRevival";
            tVirusRevival.nameToken = "ROB_HUNK_T_VIRUS_NAME";
            tVirusRevival.descriptionToken = "ROB_HUNK_T_VIRUS_DESC";
            tVirusRevival.pickupToken = "ROB_HUNK_T_VIRUS_DESC";
            tVirusRevival.loreToken = "ROB_HUNK_T_VIRUS_DESC";
            tVirusRevival.canRemove = false;
            tVirusRevival.hidden = true;
            tVirusRevival.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKeycardDiamondIcon");
            tVirusRevival.requiredExpansion = null;
            tVirusRevival.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotDuplicate,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };
            tVirusRevival.unlockableDef = null;

            gVirus = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/DrizzlePlayerHelper/DrizzlePlayerHelper.asset").WaitForCompletion());
            gVirus.name = "GVirus";
            gVirus.nameToken = "ROB_HUNK_G_VIRUS_NAME";
            gVirus.descriptionToken = "ROB_HUNK_G_VIRUS_DESC";
            gVirus.pickupToken = "ROB_HUNK_G_VIRUS_DESC";
            gVirus.loreToken = "ROB_HUNK_G_VIRUS_DESC";
            gVirus.canRemove = false;
            gVirus.hidden = true;
            gVirus.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKeycardDiamondIcon");
            gVirus.requiredExpansion = null;
            gVirus.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotDuplicate,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };
            gVirus.unlockableDef = null;

            gVirus2 = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/DrizzlePlayerHelper/DrizzlePlayerHelper.asset").WaitForCompletion());
            gVirus2.name = "GVirus2";
            gVirus2.nameToken = "ROB_HUNK_G_VIRUS2_NAME";
            gVirus2.descriptionToken = "ROB_HUNK_G_VIRUS_DESC";
            gVirus2.pickupToken = "ROB_HUNK_G_VIRUS_DESC";
            gVirus2.loreToken = "ROB_HUNK_G_VIRUS_DESC";
            gVirus2.canRemove = false;
            gVirus2.hidden = true;
            gVirus2.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKeycardDiamondIcon");
            gVirus2.requiredExpansion = null;
            gVirus2.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotDuplicate,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };
            gVirus2.unlockableDef = null;

            gVirusFinal = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/DrizzlePlayerHelper/DrizzlePlayerHelper.asset").WaitForCompletion());
            gVirusFinal.name = "GVirusFinal";
            gVirusFinal.nameToken = "ROB_HUNK_G_VIRUSFINAL_NAME";
            gVirusFinal.descriptionToken = "ROB_HUNK_G_VIRUS_DESC";
            gVirusFinal.pickupToken = "ROB_HUNK_G_VIRUS_DESC";
            gVirusFinal.loreToken = "ROB_HUNK_G_VIRUS_DESC";
            gVirusFinal.canRemove = false;
            gVirusFinal.hidden = true;
            gVirusFinal.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKeycardDiamondIcon");
            gVirusFinal.requiredExpansion = null;
            gVirusFinal.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotDuplicate,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };
            gVirusFinal.unlockableDef = null;

            ammoItem = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/ArtifactKey/ArtifactKey.asset").WaitForCompletion());
            ammoItem.name = "HunkAmmo";
            ammoItem.nameToken = "ROB_HUNK_AMMOITEM_NAME";
            ammoItem.descriptionToken = "ROB_HUNK_AMMOITEM_DESC";
            ammoItem.pickupToken = "ROB_HUNK_AMMOITEM_DESC";
            ammoItem.loreToken = "ROB_HUNK_AMMOITEM_DESC";
            ammoItem.canRemove = false;
            ammoItem.hidden = true;
            ammoItem.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKeycardDiamondIcon");
            ammoItem.requiredExpansion = null;
            ammoItem.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotDuplicate,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };
            ammoItem.unlockableDef = null;

            HunkWeaponCatalog.itemDefs.Add(spadeKeycard);
            HunkWeaponCatalog.itemDefs.Add(clubKeycard);
            HunkWeaponCatalog.itemDefs.Add(heartKeycard);
            HunkWeaponCatalog.itemDefs.Add(diamondKeycard);
            HunkWeaponCatalog.itemDefs.Add(wristband);
            HunkWeaponCatalog.itemDefs.Add(goldKeycard);
            HunkWeaponCatalog.itemDefs.Add(masterKeycard);
            HunkWeaponCatalog.itemDefs.Add(gVirusSample);
            HunkWeaponCatalog.itemDefs.Add(gVirus);
            HunkWeaponCatalog.itemDefs.Add(gVirus2);
            HunkWeaponCatalog.itemDefs.Add(gVirusFinal);
            HunkWeaponCatalog.itemDefs.Add(tVirusSample);
            HunkWeaponCatalog.itemDefs.Add(tVirus);
            HunkWeaponCatalog.itemDefs.Add(tVirusRevival);
            HunkWeaponCatalog.itemDefs.Add(ammoItem);
        }

        private void CreateAmmoInteractable()
        {
            ammoPickupInteractable = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Barrel1/Barrel1.prefab").WaitForCompletion().InstantiateClone("HunkAmmoPickupInteractable", true);

            MainPlugin.Destroy(ammoPickupInteractable.GetComponent<BarrelInteraction>());
            MainPlugin.Destroy(ammoPickupInteractable.GetComponent<Highlight>());
            MainPlugin.Destroy(ammoPickupInteractable.GetComponent<GenericDisplayNameProvider>());

            ammoPickupInteractable.GetComponent<SfxLocator>().openSound = "sfx_hunk_pickup";

            Transform modelTransform = ammoPickupInteractable.GetComponent<ModelLocator>().modelTransform;
            ammoPickupInteractable.AddComponent<AmmoPickupInteraction>().destroyOnOpen = modelTransform.gameObject;
            modelTransform.GetComponent<Animator>().enabled = false;
            modelTransform.Find("BarrelMesh").GetComponent<SkinnedMeshRenderer>().enabled = false;
            ammoPickupInteractable.GetComponent<AmmoPickupInteraction>().multiplier = 1f;

            GameObject interactionEffect = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("AmmoInteraction"));
            interactionEffect.transform.SetParent(modelTransform, false);
            interactionEffect.transform.localPosition = new Vector3(0f, 0.75f, 0f);
            interactionEffect.transform.localRotation = Quaternion.identity;
        }

        private void CreateBarrelAmmoInteractable()
        {
            ammoPickupInteractableSmall = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Barrel1/Barrel1.prefab").WaitForCompletion().InstantiateClone("HunkAmmoPickupInteractableSmall", true);

            MainPlugin.Destroy(ammoPickupInteractableSmall.GetComponent<BarrelInteraction>());
            MainPlugin.Destroy(ammoPickupInteractableSmall.GetComponent<Highlight>());
            MainPlugin.Destroy(ammoPickupInteractableSmall.GetComponent<GenericDisplayNameProvider>());

            ammoPickupInteractableSmall.GetComponent<SfxLocator>().openSound = "sfx_hunk_pickup";

            Transform modelTransform = ammoPickupInteractableSmall.GetComponent<ModelLocator>().modelTransform;
            ammoPickupInteractableSmall.AddComponent<AmmoPickupInteraction>().destroyOnOpen = modelTransform.gameObject;
            modelTransform.GetComponent<Animator>().enabled = false;
            modelTransform.Find("BarrelMesh").GetComponent<SkinnedMeshRenderer>().enabled = false;
            ammoPickupInteractableSmall.GetComponent<AmmoPickupInteraction>().multiplier = 0.25f;

            GameObject interactionEffect = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("AmmoInteraction"));
            interactionEffect.transform.SetParent(modelTransform, false);
            interactionEffect.transform.localPosition = new Vector3(0f, 1f, 0f);
            interactionEffect.transform.localRotation = Quaternion.identity;
        }

        private static void Hook()
        {
            // headshots and ammo drops
            RoR2.GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;

            // custom hud
            RoR2.UI.HUD.onHudTargetChangedGlobal += HUDSetup;

            // hide the bazooka skin
            On.RoR2.UI.LoadoutPanelController.Row.AddButton += Row_AddButton;

            // rummage passive
            On.RoR2.ChestBehavior.Open += ChestBehavior_Open;
            On.RoR2.ChestBehavior.ItemDrop += ChestBehavior_ItemDrop;
            On.RoR2.ChestBehavior.Roll += ChestBehavior_Roll;
            On.RoR2.BarrelInteraction.CoinDrop += BarrelInteraction_CoinDrop;
            On.RoR2.ShopTerminalBehavior.DropPickup += ShopTerminalBehavior_DropPickup;
            On.RoR2.RouletteChestController.EjectPickupServer += RouletteChestController_EjectPickupServer;
            On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;
            On.RoR2.PickupDropletController.OnCollisionEnter += PickupDropletController_OnCollisionEnter;

            // bandolier
            On.RoR2.SkillLocator.ApplyAmmoPack += SkillLocator_ApplyAmmoPack;

            // custom shield overlay
            if (Modules.Config.fancyShieldGlobal.Value) On.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlays2;
            else if (Modules.Config.fancyShield.Value) On.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlays;

            // knife ammo drop mechanic
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;

            // help me
            On.RoR2.Inventory.ShrineRestackInventory += Inventory_ShrineRestackInventory;

            // add chest cost types
            CostTypeCatalog.modHelper.getAdditionalEntries += AddHeartCostType;
            CostTypeCatalog.modHelper.getAdditionalEntries += AddSpadeCostType;
            CostTypeCatalog.modHelper.getAdditionalEntries += AddDiamondCostType;
            CostTypeCatalog.modHelper.getAdditionalEntries += AddClubCostType;
            CostTypeCatalog.modHelper.getAdditionalEntries += AddStarCostType;
            CostTypeCatalog.modHelper.getAdditionalEntries += AddWristbandCostType;
            CostTypeCatalog.modHelper.getAdditionalEntries += AddSampleCostType;

            // spawn g-young
            On.RoR2.CharacterBody.OnDeathStart += CharacterBody_OnDeathStart;

            // t-virus revival
            On.RoR2.CharacterMaster.OnBodyDeath += CharacterMaster_OnBodyDeath;
            //On.RoR2.CharacterMaster.RespawnExtraLife += CharacterMaster_RespawnExtraLife;
            //On.RoR2.CharacterMaster.IsDeadAndOutOfLivesServer += CharacterMaster_IsDeadAndOutOfLivesServer;
            //On.EntityStates.GenericCharacterDeath.OnEnter += GenericCharacterDeath_OnEnter;
            IL.RoR2.HealthComponent.TakeDamage += TVirusDeathIL;

            // place chests
            On.RoR2.SceneDirector.Start += SceneDirector_Start;

            // set objective bullshit..
            On.RoR2.UI.ObjectivePanelController.GetObjectiveSources += ObjectivePanelController_GetObjectiveSources;

            // spawn rocket launcher on mithrix last phase
            //On.EntityStates.BrotherMonster.UltExitState.OnEnter += UltExitState_OnEnter;
            On.EntityStates.Missions.BrotherEncounter.Phase4.OnEnter += Phase4_OnEnter;

            // prevent guns from being picked up by non hunk or hunk with full inventory
            On.RoR2.GenericPickupController.AttemptGrant += GenericPickupController_AttemptGrant;

            // infected health bar
            On.RoR2.UI.HealthBar.Update += HealthBar_Update;

            // infected name tag
            On.RoR2.Util.GetBestBodyName += MakeInfectedName;

            // what
            On.RoR2.UI.LoadoutPanelController.Rebuild += LoadoutPanelController_Rebuild;

            // make dodges more consistent
            On.EntityStates.Bison.Charge.FixedUpdate += Charge_FixedUpdate;
            On.EntityStates.ClayBruiser.Weapon.MinigunFire.FixedUpdate += MinigunFire_FixedUpdate;
            On.EntityStates.BrotherMonster.WeaponSlam.FixedUpdate += WeaponSlam_FixedUpdate;
            On.EntityStates.ParentMonster.GroundSlam.FixedUpdate += GroundSlam_FixedUpdate;
            On.EntityStates.BeetleGuardMonster.GroundSlam.FixedUpdate += GroundSlam_FixedUpdate1;

            // escape bgm
            On.RoR2.EscapeSequenceController.BeginEscapeSequence += EscapeSequenceController_BeginEscapeSequence;

            // network dodge. fuck you.
            On.RoR2.CharacterBody.OnSkillActivated += CharacterBody_OnSkillActivated;

            // if i speak i am in trouble
            if (Modules.Config.menuSFX.Value) On.RoR2.UI.MainMenu.BaseMainMenuScreen.Awake += BaseMainMenuScreen_Awake;
            On.RoR2.UI.MainMenu.BaseMainMenuScreen.Update += BaseMainMenuScreen_Update;
            // 🙈 🙉 🙊

            // heresy anims
            On.EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.OnEnter += PlayVisionsAnimation;
            On.EntityStates.GlobalSkills.LunarNeedle.ChargeLunarSecondary.PlayChargeAnimation += PlayChargeLunarAnimation;
            On.EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary.PlayThrowAnimation += PlayThrowLunarAnimation;
            On.EntityStates.GlobalSkills.LunarDetonator.Detonate.OnEnter += PlayRuinAnimation;
        }

        private static void TVirusDeathIL(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            bool ILFound = c.TryGotoNext(MoveType.Before,
                x => x.MatchLdloc(11),
                x => x.MatchCallOrCallvirt<GlobalEventManager>(nameof(GlobalEventManager.ServerDamageDealt)),
                x => x.MatchLdarg(0),
                x => x.MatchCallOrCallvirt<HealthComponent>("get_alive")
            );

            c.Index += 3;

            if (ILFound)
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Action<HealthComponent>>((hc) =>
                {
                    if (hc.health <= 0 && hc.body.HasBuff(Hunk.infectedBuff2))
                    {
                        if (hc.body.master)
                        {
                            TVirusMaster virus = hc.body.master.GetComponent<TVirusMaster>();
                            if (virus && virus.revivalCount > 0)
                            {
                                virus.revivalCount--;
                                if (virus.revivalCount == 0) hc.health = hc.fullHealth * 0.5f;
                                else hc.health = hc.fullHealth;

                                hc.body.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.5f);

                                NetworkIdentity identity = hc.GetComponent<NetworkIdentity>();
                                if (identity) new SyncTVirusOverlay(identity.netId, hc.gameObject).Send(NetworkDestination.Clients);
                            }
                        }
                    }
                });
            }
        }

        private static bool CharacterMaster_IsDeadAndOutOfLivesServer(On.RoR2.CharacterMaster.orig_IsDeadAndOutOfLivesServer orig, CharacterMaster self)
        {
            if (NetworkServer.active)
            {
                CharacterBody body = self.GetBody();
                if (!body || !body.healthComponent.alive)
                {
                    if (self.inventory.GetItemCount(Hunk.tVirusRevival) >= 0) return false;
                }
            }

            return orig(self);
        }

        private static void GenericCharacterDeath_OnEnter(On.EntityStates.GenericCharacterDeath.orig_OnEnter orig, EntityStates.GenericCharacterDeath self)
        {
            if (Hunk.virusObjectiveObjects2.Count > 0 && self.GetComponent<TVirusHandler>())
            {
                if (self.characterBody && self.characterBody.inventory && self.characterBody.inventory.GetItemCount(Hunk.tVirusRevival) > 0)
                {
                    if (self.modelLocator && self.modelLocator.modelTransform)
                    {
                        self.modelLocator.modelTransform.gameObject.AddComponent<DestroyOnTimer>().duration = 1f;
                    }
                }
            }

            orig(self);
        }

        private static void CharacterMaster_RespawnExtraLife(On.RoR2.CharacterMaster.orig_RespawnExtraLife orig, CharacterMaster self)
        {
            if (Hunk.virusObjectiveObjects2.Count > 0 && self.GetComponent<TVirusRevivalBehavior>())
            {
                Vector3 vector = self.deathFootPosition;
                if (self.killedByUnsafeArea)
                {
                    vector = (TeleportHelper.FindSafeTeleportDestination(self.deathFootPosition, self.bodyPrefab.GetComponent<CharacterBody>(), RoR2Application.rng) ?? self.deathFootPosition);
                }

                Quaternion rot = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
                if (self.bodyInstanceObject)
                {
                    var cd = self.bodyInstanceObject.GetComponent<CharacterDirection>();
                    if (cd) rot = Quaternion.Euler(cd.forward);
                }

                self.Respawn(vector, rot);
                self.GetBody().AddTimedBuff(RoR2Content.Buffs.Immune, 0.25f);

                //GameObject gameObject = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/HippoRezEffect");
                if (self.bodyInstanceObject)
                {
                    foreach (EntityStateMachine entityStateMachine in self.bodyInstanceObject.GetComponents<EntityStateMachine>())
                    {
                        entityStateMachine.initialStateType = entityStateMachine.mainStateType;
                    }
                    /*if (gameObject)
                    {
                        EffectManager.SpawnEffect(gameObject, new EffectData
                        {
                            origin = vector,
                            rotation = self.bodyInstanceObject.transform.rotation
                        }, true);
                    }*/
                }

                return;
            }
            orig(self);
        }

        private static void CharacterMaster_OnBodyDeath(On.RoR2.CharacterMaster.orig_OnBodyDeath orig, CharacterMaster self, CharacterBody body)
        {
            if (self && body)
            {
                if (Hunk.virusObjectiveObjects2.Count > 0)
                {
                    if (body.HasBuff(Hunk.infectedBuff2))
                    {
                        Hunk.requiredKills--;
                        if (Hunk.requiredKills <= 0)
                        {
                            Hunk.requiredKills = 10;

                            PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(Modules.Survivors.Hunk.tVirusSample.itemIndex),
body.corePosition + (Vector3.up * 0.5f),
Vector3.up * 20f);
                        }
                    }
                }

                /*if (Hunk.virusObjectiveObjects2.Count > 0)
                {
                    if (self.inventory && self.inventory.GetItemCount(Hunk.tVirusRevival) > 0)
                    {
                        self.lostBodyToDeath = true;
                        self.deathFootPosition = body.footPosition;

                        BaseAI[] array = self.aiComponents;
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i].OnBodyDeath(body);
                        }

                        if (self.playerCharacterMasterController) self.playerCharacterMasterController.OnBodyDeath();

                        self.ResetLifeStopwatch();
                        self.onBodyDeath?.Invoke();

                        self.gameObject.AddComponent<TVirusRevivalBehavior>().master = self;
                    }
                }*/
            }

            orig(self, body);
        }

        private static void PickupDropletController_OnCollisionEnter(On.RoR2.PickupDropletController.orig_OnCollisionEnter orig, PickupDropletController self, Collision collision)
        {
            if (self)
            {
                if (self.pickupIndex == PickupCatalog.FindPickupIndex(Hunk.ammoItem.itemIndex))
                {
                    if (NetworkServer.active && self.alive)
                    {
                        self.alive = false;
                        NetworkServer.Spawn(GameObject.Instantiate(Hunk.instance.ammoPickupInteractable, self.transform.position, Quaternion.identity));
                        UnityEngine.Object.Destroy(self.gameObject);
                    }
                    return;
                }
            }

            orig(self, collision);
        }

        private static void CharacterBody_OnDeathStart(On.RoR2.CharacterBody.orig_OnDeathStart orig, CharacterBody self)
        {
            if (self)
            {
                VirusHandler virusHandler = self.GetComponent<VirusHandler>();
                if (virusHandler) virusHandler.TrySpawn();
            }

            orig(self);
        }

        private static void GroundSlam_FixedUpdate1(On.EntityStates.BeetleGuardMonster.GroundSlam.orig_FixedUpdate orig, EntityStates.BeetleGuardMonster.GroundSlam self)
        {
            if (self.characterBody) self.characterBody.outOfCombatStopwatch = 0f;
            orig(self);
        }

        private static void GroundSlam_FixedUpdate(On.EntityStates.ParentMonster.GroundSlam.orig_FixedUpdate orig, EntityStates.ParentMonster.GroundSlam self)
        {
            if (self.characterBody) self.characterBody.outOfCombatStopwatch = 0f;
            orig(self);
        }

        private static void WeaponSlam_FixedUpdate(On.EntityStates.BrotherMonster.WeaponSlam.orig_FixedUpdate orig, EntityStates.BrotherMonster.WeaponSlam self)
        {
            if (self.characterBody) self.characterBody.outOfCombatStopwatch = 0f;
            orig(self);
        }

        private static void CharacterModel_UpdateOverlays(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
        {
            Material cachedMat1 = null;
            Material cachedMat2 = null;

            if (self && self.body && self.body.baseNameToken == Hunk.bodyNameToken)
            {
                cachedMat1 = CharacterModel.energyShieldMaterial;
                cachedMat2 = CharacterModel.voidShieldMaterial;

                CharacterModel.energyShieldMaterial = Modules.Assets.shieldOverlayMat;
                CharacterModel.voidShieldMaterial = Modules.Assets.voidShieldOverlayMat;
            }

            orig(self);

            if (self && self.body && self.body.baseNameToken == Hunk.bodyNameToken)
            {
                CharacterModel.energyShieldMaterial = cachedMat1;
                CharacterModel.voidShieldMaterial = cachedMat2;
            }
        }

        private static void CharacterModel_UpdateOverlays2(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
        {
            CharacterModel.energyShieldMaterial = Modules.Assets.shieldOverlayMat;
            CharacterModel.voidShieldMaterial = Modules.Assets.voidShieldOverlayMat;
            orig(self);
        }

        private static void EscapeSequenceController_BeginEscapeSequence(On.RoR2.EscapeSequenceController.orig_BeginEscapeSequence orig, EscapeSequenceController self)
        {
            orig(self);
            
            if (Modules.Config.customEscapeSequence.Value)
            {
                foreach (Modules.Components.HunkController i in MonoBehaviour.FindObjectsOfType<Modules.Components.HunkController>())
                {
                    if (i && i.characterBody.hasAuthority)
                    {
                        i.StartBGM();
                        i.StartDialogue2();
                    }
                }
            }
        }

        private static void GenericPickupController_AttemptGrant(On.RoR2.GenericPickupController.orig_AttemptGrant orig, GenericPickupController self, CharacterBody body)
        {
            if (self && body)
            {
                if (self.pickupIndex.isValid)
                {
                    string nameToken = PickupCatalog.GetPickupDef(self.pickupIndex).nameToken;
                    if (nameToken.Contains("ROB_HUNK_WEAPON_") && !nameToken.Contains("ADDON"))
                    {
                        if (body.baseNameToken == Hunk.bodyNameToken)
                        {
                            HunkController hunk = body.GetComponent<HunkController>();
                            if (!hunk) return; // < should never happen
                            foreach (HunkWeaponData i in hunk.weaponTracker.weaponData)
                            {
                                if (i.weaponDef.itemDef.nameToken == nameToken)
                                {
                                    if (hunk.notificationHandler) hunk.notificationHandler.Init("You already have a " + Language.GetString(i.weaponDef.nameToken), Color.red);
                                    return; // prevent duplicate pickups
                                }
                            }

                            if (hunk.weaponTracker.weaponData.Length > 7)
                            {
                                if (hunk.notificationHandler) hunk.notificationHandler.Init("Inventory full!\nDrop a weapon to pick this up", Color.red);
                                return; // prevent excess pickups
                            }
                        }
                        else
                        {
                            return; // non-hunk can't pick up guns
                        }
                    }

                    // prevent others from grabbing samples and keycards
                    if (Modules.Config.blacklistHunkItems.Value)
                    {
                        if (nameToken == Hunk.gVirusSample.nameToken || (nameToken.Contains("ROB_HUNK_") && nameToken.Contains("_KEYCARD_")))
                        {
                            if (body.baseNameToken != Hunk.bodyNameToken)
                            {
                                return;
                            }
                        }
                    }
                }
            }

            orig(self, body);
        }

        private static void CharacterBody_OnSkillActivated(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody self, GenericSkill skill)
        {
            if (self && skill)
            {
                if (skill.isCombatSkill)
                {
                    NetworkIdentity identity = self.GetComponent<NetworkIdentity>();
                    if (identity) new SyncCombatStopwatch(identity.netId, self.gameObject).Send(NetworkDestination.Clients);
                }
            }
            orig(self, skill);
        }

        private static void MinigunFire_FixedUpdate(On.EntityStates.ClayBruiser.Weapon.MinigunFire.orig_FixedUpdate orig, EntityStates.ClayBruiser.Weapon.MinigunFire self)
        {
            if (self.characterBody) self.characterBody.outOfCombatStopwatch = 0f;
            orig(self);
        }

        private static void Charge_FixedUpdate(On.EntityStates.Bison.Charge.orig_FixedUpdate orig, EntityStates.Bison.Charge self)
        {
            if (self.characterBody) self.characterBody.outOfCombatStopwatch = 0f;
            orig(self);
        }

        private static void HealthBar_Update(On.RoR2.UI.HealthBar.orig_Update orig, HealthBar self)
        {
            orig(self);

            if (self && self.source)
            {
                if (Hunk.virusObjectiveObjects.Count > 0)
                { 
                    if (self.source.GetComponent<VirusHandler>() || self.source.GetComponent<ParasiteController>())
                    {
                        if (self.eliteBackdropRectTransform)
                        {
                            if (!self.transform.Find("Backdrop,Elite/Backdrop, Infected"))
                            {
                                GameObject infectedBackdrop = GameObject.Instantiate(self.transform.Find("Backdrop,Elite").gameObject, self.transform.Find("Backdrop,Elite"));
                                infectedBackdrop.SetActive(true);
                                infectedBackdrop.name = "Backdrop, Infected";
                                infectedBackdrop.GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 111f / 255f, 184f / 255f);
                                infectedBackdrop.GetComponent<RectTransform>().localScale = new Vector3(0.94f, 0.5f, 1f);

                                self.style = Hunk.instance.infectedHealthBarStyle;

                                self.eliteBackdropRectTransform = null;
                                self.transform.Find("Backdrop,Elite").gameObject.SetActive(true);
                                self.transform.Find("Backdrop,Elite/Arrow,EliteBackdrop").gameObject.SetActive(true);

                                MonoBehaviour.Destroy(self.GetComponent<LevelText>());
                                var it = self.gameObject.AddComponent<InfectionText>();
                                it.target = self.source.body;
                                it.text = self.transform.Find("LevelRoot/ValueText").GetComponent<HGTextMeshProUGUI>();
                                self.transform.Find("LevelRoot").gameObject.SetActive(true);
                                self.transform.Find("LevelRoot").GetComponent<RectTransform>().localPosition = new Vector3(-78f, -2f, 0f);
                            }
                        }
                    }
                }

                if (Hunk.virusObjectiveObjects2.Count > 0)
                {
                    if (self.source.GetComponent<TVirusHandler>())
                    {
                        if (self.eliteBackdropRectTransform)
                        {
                            if (!self.transform.Find("Backdrop,Elite/Backdrop, Infected"))
                            {
                                GameObject infectedBackdrop = GameObject.Instantiate(self.transform.Find("Backdrop,Elite").gameObject, self.transform.Find("Backdrop,Elite"));
                                infectedBackdrop.SetActive(true);
                                infectedBackdrop.name = "Backdrop, Infected";
                                infectedBackdrop.GetComponent<UnityEngine.UI.Image>().color = new Color(130f / 255f, 171f / 255f, 1f);
                                infectedBackdrop.GetComponent<RectTransform>().localScale = new Vector3(0.94f, 0.5f, 1f);

                                self.style = Hunk.instance.tInfectedHealthBarStyle;

                                self.eliteBackdropRectTransform = null;
                                self.transform.Find("Backdrop,Elite").gameObject.SetActive(true);
                                self.transform.Find("Backdrop,Elite/Arrow,EliteBackdrop").gameObject.SetActive(true);

                                //MonoBehaviour.Destroy(self.GetComponent<LevelText>());
                                //var it = self.gameObject.AddComponent<InfectionText>();
                                //it.target = self.source.body;
                                //it.text = self.transform.Find("LevelRoot/ValueText").GetComponent<HGTextMeshProUGUI>();
                                //self.transform.Find("LevelRoot").gameObject.SetActive(true);
                                //self.transform.Find("LevelRoot").GetComponent<RectTransform>().localPosition = new Vector3(-78f, -2f, 0f);
                            }
                        }
                    }
                }
            }
        }

        private static void BaseMainMenuScreen_Awake(On.RoR2.UI.MainMenu.BaseMainMenuScreen.orig_Awake orig, RoR2.UI.MainMenu.BaseMainMenuScreen self)
        {
            if (self) Util.PlaySound("sfx_hunk_retheme_global", self.gameObject);
            orig(self);
        }

        private static void Phase4_OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase4.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase4 self)
        {
            foreach (HunkController i in MonoBehaviour.FindObjectsOfType<HunkController>())
            {
                if (i)
                {
                    i.TrySpawnRocketLauncher();
                }
            }

            orig(self);
        }

        private static void ObjectivePanelController_GetObjectiveSources(On.RoR2.UI.ObjectivePanelController.orig_GetObjectiveSources orig, ObjectivePanelController self, CharacterMaster master, List<ObjectivePanelController.ObjectiveSourceDescriptor> output)
        {
            orig(self, master, output);

            if (master.bodyPrefab == characterPrefab)
            {
                HunkWeaponTracker hunk = master.GetComponent<HunkWeaponTracker>();
                if (hunk)
                {
                    if (Hunk.virusObjectiveObjects.Count > 0)
                    {
                        output.Add(new ObjectivePanelController.ObjectiveSourceDescriptor
                        {
                            source = master,
                            master = master,
                            objectiveType = typeof(Modules.Objectives.KillVirus)
                        });
                    }

                    if (Hunk.virusObjectiveObjects2.Count > 0)
                    {
                        output.Add(new ObjectivePanelController.ObjectiveSourceDescriptor
                        {
                            source = master,
                            master = master,
                            objectiveType = typeof(Modules.Objectives.KillTVirus)
                        });
                    }

                    if (master.inventory.GetItemCount(Hunk.gVirusSample) > 0)
                    {
                        output.Add(new ObjectivePanelController.ObjectiveSourceDescriptor
                        {
                            source = master,
                            master = master,
                            objectiveType = typeof(Modules.Objectives.TurnInSample)
                        });
                    }

                    if (master.inventory.GetItemCount(Hunk.tVirusSample) > 0)
                    {
                        output.Add(new ObjectivePanelController.ObjectiveSourceDescriptor
                        {
                            source = master,
                            master = master,
                            objectiveType = typeof(Modules.Objectives.TurnInTSample)
                        });
                    }
                }
            }
        }

        private static void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            orig(self, activator);

            if (self && self.gameObject.name.Contains("HunkTerminal"))
            {
                if (activator)
                {
                    CharacterBody characterBody = activator.GetComponent<CharacterBody>();
                    if (characterBody)
                    {
                        if (characterBody.inventory)
                        {
                            bool valid = false;
                            int tries = 0;
                            while (!valid)
                            {
                                tries++;
                                if (characterBody.inventory.GetItemCount(Hunk.spadeKeycard) <= 0)
                                {
                                    self.GetComponent<Terminal>().itemDef = Hunk.spadeKeycard;
                                    valid = true;
                                }
                                else
                                {
                                    float rng = Random.Range(0, 3);
                                    switch (rng)
                                    {
                                        case 0:
                                            if (characterBody.inventory.GetItemCount(Hunk.clubKeycard) <= 0)
                                            {
                                                self.GetComponent<Terminal>().itemDef = Hunk.clubKeycard;
                                                valid = true;
                                            }
                                            break;
                                        case 1:
                                            if (characterBody.inventory.GetItemCount(Hunk.heartKeycard) <= 0)
                                            {
                                                self.GetComponent<Terminal>().itemDef = Hunk.heartKeycard;
                                                valid = true;
                                            }
                                            break;
                                        case 2:
                                            if (characterBody.inventory.GetItemCount(Hunk.diamondKeycard) <= 0)
                                            {
                                                self.GetComponent<Terminal>().itemDef = Hunk.diamondKeycard;
                                                valid = true;
                                            }
                                            break;
                                    }
                                }

                                if (tries >= 50)
                                {
                                    valid = true;
                                    if (characterBody.inventory.GetItemCount(Hunk.wristband) > 0)
                                    {
                                        if (characterBody.inventory.GetItemCount(Hunk.masterKeycard) > 0) self.GetComponent<Terminal>().itemDef = RoR2Content.Items.Pearl;
                                        else self.GetComponent<Terminal>().itemDef = Hunk.masterKeycard;
                                    }
                                    else self.GetComponent<Terminal>().itemDef = Hunk.wristband;
                                }
                            }

                            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "goldshores") self.GetComponent<Terminal>().itemDef = Hunk.goldKeycard;

                            if (Modules.Config.depositKeycards.Value)
                            {
                                characterBody.inventory.GiveItem(self.GetComponent<Terminal>().itemDef);
                            }
                        }
                    }
                }
            }
        }

        private static void ChestBehavior_Roll(On.RoR2.ChestBehavior.orig_Roll orig, ChestBehavior self)
        {
            if (self && self.gameObject.name.Contains("HunkTerminal")) return;
            orig(self);
        }

        private static void ChestBehavior_Open(On.RoR2.ChestBehavior.orig_Open orig, ChestBehavior self)
        {
            if (Modules.Helpers.isHunkInPlay)
            {
                if (self.gameObject.name.Contains("HunkChest2"))
                {
                    //Util.PlaySound("sfx_hunk_weapon_case_open", self.gameObject);
                }

                else if (self.gameObject.name.Contains("HunkChest") && !self.gameObject.name.Contains("2")) // bro wtf is this code seriously?
                {
                    Util.PlaySound("sfx_hunk_keycard_accepted", self.gameObject);
                }
            }

            orig(self);
        }

        private static void Inventory_ShrineRestackInventory(On.RoR2.Inventory.orig_ShrineRestackInventory orig, Inventory self, Xoroshiro128Plus rng)
        {
            HunkWeaponTracker hunk = self.GetComponent<HunkWeaponTracker>();
            if (hunk)
            {
                foreach (HunkWeaponData i in hunk.weaponData)
                {
                    self.RemoveItem(i.weaponDef.itemDef);
                }
            }

            orig(self, rng);

            if (hunk)
            {
                foreach (HunkWeaponData i in hunk.weaponData)
                {
                    self.GiveItem(i.weaponDef.itemDef);
                }
            }
        }

        private static void RouletteChestController_EjectPickupServer(On.RoR2.RouletteChestController.orig_EjectPickupServer orig, RouletteChestController self, PickupIndex pickupIndex)
        {
            orig(self, pickupIndex);
        }

        private static void ShopTerminalBehavior_DropPickup(On.RoR2.ShopTerminalBehavior.orig_DropPickup orig, ShopTerminalBehavior self)
        {
            if (Modules.Helpers.isHunkInPlay && !self.gameObject.name.Contains("uplica"))
            {
                Helpers.CreateAmmoPickup(self.transform.position, self.transform.rotation);
            }

            orig(self);
        }

        private static void BaseMainMenuScreen_Update(On.RoR2.UI.MainMenu.BaseMainMenuScreen.orig_Update orig, RoR2.UI.MainMenu.BaseMainMenuScreen self)
        {
            orig(self);
            Transform buttonPanel = self.transform.Find("SafeZone/GenericMenuButtonPanel/ModPanel(Clone)");
            if (buttonPanel) buttonPanel.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));

            buttonPanel = self.transform.Find("SafeZone/GenericMenuButtonPanel/JuicePanel/ModPanel(Clone)");
            if (buttonPanel) buttonPanel.gameObject.SetActive(false);
        }

        private static void Row_AddButton(On.RoR2.UI.LoadoutPanelController.Row.orig_AddButton orig, object self, LoadoutPanelController owner, Sprite icon, string titleToken, string bodyToken, Color tooltipColor, UnityEngine.Events.UnityAction callback, string unlockableName, ViewablesCatalog.Node viewableNode, bool isWIP)
        {
            if (unlockableName == earlySupporterUnlockableDef.nameToken)
            {
                bool unlocked = LocalUserManager.readOnlyLocalUsersList.Any((LocalUser localUser) => localUser.userProfile.HasUnlockable(earlySupporterUnlockableDef));
                if (!unlocked) return;
            }
            orig(self, owner, icon, titleToken, bodyToken, tooltipColor, callback, unlockableName, viewableNode, isWIP);
        }

        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if ((damageInfo.damageType & DamageType.ApplyMercExpose) > DamageType.Generic)
            {
                if (damageInfo.attacker && damageInfo.attacker.name.Contains("RobHunkBody"))
                {
                    if (self)
                    {
                        if (self.body)
                        {
                            self.body.AddTimedBuff(Hunk.mangledBuff, 15f);

                            int count = self.body.GetBuffCount(Hunk.mangledBuff);
                            if (count >= 6)
                            {
                                self.body.ClearTimedBuffs(Hunk.mangledBuff.buffIndex);

                                // kaboom
                                EffectManager.SpawnEffect(Modules.Assets.mangledExplosionEffect, new EffectData
                                {
                                    origin = self.body.corePosition,
                                    rotation = Quaternion.identity,
                                    scale = 1f
                                }, true);

                                self.TakeDamage(new DamageInfo
                                {
                                    attacker = damageInfo.attacker,
                                    canRejectForce = false,
                                    crit = false,
                                    damage = damageInfo.attacker.GetComponent<CharacterBody>().damage * 28f,
                                    damageColorIndex = DamageColorIndex.DeathMark,
                                    damageType = DamageType.Stun1s,
                                    force = Vector3.zero,
                                    inflictor = damageInfo.attacker,
                                    position = self.body.corePosition,
                                    procChainMask = default(ProcChainMask),
                                    procCoefficient = 0f
                                });
                            }
                        }
                    }
                }
            }

            if ((damageInfo.damageType & DamageType.ClayGoo) > DamageType.Generic)
            {
                if (damageInfo.attacker && damageInfo.attacker.name.Contains("RobHunkBody"))
                {
                    if ((damageInfo.damageType & DamageType.Stun1s) > DamageType.Generic) damageInfo.damageType = DamageType.Stun1s;
                    else damageInfo.damageType = DamageType.Generic;

                    if (self)
                    {
                        if (self.body) self.body.gameObject.AddComponent<HunkKnifeTracker>();
                    }
                }
            }

            orig(self, damageInfo);
        }

        private static void SkillLocator_ApplyAmmoPack(On.RoR2.SkillLocator.orig_ApplyAmmoPack orig, SkillLocator self)
        {
            orig(self);

            if (self && self.secondary && self.secondary.baseSkill && self.secondary.baseSkill.skillNameToken == MainPlugin.developerPrefix + "_HUNK_BODY_SECONDARY_AIM_NAME")
            {
                HunkController hunk = self.GetComponent<HunkController>();
                if (hunk)
                {
                    hunk.ApplyBandolier();
                }
            }
        }

        private static void BarrelInteraction_CoinDrop(On.RoR2.BarrelInteraction.orig_CoinDrop orig, BarrelInteraction self)
        {
            if (Modules.Helpers.isHunkInPlay)
            {
                GameObject pickup = GameObject.Instantiate(Hunk.instance.ammoPickupInteractableSmall, self.transform.position, self.transform.rotation);
                NetworkServer.Spawn(pickup); // barrel opening is server-sided
            }

            orig(self);
        }

        private static void ChestBehavior_ItemDrop(On.RoR2.ChestBehavior.orig_ItemDrop orig, ChestBehavior self)
        {
            if (Modules.Helpers.isHunkInPlay)
            {
                if (self.gameObject.name.Contains("HunkChest2"))
                {
                    // this is the worst place to put this btw

                    self.GetComponent<WeaponChest>().gunPickup.enabled = true;
                    self.GetComponent<WeaponChest>().gunPickup.GetComponent<GenericPickupController>().enabled = true;
                    //self.GetComponent<Highlight>().targetRenderer.transform.parent.parent.GetComponent<Animator>().Play("Open");
                    Util.PlaySound("sfx_hunk_weapon_case_open", self.gameObject);

                    NetworkIdentity identity = self.GetComponent<NetworkIdentity>();
                    if (identity) new SyncWeaponCaseOpen(identity.netId, self.gameObject).Send(NetworkDestination.Clients);

                    if (RoR2Application.isInMultiPlayer || MainPlugin.qolChestsInstalled || MainPlugin.emptyChestsInstalled)
                    {
                        PickupDropletController.CreatePickupDroplet(
                            PickupCatalog.FindPickupIndex(self.GetComponent<WeaponChest>().itemDef.itemIndex),
                            self.transform.position + Vector3.up,
                            Vector3.up * 25f);
                    }

                    return;
                }

                if (self.gameObject.name.Contains("HunkChest"))
                {
                    self.GetComponent<WeaponChest>().gunPickup.enabled = true;
                    self.GetComponent<WeaponChest>().gunPickup.GetComponent<GenericPickupController>().enabled = true;
                    //self.GetComponent<Highlight>().targetRenderer.transform.parent.parent.parent.parent.GetComponent<Animator>().Play("Open");
                    Util.PlaySound("sfx_hunk_weapon_case_open", self.gameObject);

                    NetworkIdentity identity = self.GetComponent<NetworkIdentity>();
                    if (identity) new SyncWeaponCaseOpen(identity.netId, self.gameObject).Send(NetworkDestination.Clients);

                    if (RoR2Application.isInMultiPlayer || MainPlugin.qolChestsInstalled || MainPlugin.emptyChestsInstalled)
                    {
                        PickupDropletController.CreatePickupDroplet(
                            PickupCatalog.FindPickupIndex(self.GetComponent<WeaponChest>().itemDef.itemIndex),
                            self.transform.position + Vector3.up,
                            Vector3.up * 25f);
                    }

                    return;
                }

                if (!self.gameObject.name.Contains("Hunk") && !self.gameObject.name.Contains("uplica"))
                {
                    Helpers.CreateAmmoPickup(self.transform.position, self.transform.rotation);

                    if (self.tier3Chance >= 0.2f)
                    {
                        Helpers.CreateAmmoPickup(self.transform.position, self.transform.rotation);
                    }

                    if (self.tier3Chance >= 1f)
                    {
                        Helpers.CreateAmmoPickup(self.transform.position, self.transform.rotation);
                    }
                }

                if (self.gameObject.name.Contains("HunkTerminal"))
                {
                    self.GetComponent<PurchaseInteraction>().SetAvailable(true);

                    if (Modules.Config.depositKeycards.Value)
                    {

                    }
                    else
                    {
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(self.GetComponent<Terminal>().itemDef.itemIndex), self.transform.position + (Vector3.up * 0.85f), (self.transform.forward * 3f) + Vector3.up * 10f);
                    }

                    return;
                }
            }

            orig(self);

        }

        private static void LoadoutPanelController_Rebuild(On.RoR2.UI.LoadoutPanelController.orig_Rebuild orig, LoadoutPanelController self)
        {
            orig(self);

            // i am sorry
            if (self.currentDisplayData.bodyIndex == BodyCatalog.FindBodyIndex("RobHunkBody"))
            {
                int j = 0;
                foreach (LanguageTextMeshController i in self.gameObject.GetComponentsInChildren<LanguageTextMeshController>())
                {
                    if (i && i.token == "LOADOUT_SKILL_MISC")
                    {
                        if (j <= 0) i.token = "Passive";
                        else if (j == 1) i.token = "Knife";
                        else i.token = "Aspect";
                        j++;
                    }
                }
            }
        }

        private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            if (damageReport.attackerBody && damageReport.attackerMaster && damageReport.victim)
            {
                // ammo drops
                if (Modules.Helpers.isHunkInPlay)
                {
                    bool isKnifeKill = false;
                    // headshot first
                    if (damageReport.attackerBody.baseNameToken == Hunk.bodyNameToken)
                    {
                        if (damageReport.victim.GetComponent<HunkHeadshotTracker>())
                        {
                            NetworkIdentity identity = damageReport.victim.gameObject.GetComponent<NetworkIdentity>();
                            if (identity)
                            {
                                new SyncDecapitation(identity.netId, damageReport.victim.gameObject).Send(NetworkDestination.Clients);
                            }
                        }

                        if (damageReport.victim.GetComponent<TemplarExplosionTracker>())
                        {
                            NetworkIdentity identity = damageReport.victim.gameObject.GetComponent<NetworkIdentity>();
                            if (identity)
                            {
                                new SyncTemplarExplosion(identity.netId, damageReport.victim.gameObject).Send(NetworkDestination.Clients);
                            }
                        }

                        if (damageReport.victim.GetComponent<HunkKnifeTracker>())
                        {
                            isKnifeKill = true;
                        }
                    }

                    // 4
                    float chance = Modules.Config.baseDropRate.Value;
                    bool fuckMyAss = chance >= 100f;

                    // higher chance if it's a big guy
                    if (damageReport.victimBody.hullClassification == HullClassification.Golem) chance = Mathf.Clamp(1.1f * chance, 0f, 100f);

                    // minimum 25% chance if the slain enemy is an elite
                    if (damageReport.victimBody.isElite) chance = Mathf.Clamp(chance, 25f, 100f);

                    // halved on swarms, fuck You
                    if (Run.instance && RoR2.RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.Swarms)) chance *= 0.5f;

                    // double drop rate on sacrifice
                    if (Run.instance && RoR2.RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.Sacrifice) && isKnifeKill) chance *= 2f;

                    chance *= Hunk.instance.pityMultiplier;

                    bool dropped = Util.CheckRoll(chance, damageReport.attackerMaster);

                    // guaranteed if the slain enemy is a boss
                    bool isBoss = damageReport.victimBody.isChampion || damageReport.victimIsChampion;

                    // simulacrum boss wave fix
                    if ((damageReport.victimBody.isBoss || damageReport.victimIsBoss) && !InfiniteTowerRun.instance) isBoss = true;

                    // stop dropping ammo when void monsters kill each other plz this is an annoying bug
                    if (damageReport.attackerTeamIndex != TeamIndex.Player) dropped = false;

                    // only drop on sacrifice- otherwise he must rummage or kill with knife
                    if (Run.instance && !RoR2.RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.Sacrifice) && !isKnifeKill) dropped = false;

                    if (isBoss || fuckMyAss) dropped = true;

                    if (dropped)
                    {
                        Hunk.instance.pityMultiplier = 0.7f;

                        Vector3 position = Vector3.zero;
                        Transform transform = damageReport.victim.transform;
                        if (transform)
                        {
                            position = damageReport.victim.transform.position;
                        }

                        GameObject ammoPickup = UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.ammoPickup, position, UnityEngine.Random.rotation);

                        //TeamFilter teamFilter = ammoPickup.GetComponent<TeamFilter>();
                        //if (teamFilter) teamFilter.teamIndex = damageReport.attackerTeamIndex;

                        NetworkServer.Spawn(ammoPickup);
                    }
                    else
                    {
                        // add pity
                        Hunk.instance.pityMultiplier += 0.025f;
                    }
                }
            }
        }

        internal static void HUDSetup(RoR2.UI.HUD hud)
        {
            if (hud.targetBodyObject && hud.targetMaster && hud.targetMaster.bodyPrefab == Hunk.characterPrefab)
            {
                if (!hud.targetMaster.hasAuthority) return;

                if (MainPlugin.riskUIInstalled)
                {
                    RiskUIHudSetup(hud);
                    return;
                }

                Transform skillsContainer = hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomRightCluster").Find("Scaler");

                if (skillsContainer && skillsContainer.Find("SprintCluster") && skillsContainer.Find("SprintCluster").gameObject.activeSelf)
                {
                    // no one will notice these missing
                    skillsContainer.Find("SprintCluster").gameObject.SetActive(false);
                    skillsContainer.Find("SprintCluster").gameObject.name = "GTFO";
                    skillsContainer.Find("InventoryCluster").gameObject.SetActive(false);

                    if (Modules.Config.showWeaponIcon.Value && !Modules.Config.customHUD.Value)
                    {
                        GameObject weaponSlot = GameObject.Instantiate(skillsContainer.Find("EquipmentSlot").gameObject, skillsContainer);
                        weaponSlot.name = "WeaponSlot";

                        EquipmentIcon equipmentIconComponent = weaponSlot.GetComponent<EquipmentIcon>();
                        Components.WeaponIcon weaponIconComponent = weaponSlot.AddComponent<Components.WeaponIcon>();

                        weaponIconComponent.iconImage = equipmentIconComponent.iconImage;
                        weaponIconComponent.displayRoot = equipmentIconComponent.displayRoot;
                        weaponIconComponent.flashPanelObject = equipmentIconComponent.stockFlashPanelObject;
                        weaponIconComponent.reminderFlashPanelObject = equipmentIconComponent.reminderFlashPanelObject;
                        weaponIconComponent.isReadyPanelObject = equipmentIconComponent.isReadyPanelObject;
                        weaponIconComponent.tooltipProvider = equipmentIconComponent.tooltipProvider;
                        weaponIconComponent.targetHUD = hud;

                        weaponSlot.GetComponent<RectTransform>().anchoredPosition = new Vector2(-480f, -17.1797f);

                        HGTextMeshProUGUI keyText = weaponSlot.transform.Find("DisplayRoot").Find("EquipmentTextBackgroundPanel").Find("EquipmentKeyText").gameObject.GetComponent<HGTextMeshProUGUI>();
                        keyText.gameObject.GetComponent<InputBindingDisplayController>().enabled = false;
                        keyText.text = "Weapon";

                        weaponSlot.transform.Find("DisplayRoot").Find("EquipmentStack").gameObject.SetActive(false);
                        weaponSlot.transform.Find("DisplayRoot").Find("CooldownText").gameObject.SetActive(false);

                        MonoBehaviour.DestroyImmediate(equipmentIconComponent);
                    }

                    // weapon pickup notification

                    /*GameObject notificationPanel = GameObject.Instantiate(hud.transform.Find("MainContainer").Find("NotificationArea").gameObject);
                    notificationPanel.transform.SetParent(hud.transform.Find("MainContainer"), true);
                    notificationPanel.GetComponent<RectTransform>().localPosition = new Vector3(0f, -265f, -150f);
                    notificationPanel.transform.localScale = Vector3.one;

                    NotificationUIController _old = notificationPanel.GetComponent<NotificationUIController>();
                    WeaponNotificationUIController _new = notificationPanel.AddComponent<WeaponNotificationUIController>();

                    _new.hud = _old.hud;
                    _new.genericNotificationPrefab = Modules.Assets.weaponNotificationPrefab;
                    _new.notificationQueue = hud.targetMaster.gameObject.AddComponent<WeaponNotificationQueue>();

                    _old.enabled = false;*/



                    // ammo display

                    Transform healthbarContainer = hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster").Find("BarRoots").Find("LevelDisplayCluster");

                    if (!hud.transform.Find("MainContainer/MainUIArea/CrosshairCanvas/CrosshairExtras/AmmoTracker"))
                    {
                        if (Modules.Config.fancyAmmoDisplay.Value)
                        {
                            GameObject ammoTracker = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("AmmoPanel"), hud.transform.Find("MainContainer/MainUIArea/CrosshairCanvas/CrosshairExtras"));
                            ammoTracker.name = "AmmoDisplay";
                            ammoTracker.transform.SetParent(hud.transform.Find("MainContainer/MainUIArea/CrosshairCanvas/CrosshairExtras"));

                            AmmoDisplay2 ammoTrackerComponent = ammoTracker.AddComponent<AmmoDisplay2>();
                            ammoTrackerComponent.targetHUD = hud;
                            ammoTrackerComponent.currentText = ammoTracker.transform.Find("Current").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
                            ammoTrackerComponent.totalText = ammoTracker.transform.Find("Total").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
                            ammoTrackerComponent.bonusText = ammoTracker.transform.Find("Bonus").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
                            ammoTrackerComponent.fontOverride = Modules.Assets.hgFont;

                            RectTransform rect = ammoTracker.GetComponent<RectTransform>();
                            rect.localScale = new Vector3(1f, 1f, 1f);
                            rect.anchorMin = new Vector2(0f, 0f);
                            rect.anchorMax = new Vector2(0f, 0f);
                            rect.pivot = new Vector2(0.5f, 0f);
                            rect.anchoredPosition = new Vector2(50f, 0f);
                            rect.localPosition = new Vector3(100f, -150f, 0f);
                        }
                        else
                        {
                            GameObject ammoTracker = GameObject.Instantiate(healthbarContainer.gameObject, hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster"));
                            ammoTracker.name = "AmmoTracker";
                            ammoTracker.transform.SetParent(hud.transform.Find("MainContainer/MainUIArea/CrosshairCanvas/CrosshairExtras"));

                            GameObject.DestroyImmediate(ammoTracker.transform.GetChild(0).gameObject);
                            MonoBehaviour.Destroy(ammoTracker.GetComponentInChildren<LevelText>());
                            MonoBehaviour.Destroy(ammoTracker.GetComponentInChildren<ExpBar>());

                            AmmoDisplay ammoTrackerComponent = ammoTracker.AddComponent<AmmoDisplay>();
                            ammoTrackerComponent.targetHUD = hud;
                            ammoTrackerComponent.targetText = ammoTracker.transform.Find("LevelDisplayRoot").Find("PrefixText").gameObject.GetComponent<LanguageTextMeshController>();

                            ammoTracker.transform.Find("LevelDisplayRoot").Find("ValueText").gameObject.SetActive(false);

                            //ammoTracker.transform.Find("ExpBarRoot").GetChild(0).GetComponent<Image>().enabled = true;

                            ammoTracker.transform.Find("LevelDisplayRoot").GetComponent<RectTransform>().anchoredPosition = new Vector2(-12f, 0f);

                            RectTransform rect = ammoTracker.GetComponent<RectTransform>();
                            rect.localScale = new Vector3(0.8f, 0.8f, 1f);
                            rect.anchorMin = new Vector2(0f, 0f);
                            rect.anchorMax = new Vector2(0f, 0f);
                            rect.pivot = new Vector2(0.5f, 0f);
                            rect.anchoredPosition = new Vector2(50f, 0f);
                            rect.localPosition = new Vector3(50f, -95f, 0f);
                        }
                    }

                    // generic notification

                    if (!hud.transform.Find("MainContainer/MainUIArea/CrosshairCanvas/CrosshairExtras/NotificationPanel"))
                    {
                        GameObject notificationObject = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("GenericTextPanel"), healthbarContainer);
                        notificationObject.name = "NotificationPanel";
                        notificationObject.transform.SetParent(hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").Find("CrosshairExtras"));

                        HunkNotificationHandler notificationHandler = notificationObject.AddComponent<HunkNotificationHandler>();
                        notificationHandler.targetHUD = hud;

                        RectTransform rect = notificationObject.GetComponent<RectTransform>();
                        rect.localScale = new Vector3(1f, 1f, 1f);
                        rect.anchorMin = new Vector2(0f, 0f);
                        rect.anchorMax = new Vector2(0f, 0f);
                        rect.pivot = new Vector2(0f, 0f);
                        rect.anchoredPosition = new Vector2(50f, 0f);
                        rect.localPosition = new Vector3(0f, -350f, 0f);
                    }

                    // railgun charge bar
                    GameObject railgunChargeBar = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("StaminaBar"), healthbarContainer);
                    railgunChargeBar.name = "RailgunChargeBar";
                    railgunChargeBar.transform.SetParent(hud.transform.Find("MainContainer/MainUIArea/CrosshairCanvas/CrosshairExtras"));
                    railgunChargeBar.AddComponent<RailgunChargeBar>().targetHUD = hud;

                    RectTransform rrect = railgunChargeBar.GetComponent<RectTransform>();
                    rrect.localScale = new Vector3(1f, 1f, 1f);
                    rrect.anchorMin = new Vector2(0f, 0f);
                    rrect.anchorMax = new Vector2(0f, 0f);
                    rrect.offsetMin = new Vector2(-150f, 0f);
                    rrect.offsetMax = new Vector2(150f, 0f);
                    rrect.pivot = new Vector2(0f, 0f);
                    rrect.anchoredPosition = new Vector2(00f, 0f);
                    rrect.localPosition = new Vector3(-50f, -50f, 0f);
                    rrect.localRotation = Quaternion.identity;
                    rrect.sizeDelta = new Vector2(100f, 10f);

                    // custom hud
                    if (Modules.Config.customHUD.Value)
                    {
                        // add component to display this only when active
                        //hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/TopCenterCluster/ItemInventoryDisplayRoot").gameObject.SetActive(false);

                        // hide skills
                        hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/BottomRightCluster/Scaler/Skill1Root").gameObject.SetActive(false);
                        hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/BottomRightCluster/Scaler/Skill2Root").gameObject.SetActive(false);
                        hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/BottomRightCluster/Scaler/Skill3Root").gameObject.SetActive(false);
                        hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/BottomRightCluster/Scaler/Skill4Root").gameObject.SetActive(false);
                        hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/BottomRightCluster/Scaler/EquipmentSlot").GetComponent<RectTransform>().localPosition = new Vector3(-160f, 170f, 0f);

                        hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/BottomRightCluster/Scaler/EquipmentSlot/DisplayRoot/BGPanel").gameObject.SetActive(false);
                        hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/BottomRightCluster/Scaler/EquipmentSlot/DisplayRoot/EquipmentTextBackgroundPanel").gameObject.SetActive(false);

                        // hide health bar
                        hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/BottomLeftCluster/BarRoots").gameObject.SetActive(false);

                        // hide money
                        //hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/UpperLeftCluster").gameObject.SetActive(false);
                        //GameObject.Destroy(hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/UpperLeftCluster/MoneyRoot").gameObject);
                        //GameObject.Destroy(hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/UpperLeftCluster/LunarCoinRoot").gameObject);

                        // dodge stamina bar
                        GameObject staminaBar = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("StaminaBar"), healthbarContainer);
                        staminaBar.name = "StaminaBar";
                        staminaBar.transform.SetParent(hud.transform.Find("MainContainer/MainUIArea/CrosshairCanvas/CrosshairExtras"));
                        staminaBar.AddComponent<StaminaBar>().targetHUD = hud;

                        //HunkNotificationHandler notificationHandler = staminaBar.AddComponent<HunkNotificationHandler>();
                        //notificationHandler.targetHUD = hud;

                        RectTransform rect = staminaBar.GetComponent<RectTransform>();
                        rect.localScale = new Vector3(1f, 1f, 1f);
                        rect.anchorMin = new Vector2(0f, 0f);
                        rect.anchorMax = new Vector2(0f, 0f);
                        rect.offsetMin = new Vector2(-150f, 0f);
                        rect.offsetMax = new Vector2(150f, 0f);
                        rect.pivot = new Vector2(0f, 0f);
                        rect.anchoredPosition = new Vector2(00f, 0f);
                        rect.localPosition = new Vector3(-150f, -350f, 0f);
                        rect.localRotation = Quaternion.identity;
                        rect.sizeDelta = new Vector2(300f, 10f);

                        // custom health bar
                        GameObject newHealthBar = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("CustomHealthBar"), healthbarContainer);
                        newHealthBar.name = "CustomHealthBar";
                        newHealthBar.transform.SetParent(hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/BottomRightCluster"));
                        newHealthBar.AddComponent<CustomHealthBar>().targetHUD = hud;

                        //HunkNotificationHandler notificationHandler = staminaBar.AddComponent<HunkNotificationHandler>();
                        //notificationHandler.targetHUD = hud;

                        rect = newHealthBar.GetComponent<RectTransform>();
                        rect.localScale = new Vector3(1f, 1f, 1f);
                        rect.anchorMin = new Vector2(0f, 0f);
                        rect.anchorMax = new Vector2(0f, 0f);
                        rect.pivot = new Vector2(0f, 0f);
                        rect.anchoredPosition = new Vector2(0f, 0f);
                        rect.localPosition = new Vector3(-300f, -100f, 0f);
                        rect.localRotation = Quaternion.identity;
                    }
                }
            }
        }

        internal static void RiskUIHudSetup(RoR2.UI.HUD hud)
        {
            Transform skillsContainer = hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomRightCluster").Find("Scaler");

            if (!skillsContainer.Find("WeaponSlot"))
            {
                if (Modules.Config.showWeaponIcon.Value && !Modules.Config.customHUD.Value)
                {
                    GameObject weaponSlot = GameObject.Instantiate(skillsContainer.Find("EquipmentSlotPos1").Find("EquipIcon").gameObject, skillsContainer);
                    weaponSlot.name = "WeaponSlot";

                    EquipmentIcon equipmentIconComponent = weaponSlot.GetComponent<EquipmentIcon>();
                    Components.WeaponIcon weaponIconComponent = weaponSlot.AddComponent<Components.WeaponIcon>();

                    weaponIconComponent.iconImage = equipmentIconComponent.iconImage;
                    weaponIconComponent.displayRoot = equipmentIconComponent.displayRoot;
                    weaponIconComponent.flashPanelObject = equipmentIconComponent.stockFlashPanelObject;
                    weaponIconComponent.reminderFlashPanelObject = equipmentIconComponent.reminderFlashPanelObject;
                    weaponIconComponent.isReadyPanelObject = equipmentIconComponent.isReadyPanelObject;
                    weaponIconComponent.tooltipProvider = equipmentIconComponent.tooltipProvider;
                    weaponIconComponent.targetHUD = hud;

                    MaterialHud.MaterialEquipmentIcon x = weaponSlot.GetComponent<MaterialHud.MaterialEquipmentIcon>();
                    Components.MaterialWeaponIcon y = weaponSlot.AddComponent<Components.MaterialWeaponIcon>();

                    y.icon = weaponIconComponent;
                    y.onCooldown = x.onCooldown;
                    y.mask = x.mask;
                    y.stockText = x.stockText;

                    RectTransform iconRect = weaponSlot.GetComponent<RectTransform>();
                    iconRect.localScale = new Vector3(2f, 2f, 2f);
                    iconRect.anchoredPosition = new Vector2(-128f, 60f);

                    HGTextMeshProUGUI keyText = weaponSlot.transform.Find("DisplayRoot").Find("BottomContainer").Find("SkillBackgroundPanel").Find("SkillKeyText").gameObject.GetComponent<HGTextMeshProUGUI>();
                    keyText.gameObject.GetComponent<InputBindingDisplayController>().enabled = false;
                    keyText.text = "Weapon";

                    weaponSlot.transform.Find("DisplayRoot").Find("BottomContainer").Find("StockTextContainer").gameObject.SetActive(false);
                    weaponSlot.transform.Find("DisplayRoot").Find("CooldownText").gameObject.SetActive(false);

                    // duration bar
                    /*GameObject chargeBar = GameObject.Instantiate(Assets.mainAssetBundle.LoadAsset<GameObject>("WeaponChargeBar"));
                    chargeBar.transform.SetParent(weaponSlot.transform.Find("DisplayRoot"));

                    RectTransform rect = chargeBar.GetComponent<RectTransform>();

                    rect.localScale = new Vector3(0.75f, 0.1f, 1f);
                    rect.anchorMin = new Vector2(0f, 0f);
                    rect.anchorMax = new Vector2(0f, 0f);
                    rect.pivot = new Vector2(0.5f, 0f);
                    rect.localPosition = new Vector3(0f, 0f, 0f);
                    rect.anchoredPosition = new Vector2(-8f, 36f);
                    rect.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));*/

                    //weaponIconComponent.durationDisplay = chargeBar;
                    //weaponIconComponent.durationBar = chargeBar.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Image>();
                    //weaponIconComponent.durationBarRed = chargeBar.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();

                    MonoBehaviour.Destroy(equipmentIconComponent);
                    MonoBehaviour.Destroy(x);
                }

                // weapon pickup notification

                /*GameObject notificationPanel = GameObject.Instantiate(hud.transform.Find("MainContainer").Find("NotificationArea").gameObject);
                notificationPanel.transform.SetParent(hud.transform.Find("MainContainer"), true);
                notificationPanel.GetComponent<RectTransform>().localPosition = new Vector3(0f, -210f, -50f);
                notificationPanel.transform.localScale = Vector3.one;

                NotificationUIController _old = notificationPanel.GetComponent<NotificationUIController>();
                WeaponNotificationUIController _new = notificationPanel.AddComponent<WeaponNotificationUIController>();

                _new.hud = _old.hud;
                _new.genericNotificationPrefab = Modules.Assets.weaponNotificationPrefab;
                _new.notificationQueue = hud.targetMaster.gameObject.AddComponent<WeaponNotificationQueue>();

                _old.enabled = false;*/


                // ammo display
                Transform mainContainer = hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").Find("CrosshairExtras");

                if (!mainContainer.Find("AmmoDisplay"))
                {
                    GameObject ammoTracker = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("AmmoPanel"), mainContainer);
                    ammoTracker.name = "AmmoDisplay";
                    ammoTracker.transform.SetParent(mainContainer);

                    AmmoDisplay2 ammoTrackerComponent = ammoTracker.AddComponent<AmmoDisplay2>();
                    ammoTrackerComponent.targetHUD = hud;
                    ammoTrackerComponent.currentText = ammoTracker.transform.Find("Current").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
                    ammoTrackerComponent.totalText = ammoTracker.transform.Find("Total").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
                    ammoTrackerComponent.bonusText = ammoTracker.transform.Find("Bonus").gameObject.GetComponent<TMPro.TextMeshProUGUI>();

                    RectTransform rect = ammoTracker.GetComponent<RectTransform>();
                    rect.localScale = new Vector3(1f, 1f, 1f);
                    rect.anchorMin = new Vector2(0f, 0f);
                    rect.anchorMax = new Vector2(0f, 0f);
                    rect.pivot = new Vector2(0.5f, 0f);
                    rect.anchoredPosition = new Vector2(50f, 0f);
                    rect.localPosition = new Vector3(100f, -150f, 0f);
                }

                // generic notification
                if (!mainContainer.Find("NotificationPanel"))
                {
                    GameObject notificationObject = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("GenericTextPanel"), mainContainer);
                    notificationObject.name = "NotificationPanel";
                    notificationObject.transform.SetParent(mainContainer);

                    HunkNotificationHandler notificationHandler = notificationObject.AddComponent<HunkNotificationHandler>();
                    notificationHandler.targetHUD = hud;

                    RectTransform rect = notificationObject.GetComponent<RectTransform>();
                    rect.localScale = new Vector3(1f, 1f, 1f);
                    rect.anchorMin = new Vector2(0f, 0f);
                    rect.anchorMax = new Vector2(0f, 0f);
                    rect.pivot = new Vector2(0f, 0f);
                    rect.anchoredPosition = new Vector2(50f, 0f);
                    rect.localPosition = new Vector3(0f, -350f, 0f);
                }

                // custom hud
                if (Modules.Config.customHUD.Value)
                {
                    // add component to display this only when active
                    //hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/TopCenterCluster/ItemInventoryDisplayRoot").gameObject.SetActive(false);

                    // hide skills
                    hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/BottomRightCluster/Scaler/SkillIconContainer").gameObject.SetActive(false);
                    //hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/BottomRightCluster/Scaler/EquipmentSlot").GetComponent<RectTransform>().localPosition = new Vector3(-160f, 170f, 0f);

                    GameObject equipmentIcon = hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/BottomRightCluster").GetComponentInChildren<EquipmentIcon>().gameObject;
                    equipmentIcon.transform.Find("DisplayRoot/GameObject").gameObject.SetActive(false);
                    equipmentIcon.transform.Find("DisplayRoot/Mask").gameObject.SetActive(false);
                    equipmentIcon.transform.Find("DisplayRoot/BgImage").gameObject.GetComponent<UnityEngine.UI.Image>().enabled = false;
                    equipmentIcon.transform.Find("DisplayRoot/BgImage/IconPanel/OnCooldown").gameObject.GetComponent<UnityEngine.UI.Image>().sprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRadialInner");
                    equipmentIcon.transform.Find("DisplayRoot/BgImage/IconPanel/OnCooldown").gameObject.GetComponent<RectTransform>().localScale = Vector3.one * 1.5f;

                    //hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/BottomRightCluster/Scaler/EquipmentSlot/DisplayRoot/BGPanel").gameObject.SetActive(false);
                    //hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/BottomRightCluster/Scaler/EquipmentSlot/DisplayRoot/EquipmentTextBackgroundPanel").gameObject.SetActive(false);

                    // hide health bar
                    HealthBar fuckMe = null;

                    fuckMe = hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/BottomLeftCluster").gameObject.GetComponentInChildren<HealthBar>();
                    if (fuckMe) fuckMe.transform.parent.gameObject.SetActive(false);

                    fuckMe = hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/BottomCenterCluster").gameObject.GetComponentInChildren<HealthBar>();
                    if (fuckMe) fuckMe.transform.parent.gameObject.SetActive(false);

                    // hide money
                    //hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/UpperLeftCluster").gameObject.SetActive(false);
                    //GameObject.Destroy(hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/UpperLeftCluster/MoneyRoot").gameObject);
                    //GameObject.Destroy(hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/UpperLeftCluster/LunarCoinRoot").gameObject);

                    // railgun charge bar
                    GameObject railgunChargeBar = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("StaminaBar"), mainContainer);
                    railgunChargeBar.name = "RailgunChargeBar";
                    railgunChargeBar.transform.SetParent(hud.transform.Find("MainContainer/MainUIArea/CrosshairCanvas/CrosshairExtras"));
                    railgunChargeBar.AddComponent<RailgunChargeBar>().targetHUD = hud;

                    RectTransform rrect = railgunChargeBar.GetComponent<RectTransform>();
                    rrect.localScale = new Vector3(1f, 1f, 1f);
                    rrect.anchorMin = new Vector2(0f, 0f);
                    rrect.anchorMax = new Vector2(0f, 0f);
                    rrect.offsetMin = new Vector2(-150f, 0f);
                    rrect.offsetMax = new Vector2(150f, 0f);
                    rrect.pivot = new Vector2(0f, 0f);
                    rrect.anchoredPosition = new Vector2(00f, 0f);
                    rrect.localPosition = new Vector3(-50f, -50f, 0f);
                    rrect.localRotation = Quaternion.identity;
                    rrect.sizeDelta = new Vector2(100f, 10f);

                    // dodge stamina bar
                    GameObject staminaBar = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("StaminaBar"), mainContainer);
                    staminaBar.name = "StaminaBar";
                    staminaBar.transform.SetParent(hud.transform.Find("MainContainer/MainUIArea/CrosshairCanvas/CrosshairExtras"));
                    staminaBar.AddComponent<StaminaBar>().targetHUD = hud;

                    //HunkNotificationHandler notificationHandler = staminaBar.AddComponent<HunkNotificationHandler>();
                    //notificationHandler.targetHUD = hud;

                    RectTransform rect = staminaBar.GetComponent<RectTransform>();
                    rect.localScale = new Vector3(1f, 1f, 1f);
                    rect.anchorMin = new Vector2(0f, 0f);
                    rect.anchorMax = new Vector2(0f, 0f);
                    rect.offsetMin = new Vector2(-150f, 0f);
                    rect.offsetMax = new Vector2(150f, 0f);
                    rect.pivot = new Vector2(0f, 0f);
                    rect.anchoredPosition = new Vector2(00f, 0f);
                    rect.localPosition = new Vector3(-150f, -350f, 0f);
                    rect.localRotation = Quaternion.identity;
                    rect.sizeDelta = new Vector2(300f, 10f);

                    // custom health bar
                    GameObject newHealthBar = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("CustomHealthBar"), mainContainer);
                    newHealthBar.name = "CustomHealthBar";
                    newHealthBar.transform.SetParent(hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/BottomRightCluster"));
                    newHealthBar.AddComponent<CustomHealthBar>().targetHUD = hud;

                    //HunkNotificationHandler notificationHandler = staminaBar.AddComponent<HunkNotificationHandler>();
                    //notificationHandler.targetHUD = hud;

                    rect = newHealthBar.GetComponent<RectTransform>();
                    rect.localScale = new Vector3(1f, 1f, 1f);
                    rect.anchorMin = new Vector2(0f, 0f);
                    rect.anchorMax = new Vector2(0f, 0f);
                    rect.pivot = new Vector2(0f, 0f);
                    rect.anchoredPosition = new Vector2(0f, 0f);
                    rect.localPosition = new Vector3(-200f, 40f, 0f);
                    rect.localRotation = Quaternion.identity;

                    newHealthBar.transform.GetChild(0).gameObject.AddComponent<RectMover>().pos = new Vector3(-300f, 80f, 0f);

                    equipmentIcon.transform.parent.SetParent(newHealthBar.transform);
                    equipmentIcon.AddComponent<RectMover>().pos = new Vector3(-180f, 90f, 0f);
                }
            }
        }

        public static void SpawnChests()
        {
            Xoroshiro128Plus rng = new Xoroshiro128Plus(Run.instance.seed);
            DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(chestInteractableCard, new DirectorPlacementRule { placementMode = DirectorPlacementRule.PlacementMode.Random }, rng));
        }

        private static void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            orig(self);

            spawnedWeaponList = new List<ItemDef>();
            spawnedCostTypeList = new List<CostTypeDef>();

            int hunkCount = Helpers.hunkCount;

            if (hunkCount > 0)
            {
                var currStage = SceneManager.GetActiveScene();
                var currStageName = currStage.name;

                var pos = Vector3.zero;
                var rot = Quaternion.Euler(0, 0, 0);
                var pos2 = Vector3.zero;
                var rot2 = Quaternion.Euler(0, 0, 0);

                string[] splitString = stageBlacklist.Split(',');

                blacklistedStageNames = new List<string>(splitString);

                bool doSpawns = true;
                bool validStage = true;

                switch (currStageName)
                {
                    case "blackbeach":
                        pos = new Vector3(-38.10372f, -213.6f, -203.5f);
                        rot = Quaternion.Euler(5.000008f, 250f, 350f);
                        pos2 = new Vector3(72f, -179f, -320f);
                        rot2 = Quaternion.Euler(0, 200f, 0);
                        break;
                    case "blackbeach2":
                        pos = new Vector3(-136.572f, 48f, -98.21206f);
                        rot = Quaternion.Euler(5.000002f, 79.99998f, 9.999999f);
                        pos2 = new Vector3(-37.19264f, -10.1909f, 100.4858f);
                        rot2 = Quaternion.Euler(355f, 39.99999f, 3.07129406f);
                        break;
                    case "golemplains":
                        pos = new Vector3(88.59721f, -133.8395f, 96.43916f);
                        rot = Quaternion.Euler(0, 321, 0);
                        pos2 = new Vector3(-7.914185f, -146f, -260.5133f);
                        rot2 = Quaternion.Euler(0, 150, 0);
                        break;
                    case "golemplains2":
                        pos = new Vector3(157f, 7.7f, -230.336f);
                        rot = Quaternion.Euler(0, 90, 0);
                        pos2 = new Vector3(-60.65578f, 0f, 33.8f);
                        rot2 = Quaternion.Euler(0, 20, 0);
                        break;
                    case "goolake":
                        pos = new Vector3(300.031f, -134.5496f, 172.794f);
                        rot = Quaternion.Euler(6.40330307f, 58f, 355f);
                        pos2 = new Vector3(-9.976514f, -130.503f, 9.452635f);
                        rot2 = Quaternion.Euler(5.000005f, 200f, 352f);
                        break;
                    case "foggyswamp":
                        pos = new Vector3(73.71142f, -149.707f, -242.2973f);
                        rot = Quaternion.Euler(0, 160f, 0);
                        pos2 = new Vector3(-18.07931f, -123.49f, 31.3618f);
                        rot2 = Quaternion.Euler(0, 0, 4f);
                        break;
                    case "frozenwall":
                        pos = new Vector3(72.94351f, 120.4808f, 117.6099f);
                        rot = Quaternion.Euler(0, 167, 0);
                        pos2 = new Vector3(-141.753f, 50.38663f, 13.00832f);
                        rot2 = Quaternion.Euler(2f, 180f, 3f);
                        break;
                    case "wispgraveyard":
                        pos = new Vector3(-383.5073f, 6.731739f, -49.00582f);
                        rot = Quaternion.Euler(0, 265, 0);
                        pos2 = new Vector3(131.7281f, 46.5f, 198.5f);
                        rot2 = Quaternion.Euler(0, 180f, 0);
                        break;
                    case "dampcavesimple":
                        pos = new Vector3(66.61202f, -87.96278f, -202.6679f);
                        rot = Quaternion.Euler(358f, 60f, -5.33933808f);
                        pos2 = new Vector3(-145.8008f, -151.7f, -270.3545f);
                        rot2 = Quaternion.Euler(0, 180f, 15f);
                        break;
                    case "shipgraveyard":
                        pos = new Vector3(-86.49648f, -30.60152f, -51.19278f);
                        rot = Quaternion.Euler(0, 240f, 358f);
                        pos2 = new Vector3(83.60206f, 73.1088f, 188.5042f);
                        rot2 = Quaternion.Euler(1.01777813f, 180f, 345f);
                        break;
                    case "rootjungle":
                        pos = new Vector3(-117.2441f, -54.73182f, -116.5612f);
                        rot = Quaternion.Euler(0, 80, 0);
                        pos2 = new Vector3(-203.3925f, 93.53692f, -166.1386f);
                        rot2 = Quaternion.Euler(0, 0, 0);
                        break;
                    case "skymeadow":
                        pos = new Vector3(157.377f, -62.490295f, -240.5255f);
                        rot = Quaternion.Euler(9f, -4.0519508f, 358f);
                        pos2 = new Vector3(-174.082f, 4.872605f, 105.5326f);
                        rot2 = Quaternion.Euler(0, 90f, 0);
                        break;
                    case "snowyforest":
                        pos = new Vector3(-99.5584f, 10.60039f, 102.537f);
                        rot = Quaternion.Euler(0, 225f, 0);
                        pos2 = new Vector3(136.0166f, 65.28467f, 53.11964f);
                        rot2 = Quaternion.Euler(0, 255f, 0);
                        break;
                    case "ancientloft":
                        pos = new Vector3(-65.35076f, 60.30833f, -291.5549f);
                        rot = Quaternion.Euler(0, 0f, 0);
                        pos2 = new Vector3(129.4765f, 10.40331f, 46.3731f);
                        rot2 = Quaternion.Euler(0, 0, 0);
                        break;
                    case "sulfurpools":
                        pos = new Vector3(22.0251f, -35.045f, 92.92287f);
                        rot = Quaternion.Euler(0, 195f, 0);
                        pos2 = new Vector3(113.6481f, 2.273259f, -149.9676f);
                        rot2 = Quaternion.Euler(0, 180, 3);
                        break;
                    case "lakes":
                        pos = new Vector3(-120.3369f, 1.2f, -32.24873f);
                        rot = Quaternion.Euler(0f, 230f, 0f);
                        pos2 = new Vector3(128.4f, 14f, -52.77487f);
                        rot2 = Quaternion.Euler(5f, 107f, 357f);
                        break;
                    case "catacombs_DS1_Catacombs":
                        pos = new Vector3(43.75f, 281.3f, -638.9667f);
                        rot = Quaternion.Euler(0f, 0f, 0f);
                        pos2 = new Vector3(98.62f, 177f, -264.2091f);
                        rot2 = Quaternion.Euler(0f, 180f, 0f);
                        break;
                    case "slumberingsatellite":
                        pos = new Vector3(-61.48913f, 89.4f, -20.67262f);
                        rot = Quaternion.Euler(0f, 315f, 4f);
                        pos2 = new Vector3(-64f, 89.25f, -18f);
                        rot2 = Quaternion.Euler(0f, 130f, 0f);
                        break;
                    case "FBLScene":
                        pos = new Vector3(192.4988f, 271.3f, 364.7507f);
                        rot = Quaternion.Euler(-6.40330307f, 15f, 340f);
                        pos2 = new Vector3(322.2951f, 231.8f, -114.0785f);
                        rot2 = Quaternion.Euler(28f, 130f, 3f);
                        break;

                    //WHY NOT???????????
                    // you were breaking out of the for loop, but the switch statement wasn't broken
                    // so the two random chests were still being spawned
                    // it's fixed but i'll leave this commented out- putting this back in is your call
                    default:
                        validStage = false;
                        bool isBlacklisted = false;
                        foreach (string stage in blacklistedStageNames)
                        {
                            if (currStageName == stage)
                            {
                                doSpawns = false;
                                isBlacklisted = true;
                                break;
                            }
                        }

                        if (!isBlacklisted)
                        {
                            SpawnChests();
                            SpawnChests();
                        }
                        break;
                    
                    /*case "arena":
                        doSpawns = false;
                        break;
                    case "artifactworld":
                        doSpawns = false;
                        break;
                    case "bazaar":
                        doSpawns = false;
                        break;*/
                    case "goldshores":
                        pos = new Vector3(-11.2222f, 47.6f, -71.46585f);
                        rot = Quaternion.Euler(0, 170f, 0);
                        pos2 = new Vector3(0f, 8000f, 0f);
                        rot2 = Quaternion.Euler(0, 10, 3);
                        break;
                    /*case "limbo":
                        doSpawns = false;
                        break;
                    case "moon":
                        doSpawns = false;
                        break;
                    case "moon2":
                        doSpawns = false;
                        break;*/
                    case "mysteryspace":
                        pos = new Vector3(378.8849f, -170f, 194.3512f);
                        rot = Quaternion.Euler(20f, 130f, 0f);
                        pos2 = new Vector3(0f, 8000f, 0f);
                        rot2 = Quaternion.Euler(0f, 10f, 3f);
                        break;
                    case "voidraid":
                        pos = new Vector3(-136.5067f, 12.4f, -193.3536f);
                        rot = Quaternion.Euler(0f, 50f, 0f);
                        pos2 = new Vector3(0f, 8000f, 0f);
                        rot2 = Quaternion.Euler(0f, 10f, 3f);
                        break;
                    /*case "voidstage":
                        doSpawns = false;
                        break;*/
                }

                if (NetworkServer.active && doSpawns)
                {
                    if (validStage)
                    {
                        NetworkServer.Spawn(GameObject.Instantiate(weaponChestPrefab, pos, rot));
                        NetworkServer.Spawn(GameObject.Instantiate(weaponChestPrefab, pos2, rot2));
                    }

                    if (hunkCount > 1)
                    {
                        for (int i = 1; i < hunkCount; i++)
                        {
                            SpawnChests();
                            SpawnChests();
                        }
                    }
                }
            }
        }

        private static string MakeInfectedName(On.RoR2.Util.orig_GetBestBodyName orig, GameObject bodyObject)
        {
            var text = orig(bodyObject);
            if (!bodyObject)
                return text;

            if (!bodyObject.TryGetComponent<CharacterBody>(out var body))
                return text;

            if (!body.HasBuff(infectedBuff) && !body.HasBuff(infectedBuff2)) return text;

            text = "Infected " + text;

            return text;
        }

        private static void PlayVisionsAnimation(On.EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.orig_OnEnter orig, EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle self)
        {
            GameObject i = EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.muzzleFlashEffectPrefab;
            if (self.characterBody.baseNameToken == bodyNameToken) EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.muzzleFlashEffectPrefab = null;

            orig(self);

            EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.muzzleFlashEffectPrefab = i;
            if (self.characterBody.baseNameToken == bodyNameToken)
            {
                self.PlayAnimation("Gesture, Override", "FireVisions");
                //EffectManager.SimpleMuzzleFlash(EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.muzzleFlashEffectPrefab, self.gameObject, "HandL", false);
            }
        }

        private static void PlayChargeLunarAnimation(On.EntityStates.GlobalSkills.LunarNeedle.ChargeLunarSecondary.orig_PlayChargeAnimation orig, EntityStates.GlobalSkills.LunarNeedle.ChargeLunarSecondary self)
        {
            orig(self);

            if (self.characterBody.baseNameToken == bodyNameToken)
            {
                self.PlayAnimation("Gesture, Override", "ChargeHooks", "Hooks.playbackRate", self.duration * 2.5f);
            }
        }

        private static void PlayThrowLunarAnimation(On.EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary.orig_PlayThrowAnimation orig, EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary self)
        {
            orig(self);

            if (self.characterBody.baseNameToken == bodyNameToken)
            {
                self.PlayAnimation("Gesture, Override", "ThrowHooks", "Hooks.playbackRate", self.duration);
            }
        }

        private static void PlayRuinAnimation(On.EntityStates.GlobalSkills.LunarDetonator.Detonate.orig_OnEnter orig, EntityStates.GlobalSkills.LunarDetonator.Detonate self)
        {
            orig(self);

            if (self.characterBody.baseNameToken == bodyNameToken)
            {
                self.PlayAnimation("Gesture, Override", "CastRuin", "Ruin.playbackRate", self.duration * 0.5f);
                //Util.PlaySound("PaladinFingerSnap", self.gameObject);
                self.StartAimMode(self.duration + 0.5f);

                EffectManager.SpawnEffect(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorMegaBlasterExplosion.prefab").WaitForCompletion(),
        new EffectData
        {
            origin = self.FindModelChild("HandL").position,
            rotation = Quaternion.identity,
            scale = 0.5f
        }, false);
            }
        }
    }
}