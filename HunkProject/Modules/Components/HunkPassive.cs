using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace HunkMod.Modules.Components
{
    public class HunkPassive : MonoBehaviour
    {
        public SkillDef rummagePassive;
        public SkillDef fullArsenalPassive;
        public GenericSkill passiveSkillSlot;

        public bool isRummage
        {
            get
            {
                if (this.rummagePassive && this.passiveSkillSlot)
                {
                    return this.passiveSkillSlot.skillDef == this.rummagePassive;
                }

                return false;
            }
        }

        public bool isFullArsenal
        {
            get
            {
                if (this.fullArsenalPassive && this.passiveSkillSlot)
                {
                    return this.passiveSkillSlot.skillDef == this.fullArsenalPassive;
                }

                return false;
            }
        }
    }
}