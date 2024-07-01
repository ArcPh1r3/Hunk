using EntityStates;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace HunkMod.SkillStates.Hunk.Weapon.AssaultRifle
{
    public class Shoot : BaseHunkSkillState
    {
        public static float damageCoefficient = 3.85f;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.225f;
        public static float force = 40f;
        public static float recoil = 0.5f;
        public static float range = 5000f;
        public static GameObject tracerEffectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerCommandoDefault");

        private float duration;
        private string muzzleString;
        private bool isCrit;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = Shoot.baseDuration / this.attackSpeedStat;

            this.muzzleString = "MuzzleSMG";

            this.isCrit = base.RollCrit();

            this.Fire();

            this.PlayAnimation("Gesture, Override", "Shoot", "Shoot.playbackRate", 0.4f);

            if (this.hunk)
            {
                this.hunk.ConsumeAmmo();
                this.hunk.machineGunVFX.Play();
            }
        }

        private void Fire()
        {
            EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, this.muzzleString, false);

            if (this.isCrit) Util.PlaySound("sfx_hunk_assaultrifle_shoot", base.gameObject);
            else Util.PlaySound("sfx_hunk_assaultrifle_shoot", base.gameObject);

            float spreadBloom = 0.27f;

            base.characterBody.AddSpreadBloom(spreadBloom);

            if (this.characterBody.HasBuff(Modules.Survivors.Hunk.bulletTimeBuff) && this.hunk.targetHurtbox && this.hunk.targetHurtbox.healthComponent && this.hunk.targetHurtbox.healthComponent.alive)
            {
                if (NetworkServer.active)
                {
                    GenericDamageOrb genericDamageOrb = this.CreateBulletOrb();
                    genericDamageOrb.damageValue = Shoot.damageCoefficient * this.damageStat * 1.5f;
                    genericDamageOrb.isCrit = this.isCrit;
                    genericDamageOrb.teamIndex = TeamComponent.GetObjectTeam(this.gameObject);
                    genericDamageOrb.attacker = this.gameObject;
                    genericDamageOrb.procCoefficient = 1f;
                    genericDamageOrb.damageColorIndex = DamageColorIndex.Sniper;

                    HurtBox hurtBox = this.hunk.targetHurtbox;
                    if (hurtBox)
                    {
                        Transform transform = this.FindModelChild(this.muzzleString);
                        genericDamageOrb.origin = transform.position;
                        genericDamageOrb.target = hurtBox;
                        OrbManager.instance.AddOrb(genericDamageOrb);
                    }
                    this.hunk.targetHurtbox.healthComponent.gameObject.AddComponent<Modules.Components.HunkHeadshotTracker>();
                }

                return;
            }

            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay2();

                float spread = 0f;// this.characterBody.spreadBloomAngle * 3f;
                BulletAttack.FalloffModel falloff = BulletAttack.FalloffModel.None;
                float recoilAmplitude = Shoot.recoil / this.attackSpeedStat;

                base.AddRecoil2(-1f * recoilAmplitude, -2f * recoilAmplitude, -0.5f * recoilAmplitude, 0.5f * recoilAmplitude);

                BulletAttack bulletAttack = new BulletAttack
                {
                    bulletCount = 1,
                    aimVector = aimRay.direction,
                    origin = aimRay.origin,
                    damage = Shoot.damageCoefficient * this.damageStat,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Generic,
                    falloffModel = falloff,
                    maxDistance = Shoot.range,
                    force = Shoot.force,
                    hitMask = LayerIndex.CommonMasks.bullet,
                    minSpread = 0f,
                    maxSpread = spread,
                    isCrit = this.isCrit,
                    owner = this.gameObject,
                    muzzleName = muzzleString,
                    smartCollision = true,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = procCoefficient,
                    radius = 0.25f,
                    sniper = false,
                    stopperMask = LayerIndex.CommonMasks.bullet,
                    weapon = null,
                    tracerEffectPrefab = this.tracerPrefab,
                    spreadPitchScale = 1f,
                    spreadYawScale = 1f,
                    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                    hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FireBarrage.hitEffectPrefab,
                };

                bulletAttack.modifyOutgoingDamageCallback = delegate (BulletAttack _bulletAttack, ref BulletAttack.BulletHit hitInfo, DamageInfo damageInfo)
                {
                    if (BulletAttack.IsSniperTargetHit(hitInfo))
                    {
                        float mult = 1.5f;

                        damageInfo.damage *= mult;
                        damageInfo.damageColorIndex = DamageColorIndex.Sniper;
                        EffectData effectData = new EffectData
                        {
                            origin = hitInfo.point,
                            rotation = Quaternion.LookRotation(-hitInfo.direction)
                        };

                        effectData.SetHurtBoxReference(hitInfo.hitHurtBox);
                        EffectManager.SpawnEffect(Modules.Assets.headshotEffect, effectData, true);
                        Util.PlaySound("sfx_hunk_headshot", hitInfo.hitHurtBox.gameObject);
                        hitInfo.hitHurtBox.healthComponent.gameObject.AddComponent<Modules.Components.HunkHeadshotTracker>();
                    }
                };

                bulletAttack.Fire();
            }
        }

        private GameObject tracerPrefab
        {
            get
            {
                return Shoot.tracerEffectPrefab;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            float kek = 0.5f;

            if (base.fixedAge >= kek * this.duration)
            {
                return InterruptPriority.Any;
            }

            return InterruptPriority.Skill;
        }
    }
}