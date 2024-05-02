using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class Snapped : MonoBehaviour
    {
        public Vector3 neckRotation = new Vector3(180f, 0f, 0f);

        private Transform headTransform;

        private void Awake()
        {
            CharacterBody characterBody = this.GetComponent<CharacterBody>();
            Transform modelTransform = this.transform;
            if (characterBody && characterBody.modelLocator && characterBody.modelLocator.modelTransform) modelTransform = characterBody.modelLocator.modelTransform;

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

            if (this.headTransform)
            {
                EffectManager.SpawnEffect(Modules.Assets.kickImpactEffect, new EffectData
                {
                    origin = this.headTransform.position,
                    rotation = Quaternion.identity,
                    scale = 0.25f
                }, false);

                //GameObject.Instantiate(Modules.Assets.bloodSpurtEffect, this.headTransform);
                //Util.PlaySound("sfx_hunk_blood_gurgle", this.headTransform.gameObject);
            }
        }

        private void LateUpdate()
        {
            if (this.headTransform) this.headTransform.localRotation = Quaternion.Euler(this.neckRotation);
        }
    }
}