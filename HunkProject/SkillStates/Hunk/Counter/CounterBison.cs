using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;

namespace HunkMod.SkillStates.Hunk.Counter
{
    public class CounterBison : BaseHunkSkillState
    {
        protected override bool hideGun => true;
        protected override bool turningAllowed => false;

        public float duration = 1.4f;
        private HealthComponent target;
        public GameObject targetObject;

        public float lerpSpeed = 5f;

        private Vector3 lookVector;
        private Vector3 targetPos;
        private Vector3 desiredPos;
        private bool hasSettled;
        private GameObject knifeInstance;
        private float bisonTurnSpeed;

        public override void OnEnter()
        {
            base.OnEnter();
            this.target = this.targetObject.GetComponent<HealthComponent>();
            this.characterMotor.velocity = Vector3.zero;
            this.targetPos = this.target.transform.position;
            this.lerpSpeed *= this.attackSpeedStat;
            this.duration /= this.attackSpeedStat;
            this.hunk.counterTimer = this.duration;

            if (NetworkServer.active)
            {
                foreach (EntityStateMachine i in this.target.GetComponents<EntityStateMachine>())
                {
                    if (i.customName == "Body") i.SetNextState(new FakeStun
                    {
                        duration = this.duration
                    });
                    else i.SetNextStateToMain();
                }
            }

            this.bisonTurnSpeed = this.target.body.characterDirection.turnSpeed;

            Vector3 fakePos = this.targetPos;
            fakePos.y = this.transform.position.y;
            this.lookVector = (this.transform.position - fakePos).normalized;
            this.hunk.immobilized = true;

            base.PlayAnimation("FullBody, Override", "CounterBison", "Dodge.playbackRate", this.duration);

            if (NetworkServer.active)
            {
                this.hunk.iFrames = this.duration;
            }

            this.desiredPos = this.targetPos + new Vector3(0f, 1.5f, 0f);

            base.gameObject.layer = LayerIndex.noCollision.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();

            Util.PlaySound("sfx_hunk_snap_foley", this.gameObject);
        }

        public override void OnExit()
        {
            if (this.knifeInstance) Destroy(this.knifeInstance);

            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
            this.characterMotor.jumpCount = 1;
            this.hunk.iFrames = 0.25f;

            if (this.target)
            {
                if (NetworkServer.active)
                {
                    foreach (EntityStateMachine i in this.target.GetComponents<EntityStateMachine>())
                    {
                        if (i.customName == "Body") i.SetNextState(new BisonCharge
                        {
                            ownerBody = this.characterBody
                        });
                        else i.SetNextStateToMain();
                    }
                }

                this.target.body.characterDirection.turnSpeed = this.bisonTurnSpeed;
            }

            base.OnExit();

            this.hunk.immobilized = false;
            if (this.inputBank.moveVector != Vector3.zero) this.characterBody.isSprinting = true;
        }

        public override void FixedUpdate()
        {
            this.characterMotor.velocity = Vector3.zero;
            this.characterMotor.moveDirection = Vector3.zero;

            base.FixedUpdate();
            this.characterBody.aimTimer = -1f;
            this.hunk.reloadTimer = 1f;
            this.characterMotor.jumpCount = this.characterBody.maxJumpCount;
            this.characterBody.isSprinting = false;

            if (base.isAuthority)
            {
                if (base.fixedAge >= this.duration)
                {
                    this.outer.SetNextState(new GenericCounterAirDodge
                    {
                        overrideVelocity = new Vector3(0f, 4f, 0f)
                    });
                    return;
                }

                this.characterMotor.Motor.SetPosition(Vector3.Lerp(this.transform.position, this.desiredPos, Time.fixedDeltaTime * this.lerpSpeed));
            }

            if (!this.hasSettled)
            {
                this.characterDirection.moveVector = -this.lookVector;
                this.characterDirection.forward = -this.lookVector;
            }

            if (this.target && NetworkServer.active)
            {
                this.target.body.characterMotor.velocity = Vector3.zero;
                this.target.body.characterMotor.moveDirection = Vector3.zero;
                this.target.body.characterMotor.Motor.SetPosition(this.targetPos);

                if (this.hasSettled) this.target.body.characterDirection.moveVector = this.lookVector;
                //if (!this.hasSettled) this.target.body.characterDirection.forward = this.lookVector;
                this.target.body.aimTimer = -1f;
            }

            if (base.fixedAge >= 0.9f * this.duration)
            {
                if (this.knifeInstance) Destroy(this.knifeInstance);
                if (base.isAuthority)
                {
                    if (this.inputBank.moveVector != Vector3.zero)
                    {
                        this.outer.SetNextState(new GenericCounterAirDodge
                        {
                            overrideVelocity = new Vector3(0f, 4f, 0f)
                        });
                        return;
                    }
                }
            }

            if (!this.hasSettled && base.fixedAge >= 0.3f * this.duration)
            {
                this.hasSettled = true;
                this.characterDirection.turnSpeed = this.hunk.baseTurnSpeed;
                if (this.target) this.target.body.characterDirection.turnSpeed = this.hunk.baseTurnSpeed;

                this.hunk.TriggerCounter();
            }

            if (this.hasSettled)
            {
                this.characterBody.SetAimTimer(0.1f);
                this.lookVector = this.characterDirection.forward;
            }

            if (base.fixedAge >= 0.85f * this.duration)
            {
                this.hunk.immobilized = false;
            }
            else
            {
                this.hunk.immobilized = true;
                this.skillLocator.secondary.stock = 0;
                this.skillLocator.secondary.rechargeStopwatch = -0.3f;
            }

            this.skillLocator.special.stock = 0;
            this.skillLocator.special.rechargeStopwatch = -0.3f;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            writer.Write(this.targetObject);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            this.targetObject = reader.ReadGameObject();
        }
    }
}