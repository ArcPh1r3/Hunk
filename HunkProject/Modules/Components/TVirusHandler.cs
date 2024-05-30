using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using System.Collections.Generic;
using R2API.Networking;
using R2API.Networking.Interfaces;

namespace HunkMod.Modules.Components
{
    public class TVirusHandler : MonoBehaviour
    {
        public float infectionRadius = 5f;
        private SphereSearch search;
        private List<HurtBox> hits;

        private CharacterBody characterBody;
        private CharacterMaster characterMaster;
        private Inventory inventory;

        private void Start()
        {
            this.characterBody = this.GetComponent<CharacterBody>();
            this.characterMaster = this.characterBody.master;
            if (this.characterMaster) this.inventory = this.characterMaster.inventory;
            this.characterMaster.gameObject.AddComponent<TVirusMaster>();

            this.characterBody.baseMaxHealth *= 1.5f;
            this.characterBody.baseDamage *= 1.2f;

            if (NetworkServer.active)
            {
                this.characterBody.AddBuff(Modules.Survivors.Hunk.infectedBuff2);
                if (this.inventory)
                {
                    this.inventory.GiveItem(RoR2Content.Items.TeleportWhenOob);
                    this.inventory.GiveItem(Modules.Survivors.Hunk.tVirus);
                    //this.inventory.GiveItem(Modules.Survivors.Hunk.tVirusRevival);
                    //this.inventory.GiveItem(RoR2Content.Items.ExtraLife);
                }
            }

            this.hits = new List<HurtBox>();
            this.search = new SphereSearch();
            this.search.mask = LayerIndex.entityPrecise.mask;
            this.search.radius = this.infectionRadius;

            if (NetworkServer.active) this.InvokeRepeating("AttemptSpread", 0f, 1f);
            this.InvokeRepeating("AddOverlay", 0.2f, 40f);
        }

        private void AttemptSpread()
        {
            if (!NetworkServer.active) return;

            NetworkIdentity identity = this.GetComponent<NetworkIdentity>();

            this.hits.Clear();
            this.search.ClearCandidates();
            this.search.origin = characterBody.corePosition;
            this.search.RefreshCandidates();
            this.search.FilterCandidatesByDistinctHurtBoxEntities();
            this.search.FilterCandidatesByHurtBoxTeam(TeamMask.none);
            this.search.GetHurtBoxes(hits);

            foreach (HurtBox h in this.hits)
            {
                HealthComponent hp = h.healthComponent;
                if (hp)
                {
                    if (hp.body.teamComponent.teamIndex == TeamIndex.Monster)
                    {
                        if (!hp.GetComponent<TVirusHandler>())
                        {
                            if (identity)
                            {
                                new SyncTVirus(identity.netId, hp.gameObject).Send(NetworkDestination.Clients);
                            }
                        }
                    }
                }
            }
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
                    temporaryOverlay.originalMaterial = Modules.Assets.tVirusBodyMat;
                    temporaryOverlay.inspectorCharacterModel = modelTransform.GetComponent<CharacterModel>();
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 40f, 1f);
                    temporaryOverlay.animateShaderAlpha = true;
                }
            }
        }

        public void TriggerRevive()
        {
            Util.PlaySound("sfx_hunk_tvirus_proc", this.gameObject);
            ModelLocator penis = this.GetComponent<ModelLocator>();
            if (penis)
            {
                Transform modelTransform = penis.modelTransform;
                if (modelTransform)
                {
                    TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 1f;
                    temporaryOverlay.destroyComponentOnEnd = false;
                    temporaryOverlay.originalMaterial = Modules.Assets.tVirusOverlay;
                    temporaryOverlay.inspectorCharacterModel = modelTransform.GetComponent<CharacterModel>();
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 3f, 1f, 0f);
                    temporaryOverlay.animateShaderAlpha = true;
                }
            }
        }
    }
}