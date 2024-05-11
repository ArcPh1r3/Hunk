using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;

namespace HunkMod.SkillStates
{
    public class FuckMyAss : EntityStates.GenericCharacterDeath
    {
		public override void OnEnter()
		{
			base.OnEnter();

			/*Vector3 vector = Vector3.up * 3f;
			if (base.characterMotor)
			{
				vector += base.characterMotor.velocity;
				base.characterMotor.enabled = false;
			}

			if (base.cachedModelTransform)
			{
				RagdollController ragdollController = base.cachedModelTransform.GetComponent<RagdollController>();
				if (ragdollController)
				{
					// i hate that i have to do this

					foreach (Transform i in ragdollController.bones)
                    {
						if (i)
						{
							i.gameObject.layer = LayerIndex.ragdoll.intVal;
							i.gameObject.SetActive(true);
						}
                    }

					ragdollController.BeginRagdoll(vector);
				}
			}*/

			Destroy(this.modelLocator.modelTransform.gameObject);
		}

		public override void PlayDeathAnimation(float crossfadeDuration = 0.1f)
		{
		}

		public override bool shouldAutoDestroy
		{
			get
			{
				return false;
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && base.fixedAge > 4f)
			{
				EntityState.Destroy(base.gameObject);
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}
	}
}