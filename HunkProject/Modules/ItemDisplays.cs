using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.Modules
{
    internal static class ItemDisplays
    {
        private static Dictionary<string, GameObject> itemDisplayPrefabs = new Dictionary<string, GameObject>();

        internal static GameObject VirusEye;
        internal static GameObject GVirusSample;
        internal static GameObject SpadeKeycard;
        internal static GameObject ClubKeycard;
        internal static GameObject HeartKeycard;
        internal static GameObject DiamondKeycard;

        internal static void PopulateDisplays()
        {
            VirusEye = Assets.mainAssetBundle.LoadAsset<GameObject>("mdlVirusEye");

            Material eyeMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpBoss.mat").WaitForCompletion());
            eyeMat.SetColor("_EmColor", new Color(1f, 202f / 255f, 0f));
            eyeMat.SetFloat("_EmPower", 3f);
            eyeMat.SetFloat("_Smoothness", 0f);
            eyeMat.SetColor("_Color", new Color(189f / 255f, 57f / 255f, 130f / 255f));
            eyeMat.SetTexture("_MainTex", null);
            eyeMat.SetTexture("_FresnelRamp", Addressables.LoadAssetAsync<Texture>("RoR2/DLC1/Common/ColorRamps/texRampConstructLaser.png").WaitForCompletion());

            VirusEye.AddComponent<ItemDisplay>().rendererInfos = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    defaultMaterial = eyeMat,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    renderer = VirusEye.GetComponentInChildren<SkinnedMeshRenderer>()
                }
            };

            VirusEye.GetComponentInChildren<SkinnedMeshRenderer>().material = eyeMat;

            GVirusSample = Assets.mainAssetBundle.LoadAsset<GameObject>("DisplayGVirusSample");
            //Modules.Assets.ConvertAllRenderersToHopooShader(GVirusSample);
            GVirusSample.transform.Find("Model").GetComponent<MeshRenderer>().material = Modules.Assets.CreateMaterial("matGVirusSample2", 1f, Color.white, 1f);
            GVirusSample.transform.Find("Model/Glass").GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/HealingPotion/matHealingPotionGlass.mat").WaitForCompletion();
            GVirusSample.transform.Find("Model/Glass2").GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/HealingPotion/matHealingPotionGlass.mat").WaitForCompletion();
            GVirusSample.transform.Find("Model/30_MainMesh-2-SubMesh-1--sm74-201-Gvirus01_0.3_16_16").GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidJailer/matVoidJailerEyes.mat").WaitForCompletion();

            GVirusSample.AddComponent<ItemDisplay>().rendererInfos = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    defaultMaterial = Modules.Assets.CreateMaterial("matGVirusSample2", 1f, Color.white, 1f),
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    renderer = GVirusSample.transform.Find("Model").GetComponent<MeshRenderer>()
                },
                new CharacterModel.RendererInfo
                {
                    defaultMaterial = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/HealingPotion/matHealingPotionGlass.mat").WaitForCompletion(),
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    renderer = GVirusSample.transform.Find("Model/Glass").GetComponent<MeshRenderer>()
                },
                new CharacterModel.RendererInfo
                {
                    defaultMaterial = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/HealingPotion/matHealingPotionGlass.mat").WaitForCompletion(),
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    renderer = GVirusSample.transform.Find("Model/Glass2").GetComponent<MeshRenderer>()
                },
                new CharacterModel.RendererInfo
                {
                    defaultMaterial = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidJailer/matVoidJailerEyes.mat").WaitForCompletion(),
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    renderer = GVirusSample.transform.Find("Model/30_MainMesh-2-SubMesh-1--sm74-201-Gvirus01_0.3_16_16").GetComponent<MeshRenderer>()
                }
            };

            SpadeKeycard = Assets.mainAssetBundle.LoadAsset<GameObject>("DisplayKeycardSpade");
            Modules.Assets.ConvertAllRenderersToHopooShader(SpadeKeycard);
            SpadeKeycard.AddComponent<ItemDisplay>();

            ClubKeycard = Assets.mainAssetBundle.LoadAsset<GameObject>("DisplayKeycardClub");
            Modules.Assets.ConvertAllRenderersToHopooShader(ClubKeycard);
            ClubKeycard.AddComponent<ItemDisplay>();

            HeartKeycard = Assets.mainAssetBundle.LoadAsset<GameObject>("DisplayKeycardHeart");
            Modules.Assets.ConvertAllRenderersToHopooShader(HeartKeycard);
            HeartKeycard.AddComponent<ItemDisplay>();

            DiamondKeycard = Assets.mainAssetBundle.LoadAsset<GameObject>("DisplayKeycardDiamond");
            Modules.Assets.ConvertAllRenderersToHopooShader(DiamondKeycard);
            DiamondKeycard.AddComponent<ItemDisplay>();

            PopulateFromBody("Commando");
            PopulateFromBody("Croco");
        }

        private static void PopulateFromBody(string bodyName)
        {
            ItemDisplayRuleSet itemDisplayRuleSet = Resources.Load<GameObject>("Prefabs/CharacterBodies/" + bodyName + "Body").GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;

            ItemDisplayRuleSet.KeyAssetRuleGroup[] item = itemDisplayRuleSet.keyAssetRuleGroups;

            for (int i = 0; i < item.Length; i++)
            {
                ItemDisplayRule[] rules = item[i].displayRuleGroup.rules;

                for (int j = 0; j < rules.Length; j++)
                {
                    GameObject followerPrefab = rules[j].followerPrefab;
                    if (followerPrefab)
                    {
                        string name = followerPrefab.name;
                        string key = (name != null) ? name.ToLower() : null;
                        if (!itemDisplayPrefabs.ContainsKey(key))
                        {
                            itemDisplayPrefabs[key] = followerPrefab;
                        }
                    }
                }
            }
        }

        internal static GameObject LoadDisplay(string name)
        {
            if (itemDisplayPrefabs.ContainsKey(name.ToLower()))
            {
                if (itemDisplayPrefabs[name.ToLower()]) return itemDisplayPrefabs[name.ToLower()];
            }

            Debug.LogError("Could not find display prefab " + name);

            return null;
        }
    }
}