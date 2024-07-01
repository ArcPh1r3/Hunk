using UnityEngine;
using RoR2;
using RoR2.Orbs;

namespace HunkMod.Modules.Misc
{
	public class HunkBulletOrb : GenericDamageOrb
	{
		public override void Begin()
		{
			this.speed = 320f;
			base.Begin();
		}

		public override GameObject GetOrbEffect()
		{
			return Modules.Assets.bulletOrbEffect;
		}
	}
}