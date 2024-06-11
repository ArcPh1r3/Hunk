using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HunkMod.Modules.Components
{
	public class HunkGravitatePickup : MonoBehaviour
	{
		[Tooltip("The rigidbody to set the velocity of.")]
		public Rigidbody rigidbody;
		public float acceleration;
		public float maxSpeed;

		private Transform gravitateTarget;

		private void OnTriggerEnter(Collider other)
		{
			if (NetworkServer.active && !this.gravitateTarget)
			{
				CharacterBody characterBody = other.gameObject.GetComponent<CharacterBody>();
				if (characterBody && characterBody.GetComponent<HunkController>())
				{
					this.gravitateTarget = other.gameObject.transform;
				}
			}
		}

		private void FixedUpdate()
		{
			if (this.gravitateTarget)
			{
				this.rigidbody.velocity = Vector3.MoveTowards(this.rigidbody.velocity, (this.gravitateTarget.transform.position - base.transform.position).normalized * this.maxSpeed, this.acceleration);
			}
		}
	}
}