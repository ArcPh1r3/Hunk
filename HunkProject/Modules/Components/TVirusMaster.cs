using UnityEngine;

namespace HunkMod.Modules.Components
{
    public class TVirusMaster : MonoBehaviour
    {
        public int revivalCount = 2;

        private void OnEnable()
        {
            Modules.Survivors.Hunk.virusObjectiveObjects2.Add(this.gameObject);
        }

        private void OnDisable()
        {
            //this.TrySpawn();
            Modules.Survivors.Hunk.virusObjectiveObjects2.Remove(this.gameObject);
        }
    }
}