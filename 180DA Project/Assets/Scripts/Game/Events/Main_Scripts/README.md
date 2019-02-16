## Overview
These are implementations of the functions we wrote in order to load and start the events that the player needs to complete. PlayerEvents_MainGame handles all the events as they all appear in the Main Game. PlayerEvents_Gesture_Minigame and PlayerEvents_Speech_Minigame handle the player events for the mini game for training. 

## Where the code came from
Most of this code was written by our group. We got some of the API information from the unity documentation. 

## Design
We moved from constantly loading and destorying objects, to starting and stopping events. We moved the common functionality of some tasks into the Event class to simpliy implementaion and helping with adding new tasks.

##Improvements
We want to thorughly test our game to see if there are any changes that need to be made.