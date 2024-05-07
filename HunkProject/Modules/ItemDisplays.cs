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