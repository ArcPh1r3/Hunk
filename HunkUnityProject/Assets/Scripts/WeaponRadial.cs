using UnityEngine;
using UnityEngine.UI;

namespace HunkMod.Modules.Components
{
    public class WeaponRadial : MonoBehaviour
    {
        public int index;

        private GameObject cursor;
        private Transform center;
        private Image[] icons;
        private float iconSize = 0.22738f;

        private void Awake()
        {
            this.cursor = this.transform.Find("Center/Cursor").gameObject;
            this.center = this.transform.Find("Center");
            this.icons = this.transform.Find("Center/Icons").GetComponentsInChildren<Image>();
        }

        private void Update()
        {
            Vector2 delta = this.center.position - Input.mousePosition;
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            angle += 180f;

            int _index = 0;
            int j = 0;
            int storedAngle = 0;
            for (int i = 0; i < 360; i += 45)
            {
                if (angle >= i && angle < i + 45)
                {
                    storedAngle = i;
                    _index = j;
                }
                j++;
            }

            switch (_index)
            {
                case 0:
                    this.index = 5;
                    break;
                case 1:
                    this.index = 0;
                    break;
                case 2:
                    this.index = 4;
                    break;
                case 3:
                    this.index = 2;
                    break;
                case 4:
                    this.index = 6;
                    break;
                case 5:
                    this.index = 1;
                    break;
                case 6:
                    this.index = 7;
                    break;
                case 7:
                    this.index = 3;
                    break;
            }

            for (int i = 0; i < this.icons.Length; i++)
            {
                if (i == this.index) this.icons[i].gameObject.transform.localScale = Vector3.one * iconSize * 1.2f;
                else this.icons[i].gameObject.transform.localScale = Vector3.one * iconSize;
            }

            this.cursor.transform.eulerAngles = new Vector3(0f, 0f, storedAngle);
            this.cursor.SetActive(true);
        }
    }
}