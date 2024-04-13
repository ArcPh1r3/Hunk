using UnityEngine;

namespace RobDriver.Modules.Components
{
    public class DriverCSS : MonoBehaviour
    {
        private ChildLocator childLocator;

        private void Awake()
        {
            this.childLocator = this.GetComponent<ChildLocator>();
        }

        public void ThrowGun()
        {
            
        }

        public void CatchGun()
        {

        }

        public void FailCatchGun()
        {

        }

        public void GunDrop()
        {

        }
    }
}