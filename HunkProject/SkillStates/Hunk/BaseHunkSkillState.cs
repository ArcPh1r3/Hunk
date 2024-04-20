using UnityEngine;
using EntityStates;
using HunkMod.Modules.Components;

namespace HunkMod.SkillStates.Hunk
{
    public class BaseHunkSkillState : BaseSkillState
    {
        private Animator _animator;

        public virtual void AddRecoil2(float x1, float x2, float y1, float y2)
        {
            //if (this.hunk.lockOnTimer > 0f) return;
            if (!Modules.Config.enableRecoil.Value) return;
            this.AddRecoil(x1, x2, y1, y2);
        }

        public virtual Ray GetAimRay2()
        {
            /*if (this.hunk.lockOnTimer > 0f && this.hunk.isAiming)
            {
                return new Ray
                {
                    origin = this.characterBody.aimOrigin,
                    direction = (this.hunk.targetHurtbox.transform.position - this.characterBody.aimOrigin).normalized
                };
            }*/
            return this.GetAimRay();
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
            this._animator = this.GetModelAnimator();
            base.OnEnter();

            if (this.hideGun) this.GetModelChildLocator().FindChild("WeaponModel").gameObject.SetActive(false);
            if (this.prop != "") this.GetModelChildLocator().FindChild(this.prop).gameObject.SetActive(true);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (this._animator)
            {
                this._animator.SetBool("isAiming", this.hunk.isAiming);
                if (this.hunk.isAiming) this._animator.SetFloat("aimBlend", 1f);
                else this._animator.SetFloat("aimBlend", 0f);
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (this.hideGun) this.GetModelChildLocator().FindChild("WeaponModel").gameObject.SetActive(true);
            if (this.prop != "") this.GetModelChildLocator().FindChild(this.prop).gameObject.SetActive(false);
        }
    }
}