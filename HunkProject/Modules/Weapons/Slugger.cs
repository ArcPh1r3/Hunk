﻿using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.Modules.Weapons
{
    public class Slugger : BaseWeapon<Slugger>
    {
        public override string weaponNameToken => "SLUGGER";
        public override string weaponName => "Slugger";
        public override string weaponDesc => "2-round capacity 12-gauge pump-action shotgun. Its sturdy steel action makes this popular model reliable and easy to control.";
        public override string iconName => "texShotgunIcon";
        public override GameObject crosshairPrefab => Modules.Assets.shotgunCrosshairPrefab;
        public override int magSize => 2;
        public override GameObject modelPrefab => Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlShotgun");
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.SMG;

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Weapon.Slugger.Shoot)),
"Weapon",
"ROB_DRIVER_BODY_PRIMARY_BFG_NAME",
"ROB_DRIVER_BODY_PRIMARY_BFG_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShootIcon"),
false);

        public override void Init()
        {
            CreateLang();
            CreateWeapon();
        }
    }
}