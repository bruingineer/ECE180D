
## Iterative Learning 

StatsProcess.cs script is attached to an empty object in the game's "Game End" scene. 

# Overview
This script is executed every time a player completes a game. Its purpose is to implement the iterative learning aspect of our video game. It calculates game statistics and updates the database. It then performs two seperate queries of the current player's game data. One is for difficulty change eligibility, and the other for a training suggestion to the user.
 
## Where code came from
For this script we utilized Unity's MQTT API in order to communicate with our database wrapper (db_wrapper.py)
 
## Decisions Made
Our iterative learning model has evolved since last quarter. Initially, we only provided the user training suggestions based on his or her average performance in our 3 skill categories(laser dodging, gestures, speech) in comparison to a threshold. Now, we have implemented a FSM with three difficulty states (easy-hard). The user can unlock difficulties or be sent back to a lower difficulty depending on his or her performance.
 
## Future Improvements
We are adding some features to our training model in order to improve its effectiveness. We will begin collecting lives left, gesture time average, and speech time average. This script will adapt to considere these in evaluation of player's performance.
