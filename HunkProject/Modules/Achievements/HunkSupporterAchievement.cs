using R2API;
using R2API.Utils;
using RoR2;
using System;
using UnityEngine;

namespace HunkMod.Modules.Achievements
{
    internal class HunkSupporterAchievement : ModdedUnlockable
    {
        public override string AchievementIdentifier { get; } = MainPlugin.developerPrefix + "_HUNK_SUPPORTER_UNLOCKABLE_ID";
        public override string UnlockableIdentifier { get; } = MainPlugin.developerPrefix + "_HUNK_SUPPORTER_UNLOCKABLE_REWARD_ID";
        public override string AchievementNameToken { get; } = MainPlugin.developerPrefix + "_HUNK_SUPPORTER_ACHIEVEMENT_NAME";
        public override string PrerequisiteUnlockableIdentifier { get; } = "";
        public override string UnlockableNameToken { get; } = MainPlugin.developerPrefix + "_HUNK_SUPPORTER_UNLOCKABLE_NAME";
        public override string AchievementDescToken { get; } = MainPlugin.developerPrefix + "_HUNK_SUPPORTER_ACHIEVEMENT_DESC";
        public override Sprite Sprite { get; } = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSuperSkin");

        public override Func<string> GetHowToUnlock { get; } = (() => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
                            {
                                Language.GetString(MainPlugin.developerPrefix + "_HUNK_SUPPORTER_ACHIEVEMENT_NAME"),
                                Language.GetString(MainPlugin.developerPrefix + "_HUNK_SUPPORTER_ACHIEVEMENT_DESC")
                            }));
        public override Func<string> GetUnlocked { get; } = (() => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
                            {
                                Language.GetString(MainPlugin.developerPrefix + "_HUNK_SUPPORTER_ACHIEVEMENT_NAME"),
                                Language.GetString(MainPlugin.developerPrefix + "_HUNK_SUPPORTER_ACHIEVEMENT_DESC")
                            }));

        public void Check(Modules.Components.HunkCSS hunk)
        {
            base.Grant();
        }

        public override void OnInstall()
        {
            base.OnInstall();

            Modules.Components.HunkCSS.onKonamiCode += Check;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            Modules.Components.HunkCSS.onKonamiCode -= Check;
        }
    }
}