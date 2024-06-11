using UnityEngine;
using System;
using System.Reflection;

namespace CTree
{
  /// <summary>
  /// Implements the logic for converting lines of text into instances of 
  /// <see cref="CTree.Behavior?alt=Behavior"/>.
  /// </summary>
  /// <remarks>
  /// Implements the logic for converting lines of text into behaviors. 
  /// The set of built-in behaviors have the following syntax in the input file. 
  /// Each line of input corresponds to a single behavior, created using one of the 
  /// functions defined in this class. 
  ///  
  /// **Control Nodes**
  /// * <see cref="CTree.Factory.Sequence?alt=Sequence"/> 
  /// * <see cref="CTree.Factory.Parallel?alt=Parallel"/> 
  /// * <see cref="CTree.Factory.Select?alt=Select"/> 
  /// * <see cref="CTree.Factory.Repeat?alt=Repeat"/> 
  /// * <see cref="CTree.Factory.RepeatWhile?alt=RepeatWhile"/> 
  ///
  /// **Events**
  /// * <see cref="CTree.Factory.If?alt=If"/> 
  /// * <see cref="CTree.Factory.IfEnter?alt=IfEnter"/> 
  /// * <see cref="CTree.Factory.IfExit?alt=IfExit"/> 
  /// * <see cref="CTree.Factory.IfPickup?alt=IfPickup"/> 
  /// * <see cref="CTree.Factory.IfDrop?alt=IfDrop"/> 
  /// * <see cref="CTree.Factory.IfClick?alt=IfClick"/> 
  /// * <see cref="CTree.Factory.IfDragEnter?alt=IfDragEnter"/> 
  /// * <see cref="CTree.Factory.IfDragExit?alt=IfDragExit"/> 
  ///
  /// **State Nodes**
  /// * <see cref="CTree.Factory.SetState?alt=SetState"/>
  /// * <see cref="CTree.Factory.Add?alt=Add"/>
  ///
  /// **Appearance Nodes**
  /// * <see cref="CTree.Factory.Wait?alt=Wait"/>
  /// * <see cref="CTree.Factory.Show?alt=Show"/>
  /// * <see cref="CTree.Factory.Hide?alt=Hide"/>
  /// * <see cref="CTree.Factory.PlayAnimation?alt=PlayAnimation"/>
  /// * <see cref="CTree.Factory.StopAnimation?alt=StopAnimation"/>
  /// * <see cref="CTree.Factory.SetText?alt=SetText"/>
  /// * <see cref="CTree.Factory.Fade?alt=Fade"/>
  /// * <see cref="CTree.Factory.Grow?alt=Grow"/>
  /// * <see cref="CTree.Factory.ChangeColor?alt=ChangeColor"/>
  /// * <see cref="CTree.Factory.RevertColor?alt=RevertClor"/>
  /// * <see cref="CTree.Factory.Move?alt=Move"/>
  /// * <see cref="CTree.Factory.Pulse?alt=Pulse"/>
  /// </remarks>
  public static class Factory
    {
        /// <summary>
        /// Given world and input text, creates a behavior
        /// </summary>
        /// <remarks>
        /// The format for input is <c>FnName:Args</c>, where FnName corresponds to a static 
        /// function defined in Factory. Args is a string that contains any parameters 
        /// that is needed to initialize the behavior.
        /// </remarks>
        /// <param name="world">Object for accessing global state.</param>
        /// <param name="config">A string with format <c>FnName:Args</c></param>
        /// <returns>An instance of behavior</returns>
      public static Behavior Create(World world, string config)
      {
          string[] mc = config.Split(':', 2);
          string fnName = mc[0];
          try
          {
              Type thisType = typeof(Factory); 
              MethodInfo theMethod = thisType.GetMethod(fnName);
              Behavior beh = theMethod.Invoke(null, new object[]{world, mc[1]}) as Behavior;
              if (beh != null) beh.name = config; // save initializing command for debugging
              return beh;
          }
          catch(Exception e)
          {
              Debug.LogError("Cannot create behavior: "+config+" "+e.ToString());
          }
          return null;
      }

