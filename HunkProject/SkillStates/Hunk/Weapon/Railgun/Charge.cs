using RoR2;
using UnityEngine;
using EntityStates;
using UnityEngine.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;

namespace HunkMod.SkillStates.Hunk.Weapon.Railgun
{
    public class Charge : BaseHunkSkillState
    {
        public float baseDuration = 3f;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration / this.attackSpeedStat;
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                if (base.fixedAge >= this.duration)
                {
                    this.outer.SetNextState(new Shoot());
                    return;
                }

                if (!this.hunk.isAiming)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }
    }
}