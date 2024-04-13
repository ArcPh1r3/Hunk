using RoR2;
using RoR2.UI;

namespace HunkMod.Modules.Components
{
    public class WeaponNotification : GenericNotification
    {
		public void SetWeapon(HunkWeaponDef weaponDef)
		{
			this.titleText.token = weaponDef.nameToken;
			this.descriptionText.token = weaponDef.descriptionToken;

			if (weaponDef.icon != null)
			{
				this.iconImage.texture = weaponDef.icon;
			}

			this.titleTMP.color = Modules.Helpers.greenItemColor;
		}
	}
}