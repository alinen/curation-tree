# Introduction

`GameLoop` is the central class of this framework. The GameLoop loads the text
script that stores the game behaviors and updates that `World` object that 
keeps track of the game's persistent state, such as player score, implements
user input handling, and handles the lookup of scene assets.

The framework implements a suite of behaviors that we use across our projects. 
Custom behaviors are also easily made by adding new creator functions to `Factory`. 
More complicated behaviors can be made by sub-classing from Behavior.

To see a listing of built-in Behaviors, read the documentation for `Factory`.

# Creating new behaviors