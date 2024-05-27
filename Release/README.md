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

tsuyoikenko - Slayer skin

Capcom - Models


Big thanks to everyone in the community for all the valuable feedback and criticism as well, he wouldn't be the same without it


## Future Plans
- More weapons
- Other things 100% decided on pure whimsy
- More mod crosscompat
- More unique counterattacks
- Item displays
- Translation support
- Emotes
- More virus types

## Known Issues
- Most item displays are missing
- Ally projectiles trigger perfect dodges; haven't decided if this is worth keeping as a mechanic yet
- Lore accurate Weskah config does not seem to be working

___

## Changelog

`1.3.0`
- Added tool: EMF Scanner - acts as a weapon case detector to make finding them less of a struggle
- Added gun: CQBR Assault Rifle
- Added a special weapon to Voidling's fight to match the one given for Mithrix
- Added item: I.D. Wristband
- Added a handful of new unlockable knife skins!!
- Added a lock-on effect to Perfect Dodges, intended to make counterattacks much more reliable- understandably can be disorienting so there is config to disable this behavior
- Perfect Dodges now ignore character collision
- Added a new counterattack when hitting Lemurians from behind that's faster and has AoE, necksnap haters rejoice
- Extended Magazine (LE 5) now has a model and icon
- Updated Imp counterattack animation
- Made Perfect Dodges a little less forgiving on projectiles
- Moved joke skins to cursed config and gave them proper icons
- Anti-tank Missile and Railgun now have a 1% chance of showing up in post-loop weapon cases
- Fixed barrels giving too much ammo
- Fixed weapon case pings not telling you what was inside the case anymore
- Fixed the contents of weapon cases being desynced on multiplayer again, not sure when this bug came back
- Fixed more virus terminals spawning every time you respawned

`1.2.2`
- Added Slayer skin, courtesy of tsuyoikenko!
- Added animations for heresy skills
- Tweaked sprints again
- Made ammo pickups slightly more conspicuous
- Fixed custom escape sequence BGM
- Fixed LE 5 not playing its SFX for other players in multiplayer
- Fixed LE 5 Extended Mag not being in the drop pool

`1.2.1`
- Big thanks to Ghor for playing HUNK on stream for this update! Very insightful and helped to identify areas where gameplay clarity could be improved
- Added keyword to passive explaining a little bit about the G-Virus
- Ammo pickups from chests now spawn from a pickup droplet to help visibility- can be reverted in config
- ^this is mostly to address new players missing all the ammo pickups, old behavior is faster so turn it off if you prefer that
- Picking up new guns now starts them with reserve ammo based on the gun, giving wiggle room to try out new weapons
- Bandolier pickups now fully refill the mag instead of only half (still fake ammo though)
- Adjusted fake ammo to be an extra display directly under your current mag, making it more immediately obvious what fake ammo is
- Fixed misplaced core position
- Fixed ammo pickups spawning while running Looming Dread in singleplayer, this was both confusing and disappointing

`1.2.0`
- Added Ancient Scepter upgrade!!
- Utility upgrade: Uroboros - Dodges now give full iframes, quicker movement, allow counterattacks without a Perfect Dodge and gain an extra charge
- ^in the future it'll have unique melee attacks but this is all for now
- Made Perfect Dodges more consistent against Beetle Guards and Parents
- Melee ammo drop rate is now doubled on Sacrifice
- Laser Sight (LE 5) now boosts the headshot bonus from 25% to 50%
- Added gun attachment: Extended Magazine (LE 5) - Boosts mag size to 60 and increases ammo on pickup from 32 to 90
- Added skin: Minecraft
- Fixed more counter animation issues
- Fixed Gun Stock no longer appearing at all due to the previous fix; all works as intended now
- Fixed rare issue where an infected enemy could die without spawning the bug
- Fixed Commencement intro dialogue replaying if HUNK died and respawned
- Fixed redundant weapon cases spawning while running Looming Dread in singleplayer
- Fixed infection passive sound being way too loud

`1.1.6`
- Added new skin: Mastermind
- Fixed Gun Stock being able to spawn more than once
- Fixed neck snap animation bug for real

`1.1.5`
- Added new passive: Hustle - HUNK's sprint now increases in speed over time up to a cap (it's not listed in the loadout yet)
- ^Overwhelming amounts of feedback regarding how slow he is, which is true, 0 traversal skills and a side objective that forces him to scour the whole map. this should subtly help his map traversal without impacting the rest of his gameplay
- Adjusted all sprint animations to be consistent with each other
- Fixed Schrödinger's Gun missing model again
- Fixed rare issue causing neck snap to have no animation
- Fixed emotes enabling the crosshair

`1.1.4`
- Networked the new counterattacks....

`1.1.3`
- Added rest emote
- Added a custom counterattack for Imp
- Added a custom counterattack for Clay Templar
- Added keywords and rewrote description for Quickstep
- Infected enemies can no longer be neck snapped- uses a different counter instead
- Enemies below half hp will have a different, faster counter instead of the neck snap
- Fixed model jittering while trying to move during certain animations
- Fixed enemies moving their heads during the neck snap animation

`1.1.2`
- Added config option for a custom HUD built for HUNK
- Added subtle camera interpolation, can be configured and even disabled
- Added a slight camera zoom out when near huge enemies to help with combat visibility, also with config
- Added a config option to adjust the distance the cursor has to be from the center to select a weapon, for those having issues
- Added more pep to his step :-)
- Added a custom (very strong) counterattack for Blind Vermin
- Enemies who survive a neck snap will no longer be stunned indefinitely, instead they will be enraged for 10 seconds
- Slightly extended the invulnerability window on neck snap- played correctly you can be indefinitely invulnerable now

`1.1.1`
- bugefix

`1.1.0`
- Added gun attachments! These show up in gun cases scattered throughout the standard gun pool
- Gun Stock (MUP): Allows you to fire 3 bullets in quick succession
- Laser Sight (LE 5): Removes spread, recoil and bullet falloff
- Long Barrel (Lightning Hawk): Increases damage from 2400% to 3200%
- Speedloader (Quickdraw Army): When the chamber is empty, reload all bullets at once
- LE 5 and MUP added to the post-loop weapon pool in case you dropped them and wanted to get them back later
- M19 ammo pickup amount doubled as a temporary solution until ammo weight system is added
- Gave M19 a laser sight because awesome or something
- Flamethrower ammo pickup amount halved, reload speed lowered to match the GM-79
- Flamethrower range reduced
- Made Perfect Dodge window smaller, but gave it more i-frames when successful
- Added incompatibility warning when playing the game with incompatible mods (ones i can't fix on my end)
- Fixed Golden Gun having no model
- Fixed MUP pickup model
- Fixed weapon cases not being synced in multiplayer

`1.0.11`
- Added a custom shield overlay (with config to disable), as well as a config to enable this overlay for ALL survivors
- Added config to add a shield bubble as well, though this one is off by default
- Added a custom ping icon for Umbrella Terminals
- Fixed Goobo Jr. spawning terminals every time he spawned

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