      #region Control Behaviors

      /// <summary>
      /// Creates a behavior that implements parallel control.
      /// </summary>
      /// <remarks>
      /// This behavior maintains a list of sub-behaviors that are executed simultaneously.
      /// The block of behaviors completes once all sub-behaviors complete. For example,
      /// <code>
      /// Parallel:
      ///    SubBehavior1
      ///    SubBehavior2
      /// End
      /// </code>
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="message">Unused</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior Parallel(World w, string message) 
      { 
          return new ParallelBehavior(w);
      }

      /// <summary>
      /// Creates a behavior that implements sequential control
      /// </summary>
      /// <remarks>
      /// This behavior maintains a list of sub-behaviors that are executed in sequence.
      /// The block of behaviors completes once all sub-behaviors complete. For example,
      /// <code>
      /// Sequence:
      ///    SubBehavior1
      ///    SubBehavior2
      /// End
      /// </code>
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="dummy">Unused</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior Sequence(World world, string dummy)
      {
          SequenceBehavior beh = new SequenceBehavior(world);
          return beh;
      }

      /// <summary>
      /// Creates a behavior that selects one behavior of a set to execute
      /// </summary>
      /// <remarks>
      /// This behavior maintains a list of sub-behaviors where the first sub-behavior to 
      /// return tree is executed. 
      /// <code>
      /// Select:
      ///    If: HasKey, 1
      ///       OpenCade
      ///    End
      ///    If: HasKey, 0
      ///       CloseDoor
      ///    End
      /// End
      /// </code>
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Unused</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior Select(World world, string args)
      {
          return new SelectBehavior(world);
      }

      /// <summary>
      /// Creates a behavior that repeats a set of behaviors 
      /// </summary>
      /// <remarks>
      /// This behavior maintains a list of sub-behaviors that run in parallel. For example,
      /// <code>
      /// Repeat:
      ///    SubBehavior1
      ///    SubBehavior2
      /// End
      /// </code>
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Unused</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior Repeat(World world, string args)
      {
          return new RepeatBehavior(world, (world) => { 
              return true; // run forever
          });
      }

      /// <summary>
      /// Creates a behavior that repeats behaviors for as long as the condition remains true
      /// </summary>
      /// <remarks>
      /// This behavior maintains a list of sub-behaviors that run in parallel. For example,
      /// <code>
      /// RepeatWhile: VariableName, 0
      ///    SubBehavior1
      ///    SubBehavior2
      ///    IfClick: AssetName
      ///       # Changes state so we exit and finish the behavior
      ///       SetState: VariableName, 1
      ///    End
      /// End
      /// </code>
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Unused</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior RepeatWhile(World world, string args)
      {
          string[] tokens = args.Split(','); 
          string stateName = tokens[0].Trim();
          int stateValue;
          int.TryParse(tokens[1].Trim(), out stateValue);  
          return new RepeatBehavior(world, (world) => { 
              return world.GetInteger(stateName) == stateValue;
          });
      }
      #endregion

      #region Events

      /// <summary>
      /// Execute a set of behaviors if the conditional is true
      /// </summary>
      /// <remarks>
      /// The behavior completes immediately if the conditional is false. If the condition is 
      /// true, the behavior completes when all sub-behaviors complete. All sub-behaviors run 
      /// in parallel.
      /// <code>
      /// If: Solved, 1
      ///   DoStuff
      /// End
      /// </code> 
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for specifying the conditional</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior If(World world, string args)
      {
          string[] tokens = args.Split(','); 
          string stateName = tokens[0].Trim();
          int stateValue;
          int.TryParse(tokens[1].Trim(), out stateValue);  
          return new IfBehavior(world, (world) => { 
              return world.GetInteger(stateName) == stateValue;
          });
      }

