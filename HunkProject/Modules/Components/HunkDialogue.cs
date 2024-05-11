using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class HunkDialogue : MonoBehaviour
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
            Util.PlaySound("vo_hunk_01", this.gameObject);
            this.hunk.notificationHandler.Init("This is Nighthawk. Come in, Alpha.", Color.white, 2.65f);
            this.Invoke("Step2", 2.85f);
        }

        private void Step2()
        {
            this.hunk.notificationHandler.Init("Alpha, do you read?", Color.white, 1.2f);
            this.Invoke("Step3", 1.3f);
        }

        private void Step3()
        {
            Util.PlaySound("vo_hunk_02", this.gameObject);
            this.hunk.notificationHandler.Init("Nighthawk, this is Hunk from Alpha Team.", Color.white, 3f);
            this.Invoke("Step4", 3.2f);
        }

        private void Step4()
        {
            Util.PlaySound("vo_hunk_03", this.gameObject);
            this.hunk.notificationHandler.Init("Man, I thought you were all wiped out.\nI've been trying to-", Color.white, 3.1f);
            this.Invoke("Step5", 3.3f);
        }

        private void Step5()
        {
            this.hunk.notificationHandler.Init("I'm at Point K12. Need info on my extraction.", Color.white, 4f);
            this.Invoke("Step6", 4.2f);
        }

        private void Step6()
        {
            this.hunk.notificationHandler.Init("Guess there's no keeping down the Grim Reaper, huh?", Color.white, 2.6f);
            this.Invoke("Step7", 2.8f);
        }

        private void Step7()
        {
            this.hunk.notificationHandler.Init("My extraction point!", Color.white, 1.3f);
            this.Invoke("Step8", 1.5f);
        }

        private void Step8()
        {
            this.hunk.notificationHandler.Init("Relax, Mr. Reaper.", Color.white, 2.2f);
            this.Invoke("Step9", 2.4f);
        }

        private void Step9()
        {
            this.hunk.notificationHandler.Init("I'm headed towards the front gate of R.P.D.\nPick you up there.", Color.white, 3.3f);
            this.Invoke("Step10", 3.5f);
        }

        private void Step10()
        {
            this.hunk.notificationHandler.Init("Got it.", Color.white, 1.5f);
        }
    }
}