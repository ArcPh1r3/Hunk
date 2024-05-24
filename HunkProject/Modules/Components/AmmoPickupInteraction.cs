using System;
using System.Runtime.InteropServices;
using EntityStates.Barrel;
using RoR2;
using RoR2.Networking;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace HunkMod.Modules.Components
{
	public sealed class AmmoPickupInteraction : NetworkBehaviour, IInteractable, IDisplayNameProvider
	{
		public string displayNameToken = "ROB_HUNK_AMMO_NAME";
		public string contextToken = "ROB_HUNK_AMMO_CONTEXT";
		public float multiplier = 1f;
		public GameObject destroyOnOpen;

		[SyncVar]
		private bool opened;

		public string GetContextString(Interactor activator)
		{
			return Language.GetString(this.contextToken);
		}

		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		public Interactability GetInteractability(Interactor activator)
		{
			if (this.opened)
			{
				return Interactability.Disabled;
			}

			if (activator)
            {
				CharacterBody cb = activator.GetComponent<CharacterBody>();
				if (cb && cb.baseNameToken != Modules.Survivors.Hunk.bodyNameToken) return Interactability.Disabled;
				else
                {
					if (cb)
                    {
						HunkPassive passive = cb.GetComponent<HunkPassive>();
						if (passive.isFullArsenal) return Interactability.Disabled;
                    }
                }
            }

			return Interactability.Available;
		}

		[Server]
		public void OnInteractionBegin(Interactor activator)
		{
			if (!NetworkServer.active) return;

			if (!this.opened)
			{
				this.Networkopened = true;
				//EntityStateMachine esm = base.GetComponent<EntityStateMachine>();
				//if (esm) esm.SetNextState(new Opening());
				/*HunkController hunk = activator.GetComponent<HunkController>();
				if (hunk)
				{
					hunk.ServerGetAmmo(this.multiplier);
				}*/

				AmmoOrb ammoOrb = new AmmoOrb();
				ammoOrb.origin = this.transform.position;
				ammoOrb.target = Util.FindBodyMainHurtBox(activator.GetComponent<CharacterBody>());
				OrbManager.instance.AddOrb(ammoOrb);

				if (this.destroyOnOpen) Destroy(this.destroyOnOpen);
				NetworkServer.Destroy(this.gameObject);
			}
		}

		public string GetDisplayName()
		{
			return Language.GetString(this.displayNameToken);
		}

		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		public void OnEnable()
		{
			// this helper is probably broken. oh well
			/*if (!Modules.Helpers.isLocalUserHunk)
			{
				Destroy(this.gameObject);
				return;
			}*/

			InstanceTracker.Add<AmmoPickupInteraction>(this);
		}

		public void OnDisable()
		{
			InstanceTracker.Remove<AmmoPickupInteraction>(this);
		}

		public bool ShouldShowOnScanner()
		{
			return !this.opened;
		}

		private void UNetVersion()
		{
		}

		public bool Networkopened
		{
			get
			{
				return this.opened;
			}
			[param: In]
			set
			{
				base.SetSyncVar<bool>(value, ref this.opened, 1U);
			}
		}

		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.opened);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.opened);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.opened = reader.ReadBoolean();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.opened = reader.ReadBoolean();
			}
		}

		public override void PreStartClient()
		{
		}
	}

	public class AmmoOrb : Orb
	{
		private HunkController hunk;
		public override void Begin()
		{
			base.duration = UnityEngine.Random.Range(0.2f, 0.3f);

			EffectData effectData = new EffectData
			{
				origin = this.origin,
				genericFloat = base.duration
			};

			effectData.SetHurtBoxReference(this.target);

			EffectManager.SpawnEffect(Modules.Survivors.Hunk.ammoOrb, effectData, true);

			HurtBox hurtBox = this.target.GetComponent<HurtBox>();
			if (hurtBox)
			{
				this.hunk = hurtBox.healthComponent.GetComponent<HunkController>();
			}
		}

		public override void OnArrival()
		{
			if (this.hunk)
			{
				this.hunk.ServerGetAmmo();
			}
		}
	}
}