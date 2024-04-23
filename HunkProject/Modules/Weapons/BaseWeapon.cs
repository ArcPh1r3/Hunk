using R2API;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;

namespace HunkMod.Modules.Weapons
{
    public abstract class BaseWeapon<T> : BaseWeapon where T : BaseWeapon<T>
    {
        public static T instance { get; private set; }

        public BaseWeapon()
        {
            if (instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting BaseWeapon was instantiated twice");
            instance = this as T;
        }
    }

    public abstract class BaseWeapon
    {
        public HunkWeaponDef weaponDef { get; private set; }
        public ItemDef itemDef { get; private set; }
        public abstract string weaponNameToken { get; }
        public abstract string weaponName { get; }
        public abstract string weaponDesc { get; }
        public abstract string iconName { get; }
        public abstract GameObject crosshairPrefab { get; }
        public abstract int magSize { get; }
        public abstract float reloadDuration { get; }
        public abstract SkillDef primarySkillDef { get; }
        public abstract GameObject modelPrefab { get; }
        public abstract HunkWeaponDef.AnimationSet animationSet { get; }
        public abstract bool storedOnBack { get; }
        public abstract float damageFillValue { get; }
        public abstract float rangefillValue { get; }
        public abstract float fireRateFillValue { get; }
        public abstract float reloadFillValue { get; }
        public abstract float accuracyFillValue { get; }

        public virtual void Init()
        {
            CreateLang();
            CreateWeapon();
        }

        protected void CreateLang()
        {
            LanguageAPI.Add("ROB_HUNK_WEAPON_" + weaponNameToken + "_NAME", weaponName);
            LanguageAPI.Add("ROB_HUNK_WEAPON_" + weaponNameToken + "_DESC", weaponDesc);
        }

        protected void CreateWeapon()
        {
            Sprite icon = null;
            if (iconName != "") icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>(iconName);

            weaponDef = HunkWeaponDef.CreateWeaponDefFromInfo(new HunkWeaponDefInfo
            {
                nameToken = "ROB_HUNK_WEAPON_" + weaponNameToken + "_NAME",
                descriptionToken = "ROB_HUNK_WEAPON_" + weaponNameToken + "_DESC",
                icon = icon,
                crosshairPrefab = crosshairPrefab,
                magSize = magSize,
                reloadDuration = reloadDuration,
                primarySkillDef = primarySkillDef,
                modelPrefab = modelPrefab,
                animationSet = animationSet,
                storedOnBack = storedOnBack,
                damageFillValue = damageFillValue,
                rangefillValue = rangefillValue,
                fireRateFillValue = fireRateFillValue,
                reloadFillValue = reloadFillValue,
                accuracyFillValue = accuracyFillValue,
            });

            itemDef = (ItemDef)ScriptableObject.CreateInstance(typeof(ItemDef));
            itemDef.name = "ROB_HUNK_WEAPON_" + weaponNameToken + "_NAME";
            itemDef.nameToken = "ROB_HUNK_WEAPON_" + weaponNameToken + "_NAME";
            itemDef.descriptionToken = "ROB_HUNK_WEAPON_" + weaponNameToken + "_DESC";
            itemDef.canRemove = false;
            itemDef.hidden = false;
            itemDef.loreToken = "";
            itemDef.pickupIconSprite = weaponDef.icon;
            itemDef.pickupToken = "";
            itemDef.requiredExpansion = null;
            itemDef.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotDuplicate,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };

            weaponDef.itemDef = itemDef;
            HunkWeaponCatalog.AddWeapon(weaponDef);

            if (modelPrefab) Modules.Assets.ConvertAllRenderersToHopooShader(modelPrefab);
        }
    }
}