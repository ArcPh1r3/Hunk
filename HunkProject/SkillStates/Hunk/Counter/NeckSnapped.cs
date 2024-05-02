using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;

namespace HunkMod.SkillStates.Hunk.Counter
{
    public class NeckSnapped : BaseState
    {
        public float duration = 3f;
        private bool hasSnapped;

        public override void OnEnter()
        {
            base.OnEnter();

            if (NetworkServer.active) this.characterBody.AddBuff(RoR2Content.Buffs.ArmorBoost);

            //this.modelLocator.modelBaseTransform.localPosition = new Vector3(0.4f, -0.914f, 0.4f);
            this.modelLocator.modelTransform.localScale = new Vector3(0.1f, 0.13f, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!this.hasSnapped && base.fixedAge >= 0.455f * this.duration)
            {
                this.hasSnapped = true;
                this.characterBody.RemoveBuff(RoR2Content.Buffs.ArmorBoost);

                this.modelLocator.modelTransform.gameObject.AddComponent<Modules.Components.Snapped>();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}