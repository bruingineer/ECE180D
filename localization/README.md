#Localization

# Overview
Our localization script uses image processing to determine an x-coordinate corresponding to the user’s position in the frame. This x-coordinate is then sent to our Unity application via an MQTT so that the game avatar can mirror the user's movement.

2 Options implemented
...1. Face recognition using Haar Cascade classifier
...2. Object tracking (color based)
 
## Where code came from
OpenSource code used:
...1. OpenCV
...2. MQTT
 
## Decisions Made
Currently, localization is being handled by our OpenPose process.
 
## Bugs
No major bugs, but the color-tracking implementation could be more consistent. We still get false positives that can affect gameplay.
 
## Future Improvements
Reduce false positives in our object tracking to make it more reliable/consistent. If this can be accomplished, we will return to OpenCV for localization, since it's much faster than OpenPose.
