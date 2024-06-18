## Changelog

`1.5.10`
- Added new frontal counterattack for Stone Golems- disables their laser attack for the next 60 seconds, and this counter cannot be repeated while the laser is disabled
- Added new counterattack for Bighorn Buffalos- mount them, aim for a second, then send them charging forward for 4000% to any enemies in its path, and 8000% self damage if it collides with a wall
- Added new counterattack for Elder Lemurians- stick a grenade in their mouth, which will detonate to interrupt their next attack for 8000% damage	
- Added new frontal counterattack for Alloy Vultures- quick, high damage two hit combo
- Added aerial "counterattack"?- hold primary during a perfect aerial dodge and collide with an enemy to cling onto them, allowing you to slash freely; this is incredibly risky so use with caution
- Polished the visuals of a handful of counterattacks

`1.5.9`
- Rolls can now be cancelled into movement MUCH earlier
- Fixed ProperSave weapon validation not happening until your inventory changed in some way
- Fixed Jacket variant having the same issue
- Fixed camera tracking breaking when rooted by void enemies
- Fixed knee counterattack sometimes having no animation

`1.5.8`
- Added custom popups when picking up items with unique interactions explaining what they do for HUNK specifically
- Fixed HUNKs disconnecting in multiplayer breaking item pickups for all players
- Fixed issues with Drop-In multiplayer
- Fixed ProperSave not properly spawning you in with your guns; this is a partial solution, and ammo saving will be added in later
- Fixed a camera config option not working as intended

`1.5.7`
- Fixed virus events happening after stage 1 as Jacket variant

`1.5.6`
- Added custom animations and weapon for Jacket variant

`1.5.5`
- Code refactoring to allow variants to exist

`1.5.4`
- Added cursed skin: Jacket
- Tweaked cursed Commando skin to be a little less awkward
- Removed the redundant second camera config option - too awkward and a waste of time to maintain
- Over the should camera config is now off by default, only new installs will be affected- too much whining about the character being "small", you win, it's now opt-in
- Improved ror2-style camera a little as well, adding some of the flair from the other option
- C-Virus enemy count: 10 > 4, it did end up feeling long despite the enemies being easier to kill, and general frustration was just high 
- G-Virus Infected allies now swap to the neutral team immediately, allowing you to kill them- I'll have no more complaints on this subject
- Blacklisted Scavengers from the C-Virus, as it prevented them from dropping their bags
- Fixed C-Virus Perfected elites never dying
- Fixed a couple harmless cosmetic errors

`1.5.3`
- Slightly increased LE 5 fire rate
- Increased CQBR Assault Rifle fire rate
- Lightning Hawk damage: 2800% > 3200%
- Quickdraw Army damage: 2800% > 3200%
- Long Barrel (Lightning Hawk) damage boost: 3200% > 4800%, now also removes distance falloff
- M19 distance falloff removed
- Weapon attachments now only show up in cases if you have the corresponding weapon
- Potentially fixed C-Virus never actually showing up? code seemed right to me but unity documentation seems to disagree so idk..

`1.5.2`
- Fixed broken unlock

`1.5.1`
- Removed leftover debugging code

`1.5.0`
- Added new virus event: C-Virus Outbreak
- C-Virus causes enemies to fully heal once killed, becoming invulnerable and frenzied for the next 8 seconds, after which they will self detonate
- C-Virus events have 10 enemies and drop a C-Virus Sample when the last one dies
- Added G-Virus eye displays to the SoTV enemies, making them 100% complete barring mod enemies (maybe one day)
- Added weapon case placements to Bob-omb Battlefield
- HUNK's multiplayer item protection is now disabled when there is no one playing HUNK (weird edge case)
- Fixed new items not using HUNK's multiplayer protection system

`1.4.7`
- Added G-Virus eye displays to the entire basegame enemy cast! SoTV enemies soon
- EMF Scanner range increased and added new effect making it actually useful
- Fixed New Game+ config not actually shuffling the weapon pool
- Fixed default weapon pool also not shuffling like it was supposed to

