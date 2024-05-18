using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class MUPBehavior : WeaponBehavior
    {
        private GameObject gunStock;

        private void Start()
        {
            this.gunStock = this.childLocator.FindChild("Stock").gameObject;
            this.gunStock.SetActive(false);
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
                        if (this.hunk.characterBody.inventory.GetItemCount(Modules.Weapons.MUP.gunStock) > 0)
                        {
                            this.gunStock.SetActive(true);
                        }
                        else
                        {
                            this.gunStock.SetActive(false);
                        }
                    }
                    else
                    {
                        this.gunStock.SetActive(false);
                    }
                }
                else
                {
                    this.gunStock.SetActive(false);
                }
            }
            else
            {
                this.gunStock.SetActive(false);
            }
        }
    }
}