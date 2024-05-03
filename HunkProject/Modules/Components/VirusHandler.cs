using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace HunkMod.Modules.Components
{
    public class VirusHandler : MonoBehaviour
    {
        public float mutationStopwatch;
        public Material overrideMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/ParentEgg/matParentEggOuter.mat").WaitForCompletion();
        public Material overrideParticleMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/ParentEgg/matParentEggOuter.mat").WaitForCompletion();

        private CharacterModel characterModel;
        private CharacterBody characterBody;
        private CharacterMaster characterMaster;
        private Inventory inventory;
        private uint soundPlayID;

        private void Awake()
        {
            this.characterBody = this.GetComponent<CharacterBody>();
            this.characterMaster = this.characterBody.master;
            if (this.characterMaster) this.inventory = this.characterMaster.inventory;
            ModelLocator modelLocator = this.GetComponent<ModelLocator>();
            if (modelLocator && modelLocator.modelTransform)
            {
                this.characterModel = modelLocator.modelTransform.GetComponent<CharacterModel>();
            }

            // in between tier 1 and tier 2 elite
            this.characterBody.baseMaxHealth *= 10f;
            this.characterBody.baseDamage *= 4f;

            if (this.inventory)
            {
                this.inventory.GiveItem(RoR2Content.Items.AdaptiveArmor);
                this.inventory.GiveItem(RoR2Content.Items.TeleportWhenOob);
            }

            this.Mutate();

            this.soundPlayID = Util.PlaySound("sfx_hunk_syringe_buff", this.gameObject);
            Util.PlaySound("sfx_hunk_injection", this.gameObject);

            if (NetworkServer.active) this.characterBody.AddBuff(Modules.Survivors.Hunk.infectedBuff);
        }

        private void OnDestroy()
        {
            AkSoundEngine.StopPlayingID(this.soundPlayID);
        }

        private void FixedUpdate()
        {
            this.mutationStopwatch -= Time.fixedDeltaTime;

            if (this.characterBody)
            {
                if (this.characterBody.healthComponent && !this.characterBody.healthComponent.alive)
                {
                    var summon = new MasterSummon();
                    summon.position = this.transform.position + (Vector3.up * 2);
                    summon.masterPrefab = Modules.Enemies.Parasite.spadeMaster;
                    summon.summonerBodyObject = this.gameObject;
                    var master = summon.Perform();

                    Destroy(this);
                    return;
                }

                if (!this.characterBody.outOfCombat) this.mutationStopwatch = 60f;
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
                        if (this.characterModel.baseRendererInfos[i].renderer.gameObject.GetComponent<ParticleSystemRenderer>()) this.characterModel.baseRendererInfos[i].defaultMaterial = this.overrideParticleMat;
                        else this.characterModel.baseRendererInfos[i].defaultMaterial = this.overrideMat;
                    }
                }
            }
        }

        private void Mutate()
        {
            this.mutationStopwatch = 60f;

            if (this.inventory)
            {
                this.inventory.GiveItem(Modules.Survivors.Hunk.gVirus);
                this.inventory.GiveItem(RoR2Content.Items.BoostHp);

                if (this.inventory.GetItemCount(Modules.Survivors.Hunk.gVirus) >= 3) this.inventory.GiveItem(RoR2Content.Items.Medkit);
            }

            this.characterBody.RecalculateStats();
            this.characterBody.healthComponent.HealFraction(1f, default(ProcChainMask));

            //Util.PlaySound("sfx_hunk_virus_spawn", this.gameObject);
        }
    }
}