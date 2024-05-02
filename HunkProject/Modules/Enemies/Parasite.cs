using R2API;
using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Linq;
using RoR2.Navigation;
using RoR2.ExpansionManagement;

namespace HunkMod.Modules.Enemies
{
    internal class Parasite
    {
        internal static Parasite instance;

        internal static CharacterSpawnCard spadeSpawnCard;
        internal static CharacterSpawnCard clubSpawnCard;
        internal static CharacterSpawnCard heartSpawnCard;
        internal static CharacterSpawnCard diamondSpawnCard;

        internal static GameObject characterPrefab;
        internal static GameObject spadeMaster;
        internal static GameObject clubMaster;
        internal static GameObject heartMaster;
        internal static GameObject diamondMaster;

        public static Color characterColor = new Color(145f / 255f, 0f, 1f);

        public const string bodyName = "RobHunkParasiteBody";

        public static int bodyRendererIndex; // use this to store the rendererinfo index containing our character's body
                                             // keep it last in the rendererinfos because teleporter particles for some reason require this. hopoo pls

        // item display stuffs
        internal static ItemDisplayRuleSet itemDisplayRuleSet;
        internal static List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules;

        internal static string bodyNameToken;

        internal void CreateCharacter()
        {
            instance = this;

            characterPrefab = CreateBodyPrefab();

            spadeMaster = CreateMaster(characterPrefab, "RobHunkParasiteSpadeMaster");
            spadeMaster.AddComponent<Modules.Components.KeycardHolder>().itemDef = Modules.Survivors.Hunk.spadeKeycard;
            clubMaster = CreateMaster(characterPrefab, "RobHunkParasiteClubMaster");
            clubMaster.AddComponent<Modules.Components.KeycardHolder>().itemDef = Modules.Survivors.Hunk.clubKeycard;
            heartMaster = CreateMaster(characterPrefab, "RobHunkParasiteHeartMaster");
            heartMaster.AddComponent<Modules.Components.KeycardHolder>().itemDef = Modules.Survivors.Hunk.heartKeycard;
            diamondMaster = CreateMaster(characterPrefab, "RobHunkParasiteDiamondMaster");
            diamondMaster.AddComponent<Modules.Components.KeycardHolder>().itemDef = Modules.Survivors.Hunk.diamondKeycard;

            CreateSpawnCards();

            Hook();
        }

        private static GameObject CreateBodyPrefab()
        {
            bodyNameToken = MainPlugin.developerPrefix + "_HUNK_PARASITE_BODY_NAME";

            #region Body
            GameObject newPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/EliteVoid/VoidInfestorBody.prefab").WaitForCompletion().InstantiateClone(bodyName, true);
            newPrefab.GetComponent<CharacterBody>().baseNameToken = bodyNameToken;

            MainPlugin.DestroyImmediate(newPrefab.GetComponent<ExpansionRequirementComponent>());
            MainPlugin.DestroyImmediate(newPrefab.GetComponent<DeathRewards>());

            Modules.Prefabs.bodyPrefabs.Add(newPrefab);

            newPrefab.GetComponent<CharacterDeathBehavior>().deathState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Parasite.Death));
            #endregion

            #region Model
            CharacterModel characterModel = newPrefab.GetComponentInChildren<CharacterModel>();
            characterModel.baseRendererInfos[1].defaultMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/ParentEgg/matParentEggOuter.mat").WaitForCompletion();
            characterModel.baseRendererInfos[2].defaultMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Junk/Parent/matParentSpawnClouds.mat").WaitForCompletion();
            characterModel.baseRendererInfos[4].defaultMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/moon2/matMoonBatteryTrailBloodSiphon.mat").WaitForCompletion();
            #endregion

            //CreateHitboxes(newPrefab);
            //SetupHurtboxes(newPrefab);
            CreateSkills(newPrefab);
            //CreateSkins(newPrefab);
            //InitializeItemDisplays(newPrefab);

