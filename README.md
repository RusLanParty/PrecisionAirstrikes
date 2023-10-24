<p>Take control and unleash devastating aerial assaults by marking your targets with precision using flares or IR marking spotlights.</p>

<p>Features:</p>
<ul>
  <li>Convenient menu access with the DELETE key (configurable via .ini file)</li>
  <li>Choose your preferred targeting mode from the menu: flares or IR marking spotlights (left mouse click)</li>
  <li>Once marked, air support will arrive to attack the designated target</li>
  <li>Three strike types:
    <ol>
      <li>Two jets equipped with powerful 30mm GAU-8 cannons, delivering precise and lethal bursts of 280 rounds each.</li>
      <li>A stealth bomber that releases a line of explosives, perfect for clearing entire streets.</li>
      <li>Twenty drones, armed with 15 small missiles each and an EMP generator. The drones rain down missiles upon the target and then generate an electromagnetic pulse, disabling all vehicles, including aircraft, in the area. Ideal for countering helicopters.</li>
    </ol>
  </li>
  <li>Immersive sounds sourced from Insurgency Sandstorm, DCS, MW2, and remixed by me (with an option to add your own sounds)</li>
  <li>Engage the jet camera by pressing R (configurable via .ini file)</li>
  <li>Addon planes are fully supported for a personalized experience</li>
</ul>


Changelog:
<li>1.0

<li>1.1<br>
-Fixed memory leak<br>
-Added radio chatter audio<br>
-Addon planes support<br>
-Ability to toggle jet audio via .ini<br>
-Ability to toggle chatter audio via .ini<br>

<li>1.2<br>
-Fixed addon planes not despawning in some cases where they blow up/crash etc<br>
-Fixed audio staying in memory in those same cases<br>

<li>1.3<br>
-Fixed only one jet shooting (strike is more precise now, no pun intended)<br>
-Fixed jet spawns, no more explosions on spawn<br>
-Fixed jet flight path, they fly more in sync now and avoid crashes better<br>
-Jets stay for longer<br>
-Audio reworked, better sound and perfectly in sync<br>
-Added jet camera, press R to toggle (key configurable via .ini)<br>
-Rate of fire tweaked<br>

<li>1.3.1<br>
-Remixed the sound, more bass and cleaned some noise<br>
-Added bassy distant jet sound, sound doesn't cut off as abruptly<br>
-Added ability to tweak jet spawn height<br>

<li>1.3.2<br>
-Fixed planes disappearing too early in some cases<br>

<li>1.4<br>
-Fixed camera toggle responsiveness<br>
-Fixed planes exploding on spawn (again)<br>
-Fixed planes not firing at the target (the only case they won't fire is when the flare stopped smoking, since the pilots can't see the target anymore they RTB)<br>
-Fixed planes exploding from their own shots<br>
-Fixed planes crashing into things<br>
-Pilots use brakes and other surfaces (make turns), flight feels more natural<br>
-Planes have turbulence now<br>
-Planes now spawn smarter, and always head in the correct direction<br>
-Fixed gear retracting/detracting mid flight<br>
-Planes now use the height of the target as an offset (if you throw a flare on the top of Mount Chiliad, the planes will spawn at the height of the mountain plus the height you specified in the ini)<br>
-The strike covers larger area now, the coverage area moves forward with the plane<br>
-Added ability to tweak the radius of the strike via ini (if radius set to zero, the shots will draw a straight line)<br>
-Added ability to toggle blips via ini<br>

<li>1.4.1<br>
-Planes change attitude to avoid obstacles<br>
-Camera improvements<br>
-Disabled debug text that I forgot<br>

<li>1.5<br>
-Fixes for strike not hitting the target<br>
-Some improvements to the strike itself<br>
-New strike mode<br>
-More configurable options in .ini<br>

<li>1.6<br>
-Menu implemented, not everything is configurable via menu for now<br>
-Strike improvements (a bit longer burst, faster fire rate, bigger coverage)<br>