      /// <summary>
      /// Execute a set of behaviors if the player clicks on an interactable object
      /// </summary>
      /// <remarks>
      /// Executes all sub-behaviors in parallel if the user clicks the given scene object.
      /// Otherwise, the condition is false and the behavior completes immediately.
      /// <code>
      /// IfClick: AssetName
      ///    DoStuff
      /// End
      /// </code>
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for specifying the conditional</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior IfClick(World world, string args)
      {
          return new IfInteractableBehavior(world, 
              IfInteractableBehavior.Type.CLICK, args); 
      }

      /// <summary>
      /// Execute a set of behaviors if player drops an interactable on a specific location
      /// </summary>
      /// <remarks>
      /// Executes all sub-behaviors in parallel if the user drops the specified asset on 
      /// another asset. 
      /// Otherwise, the condition is false and the behavior completes immediately.
      /// <code>
      /// IfDrop: PickUpAsset, DropTarget
      ///    DoStuff
      /// End
      /// </code>
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for specifying the conditional</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior IfDrop(World world, string args)
      {
          string[] tokens = args.Split(',', 2);
          string src = tokens[0].Trim();
          string tgt = tokens[1].Trim();
          return new IfInteractableBehavior(world, 
              IfInteractableBehavior.Type.DROP, src, tgt); 
      }

      /// <summary>
      /// Execute a set of behaviors if the player hovers on top of an interactable object
      /// </summary>
      /// <remarks>
      /// Executes all sub-behaviors in parallel if the user hovers the mouse over the specified scene object.
      /// Otherwise, the condition is false and the behavior completes immediately.
      /// <code>
      /// IfEnter: AssetName
      ///    ChangeColor: AssetName, 1,1,0, 0.1
      /// End
      /// </code>
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for specifying the conditional</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior IfEnter(World world, string args)
      {
          return new IfInteractableBehavior(world, 
              IfInteractableBehavior.Type.ENTER, args); 
      }

      /// <summary>
      /// Execute a set of behaviors if the play stops hovering over an object
      /// </summary>
      /// <remarks>
      /// Executes all sub-behaviors in parallel if the user hovers the mouse over the specified scene object.
      /// Otherwise, the condition is false and the behavior completes immediately.
      /// <code>
      /// IfExit: AssetName
      ///    RevertColor: AssetName, 0.1
      /// End
      /// </code>
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for specifying the conditional</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior IfExit(World world, string args)
      {
          return new IfInteractableBehavior(world, 
              IfInteractableBehavior.Type.EXIT, args); 
      }

      /// <summary>
      /// Execute a set of behaviors if the player drags an interactable on to of a location
      /// </summary>
      /// <remarks>
      /// Executes all sub-behaviors (parallel) if the user drags one object on top of 
      /// a potential drop target
      /// Otherwise, the condition is false and the behavior completes immediately.
      /// <code>
      /// IfDragEnter: PickUpAsset, DropTarget
      ///    ChangeColor: DropTarget, 0, 0, 1, 2.0
      /// End
      /// </code>
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for specifying the conditional</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior IfDragEnter(World world, string args)
      {
          string[] tokens = args.Split(',', 2);
          string src = tokens[0].Trim();
          string tgt = tokens[1].Trim();
          return new IfInteractableBehavior(world, 
              IfInteractableBehavior.Type.DRAG_ENTER, src, tgt); 
      }

      /// <summary>
      /// Execute a set of behaviors if the player stops dragging an object on a location
      /// </summary>
      /// <remarks>
      /// Executes all sub-behaviors (parallel) if the user drags one object on top of 
      /// a potential drop target
      /// Otherwise, the condition is false and the behavior completes immediately.
      /// <code>
      /// IfDragExit: PickUpAsset, DropTarget
      ///    RevertColor: DropTarget, 0.1
      /// End
      /// </code>
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for specifying the conditional</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior IfDragExit(World world, string args)
      {
          string[] tokens = args.Split(',', 2);
          string src = tokens[0].Trim();
          string tgt = tokens[1].Trim();
          return new IfInteractableBehavior(world, 
              IfInteractableBehavior.Type.DRAG_EXIT, src, tgt); 
      }

