using UnityEngine;
using RoR2;
using System;

namespace HunkMod.Modules.Components
{
    public class HunkCSS : MonoBehaviour
    {
        public static Action<HunkCSS> onKonamiCode;

        private float stopwatch;
        private int step;
        private ChildLocator childLocator;

        private void Awake()
        {
            this.childLocator = this.GetComponent<ChildLocator>();
        }

        private void Update()
        {
            this.stopwatch -= Time.deltaTime;

            if (this.step > 0 && this.stopwatch <= 0f) this.step = 0;

            switch (this.step)
            {
                case 0:
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        this.stopwatch = 1f;
                        this.step++;
                    }
                    break;
                case 1:
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        this.stopwatch = 1f;
                        this.step++;
                    }
                    break;
                case 2:
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        this.stopwatch = 1f;
                        this.step++;
                    }
                    break;
                case 3:
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        this.stopwatch = 1f;
                        this.step++;
                    }
                    break;
                case 4:
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        this.stopwatch = 1f;
                        this.step++;
                    }
                    break;
                case 5:
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        this.stopwatch = 1f;
                        this.step++;
                    }
                    break;
                case 6:
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        this.stopwatch = 1f;
                        this.step++;
                    }
                    break;
                case 7:
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        this.stopwatch = 1f;
                        this.step++;

                        onKonamiCode(this);
                    }
                    break;
            }
        }

        public void ThrowGun()
        {
            Util.PlaySound("sfx_driver_gun_throw", this.gameObject);
        }

        public void CatchGun()
        {
            Util.PlaySound("sfx_driver_gun_catch", this.gameObject);
        }

        public void FailCatchGun()
        {

        }

        public void GunDrop()
        {
            Util.PlaySound("sfx_driver_gun_drop", this.gameObject);
        }
    }
}