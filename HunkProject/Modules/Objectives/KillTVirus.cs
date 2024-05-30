using RoR2;
using RoR2.UI;

namespace HunkMod.Modules.Objectives
{
	public class KillTVirus : ObjectivePanelController.ObjectiveTracker
	{
		public KillTVirus()
		{
			this.baseToken = "Stop the <color=#" + Helpers.lunarItemHex + ">T-Virus" + Helpers.colorSuffix + " outbreak";
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