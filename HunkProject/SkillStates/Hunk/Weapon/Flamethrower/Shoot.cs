using EntityStates;
using RoR2;
using UnityEngine;

namespace HunkMod.SkillStates.Hunk.Weapon.Flamethrower
{
    public class Shoot : BaseHunkSkillState
    {
        public static float damageCoefficient = 2f;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.1f;
        public static float force = 5f;
        public static float recoil = 0.5f;
        public static float range = 50f;

        private float duration;
        private string muzzleString;
        private bool isCrit;
        private bool startingUp;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = Shoot.baseDuration / this.attackSpeedStat;

            if (this.hunk.flamethrowerLifetime <= 0f)
            {
                this.startingUp = true;
                this.duration = 0.2f / this.attackSpeedStat;
                Util.PlaySound("sfx_hunk_flamethrower_start", this.gameObject);
            }

            this.muzzleString = "MuzzleSMG";

            this.isCrit = base.RollCrit();

            if (!this.startingUp) this.Fire();

            //this.PlayAnimation("Gesture, Override", "Shoot", "Shoot.playbackRate", 0.2f);

            if (!this.startingUp)
            {
                this.hunk.ConsumeAmmo(2);
                this.hunk.flamethrowerLifetime = 0.25f / this.attackSpeedStat;
            }
        }

        private void Fire()
        {
            //EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, this.muzzleString, false);

            //if (this.isCrit) Util.PlaySound("sfx_hunk_smg_shoot", base.gameObject);
            //else Util.PlaySound("sfx_hunk_smg_shoot", base.gameObject);

            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay2();

                float recoilAmplitude = Shoot.recoil / this.attackSpeedStat;

                base.AddRecoil2(-1f * recoilAmplitude, -2f * recoilAmplitude, -0.5f * recoilAmplitude, 0.5f * recoilAmplitude);

                BulletAttack bulletAttack = new BulletAttack
                {
                    bulletCount = 1,
                    aimVector = aimRay.direction,
                    origin = aimRay.origin,
                    damage = Shoot.damageCoefficient * this.damageStat,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.IgniteOnHit,
                    falloffModel = BulletAttack.FalloffModel.None,
                    maxDistance = Shoot.range,
                    force = Shoot.force,
                    hitMask = LayerIndex.CommonMasks.bullet,
                    minSpread = 0f,
                    maxSpread = 0f,
                    isCrit = this.isCrit,
                    owner = this.gameObject,
                    muzzleName = muzzleString,
                    smartCollision = true,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = procCoefficient,
                    radius = 1.5f,
                    sniper = false,
                    stopperMask = LayerIndex.world.intVal,
                    weapon = null,
                    tracerEffectPrefab = null,
                    spreadPitchScale = 0f,
                    spreadYawScale = 0f,
                    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                    hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FireBarrage.hitEffectPrefab,
                };

                bulletAttack.Fire();
            }

            base.characterBody.AddSpreadBloom(0.6f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.duration)
            {
                if (this.startingUp)
                {
                    this.hunk.flamethrowerLifetime = 0.1f;
                    if (base.isAuthority)
                    {
                        this.outer.SetNextState(new Shoot());
                        return;
                    }
                }

                if (base.isAuthority)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}