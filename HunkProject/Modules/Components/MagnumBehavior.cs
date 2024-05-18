using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class MagnumBehavior : WeaponBehavior
    {
        private GameObject shortBarrel;
        private GameObject longBarrel;

        private void Start()
        {
            this.shortBarrel = this.childLocator.FindChild("ShortBarrel").gameObject;
            this.longBarrel = this.childLocator.FindChild("LongBarrel").gameObject;

            this.shortBarrel.SetActive(true);
            this.longBarrel.SetActive(false);
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
                        if (this.hunk.characterBody.inventory.GetItemCount(Modules.Weapons.Magnum.longBarrel) > 0)
                        {
                            this.shortBarrel.SetActive(false);
                            this.longBarrel.SetActive(true);
                        }
                        else
                        {
                            this.shortBarrel.SetActive(true);
                            this.longBarrel.SetActive(false);
                        }
                    }
                    else
                    {
                        this.shortBarrel.SetActive(true);
                        this.longBarrel.SetActive(false);
                    }
                }
                else
                {
                    this.shortBarrel.SetActive(true);
                    this.longBarrel.SetActive(false);
                }
            }
            else
            {
                this.shortBarrel.SetActive(true);
                this.longBarrel.SetActive(false);
            }
        }
    }
}