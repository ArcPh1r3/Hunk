using RoR2;
using RoR2.UI;
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
        public int controllerLatchIndex = 99;

        private GameObject cursor;
        private RectTransform center;
        private Image[] icons;
        private float iconSize = 0.22738f;
        private LocalUser localUser;
        private int controllerLatchTimer;

        private GameObject statsPanel;
        private TextMeshProUGUI label;
        private TextMeshProUGUI ammoCount;

        private TextMeshProUGUI[] statText;
        private Image[] statFill;

        internal RoR2.UI.MPEventSystem events;
        internal RoR2.UI.MPInputModule inputModule;

        private float scaleSpeed = 14f;

        private void Awake()
        {
            this.cursor = this.transform.Find("Center/Cursor").gameObject;
            this.center = this.transform.Find("Center").GetComponent<RectTransform>();
            this.icons = this.transform.Find("Center/Icons").GetComponentsInChildren<Image>();

            this.label = this.transform.Find("Center/Inner/StatsPanel/Title").GetComponent<TextMeshProUGUI>();
            this.ammoCount = this.transform.Find("Center/Inner/StatsPanel/Ammo").GetComponent<TextMeshProUGUI>();
            this.statsPanel = this.transform.Find("Center/Inner/StatsPanel").gameObject;

            //inputModule.input should be the same as input but it does different things because Hopoo Games!! idk
            var input = GameObject.Find("MPEventSystem Player0").GetComponent<RoR2.UI.MPInput>();
            this.events = input.GetFieldValue<RoR2.UI.MPEventSystem>("eventSystem");
            this.inputModule = events.currentInputModule as MPInputModule;

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

            TMP_FontAsset hgFont = Modules.Assets.hgFont;

            this.label.font = hgFont;
            this.ammoCount.font = hgFont;
            foreach (TextMeshProUGUI i in this.statText)
            {
                i.font = hgFont;
            }
        }

        private void Start()
        {
            Util.PlaySound("sfx_hunk_menu_open", this.gameObject);

            this.events.cursorOpenerForGamepadCount += 1;
            this.events.cursorOpenerCount += 1;

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
        }

        private void LateUpdate()
        {
            var vCursor = this.events.cursorIndicatorController;
            if(this.controllerInput)
            {
                vCursor.SetCursor(vCursor.noneCursorSet,CursorIndicatorController.CursorImage.None,events.GetColor());
            }
        }

        private void OnDestroy()
        {
            if (this.events)
            {
                Util.PlaySound("sfx_hunk_menu_click", this.gameObject);

                this.events.cursorOpenerForGamepadCount -= 1;
                this.events.cursorOpenerCount -= 1;
            }
        }

        public bool isValidIndex
        {
            get
            {
                return this.ValidIndex(this.index, false);
            }
        }

        public bool ValidIndex(int i, bool countEquippedAsValid = true)
        {
            if (i >= this.hunk.weaponData.Length) return false;
            if (i == this.hunk.equippedIndex && !countEquippedAsValid) return false;
            return true;
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
                Vector3 mousePosition =  new Vector3(this.inputModule.input.mousePosition.x - (Screen.width / 2.0f), this.inputModule.input.mousePosition.y - (Screen.height / 2.0f), 0f);
                if(this.controllerInput)
                {
                    var player = (this.inputModule.input as MPInput).player;
                    mousePosition = new Vector3(player.GetAxis(23),player.GetAxis(24),0);
                }
                float dist = Vector3.Distance(mousePosition, this.center.localPosition);
                return dist <= (controllerInput? 0.2f : 172f);
            }
        }

        public bool controllerInput
        {
            get
            {
                return events?.currentInputSource == MPEventSystem.InputSource.Gamepad;
            }
        }

        private void UpdateScale()
        {
            this.transform.localScale = Vector3.Slerp(this.transform.localScale, Vector3.one * 2f, Time.unscaledDeltaTime * this.scaleSpeed);
        }

        private void Update()
        {
            this.UpdateScale();
            this.UpdateIcons();

            if (this.localUser != null) this.localUser.eventSystem.m_CurrentSelected = this.gameObject;

            Vector3 mousePosition =  new Vector3(inputModule.input.mousePosition.x - (Screen.width / 2.0f), inputModule.input.mousePosition.y - (Screen.height / 2.0f), 0f);
            if(this.controllerInput)
            {
                var player = (inputModule.input as MPInput).player;
                mousePosition = new Vector3(player.GetAxis(23),player.GetAxis(24),0);
            }
            if(this.controllerLatchTimer <= 0)
            {
                this.controllerLatchIndex = 99;
                this.controllerLatchTimer = 0;
            }
            else{
                this.controllerLatchTimer--;
            }

            if (this.cursorInCenter)
            {
                this.statsPanel.SetActive(false);
                this.cursor.SetActive(false);
                this.index = -1;

                for (int i = 0; i < this.icons.Length; i++)
                {
                    this.icons[i].gameObject.transform.localScale = Vector3.one * this.iconSize;
                }

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

            if (_index != this.index && this.ValidIndex(_index))
            {
                Util.PlaySound("sfx_hunk_menu_cursor", this.gameObject);
                if(this.controllerInput)
                {
                    this.controllerLatchIndex = _index;
                    this.controllerLatchTimer = 15;
                }
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

                if (weaponDef.magSize >= 999)
                {
                    this.ammoCount.text = "<color=#" + Helpers.yellowItemHex + ">Infinite" + Helpers.colorSuffix;
                }
                else
                {
                    this.ammoCount.text = this.hunk.weaponData[this.index].currentAmmo + " / " + this.hunk.weaponData[this.index].totalAmmo;
                }

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