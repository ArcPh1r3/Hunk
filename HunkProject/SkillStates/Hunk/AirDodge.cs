using UnityEngine;
using EntityStates;
using RoR2;
using System.Collections.Generic;
using RoR2.Projectile;
using UnityEngine.Networking;

namespace HunkMod.SkillStates.Hunk
{
    public class AirDodge : BaseHunkSkillState
    {
        protected Vector3 slipVector = Vector3.zero;
        private float stopwatch;
        private float previousAirControl;

        public float checkRadius = 10f;
        private SphereSearch search;
        private List<HurtBox> hits;
        private bool success;

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterMotor.jumpCount = base.characterBody.maxJumpCount;
            this.slipVector = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;

            this.previousAirControl = base.characterMotor.airControl;
            base.characterMotor.airControl = EntityStates.Croco.Leap.airControl;

            Vector3 direction = base.GetAimRay().direction;

            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

            hits = new List<HurtBox>();
            search = new SphereSearch();
            search.mask = LayerIndex.entityPrecise.mask;
            search.radius = checkRadius;

            if (!this.SearchAttacker())
            {
                base.PlayCrossfade("FullBody, Override", "AirDodge", 0.05f);

                if (base.isAuthority)
                {
                    base.characterBody.isSprinting = true;

                    direction.y = Mathf.Max(direction.y, 0.25f * EntityStates.Croco.Leap.minimumY);
                    Vector3 a = this.slipVector * (0.35f * EntityStates.Croco.Leap.aimVelocity) * 12f;
                    Vector3 b = Vector3.up * 0.75f * EntityStates.Croco.Leap.upwardVelocity;
                    Vector3 a2 = direction * (0.35f * EntityStates.Croco.Leap.aimVelocity) * 12f;
                    a2.x = 0f;
                    a2.z = 0f;
                    Vector3 b2 = new Vector3(direction.x, 0f, direction.z).normalized * (1.1f * EntityStates.Croco.Leap.forwardVelocity);

                    base.characterMotor.Motor.ForceUnground();
                    base.characterMotor.velocity = a + a2 + b + b2;
                }
            }
            else
            {
                base.PlayCrossfade("FullBody, Override", "AirDodgePerfect", 0.05f);
                this.hunk.iFrames = 0.5f;

                this.success = true;
                this.hunk.lockOnTimer = 1.5f;
                this.hunk.TriggerDodge();

                if (base.isAuthority)
                {
                    Util.PlaySound("sfx_hunk_dodge_success", this.gameObject);
                    base.characterBody.isSprinting = true;

                    direction.y = Mathf.Max(direction.y, 1.05f * EntityStates.Croco.Leap.minimumY);
                    Vector3 a = this.slipVector * (0.5f * EntityStates.Croco.Leap.aimVelocity) * 12f;
                    Vector3 b = Vector3.up * 0.75f * EntityStates.Croco.Leap.upwardVelocity;
                    Vector3 a2 = direction * (0.75f * EntityStates.Croco.Leap.aimVelocity) * 12f;
                    a2.x = 0f;
                    a2.z = 0f;
                    Vector3 b2 = new Vector3(direction.x, 0f, direction.z).normalized * (1.1f * EntityStates.Croco.Leap.forwardVelocity);

                    base.characterMotor.Motor.ForceUnground();
                    base.characterMotor.velocity = a + a2 + b + b2;
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
                    if (hp.body.outOfCombatStopwatch <= 1.4f)
                    {
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
                        return true;
                    }
                }
            }

            foreach (Modules.Components.GolemLaser i in Modules.Survivors.Hunk.golemLasers)
            {
                if (i && Vector3.Distance(i.endPoint, this.transform.position) <= this.checkRadius * 0.5f)
                {
                    return true;
                }
            }

            return false;
        }

        public override void OnExit()
        {
            base.OnExit();

            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.characterMotor.airControl = this.previousAirControl;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.characterBody.aimTimer = -1f;
            this.hunk.reloadTimer = 1f;
            this.stopwatch += Time.fixedDeltaTime;

            EntityStateMachine.FindByCustomName(this.gameObject, "Aim").SetNextStateToMain();

            this.skillLocator.primary.stock = 0;
            this.skillLocator.primary.rechargeStopwatch = -0.3f;

            this.skillLocator.secondary.stock = 0;
            this.skillLocator.secondary.rechargeStopwatch = -0.3f;

            if (this.stopwatch >= 0.1f && base.isAuthority && base.characterMotor.isGrounded)
            {
                if (this.success)
                {
                    this.outer.SetNextState(new PerfectLanding());
                }
                else
                {
                    this.outer.SetNextState(new SlowRoll());
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}