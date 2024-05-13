# HUNK
- Adds HUNK from the Resident Evil franchise
- Has a couple item displays, unlockable skins and skills and is fully multiplayer compatible
- Comes with a unique side objective and a handful of unique weapons to obtain (permanently)
- Configurable stats and a few other things to mess around with
- Full Risk of Options support for all configuration options

[![](https://github.com/ArcPh1r3/Hunk/blob/main/Release/FuckShit/screen1.png?raw=true)]()

[![](https://github.com/ArcPh1r3/Hunk/blob/main/Release/FuckShit/screen2.png?raw=true)]()

[![](https://github.com/ArcPh1r3/Hunk/blob/main/Release/FuckShit/screen3.png?raw=true)]()

[![](https://github.com/ArcPh1r3/Hunk/blob/main/HunkUnityProject/Assets/Hunk/Icons/texHunkIcon.png?raw=true)]()


To share feedback, report bugs, or offer suggestions feel free to create an issue on the GitHub repo or join the Discord: https://discord.gg/HV68ujvkqe

___

# Skills

## Passive
Every stage an enemy will get infected with the G-Virus, powering it up a little bit. This buff grows substantially over time so it's crucial to find the virus early and neutralize it before it becomes a serious threat.

[![](https://github.com/ArcPh1r3/Hunk/blob/main/Release/FuckShit/passive.gif?raw=true)]()

Once defeated, you will obtain a G-Virus sample that can be turned into an Umbrella Terminal to exchange it for a keycard. Keycards can be used to open the corresponding weapon cases which you'll find on each stage.

[![](https://github.com/ArcPh1r3/Hunk/blob/main/Release/FuckShit/passive3.gif?raw=true)]()

## Primary
Basic melee attack meant for conserving ammo when it's safe to do so. Killing with this also has a chance for them to drop ammo on death.

[![](https://github.com/ArcPh1r3/Hunk/blob/main/Release/FuckShit/primary.gif?raw=true)]()

## Secondary
Aim your held gun and fire. Each gun has ammo and reloads either passively or manually when out.

[![](https://github.com/ArcPh1r3/Hunk/blob/main/Release/FuckShit/secondary.gif?raw=true)]()

Each gun has its own unique properties, such as AoE or headshot multipliers.

[![](https://github.com/ArcPh1r3/Hunk/blob/main/Release/FuckShit/secondary2.gif?raw=true)]()

## Utility
Take a quick step forward. Using this in anticipation of an enemy attack will perform a Perfect Dodge, refunding the cooldown and allowing you to retaliate with a devastating counterattack.

[![](https://github.com/ArcPh1r3/Hunk/blob/main/Release/FuckShit/utility.gif?raw=true)]()

Using primary after a Perfect Dodge will perform a counterattack. These do leave you open so use them wisely.

[![](https://github.com/ArcPh1r3/Hunk/blob/main/Release/FuckShit/utility2.gif?raw=true)]()

## Special
Open up the weapon menu and swap weapons. Pressing secondary will drop the selected weapon. Tapping the key swaps to your last equipped weapon, shown on your back.

[![](https://github.com/ArcPh1r3/Hunk/blob/main/Release/FuckShit/special.gif?raw=true)]()

___

# Unique Item Interactions
Since cooldown items do almost nothing on HUNK, most of them have unique interactions that play around his ammo system instead


Backup Magazine - +1 fake bullet every time you complete a reload

Bandolier - Gives you half a mag of fake bullets on pickup

Alien Head - Increased ammo on pickup, same scaling and nerfed if Green Alien Head is installed

Brainstalks - Shots don't consume ammo while active


FAKE BULLETS can be fired without consuming any actual ammo, but are all lost when you reload or swap weapons

___

# Compatibility Stuff

### RiskUI
HUNK's custom weapon HUD is fully compatible with this UI overhaul

[![](https://github.com/ArcPh1r3/Hunk/blob/main/Release/FuckShit/riskui.png?raw=true)]()

___


## Donations
If you enjoy my work and would like to support me, you can donate to my [Ko-fi](https://ko-fi.com/robdev)


## Credits
rob - Code, animation, sfx, mostly everything

swuff - Code, design help, entire interactable system, just an awesome co-dev overall

RandomlyAwesome - Controller support for radial menu

Thingw - Secondary and Special skill icons

Bruh - Golden Gun gun model

Capcom - Models


Big thanks to everyone in the community for all the valuable feedback and criticism as well, he wouldn't be the same without it


## Future Plans
- More weapons
- Other things 100% decided on pure whimsy
- Proper skill icons
- More mod crosscompat
- More unique counterattacks
- Item displays
- Translation support
- Ancient Scepter support
- Emotes

## Known Issues
- Most item displays are missing
- Emote buttons are there but he doesn't have any animations yet
- Ally projectiles trigger perfect dodges; haven't decided if this is worth keeping as a mechanic yet
- Goobo Jr. has no ammo and only shanks
- Weapon cases are not fully synced; this is purely visual and has no actual gameplay impact

___

## Changelog

`1.0.10`
- Added config to hide the gun icon from the HUD as it's kinda unnecessary and just clutter (unlike Driver who needs it)
- Added the fancy ammo display from RiskUI to the normal UI, with a config to revert this
- Added config to control the opacity of the background for this ammo display
- Added config to let the infection event continue even after getting every keycard
- GM-79 damage: 1600% > 3200%, it was simply way too weak for its very limited availability
- Non-HUNK players can no longer pick up G-Virus Samples or U.C. Keycards, this can also be reverted via config
- Fixed broken weapon icon again

`1.0.9`
- Fixed broken weapon icon and NRE spam

`1.0.8`
- Neck snap can now be cancelled early by simply moving, and the dodge cancel window has been moved to immediately after the snap
- Stage 5 Infected monsters now swap to the neutral team, heavily nerfed their sustain as well
- Reanimated SMG and Pistol reloads
- Fixed weapon/item models not having any normal maps
- Fixed weapon menu not working on controllers (fix by RandomlyAwesome!)

`1.0.7`
- Added a custom escape sequence with BGM and everything (can be disabled in config if you hate epicness)
- Fixed possibility of Chemical Flamethrower SFX playing infinitely if HUNK died while using it (unsure if this actually happened, but just in case)

`1.0.6`
- Added custom portrait for Tofu

`1.0.5`
- Fixed anyone being able to pick up HUNK's guns
- Fixed HUNK being able to grab more guns even with a full inventory
- Fixed HUNK being able to grab duplicate guns
- Fixed broken ragdoll turning into some body horror monstrosity and covering the whole screen
- Fixed Goobo Jr. giving the Lightweight achievement
- Fixed Goobo also spawning the ATM-4
- Fixed ammo pickups in multiplayer properly this time

`1.0.4`
- Fixed Chemical Flamethrower spamming its startup sound for everyone else, inflicting severe pain to everyone's eardrums

`1.0.3`
- Added config to disable the global sound cue when an Infected enemy spawns; don't blame me if you turn this off and they mutate to max level
- Infection is now capped at 5 mutation stacks, but the final stack gives much more noticeable buffs to compensate
- ^There is a config option to uncap it and restore the original behavior
- Lightning Hawk damage: 1800% > 2400%
- Quickdraw Army damage: 1800% > 2400%, fire rate drastically increased
- ^ damage being equal to shotguns made no sense as they're single target only
- G-Virus infection is now capped at 5 stacks; this can be disabled in config for those who enjoy spawning superbosses
- Fixed Lightweight unlock; apparently monsters were giving you the unlock as they had no items
- Fixed MUP and M19 SFX not playing for others in multiplayer
- Fixed Weapon Cases breaking in singleplayer when EmptyChestsBeGone or ChestQoL were installed
- Fixed Bandolier pickups getting rid of any excess ammo from a Backup Mag if you had any
- Fixed G-Virus (the hidden item given to Infected enemies) showing up in the logbook
- Fixed ammo pickups being visible (and stealable) by non-HUNK players in multiplayer

`1.0.2`
- attempted another bugfix..

`1.0.1`
- tiny fix whoops

`1.0.0`
- Initial release