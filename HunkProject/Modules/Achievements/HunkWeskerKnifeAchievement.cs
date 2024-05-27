using R2API;
using RoR2;
using System;
using UnityEngine;

namespace HunkMod.Modules.Achievements
{
    internal class HunkWeskerKnifeAchievement : ModdedUnlockable
    {
        public override string AchievementIdentifier { get; } = MainPlugin.developerPrefix + "_HUNK_WESKERKNIFE_UNLOCKABLE_ID";
        public override string UnlockableIdentifier { get; } = MainPlugin.developerPrefix + "_HUNK_WESKERKNIFE_UNLOCKABLE_REWARD_ID";
        public override string AchievementNameToken { get; } = MainPlugin.developerPrefix + "_HUNK_WESKERKNIFE_ACHIEVEMENT_NAME";
        public override string PrerequisiteUnlockableIdentifier { get; } = "";
        public override string UnlockableNameToken { get; } = MainPlugin.developerPrefix + "_HUNK_WESKERKNIFE_UNLOCKABLE_NAME";
        public override string AchievementDescToken { get; } = MainPlugin.developerPrefix + "_HUNK_WESKERKNIFE_ACHIEVEMENT_DESC";
        public override Sprite Sprite { get; } = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKnifeWesker");

        public override Func<string> GetHowToUnlock { get; } = (() => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
                            {
                                Language.GetString(MainPlugin.developerPrefix + "_HUNK_WESKERKNIFE_ACHIEVEMENT_NAME"),
                                Language.GetString(MainPlugin.developerPrefix + "_HUNK_WESKERKNIFE_ACHIEVEMENT_DESC")
                            }));
        public override Func<string> GetUnlocked { get; } = (() => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
                            {
                                Language.GetString(MainPlugin.developerPrefix + "_HUNK_WESKERKNIFE_ACHIEVEMENT_NAME"),
                                Language.GetString(MainPlugin.developerPrefix + "_HUNK_WESKERKNIFE_ACHIEVEMENT_DESC")
                            }));

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("RobHunkBody");
        }

        private void Check(Inventory inventory)
        {
            if (inventory && base.meetsBodyRequirement)
            {
                bool hasAttachment = false;

                // todo - turn this into a static list and iterate through it
                if (inventory.GetItemCount(Modules.Weapons.SMG.laserSight) > 0) hasAttachment = true;
                if (inventory.GetItemCount(Modules.Weapons.SMG.extendedMag) > 0) hasAttachment = true;
                if (inventory.GetItemCount(Modules.Weapons.MUP.gunStock) > 0) hasAttachment = true;
                if (inventory.GetItemCount(Modules.Weapons.Magnum.longBarrel) > 0) hasAttachment = true;
                if (inventory.GetItemCount(Modules.Weapons.Revolver.speedloader) > 0) hasAttachment = true;

                CharacterMaster master = inventory.GetComponent<CharacterMaster>();
                if (master)
                {
                    if (master.GetBodyObject())
                    {
                        Modules.Components.HunkController hunk = master.GetBodyObject().GetComponent<Modules.Components.HunkController>();
                        if (hunk)
                        {
                            if (hunk.passive.isFullArsenal) hasAttachment = false;
                        }
                    }
                }

                if (hasAttachment) base.Grant();
            }
        }

        public override void OnInstall()
        {
            base.OnInstall();

            Modules.Components.HunkController.onInventoryUpdate += Check;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            Modules.Components.HunkController.onInventoryUpdate -= Check;
        }
    }
}