## Rocket

[![](https://i.imgur.com/W3dQUcH.jpg)]()
[![](https://i.imgur.com/3Q0wCWP.png)]()
	
Original Concept Art by bruh#6900
[![](https://i.imgur.com/P2GpdDk.png)]()

If you have any feedback/suggestions, open an issue on the Github page, or join the discord at https://discord.gg/pKE3QCEsxG

- Stuff not mentioned in-game:
	- Rocket Jumping
		- Maintain momentum by either inputting in the direction of your jump trajectory, or by letting go of the movement keys.
		- Can gradually turn without losing speed.
		
	- Rocket Launcher
		- Rocket knockback scales with enemy mass when hitting non-boss enemies (up to 250kg, Brass Contraption weight)

	- Remote Detonator
		- Removes blast falloff from explosions.
		
	- Bombing Run
		- Crit Chance is converted to crit damage when using this skill.
		
	- Rapid Rearm
		- Rockets have less knockback against enemies.

## To Do

- Mastery Skin

## Installation
Drop the dll into \BepInEx\plugins\

## Credits

- Moffein (Code)
- TimeSweeper (Henry template, animation, getting the model working in-game)
- SkeletorChampion (Model)
- MoistyBoy (UES Prime's Rocket)
- bruh#6900 (Original Concept Art)
- DaPedro#2713 (Skill Icons)
- Domi (Animations)
- swuff (Bankroller)
- BoxRoss (VR C4 stuff)

- Sounds taken from:
	- Reflex Arena
	- Quake Champions
	- Risk of Rain 1
	- TF2
	- Dirty Bomb

## Changelog

`0.9.7`

- Made emote skeleton less buggy.
	- No more getting permanently rotated, but your height/hip position can still permanently change.

<details>

`0.9.6`

- Added RU translation (Thanks ILIa3174!)

`0.9.5`

- Fixed Visions of Heresy breaking Rapid Rearm.

`0.9.4`

- Added FR translation (Thanks darkwarrior45!)

`0.9.3`

- Added BR translation (Thanks Kauzok!)

`0.9.2`

- VR: Fixed C4 MethodAccessException

`0.9.1`

- VR: Removed some redunant VR checks from C4.
	- Still needs testing.

`0.9.0`

- Rapid Rearm
	- Now scales off of max primary stocks. (No change unless you have mods that let you increase max primary stocks)
	- Rewrote code to be less hardcoded. Should be easier to make custom skills work with it.
		- See FireAllRockets.RocketSkillInfo and FireAllRockets.rocketSkillInfoList if you are a developer.
	- Fixed incorrect interaction with Pocket ICBM.
		- Old (Bugged): Outer 2 rockets had knockback, center rocket didn't.
		- New (Fixed): Center rocket has knockback, outer 2 rockets don't.
	- Now plays the correct sound when using the alt primary.



`0.8.7`

- VR: C4 is thrown from offhand (thanks BoxRoss!)
	- Untested.	

`0.8.6`

- Added footstep sounds (thanks TimeSweeper!)

`0.8.5`

- Added IdleIn animation (thanks TimeSweeper!)
- Fixed reload particle effect being sideways.

`0.8.4`

- Extra null checking on Remote Detonator.

`0.8.3`

- Fixed Rapid Rearm ignoring ICBM knockback config due to 0.8.0

`0.8.2`

- Fixed TestState console spam.

`0.8.1`

- Fixed C4 not triggering blast jump for clients when detonated by placing a new C4.

`0.8.0`

- Netcode Overhaul
	- HG4 Rocket Launcher: Major Improvement
		- Blast jumping is mostly clientside now.
			- Blast jump AoE might not line up with the projectile visual when playing online, but rapid detonating to blast jump with M2 should work a lot better now.
		
	- Nitro Charge: Improved
		- Blast jump is clientside when detonating with Remote Detonator.
		- Position is still server-side, so it won't be as responsive as the default primary.
		
	- HG4 SAM Launcher: No Change (Technical Limitations)
		- New rocket stuff doesn't work with this because of the homing (forces prediction to be disabled).
			- Nitro Charge also has prediction disabled, but doesnt rely on impact detonation which is why it can benefit from clientside blast jumps.
	
	- Remote Detonator
		- Blast jump immediately triggers for clients.
			- SAM Launcher does not benefit from this.
			
		- Rocket/Projectile counter is now client-side instead of server-side.
			- This makes blast jumping more responsive but **can cause cases where you dont actually detonate the projectile on the server**.
		
`0.7.3`

- Fixed Ceremonial Dagger itemdisplay being massive.
- Disabled unused Mastery Achievement. Will be re-enabled when Mastery Skin is added.

`0.7.2`

- Fixed Rapid Rearm using the Scepter skill description.

`0.7.1`

- Added unlock requirements for alt skills.
	- Can be bypassed with the Force Unlock config.
	- Mastery Unlock will be added when the Mastery skin is ready.
- AssetBundle/Soundbank are no longer embedded in the dll. This will reduce RAM usage.
- Added Character Select sound.
- Emotes
	- Fixed emotes not working online.
	- Fixed the Survivor Select Pose emote only working if used while in another emote state.
	- Added sounds.
- Added Language folder.

- HG4 SAM Launcher
	- Removed airshot bonus.
	
	*This was a holdover from before this skill had homing. The skill's already strong enough to not need it.*

- Bombing Run
	- Speed damage bonus now only counts speed above base sprinting speed.
	
- Rocketstorm (Scepter)
	- Increased rocket count from 8 -> 10

`0.7.0`

*Pre-1.0 update. All that's needed is the Mastery skin, a few animation tweaks, and sounds on Emotes + Character Select.*

- Huge animation overhaul (Thanks Domi!)
- Added missing Scepter icon.
- Added built-in emotes (don't press 3).
- Added EmoteAPI support.
- Set up Vengeance/Goobo AI.

- Stats
	- Fixed HP/level being set to 30 instead of 33.

- Primaries
	- Increased damage from 570% -> 600%

- Remote Detonator
	- Reduced cooldown from 3s -> 2s

- Nitro Charge
	- Increased damage from 1000% -> 1200%
	- No longer cancels primary reload.
	- Fixed bombs persisting after death.
	
- Bombing Run
	- Is now Heavy. (Deals extra damage based on speed)
	- Increased damage from 1000% -> 1200%
	- No longer cancels primary reload.
	- No longer restores cooldown on miss.
	- Changed input method.
		- Hold the skill down to ready your shovel.
		- Release to instantly swing without delay.
			- There is no minimum duration, so you can just tap the button to release it instantly.
	
- Rapid Rearm
	- Reduced cooldown from 10s -> 8s
	- No longer cancels sprinting.
	- Removed random spread.

`0.6.1`

- HG4 Rocket Launcher
	- Reduced blast damage falloff from -75% -> -50%

`0.6.0`

- Improved online rocket jump responsiveness.
	- Physics are now calculated client-side.
	- Explosion position/detonation time are still server-side.

`0.5.6`

- Fixed SAM Launcher rockets not homing in MP.
	- They'd home on the server, but clients would only see them going straight.

`0.5.5`

- Updated CachedName to be the same as what's listed on ModdedCharacterEclipseFix.

`0.5.4`

- Remembered to set CachedName field in SurvivorDef. Hopefully this will fix Eclipse progress not saving.
- Fixed M2 being broken online due to a recent update.
- Fixed Nitro Chage not sticking to walls online.

`0.5.3`

- HG4 Rocket Launcher
	- Increased damage from 520% -> 570%
	
- HG4 SAM Launcher
	- Reduced damage from 650% -> 570%
	
	*The SAM Launcher ended up getting buffed too much over the past few updates. Homing and airshot bonus alone are enough to overcome its -45% AoE downside.*

`0.5.2`

- Moved aimOrigin back slightly to reduce instances where rockets go through walls at close range.
- Updated Nitro Charge icon.

`0.5.1`

- Fixed Bombing Run damage not getting multiplied by your damage stat. (from 0.5.0)

`0.5.0`

- Added skill icons (Thanks DaPedro!)
- Enemy knockback scaling now only scales up to 250kg (Brass Contraption weight)
	- Golems can still be launched, but they won't go as far as before.
- Removed Concussion Blast
- Added Nitro Charge (Default Utility)
	- TODO: New icon
	- Toss an explosive charge. Detonate it with Remote Detonator for 1000% damage.
	- Knockback is the same as Concussion Blast (but does not get increased by Remote Detonator).
	- Can have up to 1 (+1 per Utility charge) active. Going above this limit will detonate your earliest one.
	
	*Modded characters have too many debuffs, and Rocket's Conc nade debuff was very niche and was only really useful for himself, as opposed to something like Sniper's Spotter where the whole team can benefit from it. Additionally, the skill felt pretty lackluster compared to Bombing Run.*
	
`0.4.0`

- Added aerial pose (Thanks TimeSweeper!)
- Remote Detonator
	- Changed explosion SFX.
	- Changed network code. Let me know if this improves/worsens online responsiveness.
- Concussion Blast
	- Changed VFX.

`0.3.4`

- Added config to disable HG4 SAM Launcher homing.

`0.3.3`

- Remembered to include the dll.

`0.3.2`

- HG4 SAM Launcher
	- Increased blast radius from 4m -> 4.5m
	- Reduced lockon angle from 20 degrees -> 12 degrees
	- Increased lockon turn speed from 50 -> 60
	- Now only attempts to lock on to targets within LoS.
	- Now prioritizes targets solely based on angle, to reduce instances where it will swerve towards a completely different target.
	
	*Lockon tolerance is tighter, but rockets will be more consistent at actually locking on to relevant things.*
	
- Remote Detonator
	- Increased damage and AoE bonuses from +30% -> +50%
		- Force multiplier remains at +30% since it already is pretty strong.
	- Increased SAM Launcher AoE from 8m -> 10m
	
	*A common feedback with this skill was that it was really good for mobility but not very impactful mid-combat.*

`0.3.1`

- Pocket ICBM
	- Extra rockets no longer have knockback.
		- Can be re-enabled in the config.

`0.3.0`

- HG4 SAM Launcher
	- Reduced blast radius from 5m -> 4m
	- Reduced rocket targetfinder range from 80m -> 60m
	
	*Previous numbers were too good at long range, being able to easily hit targets even if your aim was way off.*

`0.2.19`

- Fixed ICBM damage mult being applied when ICBM Config is disabled.
- Fixed Special not benefitting from the ICBM damage mult.

`0.2.18`

- HG4 SAM Launcher
	- Increased damage from 600% -> 650%
	- Increased blast radius from 3m -> 5m
		- Note: SAM Launcher has no blast falloff.
	- Rockets now slightly home.
		- Numbers need adjustment (homing strength/range/lockon FOV), give feedback!
	
	*Experimenting with changing up the skill a bit. Unsure about the homing since it makes juggling harder due to autotargeting the center of certain enemies.*

- Pocket ICBM now affects rocket skills.
	- Can be disabled in config.

`0.2.17`

- Fixed Bombing Run immediately getting cancelled by Primary skills.

`0.2.16`

- Fixed outro tokens not showing.

`0.2.15`

- Updated default skin icon(Thanks Timesweeper!)

`0.2.14`

- Fixed internal version number.

`0.2.13`

- Increased Rocket Jump no-momentum-loss angle from +/- 10 -> +/- 20
	- Hoping to make forwards jumping more consistent. Will adjust this further if needed.
- Fixed certain skills getting added to the game's skilldef catalog twice.
- Added Scepter skill.
	- Rocketstorm: doubles Rapid Rearm's rockets fired and fire rate.
- Removed unused Henry assets to reduce filesize.
- Added default skin icon.

`0.2.12`

- Remote Detonator
	- Now detonates all active rockets.
	
	*This should fix the issue where you're able to waste charges on rockets that are flying away into the skybox.*

`0.2.11`

- HG4 SAM Launcher
	- Increased Remote Detonator AoE from 3.9m -> 8m
		- 8m is the radius of the default Primary's rockets WITHOUT Remote Detonator.

- Concussion Blast (Utility)
	- Reduced cooldown from 7s -> 5s
	
- Bombing Run (Utility)
	- Reduced cooldown from 7s -> 5s

- Rapid Rearm (Special)
	- Reduced cooldown from 12s -> 10s

`0.2.10`

- Fixed Rocket being unable to be healed by Heal Drones and HAN-D.
	- Probably fixes a few other bugs related to autotarget skills.

`0.2.9`

- Improved rocket jump consistency.
	- Now uses a proper hitbox overlap check, instead of just calculating distance from the explosion to the player.

`0.2.8`

- Fixed Alt M1 being bugged in multiplayer.
	- Might also result in a few other things being fixed.

`0.2.7`

- Remote Detonator
	- Attempted to improve online responsiveness.
		- Instead of waiting for the EntityState to run on the server, the client will send a command to tell the server to detonate.
			- Not sure if this actually improves things or not. Feedback will be super helpful!
	
- Bombing Run
	- Now only consumes a stock if it hits at least 1 enemy.
	- Increased melee hitbox size from 4.4x4.4x3.2 -> 6x6x4.3
	- Increased blast radius from 8m -> 10m
	
- Additional Notes:
	- Concussion Blast
		- I'm not quite happy with this skill. I feel it's boring compared to Bombing Run.
			- Turn it into an actual grenade/clusterbomb?
		- Could maybe replace it with something else entirely if any ideas come up.
		
	- Rapid Rearm
		- Is this too basic?

`0.2.6`

- Thunderstore release.

</details>