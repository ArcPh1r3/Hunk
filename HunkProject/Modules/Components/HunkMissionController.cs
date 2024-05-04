using UnityEngine;

namespace HunkMod.Modules.Components
{
    public class HunkMissionController : MonoBehaviour
    {
        public static HunkMissionController instance;

        public int step;

        public void NextStep()
        {
            this.step++;
        }
    }
}