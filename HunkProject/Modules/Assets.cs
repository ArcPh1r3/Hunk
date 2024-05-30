﻿using System.Reflection;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using System.IO;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using TMPro;
using RoR2.UI;
using UnityEngine.UI;
using HunkMod.Modules.Components;
using UnityEngine.Rendering.PostProcessing;
using ThreeEyedGames;

namespace HunkMod.Modules
{
    public static class Assets
    {
        public static AssetBundle mainAssetBundle;

        internal static Shader hotpoo = Resources.Load<Shader>("Shaders/Deferred/HGStandard");
        internal static Material commandoMat;

        internal static List<EffectDef> effectDefs = new List<EffectDef>();
        internal static List<NetworkSoundEventDef> networkSoundEventDefs = new List<NetworkSoundEventDef>();

        internal static NetworkSoundEventDef knifeImpactSoundDef;
        internal static NetworkSoundEventDef kickImpactSoundDef;
        internal static NetworkSoundEventDef punchImpactSoundDef;

        public static GameObject railgunTracer;
        public static GameObject railgunImpact;
        public static GameObject railgunMuzzleFlash;

        public static GameObject headshotEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Common/VFX/WeakPointProcEffect.prefab").WaitForCompletion();
        public static GameObject virusPositionIndicator;
        public static GameObject tVirusPositionIndicator;

        public static Material dodgeOverlayMat;
        public static Material targetOverlayMat;

        public static Material virusBodyMat;
        public static Material tVirusBodyMat;
        public static Material tVirusOverlay;

        public static Material shieldOverlayMat;
        public static Material voidShieldOverlayMat;
        public static Material shieldMat;
        public static Material voidShieldMat;
        public static Material ravagerMat;

        public static GameObject ammoPickupEffectPrefab;
        public static GameObject explosionEffect;
        public static GameObject smallExplosionEffect;

        public static GameObject pistolCrosshairPrefab;
        public static GameObject magnumCrosshairPrefab;
        public static GameObject smgCrosshairPrefab;
        public static GameObject rocketLauncherCrosshairPrefab;
        public static GameObject rocketLauncherCrosshairPrefab2;
        public static GameObject grenadeLauncherCrosshairPrefab;
        public static GameObject grenadeLauncherCrosshairPrefab2;
        public static GameObject needlerCrosshairPrefab;
        public static GameObject shotgunCrosshairPrefab;
        public static GameObject circleCrosshairPrefab;

        public static GameObject weaponNotificationPrefab;
        public static GameObject headshotOverlay;
        public static GameObject headshotVisualizer;

        public static GameObject ammoPickupModel;
        public static GameObject bloodExplosionEffect;
        public static GameObject impEyeExplosionEffect;
        public static GameObject mangledExplosionEffect;
        public static GameObject bloodSpurtEffect;
        public static GameObject ammoPickupSparkle;
        public static GameObject ammoSpawnEffect;

        public static GameObject shotgunShell;
        public static GameObject shotgunSlug;

        public static GameObject ammoPickup;

        public static GameObject ammoPickupEffect;

        internal static GameObject knifeImpactEffect;
        internal static GameObject knifeSwingEffect;

        internal static GameObject knifeImpactEffectRed;
        internal static GameObject knifeSwingEffectRed;

        internal static GameObject kickImpactEffect;
        internal static GameObject kickSwingEffect;

        public static GameObject shotgunTracer;
        public static GameObject shotgunTracerCrit;

        public static GameObject weaponRadial;
        public static GameObject cardboardBox;
        public static GameObject fragGrenade;
        public static GameObject impEye;
        public static GameObject uroborosEffect;

        public static GameObject tarExplosion;

        internal static Material woundOverlayMat;
        public static Material tVirusMat;

        internal static TMP_FontAsset hgFont;

