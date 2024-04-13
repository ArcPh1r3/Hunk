using EntityStates;
using HunkMod.Modules.Components;

namespace HunkMod.SkillStates.Hunk
{
    public class BaseHunkSkillState : BaseSkillState
    {
        public virtual void AddRecoil2(float x1, float x2, float y1, float y2)
        {
            if (!Modules.Config.enableRecoil.Value) return;
            this.AddRecoil(x1, x2, y1, y2);
        }

        protected virtual bool hideGun
        {
            get
            {
                return false;
            }
        }
        protected HunkController hunk;
        protected HunkWeaponDef cachedWeaponDef;
        protected virtual string prop
        {
            get
            {
                return "";
            }
        }

        public override void OnEnter()
        {
            this.hunk = this.GetComponent<HunkController>();
            if (this.hunk) this.cachedWeaponDef = this.hunk.weaponDef;
            base.OnEnter();

            if (this.hideGun) this.GetModelChildLocator().FindChild("WeaponModel").gameObject.SetActive(false);
            if (this.prop != "") this.GetModelChildLocator().FindChild(this.prop).gameObject.SetActive(true);
        }

        public override void OnExit()
        {
            base.OnExit();

            if (this.hideGun) this.GetModelChildLocator().FindChild("WeaponModel").gameObject.SetActive(true);
            if (this.prop != "") this.GetModelChildLocator().FindChild(this.prop).gameObject.SetActive(false);
        }
    }
}