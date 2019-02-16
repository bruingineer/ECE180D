## Overview
This is the base classes for the different events that we have. This base starts and event through StartEvent function. It keeps track of the difficulty and the time remaining for the task. As a base class it contains various abstracted functions that the dervived classes implement differently.

## Where the code came from
Much of code was code that was common among our speech and gesture tasks. 

## Decisions Made
At the end of last quarter we noticed that our speech and gesture classes were using very similay code, therefore was best to creeate a base class of the common functions. Additionlly, rather than creating a destorying gameobjects we wanted to start and stop events. Now our game is much more event based. And we can implement more events more easily.

## Bugs
Currenly there are no bugs to report.

## Improvements
After the addition of the Trivia task new noticed even more common functionality. Therefore, we will either move more function into this base class. More likely we will create a bass class for the two speech tasks which will be derivied from Event.