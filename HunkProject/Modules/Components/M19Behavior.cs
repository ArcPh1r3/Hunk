using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class M19Behavior : WeaponBehavior
    {
        private GameObject laser;
        private LineRenderer laserLine;
        private GameObject pointer;

        private void Start()
        {
            if (!this.hunk) this.GetHunkController();
            this.laser = this.childLocator.FindChild("Laser").gameObject;
            this.laserLine = this.laser.GetComponent<LineRenderer>();
            this.pointer = this.hunk.childLocator.FindChild("Pointer").gameObject;
            this.pointer.transform.GetChild(0).gameObject.layer = 21;
            this.laser.SetActive(false);
        }

        private void OnDisable()
        {
            if (this.pointer) this.pointer.SetActive(false);
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

                if (equipped)
                {
                    this.laser.SetActive(this.hunk.isAiming);
                    if (this.hunk.isAiming) this.UpdatePointer();
                    else this.pointer.SetActive(false);
                }
            }
        }

        private void UpdatePointer()
        {
            this.laserLine.SetPosition(0, this.laserLine.transform.position);

            Ray aimRay = this.hunk.characterBody.inputBank.GetAimRay();
            RaycastHit raycastHit;
            if (Physics.Raycast(aimRay.origin, aimRay.direction, out raycastHit, 5000f, LayerIndex.CommonMasks.bullet))
            {
                if (this.pointer)
                {
                    this.pointer.transform.position = raycastHit.point;
                    this.pointer.SetActive(true);
                }
                this.laserLine.SetPosition(1, raycastHit.point);
            }
            else
            {
                if (this.pointer) this.pointer.SetActive(false);
                this.laserLine.SetPosition(1, aimRay.origin + (aimRay.direction * 5000f));
            }
        }
    }
}   