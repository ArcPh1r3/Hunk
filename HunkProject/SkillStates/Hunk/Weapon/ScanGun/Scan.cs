using EntityStates;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using HunkMod.Modules.Components;

namespace HunkMod.SkillStates.Hunk.Weapon.ScanGun
{
    public class Scan : BaseHunkSkillState
    {
        public static float recoil = 0.25f;

        private float duration;
        private WeaponChest[] weaponCases;

        public override void OnEnter()
        {
            base.OnEnter();

            this.weaponCases = MonoBehaviour.FindObjectsOfType<WeaponChest>();

            float dist = 201f;
            for (int i = 0; i < this.weaponCases.Length; i++)
            {
                if (this.weaponCases[i] && this.weaponCases[i].alive)
                {
                    float _dist = Vector3.Distance(this.weaponCases[i].transform.position, this.transform.position);
                    if (_dist <= dist)
                    {
                        dist = _dist;
                    }
                }
            }
            this.duration = Util.Remap(dist, 0f, 200f, 0.15f, 1.4f);

            if (dist <= 200f)
            {
                this.Fire();
            }
            else this.duration = 0.25f;
        }

        private void Fire()
        {
            Util.PlaySound("sfx_hunk_scanner_beep", base.gameObject);

            if (base.isAuthority)
            {
                float recoilAmplitude = Shoot.recoil / this.attackSpeedStat;
                base.AddRecoil2(-1f * recoilAmplitude, -2f * recoilAmplitude, -0.5f * recoilAmplitude, 0.5f * recoilAmplitude);
            }

            base.characterBody.AddSpreadBloom(0.7f);
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
            return InterruptPriority.Skill;
        }
    }
}