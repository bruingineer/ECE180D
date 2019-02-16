## Overview
This code includes the functions to implement the Gesture task. GestureClinet does the publishing and recieving via MQTT. GestureGame is responsible for intiaizing and updating the text on the screen for a gesture task. GestureGame updates a player's status based on whether the player completed a task succesfully or not.

## Where the code came from
Most of the MQTT code came from the MQTT APIs. By sending a message to the gesture recognition server running OpenPose, we can start and stop the gesture that the server looks for. 

## Decisions Made
The main decision that we made was to check establish the relationship between the GesturClient and the Openpose server. We decided to have the server look for the pose the clinet told it to look for. We choose to have an server running openpose, so that it could run on a host with a dedicated GPU in order to increase the frame rate for looking for gesture.

## Bugs
Currently there are no bugs to report. <br/>
In the past we need to debug the intial setup via MQTT. Once we figured that out it was simple to implement.

## Improvements
One of the improvement we want to make is to increase the number of gestures and poses that we can look for. Adding more gestures will make it a bit harder for the player as they will now have to know more gestures, thus increasing the probababilty of failing to succesful make a gesture. 
