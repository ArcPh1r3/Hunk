using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;
using static RoR2.CameraTargetParams;
using R2API.Networking;
using R2API.Networking.Interfaces;

namespace HunkMod.SkillStates.Hunk.Counter
{
    public class CounterKnee : BaseHunkSkillState
    {
        protected override bool hideGun => true;
        protected override bool turningAllowed => false;

        public float duration = 2.4f;
        private HealthComponent target;
        public GameObject targetObject;

        public float lerpSpeed = 6f;

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
            this.animator.SetBool("isMoving", false);
            this.characterMotor.velocity = Vector3.zero;
            this.target = this.targetObject.GetComponent<HealthComponent>();
            this.targetPos = this.target.transform.position;
            this.lerpSpeed *= this.attackSpeedStat;
            this.duration /= this.attackSpeedStat;

            if (NetworkServer.active)
            {
                foreach (EntityStateMachine i in this.target.GetComponents<EntityStateMachine>())
                {
                    if (i.customName == "Body") i.SetNextState(new EntityStates.StunState());
                    else i.SetNextStateToMain();
                }
            }

            if (!this.camParamsOverrideHandle.isValid) this.camParamsOverrideHandle = Modules.CameraParams.OverrideCameraParams(base.cameraTargetParams, HunkCameraParams.MELEE, 0.25f);

            Vector3 fakePos = this.targetPos;
            fakePos.y = this.transform.position.y;
            this.lookVector = (this.transform.position - fakePos).normalized;
            this.hunk.immobilized = true;

            base.PlayAnimation("FullBody, Override", "CounterKnee", "Dodge.playbackRate", this.duration);

            if (NetworkServer.active)
            {
                this.hunk.iFrames = this.duration;
            }

            this.desiredPos = this.targetPos + (this.lookVector * 1.4f);

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

            this.hunk.immobilized = false;
            this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);

            if (NetworkServer.active)
            {
                this.characterBody.RemoveBuff(RoR2Content.Buffs.ArmorBoost);
            }

            /*if (base.fixedAge <= (0.8f * this.duration))
            {
                base.PlayAnimation("FullBody, Override", "BufferEmpty");
            }*/

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

            if (this.target && NetworkServer.active)
            {
                this.target.body.characterMotor.velocity = Vector3.zero;
                this.target.body.characterMotor.moveDirection = Vector3.zero;
                this.target.body.characterMotor.Motor.SetPosition(this.targetPos);

                this.target.body.characterDirection.moveVector = this.lookVector;
                this.target.body.characterDirection.forward = this.lookVector;
                this.target.body.aimTimer = -1f;
            }

            if (base.fixedAge >= 0.75f * this.duration)
            {
                this.aimWeight = Mathf.Lerp(this.aimWeight, 1f, Time.fixedDeltaTime * 0.5f);

                if (base.isAuthority)
                {
                    if (this.inputBank.moveVector != Vector3.zero)
                    {
                        this.skillLocator.secondary.rechargeStopwatch = 1f;
                        base.PlayAnimation("Body", "Sprint");
                        this.outer.SetNextStateToMain();
                        return;
                    }
                }
            }
            else
            {
                this.aimWeight = Mathf.Lerp(this.aimWeight, 0f, Time.fixedDeltaTime * 5f);
            }

            if (!this.hasSnapped && base.fixedAge >= 0.23f * this.duration)
            {
                this.hasSnapped = true;
                this.hunk.immobilized = false;
                this.hunk.iFrames = 0.25f;

                //Util.PlaySound("sfx_hunk_snap", this.gameObject);

                if (this.target)
                {
                    if (base.isAuthority)
                    {
                        float recoil = 16f;
                        base.AddRecoil2(-1f * recoil, -2f * recoil, -0.5f * recoil, 0.5f * recoil);
                    }

                    this.hunk.TriggerCounter();

                    EffectManager.SpawnEffect(Modules.Assets.kickImpactEffect, new EffectData
                    {
                        origin = this.target.body.corePosition,
                        scale = 1f,
                        rotation = Quaternion.identity
                    }, false);

                    Util.PlaySound("sfx_hunk_kick_impact", this.gameObject);

                    if (NetworkServer.active)
                    {
                        /*this.target.gameObject.AddComponent<Modules.Components.HunkKnifeTracker>();
                        this.target.gameObject.AddComponent<Modules.Components.HunkHeadshotTracker>();

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

                        this.characterBody.AddBuff(RoR2Content.Buffs.ArmorBoost);*/
                    }

                    if (base.isAuthority)
                    {
                        BlastAttack blastAttack = new BlastAttack();
                        blastAttack.radius = 5f;
                        blastAttack.procCoefficient = 1f;
                        blastAttack.position = this.target.body.corePosition;
                        blastAttack.attacker = this.gameObject;
                        blastAttack.crit = this.RollCrit();
                        blastAttack.baseDamage = this.damageStat * 20f;
                        blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                        blastAttack.baseForce = 0f;
                        blastAttack.bonusForce = this.GetAimRay().direction * 1000f + (Vector3.up * 2000f);
                        blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                        blastAttack.damageType = DamageType.Stun1s | DamageType.ClayGoo;
                        blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;

                        // spawn hit effect on targets
                        BlastAttack.HitPoint[] hitPoints = blastAttack.CollectHits();
                        if (hitPoints.Length > 0)
                        {
                            foreach (BlastAttack.HitPoint i in hitPoints)
                            {
                                if (i.hurtBox)
                                {
                                    if (i.hurtBox.healthComponent)
                                    {
                                        NetworkIdentity identity = this.GetComponent<NetworkIdentity>();
                                        if (identity) new Modules.Components.SyncHeadshot(identity.netId, i.hurtBox.healthComponent.gameObject).Send(NetworkDestination.Server);

                                        i.hurtBox.healthComponent.gameObject.AddComponent<Modules.Components.HunkHeadshotTracker>();
                                    }
                                }
                                EffectManager.SpawnEffect(Modules.Assets.kickImpactEffect, new EffectData
                                {
                                    origin = i.hitPosition,
                                    scale = 1f
                                }, true);
                            }
                        }

                        blastAttack.Fire();
                    }
                }
            }

            if (base.fixedAge >= 0.75f * this.duration)
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

            this.animator.SetLayerWeight(this.animator.GetLayerIndex("AimYaw"), this.aimWeight);
            this.animator.SetLayerWeight(this.animator.GetLayerIndex("AimPitch"), this.aimWeight);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge >= 0.34f * this.duration) return InterruptPriority.Skill;
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