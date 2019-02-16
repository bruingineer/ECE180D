# Overview 
Two folders are contained within this folder: Game Scripts and Lasers. There is also Obstacles.cs source code contained.

# Game Scripts
Located in Game Scripts is the source code necessary to generate the obstacles during the game.

# Lasers
Located in Lasers is the source code that deals with the lasers that can harm the player.

# Obstacle.cs

## Overview
The main job of this source code is to provide the framework to generate the obstacles. As of now, there are two obstacles: a Big Laser Obstacle and a Small Laser Obstacle. Its job is to start events for the game. 
 
# Where code came from
All the code was through Unity functions.
 
# Decisions Made
The biggest decision made recently was changing from GameObjects to using individual scripts. This helped a lot with object-oriented design of classes and less wasted tearing up/down of GameObjects. Now, components (scripts) are added to the main Game Manager Object, which is "creating" the class. 
 
# Bugs
As of now, there are no bugs seen with the Obstacles script. 
 
# Future Improvements
As of now, there are no future improvements.
