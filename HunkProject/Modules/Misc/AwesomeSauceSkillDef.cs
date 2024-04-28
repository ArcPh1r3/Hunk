using UnityEngine;
using RoR2;
using RoR2.Skills;
using JetBrains.Annotations;
using EntityStates;

namespace HunkMod.Modules.Misc
{
    public class AwesomeSauceSkillDef : SkillDef
    {
		public override EntityState InstantiateNextState([NotNull] GenericSkill skillSlot)
		{
			if (skillSlot && skillSlot.characterBody && skillSlot.characterBody.characterMotor && !skillSlot.characterBody.characterMotor.isGrounded)
            {
				EntityState entityState = EntityStateCatalog.InstantiateState(Modules.Survivors.Hunk.airDodgeState);
				ISkillState skillState;
				if ((skillState = (entityState as ISkillState)) != null)
				{
					skillState.activatorSkillSlot = skillSlot;
				}
				return entityState;
			}

			return base.InstantiateNextState(skillSlot);
		}
	}
}