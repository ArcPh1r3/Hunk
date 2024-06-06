using RoR2;
using RoR2.UI;

namespace HunkMod.Modules.Objectives
{
	public class TurnInCSample : ObjectivePanelController.ObjectiveTracker
	{
		public TurnInCSample()
		{
			this.baseToken = "Take the <color=#" + Helpers.redItemHex + ">C-Virus Sample" + Helpers.colorSuffix + " to the extraction point";
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