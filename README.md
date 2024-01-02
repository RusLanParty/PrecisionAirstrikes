<a href=https://www.patreon.com/RusLanParty>Patreon</a>
<a href=https://discord.gg/yAHThHFcAx>Discord</a></b>
-------------------------------------------------------------------------------------------------------------------------

<p>Unleash devastating aerial assaults by marking your targets with flares or IR marking spotlights.</p>

<p>Features:</p>
<ul>
  <li>Config menu, use DELETE key (configurable via .ini file)</li>
  <li>Two targeting modes: flare and IR marking spotlight (left mouse click)</li>
  <li>Once marked, air support will arrive to attack the designated target</li>
  <li>Three strike types:
    <ol>
      <li>Two jets equipped with powerful 30mm GAU-8 cannons, delivering precise and lethal bursts of 280 rounds each.</li>
      <li>A stealth bomber that releases a line of explosives, that break into smaller ones when reaching certain height, perfect for clearing entire streets.</li>
      <li>Twenty drones, armed with 15 small missiles each and an EMP generator. The drones rain down missiles upon the target and then generate an electromagnetic pulse, disabling all vehicles, including aircraft, in the area and cause a short power outage in the city. Ideal for countering helicopters.</li>
    </ol>
  </li>
  <li>Audio taken from Insurgency Sandstorm, DCS, MW2, and remixed by me</li>
  <li>Engage the jet camera by pressing R (configurable via .ini file)</li>
  <li>Addon planes are fully supported</li>
</ul>

Changelog:
<li>1.0

<li>1.1
-Fixed memory leak
-Added radio chatter audio
-Addon planes support
-Ability to toggle jet audio via .ini
-Ability to toggle chatter audio via .ini

<li>1.2
-Fixed addon planes not despawning in some cases where they blow up/crash etc
-Fixed audio staying in memory in those same cases

<li>1.3
-Fixed only one jet shooting (strike is more precise now, no pun intended)
-Fixed jet spawns, no more explosions on spawn
-Fixed jet flight path, they fly more in sync now and avoid crashes better
-Jets stay for longer
-Audio reworked, better sound and perfectly in sync
-Added jet camera, press R to toggle (key configurable via .ini)
-Rate of fire tweaked

<li>1.3.1
-Remixed the sound, more bass and cleaned some noise
-Added bassy distant jet sound, sound doesn't cut off as abruptly
-Added ability to tweak jet spawn height

<li>1.3.2
-Fixed planes disappearing too early in some cases

<li>1.4
-Fixed camera toggle responsiveness
-Fixed planes exploding on spawn (again)
-Fixed planes not firing at the target (the only case they won't fire is when the flare stopped smoking, since the pilots can't see the target anymore they RTB)
-Fixed planes exploding from their own shots
-Fixed planes crashing into things
-Pilots use brakes and other surfaces (make turns), flight feels more natural
-Planes have turbulence now
-Planes now spawn smarter, and always head in the correct direction
-Fixed gear retracting/detracting mid flight
-Planes now use the height of the target as an offset (if you throw a flare on the top of Mount Chiliad, the planes will spawn at the height of the mountain plus the height you specified in the ini)
-The strike covers larger area now, the coverage area moves forward with the plane
-Added ability to tweak the radius of the strike via ini (if radius set to zero, the shots will draw a straight line)
-Added ability to toggle blips via ini

<li>1.4.1
-Planes change attitude to avoid obstacles
-Camera improvements
-Disabled debug text that I forgot

<li>1.5
-Fixes for strike not hitting the target
-Some improvements to the strike itself
-New strike mode
-More configurable options in .ini

<li>1.6
-Menu implemented, not everything is configurable via menu for now
-Strike improvements (a bit longer burst, faster fire rate, bigger coverage)

<li>2.0
-New targeting mode - you can switch between flare and the IR Spotlight marker in the menu. Different strikes, different spotlight colors
-New airstrike, EMP drone swarm. Blast disables all vehicles including planes and helis, and causes a short power surge in the city. <a href="https://youtu.be/CSU7i4wLFk0">Here it is in action</a> It is still WIP, feel free to post your ideas and suggestions
-Menu adjustments
-Added effects on radar when planes spawn/despawn
-Incase of incorrect model name in the ini you will see a warning message, instead of crashing
-All strikes slightly tweaked
-Script cleans memory on loading, disposes of the models if they exist. Feel free to restart midgame if needed, should be stable now
-Player receives flare/spotlight automatically when selecting targeting mode
-Added ability to change menu key
-Menu settings automatically saved
-Minimap is hidden when jet camera is active
-Various bug fixes

<li>2.1
-Improved drone swarm spawning
-Drones now rain down rockets
-EMP effect improved

<li>2.2
-Minor bug fixes

<li>2.3
-Fixed planes not shooting sometimes
-Fixed menu flickering while accessing it mid strike
-Added timeout counter in the menu
-Menu cosmetic tweaks
-Minor tweaks

<li>2.3.1
-Cars that were affected by the EMP can no longer be started
-You can now use "fire" button on your gamepad, to call an airstrike by IR Spotlight (menu is still not compatible with gamepad)

<li>2.3.2
-Possible fix for menu not opening for some people

<li>3.0
-Rewrote menu, now supports gamepad (needs confirmation from people with gamepads)
-Hopefully fixed menu not opening no matter what key is set in the .ini for some people
-Menu redesigned, new options in the menu
-Gatling gun strike is now much longer
-Some people said they preferred the old gatling gun sound (from MW2), some said they like the new sound better (from Insurgency Sandstorm). I mixed them together
-Drone swarm tweaks
-Fixed only one jet visible on jet camera, FOV tweaked
-Fixed planes with cartoonish colors
-Various minor tweaks

<li>3.1
-Fixed gatling gun audio not playing when radio chatter set to off
-Fixed planes not despawning when reloading script midstrike
-Fixed camera not resetting while jets despawning when jet camera is active
-Sound improved

<li>3.2
-Added ability to edit gamepad controls for gamepad users(need a confirmation that it works)
-Added ability to disable automatic HUD hiding while in jet camera, for compatibility with "hud on phone up" trainer option

<li>4.0
-Red tracers for the 30mm cannons
-All strike types reworked
-Missile barrage replaced with carpet bombing
-GAU-8 strike is much longer
-Jets start shooting earlier, now they fly overhead AFTER finishing the strike, which looks much better
-Audio tweaked to be more in sync
-Settings check on startup, invalid values will be reset and a message will be displayed
-Various tweaks and improvements

<li>4.1
-Stealth bomber rework (https://youtu.be/2fuzz25cxGs)
-Added the ability to configure the EMP of the Drone Swarm. 3 modes available:
1.Normal - All vehicle engines in range are disabled
2.Aircraft only - EMP affects planes and helis only
3.Disabled - No EMP, missiles only
-Improved precision of all strikes
-Fixed "height" displaying "0" at startup while set to 150
-Various minor tweaks

<li>5.0
-Compatibility with newest version
-Minor bug fixes

<li>5.0.0.1
-Major code cleanup
-Fixed jets not spawning
-Fixed jets spawning but not shooting
-Fixed annoying error message on launch
-Added an electric flash on EMP blast

Requirements:
<li>ASI Loader
<li>Script Hook V
<li>Community Script Hook V .NET 3.6.0
<li>LemonUI (included)
<li>NAudio (included)

Installation:
<li>Extract files in to the scripts folder
<li>Use PrecisionAirstrike.ini to configure

Please do not hesitate to comment if you have any features you would like to be added.

Source code:
<a href="https://github.com/RusLanParty/Airstrike">GitHub</a>
