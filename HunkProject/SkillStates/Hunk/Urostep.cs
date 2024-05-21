using UnityEngine;
using RoR2;
using EntityStates;
using RoR2.Projectile;
using System.Collections.Generic;

namespace HunkMod.SkillStates.Hunk
{
    public class Urostep : BaseHunkSkillState
    {
        public float duration = 0.8f;

        private bool hasBlinked;
        private bool removedInvis;
        private CharacterModel characterModel;

        public float checkRadius = 10f;
        private SphereSearch search;
        private List<HurtBox> hits;

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
            this.hasBlinked = false;
            this.characterModel = this.GetModelTransform().GetComponent<CharacterModel>();

            base.PlayAnimation("Gesture, Override", "BufferEmpty");
            //base.PlayAnimation("FullBody, Override", "GreatswordDash", "Roll.playbackRate", this.duration * 1.5f);
            Util.PlaySound(this.soundString, this.gameObject);

            this.characterModel.invisibilityCount++;
            this.hunk.iFrames = this.duration;

            hits = new List<HurtBox>();
            search = new SphereSearch();
            search.mask = LayerIndex.entityPrecise.mask;
            search.radius = checkRadius;

            this.Blink();

            EntityStateMachine.FindByCustomName(this.gameObject, "Aim").SetNextStateToMain();
            this.skillLocator.secondary.stock = 0;
            this.skillLocator.secondary.rechargeStopwatch = 0f;

            this.skillLocator.primary.SetSkillOverride(this, Modules.Survivors.Hunk.scepterCounterSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            if (this.SearchAttacker())
            {
                this.skillLocator.utility.AddOneStock();
                this.skillLocator.utility.stock = this.skillLocator.utility.maxStock;
                this.hunk.TriggerDodge();
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
                            //Roll nextState = new Roll();

                            /*foreach (HurtBox i in h.hurtBoxGroup.hurtBoxes)
                            {
                                if (i.isSniperTarget)
                                {
                                    this.hunk.targetHurtbox = i;
                                }
                            }*/

                            //outer.SetNextState(nextState);
                        }
                        return true;
                    }
                }
            }

            Collider[] array = Physics.OverlapSphere(characterBody.corePosition, checkRadius * 0.5f, LayerIndex.projectile.mask);

            for (int i = 0; i < array.Length; i++)
            {
                ProjectileController pc = array[i].GetComponentInParent<ProjectileController>();
                if (pc)
                {
                    if (pc.teamFilter.teamIndex != characterBody.teamComponent.teamIndex)
                    {
                        if (base.isAuthority)
                        {
                            //Roll nextState = new Roll();
                            //outer.SetNextState(nextState);
                        }
                        return true;
                    }
                }
            }

            foreach (Modules.Components.HunkProjectileTracker i in MainPlugin.projectileList)
            {
                if (i && Vector3.Distance(i.transform.position, this.transform.position) <= this.checkRadius)
                {
                    if (base.isAuthority)
                    {
                        //Roll nextState = new Roll();
                        //outer.SetNextState(nextState);
                    }
                    return true;
                }
            }

            foreach (Modules.Components.GolemLaser i in Modules.Survivors.Hunk.golemLasers)
            {
                if (i && Vector3.Distance(i.endPoint, this.transform.position) <= this.checkRadius * 0.5f)
                {
                    if (base.isAuthority)
                    {
                        //Roll nextState = new Roll();
                        //outer.SetNextState(nextState);
                    }
                    return true;
                }
            }

            return false;
        }

        public override void OnExit()
        {
            base.OnExit();

            this.skillLocator.primary.UnsetSkillOverride(this, Modules.Survivors.Hunk.scepterCounterSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            if (!this.removedInvis)
            {
                this.removedInvis = true;
                this.characterModel.invisibilityCount--;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!this.hasBlinked)
            {
                this.skillLocator.secondary.stock = 0;
                this.skillLocator.secondary.rechargeStopwatch = 0f;
            }
            else
            {
                this.skillLocator.secondary.stock = 1;
            }

            if (!this.hasBlinked && base.fixedAge >= this.duration * 0.4f)
            {
                this.hasBlinked = true;

                if (!this.removedInvis)
                {
                    this.removedInvis = true;
                    this.characterModel.invisibilityCount--;
                }

                EffectManager.SpawnEffect(this.blinkEffect, new EffectData
                {
                    origin = this.characterBody.corePosition,
                    rotation = Quaternion.identity,
                    scale = 1f
                }, false);
            }

            if (base.fixedAge >= 0.75f * this.duration)
            {
                if (base.isAuthority)
                {
                    if (this.inputBank.moveVector != Vector3.zero || this.inputBank.jump.justPressed)
                    {
                        this.outer.SetNextStateToMain();
                        return;
                    }
                }
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        protected virtual void Blink()
        {
            base.characterMotor.velocity = Vector3.zero;
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            base.PlayAnimation("Gesture, Override", "BufferEmpty");

            float blinkDistance = 5f;
            Vector3 desiredPosition = this.transform.position + (Vector3.up * blinkDistance);

            /*if (!this.isGrounded)
            {
                // if airborne, blink down
                RaycastHit raycastHit = default(RaycastHit);
                if (Physics.Raycast(new Ray(this.transform.position, Vector3.down), out raycastHit, 1.25f * blinkDistance, LayerIndex.world.mask, QueryTriggerInteraction.Collide))
                {
                    desiredPosition = raycastHit.point + new Vector3(0f, -0.1f, 0f);
                }
                else desiredPosition = this.transform.position + (Vector3.up * -blinkDistance);
            }*/
            // nvm

            if (this.inputBank.moveVector != Vector3.zero && this.isGrounded)
            {
                desiredPosition = this.transform.position + (this.inputBank.moveVector * blinkDistance);
                desiredPosition.y = this.transform.position.y;

                RaycastHit raycastHit = default(RaycastHit);
                if (Physics.Raycast(new Ray(desiredPosition, Vector3.down), out raycastHit, 4f, LayerIndex.world.mask, QueryTriggerInteraction.Collide))
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

                this.characterMotor.velocity = this.inputBank.moveVector * 12f;
            }

            EffectManager.SpawnEffect(this.blinkEffect, new EffectData
            {
                origin = this.characterBody.corePosition,
                rotation = Quaternion.identity,
                scale = 1f
            }, false);

            if (this.characterMotor && base.isAuthority)
            {
                this.characterMotor.Motor.SetPosition(desiredPosition);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge >= 0.65f * this.duration) return InterruptPriority.Any;
            return InterruptPriority.Frozen;
        }
    }
}