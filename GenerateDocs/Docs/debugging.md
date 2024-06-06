
# Debuggging and Troubleshooting

The GameLoop component has options for debugging the execution of the behavior tree and 
for selection and raycasting. 

## Behavior Tree

In the `GameLoop` component, toggle Option -> Debug -> Tree to show the current state of the behavior tree. 

![SampleOutput](images/DebugTree.png)

Currently running behaviors are prefixed with `*`. Finished behaviors are suffixed with `(f)`. 
Nested behaviors are shown with indentation.  The pause button in the Unity viewer can be used 
to step through each behavior.

### Typical mistakes

* A behavior could not be initialized because it is mispelled or has incorrect parameters.
* Multiple assets with the same name. In this case, the behavior may modify an asset that you are not expecting.
* The framework only supports TextMeshPro.
* If you want to change the transparency of an object, make sure it has a transparent texture.
* Behaviors that change color assume that materials have a _Color attribute.

Check the console for errors also. 

## Raycasting

In the `GameLoop` component, toggle Option -> Debug -> Selection to show additional information about selection.
This will print the names of selected, dragged, clicked, or dropped objects. In the scene view, you will also see 
raycast lines from the camera. 

### Typical mistakes

* Incorrect collliders. If the collider for an object is wrong, the raycast and collision object will not work. Check the collider volumes while the game is running. 
* Comflicting layer mask. The selection system puts all interactable objects onto their own layer for more efficient collision testing. The default is layer 7. If this value conflicts with other layers in your game, you can change it in the `GameLoop` component, under Options -> InteractableLayerMask.
* Canvas configuration is incompatible. For 1D games, the Canvas component of your UI object should have render mode `ScreenSpace - Camera`.
* Two overlapping objects have similar sized colliders on them. In this case, the raycast won't detect one of them.