        internal static void PopulateAssets()
        {
            if (mainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("HunkMod.robhunk"))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }
            }

            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("HunkMod.hunk_bank.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }

            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("HunkMod.hunk_footsteps.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }

            ammoPickupEffectPrefab = CreateTextPopupEffect("HunkAmmoPickupEffect", "");
            //MainPlugin.Destroy(ammoPickup.GetComponent<EffectComponent>());

            weaponRadial = mainAssetBundle.LoadAsset<GameObject>("WeaponRadial");
            weaponRadial.AddComponent<WeaponRadial>();

            GameObject iLovePenis = mainAssetBundle.LoadAsset<GameObject>("AmmoInteraction");
            iLovePenis.transform.Find("Ring/Pillar").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matTracerBright.mat").WaitForCompletion();
            iLovePenis.transform.Find("Ring/Spark").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matWideGlow.mat").WaitForCompletion();

            Material intersectionMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Engi/matEngiMineZoneIntersection.mat").WaitForCompletion());
            intersectionMat.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampGolem.png").WaitForCompletion());
            intersectionMat.SetTexture("_Cloud1Tex", null);

            /*Material ammoPillarMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/moon2/matMoonElevatorPillarSubtle.mat").WaitForCompletion());
            ammoPillarMat.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampParent.png").WaitForCompletion());
            ammoPillarMat.SetColor("_TintColor", new Color(1f, 235f / 255f, 0.5f, 1f));
            ammoPillarMat.SetFloat("_Boost", 10f);
            ammoPillarMat.SetFloat("_AlphaBoost", 3f);
            ammoPillarMat.SetFloat("_AlphaBias", 0.35f);*/

            iLovePenis.transform.Find("Ring/SphereBorder").GetComponent<ParticleSystemRenderer>().material = intersectionMat;
            /*iLovePenis.transform.Find("Ring/CylinderBorder").GetComponent<ParticleSystemRenderer>().material = ammoPillarMat;

            iLovePenis.transform.Find("Ring/CylinderBorder").localPosition = new Vector3(0f, 3f, 0f);
            iLovePenis.transform.Find("Ring/CylinderBorder").localRotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
            iLovePenis.transform.Find("Ring/CylinderBorder").localScale = new Vector3(0.5f, 1f, 0.5f);*/
            iLovePenis.transform.Find("Ring/CylinderBorder").gameObject.SetActive(false);
            // this ended up looking bad

            hgFont = Addressables.LoadAssetAsync<TMP_FontAsset>("RoR2/Base/Common/Fonts/Bombardier/tmpBombDropshadow.asset").WaitForCompletion();

            /*GameObject textObjecthH = GameObject.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/HUDSimple.prefab").WaitForCompletion().transform.Find("MainContainer/MainUIArea/SpringCanvas/BottomLeftCluster/BarRoots/LevelDisplayCluster/PrefixText").gameObject);
            textObjecthH.transform.parent = weaponRadial.transform.Find("Center/Inner");
            textObjecthH.transform.localPosition = Vector3.zero;
            textObjecthH.GetComponentInChildren<LanguageTextMeshController>().token = "Lightning Hawk";*/

            woundOverlayMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/ArmorReductionOnHit/matPulverizedOverlay.mat").WaitForCompletion());
            woundOverlayMat.SetColor("_TintColor", Color.red);

            knifeImpactSoundDef = CreateNetworkSoundEventDef("sfx_hunk_knife_hit");
            kickImpactSoundDef = CreateNetworkSoundEventDef("sfx_hunk_kick_impact");
            punchImpactSoundDef = CreateNetworkSoundEventDef("sfx_hunk_punch_impact");

            virusPositionIndicator = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/BossPositionIndicator.prefab").WaitForCompletion().InstantiateClone("HunkVirusPositionIndicator", false);
            foreach (SpriteRenderer i in virusPositionIndicator.GetComponentsInChildren<SpriteRenderer>())
            {
                if (i) i.color = new Color(215f / 255f, 118f / 255f, 156f / 255f);
            }

            tVirusPositionIndicator = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/BossPositionIndicator.prefab").WaitForCompletion().InstantiateClone("HunkVirusPositionIndicator", false);
            foreach (SpriteRenderer i in tVirusPositionIndicator.GetComponentsInChildren<SpriteRenderer>())
            {
                if (i) i.color = new Color(28f / 255f, 69f / 255f, 1f);
            }

            headshotOverlay = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerScopeLightOverlay.prefab").WaitForCompletion().InstantiateClone("HunkHeadshotOverlay", false);
            SniperTargetViewer viewer = headshotOverlay.GetComponentInChildren<SniperTargetViewer>();
            headshotOverlay.transform.Find("ScopeOverlay").gameObject.SetActive(false);

            headshotVisualizer = viewer.visualizerPrefab.InstantiateClone("HunkHeadshotVisualizer", false);
            headshotVisualizer.GetComponentInChildren<ObjectScaleCurve>().baseScale = Vector3.one * 0.05f;
            Image headshotImage = headshotVisualizer.transform.Find("Scaler/Rectangle").GetComponent<Image>();
            headshotVisualizer.transform.Find("Scaler/Outer").gameObject.SetActive(false);
            headshotImage.color = new Color(1f, 1f, 1f, 0.95f);
            headshotImage.sprite = mainAssetBundle.LoadAsset<Sprite>("texWeakPointIndicator");

            viewer.visualizerPrefab = headshotVisualizer;
            bool dynamicCrosshair = Modules.Config.dynamicCrosshair.Value;

            Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Golem/LaserGolem.prefab").WaitForCompletion().AddComponent<Modules.Components.GolemLaser>();
            //Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Wisp/LaserWisp.prefab").WaitForCompletion().AddComponent<Modules.Components.GolemLaser>();

            #region Pistol Crosshair
            pistolCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion().InstantiateClone("HunkPistolCrosshair", false);
            pistolCrosshairPrefab.GetComponent<RawImage>().enabled = false;
            if (dynamicCrosshair) pistolCrosshairPrefab.AddComponent<DynamicCrosshair>();
            #endregion

            #region Magnum Crosshair
            magnumCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion().InstantiateClone("HunkMagnumCrosshair", false);
            magnumCrosshairPrefab.GetComponent<RawImage>().enabled = false;
            if (dynamicCrosshair) magnumCrosshairPrefab.AddComponent<DynamicCrosshair>();
            magnumCrosshairPrefab.AddComponent<CrosshairStartRotate>();
            #endregion

            #region SMG Crosshair
            smgCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion().InstantiateClone("HunkSMGCrosshair", false);
            smgCrosshairPrefab.GetComponent<RawImage>().enabled = false;
            if (dynamicCrosshair) smgCrosshairPrefab.AddComponent<DynamicCrosshair>();
            smgCrosshairPrefab.transform.GetChild(2).gameObject.SetActive(false);
            #endregion

            #region Grenade Launcher Crosshair
            grenadeLauncherCrosshairPrefab = PrefabAPI.InstantiateClone(LoadCrosshair("ToolbotGrenadeLauncher"), "HunkGrenadeLauncherCrosshair", false);
            if (dynamicCrosshair) grenadeLauncherCrosshairPrefab.AddComponent<DynamicCrosshair>();
            CrosshairController crosshair = grenadeLauncherCrosshairPrefab.GetComponent<CrosshairController>();
            crosshair.skillStockSpriteDisplays = new CrosshairController.SkillStockSpriteDisplay[0];

            grenadeLauncherCrosshairPrefab.transform.GetChild(0).GetComponentInChildren<Image>().sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/Railgunner/texCrosshairRailgunSniperNib.png").WaitForCompletion();
            RectTransform rect = grenadeLauncherCrosshairPrefab.transform.GetChild(0).GetComponent<RectTransform>();
            rect.localEulerAngles = Vector3.zero;
            rect.anchoredPosition = new Vector2(-50f, -10f);

            grenadeLauncherCrosshairPrefab.transform.GetChild(1).GetComponentInChildren<Image>().sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/Railgunner/texCrosshairRailgunSniperNib.png").WaitForCompletion();
            rect = grenadeLauncherCrosshairPrefab.transform.GetChild(1).GetComponent<RectTransform>();
            rect.localEulerAngles = new Vector3(0f, 0f, 90f);

            grenadeLauncherCrosshairPrefab.transform.GetChild(2).GetComponentInChildren<Image>().sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/Railgunner/texCrosshairRailgunSniperNib.png").WaitForCompletion();
            rect = grenadeLauncherCrosshairPrefab.transform.GetChild(2).GetComponent<RectTransform>();
            rect.localEulerAngles = Vector3.zero;
            rect.anchoredPosition = new Vector2(50f, -10f);

            grenadeLauncherCrosshairPrefab.transform.Find("StockCountHolder").gameObject.SetActive(false);
            grenadeLauncherCrosshairPrefab.transform.Find("Image, Arrow (1)").gameObject.SetActive(true);

            crosshair.spriteSpreadPositions[0].zeroPosition = new Vector3(25f, 25f, 0f);
            crosshair.spriteSpreadPositions[0].onePosition = new Vector3(-25f, 25f, 0f);

            crosshair.spriteSpreadPositions[1].zeroPosition = new Vector3(75f, 0f, 0f);
            crosshair.spriteSpreadPositions[1].onePosition = new Vector3(125f, 0f, 0f);

            crosshair.spriteSpreadPositions[2].zeroPosition = new Vector3(-25f, 25f, 0f);
            crosshair.spriteSpreadPositions[2].onePosition = new Vector3(25f, 25f, 0f);
            #endregion

            #region Rocket Launcher Crosshair
            rocketLauncherCrosshairPrefab = PrefabAPI.InstantiateClone(LoadCrosshair("ToolbotGrenadeLauncher"), "HunkRocketLauncherCrosshair", false);
            if (dynamicCrosshair) rocketLauncherCrosshairPrefab.AddComponent<DynamicCrosshair>();
            crosshair = rocketLauncherCrosshairPrefab.GetComponent<CrosshairController>();
            crosshair.skillStockSpriteDisplays = new CrosshairController.SkillStockSpriteDisplay[0];
            rocketLauncherCrosshairPrefab.transform.Find("StockCountHolder").gameObject.SetActive(false);
            #endregion

            #region Needler Crosshair
            needlerCrosshairPrefab = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/LoaderCrosshair"), "HunkNeedlerCrosshair", false);
            MainPlugin.Destroy(needlerCrosshairPrefab.GetComponent<LoaderHookCrosshairController>());
            if (dynamicCrosshair) needlerCrosshairPrefab.AddComponent<DynamicCrosshair>();

            needlerCrosshairPrefab.GetComponent<RawImage>().enabled = false;

            var control = needlerCrosshairPrefab.GetComponent<CrosshairController>();

            control.maxSpreadAlpha = 0;
            control.maxSpreadAngle = 3;
            control.minSpreadAlpha = 0;
            control.spriteSpreadPositions = new CrosshairController.SpritePosition[]
            {
                new CrosshairController.SpritePosition
                {
                    target = needlerCrosshairPrefab.transform.GetChild(2).GetComponent<RectTransform>(),
                    zeroPosition = new Vector3(-20f, 0, 0),
                    onePosition = new Vector3(-48f, 0, 0)
                },
                new CrosshairController.SpritePosition
                {
                    target = needlerCrosshairPrefab.transform.GetChild(3).GetComponent<RectTransform>(),
                    zeroPosition = new Vector3(20f, 0, 0),
                    onePosition = new Vector3(48f, 0, 0)
                }
            };

            MainPlugin.Destroy(needlerCrosshairPrefab.transform.GetChild(0).gameObject);
            MainPlugin.Destroy(needlerCrosshairPrefab.transform.GetChild(1).gameObject);
            #endregion

            #region Shotgun Crosshair
            shotgunCrosshairPrefab = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/LoaderCrosshair"), "HunkShotgunCrosshair", false);
            MainPlugin.Destroy(shotgunCrosshairPrefab.GetComponent<LoaderHookCrosshairController>());
            if (dynamicCrosshair) shotgunCrosshairPrefab.AddComponent<DynamicCrosshair>();

            shotgunCrosshairPrefab.GetComponent<RawImage>().enabled = false;

            control = shotgunCrosshairPrefab.GetComponent<CrosshairController>();

            control.maxSpreadAlpha = 0;
            control.maxSpreadAngle = 3;
            control.minSpreadAlpha = 0;
            control.spriteSpreadPositions = new CrosshairController.SpritePosition[]
            {
                new CrosshairController.SpritePosition
                {
                    target = shotgunCrosshairPrefab.transform.GetChild(2).GetComponent<RectTransform>(),
                    zeroPosition = new Vector3(-32f, 0, 0),
                    onePosition = new Vector3(-75f, 0, 0)
                },
                new CrosshairController.SpritePosition
                {
                    target = shotgunCrosshairPrefab.transform.GetChild(3).GetComponent<RectTransform>(),
                    zeroPosition = new Vector3(32f, 0, 0),
                    onePosition = new Vector3(75f, 0, 0)
                }
            };

            control.transform.Find("Bracket (2)").GetComponent<RectTransform>().localScale = new Vector3(1.25f, 1.75f, 1f);
            control.transform.Find("Bracket (3)").GetComponent<RectTransform>().localScale = new Vector3(1.25f, 1.75f, 1f);

            MainPlugin.Destroy(shotgunCrosshairPrefab.transform.GetChild(0).gameObject);
            MainPlugin.Destroy(shotgunCrosshairPrefab.transform.GetChild(1).gameObject);
            #endregion

            #region Grenade Launcher Crosshair 2
            grenadeLauncherCrosshairPrefab2 = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/LoaderCrosshair"), "HunkGrenadeLauncherCrosshair2", false);
            MainPlugin.Destroy(grenadeLauncherCrosshairPrefab2.GetComponent<LoaderHookCrosshairController>());
            if (dynamicCrosshair) grenadeLauncherCrosshairPrefab2.AddComponent<DynamicCrosshair>();

            grenadeLauncherCrosshairPrefab2.GetComponent<RawImage>().enabled = false;

            control = grenadeLauncherCrosshairPrefab2.GetComponent<CrosshairController>();

            control.maxSpreadAlpha = 0;
            control.maxSpreadAngle = 3;
            control.minSpreadAlpha = 0;
            control.spriteSpreadPositions = new CrosshairController.SpritePosition[]
            {
                new CrosshairController.SpritePosition
                {
                    target = grenadeLauncherCrosshairPrefab2.transform.GetChild(2).GetComponent<RectTransform>(),
                    zeroPosition = new Vector3(-32f, 0, 0),
                    onePosition = new Vector3(-75f, 0, 0)
                },
                new CrosshairController.SpritePosition
                {
                    target = grenadeLauncherCrosshairPrefab2.transform.GetChild(3).GetComponent<RectTransform>(),
                    zeroPosition = new Vector3(32f, 0, 0),
                    onePosition = new Vector3(75f, 0, 0)
                }
            };

            control.transform.Find("Bracket (2)").GetComponent<RectTransform>().localScale = new Vector3(2f, 2f, 1f);
            control.transform.Find("Bracket (3)").GetComponent<RectTransform>().localScale = new Vector3(2f, 2f, 1f);

            control.transform.Find("Bracket (2)").GetComponent<Image>().sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/Railgunner/texCrosshairRailgunnerBracket.png").WaitForCompletion();
            control.transform.Find("Bracket (3)").GetComponent<Image>().sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/Railgunner/texCrosshairRailgunnerBracket.png").WaitForCompletion();

            MainPlugin.Destroy(grenadeLauncherCrosshairPrefab2.transform.GetChild(0).gameObject);
            MainPlugin.Destroy(grenadeLauncherCrosshairPrefab2.transform.GetChild(1).gameObject);
            #endregion

            #region Rocket Launcher Crosshair 2
            rocketLauncherCrosshairPrefab2 = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/LoaderCrosshair"), "HunkRocketLauncherCrosshair2", false);
            MainPlugin.Destroy(rocketLauncherCrosshairPrefab2.GetComponent<LoaderHookCrosshairController>());
            if (dynamicCrosshair) rocketLauncherCrosshairPrefab2.AddComponent<DynamicCrosshair>();

            rocketLauncherCrosshairPrefab2.GetComponent<RawImage>().texture = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/UI/texCrosshairCircle.png").WaitForCompletion().texture;

            control = rocketLauncherCrosshairPrefab2.GetComponent<CrosshairController>();

            control.maxSpreadAlpha = 0;
            control.maxSpreadAngle = 3;
            control.minSpreadAlpha = 0;
            control.spriteSpreadPositions = new CrosshairController.SpritePosition[]
            {
                new CrosshairController.SpritePosition
                {
                    target = rocketLauncherCrosshairPrefab2.transform.GetChild(2).GetComponent<RectTransform>(),
                    zeroPosition = new Vector3(0f, 0, 0),
                    onePosition = new Vector3(0f, 0, 0)
                },
                new CrosshairController.SpritePosition
                {
                    target = rocketLauncherCrosshairPrefab2.transform.GetChild(3).GetComponent<RectTransform>(),
                    zeroPosition = new Vector3(0f, 0, 0),
                    onePosition = new Vector3(0f, 0, 0)
                }
            };

            control.transform.Find("Bracket (2)").GetComponent<RectTransform>().localScale = new Vector3(24f, 2f, 1f);
            control.transform.Find("Bracket (3)").GetComponent<RectTransform>().localScale = new Vector3(24f, 2f, 1f);
            control.transform.Find("Bracket (3)").GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));

