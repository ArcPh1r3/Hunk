using RoR2;
using UnityEngine;

namespace HunkMod.Modules.Components
{
    public class GolemEyeDisabler : MonoBehaviour
    {
        public float lifetime = 60f;

        private ModelLocator modelLocator;
        private SkillLocator skillLocator;
        private CharacterModel characterModel;
        private float originalEmissionPower;
    
        private void Awake()
        {
            this.modelLocator = this.GetComponent<ModelLocator>();
            this.skillLocator = this.GetComponent<SkillLocator>();
            if (this.modelLocator) this.characterModel = this.modelLocator.modelTransform.GetComponent<CharacterModel>();

            if (this.characterModel)
            {
                this.originalEmissionPower = this.characterModel.baseRendererInfos[0].defaultMaterial.GetFloat("_EmPower");
            }
        }

        private void FixedUpdate()
        {
            this.lifetime -= Time.fixedDeltaTime;

            if (this.lifetime <= 0f)
            {
                if (this.characterModel)
                {
                    this.characterModel.baseRendererInfos[0].defaultMaterial.SetFloat("_EmPower", this.originalEmissionPower);
                    this.characterModel.baseLightInfos[0].light.gameObject.SetActive(true);
                }

                Destroy(this);
                return;
            }

            if (this.skillLocator)
            {
                this.skillLocator.secondary.stock = 0;
                this.skillLocator.secondary.rechargeStopwatch = -0.1f;
            }

            if (this.characterModel)
            {
                this.characterModel.baseRendererInfos[0].defaultMaterial.SetFloat("_EmPower", 0f);
                this.characterModel.baseLightInfos[0].light.gameObject.SetActive(false);
            }
        }
    }
}