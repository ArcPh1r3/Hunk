using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.Modules.Weapons
{
    public class Shotgun : BaseWeapon<Shotgun>
    {
        public override string weaponNameToken => "SHOTGUN";
        public override string weaponName => "W-870";
        public override string weaponDesc => "";
        public override string iconName => "texShotgunIcon";
        public override GameObject crosshairPrefab => Modules.Assets.shotgunCrosshairPrefab;
        public override int magSize => 4;
        public override GameObject modelPrefab => Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlShotgun");
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.SMG;

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Weapon.Shotgun.Shoot)),
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