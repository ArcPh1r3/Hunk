using RoR2;
using RoR2.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HunkMod.Modules.Components
{
    public class DynamicCrosshair : MonoBehaviour
    {
        public float range = 256f;
        public float interval = 0.15f;

        private CrosshairController crosshairController;
        private Image[] crosshairSprites;
        private float stopwatch;

        private void Awake()
        {
            this.crosshairController = this.GetComponent<CrosshairController>();

            List<Image> hhhh = new List<Image>();

            foreach (CrosshairController.SpritePosition fuckYouDontTellMeToStopUsingIAsMyVariableName in this.crosshairController.spriteSpreadPositions)
            {
                if (fuckYouDontTellMeToStopUsingIAsMyVariableName.target) hhhh.Add(fuckYouDontTellMeToStopUsingIAsMyVariableName.target.GetComponent<Image>());
            }

            this.crosshairSprites = hhhh.ToArray();
        }

        private void FixedUpdate()
        {
            this.stopwatch -= Time.fixedDeltaTime;

            if (this.stopwatch <= 0f)
            {
                this.Simulate();
            }
        }

        private void Simulate()
        {
            this.stopwatch = this.interval;

            if (this.crosshairController && this.crosshairController.hudElement)
            {
                if (this.crosshairController.hudElement.targetCharacterBody && this.crosshairController.hudElement.targetCharacterBody.hasAuthority)
                {
                    Vector3 origin = this.crosshairController.hudElement.targetCharacterBody.aimOrigin;
                    Ray aimRay = this.crosshairController.hudElement.targetCharacterBody.inputBank.GetAimRay();

                    // check if there's something in front of the crosshair
                    RaycastHit raycastHit;
                    if (Physics.Raycast(aimRay, out raycastHit, this.range, LayerIndex.CommonMasks.bullet, QueryTriggerInteraction.Collide))
                    {
                        if (raycastHit.collider)
                        {
                            GameObject target = raycastHit.collider.gameObject;

                            HurtBox hurtbox = target.GetComponent<HurtBox>();
                            if (hurtbox)
                            {
                                if (hurtbox.healthComponent && hurtbox.healthComponent.body == this.crosshairController.hudElement.targetCharacterBody)
                                {
                                    this.ColorCrosshair(Color.white);
                                    return;
                                }

                                if (hurtbox.healthComponent.body.teamComponent.teamIndex == TeamIndex.Player) this.ColorCrosshair(Color.green);
                                else this.ColorCrosshair(Color.red);
                            }
                            else
                            {
                                this.ColorCrosshair(Color.white);
                            }
                        }
                    }
                    else
                    {
                        this.ColorCrosshair(Color.white);
                    }
                }
            }
        }

        private void ColorCrosshair(Color newColor)
        {
            if (this.crosshairSprites != null && this.crosshairSprites.Length > 0)
            {
                for (int i = 0; i < this.crosshairSprites.Length; i++)
                {
                    if (this.crosshairSprites[i]) this.crosshairSprites[i].color = newColor;
                }
            }
        }
    }
}