      /// <summary>
      /// Execute a set of behaviors if the player picks up an object
      /// </summary>
      /// <remarks>
      /// Executes all sub-behaviors in parallel if the user picks up the give object. 
      /// Otherwise, the condition is false and the behavior completes immediately.
      /// <code>
      /// IfPickup: AssetName
      ///    DoStuff
      /// End
      /// </code>
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for specifying the conditional</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior IfPickup(World world, string args)
      {
          return new IfInteractableBehavior(world,
              IfInteractableBehavior.Type.PICKUP, args); 
      }

      #endregion

      #region Appearance Behaviors

      /// <summary>
      /// Creates a behavior that waits for a specified amount of time (e.g. pauses)
      /// </summary>
      /// <remarks>
      /// Implements the following input string
      /// <code>
      /// Wait: Duration
      /// </code>
      /// For example, <c>Wait: 1.0</c> pauses for one second. The behavior completes when the duration as passed.
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for the behavior</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior Wait(World world, string args)
      {
          return new CoroutineBehavior(world, args, (b, args) => {
             float duration = 1.0f;
             Single.TryParse(args, out duration);
             return ProceduralAnimator.Wait(duration);
          });
      }

      /// <summary>
      /// Creates a behavior that plays a sound
      /// </summary>
      /// <remarks>
      /// Implements the following input string
      /// <code>
      /// PlaySound: AssetName
      /// </code>
      /// For example, <c>PlaySound: MoveSound</c> plays sound on the asset MoveSound.
      /// MoveSound should have an AudioSource component on it. 
      /// The behavior completes when the Sound is complete (note: looped sounds play forever). 
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for the behavior</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior PlaySound(World w, string config) 
      { 
          string rootName = config.Trim();
          return new Sound(w, rootName, Sound.Mode.PLAY);
      }

      /// <summary>
      /// Creates a behavior that stops an animation.
      /// </summary>
      /// <remarks>
      /// Implements the following input string
      /// <code>
      /// StopSound: AssetName
      /// </code>
      /// For example, <c>StopSound: Music</c> stops the AudioSource component on 
      /// the asset with name <c>Music</c>.
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for the behavior</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior StopSound(World w, string config) 
      { 
          string rootName = config.Trim();
          return new Sound(w, rootName, Sound.Mode.STOP);
      }


      /// <summary>
      /// Creates a behavior that plays an animation
      /// </summary>
      /// <remarks>
      /// Implements the following input string
      /// <code>
      /// PlayAnimation: AssetName, AnimationName
      /// </code>
      /// For example, <c>PlayAnimation: Squirrel, Dance</c> plays the dance animation on the asset <c>Squirrel</c>.
      /// The behavior completes when the animation is complete. 
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for the behavior</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior PlayAnimation(World w, string config) 
      { 
          string[] tokens = config.Split(',', 2);
          string rootName = tokens[0].Trim();
          string aniName = tokens[1].Trim();
          return new Animation(w, rootName, aniName, Animation.Mode.PLAY);
      }

      /// <summary>
      /// Creates a behavior that stops an animation.
      /// </summary>
      /// <remarks>
      /// Implements the following input string
      /// <code>
      /// StopAnimation: AssetName, AnimationName
      /// </code>
      /// For example, <c>StopAnimation: Squirrel, Dance</c> stops the dance animation for <c>Squirrel</c>.
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for the behavior</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior StopAnimation(World w, string config) 
      { 
          string[] tokens = config.Split(',', 3);
          string rootName = tokens[0].Trim();
          string aniName = tokens[1].Trim();
          return new Animation(w, rootName, aniName, Animation.Mode.STOP);
      }