            control.transform.Find("Bracket (2)").GetComponent<Image>().sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/UI/texCrosshairNibBar.png").WaitForCompletion();
            control.transform.Find("Bracket (3)").GetComponent<Image>().sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/UI/texCrosshairNibBar.png").WaitForCompletion();

            MainPlugin.Destroy(rocketLauncherCrosshairPrefab2.transform.GetChild(0).gameObject);
            MainPlugin.Destroy(rocketLauncherCrosshairPrefab2.transform.GetChild(1).gameObject);
            #endregion

            circleCrosshairPrefab = CreateCrosshair();

            shotgunShell = mainAssetBundle.LoadAsset<GameObject>("ShotgunShell");
            shotgunShell.GetComponentInChildren<MeshRenderer>().material = CreateMaterial("matShotgunShell");
            shotgunShell.AddComponent<Modules.Components.ShellController>();

            shotgunSlug = mainAssetBundle.LoadAsset<GameObject>("ShotgunSlug");
            shotgunSlug.GetComponentInChildren<MeshRenderer>().material = CreateMaterial("matShotgunSlug");
            shotgunSlug.AddComponent<Modules.Components.ShellController>();

            #region Ammo Pickup
            ammoPickup = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandolier/AmmoPack.prefab").WaitForCompletion().InstantiateClone("HunkAmmoPickup", true);

            ammoPickup.GetComponent<BeginRapidlyActivatingAndDeactivating>().delayBeforeBeginningBlinking = 55f;
            ammoPickup.GetComponent<DestroyOnTimer>().duration = 60f;

            AmmoPickup oldAmmoPickupComponent = ammoPickup.GetComponentInChildren<AmmoPickup>();
            HunkAmmoPickup ammoPickupComponent = oldAmmoPickupComponent.gameObject.AddComponent<HunkAmmoPickup>();

            ammoPickupComponent.baseObject = oldAmmoPickupComponent.baseObject;
            ammoPickupComponent.pickupEffect = oldAmmoPickupComponent.pickupEffect;
            ammoPickupComponent.teamFilter = oldAmmoPickupComponent.teamFilter;

            ammoPickup.GetComponentInChildren<MeshRenderer>().enabled = false;

            GameObject pickupModel = GameObject.Instantiate(mainAssetBundle.LoadAsset<GameObject>("AmmoPickup"));
            pickupModel.transform.parent = ammoPickup.transform.Find("Visuals");
            pickupModel.transform.localPosition = new Vector3(0f, -0.35f, 0f);
            pickupModel.transform.localRotation = Quaternion.identity;
            ConvertAllRenderersToHopooShader(pickupModel);

            //MeshRenderer pickupMesh = pickupModel.GetComponentInChildren<MeshRenderer>();
            //pickupMesh.material = CreateMaterial("matAmmoPickup");

            GravitatePickup oldGrav = ammoPickup.GetComponentInChildren<GravitatePickup>();
            HunkGravitatePickup grav = oldGrav.gameObject.AddComponent<HunkGravitatePickup>();

            grav.rigidbody = oldGrav.rigidbody;
            grav.acceleration = oldGrav.acceleration;
            grav.maxSpeed = oldGrav.maxSpeed;

            MonoBehaviour.Destroy(oldAmmoPickupComponent);
            MonoBehaviour.Destroy(oldGrav);
            #endregion

            ammoPickupEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandolier/AmmoPack.prefab").WaitForCompletion().GetComponentInChildren<AmmoPickup>().pickupEffect.InstantiateClone("RobHunkWeaponPickupEffect", true);
            ammoPickupEffect.AddComponent<NetworkIdentity>();
            AddNewEffectDef(ammoPickupEffect, "");

            weaponNotificationPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/NotificationPanel2.prefab").WaitForCompletion().InstantiateClone("HunkWeaponNotification", false);
            WeaponNotification _new = weaponNotificationPrefab.AddComponent<WeaponNotification>();
            GenericNotification _old = weaponNotificationPrefab.GetComponent<GenericNotification>();

            _new.titleText = _old.titleText;
            _new.titleTMP = _old.titleTMP;
            _new.descriptionText = _old.descriptionText;
            _new.iconImage = _old.iconImage;
            _new.previousIconImage = _old.previousIconImage;
            _new.canvasGroup = _old.canvasGroup;
            _new.fadeOutT = _old.fadeOutT;

            _old.enabled = false;


            explosionEffect = LoadEffect("BigExplosion", "sfx_hunk_explosion", false);
            explosionEffect.transform.Find("Shockwave").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matDistortion.mat").WaitForCompletion();
            ShakeEmitter shake = explosionEffect.AddComponent<ShakeEmitter>();
            ShakeEmitter shake2 = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BFG/BeamSphereExplosion.prefab").WaitForCompletion().GetComponent<ShakeEmitter>();
            shake.shakeOnStart = true;
            shake.shakeOnEnable = false;
            shake.wave = shake2.wave;
            shake.duration = 0.75f;
            shake.radius = 250f;
            shake.scaleShakeRadiusWithLocalScale = false;
            shake.amplitudeTimeDecay = true;

            GameObject pp = explosionEffect.transform.Find("PP").gameObject;
            pp.layer = 20;

            PostProcessVolume ppv = pp.AddComponent<PostProcessVolume>();
            ppv.sharedProfile = Addressables.LoadAssetAsync<PostProcessProfile>("RoR2/Base/title/ppLocalClayBossDeath.asset").WaitForCompletion();
            ppv.blendDistance = 40f;
            ppv.priority = 6f;
            ppv.weight = 1f;
            ppv.isGlobal = false;

            PostProcessDuration ppd = pp.AddComponent<PostProcessDuration>();
            ppd.ppVolume = ppv;
            ppd.ppWeightCurve = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossBlink.prefab").WaitForCompletion().GetComponentInChildren<PostProcessDuration>().ppWeightCurve;
            ppd.maxDuration = 3.5f;
            ppd.destroyOnEnd = true;

            SphereCollider sc = pp.AddComponent<SphereCollider>();
            sc.contactOffset = 0.01f;
            sc.isTrigger = true;
            sc.radius = 25f;

            GameObject nadeEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/OmniExplosionVFXCommandoGrenade.prefab").WaitForCompletion();
            GameObject radiusIndicator = nadeEffect.transform.Find("Nova Sphere").gameObject.InstantiateClone("HunkFuckShit", false);
            radiusIndicator.transform.parent = pp.transform.parent;
            radiusIndicator.transform.localPosition = Vector3.zero;
            radiusIndicator.transform.localScale = Vector3.one * 3.75f;
            radiusIndicator.transform.localRotation = Quaternion.identity;

            smallExplosionEffect = LoadEffect("SmallExplosion", "sfx_hunk_grenade_explosion", false);
            smallExplosionEffect.transform.Find("Shockwave").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matDistortion.mat").WaitForCompletion();
            shake = smallExplosionEffect.AddComponent<ShakeEmitter>();
            shake.shakeOnStart = true;
            shake.shakeOnEnable = false;
            shake.wave = shake2.wave;
            shake.duration = 0.5f;
            shake.radius = 60f;
            shake.scaleShakeRadiusWithLocalScale = false;
            shake.amplitudeTimeDecay = true;

            shotgunTracer = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerCommandoShotgun").InstantiateClone("HunkShotgunTracer", true);

            if (!shotgunTracer.GetComponent<EffectComponent>()) shotgunTracer.AddComponent<EffectComponent>();
            if (!shotgunTracer.GetComponent<VFXAttributes>()) shotgunTracer.AddComponent<VFXAttributes>();
            if (!shotgunTracer.GetComponent<NetworkIdentity>()) shotgunTracer.AddComponent<NetworkIdentity>();

            Material bulletMat = null;

            foreach (LineRenderer i in shotgunTracer.GetComponentsInChildren<LineRenderer>())
            {
                if (i)
                {
                    bulletMat = UnityEngine.Object.Instantiate<Material>(i.material);
                    bulletMat.SetColor("_TintColor", new Color(0.68f, 0.58f, 0.05f));
                    i.material = bulletMat;
                    i.startColor = new Color(0.68f, 0.58f, 0.05f);
                    i.endColor = new Color(0.68f, 0.58f, 0.05f);
                }
            }

            shotgunTracerCrit = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerCommandoShotgun").InstantiateClone("HunkShotgunTracerCritical", true);

            if (!shotgunTracerCrit.GetComponent<EffectComponent>()) shotgunTracerCrit.AddComponent<EffectComponent>();
            if (!shotgunTracerCrit.GetComponent<VFXAttributes>()) shotgunTracerCrit.AddComponent<VFXAttributes>();
            if (!shotgunTracerCrit.GetComponent<NetworkIdentity>()) shotgunTracerCrit.AddComponent<NetworkIdentity>();

            foreach (LineRenderer i in shotgunTracerCrit.GetComponentsInChildren<LineRenderer>())
            {
                if (i)
                {
                    Material material = UnityEngine.Object.Instantiate<Material>(i.material);
                    material.SetColor("_TintColor", Color.yellow);
                    i.material = material;
                    i.startColor = new Color(0.8f, 0.24f, 0f);
                    i.endColor = new Color(0.8f, 0.24f, 0f);
                }
            }

            AddNewEffectDef(shotgunTracer);
            AddNewEffectDef(shotgunTracerCrit);

            Modules.Config.InitROO(Assets.mainAssetBundle.LoadAsset<Sprite>("texHunkIcon"), "My extraction point.");

            knifeSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlash.prefab").WaitForCompletion().InstantiateClone("HunkKnifeSwing", false);
            knifeSwingEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressSwingTrail.mat").WaitForCompletion();

            knifeSwingEffectRed = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlash.prefab").WaitForCompletion().InstantiateClone("HunkKnifeSwingRed", false);
            Material swingMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressSwingTrail.mat").WaitForCompletion());
            swingMat.SetColor("_TintColor", Color.red);
            swingMat.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampWisp.png").WaitForCompletion());
            swingMat.SetFloat("_Boost", 4.680387f);
            knifeSwingEffectRed.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = swingMat;
            //

            kickSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlash.prefab").WaitForCompletion().InstantiateClone("HunkKickSwing", false);

            Material swipeMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matMercSwipe3.mat").WaitForCompletion());
            swipeMat.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/DLC1/VoidSurvivor/texRampVoidSurvivorCorrupted2.png").WaitForCompletion());
            swipeMat.SetFloat("_AlphaBias", 0f);
            //swipeMat.SetColor("_TintColor", Color.red);

            kickSwingEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = swipeMat;

            #region KnifeImpact
            knifeImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/OmniImpactVFXSlashMerc.prefab").WaitForCompletion().InstantiateClone("HunkKnifeImpact", false);
            knifeImpactEffect.GetComponent<OmniEffect>().enabled = false;

            Material hitsparkMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniHitspark3Merc.mat").WaitForCompletion());
            hitsparkMat.SetColor("_TintColor", Color.white);

            knifeImpactEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = hitsparkMat;

            //knifeImpactEffect.transform.GetChild(2).localScale = Vector3.one * 1.5f;
            knifeImpactEffect.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matOmniRing2Huntress.mat").WaitForCompletion();

            Material slashMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniRadialSlash1Generic.mat").WaitForCompletion());

            knifeImpactEffect.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = slashMat;

            knifeImpactEffect.transform.GetChild(6).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarWisp/matOmniHitspark1LunarWisp.mat").WaitForCompletion();
            knifeImpactEffect.transform.GetChild(6).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniHitspark2Generic.mat").WaitForCompletion();

            //knifeImpactEffect.transform.GetChild(1).localScale = Vector3.one * 1.5f;

