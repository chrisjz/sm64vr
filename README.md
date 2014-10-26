# Super Mario 64 VR

Super Mario 64 partial re-make in VR using the Oculus Rift and Razer Hydra.

## Download

You can download the latest release executable here:

[Windows](http://assets.zookal.com/downloads/sm64vr/sm64vr.zip)

## Supported Devices

* __Oculus Rift DK1__
* __Oculus Rift DK2__
 * Positional tracking
 * Movement & jumping via positional tracking
* __Razer Hydra__
 * Hand tracking
 * Grab objects
 * Movement
* __Leap Motion__
 * Grounded and HMD mounted
 * Hand tracking
 * Grab objects

## Calibration

### Hydra

To calibrate a controller:
1. Point it to the base receiver.
2. Press __LT__/__RT__.
3. Place controller in front of respective shoulder and press __Start__.
4. Move controller forward and your virtual hand should appear in front of you.

## Controls

### Keyboard

* __W / Up Arrow__ - Forward
* __A / Left Arrow__ - Left
* __S / Down Arrow__ - Backward
* __D / Right Arrow__ - Right
* __Space__ - Jump
* __Esc__ - Menu
* __Ctrl + M__ - Trigger movement using DK2 positional tracking
* __Ctrl + J__ - Trigger jump using DK2 positional tracking
* __Shift + M__ - Swap between rotation and straffing on X axis when using DK2 positional tracking for movement

### Hydra

* __Left Joystick__ - Move
* __Right Joystick__ - Look
* __LT / RT__ - Grab object
* __RB__ - Jump
* __L3 / R4__ - Menu
* __Start__ - Freeze / Unfreeze hand

### Leap

* __Pinch__ - Grab object

### Rift DK2

The following controls apply in case where "Positional Tracking" is enabled for movement.

* __Lean Forward__ - Forward
* __Lean Left__ - Left
* __Lean Backward__ - Backward
* __Lean Right__ - Right
* __Poke Head Upward__ - Jump

## Release Notes

### 0.1.1

- Significant performance improvements, particularly on reducing stutter.

### 0.1

- Initial release.

## To do

### v0.1.x

- Mouse: Look up/down when Rift isn't present.
- Hydra: Slow down rotation with joystick.
- Mario scaling issue with doorways i.e. Mario can't enter through doors easily.
- Shortcut key to recenter camera for Rift. Shortcut can be Control + R.
- Leap: Hand controller doesn't track with the head as it moves around in the Rift.

### v0.2

* Co-op Multiplayer via LAN or online.

### Player

* Screen fades out when Mario loses life.
* Improve Mario's left hand model stretching when using Sixense tracking.
* Look script uses both X and Y axis while also rotating Mario's head
* Mario has arms when using leap motion.
* Better skeletal movement of arms when using hand tracking.
* Player can swim
* Mario animations for:
 * running
 * jumping
 * stomping
 * climbing tree/pole
 * swimming
* Mario sound effects for:
 * walking
 * running
 * jumping
 * double and triple jumping
 * stomping
 * climbing tree/pole
 * swimming
* Mario's hat bobs as player shakes their head.
* Full body tracking (PrioVR, SixenseVR).

### Princess Peach's Castle

* Fix transparency on textures for Princess Peach's Castle area.
* Toad inside first floor.
* Animate waterfall.
* Birds & butterflies animated outside castle.
* Sounds outside castle including birds, waterfall.

### Bob-omb Battlefield

* Enemies release coins on first spawn.
* Increase frequency of boulders coming down the mountain.
* Breaking certain small and big blocks releases coins.
* Goomba sometimes disappears likely due to occlusion setup.
* Transition to Bob-omb Battlefield when jumping into its painting.
* Add rotation to lifts when they reach top and bottom of cliff.
* Replace goomba's jumping sound with correct one.
* Replace goomba's running sound with correct one.
* Walking sound for King Bob-omb.
* Smoke effect when goomba disappears after being squished.
* Sound for squishing goomba.
* Goomba stops following player when player is past a certain distance away from goomba.
* Mario can open gate at ground of mountain by hitting switch.
* Chomp enemy.
* Heart near top of mountain.
* Certain cannons shoot bubbles.
* Mario can enter and be shot out of certain cannons.
* Teleportation points in group of flowers and cave along mountain.

### King Bob-omb

* Letting KB go gently at ground sometimes causes him to freeze without registering damage to KB. This is due to the collision occuring before player lets go of KB.
* Grabbing KB using Leap whilst being thrown causes KB to float away waving.
* KB doesn't return to spawn point if player throws KB and then player leaves the battleground perimeter.
* Level is completed when player grabs star with Leap hands, instead of only when colliding with it with hands.
* Player can grab KB too soon after he recovers from damage.
* Player should drop boss if he falls off ledge.
* Better movement of star to its final position.

### Menus

* Display stats on stars collected in level or world.
* Use Leap to navigate through menu.
* Use Sixense to navigate through menu.
* Use MYO to navigate through menu.
* Add title scene menu music from original game.

### Miscellaneous

* Mario spins continuously when he is atop certain moving objects, including: pink bob-omb.
* Shadows for signs, trees.
* Star model has a hole at its top tip.
* Replace current coin sound with original sound from game.

### Enhancements

* Minigames in World 1, such as throwing shells at goombas with timer.
* Luigi as a second player.
* Enhanced Multiplayer via LAN or online.

### Devices

* MYO Integration.

## Contributing

Please don't hesitate to send pull requests or suggest issues.

Note that managing changes to scenes can be tricky from a merging point of view, so please coordinate with me if you wish to do so.

## Licence

The MIT License (MIT).

See LICENSE file for more details.

## Credits

### Development

* [Chris Zaharia](http://github.com/chrisjz)

### 3D Models

* [Alec Pike](http://www.models-resource.com/submitter/alecpike/)
* [Chris Zaharia](http://github.com/chrisjz)
* Friedslick6
* [Jon Gonzalez](http://xenosmashgames.com/author/gonzosan/)
* MarioKart64n
* Maximo
* William Burke
 
### Sound & Music

* [KHInsider](http://http://www.khinsider.com)
* [Sound FX Center](http://soundfxcenter.com)
* [The Trasher](http://www.mfgg.net/index.php?act=user&param=01&uid=8)
* [YoungLink](http://www.mfgg.net/index.php?act=user&param=09&c=3&o=&uid=8164&st=0)

### Fonts

* [Mario Monsters](http://www.fontspace.com/mario-monsters)

## Disclaimer

Super Mario 64 is owned by Nintendo Co., Ltd. This project has no affiliation with Nintendo Co., Ltd.

This project is only a not-commercial tech demonstration on the implementation of virtual reality for the popular title.
