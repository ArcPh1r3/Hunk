using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class IncompatWarning : MonoBehaviour
    {
        private HunkController hunk;

        private void Awake()
        {
            this.hunk = this.GetComponent<HunkController>();
            this.Invoke("Warning", 1f);
        }

        private void Warning()
        {
            this.hunk.notificationHandler.Init("Incompatible mod detected!\nDisable MoreShrines or there will be issues!", Color.red, 3f);
            this.Invoke("Warning", 3.5f);
        }
    }
}