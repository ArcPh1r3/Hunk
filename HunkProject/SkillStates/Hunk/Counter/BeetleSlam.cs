using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;

namespace HunkMod.SkillStates.Hunk.Counter
{
    public class BeetleSlam : BaseState
    {
        public float duration = 3f;
        public float animSpeed = 0.8132f;

        private Transform modelTransform;
        private ChildLocator childLocator;
        private Animator animator;
        private GameObject leftHandChargeEffect;
        private GameObject rightHandChargeEffect;

        private enum SubState
        {
            Start,
            Stopped,
            PushingBack,
            End
        }
        private SubState subState;

        public override void OnEnter()
        {
            base.OnEnter();
            this.modelTransform = this.GetModelTransform();
            this.childLocator = this.GetModelChildLocator();
            this.animator = this.GetModelAnimator();
            Util.PlaySound(EntityStates.BeetleGuardMonster.GroundSlam.initialAttackSoundString, base.gameObject);
            base.PlayCrossfade("Body", "GroundSlam", "GroundSlam.playbackRate", this.duration * this.animSpeed, 0.2f);
            this.subState = SubState.Start;

            if (this.modelTransform)
            {
                this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
                if (this.childLocator)
                {
                    GameObject original = EntityStates.BeetleGuardMonster.GroundSlam.chargeEffectPrefab;
                    Transform transform = this.childLocator.FindChild("HandL");
                    Transform transform2 = this.childLocator.FindChild("HandR");
                    if (transform)
                    {
                        this.leftHandChargeEffect = UnityEngine.Object.Instantiate<GameObject>(original, transform);
                    }
                    if (transform2)
                    {
                        this.rightHandChargeEffect = UnityEngine.Object.Instantiate<GameObject>(original, transform2);
                    }
                }
            }

            if (NetworkServer.active) this.characterBody.AddBuff(RoR2Content.Buffs.ArmorBoost);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            switch (this.subState)
            {
                case SubState.Start:
                    if (base.fixedAge >= 0.41f * this.duration)
                    {
                        this.subState = SubState.Stopped;
                        this.animator.SetFloat("GroundSlam.playbackRate", 0f);

                        if (this.leftHandChargeEffect) Destroy(this.leftHandChargeEffect);
                        if (this.rightHandChargeEffect) Destroy(this.rightHandChargeEffect);
                        Util.PlaySound("sfx_hunk_grapple", this.gameObject);
                    }
                    break;
                case SubState.Stopped:
                    if (base.fixedAge >= 0.5f * this.duration)
                    {
                        this.subState = SubState.PushingBack;
                        this.animator.SetFloat("GroundSlam.playbackRate", -0.1f);
                        if (NetworkServer.active) this.characterBody.RemoveBuff(RoR2Content.Buffs.ArmorBoost);
                    }
                    break;
                case SubState.PushingBack:
                    if (base.fixedAge >= 0.67f * this.duration)
                    {
                        this.subState = SubState.End;
                        base.PlayCrossfade("Body", "Idle", 0.5f);
                    }
                    break;
                case SubState.End:
                    if (base.isAuthority && base.fixedAge >= this.duration)
                    {
                        this.outer.SetNextStateToMain();
                    }
                    break;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (this.leftHandChargeEffect) Destroy(this.leftHandChargeEffect);
            if (this.rightHandChargeEffect) Destroy(this.rightHandChargeEffect);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}