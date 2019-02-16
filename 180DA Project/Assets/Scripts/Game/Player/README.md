 # Overview
 This code includes all the code that gives our player functionality, as well as code that helps us keep track of information for our database. The main source code includes functionality for movement, and what happens when the player gets hit. Y movement is dictated mainly by a flag that is accessed whenever there are correct events. The X movement is dictated mainly by MQTT messages given from our localization process. Whenever the player gets hit, the number of lives decreases, updates the game UI by taking away a life from it, and the player alternates between red and light red, signifying a recovery time in which it cannot be hit. 
 
 # Where code came from
 Most of the code has come from Unity, but some code has also come from MQTT APIs. Most of the MQTT code has been used for the
 X and Y movement of the player. In terms of the X movement, the player receives MQTT messages from an OpenPose process dealing
 with the localization. In terms of the Y movement, when there is an event dealing with gestures, another OpenPose process
 sends MQTT messages whenever gestures are classified correctly.
 
 # Decisions Made
 The biggest decisions made were in regards to the movement of the player. One big decision that was made was to make sure that 
 only movement was going on at a time. If there was a simultaneous movement in the X and Y direction, the player would glitch out
 a little during its journey to its desired position. By only letting one direction move at a time, the animation became smoother.
 In the function that controls the X direction, there is also a sort of normalization just in case the camera doesn't pick
 up the player movement sequentially.
 
 # Bugs
 As of now, there are no bugs seen with our player scripts.
 
 # Future Improvements
 As of now, the biggest improvements would be to allow functionality for the player to have powerups whenever a certain amount
 of events have been done correctly, as well as punishments when they miss to many. 
