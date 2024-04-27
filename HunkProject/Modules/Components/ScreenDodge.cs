using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.Modules.Components
{
	public class ScreenDodge : MonoBehaviour
	{
		private CameraRigController cameraRigController;
		public Material mat = Modules.Assets.dodgeOverlayMat;
		public float DistortionScale = 0.03f;
		public float DistortionPower = 4.84f;
		public float DesaturationScale = 0.9f;
		public float DesaturationPower = 2f;
		public float TintScale = 2f;
		public float TintPower = 2.36f;

		private HunkController hunk;
		private bool wellShit;

		private void Awake()
		{
			this.cameraRigController = base.GetComponentInParent<CameraRigController>();
			this.mat = UnityEngine.Object.Instantiate<Material>(this.mat);
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			float value = 0f;

			if (this.cameraRigController)
			{
				if (this.cameraRigController.target)
				{
					if (!this.hunk && !this.wellShit)
                    {
						this.wellShit = true;
						this.hunk = this.cameraRigController.target.GetComponent<HunkController>();
                    }

					if (this.hunk)
                    {
						value = Mathf.Clamp01(Util.Remap(this.hunk.lockOnTimer, 0f, 1f, 0f, 1f));

                    }
				}
				this.mat.SetFloat("_DistortionStrength", Util.Remap(value, 0f, 1f, 0f, this.DistortionPower) * this.DistortionScale);
				this.mat.SetFloat("_DesaturationStrength", Util.Remap(value, 0f, 1f, 0f, this.DesaturationPower) * this.DesaturationScale);
				this.mat.SetFloat("_TintStrength", Util.Remap(value, 0f, 1f, 0f, this.TintPower) * this.TintScale);
			}
			Graphics.Blit(source, destination, this.mat);
		}
	}
}