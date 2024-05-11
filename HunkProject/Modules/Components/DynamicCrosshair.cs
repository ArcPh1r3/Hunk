using RoR2;
using RoR2.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HunkMod.Modules.Components
{
    [DisallowMultipleComponent]
    public class DynamicCrosshair : MonoBehaviour
    {
        public float range = 256f;
        public float interval = 0.15f;

        private HunkController hunk;
        private CrosshairController crosshairController;
        private Image[] crosshairSprites;
        private float stopwatch;
        private Vector3 originPos;

        private void Awake()
        {
            this.crosshairController = this.GetComponent<CrosshairController>();

            List<Image> hhhh = new List<Image>();

            foreach (CrosshairController.SpritePosition fuckYouDontTellMeToStopUsingIAsMyVariableName in this.crosshairController.spriteSpreadPositions)
            {
                if (fuckYouDontTellMeToStopUsingIAsMyVariableName.target) hhhh.Add(fuckYouDontTellMeToStopUsingIAsMyVariableName.target.GetComponent<Image>());
            }

            this.crosshairSprites = hhhh.ToArray();
            this.originPos = this.transform.position;
        }

        private void FixedUpdate()
        {
            this.stopwatch -= Time.fixedDeltaTime;

            if (this.stopwatch <= 0f)
            {
                this.Simulate();
            }
        }

        private void LateUpdate()
        {
            /*if (this.crosshairController && this.crosshairController.hudElement)
            {
                if (this.crosshairController.hudElement.targetCharacterBody && this.crosshairController.hudElement.targetCharacterBody.hasAuthority)
                {
                    if (!this.hunk)
                    {
                        this.hunk = this.crosshairController.hudElement.targetCharacterBody.GetComponent<HunkController>();
                    }

                    if (this.hunk)
                    {
                        if (this.hunk.lockOnTimer > 0f && this.hunk.targetHurtbox)
                        {
                            Vector3 worldPos = new Vector3(this.hunk.targetHurtbox.transform.position.x, this.hunk.targetHurtbox.transform.position.y, this.hunk.targetHurtbox.transform.position.z);
                            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
                            this.transform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);
                        }
                        else this.transform.position = this.originPos;
                    }
                }
            }*/
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