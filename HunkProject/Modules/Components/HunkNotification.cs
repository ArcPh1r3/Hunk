using RoR2;
using RoR2.UI;

namespace HunkMod.Modules.Components
{
	public class HunkNotification : GenericNotification
	{
		public void SetText(string newToken)
		{
			this.titleText.token = MainPlugin.developerPrefix + "ITEM_EFFECT_TITLE";
			this.descriptionText.token = newToken;

			this.iconImage.texture = Modules.Assets.LoadCharacterIcon("Hunk");

			this.titleTMP.color = Modules.Survivors.Hunk.characterColor;
		}
	}
}