`1.4.6`
- Added New Game+ config option for fun
- Rewrote T-Virus to boost compatibility with other mods
- T-Virus outbreak victims lowered from 10 to 6, infected enemies now have a -10 armor debuff - T event was way more of a time sink than G, hoping to even that out
- Holding jump while firing Slugger now doubles the pushback
- Slugger damage: 1800% > 2400%
- Lightning Hawk damage: 2400% > 2800%
- Quickdraw Army damage: 2400% > 2800%
- Blue Rose damage: 2400x2% > 3800x2%
- Lively Pots are now always counted as in combat and can be perfect dodged
- Sentinels from TitansOfTheRift are now counted as Golems, meaning HUNK's camera adapts and his counterattack on them is now a punch
- M19 fire rate slightly increased and ammo pickup multiplier increased - Gun Stock shot MUP too far ahead so this one's getting a slight boost to keep up
- Fixed Alien Head boosting ammo amount way more than intended

`1.4.5`
- Bugfix

`1.4.4`
- Made Anti-Tank Missile spawn much more prominent - too many people were missing this which is my bad

`1.4.3`
- Overhauled custom HUD with almost full functionality and a properly working health bar with curse, barrier and shield all supported
- ^Most UI elements are hidden unless tab is being held or they're being used
- buffs are in progress
- works with riskui, but needs specific config options- inventory top, money/run info default, skill icons bottom right, rest shouldn't matter? 

`1.4.2`
- Fixed infection sound cues not playing at all

`1.4.1`
- Fixed terminals not showing their light beam if you were holding a T-Virus Sample

`1.4.0`
- Added new virus event: T-Virus Outbreak - there's now a chance for this to be triggered instead of the G-Virus event
- T-Virus infects every enemy currently alive, allowing them to defy death 2 times, and continues until at least 10 enemies have been infected
- Killing the last infected enemy will drop a T-Virus Sample, with the same function as the G-Virus Sample
- In multiplayer, the Sample drops every 10 kills and each HUNK gets their own 10 enemies - they also roll their virus types separately so you can have both events ongoing at once
- Added item: U.C. Keycard (Star) - used to access the Golden Gun
- Added item: U.C. Keycard (Master) - allows access to all cases by itself, allowing you to scrap/print all the others
- Added config to allow viruses to infect bosses, G-Virus can by default but T-Virus cannot
- Mangled damage: 2100% > 2800%, stack falloff timer 5s > 15s, still has room to be more rewarding/consistent
- Updated footstep sounds
- Tweaked G-Virus visuals
- Tweaked ammo popups to be more visible
- Fixed broken unlock
- Fixed Lemurians remaining shrunken after the neck snap finished
- Fixed new Beetle Guard counter animation not being networked
- Fixed certain counters having the enemies mispositioned when playing as a client in multiplayer

`1.3.3`
- LE 5 damage: 280% > 300%, fire rate slightly increased, spread slightly reduced - i am just biased i like this gun
- Primary hit hop height greatly increased
- Mangled damage: 800% > 2100% - just wasn't anywhere near rewarding enough for the required effort
- Fixed Mangled only proccing once per enemy
- Fixed Perfect Dodges on Golem lasers not properly identifying the attacker

`1.3.2`
- Added new debuff: Mangled - each knife hit adds one stack, upon reaching 6 stacks the enemy suffers an instant 800% damage
- Added custom interactions for Lysate Cell and Purity
- Added weapon case placements for Verdant Falls and the mod stages Fogbound Lagoon, Catacombs and Slumbering Satellite
- Added frontal counterattack for Beetle Guards
- Alloy Vultures can now be suplexed
- Countering Blind Vermin from behind is also a suplex now
- Generic knife counter (hold m1 while lunging without running into anything) damage boosted to 800%, uses the proper knife model and now counts as a counterattack
- Greatly extended the i-frames on counter kicks and punches to make them less of a flight risk
- Made EMF Scanner more precise
- Fixed extra weapon cases spawning on stages without preset positions
- Fixed Alien Head scaling negatively after getting more than one, resulting in 0 ammo from each pickup..

`1.3.1`
- Added a config option to deposit keycards straight to your inventory as a temporary fix for some edge case mod incompats

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