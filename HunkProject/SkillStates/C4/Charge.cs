using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.AddressableAssets;

namespace HunkMod.SkillStates.C4
{
    public class Charge : BaseState
    {
        private GameObject chargeEffectInstance;

        public override void OnEnter()
        {
            base.OnEnter();

            Util.PlaySound("sfx_hunk_button_press", this.gameObject);

            this.chargeEffectInstance = GameObject.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/QuestVolatileBattery/VolatileBatteryPreDetonation.prefab").WaitForCompletion(),
                this.transform.position + Vector3.up,
                this.transform.rotation);
        }

        public override void OnExit()
        {
            base.OnExit();
            if (this.chargeEffectInstance) Destroy(this.chargeEffectInstance);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            
            if (base.fixedAge >= 2f && base.isAuthority)
            {
                this.outer.SetNextState(new Detonate());
            }
        }
    }
}