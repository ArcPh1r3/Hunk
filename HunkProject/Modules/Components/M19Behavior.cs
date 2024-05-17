using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class M19Behavior : WeaponBehavior
    {
        private GameObject laser;

        private void Start()
        {
            this.laser = this.childLocator.FindChild("Laser").gameObject;
            this.laser.SetActive(false);
        }

        protected override void RunFixedUpdate()
        {
            base.RunFixedUpdate();

            if (this.hunk)
            {
                bool equipped = false;
                if (this.hunk.weaponDef == Modules.Weapons.M19.instance.weaponDef)
                {
                    equipped = true;
                }

                if (equipped) this.laser.SetActive(this.hunk.isAiming);
            }
        }
    }
}   