<li>2.0<br>
-New targeting mode - you can switch between flare and the IR Spotlight marker in the menu. Different strikes, different spotlight colors<br>
-New airstrike, EMP drone swarm. Blast disables all vehicles including planes and helis, and causes a short power surge in the city. <a href="https://youtu.be/CSU7i4wLFk0">Here it is in action</a> It is still WIP, feel free to post your ideas and suggestions<br>
-Menu adjustments<br>
-Added effects on radar when planes spawn/despawn<br>
-Incase of incorrect model name in the ini you will see a warning message, instead of crashing<br>
-All strikes slightly tweaked<br>
-Script cleans memory on loading, disposes of the models if they exist. Feel free to restart midgame if needed, should be stable now<br>
-Player receives flare/spotlight automatically when selecting targeting mode<br>
-Added ability to change menu key<br>
-Menu settings automatically saved<br>
-Minimap is hidden when jet camera is active<br>
-Various bug fixes<br>

<li>2.1<br>
-Improved drone swarm spawning<br>
-Drones now rain down rockets<br>
-EMP effect improved<br>

<li>2.2<br>
-Minor bug fixes<br>

<li>2.3<br>
-Fixed planes not shooting sometimes<br>
-Fixed menu flickering while accessing it mid strike<br>
-Added timeout counter in the menu<br>
-Menu cosmetic tweaks<br>
-Minor tweaks<br>

<li>2.3.1<br>
-Cars that were affected by the EMP can no longer be started<br>
-You can now use "fire" button on your gamepad, to call an airstrike by IR Spotlight (menu is still not compatible with gamepad)<br>

<li>2.3.2<br>
-Possible fix for menu not opening for some people<br>

<li>3.0<br>
-Rewrote menu, now supports gamepad (needs confirmation from people with gamepads)<br>
-Hopefully fixed menu not opening no matter what key is set in the .ini for some people<br>
-Menu redesigned, new options in the menu<br>
-Gatling gun strike is now much longer<br>
-Some people said they preferred the old gatling gun sound (from MW2), some said they like the new sound better (from Insurgency Sandstorm). I mixed them together<br>
-Drone swarm tweaks<br>
-Fixed only one jet visible on jet camera, FOV tweaked<br>
-Fixed planes with cartoonish colors<br>
-Various minor tweaks<br>

<li>3.1<br>
-Fixed gatling gun audio not playing when radio chatter set to off<br>
-Fixed planes not despawning when reloading script midstrike<br>
-Fixed camera not resetting while jets despawning when jet camera is active<br>
-Sound improved<br>

<li>3.2<br>
-Added ability to edit gamepad controls for gamepad users(need a confirmation that it works)<br>
-Added ability to disable automatic HUD hiding while in jet camera, for compatibility with "hud on phone up" trainer option<br>

<li>4.0<br>
-Red tracers for the 30mm cannons<br>
-All strike types reworked<br>
-Missile barrage replaced with carpet bombing<br>
-GAU-8 strike is much longer<br>
-Jets start shooting earlier, now they fly overhead AFTER finishing the strike, which looks much better<br>
-Audio tweaked to be more in sync<br>
-Settings check on startup, invalid values will be reset and a message will be displayed<br>
-Various tweaks and improvements<br>

<li>4.1<br>
-Stealth bomber rework (https://youtu.be/2fuzz25cxGs)<br>
-Added the ability to configure the EMP of the Drone Swarm. 3 modes available:<br>
1.Normal - All vehicle engines in range are disabled<br>
2.Aircraft only - EMP affects planes and helis only<br>
3.Disabled - No EMP, missiles only<br>
-Improved precision of all strikes<br>
-Fixed "height" displaying "0" at startup while set to 150<br>
-Various minor tweaks<br>

Requirements:
<li>ASI Loader
<li>Script Hook V
<li>Community Script Hook V .NET 3.5.1
<li>LemonUI (included)
<li>NAudio (included)

Installation:
<li>Extract files in to the scripts folder
<li>Use PrecisionAirstrike.ini to configure

Please do not hesitate to comment if you have any features you would like to be added.
