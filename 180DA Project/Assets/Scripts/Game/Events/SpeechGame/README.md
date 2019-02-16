## Overview
 This code included the source code needed to Implement the speech recogniton tasks of the game. Speech uses Unity's Dicatation Recognizer in order to recogznize speech input from the player. By use an instance of WordDisplay Speech keeps track of the word that is on display as well as making the word blink during harder levels. Speech chooses a word scrabmle or trivia task at random. It updates the player's status based on whether a task was completed succcesfully or failed. Speech selects a word or trivia question from SpeechList.

## Where the code came from
The code for how to use Dictation Recognizer came from the 
[Unity's documentation.](https://docs.unity3d.com/ScriptReference/Windows.Speech.DictationRecognizer.html)

## Decisons Made
The biggest improvement made from last quarter is the we are now using a Dictation Recognizer rather than a Phrase Recognizer. This allows use to display the word the player says, where as before the gamescreen would only show the word upon saying the correct word. Speech handles both the word scrable and triva in order to keep both tasks in the same class.

## Bugs
As of now there are no bugs. <br/>
In the past, we were using both a Phrase and Dictionation Recoginzer, which pasued the game due to the sharing and freeing of the same resources. 

## Future Improvements
One of the improvements we want to do is make seperate the trivia and word scrabmle task as a derived class of common tasks. <br/>
Additionally, we can add a hint mechanism to WordDisplay to aid the player by highlighting the first letter, when the player is consistenly bad at word scrabmles.