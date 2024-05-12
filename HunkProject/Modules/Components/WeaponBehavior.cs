using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class WeaponBehavior : MonoBehaviour
    {
        protected HunkController hunk;
        protected ChildLocator childLocator;

        private void Awake()
        {
            this.childLocator = this.GetComponent<ChildLocator>();
        }

        private void GetHunkController()
        {
            if (!this.hunk)
            {
                CharacterModel cm = this.GetComponentInParent<CharacterModel>();
                if (cm)
                {
                    CharacterBody cb = cm.body;
                    if (cb)
                    {
                        this.hunk = cb.GetComponent<HunkController>();
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (!this.hunk) this.GetHunkController();
            else this.RunFixedUpdate();
        }

        protected virtual void RunFixedUpdate()
        {

        }
    }
}