using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class SMGBehavior : WeaponBehavior
    {
        private GameObject laserSight;
        private GameObject laser;
        private LineRenderer laserLine;
        private GameObject pointer;
        private GameObject magazine;
        private GameObject extendedMag;
        private MeshRenderer magRenderer;

        private void Start()
        {
            if (!this.hunk) this.GetHunkController();
            this.laserSight = this.childLocator.FindChild("LaserSight").gameObject;
            this.laser = this.childLocator.FindChild("Laser").gameObject;
            this.laserLine = this.laser.GetComponent<LineRenderer>();
            this.pointer = this.hunk.childLocator.FindChild("Pointer").gameObject;
            this.pointer.transform.GetChild(0).gameObject.layer = 21;
            this.magazine = this.childLocator.FindChild("Magazine").gameObject;
            this.extendedMag = this.childLocator.FindChild("ExtendedMag").gameObject;
            this.magRenderer = this.magazine.GetComponent<MeshRenderer>();

            this.laserSight.SetActive(false);
            this.laser.SetActive(false);
            this.extendedMag.SetActive(false);
            this.magRenderer.enabled = true;
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
                if (this.hunk.weaponDef == Modules.Weapons.SMG.instance.weaponDef)
                {
                    equipped = true;
                    if (!this.hunk.isReloading) this.ShowMag();
                }

                if (this.hunk.characterBody)
                {
                    if (this.hunk.characterBody.inventory)
                    {
                        if (this.hunk.characterBody.inventory.GetItemCount(Modules.Weapons.SMG.laserSight) > 0)
                        {
                            this.laserSight.SetActive(true);
                            if (equipped) this.laser.SetActive(this.hunk.isAiming);
                            if (this.hunk.isAiming) this.UpdatePointer();
                            else this.pointer.SetActive(false);
                        }
                        else
                        {
                            this.laserSight.SetActive(false);
                        }

                        if (this.hunk.characterBody.inventory.GetItemCount(Modules.Weapons.SMG.extendedMag) > 0)
                        {
                            this.extendedMag.SetActive(true);
                            this.magRenderer.enabled = false;
                        }
                        else
                        {
                            this.extendedMag.SetActive(false);
                            this.magRenderer.enabled = true;
                        }
                    }
                    else
                    {
                        this.laserSight.SetActive(false);
                        this.extendedMag.SetActive(false);
                        this.magRenderer.enabled = true;
                    }
                }
                else
                {
                    this.laserSight.SetActive(false);
                    this.extendedMag.SetActive(false);
                    this.magRenderer.enabled = true;
                }
            }
            else
            {
                this.laserSight.SetActive(false);
                this.extendedMag.SetActive(false);
                this.magRenderer.enabled = true;
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

        public override void HideMag()
        {
            this.magazine.SetActive(false);
        }

        public override void ShowMag()
        {
            this.magazine.SetActive(true);
        }
    }
}