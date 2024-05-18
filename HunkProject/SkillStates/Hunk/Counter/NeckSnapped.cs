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
            if (this.gameObject.name.Contains("Lemurian")) this.modelLocator.modelTransform.localScale = new Vector3(0.1f, 0.13f, 0.1f);
            if (this.gameObject.name.Contains("Clay")) this.modelLocator.modelTransform.localScale = new Vector3(1.1f, 1.23f, 1.4f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!this.hasSnapped && base.fixedAge >= 0.454f * this.duration)
            {
                this.hasSnapped = true;
                if (NetworkServer.active) this.characterBody.RemoveBuff(RoR2Content.Buffs.ArmorBoost);

                // their death is an explosion anyway
                //if (this.gameObject.name.Contains("Clay")) this.modelLocator.modelTransform.localScale = new Vector3(1.3f, 1.3f, 1.3f);

                this.modelLocator.modelTransform.gameObject.AddComponent<Modules.Components.Snapped>();
            }

            if (base.fixedAge >= this.duration && this.healthComponent && this.healthComponent.alive)
            {
                if (NetworkServer.active) this.characterBody.AddTimedBuff(RoR2Content.Buffs.WarCryBuff, 10f);
                if (base.isAuthority) this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}