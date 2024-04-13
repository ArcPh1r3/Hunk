using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class Decapitation : MonoBehaviour
    {
        private Transform headTransform;

        private void Awake()
        {
            CharacterBody characterBody = this.GetComponent<CharacterBody>();
            if (characterBody)
            {
                ModelLocator modelLocator = characterBody.modelLocator;
                if (modelLocator)
                {
                    Transform modelTransform = modelLocator.modelTransform;
                    if (modelTransform)
                    {
                        ChildLocator childLocator = modelTransform.GetComponent<ChildLocator>();
                        if (childLocator)
                        {
                            Transform head = childLocator.FindChild("Head");
                            Transform head2 = childLocator.FindChild("HeadCenter");

                            if (head) this.headTransform = head;
                            if (!head && head2) this.headTransform = head2;
                        }
                    }
                }
            }

            if (this.headTransform)
            {
                EffectManager.SpawnEffect(Modules.Assets.bloodExplosionEffect, new EffectData
                {
                    origin = this.headTransform.position,
                    rotation = Quaternion.identity,
                    scale = 1f
                }, false);

                GameObject.Instantiate(Modules.Assets.bloodSpurtEffect, this.headTransform);
                Util.PlaySound("sfx_driver_blood_gurgle", this.headTransform.gameObject);
            }
        }

        private void LateUpdate()
        {
            if (this.headTransform) this.headTransform.localScale = Vector3.zero;
        }
    }
}