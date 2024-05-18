using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class RevolverBehavior : WeaponBehavior
    {
        private GameObject speedloader;
        private Transform barrel;

        private void Start()
        {
            this.speedloader = this.childLocator.FindChild("Speedloader").gameObject;
            this.barrel = this.childLocator.FindChild("Barrel");

            this.speedloader.SetActive(false);
        }

        protected override void RunFixedUpdate()
        {
            base.RunFixedUpdate();

            if (this.hunk)
            {
                if (this.hunk.characterBody)
                {
                    if (this.hunk.characterBody.inventory)
                    {
                        if (this.hunk.characterBody.inventory.GetItemCount(Modules.Weapons.Revolver.speedloader) > 0)
                        {
                            this.speedloader.SetActive(true);
                        }
                        else
                        {
                            this.speedloader.SetActive(false);
                        }
                    }
                    else
                    {
                        this.speedloader.SetActive(false);
                    }
                }
                else
                {
                    this.speedloader.SetActive(false);
                }
            }
            else
            {
                this.speedloader.SetActive(false);
            }
        }

        private void LateUpdate()
        {
            if (this.hunk)
            {
                if (this.hunk.characterBody)
                {
                    if (this.hunk.characterBody.inventory)
                    {
                        if (this.hunk.characterBody.inventory.GetItemCount(Modules.Weapons.Revolver.speedloader) > 0)
                        {
                            this.barrel.transform.localScale = Vector3.zero;
                        }
                        else
                        {
                            this.barrel.transform.localScale = Vector3.one;
                        }
                    }
                    else
                    {
                        this.barrel.transform.localScale = Vector3.one;
                    }
                }
                else
                {
                    this.barrel.transform.localScale = Vector3.one;
                }
            }
            else
            {
                this.barrel.transform.localScale = Vector3.one;
            }
        }
    }
}