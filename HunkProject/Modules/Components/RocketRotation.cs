using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace HunkMod.Modules.Components
{
    public class RocketRotation : MonoBehaviour
    {
        private Rigidbody rb;

        private void Awake()
        {
            this.rb = this.GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            this.transform.rotation = Util.QuaternionSafeLookRotation(this.rb.velocity.normalized);
        }
    }
}