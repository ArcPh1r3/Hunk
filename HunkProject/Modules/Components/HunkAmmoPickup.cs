using UnityEngine;
using RoR2;
using UnityEngine.Networking;

namespace HunkMod.Modules.Components
{
	public class HunkAmmoPickup : MonoBehaviour
	{
		[Tooltip("The base object to destroy when this pickup is consumed.")]
		public GameObject baseObject;
		[Tooltip("The team filter object which determines who can pick up this pack.")]
		public TeamFilter teamFilter;

		public GameObject pickupEffect;

		private bool alive = true;

		private void Awake()
		{
			// disable visuals for non hunk
			BeginRapidlyActivatingAndDeactivating blinker = this.transform.parent.GetComponentInChildren<BeginRapidlyActivatingAndDeactivating>();
			if (blinker)
			{
				bool isHunk = false;

				var localPlayers = LocalUserManager.readOnlyLocalUsersList;
				foreach (LocalUser i in localPlayers)
				{
					if (i.cachedBody.baseNameToken == Modules.Survivors.Hunk.bodyNameToken) isHunk = true;
				}

				if (!isHunk)
				{
					blinker.blinkingRootObject.SetActive(false);
					Destroy(blinker);
				}
			}
		}

		private void OnTriggerStay(Collider collider)
		{
			if (NetworkServer.active && this.alive)
			{
				HunkController hunk = collider.GetComponent<HunkController>();
				if (hunk)
				{
					this.alive = false;

					hunk.ServerGetAmmo();
					EffectManager.SimpleEffect(this.pickupEffect, this.transform.position, Quaternion.identity, true);
					UnityEngine.Object.Destroy(this.baseObject);
				}
			}
		}
	}
}