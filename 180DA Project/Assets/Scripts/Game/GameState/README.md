# Overview
The main job of this source code is to provide functionality for managing the game and deciding what happens once the player wins or loses.
Most of the game's parameters are included within this class. The main GameState class serves as the base class, and there are many
classes that derive from this class depending on the type of game needed. For example, the GameState with Player is used for the Main Level
and the Laser Level, because it requires a Player in order for the game to work correctly.
 
## Where code came from
All the code was through Unity functions.
 
## Decisions Made
The biggest decision made recently was creating a Laser base class and having the Small and Big Laser scripts derive from this class, in order to provide better code design.
 
## Bugs
As of now, there are no bugs seen within each of the GameState scripts.
 
## Future Improvements
As of now, the main future improvement will be to create a GameState to handle Multiplayer.
