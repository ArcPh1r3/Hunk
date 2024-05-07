using UnityEngine;
using RoR2;
using RoR2.UI;

namespace HunkMod.Modules.Components
{
    public class InfectionText : MonoBehaviour
    {
        public CharacterBody target;
        public HGTextMeshProUGUI text;

        private void FixedUpdate()
        {
            if (this.target && this.text)
            {
                if (this.target.inventory && this.target.inventory.GetItemCount(Modules.Survivors.Hunk.gVirus) > 0)
                {
                    this.text.text = this.target.inventory.GetItemCount(Modules.Survivors.Hunk.gVirus).ToString();
                }
                else this.text.text = "";
            }
        }
    }
}