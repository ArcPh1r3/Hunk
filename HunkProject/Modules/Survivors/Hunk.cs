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
        internal static UnlockableDef lightweightUnlockableDef;
        internal static UnlockableDef earlySupporterUnlockableDef;

        // skill overrides
        internal static SkillDef reloadSkillDef;
        internal static SkillDef counterSkillDef;

        internal static SkillDef confirmSkillDef;
        internal static SkillDef cancelSkillDef;

        internal static EntityStates.SerializableEntityStateType airDodgeState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.AirDodge));

        internal static string bodyNameToken;

        internal static GameObject spawnPodPrefab;
        internal static GameObject podPanelPrefab;
        internal static GameObject podContentPrefab;
        internal static Material miliMat;

        public static List<HunkWeaponDef> defaultWeaponPool = new List<HunkWeaponDef>();
        public static List<HunkWeaponDef> spawnedWeaponList = new List<HunkWeaponDef>();
        public static List<GameObject> virusObjectiveObjects = new List<GameObject>();


        public static string stageBlacklist = "arena,artifactworld,bazaar,limbo,moon,moon2,outro,voidoutro,voidraid,voidstage";
        public static List<string> blacklistedStageNames = new List<string>();

        public HealthBarStyle infectedHealthBarStyle;

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
        internal static ItemDef gVirusSample;
        internal static ItemDef gVirus;
        internal static ItemDef gVirus2;
        internal static ItemDef gVirusFinal;

        internal static BuffDef immobilizedBuff;
        internal static BuffDef infectedBuff;

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
                lightweightUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.HunkLightweightAchievement>();
                earlySupporterUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.HunkSupporterAchievement>();

                CreateKeycards();
                CreateAmmoInteractable();
                CreateBarrelAmmoInteractable();
                CreateChest();
                CreateCase();
                CreateTerminal();
                CreatePod();
                CreateHealthBarStyle();

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

                AddVirusDisplayRules();
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
            });;;

            ChildLocator childLocator = newPrefab.GetComponentInChildren<ChildLocator>();

            childLocator.gameObject.AddComponent<Modules.Components.HunkAnimationEvents>();

            CharacterBody body = newPrefab.GetComponent<CharacterBody>();
            //body.preferredInitialStateType = new EntityStates.SerializableEntityStateType(typeof(SpawnState));
            //body.bodyFlags = CharacterBody.BodyFlags.IgnoreFallDamage;
            //body.bodyFlags |= CharacterBody.BodyFlags.SprintAnyDirection;
            //body.sprintingSpeedMultiplier = 1.75f;
            body.hideCrosshair = true;

            //newPrefab.AddComponent<NinjaMod.Modules.Components.NinjaController>();

            //SfxLocator sfx = newPrefab.GetComponent<SfxLocator>();
            //sfx.barkSound = "";
            //sfx.landingSound = "";
            //sfx.deathSound = "";
            //sfx.fallDamageSound = "";

            //FootstepHandler footstep = newPrefab.GetComponentInChildren<FootstepHandler>();
            //footstep.footstepDustPrefab = Resources.Load<GameObject>("Prefabs/GenericHugeFootstepDust");
            //footstep.baseFootstepString = "Play_moonBrother_step";
            //footstep.sprintFootstepOverrideString = "Play_moonBrother_sprint";

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

            Hunk.reloadSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_PRIMARY_RELOAD_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_PRIMARY_RELOAD_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_RELOAD_DESCRIPTION",
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
                skillName = prefix + "_DRIVER_BODY_CONFIRM_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_CONFIRM_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_CONFIRM_DESCRIPTION",
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
                skillName = prefix + "_DRIVER_BODY_CANCEL_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_CANCEL_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_CANCEL_DESCRIPTION",
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
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texLeadfootIcon"),
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

            //Modules.Skills.AddUnlockablesToFamily(passive.passiveSkillSlot.skillFamily,
            //null, altPassiveUnlockableDef);
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
                MainPlugin.developerPrefix + "_HUNK_KEYWORD_LOOTING"
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
            SkillDef defaultKnifeSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HUNK_BODY_KNIFE_DEFAULT_NAME",
                skillNameToken = prefix + "_HUNK_BODY_KNIFE_DEFAULT_NAME",
                skillDescriptionToken = prefix + "_HUNK_BODY_KNIFE_DEFAULT_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKnifeIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "Default",
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

            SkillDef hiddenKnifeSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HUNK_BODY_KNIFE_HIDDEN_NAME",
                skillNameToken = prefix + "_HUNK_BODY_KNIFE_HIDDEN_NAME",
                skillDescriptionToken = prefix + "_HUNK_BODY_KNIFE_HIDDEN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKnifeIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "Hidden",
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
                    defaultKnifeSkillDef,
                    hiddenKnifeSkillDef
                });

            //Modules.Skills.AddUnlockablesToFamily(passive.passiveSkillSlot.skillFamily,
            //null, altPassiveUnlockableDef);
            #endregion


            if (MainPlugin.scepterInstalled) InitializeScepterSkills();
        }

        private static void InitializeScepterSkills()
        {
            /*AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterGrenadeSkillDef, bodyName, SkillSlot.Special, 0);
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterSupplyDropSkillDef, bodyName, SkillSlot.Special, 1);

            if (Modules.Config.cursed.Value)
            {
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterSupplyDropLegacySkillDef, bodyName, SkillSlot.Special, 2);
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterKnifeSkillDef, bodyName, SkillSlot.Special, 3);
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterSyringeSkillDef, bodyName, SkillSlot.Special, 4);
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterSyringeLegacySkillDef, bodyName, SkillSlot.Special, 5);
            }
            else
            {
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterKnifeSkillDef, bodyName, SkillSlot.Special, 2);
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterSyringeSkillDef, bodyName, SkillSlot.Special, 3);
            }*/
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

            skins.Add(commandoSkin);
            #endregion

            #region SuperSkin
            SkinDef superSkin = Modules.Skins.CreateSkinDef(MainPlugin.developerPrefix + "_HUNK_BODY_SUPER_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texSuperSkin"),
                defaultRenderers,
                mainRenderer,
                model,
                earlySupporterUnlockableDef);

            skins.Add(superSkin);
            #endregion

            skinController.skins = skins.ToArray();
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
        }

        internal static void SetItemDisplays()
        {
            // uhh
            Modules.ItemDisplays.PopulateDisplays();

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

            weaponChestPrefab.GetComponent<Highlight>().targetRenderer = displayCaseModel.transform.Find("Pivot/Model/SM_Weapon_Case_low.001").gameObject.GetComponent<MeshRenderer>();

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

            gVirus = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/ArtifactKey/ArtifactKey.asset").WaitForCompletion());
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

            gVirus2 = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/ArtifactKey/ArtifactKey.asset").WaitForCompletion());
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

            gVirusFinal = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/ArtifactKey/ArtifactKey.asset").WaitForCompletion());
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

            HunkWeaponCatalog.itemDefs.Add(spadeKeycard);
            HunkWeaponCatalog.itemDefs.Add(clubKeycard);
            HunkWeaponCatalog.itemDefs.Add(heartKeycard);
            HunkWeaponCatalog.itemDefs.Add(diamondKeycard);
            HunkWeaponCatalog.itemDefs.Add(gVirusSample);
            HunkWeaponCatalog.itemDefs.Add(gVirus);
            HunkWeaponCatalog.itemDefs.Add(gVirus2);
            HunkWeaponCatalog.itemDefs.Add(gVirusFinal);
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

            // bandolier
            On.RoR2.SkillLocator.ApplyAmmoPack += SkillLocator_ApplyAmmoPack;

            // knife ammo drop mechanic
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;

            // help me
            On.RoR2.Inventory.ShrineRestackInventory += Inventory_ShrineRestackInventory;

            // add chest cost types
            CostTypeCatalog.modHelper.getAdditionalEntries += AddHeartCostType;
            CostTypeCatalog.modHelper.getAdditionalEntries += AddSpadeCostType;
            CostTypeCatalog.modHelper.getAdditionalEntries += AddDiamondCostType;
            CostTypeCatalog.modHelper.getAdditionalEntries += AddClubCostType;
            CostTypeCatalog.modHelper.getAdditionalEntries += AddSampleCostType;

            // place chests
            On.RoR2.SceneDirector.Start += SceneDirector_Start;

            // set objective bullshit..
            On.RoR2.UI.ObjectivePanelController.GetObjectiveSources += ObjectivePanelController_GetObjectiveSources;

            // infected health bar
            On.RoR2.UI.HealthBar.Update += HealthBar_Update;

            // spawn rocket launcher on mithrix last phase
            //On.EntityStates.BrotherMonster.UltExitState.OnEnter += UltExitState_OnEnter;
            On.EntityStates.Missions.BrotherEncounter.Phase4.OnEnter += Phase4_OnEnter;

            // infected name tag
            On.RoR2.Util.GetBestBodyName += MakeInfectedName;

            // what
            On.RoR2.UI.LoadoutPanelController.Rebuild += LoadoutPanelController_Rebuild;

            // if i speak i am in trouble
            On.RoR2.UI.MainMenu.BaseMainMenuScreen.Awake += BaseMainMenuScreen_Awake;
            On.RoR2.UI.MainMenu.BaseMainMenuScreen.Update += BaseMainMenuScreen_Update;
            // 🙈 🙉 🙊

            // heresy anims
            //On.EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.OnEnter += PlayVisionsAnimation;
            //On.EntityStates.GlobalSkills.LunarNeedle.ChargeLunarSecondary.PlayChargeAnimation += PlayChargeLunarAnimation;
            //On.EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary.PlayThrowAnimation += PlayThrowLunarAnimation;
            //On.EntityStates.GlobalSkills.LunarDetonator.Detonate.OnEnter += PlayRuinAnimation;
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
            }
        }

        private static void BaseMainMenuScreen_Awake(On.RoR2.UI.MainMenu.BaseMainMenuScreen.orig_Awake orig, RoR2.UI.MainMenu.BaseMainMenuScreen self)
        {
            if (self) Util.PlaySound("sfx_hunk_virus_spawn", self.gameObject);
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

                    if (master.inventory.GetItemCount(Hunk.gVirusSample) > 0)
                    {
                        output.Add(new ObjectivePanelController.ObjectiveSourceDescriptor
                        {
                            source = master,
                            master = master,
                            objectiveType = typeof(Modules.Objectives.TurnInSample)
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
                                    self.GetComponent<Terminal>().itemDef = RoR2Content.Items.Pearl;
                                }
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
                    Util.PlaySound("sfx_hunk_weapon_case_open", self.gameObject);
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
                GameObject.Instantiate(Hunk.instance.ammoPickupInteractable, self.transform.position, self.transform.rotation);
            }

            orig(self);
        }

        private static void BaseMainMenuScreen_Update(On.RoR2.UI.MainMenu.BaseMainMenuScreen.orig_Update orig, RoR2.UI.MainMenu.BaseMainMenuScreen self)
        {
            orig(self);
            Transform buttonPanel = self.transform.Find("SafeZone/GenericMenuButtonPanel/ModPanel(Clone)");
            if (buttonPanel) buttonPanel.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
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

                    self.GetComponent<Highlight>().targetRenderer.transform.parent.parent.GetComponent<Animator>().Play("Open");

                    Util.PlaySound("sfx_hunk_weapon_case_open", self.gameObject);

                    return;
                }

                if (self.gameObject.name.Contains("HunkChest"))
                {
                    // this is the worst place to put this btw

                    self.GetComponent<WeaponChest>().gunPickup.enabled = true;
                    self.GetComponent<WeaponChest>().gunPickup.GetComponent<GenericPickupController>().enabled = true;

                    self.GetComponent<Highlight>().targetRenderer.transform.parent.parent.parent.GetComponent<Animator>().Play("Open");

                    Util.PlaySound("sfx_hunk_weapon_case_open", self.gameObject);

                    return;
                }

                if (!self.gameObject.name.Contains("Hunk") && !self.gameObject.name.Contains("uplica"))
                {
                    GameObject.Instantiate(Hunk.instance.ammoPickupInteractable, self.transform.position, self.transform.rotation);

                    if (self.tier3Chance >= 0.2f)
                    {
                        GameObject.Instantiate(Hunk.instance.ammoPickupInteractable, self.transform.position, self.transform.rotation);
                    }

                    if (self.tier3Chance >= 1f)
                    {
                        GameObject.Instantiate(Hunk.instance.ammoPickupInteractable, self.transform.position, self.transform.rotation);
                    }
                }

                if (self.gameObject.name.Contains("HunkTerminal"))
                {
                    self.GetComponent<PurchaseInteraction>().SetAvailable(true);

                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(self.GetComponent<Terminal>().itemDef.itemIndex), self.transform.position, (self.transform.forward * 5f) + Vector3.up * 25f);

                    return;
                }
            }

            orig(self);

        }

        private static void LoadoutPanelController_Rebuild(On.RoR2.UI.LoadoutPanelController.orig_Rebuild orig, LoadoutPanelController self)
        {
            orig(self);

            // this is beyond stupid lmfao who let this monkey code
            if (self.currentDisplayData.bodyIndex == BodyCatalog.FindBodyIndex("RobHunkBody"))
            {
                int j = 0;
                foreach (LanguageTextMeshController i in self.gameObject.GetComponentsInChildren<LanguageTextMeshController>())
                {
                    if (i && i.token == "LOADOUT_SKILL_MISC")
                    {
                        if (j <= 0) i.token = "Passive";
                        else i.token = "Knife";
                    }
                    j++;
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

                    // duration bar
                    GameObject chargeBar = GameObject.Instantiate(Assets.mainAssetBundle.LoadAsset<GameObject>("WeaponChargeBar"));
                    chargeBar.transform.SetParent(weaponSlot.transform.Find("DisplayRoot"));

                    RectTransform rect = chargeBar.GetComponent<RectTransform>();

                    rect.localScale = new Vector3(0.75f, 0.1f, 1f);
                    rect.anchorMin = new Vector2(0f, 0f);
                    rect.anchorMax = new Vector2(0f, 0f);
                    rect.pivot = new Vector2(0.5f, 0f);
                    rect.anchoredPosition = new Vector2(-10f, 13f);
                    rect.localPosition = new Vector3(-33f, -10f, 0f);
                    rect.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));

                    weaponIconComponent.durationDisplay = chargeBar;
                    weaponIconComponent.durationBar = chargeBar.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Image>();
                    weaponIconComponent.durationBarRed = chargeBar.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();

                    MonoBehaviour.Destroy(equipmentIconComponent);


                    // weapon pickup notification

                    GameObject notificationPanel = GameObject.Instantiate(hud.transform.Find("MainContainer").Find("NotificationArea").gameObject);
                    notificationPanel.transform.SetParent(hud.transform.Find("MainContainer"), true);
                    notificationPanel.GetComponent<RectTransform>().localPosition = new Vector3(0f, -265f, -150f);
                    notificationPanel.transform.localScale = Vector3.one;

                    NotificationUIController _old = notificationPanel.GetComponent<NotificationUIController>();
                    WeaponNotificationUIController _new = notificationPanel.AddComponent<WeaponNotificationUIController>();

                    _new.hud = _old.hud;
                    _new.genericNotificationPrefab = Modules.Assets.weaponNotificationPrefab;
                    _new.notificationQueue = hud.targetMaster.gameObject.AddComponent<WeaponNotificationQueue>();

                    _old.enabled = false;



                    // ammo display

                    Transform healthbarContainer = hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster").Find("BarRoots").Find("LevelDisplayCluster");

                    if (!hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster").Find("AmmoTracker"))
                    {
                        GameObject ammoTracker = GameObject.Instantiate(healthbarContainer.gameObject, hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster"));
                        ammoTracker.name = "AmmoTracker";
                        ammoTracker.transform.SetParent(hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").Find("CrosshairExtras"));

                        GameObject.DestroyImmediate(ammoTracker.transform.GetChild(0).gameObject);
                        MonoBehaviour.Destroy(ammoTracker.GetComponentInChildren<LevelText>());
                        MonoBehaviour.Destroy(ammoTracker.GetComponentInChildren<ExpBar>());

                        AmmoDisplay ammoTrackerComponent = ammoTracker.AddComponent<AmmoDisplay>();
                        ammoTrackerComponent.targetHUD = hud;
                        ammoTrackerComponent.targetText = ammoTracker.transform.Find("LevelDisplayRoot").Find("PrefixText").gameObject.GetComponent<LanguageTextMeshController>();

                        ammoTracker.transform.Find("LevelDisplayRoot").Find("ValueText").gameObject.SetActive(false);

                        //ammoTracker.transform.Find("ExpBarRoot").GetChild(0).GetComponent<Image>().enabled = true;

                        ammoTracker.transform.Find("LevelDisplayRoot").GetComponent<RectTransform>().anchoredPosition = new Vector2(-12f, 0f);

                        rect = ammoTracker.GetComponent<RectTransform>();
                        rect.localScale = new Vector3(0.8f, 0.8f, 1f);
                        rect.anchorMin = new Vector2(0f, 0f);
                        rect.anchorMax = new Vector2(0f, 0f);
                        rect.pivot = new Vector2(0.5f, 0f);
                        rect.anchoredPosition = new Vector2(50f, 0f);
                        rect.localPosition = new Vector3(50f, -95f, 0f);
                    }
                }
            }
        }

        internal static void RiskUIHudSetup(RoR2.UI.HUD hud)
        {
            Transform skillsContainer = hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomRightCluster").Find("Scaler");

            if (!skillsContainer.Find("WeaponSlot"))
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
                GameObject chargeBar = GameObject.Instantiate(Assets.mainAssetBundle.LoadAsset<GameObject>("WeaponChargeBar"));
                chargeBar.transform.SetParent(weaponSlot.transform.Find("DisplayRoot"));

                RectTransform rect = chargeBar.GetComponent<RectTransform>();

                rect.localScale = new Vector3(0.75f, 0.1f, 1f);
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(0f, 0f);
                rect.pivot = new Vector2(0.5f, 0f);
                rect.localPosition = new Vector3(0f, 0f, 0f);
                rect.anchoredPosition = new Vector2(-8f, 36f);
                rect.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));

                weaponIconComponent.durationDisplay = chargeBar;
                weaponIconComponent.durationBar = chargeBar.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Image>();
                weaponIconComponent.durationBarRed = chargeBar.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();

                MonoBehaviour.Destroy(equipmentIconComponent);
                MonoBehaviour.Destroy(x);


                // weapon pickup notification

                GameObject notificationPanel = GameObject.Instantiate(hud.transform.Find("MainContainer").Find("NotificationArea").gameObject);
                notificationPanel.transform.SetParent(hud.transform.Find("MainContainer"), true);
                notificationPanel.GetComponent<RectTransform>().localPosition = new Vector3(0f, -210f, -50f);
                notificationPanel.transform.localScale = Vector3.one;

                NotificationUIController _old = notificationPanel.GetComponent<NotificationUIController>();
                WeaponNotificationUIController _new = notificationPanel.AddComponent<WeaponNotificationUIController>();

                _new.hud = _old.hud;
                _new.genericNotificationPrefab = Modules.Assets.weaponNotificationPrefab;
                _new.notificationQueue = hud.targetMaster.gameObject.AddComponent<WeaponNotificationQueue>();

                _old.enabled = false;


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

                    rect = ammoTracker.GetComponent<RectTransform>();
                    rect.localScale = new Vector3(1f, 1f, 1f);
                    rect.anchorMin = new Vector2(0f, 0f);
                    rect.anchorMax = new Vector2(0f, 0f);
                    rect.pivot = new Vector2(0.5f, 0f);
                    rect.anchoredPosition = new Vector2(50f, 0f);
                    rect.localPosition = new Vector3(100f, -150f, 0f);
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

            spawnedWeaponList = new List<HunkWeaponDef>();

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
                        pos2 = new Vector3(179.5892f, 77.46913f, -10.06462f);
                        rot2 = Quaternion.Euler(0, 335, 0);
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
                        pos = new Vector3(-65.35076f, 60.30833f, -292.9549f);
                        rot = Quaternion.Euler(0, 354.5f, 0);
                        pos2 = new Vector3(129.4765f, 10.40331f, 46.3731f);
                        rot2 = Quaternion.Euler(0, 0, 0);
                        break;
                    case "sulfurpools":
                        pos = new Vector3(22.0251f, -35.045f, 92.92287f);
                        rot = Quaternion.Euler(0, 195f, 0);
                        pos2 = new Vector3(113.6481f, 2.273259f, -149.9676f);
                        rot2 = Quaternion.Euler(0, 180, 3);
                        break;

                    //WHY NOT???????????
                    // you were breaking out of the for loop, but the switch statement wasn't broken
                    // so the two random chests were still being spawned
                    // it's fixed but i'll leave this commented out- putting this back in is your call
                    default:
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
                    /*case "voidraid":
                        doSpawns = false;
                        break;
                    case "voidstage":
                        doSpawns = false;
                        break;*/
                }

                if (NetworkServer.active && doSpawns)
                {
                    GameObject chest1 = Object.Instantiate(weaponChestPrefab, pos, rot);
                    NetworkServer.Spawn(chest1);

                    GameObject chest2 = Object.Instantiate(weaponChestPrefab, pos2, rot2);
                    NetworkServer.Spawn(chest2);

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
            var infectedIndex = infectedBuff.buffIndex;
            if (!bodyObject)
                return text;

            if (!bodyObject.TryGetComponent<CharacterBody>(out var body))
                return text;

            if (!body.HasBuff(infectedIndex))
            {
                return text;
            }

            text = "Infected " + text;

            return text;
        }

        private static void PlayVisionsAnimation(On.EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.orig_OnEnter orig, EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle self)
        {
            orig(self);

            if (self.characterBody.baseNameToken == bodyNameToken)
            {
                self.PlayAnimation("Gesture, Override", "Shoot", "Shoot.playbackRate", self.duration * 12f);
                EffectManager.SimpleMuzzleFlash(EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.muzzleFlashEffectPrefab, self.gameObject, "PistolMuzzle", false);
            }
        }

        private static void PlayChargeLunarAnimation(On.EntityStates.GlobalSkills.LunarNeedle.ChargeLunarSecondary.orig_PlayChargeAnimation orig, EntityStates.GlobalSkills.LunarNeedle.ChargeLunarSecondary self)
        {
            orig(self);

            if (self.characterBody.baseNameToken == bodyNameToken)
            {
                self.PlayAnimation("Gesture, Override", "ChargeHooks", "Hooks.playbackRate", self.duration * 0.5f);
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
                //self.PlayAnimation("Gesture, Override", "CastRuin", "Ruin.playbackRate", self.duration * 0.5f);
                //Util.PlaySound("PaladinFingerSnap", self.gameObject);
                self.PlayAnimation("Gesture, Override", "PressVoidButton", "Action.playbackRate", 0.5f * self.duration);
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