using UnityEngine;

namespace HunkMod.Modules.Components
{
    public class CVirusMaster : MonoBehaviour
    {
        public int revivalCount = 1;

        private void OnEnable()
        {
            Modules.Survivors.Hunk.virusObjectiveObjects3.Add(this.gameObject);
        }

        private void OnDisable()
        {
            //this.TrySpawn();
            Modules.Survivors.Hunk.virusObjectiveObjects3.Remove(this.gameObject);
        }
    }
}