            knifeImpactEffect.transform.GetChild(1).gameObject.SetActive(true);
            knifeImpactEffect.transform.GetChild(2).gameObject.SetActive(true);
            knifeImpactEffect.transform.GetChild(3).gameObject.SetActive(true);
            knifeImpactEffect.transform.GetChild(4).gameObject.SetActive(true);
            knifeImpactEffect.transform.GetChild(5).gameObject.SetActive(true);
            knifeImpactEffect.transform.GetChild(6).gameObject.SetActive(true);
            knifeImpactEffect.transform.GetChild(6).GetChild(0).gameObject.SetActive(true);

            //knifeImpactEffect.transform.GetChild(6).transform.localScale = new Vector3(1f, 1f, 3f);

            //knifeImpactEffect.transform.localScale = Vector3.one * 1.5f;

            AddNewEffectDef(knifeImpactEffect);
            #endregion

            #region KnifeImpactRed
            knifeImpactEffectRed = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/OmniImpactVFXSlashMerc.prefab").WaitForCompletion().InstantiateClone("HunkKnifeImpactRed", false);
            knifeImpactEffectRed.GetComponent<OmniEffect>().enabled = false;

            hitsparkMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniHitspark3Merc.mat").WaitForCompletion());
            hitsparkMat.SetColor("_TintColor", Color.red);

            knifeImpactEffectRed.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = hitsparkMat;

            //knifeImpactEffect.transform.GetChild(2).localScale = Vector3.one * 1.5f;
            knifeImpactEffectRed.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matOmniRing2Huntress.mat").WaitForCompletion();

            slashMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniRadialSlash1Generic.mat").WaitForCompletion());
            slashMat.SetColor("_TintColor", Color.red);

            knifeImpactEffectRed.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = slashMat;

            hitsparkMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniHitspark2Generic.mat").WaitForCompletion());
            hitsparkMat.SetColor("_TintColor", new Color(40f / 255f, 0f, 0f, 1f));
            knifeImpactEffectRed.transform.GetChild(6).gameObject.GetComponent<ParticleSystemRenderer>().material = hitsparkMat;

            hitsparkMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniHitspark1Generic.mat").WaitForCompletion());
            hitsparkMat.SetColor("_TintColor", new Color(40f / 255f, 0f, 0f, 1f));
            knifeImpactEffectRed.transform.GetChild(6).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarWisp/matOmniHitspark1LunarWisp.mat").WaitForCompletion();

            //knifeImpactEffect.transform.GetChild(1).localScale = Vector3.one * 1.5f;

            knifeImpactEffectRed.transform.GetChild(1).gameObject.SetActive(true);
            knifeImpactEffectRed.transform.GetChild(2).gameObject.SetActive(true);
            knifeImpactEffectRed.transform.GetChild(3).gameObject.SetActive(true);
            knifeImpactEffectRed.transform.GetChild(4).gameObject.SetActive(true);
            knifeImpactEffectRed.transform.GetChild(5).gameObject.SetActive(true);
            knifeImpactEffectRed.transform.GetChild(6).gameObject.SetActive(true);
            knifeImpactEffectRed.transform.GetChild(6).GetChild(0).gameObject.SetActive(true);

            //knifeImpactEffect.transform.GetChild(6).transform.localScale = new Vector3(1f, 1f, 3f);

            //knifeImpactEffect.transform.localScale = Vector3.one * 1.5f;

            AddNewEffectDef(knifeImpactEffectRed);
            #endregion

            #region KickImpact
            kickImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/OmniImpactVFXLoader.prefab").WaitForCompletion().InstantiateClone("HunkKickImpact", false);
            kickImpactEffect.GetComponent<OmniEffect>().enabled = false;

            hitsparkMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matOmniHitspark1Huntress.mat").WaitForCompletion());
            hitsparkMat.SetColor("_TintColor", Color.red);

            kickImpactEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = hitsparkMat;

            hitsparkMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matOmniHitspark3Huntress.mat").WaitForCompletion());
            hitsparkMat.SetColor("_TintColor", Color.red);
            kickImpactEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = hitsparkMat;

            kickImpactEffect.transform.GetChild(3).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Railgunner/matRailgunTracerHead.mat").WaitForCompletion();

            hitsparkMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniRing2Generic.mat").WaitForCompletion());
            hitsparkMat.SetColor("_TintColor", Color.red);
            kickImpactEffect.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = hitsparkMat;

            kickImpactEffect.transform.GetChild(4).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/moon2/matBloodSiphon.mat").WaitForCompletion();

            kickImpactEffect.transform.GetChild(0).gameObject.SetActive(true);
            kickImpactEffect.transform.GetChild(1).gameObject.SetActive(true);
            kickImpactEffect.transform.GetChild(2).gameObject.SetActive(true);
            kickImpactEffect.transform.GetChild(3).gameObject.SetActive(true);
            kickImpactEffect.transform.GetChild(4).gameObject.SetActive(true);
            kickImpactEffect.transform.GetChild(5).gameObject.SetActive(true);

            AddNewEffectDef(kickImpactEffect);
            #endregion

            Material lightningMat2 = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/ShockNearby/matLightningTeslaSlow.mat").WaitForCompletion());
            lightningMat2.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampLightning2.png").WaitForCompletion());

            railgunTracer = CreateTracer("TracerHuntressSnipe", "TracerHunkRailgun");

            Tracer tracer = railgunTracer.GetComponent<Tracer>();
            GameObject railgunBeam = tracer.beamObject;
            railgunBeam.GetComponent<ParticleSystemRenderer>().trailMaterial = lightningMat2;
            railgunBeam.transform.localScale *= 1.5f;
            //tracer.beamObject = railgunBeam;

            //Material lightningMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matLightningArm.mat").WaitForCompletion();

            //LineRenderer line = railgunTracer.transform.Find("TracerHead").GetComponent<LineRenderer>();
            //line.material = lightningMat;
            //line.startWidth *= 0.25f;
            //line.endWidth *= 0.25f;
            // this did not work.
            //line.material = Addressables.LoadAssetAsync<Material>("RoR2/Base/MagmaWorm/matMagmaWormFireballTrail.mat").WaitForCompletion();

            railgunMuzzleFlash = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lightning/LightningStrikeImpact.prefab").WaitForCompletion().InstantiateClone("HunkRailgunMuzzleFlash");

            railgunMuzzleFlash.transform.Find("LightningRibbon").gameObject.SetActive(false);
            railgunMuzzleFlash.transform.Find("Flash Lines").gameObject.SetActive(false);
            railgunMuzzleFlash.transform.Find("Flash").gameObject.SetActive(false);
            railgunMuzzleFlash.transform.Find("Distortion").gameObject.SetActive(false);
            railgunMuzzleFlash.GetComponent<ShakeEmitter>().wave.amplitude *= 0.25f;

            AddNewEffectDef(railgunMuzzleFlash);

            railgunImpact = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lightning/LightningStrikeImpact.prefab").WaitForCompletion().InstantiateClone("HunkRailgunImpactEffect");

            railgunImpact.transform.Find("LightningRibbon").gameObject.SetActive(false);
            railgunImpact.transform.Find("Sphere").gameObject.SetActive(false);

            AddNewEffectDef(railgunImpact, "sfx_hunk_railgun_impact");

            //
            bloodExplosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossBlink.prefab").WaitForCompletion().InstantiateClone("HunkBloodExplosion", false);

            Material bloodMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodHumanLarge.mat").WaitForCompletion();
            Material bloodMat2 = Addressables.LoadAssetAsync<Material>("RoR2/Base/moon2/matBloodSiphon.mat").WaitForCompletion();

            bloodExplosionEffect.transform.Find("Particles/LongLifeNoiseTrails").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodExplosionEffect.transform.Find("Particles/LongLifeNoiseTrails, Bright").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodExplosionEffect.transform.Find("Particles/Dash").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodExplosionEffect.transform.Find("Particles/Dash, Bright").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodExplosionEffect.transform.Find("Particles/DashRings").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/moon2/matBloodSiphon.mat").WaitForCompletion();
            bloodExplosionEffect.GetComponentInChildren<Light>().gameObject.SetActive(false);

            bloodExplosionEffect.GetComponentInChildren<PostProcessVolume>().sharedProfile = Addressables.LoadAssetAsync<PostProcessProfile>("RoR2/Base/title/ppLocalGold.asset").WaitForCompletion();
            bloodExplosionEffect.GetComponentInChildren<ShakeEmitter>().wave.amplitude *= 0.15f;
            bloodExplosionEffect.GetComponentInChildren<ShakeEmitter>().duration *= 0.25f;

