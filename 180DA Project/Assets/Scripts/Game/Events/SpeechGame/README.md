## Speech.cs

* Handles the Speech Recognition event.
* Uses a the Dictation Recognizer provided by Unity
* Calls funtions to Display and Remove Text from the Gamescreen
* Initalizes a word scrabmle or trivia task, selected at random
* Updates Speech Metric
* Updates Player Position

## SpeechList.cs

* List of all the words for word scramble and the questions for trivia
* implements function to return a word or question

## WordDisplay.cs

* Maintains track the Text Gameobject on the gamescreen
* Displays and Removes text from the Game
* Implements a funtion to make text blink

## Problems Encountered

* Initally the word scrambler and trivia tasks were handled by seprate scripts. One of the scripts used a Phrase Recognizer and the other used a Dication Recognizer. We found that both use the same resources on the machine therefore, we need to stop the Phrase Recogizer every time we finished the task. After testing we found that the game pause for a small but significant amount of time. After debugging we found that using only one of the speeech recognizer tools the game flowed without delays. Because the Dictation Recognizer allows use to display every phrase that a player speak, therefore gives some feedback, we decieded to use the Dictation Recognizer. 
* This moduler design allows use to add an indefinite amount of words and trivia questions. Speech List only need to mange the Dication Recognizing and called function to an instance of WordDisplay to handle word display on the game screen.