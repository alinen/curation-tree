# curation-tree

Lightweight behavior tree API for creating puzzle and narrative games in Unity.
This repository demonstrates the framework we have been using to create
point-and-click games for teaching and research using Unity. For example,
the following demo implements a mechanic where dragging the casette onto to 
Boom Box plays music and makes the squirrel dance.



https://github.com/alinen/curation-tree/assets/259657/492408ef-3679-457d-a626-6475dc5b8a56



To run the demo, open the scene `SquirrelHouse.unity` in `BasicExample/Assets/Scenes`

This demo has been tested with Unity version 2022.3.16f1.

### How the system works

The basic workflow for this framework involves 

1. setting up the assets (images, sounds, animations) in the scene hierarchy
2. scripting the interactions between the game and the user with a script

For example, in the _Squirrel House_ demo, we can drag and drop the casette
onto the boom box to trigger music and the squirrel's dancing. Removing the
casette turns the music and dancing off. This is implemented with the following
the script. 

```
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

_IfDrag_ triggers whenever the player picks up the object. _IfDrop_ triggers
when the casette is dropped on the boom box. The names, _Casette_, _BoomBox_,
_Squirrel_, _SquirrelDance_, and _Music_, refer to assets in the scene. 
To execute the script, we create an
empty GameObject and attach a `GameLoop` component to it. The framework will 
connect behaviors automatically by matching the names from the script to the 
assets in the scene.

## Using the curation-tree in a new project

Please import the unity package [`CurationTree.unitypackage`](CurationTree.unitypackage) into your own project.
This package includes the sources under `BasicExample/Assets/Scripts/CurationTree`.

Please import the unity package [`CurationTree.unitypackage`](CurationTree.unitypackage) into your own project.
This package includes the sources under `BasicExample/Assets/Scripts/CurationTree`.

* [Overview and Examples](https://alinen.github.io/curation-tree/Docs/getting-started.html)
* [Debugging and troublshooting](https://alinen.github.io/curation-tree/Docs/debugging.html)
* [API Documentation](https://alinen.github.io/curation-tree/Docs/introduction.html)


## Citation

Aline Normoyle, Sophie Jörg, and Jennifer Hill. 2024. The Curation Tree:
A Lightweight Behavior Tree Framework for Implementing Puzzle and
Narrative Games. In _Proceedings of the 19th International Conference on the
Foundations of Digital Games (FDG 2024), May 21–24, 2024, Worcester, MA,
USA_. ACM, New York, NY, USA, 4 pages. https://doi.org/10.1145/3649921.
3659840
