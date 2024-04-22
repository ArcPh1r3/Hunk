using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using TMPro;

namespace HunkMod.Modules.Components
{
    public class WeaponIcon : MonoBehaviour
    {
		public HUD targetHUD;
		public HunkController hunk;

		public GameObject displayRoot;
		public PlayerCharacterMasterController playerCharacterMasterController;
		public RawImage iconImage;

		public GameObject flashPanelObject;
		public GameObject reminderFlashPanelObject;
		public GameObject isReadyPanelObject;
		public TooltipProvider tooltipProvider;
		public GameObject durationDisplay;
		public Image durationBar;
		public Image durationBarRed;

		private void Update()
        {
			// REWRITE THIS ASAP
			if (!this.hunk)
            {
				if (!this.playerCharacterMasterController)
                {
					this.playerCharacterMasterController = (this.targetHUD.targetMaster ? this.targetHUD.targetMaster.GetComponent<PlayerCharacterMasterController>() : null);
				}

				if (this.playerCharacterMasterController && this.playerCharacterMasterController.master.hasBody)
                {
					HunkController fuckYou = this.playerCharacterMasterController.master.GetBody().GetComponent<HunkController>();
					if (fuckYou) this.SetTarget(fuckYou);
                }
            }
			else
            {
				this.UpdateDisplay();
            }
        }

		public void SetTarget(HunkController h)
        {
			this.hunk = h;
			this.hunk.onWeaponUpdate += this.SetDisplay;
			this.SetDisplay(this.hunk);
        }

		private void UpdateDisplay()
        {
			if (this.hunk.maxAmmo > 0f)
            {
				//this.durationDisplay.SetActive(true);
				this.durationDisplay.SetActive(false);

				/*float fill = Util.Remap(this.iDrive.ammo, 0f, this.iDrive.maxAmmo, 0f, 1f);

				if (this.durationBarRed)
				{
					if (fill >= 1f) this.durationBarRed.fillAmount = 1f;
					this.durationBarRed.fillAmount = Mathf.Lerp(this.durationBarRed.fillAmount, fill, Time.deltaTime * 2f);
				}

				this.durationBar.fillAmount = fill;*/
            }
			else
            {
				this.durationDisplay.SetActive(false);
            }
        }

		private void SetDisplay(HunkController z)
		{
			if (!this.hunk) return;

			this.DoStockFlash();

            if (this.displayRoot) this.displayRoot.SetActive(true); 
            if (this.isReadyPanelObject) this.isReadyPanelObject.SetActive(true);

            if (this.iconImage)
			{
				this.iconImage.texture = this.hunk.weaponDef.icon.texture;
				this.iconImage.color = Color.white;
				this.iconImage.enabled = true;
			}

            if (this.tooltipProvider)
			{
				this.tooltipProvider.titleToken = this.hunk.weaponDef.nameToken;
				this.tooltipProvider.bodyToken = this.hunk.weaponDef.descriptionToken;
				this.tooltipProvider.titleColor = Modules.Survivors.Hunk.characterColor;
				this.tooltipProvider.bodyColor = Color.gray;
			}
        }

        private void DoReminderFlash()
		{
			if (this.reminderFlashPanelObject)
			{
				AnimateUIAlpha animateUI = this.reminderFlashPanelObject.GetComponent<AnimateUIAlpha>();
				if (animateUI) animateUI.time = 0f;
				this.reminderFlashPanelObject.SetActive(true);
			}
		}

		private void DoStockFlash()
		{
			this.DoReminderFlash();
			if (this.flashPanelObject)
			{
				AnimateUIAlpha animateUI = this.flashPanelObject.GetComponent<AnimateUIAlpha>();
				if (animateUI) animateUI.time = 0f;
				this.flashPanelObject.SetActive(true);
			}
		}
	}
}