using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.SkillStates.Hunk.Weapon.RocketLauncher
{
    public class Shoot : BaseHunkSkillState
    {
        public static float damageCoefficient = 48f;
        public static float procCoefficient = 1f;
        public float baseDuration = 1.8f; // the base skill duration. i.e. attack speed
        public static float recoil = 40f;

        private float earlyExitTime;
        protected float duration;
        private bool isCrit;
        protected string muzzleString;

        protected virtual string soundString
        {
            get
            {
                if (this.isCrit) return "sfx_hunk_rocket_fire";
                return "sfx_hunk_rocket_fire";
            }
        }

        protected virtual GameObject projectilePrefab
        {
            get
            {
                return Modules.Projectiles.rocketProjectilePrefab;
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.muzzleString = "MuzzleRocket";
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.isCrit = base.RollCrit();
            this.earlyExitTime = 0.9f * this.duration;

            Util.PlaySound(this.soundString, base.gameObject);
            this.PlayAnimation("Gesture, Override", "Shoot", "Shoot.playbackRate", this.duration * 1.25f);

            this.Fire();

            if (this.hunk) this.hunk.ConsumeAmmo();
        }

        protected virtual float _damageCoefficient
        {
            get
            {
                return Shoot.damageCoefficient;
            }
        }

        public virtual void Fire()
        {
            float recoilAmplitude = Shoot.recoil / this.attackSpeedStat;

            base.AddRecoil2(-0.4f * recoilAmplitude, -0.8f * recoilAmplitude, -0.3f * recoilAmplitude, 0.3f * recoilAmplitude);
            this.characterBody.AddSpreadBloom(4f);
            EffectManager.SimpleMuzzleFlash(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/MuzzleflashSmokeRing.prefab").WaitForCompletion(), this.gameObject, this.muzzleString, false);

            if (base.isAuthority)
            {
                Ray aimRay = this.GetAimRay();

                // copied from moff's rocket
                // the fact that this item literally has to be hardcoded into character skillstates makes me so fucking angry you have no idea
                if (this.characterBody.inventory && this.characterBody.inventory.GetItemCount(DLC1Content.Items.MoreMissile) > 0)
                {
                    float damageMult = MainPlugin.GetICBMDamageMult(this.characterBody);

                    Vector3 rhs = Vector3.Cross(Vector3.up, aimRay.direction);
                    Vector3 axis = Vector3.Cross(aimRay.direction, rhs);

                    float currentSpread = 0f;
                    float num2 = UnityEngine.Random.Range(1f + currentSpread, 1f + currentSpread) * 3f;   //Bandit is x2
                    float angle = num2 / 2f;  //3 - 1 rockets

                    Vector3 direction = Quaternion.AngleAxis(-num2 * 0.5f, axis) * aimRay.direction;
                    Quaternion rotation = Quaternion.AngleAxis(angle, axis);
                    Ray aimRay2 = new Ray(aimRay.origin, direction);
                    for (int i = 0; i < 3; i++)
                    {
                        ProjectileManager.instance.FireProjectile(this.projectilePrefab, aimRay2.origin, Util.QuaternionSafeLookRotation(aimRay2.direction), this.gameObject, damageMult * this.damageStat * this._damageCoefficient, 1000f, this.isCrit, DamageColorIndex.Default, null, 120f);
                        aimRay2.direction = rotation * aimRay2.direction;
                    }
                }
                else
                {
                    ProjectileManager.instance.FireProjectile(this.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), this.gameObject, this.damageStat * this._damageCoefficient, 1000f, this.isCrit, DamageColorIndex.Default, null, 120f);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

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