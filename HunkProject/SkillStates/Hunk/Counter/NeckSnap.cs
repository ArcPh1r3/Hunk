using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;
using static RoR2.CameraTargetParams;

namespace HunkMod.SkillStates.Hunk.Counter
{
    public class NeckSnap : BaseHunkSkillState
    {
        protected override bool hideGun => true;

        public float duration = 3f;
        public HealthComponent target;

        public float lerpSpeed = 4f;

        private Vector3 lookVector;
        private Vector3 targetPos;
        private Vector3 desiredPos;
        private bool hasSnapped;

        public CameraParamsOverrideHandle camParamsOverrideHandle;
        private Animator animator;
        private float aimWeight;

        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = this.GetModelAnimator();
            this.targetPos = this.target.transform.position;
            this.lerpSpeed *= this.attackSpeedStat;
            this.duration /= this.attackSpeedStat;

            if (!this.camParamsOverrideHandle.isValid) this.camParamsOverrideHandle = Modules.CameraParams.OverrideCameraParams(base.cameraTargetParams, HunkCameraParams.MELEE, 0.25f);

            Vector3 fakePos = this.targetPos;
            fakePos.y = this.transform.position.y;
            this.lookVector = (this.transform.position - fakePos).normalized;

            base.PlayAnimation("FullBody, Override", "NeckSnap", "Dodge.playbackRate", this.duration);

            if (NetworkServer.active)
            {
                this.hunk.iFrames = this.duration;
                this.characterBody.AddBuff(Modules.Survivors.Hunk.immobilizedBuff);
            }

            this.desiredPos = this.targetPos + (this.lookVector * this.hunk.snapOffset);

            base.gameObject.layer = LayerIndex.fakeActor.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();

            Util.PlaySound("sfx_hunk_snap_foley", this.gameObject);
        }

        public override void OnExit()
        {
            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
            this.characterMotor.jumpCount = 0;

            base.OnExit();

            this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);

            if (NetworkServer.active)
            {
                this.characterBody.RemoveBuff(Modules.Survivors.Hunk.immobilizedBuff);
                this.characterBody.RemoveBuff(RoR2Content.Buffs.ArmorBoost);
            }

            if (base.fixedAge <= (0.8f * this.duration))
            {
                base.PlayAnimation("FullBody, Override", "BufferEmpty");
            }

            this.animator.SetLayerWeight(this.animator.GetLayerIndex("AimYaw"), 1f);
            this.animator.SetLayerWeight(this.animator.GetLayerIndex("AimPitch"), 1f);

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

            // failsafe
            if (!this.characterBody.HasBuff(Modules.Survivors.Hunk.immobilizedBuff) && base.fixedAge <= 0.2f)
            {
                if (NetworkServer.active)
                {
                    this.characterBody.AddBuff(Modules.Survivors.Hunk.immobilizedBuff);
                }
            }

            if (base.isAuthority)
            {
                if (base.fixedAge >= this.duration)
                {
                    this.outer.SetNextStateToMain();
                }

                this.characterMotor.Motor.SetPosition(Vector3.Lerp(this.transform.position, this.desiredPos, Time.fixedDeltaTime * this.lerpSpeed));
            }

            this.characterDirection.moveVector = -this.lookVector;
            this.characterDirection.forward = -this.lookVector;

            if (this.target)
            {
                this.target.body.characterMotor.velocity = Vector3.zero;
                this.target.body.characterMotor.moveDirection = Vector3.zero;
                this.target.body.characterMotor.Motor.SetPosition(this.targetPos);

                this.target.body.characterDirection.moveVector = this.lookVector;
                this.target.body.characterDirection.forward = this.lookVector;
                this.target.body.aimTimer = -1f;
            }

            if (base.fixedAge >= 0.65f * this.duration)
            {
                this.aimWeight = Mathf.Lerp(this.aimWeight, 1f, Time.fixedDeltaTime * 0.5f);
            }
            else
            {
                this.aimWeight = Mathf.Lerp(this.aimWeight, 0f, Time.fixedDeltaTime * 5f);
            }

            if (!this.hasSnapped && base.fixedAge >= 0.455f * this.duration)
            {
                this.hasSnapped = true;

                Util.PlaySound("sfx_hunk_snap", this.gameObject);

                if (this.target)
                {
                    if (base.isAuthority)
                    {
                        float recoil = 16f;
                        base.AddRecoil2(-1f * recoil, -2f * recoil, -0.5f * recoil, 0.5f * recoil);
                    }

                    this.hunk.iFrames = 0f;

                    if (NetworkServer.active)
                    {
                        this.target.gameObject.AddComponent<Modules.Components.HunkKnifeTracker>();

                        this.target.TakeDamage(new DamageInfo
                        {
                            attacker = this.gameObject,
                            canRejectForce = false,
                            crit = false,
                            damage = this.target.fullCombinedHealth,
                            damageColorIndex = DamageColorIndex.DeathMark,
                            damageType = DamageType.BypassArmor,
                            force = Vector3.zero,
                            inflictor = this.gameObject,
                            position = this.target.transform.position,
                            procChainMask = default(ProcChainMask),
                            procCoefficient = 1f
                        });

                        this.characterBody.AddBuff(RoR2Content.Buffs.ArmorBoost);
                    }
                }
            }

            if (base.fixedAge >= 0.75f * this.duration)
            {
                if (this.characterBody.HasBuff(Modules.Survivors.Hunk.immobilizedBuff))
                {
                    if (NetworkServer.active) this.characterBody.RemoveBuff(Modules.Survivors.Hunk.immobilizedBuff);
                }
            }
            else
            {
                this.skillLocator.secondary.stock = 0;
                this.skillLocator.secondary.rechargeStopwatch = -0.3f;
            }

            this.skillLocator.special.stock = 0;
            this.skillLocator.special.rechargeStopwatch = -0.3f;

            this.animator.SetLayerWeight(this.animator.GetLayerIndex("AimYaw"), this.aimWeight);
            this.animator.SetLayerWeight(this.animator.GetLayerIndex("AimPitch"), this.aimWeight);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge >= 0.6f * this.duration) return InterruptPriority.Skill;
            return InterruptPriority.Frozen;
        }
    }
}