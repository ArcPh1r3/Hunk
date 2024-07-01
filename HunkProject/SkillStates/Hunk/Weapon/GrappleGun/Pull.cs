using UnityEngine;
using RoR2;
using EntityStates;

namespace HunkMod.SkillStates.Hunk.Weapon.GrappleGun
{
    public class Pull : BaseHunkSkillState
    {
        public float baseSpeed = 50f;
        public Vector3 hookPoint;
        private LineRenderer wire;
        private uint playID;

        public override void OnEnter()
        {
            base.OnEnter();
            this.wire = this.FindModelChild("Wire").GetComponent<LineRenderer>();
            this.playID = Util.PlaySound("sfx_hunk_grapple_loop", base.gameObject);
        }

        public override void OnExit()
        {
            base.OnExit();
            if (this.wire) this.wire.gameObject.SetActive(false);
            AkSoundEngine.StopPlayingID(this.playID);
        }

        public override void Update()
        {
            base.Update();

            if (this.wire)
            {
                this.wire.SetPosition(0, this.wire.transform.position);
                this.wire.SetPosition(1, this.hookPoint);
                this.wire.gameObject.SetActive(true);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.wire)
            {
                this.wire.SetPosition(0, this.wire.transform.position);
                this.wire.SetPosition(1, this.hookPoint);
                this.wire.gameObject.SetActive(true);
            }

            if (base.isAuthority)
            {
                this.characterMotor.Motor.ForceUnground();
                this.characterMotor.rootMotion = Vector3.zero;
                this.characterMotor.velocity = (this.hookPoint - this.transform.position).normalized * this.baseSpeed;

                if (Vector3.Distance(this.transform.position, this.hookPoint) <= 8f)
                {
                    this.outer.SetNextState(new Counter.GenericCounterAirDodge
                    {
                        overrideVelocity = this.characterMotor.velocity,
                        fastLanding = true,
                        resetCooldown = false,
                        allowCounter = true
                    });
                }

                if (this.inputBank.jump.justPressed)
                {
                    this.outer.SetNextState(new Counter.GenericCounterAirDodge
                    {
                        overrideVelocity = new Vector3(0f, 5f, 0f) + (this.characterMotor.velocity * 0.4f),
                        fastLanding = true,
                        resetCooldown = false,
                        allowCounter = true
                    });
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}