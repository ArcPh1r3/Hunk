using UnityEngine;
using EntityStates;
using HunkMod.Modules.Components;
using System;

namespace HunkMod.SkillStates.Hunk
{
    public class BaseHunkSkillState : BaseSkillState
    {
        private Animator _animator;
        private bool knifeWasActive;
        protected virtual bool turningAllowed
        {
            get
            {
                return true;
            }
        }

        protected virtual bool normalizeModel
        {
            get
            {
                return false;
            }
        }

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
        protected virtual string prop { get; set; }

        public override void OnEnter()
        {
            this.hunk = this.GetComponent<HunkController>();
            if (this.hunk) this.cachedWeaponDef = this.hunk.weaponDef;
            this._animator = this.GetModelAnimator();
            base.OnEnter();

            if (!this.turningAllowed) this.characterDirection.turnSpeed = 0f;
            if (this.hideGun)
            {
                this.GetModelChildLocator().FindChild("Weapon").gameObject.SetActive(false);
                this.knifeWasActive = this.FindModelChild("KnifeModel").gameObject.activeSelf;
                this.FindModelChild("KnifeModel").gameObject.SetActive(false);
            }
            if (!String.IsNullOrEmpty(this.prop)) this.GetModelChildLocator().FindChild(this.prop).gameObject.SetActive(true);
            if (this.normalizeModel) this.modelLocator.normalizeToFloor = true;
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

            if (!this.turningAllowed) this.characterDirection.turnSpeed = this.hunk.baseTurnSpeed;
            if (this.hideGun)
            {
                this.GetModelChildLocator().FindChild("Weapon").gameObject.SetActive(true);
                this.FindModelChild("KnifeModel").gameObject.SetActive(this.knifeWasActive);
            }
            if (!String.IsNullOrEmpty(this.prop)) this.GetModelChildLocator().FindChild(this.prop).gameObject.SetActive(false);
            if (this.normalizeModel) this.modelLocator.normalizeToFloor = false;
        }
    }
}