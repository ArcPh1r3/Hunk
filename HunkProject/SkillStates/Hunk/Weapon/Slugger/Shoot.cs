﻿using RoR2;
using UnityEngine;
using EntityStates;

namespace HunkMod.SkillStates.Hunk.Weapon.Slugger
{
    public class Shoot : BaseHunkSkillState
    {
        public static float damageCoefficient = 16f;
        public static float procCoefficient = 1f;
        public float baseDuration = 1.5f;
        public static float bulletRecoil = 8f;
        public static float bulletRange = 128f;
        public static float bulletThiccness = 1f;
        public float selfForce = 1200f;

        private float earlyExitTime;
        protected float duration;
        protected float fireDuration;
        protected bool hasFired;
        private bool isCrit;
        protected string muzzleString;

        public override void OnEnter()
        {
            base.OnEnter();
            this.characterBody.SetAimTimer(5f);
            this.muzzleString = "ShotgunMuzzle";
            this.hasFired = false;
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.isCrit = base.RollCrit();
            this.earlyExitTime = 0.75f * this.duration;

            if (this.isCrit) Util.PlaySound("sfx_driver_riot_shotgun_shoot_critical", base.gameObject);
            else Util.PlaySound("sfx_driver_riot_shotgun_shoot", base.gameObject);

            base.PlayAnimation("Gesture, Override", "FireRiotShotgun", "Shoot.playbackRate", this.duration);
            base.PlayAnimation("AimPitch", "Shoot");

            this.fireDuration = 0;

            this.hunk.ConsumeAmmo();
        }

        public virtual void FireBullet()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;

                this.hunk.DropShell(-this.GetModelBaseTransform().transform.right * -Random.Range(4, 12));

                float recoilAmplitude = Shoot.bulletRecoil / this.attackSpeedStat;

                base.AddRecoil2(-0.4f * recoilAmplitude, -0.8f * recoilAmplitude, -0.3f * recoilAmplitude, 0.3f * recoilAmplitude);
                this.characterBody.AddSpreadBloom(4f);
                EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FireBarrage.effectPrefab, gameObject, muzzleString, false);

                GameObject tracer = Modules.Assets.shotgunTracer;
                if (this.isCrit) tracer = Modules.Assets.shotgunTracerCrit;

                if (base.isAuthority)
                {
                    float damage = Shoot.damageCoefficient * this.damageStat;

                    Ray aimRay = GetAimRay();

                    float force = 1500;

                    BulletAttack bulletAttack = new BulletAttack
                    {
                        aimVector = aimRay.direction,
                        origin = aimRay.origin,
                        damage = damage,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageType.Generic,
                        falloffModel = BulletAttack.FalloffModel.None,
                        maxDistance = bulletRange,
                        force = force,
                        hitMask = LayerIndex.CommonMasks.bullet,
                        isCrit = this.isCrit,
                        owner = gameObject,
                        muzzleName = muzzleString,
                        smartCollision = true,
                        procChainMask = default,
                        procCoefficient = procCoefficient,
                        radius = Shoot.bulletThiccness,
                        sniper = false,
                        stopperMask = LayerIndex.world.collisionMask,
                        weapon = null,
                        tracerEffectPrefab = tracer,
                        spreadPitchScale = 1f,
                        spreadYawScale = 1f,
                        queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                        hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FireBarrage.hitEffectPrefab,
                        HitEffectNormal = false,
                    };

                    bulletAttack.minSpread = 0;
                    bulletAttack.maxSpread = 0;
                    bulletAttack.bulletCount = 1;
                    bulletAttack.Fire();

                    this.characterMotor.ApplyForce(aimRay.direction * -this.selfForce);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireDuration)
            {
                this.FireBullet();
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge >= this.earlyExitTime) return InterruptPriority.Any;
            return InterruptPriority.Skill;
        }
    }
}