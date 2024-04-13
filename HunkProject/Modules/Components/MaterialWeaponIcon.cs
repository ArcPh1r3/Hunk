using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using TMPro;

namespace HunkMod.Modules.Components
{
	public class MaterialWeaponIcon : MonoBehaviour
	{
		public WeaponIcon icon;
		public Image onCooldown;
		public Image mask;
		public TextMeshProUGUI stockText;

		public void Update()
		{
			if (this.icon.iDrive)
            {
				if (this.icon.iDrive.maxAmmo > 0f)
                {
					float value = Util.Remap(this.icon.iDrive.ammo, 0f, this.icon.iDrive.maxAmmo, 0f, 1f);
					this.mask.fillAmount = value;
                }
				else
                {
					this.mask.fillAmount = 0f;
                }

				this.onCooldown.enabled = false;
            }
		}
	}
}