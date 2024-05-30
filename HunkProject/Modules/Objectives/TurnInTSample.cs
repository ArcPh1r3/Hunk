using RoR2;
using RoR2.UI;

namespace HunkMod.Modules.Objectives
{
	public class TurnInTSample : ObjectivePanelController.ObjectiveTracker
	{
		public TurnInTSample()
		{
			this.baseToken = "Take the <color=#" + Helpers.lunarItemHex + ">T-Virus Sample" + Helpers.colorSuffix + " to the extraction point";
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