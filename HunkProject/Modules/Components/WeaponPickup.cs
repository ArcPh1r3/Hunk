using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;

namespace HunkMod.Modules.Components
{
    public class WeaponPickup : MonoBehaviour
    {
		[Tooltip("The base object to destroy when this pickup is consumed.")]
		public GameObject baseObject;
		[Tooltip("The team filter object which determines who can pick up this pack.")]
		public TeamFilter teamFilter;
		public HunkWeaponDef weaponDef;

		public GameObject pickupEffect;
		public bool cutAmmo = false;
		public bool isAmmoBox = false;

		private bool alive = true;

		private void Awake()
        {
			// disable visuals for non driver
			BeginRapidlyActivatingAndDeactivating blinker = this.transform.parent.GetComponentInChildren<BeginRapidlyActivatingAndDeactivating>();
			if (blinker)
			{
				bool isDriver = false;

				var localPlayers = LocalUserManager.readOnlyLocalUsersList;
				foreach (LocalUser i in localPlayers)
				{
					if (i.cachedBody.baseNameToken == Modules.Survivors.Hunk.bodyNameToken) isDriver = true;
				}

				if (!isDriver)
				{
					blinker.blinkingRootObject.SetActive(false);
					Destroy(blinker);
				}
			}
		}

		private void Start()
        {
			this.SetWeapon(this.weaponDef, this.cutAmmo, this.isAmmoBox);
		}

		public void ServerSetWeapon(HunkWeaponDef newWeaponDef)
        {
			// this didn't work lole
			this.weaponDef = newWeaponDef;

			if (NetworkServer.active)
			{
				NetworkIdentity identity = this.transform.root.GetComponentInChildren<NetworkIdentity>();
				if (!identity) return;

				new SyncWeaponPickup(identity.netId, (ushort)this.weaponDef.index, this.cutAmmo).Send(NetworkDestination.Clients);
			}
		}

		public void SetWeapon(HunkWeaponDef newWeapon, bool _cutAmmo = false, bool _isAmmoBox = false)
        {
			this.weaponDef = newWeapon;
			this.cutAmmo = _cutAmmo;
			this.isAmmoBox = _isAmmoBox;

			// wow this is awful!
			RoR2.UI.LanguageTextMeshController textComponent = this.transform.parent.GetComponentInChildren<RoR2.UI.LanguageTextMeshController>();
			if (textComponent)
			{
				if (!this.weaponDef)
				{
					// band-aid i don't have the time to keep fighting with this code rn
					textComponent.token = "FUCK YOU FUCK YOU FUCK/nYOU FUCK YOU FUCK YOU";
					return;
				}

				textComponent.token = this.weaponDef.nameToken;

				if (this.cutAmmo)
                {
					textComponent.textMeshPro.color = Modules.Helpers.badColor;
				}
				else
                {
					textComponent.textMeshPro.color = Modules.Helpers.yellowItemColor;
				}
			}
		}

		private void OnTriggerStay(Collider collider)
		{
			if (NetworkServer.active && this.alive/* && TeamComponent.GetObjectTeam(collider.gameObject) == this.teamFilter.teamIndex*/)
			{
				HunkController iDrive = collider.GetComponent<HunkController>();
				if (iDrive)
				{
					this.alive = false;

					iDrive.ServerPickUpWeapon(this.weaponDef, this.cutAmmo, iDrive, this.isAmmoBox);
					EffectManager.SimpleEffect(this.pickupEffect, this.transform.position, Quaternion.identity, true);
					UnityEngine.Object.Destroy(this.baseObject);
				}
			}
		}
	}
}