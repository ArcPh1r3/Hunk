using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class SMGBehavior : WeaponBehavior
    {
        private GameObject laserSight;
        private GameObject laser;
        private GameObject magazine;
        private GameObject extendedMag;
        private MeshRenderer magRenderer;

        private void Start()
        {
            this.laserSight = this.childLocator.FindChild("LaserSight").gameObject;
            this.laser = this.childLocator.FindChild("Laser").gameObject;
            this.magazine = this.childLocator.FindChild("Magazine").gameObject;
            this.extendedMag = this.childLocator.FindChild("ExtendedMag").gameObject;
            this.magRenderer = this.magazine.GetComponent<MeshRenderer>();

            this.laserSight.SetActive(false);
            this.laser.SetActive(false);
            this.extendedMag.SetActive(false);
            this.magRenderer.enabled = true;
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

        public void HideMag()
        {
            this.magazine.SetActive(false);
        }

        public void ShowMag()
        {
            this.magazine.SetActive(true);
        }
    }
}