      /// <summary>
      /// Creates a behavior that enabled a GameObject
      /// </summary>
      /// <remarks>
      /// Implements the following input string
      /// <code>
      /// Show: AssetName
      /// </code>
      /// For example, <c>Show: Squirrel</c> enables the GameObjet with name Squirrel. 
      /// This will show the asset's geometry and activate any components on it. 
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for the behavior</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior Show(World world, string objName)
      {
          return new AtomicBehavior(world, (b) =>
          {
              //Debug.Log("SHOW: "+objName.Trim());
              Transform xform = b.Get(objName.Trim());
              xform.gameObject.SetActive(true);
          });
      }

      /// <summary>
      /// Creates a behavior that hides a GameObject.
      /// </summary>
      /// <remarks>
      /// Implements the following input string
      /// <code>
      /// Hide: AssetName
      /// </code>
      /// For example, <c>Hide: Squirrel</c> will hide the geometry associated with <c>Squirrel</c>
      /// and disable its associaed components. 
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for the behavior</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior Hide(World world, string objName)
      {
          return new AtomicBehavior(world, (b) =>
          {
              Transform xform = b.Get(objName.Trim());
              xform.gameObject.SetActive(false);
          });
      }

      /// <summary>
      /// Creates a behavior that changes the text on an object.
      /// </summary>
      /// <remarks>
      /// Implements the following input string
      /// <code>
      /// SetText: AssetName, NewText
      /// </code>
      /// For example, <c>SetText: UItext, Hello, World!</c> will set the contents of <c>UIText</c> to the 
      /// string "Hello, World!" The behavior completes in one frame.
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for the behavior</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior SetText(World world, string config)
      {
          string[] tokens = config.Split(',', 2);
          string rootName = tokens[0].Trim();
          string message = tokens[1].Trim();
          return new AtomicBehavior(world, (b) =>
          {
              Transform xform = b.Get(rootName);
              ProceduralAnimator.SetText(xform, message);
          });
      }

      /// <summary>
      /// Creates a behavior that reverts to an asset's original color.
      /// </summary>
      /// <remarks>
      /// Implements the following input string
      /// <code>
      /// RevertColor: AssetName, Duration
      /// </code>
      /// For example, <c>RevertColor: Star</c> will revert the color of the asset <c>Star</c> to 
      /// its original color, over the given duration. 
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for the behavior</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior RevertColor(World w, string config)
      {
          return new CoroutineBehavior(w, config, (b, config) => {
             string[] tokens = config.Split(',', 2);
             string rootName = tokens[0];
             float d = 1.0f;
             Single.TryParse(tokens[1], out d);
             Transform obj = b.Get(rootName.Trim());
             return ProceduralAnimator.RevertColor(obj, d);
          });
      }

      /// <summary>
      /// Creates a behavior that changes an asset's color.
      /// </summary>
      /// <remarks>
      /// Implements the following input string
      /// <code>
      /// ChangeColor: AssetName, R, G, B, Duration
      /// </code>
      /// For example, <c>ChangeColor: Star, 1, 0, 0, 2.0</c> will change the color of <c>Star</c> to 
      /// the color (1,0,0), or Red. Colors should be in the range 0 and 1. The duration indicates the 
      /// length of the transition. The first time <c>ChangeColor</c> is called, we cache the original 
      /// colors s we can revert them later.
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for the behavior</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior ChangeColor(World w, string config)
      {
          return new CoroutineBehavior(w, config, (beh, config) => {  
            string[] tokens = config.Split(',', 5);
            string rootName = tokens[0];
            float r = 1.0f;
            float g = 1.0f;
            float b = 1.0f;
            float d = 1.0f;
            Single.TryParse(tokens[1], out r);
            Single.TryParse(tokens[2], out g);
            Single.TryParse(tokens[3], out b);
            Single.TryParse(tokens[4], out d);
            Transform obj = beh.Get(rootName.Trim());
            return ProceduralAnimator.ChangeColor(obj, new Color(r,g,b), d);
          });
      }

