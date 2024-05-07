using RoR2;
using RoR2.UI;

namespace HunkMod.Modules.Objectives
{
	public class KillVirus : ObjectivePanelController.ObjectiveTracker
	{
		public KillVirus()
		{
			this.baseToken = "Neutralize the <color=#" + Helpers.voidItemHex + ">G-Virus" + Helpers.colorSuffix;
		}

		public override string GenerateString()
		{
			return Language.GetString(this.baseToken);
		}

		public override bool IsDirty()
		{
			return true;
		}
	}
}