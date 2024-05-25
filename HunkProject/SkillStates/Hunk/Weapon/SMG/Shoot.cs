﻿using EntityStates;
using RoR2;
using UnityEngine;

namespace HunkMod.SkillStates.Hunk.Weapon.SMG
{
    public class Shoot : BaseHunkSkillState
    {
        public static float damageCoefficient = 2.8f;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.21f;
        public static float force = 20f;
        public static float recoil = 1.5f;
        public static float range = 2000f;
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

            if (this.isCrit) Util.PlaySound("sfx_hunk_smg_shoot", base.gameObject);
            else Util.PlaySound("sfx_hunk_smg_shoot", base.gameObject);

            float spreadBloom = 0.7f;

            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay2();

                float spread = this.characterBody.spreadBloomAngle * 3f;
                BulletAttack.FalloffModel falloff = BulletAttack.FalloffModel.DefaultBullet;
                float recoilAmplitude = Shoot.recoil / this.attackSpeedStat;

                if (this.characterBody.inventory && this.characterBody.inventory.GetItemCount(Modules.Weapons.SMG.laserSight) > 0)
                {
                    recoil *= 0.25f;
                    spread = 0f;
                    falloff = BulletAttack.FalloffModel.None;
                    spreadBloom = 0.1f;
                }

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
                        float mult = 1.25f;
                        if (this.characterBody.inventory && this.characterBody.inventory.GetItemCount(Modules.Weapons.SMG.laserSight) > 0)
                        {
                            mult = 1.5f;
                        }

                        damageInfo.damage *= mult;
                        damageInfo.damageColorIndex = DamageColorIndex.Sniper;
                        EffectData effectData = new EffectData
                        {
                            origin = hitInfo.point,
                            rotation = Quaternion.LookRotation(-hitInfo.direction)
                        };

                        effectData.SetHurtBoxReference(hitInfo.hitHurtBox);
                        //EffectManager.SpawnEffect(Modules.Assets.headshotEffect, effectData, true);
                        Util.PlaySound("sfx_hunk_headshot", hitInfo.hitHurtBox.gameObject);
                        //hitInfo.hitHurtBox.healthComponent.gameObject.AddComponent<Modules.Components.HunkHeadshotTracker>();
                    }
                };

                bulletAttack.Fire();
            }

            base.characterBody.AddSpreadBloom(spreadBloom);
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