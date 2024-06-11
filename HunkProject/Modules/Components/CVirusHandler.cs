using UnityEngine;
using RoR2;
using UnityEngine.Networking;

namespace HunkMod.Modules.Components
{
    public class CVirusHandler : MonoBehaviour
    {
        public GameObject attacker;

        private CharacterBody characterBody;
        private CharacterMaster characterMaster;
        private Inventory inventory;
        private bool revived;

        private void Start()
        {
            this.characterBody = this.GetComponent<CharacterBody>();
            if (this.characterBody)
            {
                this.characterMaster = this.characterBody.master;
                if (this.characterMaster)
                {
                    this.inventory = this.characterMaster.inventory;
                    this.characterMaster.gameObject.AddComponent<CVirusMaster>();
                }

                this.characterBody.baseMaxHealth *= 1.25f;
                this.characterBody.baseDamage *= 1.25f;

                if (NetworkServer.active)
                {
                    this.characterBody.AddBuff(Modules.Survivors.Hunk.infectedBuff3);
                    if (this.inventory)
                    {
                        this.inventory.GiveItem(RoR2Content.Items.TeleportWhenOob);
                        this.inventory.GiveItem(Modules.Survivors.Hunk.cVirus);
                        this.inventory.GiveItem(Modules.Survivors.Hunk.gVirus2);
                        this.inventory.GiveItem(Modules.Survivors.Hunk.gVirusFinal);
                    }
                }

                this.InvokeRepeating("AddOverlay", 0.2f, 40f);
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
                    temporaryOverlay.originalMaterial = Modules.Assets.cVirusBodyMat;
                    temporaryOverlay.inspectorCharacterModel = modelTransform.GetComponent<CharacterModel>();
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 40f, 1f);
                    temporaryOverlay.animateShaderAlpha = true;
                }
            }
        }

        private void AddOverlay2()
        {
            ModelLocator penis = this.GetComponent<ModelLocator>();
            if (penis)
            {
                Transform modelTransform = penis.modelTransform;
                if (modelTransform)
                {
                    TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 2f;
                    temporaryOverlay.destroyComponentOnEnd = false;
                    temporaryOverlay.originalMaterial = Modules.Assets.cVirusOverlay;
                    temporaryOverlay.inspectorCharacterModel = modelTransform.GetComponent<CharacterModel>();
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 1f);
                    temporaryOverlay.animateShaderAlpha = true;
                }
            }
        }

        private void Explode()
        {
            if (this.characterBody && this.characterBody.healthComponent.alive) this.characterBody.healthComponent.Suicide();
        }

        public void TriggerRevive()
        {
            if (this.revived) return;
            this.revived = true;

            this.Invoke("Explode", 9f);

            Util.PlaySound("sfx_hunk_cvirus_proc", this.gameObject);

            this.InvokeRepeating("AddOverlay2", 2, 2f);

            if (NetworkServer.active) this.characterBody.AddBuff(RoR2Content.Buffs.HealingDisabled);
            if (NetworkServer.active) this.characterBody.AddBuff(RoR2Content.Buffs.NoCooldowns);

            ModelLocator penis = this.GetComponent<ModelLocator>();
            if (penis)
            {
                Transform modelTransform = penis.modelTransform;
                if (modelTransform)
                {
                    TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 1f;
                    temporaryOverlay.destroyComponentOnEnd = false;
                    temporaryOverlay.originalMaterial = Modules.Assets.ravagerMat;
                    temporaryOverlay.inspectorCharacterModel = modelTransform.GetComponent<CharacterModel>();
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 5f, 1f, 0f);
                    temporaryOverlay.animateShaderAlpha = true;

                    TemporaryOverlay temporaryOverlay2 = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay2.duration = 8f;
                    temporaryOverlay2.destroyComponentOnEnd = false;
                    temporaryOverlay2.originalMaterial = Modules.Assets.ravagerMat;
                    temporaryOverlay2.inspectorCharacterModel = modelTransform.GetComponent<CharacterModel>();
                    temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, -6f, 1f, 8f);
                    temporaryOverlay2.animateShaderAlpha = true;
                }
            }

            if (this.GetComponent<DeathRewards>())
            {
                this.GetComponent<DeathRewards>().OnKilledServer(new DamageReport(new DamageInfo
                {
                    attacker = this.attacker
                }, this.characterBody.healthComponent, 1f, 1f));
            }

            if (this.GetComponent<CharacterDeathBehavior>()) this.GetComponent<CharacterDeathBehavior>().deathState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Virus.Death));

            this.characterBody.baseMoveSpeed *= 1.5f;
            this.characterBody.baseAttackSpeed *= 2.5f;
            this.characterBody.baseDamage *= 2f;
            this.characterBody.baseRegen = -this.characterBody.healthComponent.fullHealth * 0.125f;
            this.characterBody.RecalculateStats();
        }
    }
}