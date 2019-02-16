## GestureClient.cs

* Publishes and Recives Messages via MQTT for gesture recognition

## GestureGame.cs

* Initalzes all the Textobjects needed for the game
* Selects a gesture at random and initlizes corresponding text on the screen
* Removes text from gamescreen
* Recieves messages via MQTT when a player completes a task correctly
* Updates Gesture Metric 
* Updates player position

## Design 

* Although simple we found it more intuitve to keep the publisher of MQTT message on a seprate script so. This way Gesture Game only need to keep track of maintaing the game scren text and updating player status.