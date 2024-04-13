﻿using R2API;
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
        public HunkWeaponDef weaponDef { get; set; }
        public abstract string weaponNameToken { get; }
        public abstract string weaponName { get; }
        public abstract string weaponDesc { get; }
        public abstract string iconName { get; }
        public abstract GameObject crosshairPrefab { get; }
        public abstract int magSize { get; }
        public abstract SkillDef primarySkillDef { get; }
        public abstract Mesh mesh { get; }
        public abstract Material material { get; }
        public abstract HunkWeaponDef.AnimationSet animationSet { get; }

        public abstract void Init();

        protected void CreateLang()
        {
            LanguageAPI.Add("ROB_HUNK_WEAPON_" + weaponNameToken + "_NAME", weaponName);
            LanguageAPI.Add("ROB_HUNK_WEAPON_" + weaponNameToken + "_DESC", weaponDesc);
        }

        protected void CreateWeapon()
        {
            Texture icon = null;
            if (iconName != "") icon = Modules.Assets.mainAssetBundle.LoadAsset<Texture>(iconName);

            weaponDef = HunkWeaponDef.CreateWeaponDefFromInfo(new HunkWeaponDefInfo
            {
                nameToken = "ROB_DRIVER_WEAPON_" + weaponNameToken + "_NAME",
                descriptionToken = "ROB_DRIVER_WEAPON_" + weaponNameToken + "_DESC",
                icon = icon,
                crosshairPrefab = crosshairPrefab,
                magSize = magSize,
                primarySkillDef = primarySkillDef,
                mesh = mesh,
                material = material,
                animationSet = animationSet
            });
            HunkWeaponCatalog.AddWeapon(weaponDef);
        }
    }
}