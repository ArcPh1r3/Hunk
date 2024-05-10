using R2API;
using R2API.Utils;
using RoR2;
using System;
using UnityEngine;

namespace HunkMod.Modules.Achievements
{
    internal class HunkLightweightAchievement : ModdedUnlockable
    {
        public override string AchievementIdentifier { get; } = MainPlugin.developerPrefix + "_HUNK_LIGHTWEIGHT_UNLOCKABLE_ID";
        public override string UnlockableIdentifier { get; } = MainPlugin.developerPrefix + "_HUNK_LIGHTWEIGHT_UNLOCKABLE_REWARD_ID";
        public override string AchievementNameToken { get; } = MainPlugin.developerPrefix + "_HUNK_LIGHTWEIGHT_ACHIEVEMENT_NAME";
        public override string PrerequisiteUnlockableIdentifier { get; } = "";
        public override string UnlockableNameToken { get; } = MainPlugin.developerPrefix + "_HUNK_LIGHTWEIGHT_UNLOCKABLE_NAME";
        public override string AchievementDescToken { get; } = MainPlugin.developerPrefix + "_HUNK_LIGHTWEIGHT_ACHIEVEMENT_DESC";
        public override Sprite Sprite { get; } = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texLightweightSkin");

        public override Func<string> GetHowToUnlock { get; } = (() => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
                            {
                                Language.GetString(MainPlugin.developerPrefix + "_HUNK_LIGHTWEIGHT_ACHIEVEMENT_NAME"),
                                Language.GetString(MainPlugin.developerPrefix + "_HUNK_LIGHTWEIGHT_ACHIEVEMENT_DESC")
                            }));
        public override Func<string> GetUnlocked { get; } = (() => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
                            {
                                Language.GetString(MainPlugin.developerPrefix + "_HUNK_LIGHTWEIGHT_ACHIEVEMENT_NAME"),
                                Language.GetString(MainPlugin.developerPrefix + "_HUNK_LIGHTWEIGHT_ACHIEVEMENT_DESC")
                            }));

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("RobHunkBody");
        }

        private void Check(CharacterBody characterBody)
        {
            if (Run.instance is null) return;

            if (Run.instance.stageClearCount >= 1)
            {
                if (characterBody.inventory && base.meetsBodyRequirement && characterBody.baseNameToken == Modules.Survivors.Hunk.bodyNameToken)
                {
                    bool fuck = false;

                    if (characterBody.inventory.GetTotalItemCountOfTier(ItemTier.Tier1) > 0) fuck = true;
                    if (characterBody.inventory.GetTotalItemCountOfTier(ItemTier.Tier2) > 0) fuck = true;
                    if (characterBody.inventory.GetTotalItemCountOfTier(ItemTier.Tier3) > 0) fuck = true;
                    if (characterBody.inventory.GetTotalItemCountOfTier(ItemTier.Boss) > 0) fuck = true;
                    if (characterBody.inventory.GetTotalItemCountOfTier(ItemTier.Lunar) > 0) fuck = true;
                    if (characterBody.inventory.GetTotalItemCountOfTier(ItemTier.VoidTier1) > 0) fuck = true;
                    if (characterBody.inventory.GetTotalItemCountOfTier(ItemTier.VoidTier2) > 0) fuck = true;
                    if (characterBody.inventory.GetTotalItemCountOfTier(ItemTier.VoidBoss) > 0) fuck = true;

                    if (!fuck) base.Grant();
                }
            }
        }

        public override void OnInstall()
        {
            base.OnInstall();

            CharacterBody.onBodyStartGlobal += Check;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            CharacterBody.onBodyStartGlobal -= Check;
        }
    }
}