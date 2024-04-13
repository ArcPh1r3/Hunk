using EntityStates;
using HunkMod.Modules;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.SkillStates.Hunk
{
    public class Shoot : BaseHunkSkillState
    {
        public static float damageCoefficient = 2.2f;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.7f;
        public static float baseCritDuration = 0.9f;
        public static float baseCritDuration2 = 1.4f;
        public static float force = 200f;
        public static float recoil = 2f;
        public static float range = 2000f;
        public static GameObject tracerEffectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerGoldGat");

        private float duration;
        private float fireTime;
        private bool hasFired;
        private string muzzleString;
        private bool isCrit;

        protected virtual float _damageCoefficient
        {
            get
            {
                return Shoot.damageCoefficient;
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = Shoot.baseDuration / this.attackSpeedStat;
            this.characterBody.isSprinting = false;
            this.fireTime = 0.1f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.muzzleString = "GunMuzzle";

            this.isCrit = base.RollCrit();

            if (base.isAuthority)
            {
                this.hasFired = true;
                this.Fire();
            }

            this.PlayAnimation("Gesture, Override", "Shoot", "Shoot.playbackRate", this.duration * 1.5f);

            this.hunk.ConsumeAmmo();
        }

        public virtual string shootSoundString
        {
            get
            {
                if (this.isCrit) return "sfx_driver_pistol_shoot_critical";
                return "sfx_driver_pistol_shoot";
            }
        }

        public virtual BulletAttack.FalloffModel falloff
        {
            get
            {
                return BulletAttack.FalloffModel.DefaultBullet;
            }
        }

        private void Fire()
        {
            EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, this.gameObject, this.muzzleString, false);

            Util.PlaySound(this.shootSoundString, this.gameObject);

            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();
                base.AddRecoil2(-1f * Shoot.recoil, -2f * Shoot.recoil, -0.5f * Shoot.recoil, 0.5f * Shoot.recoil);

                BulletAttack attack = new BulletAttack
                {
                    bulletCount = 1,
                    aimVector = aimRay.direction,
                    origin = aimRay.origin,
                    damage = this._damageCoefficient * this.damageStat,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Generic,
                    falloffModel = this.falloff,
                    maxDistance = Shoot.range,
                    force = Shoot.force,
                    hitMask = LayerIndex.CommonMasks.bullet,
                    minSpread = 0f,
                    maxSpread = this.characterBody.spreadBloomAngle * 2f,
                    isCrit = this.isCrit,
                    owner = base.gameObject,
                    muzzleName = muzzleString,
                    smartCollision = true,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = procCoefficient,
                    radius = 0.75f,
                    sniper = false,
                    stopperMask = LayerIndex.CommonMasks.bullet,
                    weapon = null,
                    tracerEffectPrefab = this.tracerPrefab,
                    spreadPitchScale = 1f,
                    spreadYawScale = 1f,
                    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                    hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab,
                };
                attack.Fire();
            }

            base.characterBody.AddSpreadBloom(1.25f);
        }

        protected virtual GameObject tracerPrefab
        {
            get
            {
                return Shoot.tracerEffectPrefab;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireTime && base.isAuthority)
            {
                if (!this.hasFired)
                {
                    this.hasFired = true;
                    this.Fire();
                }
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge >= 0.5f * this.duration)
            {
                return InterruptPriority.Any;
            }

            return InterruptPriority.Skill;
        }
    }
}