            AddNewEffectDef(bloodExplosionEffect);
            //

            //
            impEyeExplosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossBlink.prefab").WaitForCompletion().InstantiateClone("HunkImpEyeExplosion", false);

            impEyeExplosionEffect.transform.Find("Particles/LongLifeNoiseTrails").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            impEyeExplosionEffect.transform.Find("Particles/LongLifeNoiseTrails, Bright").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            impEyeExplosionEffect.transform.Find("Particles/Dash").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            impEyeExplosionEffect.transform.Find("Particles/Dash, Bright").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            impEyeExplosionEffect.transform.Find("Particles/DashRings").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/moon2/matBloodSiphon.mat").WaitForCompletion();
            impEyeExplosionEffect.transform.Find("Particles/DashRings").localScale *= 0.25f;
            impEyeExplosionEffect.transform.Find("Particles/Distortion").gameObject.SetActive(false);
            impEyeExplosionEffect.transform.Find("Particles/Sphere").gameObject.SetActive(false);
            impEyeExplosionEffect.transform.Find("Particles/Flash, White").gameObject.SetActive(false);
            impEyeExplosionEffect.transform.Find("Particles/Flash, Red").gameObject.SetActive(false);
            impEyeExplosionEffect.GetComponentInChildren<Light>().gameObject.SetActive(false);

            impEyeExplosionEffect.GetComponentInChildren<PostProcessVolume>().sharedProfile = Addressables.LoadAssetAsync<PostProcessProfile>("RoR2/Base/title/ppLocalGold.asset").WaitForCompletion();
            impEyeExplosionEffect.GetComponentInChildren<ShakeEmitter>().wave.amplitude *= 0.05f;
            impEyeExplosionEffect.GetComponentInChildren<ShakeEmitter>().duration *= 0.1f;

            AddNewEffectDef(impEyeExplosionEffect);
            //

            //
            mangledExplosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossBlink.prefab").WaitForCompletion().InstantiateClone("HunkMangledExplosion", false);

            mangledExplosionEffect.transform.Find("Particles/LongLifeNoiseTrails").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            mangledExplosionEffect.transform.Find("Particles/LongLifeNoiseTrails, Bright").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            mangledExplosionEffect.transform.Find("Particles/Dash").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            mangledExplosionEffect.transform.Find("Particles/Dash, Bright").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            mangledExplosionEffect.transform.Find("Particles/DashRings").gameObject.SetActive(false);//GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/moon2/matBloodSiphon.mat").WaitForCompletion();
            mangledExplosionEffect.transform.Find("Particles/Distortion").gameObject.SetActive(false);
            mangledExplosionEffect.transform.Find("Particles/Sphere").gameObject.SetActive(false);
            mangledExplosionEffect.transform.Find("Particles/Flash, White").gameObject.SetActive(false);
            mangledExplosionEffect.transform.Find("Particles/Flash, Red").gameObject.SetActive(false);
            mangledExplosionEffect.GetComponentInChildren<Light>().gameObject.SetActive(false);

            mangledExplosionEffect.GetComponentInChildren<PostProcessVolume>().sharedProfile = Addressables.LoadAssetAsync<PostProcessProfile>("RoR2/Base/title/ppLocalGold.asset").WaitForCompletion();
            mangledExplosionEffect.GetComponentInChildren<ShakeEmitter>().wave.amplitude *= 0.05f;
            mangledExplosionEffect.GetComponentInChildren<ShakeEmitter>().duration *= 0.05f;

            AddNewEffectDef(mangledExplosionEffect, "sfx_hunk_mangle");
            //

            bloodSpurtEffect = mainAssetBundle.LoadAsset<GameObject>("BloodSpurtEffect");

            bloodSpurtEffect.transform.Find("Blood").GetComponent<ParticleSystemRenderer>().material = bloodMat2;
            bloodSpurtEffect.transform.Find("Trails").GetComponent<ParticleSystemRenderer>().trailMaterial = bloodMat2;

            ammoPickupModel = mainAssetBundle.LoadAsset<GameObject>("AmmoPickup");
            ConvertAllRenderersToHopooShader(ammoPickupModel);

            dodgeOverlayMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/moon2/matBloodSiphon.mat").WaitForCompletion());

            targetOverlayMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/matEnergyShield.mat").WaitForCompletion());
            targetOverlayMat.SetColor("_TintColor", Color.red);

            shieldMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/BearVoid/matBearVoidShield.mat").WaitForCompletion());
            shieldMat.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampShield.png").WaitForCompletion());
            shieldMat.SetTexture("_Cloud1Tex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/texHologramCloud.png").WaitForCompletion());
            shieldMat.SetFloat("_Boost", 20f);
            shieldMat.SetFloat("_AlphaBoost", 0.25f);
            shieldMat.SetFloat("_AlphaBias", 0.125f);

            voidShieldMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/BearVoid/matBearVoidShield.mat").WaitForCompletion());
            voidShieldMat.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampShield.png").WaitForCompletion());
            voidShieldMat.SetTexture("_Cloud1Tex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/texCloudStroke1.png").WaitForCompletion());
            voidShieldMat.SetColor("_TintColor", new Color(1f, 0f, 45f / 255f, 1f));
            voidShieldMat.SetFloat("_Boost", 20f);
            voidShieldMat.SetFloat("_AlphaBoost", 0.25f);
            voidShieldMat.SetFloat("_AlphaBias", 0.125f);

            shieldOverlayMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidMegaCrab/matVoidCrabMatterOverlay.mat").WaitForCompletion());
            shieldOverlayMat.SetTexture("_Cloud1Tex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/texCloudStroke1.png").WaitForCompletion());
            shieldOverlayMat.SetColor("_TintColor", new Color(0f, 88f / 255f, 144f / 255f, 1f));
            shieldOverlayMat.SetFloat("_Boost", 6.5f);
            shieldOverlayMat.SetFloat("_AlphaBoost", 4f);
            shieldOverlayMat.SetFloat("_AlphaBias", 0.6f);

            voidShieldOverlayMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidMegaCrab/matVoidCrabMatterOverlay.mat").WaitForCompletion());
            voidShieldOverlayMat.SetTexture("_Cloud1Tex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/texCloudStroke1.png").WaitForCompletion());
            voidShieldOverlayMat.SetColor("_TintColor", new Color(144f / 255f, 0f, 125f / 255f, 1f));
            voidShieldOverlayMat.SetFloat("_Boost", 6.5f);
            voidShieldOverlayMat.SetFloat("_AlphaBoost", 4f);
            voidShieldOverlayMat.SetFloat("_AlphaBias", 0.6f);

            ravagerMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidMegaCrab/matVoidCrabMatterOverlay.mat").WaitForCompletion());
            ravagerMat.SetTexture("_Cloud1Tex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/texHologramCloud.png").WaitForCompletion());
            ravagerMat.SetColor("_TintColor", Color.red);
            ravagerMat.SetFloat("_Boost", 20f);
            ravagerMat.SetFloat("_AlphaBoost", 2f);
            ravagerMat.SetFloat("_AlphaBias", 0.5f);

            cardboardBox = mainAssetBundle.LoadAsset<GameObject>("CardboardBox");
            ConvertAllRenderersToHopooShader(cardboardBox);

            fragGrenade = mainAssetBundle.LoadAsset<GameObject>("FragGrenade");
            ConvertAllRenderersToHopooShader(fragGrenade);

            Material eyeMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpBoss.mat").WaitForCompletion());
            eyeMat.SetFloat("_EmPower", 0f);

            impEye = mainAssetBundle.LoadAsset<GameObject>("mdlImpEye");
            impEye.GetComponentInChildren<SkinnedMeshRenderer>().material = eyeMat;

            uroborosEffect = mainAssetBundle.LoadAsset<GameObject>("UroborosEffect");
            uroborosEffect.AddComponent<EffectComponent>();
            uroborosEffect.AddComponent<VFXAttributes>();
            uroborosEffect.AddComponent<DestroyOnTimer>().duration = 10f;

            Material smokeMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matDustDirectional.mat").WaitForCompletion());
            smokeMat.SetColor("_TintColor", new Color(1f, 0f, 0f, 80f / 255f));
            smokeMat.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampShadowClone.png").WaitForCompletion());
            smokeMat.SetFloat("_AlphaBoost", 3.75f);
            smokeMat.SetFloat("_AlphaBias", 0.1f);

            uroborosEffect.transform.Find("Smoke").GetComponent<ParticleSystemRenderer>().material = smokeMat;
            uroborosEffect.transform.Find("Smoke/Embers").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Titan/matTitanLaserGlob.mat").WaitForCompletion();
            uroborosEffect.transform.Find("Smoke/Embers (1)").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Titan/matTitanLaserGlob.mat").WaitForCompletion();

            AddNewEffectDef(uroborosEffect);

            tarExplosion = CreateBloodExplosionEffect("HunkTarBloodExplosion", Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodClayLarge.mat").WaitForCompletion());

            Material sparkleMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Firework/matFireworkSparkle.mat").WaitForCompletion());
            sparkleMat.SetColor("_TintColor", new Color(1f, 47f / 255f, 0f, 1f));

            ammoPickupSparkle = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Infusion/InfusionOrbFlash.prefab").WaitForCompletion().InstantiateClone("HunkAmmoFlash", false);
            ammoPickupSparkle.transform.Find("Blood").GetComponent<ParticleSystemRenderer>().material = sparkleMat;
            AddNewEffectDef(ammoPickupSparkle);

            ammoSpawnEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/StunChanceOnHit/ImpactStunGrenade.prefab").WaitForCompletion().InstantiateClone("HunkAmmoSpawnEffect", false);

            ammoSpawnEffect.transform.Find("Sparks, Long Life").gameObject.SetActive(false);
            ammoSpawnEffect.transform.Find("Sparks, Short Life").transform.localScale = Vector3.one * 0.2f;
            ammoSpawnEffect.transform.Find("Flash").gameObject.SetActive(false);
            ammoSpawnEffect.transform.Find("SoftGlow").gameObject.SetActive(false);

            AddNewEffectDef(ammoSpawnEffect);

            //virusBodyMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/ParentEgg/matParentEggOuter.mat").WaitForCompletion());
            virusBodyMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/ArmorReductionOnHit/matPulverizedOverlay.mat").WaitForCompletion());
            virusBodyMat.SetColor("_TintColor", new Color(1f, 61f / 255f, 1f, 1f));
            virusBodyMat.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampParentTeleport.png").WaitForCompletion());
            virusBodyMat.SetFloat("_Boost", 1f);
            virusBodyMat.SetFloat("_AlphaBoost", 3f);
            virusBodyMat.SetFloat("_AlphaBias", 0.35f);

            tVirusBodyMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/ArmorReductionOnHit/matPulverizedOverlay.mat").WaitForCompletion());
            tVirusBodyMat.SetColor("_TintColor", new Color(28f / 255f, 69f / 255f, 1f));
            tVirusBodyMat.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampParentTeleport.png").WaitForCompletion());
            tVirusBodyMat.SetFloat("_Boost", 3f);
            tVirusBodyMat.SetFloat("_AlphaBoost", 1f);
            tVirusBodyMat.SetFloat("_AlphaBias", 0.35f);

            tVirusOverlay = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/ArmorReductionOnHit/matPulverizedOverlay.mat").WaitForCompletion());
            tVirusOverlay.SetColor("_TintColor", new Color(28f / 255f, 69f / 255f, 1f));
            tVirusOverlay.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampParentTeleport.png").WaitForCompletion());
            tVirusOverlay.SetFloat("_Boost", 20f);
            tVirusOverlay.SetFloat("_AlphaBoost", 1.8f);
            tVirusOverlay.SetFloat("_AlphaBias", 0.1f);

            tVirusMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidJailer/matVoidJailerEyes.mat").WaitForCompletion());
            tVirusMat.SetColor("_EmColor", new Color(44f / 255f, 61f / 255f, 1f, 1f));
        }

        private static GameObject CreateBloodExplosionEffect(string effectName, Material bloodMat, float scale = 1f)
        {
            GameObject newEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherSlamImpact.prefab").WaitForCompletion().InstantiateClone(effectName, true);

            var huh = newEffect.GetComponent<DestroyOnTimer>();
            if (huh) huh.duration = 80f;

            newEffect.transform.Find("Spikes, Small").gameObject.SetActive(false);

            newEffect.transform.Find("PP").gameObject.SetActive(false);//.GetComponent<PostProcessVolume>().sharedProfile = Addressables.LoadAssetAsync<PostProcessProfile>("RoR2/Base/title/ppLocalMagmaWorm.asset").WaitForCompletion();
            newEffect.transform.Find("Point light").gameObject.SetActive(false);//.GetComponent<Light>().color = Color.yellow;
            newEffect.transform.Find("Flash Lines").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOpaqueDustLargeDirectional.mat").WaitForCompletion();

            newEffect.transform.GetChild(3).GetComponent<ParticleSystemRenderer>().material = bloodMat;
            newEffect.transform.Find("Flash Lines, Fire").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            newEffect.transform.GetChild(6).GetComponent<ParticleSystemRenderer>().material = bloodMat;
            newEffect.transform.Find("Fire").GetComponent<ParticleSystemRenderer>().material = bloodMat;

            var sex = newEffect.transform.Find("Fire").GetComponent<ParticleSystem>().main;
            sex.startLifetimeMultiplier = 1.85f;
            sex = newEffect.transform.Find("Flash Lines, Fire").GetComponent<ParticleSystem>().main;
            sex.startLifetimeMultiplier = 1.53f;
            sex = newEffect.transform.GetChild(6).GetComponent<ParticleSystem>().main;
            sex.startLifetimeMultiplier = 1.74f;

            newEffect.transform.Find("Physics").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/MagmaWorm/matFracturedGround.mat").WaitForCompletion();

            newEffect.transform.Find("Decal").GetComponent<Decal>().Material = Addressables.LoadAssetAsync<Material>("RoR2/Base/ClayBruiser/matClayGooDecalLong.mat").WaitForCompletion();
            newEffect.transform.Find("Decal").GetComponent<AnimateShaderAlpha>().timeMax = 80f;
            newEffect.transform.Find("Decal").transform.localPosition = Vector3.zero;
            newEffect.transform.Find("Decal").transform.localScale = Vector3.one * 24f;

            newEffect.transform.Find("FoamSplash").gameObject.SetActive(false);
            newEffect.transform.Find("FoamBilllboard").gameObject.SetActive(false);
            newEffect.transform.Find("Dust").gameObject.SetActive(false);
            newEffect.transform.Find("Dust, Directional").gameObject.SetActive(false);

            newEffect.transform.localScale = Vector3.one * scale;

            AddNewEffectDef(newEffect);

            ParticleSystemColorFromEffectData fuck = newEffect.AddComponent<ParticleSystemColorFromEffectData>();
            fuck.particleSystems = new ParticleSystem[]
            {
                newEffect.transform.Find("Fire").GetComponent<ParticleSystem>(),
                newEffect.transform.Find("Flash Lines, Fire").GetComponent<ParticleSystem>(),
                newEffect.transform.GetChild(6).GetComponent<ParticleSystem>(),
                newEffect.transform.GetChild(3).GetComponent<ParticleSystem>()
            };
            fuck.effectComponent = newEffect.GetComponent<EffectComponent>();

            return newEffect;
        }

        private static GameObject CreateTracer(string originalTracerName, string newTracerName)
        {
            GameObject newTracer = R2API.PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName), newTracerName, true);

            if (!newTracer.GetComponent<EffectComponent>()) newTracer.AddComponent<EffectComponent>();
            if (!newTracer.GetComponent<VFXAttributes>()) newTracer.AddComponent<VFXAttributes>();
            if (!newTracer.GetComponent<NetworkIdentity>()) newTracer.AddComponent<NetworkIdentity>();

            newTracer.GetComponent<Tracer>().speed = 250f;
            newTracer.GetComponent<Tracer>().length = 50f;

            AddNewEffectDef(newTracer);

            return newTracer;
        }

        private static GameObject CreateCrosshair()
        {
            GameObject crosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2CrosshairPrepRevolver.prefab").WaitForCompletion().InstantiateClone("HunkCircleCrosshair", false);
            CrosshairController crosshair = crosshairPrefab.GetComponent<CrosshairController>();
            crosshair.skillStockSpriteDisplays = new CrosshairController.SkillStockSpriteDisplay[0];

            MainPlugin.DestroyImmediate(crosshairPrefab.transform.Find("Outer").GetComponent<ObjectScaleCurve>());
            crosshairPrefab.transform.Find("Outer").GetComponent<Image>().sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/UI/texCrosshairTridant.png").WaitForCompletion();
            RectTransform rectR = crosshairPrefab.transform.Find("Outer").GetComponent<RectTransform>();
            rectR.localScale = Vector3.one * 0.75f;

            GameObject nibL = GameObject.Instantiate(crosshair.transform.Find("Outer").gameObject);
            nibL.transform.SetParent(crosshairPrefab.transform);
            //nibL.GetComponent<Image>().sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/Railgunner/texCrosshairRailgunSniperCenter.png").WaitForCompletion();
            RectTransform rectL = nibL.GetComponent<RectTransform>();
            rectL.localEulerAngles = new Vector3(0f, 0f, 180f);

            crosshair.spriteSpreadPositions = new CrosshairController.SpritePosition[]
            {
                new CrosshairController.SpritePosition
                {
                    target = rectR,
                    zeroPosition = new Vector3(0f, 0f, 0f),
                    onePosition = new Vector3(10f, 10f, 0f)
                },
                new CrosshairController.SpritePosition
                {
                    target = rectL,
                    zeroPosition = new Vector3(0f, 0f, 0f),
                    onePosition = new Vector3(-10f, -10f, 0f)
                }
            };

            crosshairPrefab.AddComponent<Modules.Components.CrosshairRotator>();

            return crosshairPrefab;
        }

        internal static GameObject CreateTextPopupEffect(string prefabName, string token, Color color)
        {
            GameObject i = CreateTextPopupEffect(prefabName, token);

            i.GetComponentInChildren<TMP_Text>().color = color;

            return i;
        }

        internal static GameObject CreateTextPopupEffect(string prefabName, string token, string soundName = "")
        {
            GameObject i = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BearProc").InstantiateClone(prefabName, true);

            i.GetComponent<EffectComponent>().soundName = soundName;
            if (!i.GetComponent<NetworkIdentity>()) i.AddComponent<NetworkIdentity>();

            i.GetComponentInChildren<RoR2.UI.LanguageTextMeshController>().token = token;

            i.GetComponentInChildren<ObjectScaleCurve>().timeMax *= 3f;
            i.GetComponent<DestroyOnTimer>().duration *= 3f;

            Assets.AddNewEffectDef(i);

            return i;
        }

        internal static NetworkSoundEventDef CreateNetworkSoundEventDef(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
            networkSoundEventDef.eventName = eventName;

            networkSoundEventDefs.Add(networkSoundEventDef);

            return networkSoundEventDef;
        }

        internal static void ConvertAllRenderersToHopooShader(GameObject objectToConvert)
        {
            foreach (Renderer i in objectToConvert.GetComponentsInChildren<Renderer>())
            {
                if (i)
                {
                    if (i.material)
                    {
                        // exclude lines from this for obvious reasons
                        if (!i.GetComponent<LineRenderer>())
                        {
                            Texture normalMap = null;
                            if (i.material.HasProperty("_BumpMap"))
                            {
                                // transfer normal maps cuz im not gay
                                normalMap = i.material.GetTexture("_BumpMap");
                            }

                            i.material.shader = hotpoo;

                            if (normalMap != null)
                            {
                                i.material.SetTexture("_NormalTex", normalMap);
                                i.material.SetFloat("_NormalStrength", 1f);
                            }
                        }
                    }
                }
            }
        }

        internal static CharacterModel.RendererInfo[] SetupRendererInfos(GameObject obj)
        {
            MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
            CharacterModel.RendererInfo[] rendererInfos = new CharacterModel.RendererInfo[meshes.Length];

            for (int i = 0; i < meshes.Length; i++)
            {
                rendererInfos[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = meshes[i].material,
                    renderer = meshes[i],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false
                };
            }

            return rendererInfos;
        }

        public static GameObject LoadSurvivorModel(string modelName) {
            GameObject model = mainAssetBundle.LoadAsset<GameObject>(modelName);
            if (model == null) {
                Log.Error("Trying to load a null model- check to see if the name in your code matches the name of the object in Unity");
                return null;
            }

            return PrefabAPI.InstantiateClone(model, model.name, false);
        }

        internal static Texture LoadCharacterIcon(string characterName)
        {
            return mainAssetBundle.LoadAsset<Texture>("tex" + characterName + "Icon");
        }

        internal static Mesh LoadMesh(string meshName)
        {
            return mainAssetBundle.LoadAsset<Mesh>(meshName);
        }

        internal static GameObject LoadKnife(string modelName)
        {
            GameObject knife = mainAssetBundle.LoadAsset<GameObject>("mdl" + modelName);
            ConvertAllRenderersToHopooShader(knife);
            return knife;
        }

        internal static GameObject LoadCrosshair(string crosshairName)
        {
            return Resources.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair");
        }

        private static GameObject LoadEffect(string resourceName)
        {
            return LoadEffect(resourceName, "", false);
        }

        private static GameObject LoadEffect(string resourceName, string soundName)
        {
            return LoadEffect(resourceName, soundName, false);
        }

        private static GameObject LoadEffect(string resourceName, bool parentToTransform)
        {
            return LoadEffect(resourceName, "", parentToTransform);
        }

        private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform)
        {
            GameObject newEffect = mainAssetBundle.LoadAsset<GameObject>(resourceName);

            newEffect.AddComponent<DestroyOnTimer>().duration = 12;
            newEffect.AddComponent<NetworkIdentity>();
            newEffect.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            var effect = newEffect.AddComponent<EffectComponent>();
            effect.applyScale = false;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = parentToTransform;
            effect.positionAtReferencedTransform = true;
            effect.soundName = soundName;

            AddNewEffectDef(newEffect, soundName);

            return newEffect;
        }

        internal static void AddNewEffectDef(GameObject effectPrefab)
        {
            AddNewEffectDef(effectPrefab, "");
        }

        internal static void AddNewEffectDef(GameObject effectPrefab, string soundName)
        {
            EffectDef newEffectDef = new EffectDef();

            EffectComponent effectComponent = effectPrefab.GetComponent<EffectComponent>();
            if (!effectComponent) effectComponent = effectPrefab.AddComponent<EffectComponent>();

            VFXAttributes vfxAttributes = effectPrefab.GetComponent<VFXAttributes>();
            if (!vfxAttributes) vfxAttributes = effectPrefab.AddComponent<VFXAttributes>();

            newEffectDef.prefab = effectPrefab;
            newEffectDef.prefabEffectComponent = effectComponent;
            newEffectDef.prefabName = effectPrefab.name;
            newEffectDef.prefabVfxAttributes = vfxAttributes;
            newEffectDef.spawnSoundEventName = soundName;

            effectDefs.Add(newEffectDef);
        }

        public static Material CreateMaterial(string materialName, float emission, Color emissionColor, float normalStrength)
        {
            if (!commandoMat) commandoMat = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial;

            Material mat = UnityEngine.Object.Instantiate<Material>(commandoMat);
            Material tempMat = Assets.mainAssetBundle.LoadAsset<Material>(materialName);

            if (!tempMat) return commandoMat;

            mat.name = materialName;
            mat.SetColor("_Color", tempMat.GetColor("_Color"));
            mat.SetTexture("_MainTex", tempMat.GetTexture("_MainTex"));
            mat.SetColor("_EmColor", emissionColor);
            mat.SetFloat("_EmPower", emission);
            mat.SetTexture("_EmTex", tempMat.GetTexture("_EmissionMap"));
            if (normalStrength > 0f) mat.SetTexture("_NormalTex", tempMat.GetTexture("_BumpMap"));
            mat.SetFloat("_NormalStrength", normalStrength);

            //mat.DisableKeyword("DITHER");

            return mat;
        }

        public static Material CreateMaterial(string materialName)
        {
            return Assets.CreateMaterial(materialName, 0f);
        }

        public static Material CreateMaterial(string materialName, float emission)
        {
            return Assets.CreateMaterial(materialName, emission, Color.black);
        }

        public static Material CreateMaterial(string materialName, float emission, Color emissionColor)
        {
            return Assets.CreateMaterial(materialName, emission, emissionColor, 0f);
        }
    }
}