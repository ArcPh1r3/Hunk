using HunkMod.Modules.Components;
using UnityEngine;

namespace HunkMod.SkillStates.Emote
{
    public class Rest : BaseEmote
    {
        private GameObject boxInstance;
        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = this.GetModelAnimator();
            this.PlayEmote("RestEmote");

            this.boxInstance = GameObject.Instantiate(Modules.Assets.cardboardBox);
            this.boxInstance.transform.rotation = this.GetModelTransform().rotation;
            this.boxInstance.transform.parent = this.GetModelTransform();
            this.boxInstance.transform.localPosition = new Vector3(0f, 0f, 0.02f);
            this.boxInstance.transform.localScale = Vector3.one * 0.0175f;

            this.modelLocator.normalizeToFloor = true;

            this.animator.SetLayerWeight(this.animator.GetLayerIndex("AimYaw"), 0f);
            this.animator.SetLayerWeight(this.animator.GetLayerIndex("AimPitch"), 0f);
        }

        public override void OnExit()
        {
            base.OnExit();

            if (this.boxInstance) Destroy(this.boxInstance);
            this.animator.SetLayerWeight(this.animator.GetLayerIndex("AimYaw"), 1f);
            this.animator.SetLayerWeight(this.animator.GetLayerIndex("AimPitch"), 1f);
            this.modelLocator.normalizeToFloor = false;
        }
    }
}