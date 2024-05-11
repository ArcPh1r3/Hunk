using EntityStates;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HunkMod.SkillStates.Hunk.Weapon.Revolver
{
    public class Shoot : BaseHunkSkillState
    {
        public static float damageCoefficient = 24f;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.25f;
        public static float force = 3000f;
        public static float recoil = 3.5f;
        public static float range = 2000f;
        public static GameObject tracerEffectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerCommandoDefault");

        private float duration;
        private string muzzleString;
        private bool isCrit;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = Shoot.baseDuration / this.attackSpeedStat;
            this.characterBody.isSprinting = false;

            base.characterBody.SetAimTimer(2f);
            this.muzzleString = "MuzzlePistol";

            this.isCrit = base.RollCrit();

            this.Fire();

            this.PlayAnimation("Gesture, Override", "ShootMagnum", "Shoot.playbackRate", this.duration * 1.5f);

            if (this.hunk)
            {
                this.hunk.ConsumeAmmo();
                this.hunk.machineGunVFX.Play();
            }
        }

        private void Fire()
        {
            EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, this.muzzleString, false);

            if (this.isCrit) Util.PlaySound("sfx_hunk_revolver_shoot", base.gameObject);
            else Util.PlaySound("sfx_hunk_revolver_shoot", base.gameObject);

            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();

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
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    maxDistance = Shoot.range,
                    force = Shoot.force,
                    hitMask = LayerIndex.CommonMasks.bullet,
                    minSpread = 0f,
                    maxSpread = this.characterBody.spreadBloomAngle * 1.5f,
                    isCrit = this.isCrit,
                    owner = this.gameObject,
                    muzzleName = muzzleString,
                    smartCollision = true,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = procCoefficient,
                    radius = 0.5f,
                    sniper = false,
                    stopperMask = LayerIndex.CommonMasks.bullet,
                    weapon = null,
                    tracerEffectPrefab = this.tracerPrefab,
                    spreadPitchScale = 1f,
                    spreadYawScale = 1f,
                    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                    hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab,
                };

                bulletAttack.modifyOutgoingDamageCallback = delegate (BulletAttack _bulletAttack, ref BulletAttack.BulletHit hitInfo, DamageInfo damageInfo)
                {
                    if (BulletAttack.IsSniperTargetHit(hitInfo))
                    {
                        damageInfo.damage *= 1.5f;
                        damageInfo.damageColorIndex = DamageColorIndex.Sniper;
                        EffectData effectData = new EffectData
                        {
                            origin = hitInfo.point,
                            rotation = Quaternion.LookRotation(-hitInfo.direction)
                        };

                        effectData.SetHurtBoxReference(hitInfo.hitHurtBox);
                        EffectManager.SpawnEffect(Modules.Assets.headshotEffect, effectData, true);
                        Util.PlaySound("sfx_hunk_headshot", base.gameObject);

                        NetworkIdentity identity = this.GetComponent<NetworkIdentity>();
                        if (identity) new Modules.Components.SyncHeadshot(identity.netId, hitInfo.hitHurtBox.healthComponent.gameObject).Send(NetworkDestination.Server);

                        hitInfo.hitHurtBox.healthComponent.gameObject.AddComponent<Modules.Components.HunkHeadshotTracker>();
                    }
                };

                bulletAttack.Fire();
            }

            base.characterBody.AddSpreadBloom(0.7f);
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
            float kek = 0.8f;

            if (base.fixedAge >= kek * this.duration)
            {
                return InterruptPriority.Any;
            }

            return InterruptPriority.Skill;
        }
    }
}