      /// <summary>
      /// Creates a behavior that pulses the size of an asset.
      /// </summary>
      /// <remarks>
      /// Implements the following input string
      /// <code>
      /// Pulse: AssetName, NumTimes, PulseSpeed, PulseGrowth
      /// </code>
      /// For example, <c>Pulse: Star, 2, 0.4, 0.1</c> will pulse the asset <c>Star</c>
      /// twice. Each pulse will last 0.4 seconds and will increase the size by 10%. 
      /// The PulseSpeed and PulseGrowth are option. The default PulseSpeed is 0.4 seconds.
      /// The default PulseGrowth is 0.1, or 10%.
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for the behavior</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior Pulse(World world, string args)
      {
          return new CoroutineBehavior(world, args, (beh, args) => { 
              string[] tokens = args.Split(',', 4);
              string rootName = tokens[0];
              int num = 1;
              float timePerPulse = 0.4f;
              float pulseSize = 0.1f;
              int.TryParse(tokens[1], out num);
              if (tokens.Length > 3) Single.TryParse(tokens[2], out timePerPulse);
              if (tokens.Length > 4) Single.TryParse(tokens[3], out pulseSize);
              Transform obj = beh.Get(rootName.Trim());
              return ProceduralAnimator.Pulse(obj, num, timePerPulse, pulseSize);
          });
      }

      /// <summary>
      /// Creates a behavior that moves an asset (both position and rotation) between two waypoints. 
      /// </summary>
      /// <remarks>
      /// Implements the following input string
      /// <code>
      /// Move: AssetName, Waypoint1, Waypoint2, Duration, InterpolationType
      /// </code>
      /// For example, <c>Move: Star, Waypoint1, Waypoint2, Duration, Cosine</c> will animate 
      /// asset <c>Star</c> so it starts with the position and rotation of <c>Waypoint1</c> and 
      /// ends with the position and rotation of <c>Waypoint2</c>. The Duration is the length of 
      /// the transition in seconds. The <c>InterpolationType</c> is optional and specifies the shape of the curve 
      /// between the start and end points. Supported choices for <c>InterpolationType</c> are 
      /// "Linear", "Cosine", and "EaseIn". The default is "Linear". 
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for the behavior</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior Move(World w, string config) 
      { 
          return new CoroutineBehavior(w, config, (beh, config) => {
              string[] tokens = config.Split(',', 5);
              string rootName = tokens[0];
              string startName = tokens[1];
              string endName = tokens[2];
              float duration = 1.0f;
              Single.TryParse(tokens[3], out duration);
              Transform obj = beh.Get(rootName.Trim());
              Transform start = beh.Get(startName.Trim());
              Transform end = beh.Get(endName.Trim());

              ProceduralAnimator.Interpolator interpolator = ProceduralAnimator.Linear;
              if (tokens.Length > 4) 
              {
                  if (tokens[4].StartsWith("EaseIn")) interpolator = ProceduralAnimator.EaseIn;
                  else if (tokens[4].StartsWith("Cosine")) interpolator = ProceduralAnimator.Cosine;
              }
              return ProceduralAnimator.Move(obj, start, end, duration, interpolator);
          });
      }

      /// <summary>
      /// Creates a behavior that modifies the size of an asset
      /// </summary>
      /// <remarks>
      /// Implements the following input string
      /// <code>
      /// Grow: AssetName, StartSize, EndSize, Duration
      /// </code>
      /// For example, <c>Grow: Star, 1, 2, 4.0</c> will increase the size of the asset <c>Star</c> 
      /// from its original size to twice the size over 4.0 seconds. 
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for the behavior</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior Grow(World w, string config) 
      { 
          return new CoroutineBehavior(w, config, (b, config) => {
              string[] tokens = config.Split(',', 4);
              string rootName = tokens[0];
              float start = 1.0f;
              float end = 1.0f;
              float duration = 1.0f;
              Single.TryParse(tokens[1], out start);
              Single.TryParse(tokens[2], out end);
              Single.TryParse(tokens[3], out duration);
              Transform obj = b.Get(rootName.Trim());
              return ProceduralAnimator.Grow(obj, start, end, duration);
          });
      }

