using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;
using RoR2.Skills;

namespace HunkMod.SkillStates.Hunk.Counter
{
    public class GolemStab : BaseHunkSkillState
    {
        protected override bool hideGun => true;
        protected override bool turningAllowed => false;

        public float duration = 1.7f;
        private HealthComponent target;
        public GameObject targetObject;

        public float lerpSpeed = 3.75f;

        private Vector3 lookVector;
        private Vector3 targetPos;
        private Vector3 desiredPos;
        private bool hasStabbed;
        private GameObject knifeInstance;
        private bool knifeHidden;

        public override void OnEnter()
        {
            this.hunk = this.GetComponent<Modules.Components.HunkController>();

            SkillDef knifeSkinDef = this.hunk.knifeSkin;
            if (knifeSkinDef)
            {
                if (Modules.Survivors.Hunk.knifeSkins.ContainsKey(knifeSkinDef))
                {
                    this.prop = "";
                    this.knifeInstance = this.CreateKnife(Modules.Survivors.Hunk.knifeSkins[knifeSkinDef]);
                }
                else
                {
                    // hidden blade is the only case for now.
                    this.prop = "HiddenKnifeModel";
                }
            }

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

            Vector3 fakePos = this.targetPos;
            fakePos.y = this.transform.position.y;
            this.lookVector = (this.transform.position - fakePos).normalized;
            this.hunk.immobilized = true;

            base.PlayAnimation("FullBody, Override", "CounterGolem", "Dodge.playbackRate", this.duration);

            if (NetworkServer.active)
            {
                this.hunk.iFrames = this.duration;
            }

            this.desiredPos = this.targetPos + new Vector3(0f, 3.3f, 0f) + (this.lookVector * this.hunk.snapOffset);

            base.gameObject.layer = LayerIndex.noCollision.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();

            Util.PlaySound("sfx_hunk_snap_foley", this.gameObject);
        }

        public override void OnExit()
        {
            if (!this.knifeHidden)
            {
                this.knifeHidden = true;
                if (this.prop != "") this.FindModelChild(this.prop).gameObject.SetActive(false);
                if (this.knifeInstance) Destroy(this.knifeInstance);
            }

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
                    this.outer.SetNextState(new GenericCounterAirDodge());
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
                if (this.knifeInstance) Destroy(this.knifeInstance);
                if (base.isAuthority)
                {
                    if (this.inputBank.moveVector != Vector3.zero)
                    {
                        this.outer.SetNextState(new GenericCounterAirDodge());
                        return;
                    }
                }
            }

            if (!this.hasStabbed && base.fixedAge >= 0.55f * this.duration)
            {
                this.hasStabbed = true;

                if (this.target)
                {
                    this.target.gameObject.AddComponent<Modules.Components.GolemEyeDisabler>();

                    if (base.isAuthority)
                    {
                        float recoil = 16f;
                        base.AddRecoil2(-1f * recoil, -2f * recoil, -0.5f * recoil, 0.5f * recoil);
                    }

                    this.hunk.TriggerCounter();

                    EffectManager.SpawnEffect(Modules.Assets.knifeImpactEffectRed, new EffectData
                    {
                        origin = this.target.body.corePosition,
                        scale = 1f,
                        rotation = Quaternion.identity
                    }, false);

                    Util.PlaySound("sfx_hunk_knife_hit", this.gameObject);

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
                        blastAttack.bonusForce = this.GetAimRay().direction * 500f + (Vector3.up * 200f);
                        blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                        blastAttack.damageType = DamageType.Stun1s | DamageType.ClayGoo;
                        blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;

                        // spawn hit effect on targets
                        BlastAttack.HitPoint[] hitPoints = blastAttack.CollectHits();
                        if (hitPoints.Length > 0)
                        {
                            foreach (BlastAttack.HitPoint i in hitPoints)
                            {
                                EffectManager.SpawnEffect(Modules.Assets.knifeImpactEffectRed, new EffectData
                                {
                                    origin = i.hitPosition,
                                    scale = 1f
                                }, true);
                            }
                        }

                        blastAttack.Fire();
                    }

                    this.desiredPos = this.targetPos + new Vector3(0f, 4f, 0f) + (this.lookVector * this.hunk.snapOffset);
                }

                /*if (!this.hasDroppedGrenade && base.fixedAge >= 0.6f * this.duration)
                {
                    this.hasDroppedGrenade = true;

                    this.desiredPos = this.targetPos + new Vector3(0f, 4f, 0f);
                    this.lerpSpeed *= 2f;

                    Util.PlaySound("sfx_hunk_grenade_roll", this.gameObject);
                }*/

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

        private GameObject CreateKnife(GameObject modelPrefab)
        {
            if (this.hunk.variant) return null;

            GameObject newKnife = GameObject.Instantiate(modelPrefab);

            newKnife.transform.parent = this.FindModelChild("KnifeBase");
            newKnife.transform.localPosition = Vector3.zero;
            newKnife.transform.localRotation = Quaternion.identity;
            newKnife.transform.localScale = Vector3.one;

            return newKnife;
        }
    }
}