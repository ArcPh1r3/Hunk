using System;
using BepInEx;
using R2API.Utils;
using RoR2;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using R2API.Networking;
using HunkMod.Modules.Components;
using HunkMod.Modules.Survivors;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace HunkMod
{
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TeamMoonstorm.Starstorm2", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ContactLight.LostInTransit", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TeamMoonstorm.Starstorm2", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.RiskySleeps.ClassicItemsReturns", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Borbo.GreenAlienHead", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("bubbet.riskui", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "LanguageAPI",
        "SoundAPI",
        "DirectorAPI",
        "LoadoutAPI",
        "UnlockableAPI",
        "NetworkingAPI",
        "RecalculateStatsAPI",
    })]

    public class MainPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.rob.Hunk";
        public const string MODNAME = "Hunk";
        public const string MODVERSION = "1.0.0";

        public const string developerPrefix = "ROB";

        public static MainPlugin instance;

        public static bool scepterInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
        public static bool rooInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
        public static bool riskUIInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("bubbet.riskui");
        public static bool greenAlienHeadInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Borbo.GreenAlienHead");

        private void Awake()
        {
            instance = this;

            Modules.Config.myConfig = Config;

            Log.Init(Logger);
            Modules.Config.ReadConfig();
            Modules.Assets.PopulateAssets();
            Modules.CameraParams.InitializeParams();
            Modules.States.RegisterStates();
            Modules.Projectiles.RegisterProjectiles();
            Modules.Tokens.AddTokens();
            Modules.ItemDisplays.PopulateDisplays();

            new Modules.Survivors.Hunk().CreateCharacter();

            NetworkingAPI.RegisterMessageType<Modules.Components.SyncWeapon>();
            NetworkingAPI.RegisterMessageType<Modules.Components.SyncOverlay>();
            NetworkingAPI.RegisterMessageType<Modules.Components.SyncStoredWeapon>();
            NetworkingAPI.RegisterMessageType<Modules.Components.SyncDecapitation>();
            NetworkingAPI.RegisterMessageType<Modules.Components.SyncAmmoPickup>();

            Hook();

            new Modules.ContentPacks().Initialize();

            RoR2.ContentManagement.ContentManager.onContentPacksAssigned += LateSetup;

            CreateWeapons();
        }

        private void LateSetup(global::HG.ReadOnlyArray<RoR2.ContentManagement.ReadOnlyContentPack> obj)
        {
            Modules.Survivors.Hunk.SetItemDisplays();
        }

        private void CreateWeapons()
        {
            new Modules.Weapons.SMG().Init();
            new Modules.Weapons.M19().Init();
            new Modules.Weapons.MUP().Init();
            new Modules.Weapons.Magnum().Init();
            new Modules.Weapons.Shotgun().Init();
            new Modules.Weapons.Slugger().Init();
            new Modules.Weapons.RocketLauncher().Init();
            new Modules.Weapons.ATM().Init();
        }

        private void Hook()
        {
            //R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            //On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;

            // uncomment this if network testing
            //On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { };
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if (self && self.HasBuff(Modules.Survivors.Hunk.immobilizedBuff))
            {
                self.moveSpeed = 0f;
            }
        }

        private void CrosshairController_Awake(On.RoR2.UI.CrosshairController.orig_Awake orig, RoR2.UI.CrosshairController self)
        {
            orig(self);

            if (!self.name.Contains("SprintCrosshair"))
            {
                if (!self.GetComponent<Modules.Components.DynamicCrosshair>())
                {
                    self.gameObject.AddComponent<Modules.Components.DynamicCrosshair>();
                }
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args) {

            /*if (sender.HasBuff(Modules.Buffs.armorBuff)) {

                args.armorAdd += 500f;
            }

            if (sender.HasBuff(Modules.Buffs.slowStartBuff)) {

                args.armorAdd += 20f;
                args.moveSpeedReductionMultAdd += 1f; //movespeed *= 0.5f // 1 + 1 = divide by 2?
                args.attackSpeedMultAdd -= 0.5f; //attackSpeed *= 0.5f;
                args.damageMultAdd -= 0.5f; //damage *= 0.5f;
            }*/
        }

        public static float GetICBMDamageMult(CharacterBody body)
        {
            float mult = 1f;
            if (body && body.inventory)
            {
                int itemcount = body.inventory.GetItemCount(DLC1Content.Items.MoreMissile);
                int stack = itemcount - 1;
                if (stack > 0) mult += stack * 0.5f;
            }
            return mult;
        }
    }
}