      /// <summary>
      /// Creates a behavior that changes the transparency of an asset
      /// </summary>
      /// <remarks>
      /// Implements the following input string
      /// <code>
      /// Fade: AssetName, StartAlpha, EndAlpha, Duration
      /// </code>
      /// For example, <c>Fade: Star, 0, 1, 2.0</c> will change the transparency of the asset 
      /// <c>Star</c> from 0 (invisible) to 1 (opaque) over 2 seconds. 
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for the behavior</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior Fade(World w, string config) 
      { 
          return new CoroutineBehavior(w, config, (b, config) => {
              string[] tokens = config.Split(',', 4);
              string rootName = tokens[0];
              float start = 1.0f;
              float end = 1.0f;
              float duration = 1.0f;
              Single.TryParse(tokens[1], out start);
              Single.TryParse(tokens[2], out end);
              Single.TryParse(tokens[3], out duration);
              Transform obj = b.Get(rootName.Trim());
              return ProceduralAnimator.Fade(obj, start, end, duration);
          });
      }
      #endregion

      #region World State Behaviors

      /// <summary>
      /// Creates a behavior that sets the value of a global variable
      /// </summary>
      /// <remarks>
      /// Implements the following input string
      /// <code>
      /// SetState: VariableName, VariableValue
      /// </code>
      /// For example, <c>SetState: Solved, 1</c> will set the global variable (stored in World) with 
      /// name <c>Solved</c> to 1.  
      /// The behavior completes in one frame.
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for the behavior</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior SetState(World world, string dummy)
      {
          return new AtomicBehavior(world, (b) =>
          {
              string[] tokens = dummy.Split(',', 2);
              string key = tokens[0].Trim();
              string value = tokens[1].Trim();

              int tmp = 0;
              if (int.TryParse(value, out tmp))
              {
                  b.world.SetInteger(key, tmp);
              }
              else
              {
                  b.world.SetString(key, value);
              }
          });
      }

      /// <summary>
      /// Creates a behavior that modifies a global integer variable
      /// </summary>
      /// <remarks>
      /// Implements the following input string
      /// <code>
      /// Add: VariableName, Delta
      /// </code>
      /// For example, <c>Add: StarCount, 1</c> will add one to the variable <c>StarCount</c>
      /// stored in World. The behavior completes in one frame.
      /// </remarks>
      /// <param name="world">Object for accessing global state.</param>
      /// <param name="args">Parameters for the behavior</param>
      /// <returns>An instance of behavior</returns>
      public static Behavior Add(World world, string args)
      {
          string[] tokens = args.Split(','); 
          string stateName = tokens[0].Trim();
          int stateValue;
          int.TryParse(tokens[1].Trim(), out stateValue);  
          return new AtomicBehavior(world, (b) => { 
              int v = b.world.GetInteger(stateName);
              v += stateValue;
              b.world.SetInteger(stateName, v);
          });
      }

      public static Behavior InitDraggable(World world, string args)
      {
          // Apply setting immediately
          string objName = args.Trim();
          Transform xform = world.Get(objName);
          world.AddDragable(xform);
          return null;
      }

      public static Behavior InitClickable(World world, string args)
      {
          // Apply setting immediately
          string objName = args.Trim();
          Transform xform = world.Get(objName);
          world.AddClickable(xform);
          return null;
      }

      public static Behavior InitLocation(World world, string args)
      {
          // Apply setting immediately
          string locName = args.Trim();
          Transform xform = world.Get(locName);
          world.AddLocation(xform);
          return null;
      }

      #endregion
  }
}
