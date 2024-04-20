using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.Modules.Weapons
{
    public class M19 : BaseWeapon<M19>
    {
        public override string weaponNameToken => "M19";
        public override string weaponName => "M19";
        public override string weaponDesc => "7-shot, large caliber, single-action handgun that fires .45 ACP rounds. Its base design hasn't changed in over 70 years so variants abound.";
        public override string iconName => "texM19Icon";
        public override GameObject crosshairPrefab => Modules.Assets.pistolCrosshairPrefab;
        public override int magSize => 7;
        public override GameObject modelPrefab => Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlM19");
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.Pistol;
        public override bool storedOnBack => false;

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Weapon.M19.Shoot)),
"Weapon",
"ROB_DRIVER_BODY_PRIMARY_BFG_NAME",
"ROB_DRIVER_BODY_PRIMARY_BFG_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShootIcon"),
false);
    }
}