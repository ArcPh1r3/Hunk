using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class HunkDialogue2 : MonoBehaviour
    {
        private HunkController hunk;
        // this will actually be fully hardcoded as i can't be bothered.
        private void Awake()
        {
            this.hunk = this.GetComponent<HunkController>();

            this.Invoke("Step1", 3f);
        }

        private void Step1()
        {
            Util.PlaySound("vo_hunk_04", this.gameObject);
            this.hunk.notificationHandler.Init("What the hell, Hunk? You're late for extraction.", Color.white, 3f);
            this.Invoke("Step2", 3.2f);
        }

        private void Step2()
        {
            Util.PlaySound("vo_hunk_05", this.gameObject);
            this.hunk.notificationHandler.Init("Door's blocked. Gotta find another way out.", Color.white, 3.1f);
            this.Invoke("Step3", 3.3f);
        }

        private void Step3()
        {
            Util.PlaySound("vo_hunk_06", this.gameObject);
            this.hunk.notificationHandler.Init("Heads up, guys at the top just ordered a full clean up.", Color.white, 3.2f);
            this.Invoke("Step4", 3.4f);
        }

        private void Step4()
        {
            this.hunk.notificationHandler.Init("So move fast, or you can kiss your ass goodbye.", Color.white, 2.8f);
        }
    }
}