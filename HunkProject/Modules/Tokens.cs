using R2API;
using System;

namespace HunkMod.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            string prefix = MainPlugin.developerPrefix + "_HUNK_BODY_";

            string desc = "HUNK is an elite infiltrator who carries a large arsenal of weapons claimed via OSP (on-site procurement).<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Use your Combat Knife against weaker foes to conserve ammo." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Find UES Keycards to unlock Weapon Cases to add to your armaments on each stage." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Use Quickstep carefully to Roll between enemy attacks while taking them down at the same time." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Scrounge around in opened chests for a chance to find ammo for your current weapons." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, fine.";
            string outroFailure = "..and so he vanished, human unit never killed.";

            string lore = "You're not any less of a man if you don't pull the trigger.\n";
            lore += "You're not necessarily a man if you do.\n";

            LanguageAPI.Add(prefix + "NAME", "HUNK");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "Grim Reaper");
            LanguageAPI.Add(prefix + "LORE", lore);
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Survivalist");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", $"HUNK is able to <style=cIsDamage>rummage</style> through <style=cIsUtility>chests and barrels</style> for spare ammo, <style=cIsHealth>only once per</style>.");

            LanguageAPI.Add(prefix + "PASSIVE2_NAME", "The Fourth Survivor");
            LanguageAPI.Add(prefix + "PASSIVE2_DESCRIPTION", $"Spawn with a <style=cIsDamage>full arsenal</style>,  <style=cIsHealth>BUT</style> you are now <style=cIsHealth>unable to gain any more ammo</style>.");

            LanguageAPI.Add(prefix + "CONFIRM_NAME", "Confirm");
            LanguageAPI.Add(prefix + "CONFIRM_DESCRIPTION", "Proceed with the current skill.");

            LanguageAPI.Add(prefix + "CANCEL_NAME", "Cancel");
            LanguageAPI.Add(prefix + "CANCEL_DESCRIPTION", "Cancel the current skill.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_KNIFE_NAME", "Combat Knife");
            LanguageAPI.Add(prefix + "PRIMARY_KNIFE_DESCRIPTION", $"<style=cIsUtility>Looting.</style> <style=cIsDamage>Slash</style> close-range combatants for <style=cIsDamage>{100f * 3.5}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_KNIFEALT_NAME", "Hidden Blade");
            LanguageAPI.Add(prefix + "PRIMARY_KNIFEALT_DESCRIPTION", $"<style=cIsUtility>Looting.</style> <style=cIsDamage>Slash</style> close-range combatants for <style=cIsDamage>{100f * 2.2}% damage</style>. <style=cIsDamage>Attacks from behind are Critical Strikes.</style>");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_AIM_NAME", "Steady Aim");
            LanguageAPI.Add(prefix + "SECONDARY_AIM_DESCRIPTION", $"Take aim, <style=cIsUtility>exposing enemy weak points</style>. <style=cIsDamage>Using primary while aiming fires your held gun.</style>");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_DODGE_NAME", "Quickstep");
            LanguageAPI.Add(prefix + "UTILITY_DODGE_DESCRIPTION", "<style=cIsUtility>Dash</style> a short distance. If used to <style=cIsUtility>avoid an attack</style>, <style=cIsDamage>roll</style> instead, restoring all charges of this skill.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_SWAP_NAME", "idk");
            LanguageAPI.Add(prefix + "SPECIAL_SWAP_DESCRIPTION", $"<style=cIsUtility>Swap</style> to a different <style=cIsDamage>gun</style>. Tap to swap to your <style=cIsUtility>last held gun</style>.");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "UNLOCKABLE_UNLOCKABLE_NAME", "Looming Dread");
            LanguageAPI.Add(prefix + "UNLOCKABLE_ACHIEVEMENT_NAME", "Looming Dread");
            LanguageAPI.Add(prefix + "UNLOCKABLE_ACHIEVEMENT_DESC", "Reach stage 3 in less than 15 minutes.");

            LanguageAPI.Add(prefix + "MONSOONUNLOCKABLE_UNLOCKABLE_NAME", "HUNK: Mastery");
            LanguageAPI.Add(prefix + "MONSOONUNLOCKABLE_ACHIEVEMENT_NAME", "HUNK: Mastery");
            LanguageAPI.Add(prefix + "MONSOONUNLOCKABLE_ACHIEVEMENT_DESC", "As HUNK, beat the game or obliterate on Monsoon.");
            #endregion

            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_AMMO_NAME", "Ammo");
            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_AMMO_CONTEXT", "Rummage for ammo");

            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_KEYWORD_LOOTING", "<style=cKeywordName>Looting</style><style=cSub>Enemies slain with this skill have a small chance to drop ammo.");
        }
    }
}
