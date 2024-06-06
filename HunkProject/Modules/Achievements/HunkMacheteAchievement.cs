﻿using HunkMod.Modules.Components;
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace HunkMod.Modules.Achievements
{
    internal class HunkMacheteAchievement : ModdedUnlockable
    {
        public override string AchievementIdentifier { get; } = MainPlugin.developerPrefix + "_HUNK_MACHETE_UNLOCKABLE_ID";
        public override string UnlockableIdentifier { get; } = MainPlugin.developerPrefix + "_HUNK_MACHETE_UNLOCKABLE_REWARD_ID";
        public override string AchievementNameToken { get; } = MainPlugin.developerPrefix + "_HUNK_MACHETE_ACHIEVEMENT_NAME";
        public override string PrerequisiteUnlockableIdentifier { get; } = "";
        public override string UnlockableNameToken { get; } = MainPlugin.developerPrefix + "_HUNK_MACHETE_UNLOCKABLE_NAME";
        public override string AchievementDescToken { get; } = MainPlugin.developerPrefix + "_HUNK_MACHETE_ACHIEVEMENT_DESC";
        public override Sprite Sprite { get; } = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texKnifeMachete");

        public override Func<string> GetHowToUnlock { get; } = (() => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
                            {
                                Language.GetString(MainPlugin.developerPrefix + "_HUNK_MACHETE_ACHIEVEMENT_NAME"),
                                Language.GetString(MainPlugin.developerPrefix + "_HUNK_MACHETE_ACHIEVEMENT_DESC")
                            }));
        public override Func<string> GetUnlocked { get; } = (() => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
                            {
                                Language.GetString(MainPlugin.developerPrefix + "_HUNK_MACHETE_ACHIEVEMENT_NAME"),
                                Language.GetString(MainPlugin.developerPrefix + "_HUNK_MACHETE_ACHIEVEMENT_DESC")
                            }));

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("RobHunkBody");
        }

        public override void OnInstall()
        {
            base.OnInstall();

            RoR2.SceneExitController.onBeginExit += SceneExitController_onBeginExit;
        }

        private void SceneExitController_onBeginExit(SceneExitController self)
        {
            foreach (var player in PlayerCharacterMasterController.instances)
            {
                if (player.networkUser && player.networkUser.isLocalPlayer && player.networkUser.bodyIndexPreference == BodyCatalog.FindBodyIndex(Modules.Survivors.Hunk.bodyName) && player.body)
                {
                    HunkController hunk = player.body.GetComponent<HunkController>();
                    if (hunk)
                    {
                        if (!hunk.weaponTracker.usedAmmoThisStage)
                        {
                            if (base.meetsBodyRequirement)
                            {
                                base.Grant();
                            }
                        }
                    }
                }
            }
        }

        public override void OnUninstall()
        {
            base.OnUninstall();
            RoR2.SceneExitController.onBeginExit -= SceneExitController_onBeginExit;
        }
    }
}