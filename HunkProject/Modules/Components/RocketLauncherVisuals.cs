using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class RocketLauncherVisuals : MonoBehaviour
    {
        // this whole thing is entirely hardcoded but i'm too tired to do it efficiently- this won't ever be reused anyways so blow me
        private GameObject rocket0;
        private GameObject rocket1;
        private GameObject rocket2;
        private GameObject rocket3;
        private HunkController hunk;

        private void Awake()
        {
            this.rocket0 = this.transform.Find("Model/24_+Additions|Rocket Lower Left.rocketll_0.3_0_0").gameObject;
            this.rocket1 = this.transform.Find("Model/24_+Additions|Rocket Lower Right.rocketlr_0.3_0_0").gameObject;
            this.rocket2 = this.transform.Find("Model/24_+Additions|Rocket Upper Left.rocketul_0.3_0_0").gameObject;
            this.rocket3 = this.transform.Find("Model/24_+Additions|Rocket Upper Right.rocketur_0.3_0_0").gameObject;
        }

        private void GetHunkController()
        {
            if (!this.hunk)
            {
                CharacterModel cm = this.GetComponentInParent<CharacterModel>();
                if (cm)
                {
                    CharacterBody cb = cm.body;
                    if (cb)
                    {
                        this.hunk = cb.GetComponent<HunkController>();
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (this.hunk)
            {
                if (this.hunk.weaponDef == Modules.Weapons.RocketLauncher.instance.weaponDef)
                {
                    switch (this.hunk.ammo)
                    {
                        case 0:
                            this.rocket0.SetActive(false);
                            this.rocket1.SetActive(false);
                            this.rocket2.SetActive(false); // lol
                            this.rocket3.SetActive(false);
                            break;
                        case 1:
                            this.rocket0.SetActive(true);
                            this.rocket1.SetActive(false);
                            this.rocket2.SetActive(false);
                            this.rocket3.SetActive(false);
                            break;
                        case 2:
                            this.rocket0.SetActive(true);
                            this.rocket1.SetActive(true);
                            this.rocket2.SetActive(false);
                            this.rocket3.SetActive(false);
                            break;
                        case 3:
                            this.rocket0.SetActive(true);
                            this.rocket1.SetActive(true);
                            this.rocket2.SetActive(true);
                            this.rocket3.SetActive(false);
                            break;
                        default:
                            this.rocket0.SetActive(true); //    :-)
                            this.rocket1.SetActive(true);
                            this.rocket2.SetActive(true);
                            this.rocket3.SetActive(true);
                            break;
                    }
                }
            }
            else this.GetHunkController();
        }
    }
}