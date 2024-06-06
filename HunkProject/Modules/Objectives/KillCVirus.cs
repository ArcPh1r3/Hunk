using RoR2;
using RoR2.UI;

namespace HunkMod.Modules.Objectives
{
	public class KillCVirus : ObjectivePanelController.ObjectiveTracker
	{
		public KillCVirus()
		{
			this.baseToken = "Eradicate the <color=#" + Helpers.redItemHex + ">C-Virus" + Helpers.colorSuffix + "";
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