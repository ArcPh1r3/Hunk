using UnityEngine;
using RoR2;
using EntityStates;

namespace HunkMod.SkillStates.Hunk.Weapon.GrappleGun
{
    public class Shoot : BaseHunkSkillState
    {
        public static float baseDuration = 0.5f;
        public static float recoil = 1f;
        public static float range = 80f;
        public static GameObject tracerEffectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerCommandoDefault");

        private float duration;
        private string muzzleString;
        private Vector3 hookPoint;
        private bool success;
        private LineRenderer wire;
        private Vector3 linePoint;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = Shoot.baseDuration / this.attackSpeedStat;
            this.wire = this.FindModelChild("Wire").GetComponent<LineRenderer>();

            this.muzzleString = "MuzzlePistol";
            this.Fire();
        }

        public override void OnExit()
        {
            base.OnExit();
            if (this.wire) this.wire.gameObject.SetActive(false);
        }

        private void Fire()
        {
            Ray ray = this.GetAimRay();
            RaycastHit raycastHit;
            if (Physics.Raycast(ray.origin, ray.direction, out raycastHit, Shoot.range, LayerIndex.CommonMasks.bullet))
            {
                this.success = true;
                this.hookPoint = raycastHit.point;
                this.linePoint = this.FindModelChild(this.muzzleString).position;
                this.PlayAnimation("Gesture, Override", "Shoot", "Shoot.playbackRate", this.duration);
                EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, this.muzzleString, false);
                Util.PlaySound("sfx_hunk_grapple_shoot", base.gameObject);

                if (base.isAuthority)
                {
                    Ray aimRay = base.GetAimRay();

                    float recoilAmplitude = Shoot.recoil / this.attackSpeedStat;

                    base.AddRecoil2(-1f * recoilAmplitude, -2f * recoilAmplitude, -0.5f * recoilAmplitude, 0.5f * recoilAmplitude);
                }
                base.characterBody.AddSpreadBloom(0.7f);
            }
            else
            {
                this.success = false;
                this.duration = 0.1f;
            }
        }

        public override void Update()
        {
            base.Update();

            if (this.wire && this.success)
            {
                this.wire.SetPosition(0, this.wire.transform.position);
                this.wire.SetPosition(1, Vector3.Lerp(this.linePoint, this.hookPoint, 30f * Time.deltaTime));
                this.wire.gameObject.SetActive(true);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                if (this.success)
                {
                    this.outer.SetNextState(new Pull
                    {
                        hookPoint = this.hookPoint
                    });

                    GameObject p = new GameObject();
                    p.transform.position = hookPoint;
                    Util.PlaySound("sfx_hunk_grapple_impact", p);
                    Destroy(p);
                    return;
                }
                else
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}