using RoR2;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using R2API.Utils;

namespace HunkMod.Modules.Components
{
    public class WeaponRadial : MonoBehaviour
    {
        public int index;
        public HunkWeaponTracker hunk;
        public CharacterBody body;

        private GameObject cursor;
        private RectTransform center;
        private Image[] icons;
        private float iconSize = 0.22738f;
        private LocalUser localUser;

        private GameObject statsPanel;
        private TextMeshProUGUI label;
        private TextMeshProUGUI ammoCount;

        private TextMeshProUGUI[] statText;
        private Image[] statFill;

        internal RoR2.UI.MPInput input = GameObject.Find("MPEventSystem Player0").GetComponent<RoR2.UI.MPInput>();
        internal RoR2.UI.MPEventSystem events;

        private float scaleSpeed = 8f;

        private void Awake()
        {
            this.cursor = this.transform.Find("Center/Cursor").gameObject;
            this.center = this.transform.Find("Center").GetComponent<RectTransform>();
            this.icons = this.transform.Find("Center/Icons").GetComponentsInChildren<Image>();

            this.label = this.transform.Find("Center/Inner/StatsPanel/Title").GetComponent<TextMeshProUGUI>();
            this.ammoCount = this.transform.Find("Center/Inner/StatsPanel/Ammo").GetComponent<TextMeshProUGUI>();
            this.statsPanel = this.transform.Find("Center/Inner/StatsPanel").gameObject;

            this.events = input.GetFieldValue<RoR2.UI.MPEventSystem>("eventSystem");
            this.events.cursorOpenerForGamepadCount += 1;
            this.events.cursorOpenerCount += 1;

            this.statText = new TextMeshProUGUI[]
            {
                this.transform.Find("Center/Inner/StatsPanel/StatBar/Label").GetComponent<TextMeshProUGUI>(),
                this.transform.Find("Center/Inner/StatsPanel/StatBar (1)/Label").GetComponent<TextMeshProUGUI>(),
                this.transform.Find("Center/Inner/StatsPanel/StatBar (2)/Label").GetComponent<TextMeshProUGUI>(),
                this.transform.Find("Center/Inner/StatsPanel/StatBar (3)/Label").GetComponent<TextMeshProUGUI>(),
                this.transform.Find("Center/Inner/StatsPanel/StatBar (4)/Label").GetComponent<TextMeshProUGUI>()
            };

            this.statFill = new Image[]
            {
                this.transform.Find("Center/Inner/StatsPanel/StatBar/FillBar").GetComponent<Image>(),
                this.transform.Find("Center/Inner/StatsPanel/StatBar (1)/FillBar").GetComponent<Image>(),
                this.transform.Find("Center/Inner/StatsPanel/StatBar (2)/FillBar").GetComponent<Image>(),
                this.transform.Find("Center/Inner/StatsPanel/StatBar (3)/FillBar").GetComponent<Image>(),
                this.transform.Find("Center/Inner/StatsPanel/StatBar (4)/FillBar").GetComponent<Image>()
            };

            if (this.localUser == null)
            {
                if (this.body)
                {
                    foreach (LocalUser lu in LocalUserManager.readOnlyLocalUsersList)
                    {
                        if (lu.cachedBody == this.body)
                        {
                            this.localUser = lu;
                            break;
                        }
                    }
                }
            }

            TMP_FontAsset hgFont = Modules.Assets.hgFont;

            this.label.font = hgFont;
            this.ammoCount.font = hgFont;
            foreach (TextMeshProUGUI i in this.statText)
            {
                i.font = hgFont;
            }
        }

        private void OnDestroy()
        {
            this.events.cursorOpenerForGamepadCount -= 1;
            this.events.cursorOpenerCount -= 1;
        }

        public bool isValidIndex
        {
            get
            {
                if (this.index >= this.hunk.weaponData.Length) return false;
                if (this.index == this.hunk.equippedIndex) return false;
                return true;
            }
        }

        public bool isRealIndex
        {
            get
            {
                if (this.index >= this.hunk.weaponData.Length) return false;
                return true;
            }
        }

        private void UpdateIcons()
        {
            for (int i = 0; i < this.icons.Length; i++)
            {
                this.icons[i].enabled = false;
            }

            for (int i = 0; i < this.hunk.weaponData.Length; i++)
            {
                this.icons[this.IconIndex(i)].sprite = this.hunk.weaponData[i].weaponDef.icon;
                this.icons[this.IconIndex(i)].enabled = true;
            }
        }

        private int IconIndex(int fuckMe)
        {
            // feed an index here to get the actual corresponding icon slot..
            switch (fuckMe)
            {
                case 0:
                    return 5;
                case 1:
                    return 0;
                case 2:
                    return 4;
                case 3:
                    return 2;
                case 4:
                    return 6;
                case 5:
                    return 1;
                case 6:
                    return 7;
                case 7:
                    return 3;
            }

            return 0;
        }

        public bool cursorInCenter
        {
            get
            {
                Vector3 mousePosition = new Vector3(Input.mousePosition.x - (Screen.width / 2.0f), Input.mousePosition.y - (Screen.height / 2.0f), 0f);
                float dist = Vector3.Distance(mousePosition, this.center.localPosition);
                return dist <= 172f;
            }
        }

        private void UpdateScale()
        {
            this.transform.localScale = Vector3.Slerp(this.transform.localScale, Vector3.one * 2f, Time.deltaTime * this.scaleSpeed);
        }

        private void Update()
        {
            this.UpdateScale();
            this.UpdateIcons();

            if (this.localUser != null) this.localUser.eventSystem.m_CurrentSelected = this.gameObject;

            Vector3 mousePosition = new Vector3(Input.mousePosition.x - (Screen.width / 2.0f), Input.mousePosition.y - (Screen.height / 2.0f), 0f);

            if (this.cursorInCenter)
            {
                this.statsPanel.SetActive(false);
                this.cursor.SetActive(false);
                this.index = -1;
                return;
            }

            Vector2 delta = this.center.localPosition - mousePosition;
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

            this.index = _index;

            for (int i = 0; i < this.icons.Length; i++)
            {
                if (i == this.IconIndex(this.index)) this.icons[i].gameObject.transform.localScale = Vector3.one * this.iconSize * 1.2f;
                else this.icons[i].gameObject.transform.localScale = Vector3.one * this.iconSize;
            }

            this.cursor.transform.eulerAngles = new Vector3(0f, 0f, storedAngle);
            this.cursor.SetActive(this.isRealIndex);

            if (this.isRealIndex)
            {
                HunkWeaponDef weaponDef = this.hunk.weaponData[this.index].weaponDef;
                this.label.text = Language.GetString(weaponDef.nameToken);
                this.ammoCount.text = this.hunk.weaponData[this.index].currentAmmo + " / " + this.hunk.weaponData[this.index].totalAmmo;

                this.statFill[0].fillAmount = weaponDef.damageFillValue;
                this.statFill[1].fillAmount = weaponDef.rangefillValue;
                this.statFill[2].fillAmount = weaponDef.fireRateFillValue;
                this.statFill[3].fillAmount = weaponDef.reloadFillValue;
                this.statFill[4].fillAmount = weaponDef.accuracyFillValue;

                this.statsPanel.SetActive(true);
            }
            else this.statsPanel.SetActive(false);
        }
    }
}