﻿using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace HunkMod.Modules.Components
{
    public class VirusHandler : MonoBehaviour
    {
        public float mutationStopwatch;
        public Material overrideMat = Modules.Assets.virusBodyMat;
        public Material overrideParticleMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/moon2/matBloodSiphon.mat").WaitForCompletion();

        private CharacterModel characterModel;
        private CharacterBody characterBody;
        private CharacterMaster characterMaster;
        private Inventory inventory;
        private bool hasSpawnedCarrier;
        private uint soundPlayID;

        private void Start()
        {
            this.characterBody = this.GetComponent<CharacterBody>();
            this.characterMaster = this.characterBody.master;
            if (this.characterMaster) this.inventory = this.characterMaster.inventory;
            ModelLocator modelLocator = this.GetComponent<ModelLocator>();
            if (modelLocator && modelLocator.modelTransform)
            {
                this.characterModel = modelLocator.modelTransform.GetComponent<CharacterModel>();
            }

            this.characterBody.baseMaxHealth *= 4f;
            this.characterBody.baseDamage *= 1.25f;

            this.Mutate();

            this.soundPlayID = Util.PlaySound("sfx_hunk_syringe_buff", this.gameObject);
            //Util.PlaySound("sfx_hunk_injection", this.gameObject);

            if (NetworkServer.active)
            {
                this.characterBody.AddBuff(Modules.Survivors.Hunk.infectedBuff);
                if (this.inventory)
                {
                    this.inventory.GiveItem(RoR2Content.Items.TeleportWhenOob);
                }
            }

            this.InvokeRepeating("AddOverlay", 0f, 40f);
            this.AddOverlay();
        }

        private void AddOverlay()
        {
            ModelLocator penis = this.GetComponent<ModelLocator>();
            if (penis)
            {
                Transform modelTransform = penis.modelTransform;
                if (modelTransform)
                {
                    TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 40f;
                    temporaryOverlay.destroyComponentOnEnd = false;
                    temporaryOverlay.originalMaterial = Modules.Assets.virusBodyMat;
                    temporaryOverlay.inspectorCharacterModel = modelTransform.GetComponent<CharacterModel>();
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 40f, 1f);
                    temporaryOverlay.animateShaderAlpha = true;
                }
            }
        }

        private void OnEnable()
        {
            Modules.Survivors.Hunk.virusObjectiveObjects.Add(this.gameObject);
        }

        private void OnDisable()
        {
            //this.TrySpawn();
            Modules.Survivors.Hunk.virusObjectiveObjects.Remove(this.gameObject);
            AkSoundEngine.StopPlayingID(this.soundPlayID);
        }

        public void TrySpawn()
        {
            if (this.hasSpawnedCarrier) return;
            if (!NetworkServer.active) return;

            var summon = new MasterSummon();
            summon.position = this.transform.position + (Vector3.up * 2);
            summon.masterPrefab = Modules.Enemies.Parasite.characterMaster;
            summon.summonerBodyObject = this.gameObject;
            var master = summon.Perform();
            this.hasSpawnedCarrier = true;
        }

        private void FixedUpdate()
        {
            this.mutationStopwatch -= Time.fixedDeltaTime;

            if (this.characterBody)
            {
                if (this.characterBody.healthComponent && !this.characterBody.healthComponent.alive)
                {
                    return;
                }

                if (!this.characterBody.outOfDanger) this.mutationStopwatch = 40f;
            }

            if (this.mutationStopwatch <= 0f)
            {
                this.Mutate();
            }

            if (this.characterModel)
            {
                for (int i = 0; i < this.characterModel.baseRendererInfos.Length; i++)
                {
                    if (this.characterModel.baseRendererInfos[i].renderer)
                    {
                        if (this.characterModel.baseRendererInfos[i].renderer.gameObject.GetComponent<ParticleSystemRenderer>() || this.characterModel.baseRendererInfos[i].renderer.gameObject.GetComponent<LineRenderer>()) this.characterModel.baseRendererInfos[i].defaultMaterial = this.overrideParticleMat;
                        //else this.characterModel.baseRendererInfos[i].defaultMaterial = this.overrideMat;
                    }
                }
            }
        }

        private void Mutate()
        {
            this.mutationStopwatch = 20f;

            bool capped = Modules.Config.capInfection.Value;
            if (capped)
            {
                if (this.inventory && this.inventory.GetItemCount(Modules.Survivors.Hunk.gVirusFinal) > 0)
                {
                    return;
                }
            }

            if (this.characterBody && this.characterBody.teamComponent)
            {
                if (this.characterBody.teamComponent.teamIndex == TeamIndex.Player)
                {
                    this.characterBody.teamComponent.teamIndex = TeamIndex.Neutral;
                    if (this.characterBody.master) this.characterBody.master.teamIndex = TeamIndex.Neutral;
                }
            }

            if (NetworkServer.active)
            {
                if (this.inventory)
                {
                    this.inventory.GiveItem(Modules.Survivors.Hunk.gVirus);
                    this.inventory.GiveItem(RoR2Content.Items.BoostHp);

                    if (this.inventory.GetItemCount(Modules.Survivors.Hunk.gVirus) >= 3)
                    {
                        this.inventory.GiveItem(Modules.Survivors.Hunk.gVirus2);
                    }

                    if (this.inventory.GetItemCount(Modules.Survivors.Hunk.gVirus) >= 5)
                    {
                        this.inventory.GiveItem(Modules.Survivors.Hunk.gVirusFinal);
                        if (capped)
                        {
                            if (this.characterBody && this.characterBody.teamComponent)
                            {
                                this.characterBody.teamComponent.teamIndex = TeamIndex.Neutral;
                                if (this.characterBody.master) this.characterBody.master.teamIndex = TeamIndex.Neutral;
                            }

                            this.inventory.GiveItem(RoR2Content.Items.Medkit, 2);
                            this.inventory.GiveItem(RoR2Content.Items.AdaptiveArmor);
                            this.inventory.GiveItem(RoR2Content.Items.BoostHp, 2);
                        }
                        else
                        {
                            this.inventory.GiveItem(RoR2Content.Items.Medkit);
                        }
                    }
                }
            }

            this.characterBody.RecalculateStats();

            if (NetworkServer.active) this.characterBody.healthComponent.HealFraction(1f, default(ProcChainMask));

            if (this.characterModel)
            {
                this.characterModel.transform.localScale *= 1.1f;
            }
            Util.PlaySound("sfx_hunk_injection", this.gameObject);
        }
    }
}