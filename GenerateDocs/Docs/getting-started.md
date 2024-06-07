# Getting Started

In Unity, simply add a `GameLoop` component to one of the assets in your scene hierarchy.
The `GameLoop` takes a `TextAsset` as an argument. This input text should contain a sequence of behaviors that implements your game. The `GameLoop` also supports options for configuration and debugging:

* **interactableLayerMask**: Layer for selecting and dragging interactable objects. (Default Value: 8)
* DebugOptions
   * **tree**: Display the state of the behavior tree while the game is running 
   * **selection**: Display debug information for selecting and dragging objects.
* LogOptions
   * **enabled**: Toggle whether logging is enabled
   * **verbose**: Toggle whether log text should be printed to console as well as saved to file baseName: Set the filename for the log. The name format is "Application.persistentDataPath/baseName-timestamp.txt"

See the following tutorials for examples.

* [Squirrel House](#squirrel-house): 2D interaction, animation, and sound
* [Animate Demo](#animate-demo): 2D procedural animation 

## Squirrel House

 <video width="100%" controls>
  <source src='../images/SquirrelHouse.mp4' type="video/mp4">
  Your browser does not support the video tag.
</video> 

This demo shows how to trigger sounds and animation based on dragging and dropping objects.
From `BasicExample/Assets/Scene`, open `SquirrelHouse.unity`. 

This is a 2D demo. All assets are stored under the `HUD` object which is configured with a canvas object. 
The scene was created from the basic 3D scene. All game elements are stored under `HUD`.

* For music, we show and hide the object `Music` which is configured with an AudioSource component.
* For the dance animation, we add an animation component to `Squirrel` and `SquirrelDance` to it. We make sure to configure the animation so it does not play automatically.
* Drag and drop is configured using `Anchor` objects. The `Anchor` under the `BoomBox` asset indicates where the casette should snap when the user lets go of it. Otherwise, the casette will remain at the position where the user let go. The `Anchor` is offset slightly in the Z direction so that the colliders for the casette and BoomBox do not overlap (which would interfere with the raycast selection). 
* All other objects are simple images and text.

Here is the behavior script. 

```
Fade: Message, 0, 1, 2.0
Repeat:
  IfDrag: Casette
    Hide: Music
    StopAnimation: Squirrel, SquirrelDance
  End
  IfDrop: Casette, BoomBox
    PlayAnimation: Squirrel, SquirrelDance
    Show: Music
  End
End
```

The root node of the behavior tree is always a sequence node, meaning the following script runs the 
`Fade` behavior followed by the `Repeat` behavior. 

Notes:

* `Fade` changes the alpha, or transparency, for an asset. The syntax is `Fade: <AssetName>, <StartAlpha>, <EndAlpha>, <Duration>`
* `Repeat` repeats the IfDrag and IfDrop conditionals forever.
* `IfDrag` triggers if the user picks up the casette asset. When true, the two sub-behaviors are executed in parallel (e.g. simultaneously)
* `IfDrop` triggers if the user drops the casette on top of the BoomBox. When true, the two sub-behaviors are executed in parallel (e.g. simultaneously)
* `PlayAnimation` plays the `SquirrelDance` animation on the `Squirrel` asset.


## Animate Demo

 <video width="100%" controls>
  <source src='../images/AnimateDemo.mp4' type="video/mp4">
  Your browser does not support the video tag.
</video> 

The system includes several simple animation behaviors for moving, rotating, resizing, fading, and changing the colors of assets.
The configuration of the scene is very simple. We add empty game objects to serve as waypoints, but all other assets are simply images. 

Here is the behavior script. The root node of the behavior tree is always a sequence node, meaning the following script runs each line in 
sequence. 

```
# Test built-in animators
SetText: Message, Start Demo
Fade: Panel, 0, 1, 2.0
Grow: Star, 0.1, 1.0, 1.0
SetText: Message, Demo
ChangeColor: Star, 1,0,1, 1.0
ChangeColor: Star, 0,1,1, 1.0
RevertColor: Star, 2.0
Move: Star, Waypoint1, Waypoint2, 2.0
Move: Star, Waypoint2, Waypoint3, 2.0, Cosine
Move: Star, Waypoint3, Waypoint1, 3.0, EaseIn
Pulse: Star, 2
SetText: Message, Demo Complete
Fade: Panel, 1, 0, 2.0
```

Notes:

* To move and rotate objects, set waypoints (e.g. empty game objects) with the desired starting and end states. Both the position and rotations of the waypoints will be used for animation.
* Colors should be RGB coordinates, each component in the range 0 and 1.
* `SetText` changes the text on the `Message` asset.
* `Fade` changes the alpha, or transparency, for an asset. The syntax is `Fade: <AssetName>, <StartAlpha>, <EndAlpha>, <Duration>`
* `Grow` changes the size uniformly for an asset. The syntax is `Grow: <AssetName>, <StartSize>, <EndSize>, <Duration>`
* `ChangeColor` changes the color for an asset and all its children. The syntax is `ChangeColor: <AssetName>, <R>, <G>, <B>, <Duration>`. The original colors of the asset are cached so they can be restored later.
* `RevertColor` restores the color of an asset. The syntax is `RevertColor: <AssetName>, <Duration>`
* `Move` translates and rotates an asset. The syntax is `Move: <AssetName>, <StartTransform>, <EndTransform>, <Duration>, <Interpolation Type>`. The interpolation type is optional. By default, we use linear interpolation. 
* `Pulse` quickly pulses the size of the asset. The syntax is `Pulse: <AssetName>, <NumPulses>`. It is also possible to set the speed and size of the pulse by passing these arguments as additional values, e.g. `Pulse: <AssetName>, <NumPulses>, <PulseSpeed>, <PulseSize>`. The default pulse speed is 0.4 seconds. The pulse size is a percentage of the original size and is currently 0.1, or ten percent.

