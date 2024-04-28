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

            this.previousAirControl = base.characterMotor.airControl;
            base.characterMotor.airControl = EntityStates.Croco.Leap.airControl;

            Vector3 direction = base.GetAimRay().direction;

            base.characterDirection.moveVector = direction;

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
                    Vector3 a = direction.normalized * (0.35f * EntityStates.Croco.Leap.aimVelocity) * this.moveSpeedStat;
                    Vector3 b = Vector3.up * 0.75f * EntityStates.Croco.Leap.upwardVelocity;
                    Vector3 b2 = new Vector3(direction.x, 0f, direction.z).normalized * (1.1f * EntityStates.Croco.Leap.forwardVelocity);

                    base.characterMotor.Motor.ForceUnground();
                    base.characterMotor.velocity = a + b + b2;
                }
            }
            else
            {
                base.PlayCrossfade("FullBody, Override", "AirDodgePerfect", 0.05f);
                if (NetworkServer.active) this.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.5f);

                this.success = true;
                this.hunk.lockOnTimer = 1.5f;
                this.hunk.TriggerDodge();

                if (base.isAuthority)
                {
                    Util.PlaySound("sfx_hunk_dodge_success", this.gameObject);
                    base.characterBody.isSprinting = true;

                    direction.y = Mathf.Max(direction.y, 1.05f * EntityStates.Croco.Leap.minimumY);
                    Vector3 a = direction.normalized * (0.75f * EntityStates.Croco.Leap.aimVelocity) * this.moveSpeedStat;
                    Vector3 b = Vector3.up * 0.75f * EntityStates.Croco.Leap.upwardVelocity;
                    Vector3 b2 = new Vector3(direction.x, 0f, direction.z).normalized * (1.25f * EntityStates.Croco.Leap.forwardVelocity);

                    base.characterMotor.Motor.ForceUnground();
                    base.characterMotor.velocity = a + b + b2;
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
            base.StartAimMode(0.5f, false);
            this.stopwatch += Time.fixedDeltaTime;

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