            return newPrefab;
        }

        private static void SetupHurtboxes(GameObject bodyPrefab)
        {
            /*HurtBoxGroup hurtboxGroup = bodyPrefab.GetComponentInChildren<HurtBoxGroup>();
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

            hurtboxGroup.hurtBoxes = hurtboxes.ToArray();*/
        }

        private static GameObject CreateMaster(GameObject bodyPrefab, string masterName)
        {
            GameObject newMaster = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/EliteVoid/VoidInfestorMaster.prefab").WaitForCompletion().InstantiateClone(masterName, true);
            newMaster.GetComponent<CharacterMaster>().bodyPrefab = bodyPrefab;
            /*
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
            */
            Modules.Prefabs.masterPrefabs.Add(newMaster);

            return newMaster;
        }

        private static void CreateSkills(GameObject prefab)
        {
            string prefix = MainPlugin.developerPrefix;
            SkillLocator skillLocator = prefab.GetComponent<SkillLocator>();

            Modules.Skills.CreateSkillFamilies(prefab);

            skillLocator.passiveSkill.enabled = false;

            SkillDef infest = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HUNK_BODY_SECONDARY_AIM_NAME",
                skillNameToken = prefix + "_HUNK_BODY_SECONDARY_AIM_NAME",
                skillDescriptionToken = prefix + "_HUNK_BODY_SECONDARY_AIM_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texAimIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Parasite.Infest)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 5f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Modules.Skills.AddPrimarySkills(prefab, infest);
            Modules.Skills.AddSecondarySkills(prefab, infest);
            Modules.Skills.AddUtilitySkills(prefab, infest);
            Modules.Skills.AddSpecialSkills(prefab, infest);
        }

        private static void CreateSpawnCards()
        {
            spadeSpawnCard = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            spadeSpawnCard.name = "cscHunkParasiteSpade";
            spadeSpawnCard.prefab = spadeMaster;
            spadeSpawnCard.sendOverNetwork = true;
            spadeSpawnCard.hullSize = HullClassification.Human;
            spadeSpawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
            spadeSpawnCard.requiredFlags = NodeFlags.None;
            spadeSpawnCard.forbiddenFlags = NodeFlags.None;
            spadeSpawnCard.directorCreditCost = 0;
            spadeSpawnCard.occupyPosition = false;
            spadeSpawnCard.loadout = new SerializableLoadout();
            spadeSpawnCard.noElites = true;
            spadeSpawnCard.forbiddenAsBoss = true;

            clubSpawnCard = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            clubSpawnCard.name = "cscHunkParasiteClub";
            clubSpawnCard.prefab = clubMaster;
            clubSpawnCard.sendOverNetwork = true;
            clubSpawnCard.hullSize = HullClassification.Human;
            clubSpawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
            clubSpawnCard.requiredFlags = NodeFlags.None;
            clubSpawnCard.forbiddenFlags = NodeFlags.None;
            clubSpawnCard.directorCreditCost = 0;
            clubSpawnCard.occupyPosition = false;
            clubSpawnCard.loadout = new SerializableLoadout();
            clubSpawnCard.noElites = true;
            clubSpawnCard.forbiddenAsBoss = true;

            heartSpawnCard = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            heartSpawnCard.name = "cscHunkParasiteHeart";
            heartSpawnCard.prefab = heartMaster;
            heartSpawnCard.sendOverNetwork = true;
            heartSpawnCard.hullSize = HullClassification.Human;
            heartSpawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
            heartSpawnCard.requiredFlags = NodeFlags.None;
            heartSpawnCard.forbiddenFlags = NodeFlags.None;
            heartSpawnCard.directorCreditCost = 0;
            heartSpawnCard.occupyPosition = false;
            heartSpawnCard.loadout = new SerializableLoadout();
            heartSpawnCard.noElites = true;
            heartSpawnCard.forbiddenAsBoss = true;

            diamondSpawnCard = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            diamondSpawnCard.name = "cscHunkParasiteDiamond";
            diamondSpawnCard.prefab = diamondMaster;
            diamondSpawnCard.sendOverNetwork = true;
            diamondSpawnCard.hullSize = HullClassification.Human;
            diamondSpawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
            diamondSpawnCard.requiredFlags = NodeFlags.None;
            diamondSpawnCard.forbiddenFlags = NodeFlags.None;
            diamondSpawnCard.directorCreditCost = 0;
            diamondSpawnCard.occupyPosition = false;
            diamondSpawnCard.loadout = new SerializableLoadout();
            diamondSpawnCard.noElites = true;
            diamondSpawnCard.forbiddenAsBoss = true;
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
            skins.Add(defaultSkin);
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

            newRendererInfos[0].defaultMaterial = materials[0];

            return newRendererInfos;
        }

        private static void Hook()
        {
        }
    }
}