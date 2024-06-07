# Introduction

<xref href="CTree.GameLoop?alt=GameLoop"/> 
is the central class of this framework. The `GameLoop` loads the text
script that stores the game behaviors and updates that 
<xref href="CTree.World?alt=World"/> object that 
keeps track of the game's persistent state, such as player score, implements
user input handling, and handles the lookup of scene assets.

The framework implements a suite of behaviors that we use across our projects. 
Custom behaviors are also easily made by adding new creator functions to 
<xref href="CTree.Factory?alt=Factory"/>. 
More complicated behaviors can be made by sub-classing from 
<xref href="CTree.Behavior?alt=Behavior"/>.

# Built-in Behaviors

All built-in behaviors are defined in <xref href="CTree.Factory?alt=Factory"/> 

# Creating new behaviors

