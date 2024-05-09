using R2API;
using System;

namespace HunkMod.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            string prefix = MainPlugin.developerPrefix + "_HUNK_BODY_";

            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_PARASITE_BODY_NAME", "G-Young");

            string desc = "HUNK is an elite infiltrator who carries a large arsenal of weapons claimed via OSP (on-site procurement).<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Use your Combat Knife against weaker foes to conserve ammo." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Find UES Keycards to unlock Weapon Cases to add to your armaments on each stage." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Use Quickstep carefully to Roll between enemy attacks while taking them down at the same time." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Scrounge around in opened chests for a chance to find ammo for your current weapons." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, fine.";
            string outroFailure = "..and so he vanished, human unit now killed.";

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
            LanguageAPI.Add(prefix + "TOFU_SKIN_NAME", "Tofu");
            LanguageAPI.Add(prefix + "SUPER_SKIN_NAME", "<color=#" + Helpers.yellowItemHex + ">Early Supporter" + Helpers.colorSuffix);
            LanguageAPI.Add(prefix + "LIGHTWEIGHT_SKIN_NAME", "Lightweight");
            LanguageAPI.Add(prefix + "COMMANDO_SKIN_NAME", "Commando");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PPASSIVE_NAME", "OSP (On-Site Procurement)");
            LanguageAPI.Add(prefix + "PPASSIVE_DESCRIPTION", $"HUNK can collect <color=#" + Helpers.yellowItemHex + ">U.C. Keycards" + Helpers.colorSuffix + " to open <style=cIsDamage>weapon cases</style> and expand his arsenal.");

            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Grim Struggle");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", $"Start with a <style=cIsDamage>LE 5</style> and <style=cIsDamage>MUP</style>. You can <style=cIsDamage>rummage</style> through <style=cIsUtility>chests</style> for spare ammo.");

            LanguageAPI.Add(prefix + "PASSIVE2_NAME", "Looming Dread");
            LanguageAPI.Add(prefix + "PASSIVE2_DESCRIPTION", $"Start with a <style=cIsDamage>full arsenal</style>,  <style=cIsHealth>BUT</style> you are now <style=cIsHealth>unable to gain any more ammo</style>.");

            LanguageAPI.Add(prefix + "CONFIRM_NAME", "Confirm");
            LanguageAPI.Add(prefix + "CONFIRM_DESCRIPTION", "Proceed with the current skill.");

            LanguageAPI.Add(prefix + "CANCEL_NAME", "Cancel");
            LanguageAPI.Add(prefix + "CANCEL_DESCRIPTION", "Cancel the current skill.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_KNIFE_NAME", "Combat Knife");
            LanguageAPI.Add(prefix + "PRIMARY_KNIFE_DESCRIPTION", $"<style=cIsUtility>Looting.</style> <style=cIsDamage>Slash</style> close-range combatants for <style=cIsDamage>{100f * 3.5}% damage</style>.");

            LanguageAPI.Add(prefix + "SHOOT_ATM_NAME", "Fire");
            LanguageAPI.Add(prefix + "SHOOT_ATM_DESCRIPTION", "Shoot for <style=cIsDamage>6400% damage</style>. <style=cIsUtility>Deals most damage on direct hits.</style>");

            LanguageAPI.Add(prefix + "SHOOT_ROCKETLAUNCHER_NAME", "Fire");
            LanguageAPI.Add(prefix + "SHOOT_ROCKETLAUNCHER_DESCRIPTION", "Shoot for <style=cIsDamage>4800% damage</style>. <style=cIsDamage>Igniting.</style>");

            LanguageAPI.Add(prefix + "SHOOT_GRENADELAUNCHER_NAME", "Fire");
            LanguageAPI.Add(prefix + "SHOOT_GRENADELAUNCHER_DESCRIPTION", "Shoot for <style=cIsDamage>4800% damage</style>. <style=cIsDamage>Igniting.</style>");

            LanguageAPI.Add(prefix + "SHOOT_FLAMETHROWER_NAME", "Fire");
            LanguageAPI.Add(prefix + "SHOOT_FLAMETHROWER_DESCRIPTION", "Shoot a stream of flame for <style=cIsDamage>500% damage</style>. <style=cIsUtility>Ignites.</style>");

            LanguageAPI.Add(prefix + "SHOOT_M19_NAME", "Fire");
            LanguageAPI.Add(prefix + "SHOOT_M19_DESCRIPTION", "Shoot for <style=cIsDamage>700% damage</style>. <style=cIsUtility>50% headshot bonus.</style>");

            LanguageAPI.Add(prefix + "SHOOT_MAGNUM_NAME", "Fire");
            LanguageAPI.Add(prefix + "SHOOT_MAGNUM_DESCRIPTION", "Shoot for <style=cIsDamage>1800% damage</style>. <style=cIsUtility>50% headshot bonus.</style>");

            LanguageAPI.Add(prefix + "SHOOT_MUP_NAME", "Fire");
            LanguageAPI.Add(prefix + "SHOOT_MUP_DESCRIPTION", "Shoot for <style=cIsDamage>320% damage</style>. <style=cIsUtility>25% headshot bonus.</style>");

            LanguageAPI.Add(prefix + "SHOOT_REVOLVER_NAME", "Fire");
            LanguageAPI.Add(prefix + "SHOOT_REVOLVER_DESCRIPTION", "Shoot for <style=cIsDamage>1800% damage</style>. <style=cIsUtility>50% headshot bonus.</style>");

            LanguageAPI.Add(prefix + "SHOOT_GOLDGUN_NAME", "Fire");
            LanguageAPI.Add(prefix + "SHOOT_GOLDGUN_DESCRIPTION", "Shoot for <style=cIsDamage>99999% damage</style>. <style=cIsUtility>50% headshot bonus.</style>");

            LanguageAPI.Add(prefix + "SHOOT_BLUEROSE_NAME", "Fire");
            LanguageAPI.Add(prefix + "SHOOT_BLUEROSE_DESCRIPTION", "Shoot for <style=cIsDamage>1800x2% damage</style>. <style=cIsUtility>50% headshot bonus.</style>");

            LanguageAPI.Add(prefix + "SHOOT_SHOTGUN_NAME", "Fire");
            LanguageAPI.Add(prefix + "SHOOT_SHOTGUN_DESCRIPTION", "Shoot for <style=cIsDamage>140x14% damage</style>. <style=cIsUtility>50% headshot bonus.</style>");

            LanguageAPI.Add(prefix + "SHOOT_SLUGGER_NAME", "Fire");
            LanguageAPI.Add(prefix + "SHOOT_SLUGGER_DESCRIPTION", "Shoot for <style=cIsDamage>1800% damage</style>. <style=cIsUtility>50% headshot bonus.</style>");

            LanguageAPI.Add(prefix + "SHOOT_SMG_NAME", "Fire");
            LanguageAPI.Add(prefix + "SHOOT_SMG_DESCRIPTION", "Shoot for <style=cIsDamage>280% damage</style>. <style=cIsUtility>25% headshot bonus.</style>");
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
            LanguageAPI.Add(prefix + "SPECIAL_SWAP_NAME", "Legion");
            LanguageAPI.Add(prefix + "SPECIAL_SWAP_DESCRIPTION", $"<style=cIsUtility>Swap</style> to a different <style=cIsDamage>gun</style>. Tap to swap to your <style=cIsUtility>last held gun</style>.");
            #endregion

            #region Knife Skins
            LanguageAPI.Add(prefix + "KNIFE_DEFAULT_NAME", "Combat Knife");
            LanguageAPI.Add(prefix + "KNIFE_DEFAULT_DESCRIPTION", "A standard military-grade knife. Sure to come in handy in a pinch.");
            LanguageAPI.Add(prefix + "KNIFE_INFINITE_NAME", "Combat Knife EX");
            LanguageAPI.Add(prefix + "KNIFE_INFINITE_DESCRIPTION", "A specially-treated military-grade knife that's been hardened to an unbelievable degree. They say it's been made to really last.");
            LanguageAPI.Add(prefix + "KNIFE_HIDDEN_NAME", "Hidden Blade");
            LanguageAPI.Add(prefix + "KNIFE_HIDDEN_DESCRIPTION", "A low profile retractable blade hidden in the sleeve. Ideal for stealth operations.");
            #endregion

            #region Achievements
            prefix = MainPlugin.developerPrefix + "_HUNK_";
            LanguageAPI.Add(prefix + "UNLOCKABLE_UNLOCKABLE_NAME", "Looming Dread");
            LanguageAPI.Add(prefix + "UNLOCKABLE_ACHIEVEMENT_NAME", "Looming Dread");
            LanguageAPI.Add(prefix + "UNLOCKABLE_ACHIEVEMENT_DESC", "Reach stage 3 in less than 15 minutes.");

            LanguageAPI.Add(prefix + "COMPLETION_UNLOCKABLE_NAME", "HUNK: Grim Reaper");
            LanguageAPI.Add(prefix + "COMPLETION_ACHIEVEMENT_NAME", "HUNK: Grim Reaper");
            LanguageAPI.Add(prefix + "COMPLETION_ACHIEVEMENT_DESC", "As HUNK, beat the game or obliterate.");

            LanguageAPI.Add(prefix + "MONSOON_UNLOCKABLE_NAME", "HUNK: Mastery");
            LanguageAPI.Add(prefix + "MONSOON_ACHIEVEMENT_NAME", "HUNK: Mastery");
            LanguageAPI.Add(prefix + "MONSOON_ACHIEVEMENT_DESC", "As HUNK, beat the game or obliterate on Monsoon.");

            LanguageAPI.Add(prefix + "SUPPORTER_UNLOCKABLE_NAME", "HUNK: Early Supporter");
            LanguageAPI.Add(prefix + "SUPPORTER_ACHIEVEMENT_NAME", "HUNK: Early Supporter");
            LanguageAPI.Add(prefix + "SUPPORTER_ACHIEVEMENT_DESC", "Play HUNK before his official release.");
                
            LanguageAPI.Add(prefix + "LIGHTWEIGHT_UNLOCKABLE_NAME", "HUNK: Minimalist");
            LanguageAPI.Add(prefix + "LIGHTWEIGHT_ACHIEVEMENT_NAME", "HUNK: Minimalist");
            LanguageAPI.Add(prefix + "LIGHTWEIGHT_ACHIEVEMENT_DESC", "As HUNK, clear stage 1 with no items.");

            LanguageAPI.Add(prefix + "CQC_UNLOCKABLE_NAME", "HUNK: CQC (Close-Quarters Combat)");
            LanguageAPI.Add(prefix + "CQC_ACHIEVEMENT_NAME", "HUNK: CQC (Close-Quarters Combat)");
            LanguageAPI.Add(prefix + "CQC_ACHIEVEMENT_DESC", "As HUNK, perform 10 successful counterattacks on one stage.");
            #endregion



            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_SPADE_KEYCARD_NAME", "U.C. Keycard (Spade)");
            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_CLUB_KEYCARD_NAME", "U.C. Keycard (Club)");
            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_HEART_KEYCARD_NAME", "U.C. Keycard (Heart)");
            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_DIAMOND_KEYCARD_NAME", "U.C. Keycard (Diamond)");
            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_KEYCARD_DESC", "Used to open Umbrella Corp <style=cIsUtility>weapon cases</style>.");

            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_G_VIRUS_SAMPLE_NAME", "G-Virus Sample");
            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_G_VIRUS_SAMPLE_DESC", "Volatile sample of the <color=#" + Helpers.voidItemHex + ">Golgotha Virus" + Helpers.colorSuffix + ". <style=cIsHealth>Handle with care.</style>");

            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_HEARTCHEST_NAME", "Heart-Key Weapon Case");
            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_HEARTCHEST_CONTEXT", "Open Heart-Key Weapon Case");
            LanguageAPI.Add(MainPlugin.developerPrefix + "_HEARTCOST", "Heart Key Card");

            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_SPADECHEST_NAME", "Spade-Key Weapon Case");
            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_SPADECHEST_CONTEXT", "Open Spade-Key Weapon Case");
            LanguageAPI.Add(MainPlugin.developerPrefix + "_SPADECOST", "Spade Key Card");

            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_CLUBCHEST_NAME", "Club-Key Weapon Case");
            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_CLUBCHEST_CONTEXT", "Open Club-Key Weapon Case");
            LanguageAPI.Add(MainPlugin.developerPrefix + "_CLUBCOST", "Club Key Card");

            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_DIAMONDCHEST_NAME", "Diamond-Key Weapon Case");
            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_DIAMONDCHEST_CONTEXT", "Open Diamond-Key Weapon Case");
            LanguageAPI.Add(MainPlugin.developerPrefix + "_DIAMONDCOST", "Diamond Key Card");

            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_TERMINAL_NAME", "Terminal");
            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_TERMINAL_CONTEXT", "Insert Sample");
            LanguageAPI.Add(MainPlugin.developerPrefix + "_SAMPLECOST", "G-Virus Sample");

            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_CHEST_NAME", "Weapon Case: ");
            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_CHEST_CONTEXT", "Open Weapon Case");

            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_AMMO_NAME", "Ammo");
            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_AMMO_CONTEXT", "Rummage for ammo");

            LanguageAPI.Add(MainPlugin.developerPrefix + "_HUNK_KEYWORD_LOOTING", "<style=cKeywordName>Looting</style><style=cSub>Enemies slain with this skill have a small chance to drop ammo.");
        }
    }
}
