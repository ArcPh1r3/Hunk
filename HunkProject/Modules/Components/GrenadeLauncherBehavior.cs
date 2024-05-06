using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class GrenadeLauncherBehavior : WeaponBehavior
    {
        private Animator animator;

        private void Awake()
        {
            this.animator = this.GetComponentInChildren<Animator>();
        }

        protected override void RunFixedUpdate()
        {
            base.RunFixedUpdate();

            if (this.animator) this.animator.SetBool("isReloading", this.hunk.isReloading);
        }
    }
}