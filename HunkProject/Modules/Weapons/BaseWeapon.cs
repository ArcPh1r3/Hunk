using R2API;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
        public abstract float magPickupMultiplier { get; }
        public abstract float reloadDuration { get; }
        public abstract string ammoName { get; }
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
                magPickupMultiplier = magPickupMultiplier,
                reloadDuration = reloadDuration,
                ammoName = ammoName,
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

            // this really should have worked man i fucking hate the solution
            //itemDef = (ItemDef)ScriptableObject.CreateInstance(typeof(ItemDef));
            itemDef = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC1/CloverVoid/CloverVoid.asset").WaitForCompletion());
            itemDef.name = "ROB_HUNK_WEAPON_" + weaponNameToken + "_NAME";
            itemDef.nameToken = "ROB_HUNK_WEAPON_" + weaponNameToken + "_NAME";
            itemDef.descriptionToken = "ROB_HUNK_WEAPON_" + weaponNameToken + "_DESC";
            itemDef.pickupToken = "ROB_HUNK_WEAPON_" + weaponNameToken + "_DESC";
            itemDef.loreToken = "ROB_HUNK_WEAPON_" + weaponNameToken + "_DESC";
            itemDef.canRemove = false;
            itemDef.hidden = false;
            itemDef.pickupIconSprite = weaponDef.icon;
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
            itemDef.unlockableDef = null;

            if (modelPrefab)
            {
                itemDef.pickupModelPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(weaponDef.modelPrefab.name + "Pickup");
                Modules.Assets.ConvertAllRenderersToHopooShader(itemDef.pickupModelPrefab);
            }

            // tell me why i can't set this tell me why this breaks the entire weapon when i do it this way
            //itemDef.tier = ItemTier.NoTier;

            weaponDef.itemDef = itemDef;
            HunkWeaponCatalog.AddWeapon(weaponDef);

            if (modelPrefab) Modules.Assets.ConvertAllRenderersToHopooShader(modelPrefab);
        }
    }
}