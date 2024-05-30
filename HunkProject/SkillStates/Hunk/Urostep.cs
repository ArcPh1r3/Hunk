using UnityEngine;
using RoR2;
using EntityStates;
using RoR2.Projectile;
using System.Collections.Generic;

namespace HunkMod.SkillStates.Hunk
{
    public class Urostep : BaseHunkSkillState
    {
        public float duration = 1.2f;

        private bool hasBlinked;
        private bool removedInvis;
        private CharacterModel characterModel;

        public float checkRadius = 10f;
        private SphereSearch search;
        private List<HurtBox> hits;
        private bool success;
        private bool startedInvis;
        private Vector3 storedVector;
        private Animator animator;

        protected override bool turningAllowed => false;

        protected virtual string soundString
        {
            get
            {
                return "";
            }
        }

        protected virtual GameObject blinkEffect
        {
            get
            {
                return Modules.Assets.uroborosEffect;
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = this.GetModelAnimator();
            this.animator.SetBool("canCancel", false);
            this.storedVector = this.inputBank.moveVector;
            if (this.inputBank.moveVector == Vector3.zero) this.storedVector = this.characterDirection.forward;
            this.hasBlinked = false;
            this.characterModel = this.GetModelTransform().GetComponent<CharacterModel>();
            this.hunk.isRolling = true;

            //base.PlayAnimation("Gesture, Override", "BufferEmpty");
            //base.PlayAnimation("FullBody, Override", "GreatswordDash", "Roll.playbackRate", this.duration * 1.5f);
            Util.PlaySound(this.soundString, this.gameObject);
            this.hunk.iFrames = this.duration;

            hits = new List<HurtBox>();
            search = new SphereSearch();
            search.mask = LayerIndex.entityPrecise.mask;
            search.radius = checkRadius;

            EntityStateMachine.FindByCustomName(this.gameObject, "Aim").SetNextStateToMain();
            this.skillLocator.secondary.stock = 0;
            this.skillLocator.secondary.rechargeStopwatch = 0f;

            this.skillLocator.primary.SetSkillOverride(this, Modules.Survivors.Hunk.scepterCounterSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            if (this.SearchAttacker())
            {
                this.success = true;
                this.skillLocator.utility.AddOneStock();
                this.skillLocator.utility.stock = this.skillLocator.utility.maxStock;
                this.hunk.TriggerDodge();
                this.hunk.lockOnTimer = 0.55f;
                base.fixedAge = this.duration * 0.25f;
                if (base.isAuthority)
                {
                    Util.PlaySound("sfx_hunk_dodge_perfect", this.gameObject);
                    this.CreateTargetedEffect();
                }
            }
            else
            {
                base.PlayCrossfade("FullBody, Override", "Urostep", "Dodge.playbackRate", this.duration * 0.2f, 0.05f);
                Util.PlaySound("sfx_hunk_step_foley", this.gameObject);
            }
        }

        private void CreateTargetedEffect()
        {
            if (this.hunk.targetHurtbox)
            {
                if (this.hunk.targetHurtbox.healthComponent && this.hunk.targetHurtbox.healthComponent.modelLocator && this.hunk.targetHurtbox.healthComponent.modelLocator.modelTransform)
                {
                    Transform modelTransform = this.hunk.targetHurtbox.healthComponent.modelLocator.modelTransform;

                    TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 1f;
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = Modules.Assets.targetOverlayMat;
                    temporaryOverlay.inspectorCharacterModel = modelTransform.GetComponent<CharacterModel>();
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 2f, 1f, 0f);
                    temporaryOverlay.animateShaderAlpha = true;
                }
            }
        }

        public bool SearchAttacker()
        {
            this.hunk.targetHurtbox = null;

            hits.Clear();
            search.ClearCandidates();
            search.origin = characterBody.corePosition;
            search.RefreshCandidates();
            search.FilterCandidatesByDistinctHurtBoxEntities();
            search.FilterCandidatesByHurtBoxTeam(TeamMask.GetUnprotectedTeams(teamComponent.teamIndex));
            search.GetHurtBoxes(hits);
            foreach (HurtBox h in hits)
            {
                HealthComponent hp = h.healthComponent;
                if (hp)
                {
                    if (hp.body.outOfCombatStopwatch <= 0.6f)
                    {
                        if (base.isAuthority)
                        {
                            foreach (HurtBox i in h.hurtBoxGroup.hurtBoxes)
                            {
                                if (i.isSniperTarget)
                                {
                                    this.hunk.targetHurtbox = i;
                                }
                            }
                        }
                        return true;
                    }
                }
            }

            Collider[] array = Physics.OverlapSphere(characterBody.corePosition, checkRadius * 0.75f, LayerIndex.projectile.mask);

            for (int i = 0; i < array.Length; i++)
            {
                ProjectileController pc = array[i].GetComponentInParent<ProjectileController>();
                if (pc)
                {
                    if (pc.teamFilter.teamIndex != characterBody.teamComponent.teamIndex)
                    {
                        if (base.isAuthority)
                        {
                            if (pc.owner)
                            {
                                HealthComponent hc = pc.owner.GetComponent<HealthComponent>();
                                if (hc) this.hunk.targetHurtbox = hc.body.mainHurtBox;
                            }
                        }
                        return true;
                    }
                }
            }

            foreach (Modules.Components.HunkProjectileTracker i in MainPlugin.projectileList)
            {
                if (i && Vector3.Distance(i.transform.position, this.transform.position) <= this.checkRadius * 0.75f)
                {
                    return true;
                }
            }

            foreach (Modules.Components.GolemLaser i in Modules.Survivors.Hunk.golemLasers)
            {
                if (i && Vector3.Distance(i.endPoint, this.transform.position) <= this.checkRadius * 0.5f)
                {
                    if (base.isAuthority)
                    {
                        if (i.characterBody) this.hunk.targetHurtbox = i.characterBody.mainHurtBox;
                    }
                    return true;
                }
            }

            return false;
        }

        public override void OnExit()
        {
            base.OnExit();

            this.hunk.isRolling = false;
            this.hunk.immobilized = false;
            this.skillLocator.primary.UnsetSkillOverride(this, Modules.Survivors.Hunk.scepterCounterSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            if (!this.removedInvis)
            {
                //this.removedInvis = true;
                //this.characterModel.invisibilityCount--;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!this.hasBlinked)
            {
                this.skillLocator.secondary.stock = 0;
                this.skillLocator.secondary.rechargeStopwatch = 0f;
                this.characterMotor.velocity *= 0.25f;
                this.characterMotor.rootMotion = Vector3.zero;
            }
            else
            {
                this.skillLocator.secondary.stock = 1;
            }

            if (base.fixedAge >= this.duration * 0.25f)
            {
                /*if (!this.startedInvis)
                {
                    this.startedInvis = true;
                    this.characterModel.invisibilityCount++;
                }*/
            }
            else
            {
                if (base.fixedAge > 0.05f)
                {
                    if (!this.success)
                    {
                        if (this.SearchAttacker())
                        {
                            this.success = true;
                            this.skillLocator.utility.AddOneStock();
                            this.skillLocator.utility.stock = this.skillLocator.utility.maxStock;
                            this.hunk.TriggerDodge();
                            base.fixedAge = this.duration * 0.25f;
                            if (base.isAuthority) Util.PlaySound("sfx_hunk_dodge_perfect", this.gameObject);
                        }
                    }
                }
            }

            if (!this.hasBlinked && base.fixedAge >= this.duration * 0.25f)
            {
                this.hasBlinked = true;
                this.Blink();

                base.PlayAnimation("FullBody, Override", "UrostepEnd", "Dodge.playbackRate", 0.8f);

                if (!this.removedInvis)
                {
                    //this.removedInvis = true;
                    //this.characterModel.invisibilityCount--;
                }

                EffectManager.SpawnEffect(this.blinkEffect, new EffectData
                {
                    origin = this.characterBody.corePosition,
                    rotation = Util.QuaternionSafeLookRotation(this.storedVector, Vector3.up),
                    scale = 1f
                }, false);
            }

            if (base.fixedAge >= 0.5f * this.duration)
            {
                this.animator.SetBool("canCancel", true);
                this.hunk.immobilized = false;
                if (base.isAuthority)
                {
                    if (!this.inputBank.skill1.down)
                    {
                        if (this.inputBank.moveVector != Vector3.zero || this.inputBank.jump.justPressed)
                        {
                            this.outer.SetNextStateToMain();
                            return;
                        }
                    }
                }
            }
            else
            {
                this.hunk.immobilized = true;
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        protected virtual void Blink()
        {
            Util.PlaySound("sfx_hunk_urostep", this.gameObject);
            base.characterMotor.velocity = Vector3.zero;

            float blinkDistance = 10f;
            Vector3 desiredPosition = this.transform.position + (this.storedVector * blinkDistance);
            desiredPosition.y = this.transform.position.y;

            RaycastHit raycastHit = default(RaycastHit);
            if (Physics.Raycast(new Ray(desiredPosition, Vector3.down), out raycastHit, 7f, LayerIndex.world.mask, QueryTriggerInteraction.Collide))
            {
                desiredPosition = raycastHit.point + new Vector3(0f, -0.1f, 0f);
            }

            RaycastHit wallHit = new RaycastHit();
            if (Physics.Linecast(this.characterBody.corePosition, desiredPosition, out wallHit, 1 << LayerMask.NameToLayer("World")))
            {
                Vector3 wallPos = wallHit.point;
                wallPos += (wallHit.normal * this.characterMotor.capsuleRadius);
                desiredPosition = wallPos;
            }

            this.characterMotor.velocity = this.storedVector * 8f;

            if (this.characterMotor && base.isAuthority)
            {
                this.characterMotor.Motor.SetPosition(desiredPosition);
            }

            EffectManager.SpawnEffect(this.blinkEffect, new EffectData
            {
                origin = desiredPosition + (0.75f * Vector3.up),
                rotation = Util.QuaternionSafeLookRotation(this.storedVector, Vector3.up),
                scale = 1f
            }, false);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge >= 0.4f * this.duration) return InterruptPriority.Any;
            return InterruptPriority.Frozen;
        }
    }
}