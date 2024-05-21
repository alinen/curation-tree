# curation-tree
Lightweight behavior tree API for creating puzzle and narrative games in Unity

## Demo

This repository demonstrates the framework we have been using to create point-and-click games for teaching and research using Unity. Here is a simple demo.

https://github.com/alinen/curation-tree/assets/259657/13519d60-7ed5-42c8-ab6a-9f373217a143

To run the demo, open the scene `BasicExample.unity`

This demo has been tested with Unity version 2022.3.16f1.

## How it works

The assets for the scene (images, sounds, animations) are set-up in the scene hierarchy. The interactions with the user are controled using a behavior tree script in text. For example, 

```
FadeAlpha: Message, 0, 1, 2.0
Hide: Message
Repeat:
  IfDrag: Casette
    Hide: Music
    StopAnimation: Squirrel, SquirrelDance
  End
  IfDrop: Casette, BoomBox
    Parallel:
      PlayAnimation: Squirrel, SquirrelDance
      Show: Music
    End
  End
End
```

To execute the above script, create an empty GameObject and attach a `GameLoop` component to it. The script references the assets in the scene by name.
