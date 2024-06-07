# Introduction

<xref href="CTree.GameLoop?alt=GameLoop"/> 
is the central class of this framework. The `GameLoop` loads the text
script that configures the game behaviors. The `GameLoop` also updates the 
<xref href="CTree.World?alt=World"/> object that 
keeps track of the game's persistent state (such as player score), implements
user input handling, and handles the lookup of scene assets.

Selection and drag-and-drop are implemented using the <xref href="CTree.Interactable?alt=Interactable"> and <xref href="CTree.Location?alt=Location"> classes. These 
components are automatically placed on scene assets when `IfClick` or `IfDrag/IfDrop` 
queries are in the input script. 

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
<xref href="CTree.Factory?alt=Factory">. 



