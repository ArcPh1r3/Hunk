using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates;

namespace HunkMod.SkillStates.Parasite
{
	public class Death : GenericCharacterDeath
	{
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge > EntityStates.VoidInfestor.Death.deathDelay && NetworkServer.active && !this.hasDied)
			{
				this.hasDied = true;

				//if (this.characterBody.master.GetComponent<Modules.Components.KeycardHolder>().itemDef)
				//	PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(this.characterBody.master.GetComponent<Modules.Components.KeycardHolder>().itemDef.itemIndex), this.characterBody.corePosition, Vector3.up * 20f);
				if (this.characterBody.master.GetComponent<Modules.Components.KeycardHolder>())
				{
					PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(Modules.Survivors.Hunk.gVirusSample.itemIndex), this.characterBody.corePosition, Vector3.up * 20f);
					//if (Modules.Components.HunkMissionController.instance) Modules.Components.HunkMissionController.instance.NextStep();
				}

				EffectManager.SimpleImpactEffect(EntityStates.VoidInfestor.Death.deathEffectPrefab, base.characterBody.corePosition, Vector3.up, true);
				base.DestroyBodyAsapServer();
				EntityState.Destroy(base.gameObject);
			}
		}

		public override void OnExit()
		{
			base.DestroyModel();
			base.OnExit();
		}

		private bool hasDied;
	}
}