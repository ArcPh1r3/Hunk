using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.Modules.Weapons
{
    public class SMG : BaseWeapon<SMG>
    {
        public override string weaponNameToken => "SMG";
        public override string weaponName => "Submachine Gun";
        public override string weaponDesc => "Close-range gun with high damage and equally high spread.";
        public override string iconName => "texSMGWeaponIcon";
        public override GameObject crosshairPrefab => Modules.Assets.smgCrosshairPrefab;
        public override int magSize => 32;
        public override Mesh mesh => Modules.Assets.LoadMesh("meshSMG");
        public override Material material => Addressables.LoadAssetAsync<Material>("RoR2/Base/Commando/matCommandoDualies.mat").WaitForCompletion();
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.SMG;

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Weapon.SMG.Shoot)),
"Weapon",
"ROB_DRIVER_BODY_PRIMARY_BFG_NAME",
"ROB_DRIVER_BODY_PRIMARY_BFG_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherIcon"),
false);

        public override void Init()
        {
            CreateLang();
            CreateWeapon();
        }
    }
}