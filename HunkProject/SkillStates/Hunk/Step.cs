using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HunkMod.SkillStates.Hunk
{
    public class Step : BaseHunkSkillState
    {
        protected override bool turningAllowed => false;
        protected Vector3 slipVector = Vector3.zero;
        public float duration = 0.9f;
        //private Vector3 cachedForward;
        private float coeff = 24f;
        private bool slowFlag = false;
        private bool slowFlag2 = false;
        private bool slowFlag3 = false;

        public float checkRadius = 10f;
        private SphereSearch search;
        private List<HurtBox> hits;

        /*private CameraTargetParams.CameraParamsOverrideHandle camOverrideHandle;
        private CharacterCameraParamsData stepCameraParams = new CharacterCameraParamsData
        {
            idealLocalCameraPos = stepCameraPosition,
        };*/

        public static Vector3 stepCameraPosition = new Vector3(0.4f, 0.65f, -3.8f);//new Vector3(1.85f, 0.08f, -3.8f);

        public override void OnEnter()
        {
            base.OnEnter();
            this.hunk.isRolling = true;
            this.slipVector = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
            //this.cachedForward = this.characterDirection.forward;

            /*Animator anim = this.GetModelAnimator();

            Vector3 rhs = base.characterDirection ? base.characterDirection.forward : this.slipVector;
            Vector3 rhs2 = Vector3.Cross(Vector3.up, rhs);
            float num = Vector3.Dot(this.slipVector, rhs);
            float num2 = Vector3.Dot(this.slipVector, rhs2);
            anim.SetFloat("dashF", num);
            anim.SetFloat("dashR", num2);*/

            /*CameraTargetParams.CameraParamsOverrideRequest request = new CameraTargetParams.CameraParamsOverrideRequest
            {
                cameraParamsData = stepCameraParams,
                priority = 0f
            };

            camOverrideHandle = cameraTargetParams.AddParamsOverride(request, 0.75f);*/

            hits = new List<HurtBox>();
            search = new SphereSearch();
            search.mask = LayerIndex.entityPrecise.mask;
            search.radius = checkRadius;

            if (!this.SearchAttacker())
            {
                base.PlayAnimation("FullBody, Override", "DodgeFull", "Dodge.playbackRate", this.duration * 1.4f);
            }

            //base.PlayAnimation("Gesture, Override", "BufferEmpty");

            Util.PlaySound("sfx_hunk_step_foley", this.gameObject);

            EntityStateMachine.FindByCustomName(this.gameObject, "Aim").SetNextStateToMain();

            this.skillLocator.primary.stock = 0;
            this.skillLocator.primary.rechargeStopwatch = -0.3f;

            this.skillLocator.secondary.stock = 0;
            this.skillLocator.secondary.rechargeStopwatch = -0.3f;

            this.hunk.immobilized = true;

            this.CreateDashEffect();
        }

        public virtual void CreateDashEffect()
        {
            /*EffectData effectData = new EffectData();
            effectData.rotation = Util.QuaternionSafeLookRotation(this.slipVector);
            effectData.origin = base.characterBody.corePosition;

            EffectManager.SpawnEffect(Modules.Assets.dashFX, effectData, false);*/
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
                            Roll nextState = new Roll();

                            foreach (HurtBox i in h.hurtBoxGroup.hurtBoxes)
                            {
                                if (i.isSniperTarget)
                                {
                                    this.hunk.targetHurtbox = i;
                                }
                            }

                            outer.SetNextState(nextState);
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
                            Roll nextState = new Roll();

                            if (pc.owner)
                            {
                                HealthComponent hc = pc.owner.GetComponent<HealthComponent>();
                                if (hc)
                                {
                                    foreach (HurtBox j in hc.body.hurtBoxGroup.hurtBoxes)
                                    {
                                        if (j.isSniperTarget)
                                        {
                                            this.hunk.targetHurtbox = j;
                                        }
                                    }
                                    this.hunk.targetHurtbox = hc.body.mainHurtBox;
                                }
                            }

                            outer.SetNextState(nextState);
                        }
                        return true;
                    }
                }
            }

            foreach (Modules.Components.HunkProjectileTracker i in MainPlugin.projectileList)
            {
                if (i && Vector3.Distance(i.transform.position, this.transform.position) <= this.checkRadius * 0.5f)
                {
                    if (base.isAuthority)
                    {
                        Roll nextState = new Roll();

                        /*ProjectileGhostController ghost = i.GetComponent<ProjectileGhostController>();
                        if (ghost)
                        {
                            if (ghost.predictionTransform)

                        }*/
                        // apparently ghosts don't even have a reference to the actual projectile controller
                        // i can't be bothered man

                        outer.SetNextState(nextState);
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
                        Roll nextState = new Roll();

                        if (i.characterBody)
                        {
                            foreach (HurtBox j in i.characterBody.hurtBoxGroup.hurtBoxes)
                            {
                                if (j.isSniperTarget)
                                {
                                    this.hunk.targetHurtbox = j;
                                }
                            }
                        }

                        outer.SetNextState(nextState);
                    }
                    return true;
                }
            }

            return false;
        }

        public override void FixedUpdate()
        {
            if (this.slowFlag2 && base.isAuthority)
            {
                this.characterMotor.jumpCount = 0;
                if (this.inputBank.jump.justPressed)
                {
                    base.PlayCrossfade("FullBody, Override", "BufferEmpty", 0.05f);
                    this.characterMotor.Motor.ForceUnground();
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
            else
            {
                this.characterMotor.jumpCount = this.characterBody.maxJumpCount;
            }

            base.FixedUpdate();
            this.characterBody.aimTimer = -1f;
            this.hunk.reloadTimer = 1f;
            this.inputBank.moveVector = Vector3.zero;
            base.characterMotor.velocity = Vector3.zero;
            base.characterMotor.rootMotion = this.slipVector * (this.coeff * Time.fixedDeltaTime) * Mathf.Cos(base.fixedAge / this.duration * 1.57079637f);

            if (base.isAuthority)
            {
                if (base.characterDirection)
                {
                    //base.characterDirection.forward = this.cachedForward;
                    base.characterDirection.forward = this.slipVector;
                }
            }

            if (!slowFlag2 && base.fixedAge > 0.05f)
            {
                if (base.isAuthority)
                {
                    SearchAttacker();
                }
            }

            // this feels fucking stupid but idc enough to refactor
            if (!this.slowFlag && base.fixedAge >= (0.05f * this.duration))
            {
                this.slowFlag = true;
                this.coeff = 7f;
            }

            if (!this.slowFlag2 && base.fixedAge >= (0.25f * this.duration))
            {
                this.slowFlag2 = true;
                Util.PlaySound("sfx_hunk_step", this.gameObject);
                this.coeff = 14f;
            }
            if (!this.slowFlag3 && base.fixedAge >= (0.85f * this.duration))
            {
                this.slowFlag3 = true;
                this.coeff = 4f;
                this.hunk.immobilized = false;
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public virtual void DampenVelocity()
        {
            base.characterMotor.velocity *= 0.75f;
        }

        public override void OnExit()
        {
            this.DampenVelocity();
            this.hunk.isRolling = false;
            this.hunk.immobilized = false;
            this.characterMotor.jumpCount = 0;
            this.hunk.desiredYOffset = this.hunk.defaultYOffset;

            base.OnExit();

            if (base.isAuthority && this.inputBank.moveVector != Vector3.zero) this.characterBody.isSprinting = true;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge <= 0.15f) return InterruptPriority.Frozen;
            if (base.fixedAge >= 0.5f) return InterruptPriority.Any;
            return InterruptPriority.Frozen;
        }
    }
}