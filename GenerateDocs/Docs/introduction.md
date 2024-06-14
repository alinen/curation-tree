# Introduction

The Curation Tree framework implements basic features for drag-and-drop puzzle and 
narrative games. It consists of a main object, called `GameLoop`, that loads a script 
that specifies the mechanics of the game. These game mechanics are implemented as 
`Behavior`s that can be nested together in a tree. Using behaviors, we can play 
sounds, play animations, move objects, set text, and change colors. If we need 
additional behaviors than this basic set, we sub-class from 
Behavior to create more. Behaviors implement rules and logic on the assets that are 
defined in the Unity scene. Behaviors can access scene objects and response to player 
actions via a `World` object that stores all global state. 

In summary,

* The <xref href="CTree.GameLoop?alt=GameLoop"/> is the central class of this framework. 
* The <xref href="CTree.World?alt=World"/> object 
keeps track of the game's persistent state (such as player score), implements
user input handling, and handles the lookup of scene assets.
* <xref href="CTree.Behavior?alt=Behavior"/> is the base class for all game behaviors.

Selection and drag-and-drop are implemented using the <xref
href="CTree.Interactable?alt=Interactable"/> and <xref
href="CTree.Location?alt=Location"/> classes. Interactable objects can be clicked 
or dragged by the player. Location objects can have objects attached to them. 
Interactable and Location components are
automatically placed on scene assets when `IfClick` or `IfPickup/IfDrop`
queries are in the input script. Alternatively, you can explicitly configure objects 
for click and drag-and-drop with the script commands `InitLocation`, `InitDraggable`, and 
`InitClickable`.


In code, you can register callbacks on `Interactable` to respond to user events. See 
<xref href="CTree.HighlightBehavior?alt=HighlightBehavior"/> for an example. 

# Built-in Behaviors

The framework implements a suite of behaviors that we use across our projects. 
All built-in behaviors are defined in <xref href="CTree.Factory?alt=Factory"/>.
The framework makes it very easy to add new behaviors. 

## Simple custom behaviors

Most custom behaviors fall into one of two categories: _atomic behaviors_ that perform 
simple actions that can be completed within a single frame; and _coroutine behaviors_ that 
perform animations that can implemented using Coroutines. Both these types of 
behaviors can be created yb defining a new creator function in 
<xref href="CTree.Factory?alt=Factory"/>.  


### Atomic Behaviors

To create a new behavior that performs an actions that only requires a single
frame to execute, use <xref href="CTree.AtomicBehavior?alt=AtomicBehavior"/>. 
For example, suppose we want to create a Behavior that displays a line between 
two transforms using the following syntax. 

```
DrawLine: LineAsset, StartTransform, EndTransform
```

We can implement this in the Factory class using an AtomicBehavior.

```
public static Behavior DrawLine(World w, string args)
{
  // Parse the arguments
  string[] tokens = args.Split(",", 3); // Parses at most 3 items
  string assetName = tokens[0].Trim(); // Trim removes whitespace
  string startName = tokens[1].Trim();
  string endName = tokens[2].Trim();
  
  // Lookup objects in the scene
  Tranform asset = w.Get(assetName);
  Tranform start = w.Get(assetName);
  Tranform end = w.Get(assetName);
  LineRenderer renderer = asset.GetComponent<LineRenderer>();

  // NOTE: There's no error checking, so any errors will throw an
  // exception that you can look for in the Unity console.
  
  // Create behavior
  Behavior b = new AtomicBehavior(world, (w) => {
    renderer.gameObject.SetActive(true);
    renderer.positionCount = 2;
    renderer.SetPosition(0, start.position);
    renderer.SetPosition(1, end.position);
  });
  return b;
}
```


### Coroutine Behaviors

To create a behavior that consists of a Coroutine, first define the Coroutine function and
then add a creator function to Factory that instantiates a CoroutineBehavior.  For example, 
suppose we wish to animate a line between two transforms using the following syntax.

```
AnimateLine: LineAsset, StartTransform, EndTransform, Duration
```

We can implement this in the Factory class using a CoroutineBehavior.

```
public static IEnumerator AnimateExtendLine(
    Transform asset, Transform start, Transform end, float duration)
{
   LineRenderer renderer = asset.GetComponent<LineRenderer>();
   renderer.positionCount = 2;
   renderer.SetPosition(0, start.position);
   renderer.SetPosition(1, start.position);
   
   for (float t = 0; t < duration; t += Time.deltaTime)
   {
      float u = t / duration; // normalize between 0 and 1
      Vector3 endPos = Vector3.Lerp(start.position, end.position, u);
      renderer.SetPosition(1, endPos);
      yield return null;
   }
}

public static Behavior ExtendLine(World w, string args)
{
  // Parse the arguments
  string[] tokens = args.Split(",", 4); // Parses at most 3 items
  string assetName = tokens[0].Trim(); // Trim removes whitespace
  string startName = tokens[1].Trim();
  string endName = tokens[2].Trim();

  float duration = 0;
  float.TryParse(tokens[4], out duration);
  
  // Lookup objects in the scene
  Tranform asset = w.Get(assetName);
  Tranform start = w.Get(assetName);
  Tranform end = w.Get(assetName);

  // NOTE: There's no error checking, so any errors will throw an
  // exception that you can look for in the Unity console.
  
  // Create behavior
  Behavior b = new CoroutineBehavior(world, 
     AnimateExtendLine(asset, start, end, duration));
  return b;
}
```  

Coroutines cannot be anonymous functions. Our Coroutines for animation behaviors are implemented in <see cref="CTree.ProceduralAnimator?alt=ProceduralAnimator"/>.


## Subclassing Behavior

More complicated behaviors can be made by sub-classing from 
<xref href="CTree.Behavior?alt=Behavior"/> and then adding a creator function to 
<xref href="CTree.Factory?alt=Factory"/>. This is useful when it's simpler 
to write C# code than it is to write the script. For an example, 
see the block demos in [Getting Started](getting-started.md)

Overriding behavior requires two steps

1. Define a subclass of Behavior
2. Add a creator function to Factory


**Step 1: Override Behavior**

Your subclass should contian a constructor that takes 
World as one of its arguments. You should also override 
Setup() to initialize your behavior, TearDown() to cleanup 
your behavior, and Tick() for any per-frame behaviors. 
When the behavior is complete, set the flag *m_finished* to 
true. For example,

```
class MyClass : Behavior
{
   public MyClass(World w, Transform obj, float value) : base(w)
   {
      // your code here
   }

   public override void Setup()
   {
      base.Setup();
      // your code here
   }

   public override void TearDown()
   {
      base.TearDown();
      // your code here
   }

   public override void Tick()
   {
      base.Tick();
      // your code here
   }
}
```

*Important:* Make sure to set *m_finished* to true when the behavior 
is complete.

**Step 2: Add a creator function to Factory**

In <xref href="CTree.Factory?alt=Factory"/>, add a creator 
function that returns an instance of your class. The name 
of your creator function must match the Behavior name that you 
want to use in the script. Inside the creator, you can parse 
any additional arguments you want to send to your Behavior.
For example,

```
public static Behavior MyClass(World w, string args)
{
   // split into maximum of 2 arguments
   string[] tokens= args.Split(",", 2); 
   string token1 = tokens[0].Trim(); 
   string token2 = tokens[1].Trim();

   // Lookup object with name matching token1
   // Trim() is important for name lookup to work!
   Transform obj = w.Get(token1);
   Debug.Assert(obj != null);

   float value = 0.0;
   float.TryParse(token2, out value);
   return new MyClass(w, obj, value);
}
```


*Important:* Make sure to `Trim()` string arguments!
