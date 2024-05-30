using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;

namespace HunkMod.SkillStates.Hunk.Counter
{
    public class TemplarGrenade : BaseHunkSkillState
    {
        protected override bool hideGun => true;
        protected override bool turningAllowed => false;

        public float duration = 1.1f;
        private HealthComponent target;
        public GameObject targetObject;

        public float lerpSpeed = 2.5f;

        private Vector3 lookVector;
        private Vector3 targetPos;
        private Vector3 desiredPos;
        private bool hasDroppedGrenade;
        private bool hasDroppedPin;
        private GameObject grenadeInstance;

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
                    if (i.customName == "Body") i.SetNextState(new EntityStates.StunState
                    {
                        duration = this.duration
                    });
                    else i.SetNextStateToMain();
                }
            }

            Vector3 fakePos = this.targetPos;
            fakePos.y = this.transform.position.y;
            this.lookVector = (this.transform.position - fakePos).normalized;
            this.hunk.immobilized = true;

            base.PlayAnimation("FullBody, Override", "CounterTemplar", "Dodge.playbackRate", this.duration);

            if (NetworkServer.active)
            {
                this.hunk.iFrames = this.duration;
            }

            this.desiredPos = this.targetPos + new Vector3(0f, 3.8f, 0f) + (this.lookVector * 1.4f);

            base.gameObject.layer = LayerIndex.noCollision.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();

            Util.PlaySound("sfx_hunk_snap_foley", this.gameObject);

            this.grenadeInstance = GameObject.Instantiate(Modules.Assets.fragGrenade);
            this.grenadeInstance.transform.parent = this.FindModelChild("HandL");
            this.grenadeInstance.transform.localPosition = new Vector3(-8f, -4f, 4f);
            this.grenadeInstance.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
            this.grenadeInstance.transform.localScale = Vector3.one * 18f;
        }

        public override void OnExit()
        {
            if (this.grenadeInstance) Destroy(this.grenadeInstance);

            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
            this.characterMotor.jumpCount = 1;
            this.hunk.iFrames = 0.25f;

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
                    this.outer.SetNextState(new TemplarAirDodge
                    {
                        targetObject = this.targetObject
                    });
                    return;
                }

                this.characterMotor.Motor.SetPosition(Vector3.Lerp(this.transform.position, this.desiredPos, Time.fixedDeltaTime * this.lerpSpeed));
            }

            this.characterDirection.moveVector = -this.lookVector;
            this.characterDirection.forward = -this.lookVector;

            if (this.target && NetworkServer.active)
            {
                this.target.body.characterMotor.velocity = Vector3.zero;
                this.target.body.characterMotor.moveDirection = Vector3.zero;
                this.target.body.characterMotor.Motor.SetPosition(this.targetPos);

                this.target.body.characterDirection.moveVector = this.lookVector;
                this.target.body.characterDirection.forward = this.lookVector;
                this.target.body.aimTimer = -1f;
            }

            if (base.fixedAge >= 0.9f * this.duration)
            {
                if (this.grenadeInstance) Destroy(this.grenadeInstance);
                if (base.isAuthority)
                {
                    if (this.inputBank.moveVector != Vector3.zero)
                    {
                        this.outer.SetNextState(new TemplarAirDodge
                        {
                            targetObject = this.targetObject
                        });
                        return;
                    }
                }
            }

            if (!this.hasDroppedPin && base.fixedAge >= 0.54f * this.duration)
            {
                this.hasDroppedPin = true;

                if (this.grenadeInstance) this.grenadeInstance.GetComponent<ChildLocator>().FindChild("Pin").gameObject.SetActive(false);
            }

            if (!this.hasDroppedGrenade && base.fixedAge >= 0.6f * this.duration)
            {
                this.hasDroppedGrenade = true;

                this.desiredPos = this.targetPos + new Vector3(0f, 4f, 0f);
                this.lerpSpeed *= 2f;

                Util.PlaySound("sfx_hunk_grenade_roll", this.gameObject);
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