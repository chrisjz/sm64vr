# Super Mario 64 VR

Super Mario 64 partial re-make in VR using the Oculus Rift and Razer Hydra.

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
 * Bottom-up tracking only
 * Hand tracking
 * Grab some objects

## Calibration

### Hydra

To calibrate a controller:
1. Point it to the base receiver.
2. Press __LT__.
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

## To do

### v0.1

* Display stats on coins collected in level.
* Title page from 3rd person transitioned to 1st person perspective.
* Player respawns in front of painting from first level.
* Coins added to first level and increase hit point by one when collected.
* KB starts shifting around randomly instead of following player sometimes. Seems to occur at least after he throws player.
* Big block collision on punch is no working completely.
* Convert Sixense GUI dialogs into 3D.
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

* Increase frequency of boulders coming down the mountain.
* If one goomba is knocked back into a second goomba, the second goomba will be knocked back too.
* Transition to Bob-omb Battlefield when jumping into its painting.
* Add rotation to lifts when they reach top and bottom of cliff.
* Replace goomba's jumping sound with correct one.
* Replace goomba's running sound with correct one.
* Walking sound for King Bob-omb.
* Smoke effect when goomba disappears after being squished.
* Sound for squishing goomba.
* Goomba stops following player when player is past a certain distance away from goomba.
* Chomp enemy.

### King Bob-omb

* Player drops boss if he falls off ledge.
* Better movement of star to its final position.

### Menus

* Display stats on stars collected in level or world.
* Add title scene menu music from original game.

### Miscellaneous

* Shadows for signs, trees.

### Enhancements

* Minigames in World 1, such as throwing shells at goombas with timer.
* Luigi as a second player.
* Enhanced Multiplayer via LAN or online.

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
* [The Trasher](http://www.mfgg.net/index.php?act=user&param=01&uid=8)

### Fonts

* [Mario Monsters](http://www.fontspace.com/mario-monsters)

## Disclaimer

Super Mario 64 is owned by Nintendo Co., Ltd. This project has no affiliation with Nintendo Co., Ltd.

This project is only a not-commercial tech demonstration on the implementation of virtual reality for the popular title.
