using System.Runtime.InteropServices;
using RoR2;
using RoR2.Networking;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace HunkMod.Modules.Components
{
	public sealed class HunkC4Interaction : NetworkBehaviour, IInteractable, IDisplayNameProvider
	{
		public string displayNameToken = "ROB_HUNK_C4_NAME";
		public string contextToken = "ROB_HUNK_C4_CONTEXT";

		[SyncVar]
		private bool activated;

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
			if (this.activated)
			{
				return Interactability.Disabled;
			}

			if (activator)
			{
				CharacterBody cb = activator.GetComponent<CharacterBody>();
				if (cb && !cb.GetComponent<HunkController>()) return Interactability.Disabled;
				else
				{
					if (cb)
					{
						HunkPassive passive = cb.GetComponent<HunkPassive>();
						if (passive && passive.isFullArsenal) return Interactability.Disabled;
					}
				}
			}

			return Interactability.Available;
		}

		[Server]
		public void OnInteractionBegin(Interactor activator)
		{
			if (!NetworkServer.active) return;

			if (!this.activated)
			{
				this.Networkopened = true;

				EntityStateMachine esm = base.GetComponent<EntityStateMachine>();
				if (esm) esm.SetNextState(new SkillStates.C4.Charge());
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

			InstanceTracker.Add<HunkC4Interaction>(this);
		}

		public void OnDisable()
		{
			InstanceTracker.Remove<HunkC4Interaction>(this);
		}

		public bool ShouldShowOnScanner()
		{
			return !this.activated;
		}

		private void UNetVersion()
		{
		}

		public bool Networkopened
		{
			get
			{
				return this.activated;
			}
			[param: In]
			set
			{
				base.SetSyncVar<bool>(value, ref this.activated, 1U);
			}
		}

		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.activated);
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
				writer.Write(this.activated);
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
				this.activated = reader.ReadBoolean();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.activated = reader.ReadBoolean();
			}
		}

		public override void PreStartClient()
		{
		}
	}
}