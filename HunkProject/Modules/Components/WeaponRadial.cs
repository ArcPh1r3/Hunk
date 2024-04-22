using UnityEngine;
using UnityEngine.UI;

namespace HunkMod.Modules.Components
{
    public class WeaponRadial : MonoBehaviour
    {
        public int index;

        private HunkWeaponTracker hunk;
        private float offset;
        private GameObject cursor;
        private Image[] icons;

        private void Awake()
        {
            this.cursor = this.transform.Find("Cursor").gameObject;
            this.icons = this.transform.Find("Icons").GetComponentsInChildren<Image>();
        }

        public void Init(HunkWeaponTracker tracker)
        {
            this.hunk = tracker;
        }

        private void UpdateIcons()
        {
            for (int i = 0; i < this.icons.Length; i++)
            {

            }
        }

        private void Update()
        {
            this.UpdateIcons();

            Vector2 delta = this.transform.position - Input.mousePosition;
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            angle += 180f;

            int _index = 0;
            int j = 0;
            int storedAngle = 0;
            for (int i = 0; i < 360; i += 45)
            {
                if (angle >= i + this.offset && angle < i + 45 + this.offset)
                {
                    storedAngle = i;
                    _index = j;
                }
                j++;
            }

            // remap it so it's organized a little more nicely
            switch (_index)
            {
                case 0:
                    this.index = 0;
                    break;
                case 1:
                    this.index = 5;
                    break;
                case 2:
                    this.index = 3;
                    break;
                case 3:
                    this.index = 7;
                    break;
                case 4:
                    this.index = 1;
                    break;
                case 5:
                    this.index = 6;
                    break;
                case 6:
                    this.index = 2;
                    break;
                case 7:
                    this.index = 4;
                    break;
            }

            for (int i = 0; i < this.icons.Length; i++)
            {
                if (i == this.index) this.icons[i].gameObject.transform.localScale = Vector3.one;
                else this.icons[i].gameObject.transform.localScale = Vector3.one * 1.1f;
            }

            this.cursor.transform.eulerAngles = new Vector3(0f, 0f, storedAngle + this.offset);
            this.cursor.SetActive(true);
        }
    }
}