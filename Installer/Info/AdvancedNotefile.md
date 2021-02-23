# Advanced Notefiles
With the new Notefile system, three varieties of "specifiers" were added:
* Variable Specifiers
* Chapter Labels
* Autoswitch/Listener specifiers

Each of these allow you to do much more with the overlay than just displaying plain texts

An example NoteFile (for Epison% comes with the P127 Launcher, but can also be found [here]())

## Variable Specifiers
The simplest variety of specifiers, these are simply replaced by the value of the "variable"

Special thanks to the community members who generated the autosplitter files, they greatly simplified the process of getting the requisite adresses for these stats/info.

### Currently, Project 1.27's note overlay supports 12 variable specifiers:

1. `$missions`: Shows the current number of main storyline missions completed,
2. `$sandf`: Shows the current number of Strangers and Freaks missions completed,
3. `$usj`: Shows the current number of succesfully completed unique stuntjumps,
4. `$bridges`: Shows the current number of bridges/knife flights (Alternates based on which was performed most recently)
5. `$randevs`: Shows the current number of completed random events
6. `$hobbies`: Shows the current number of hobbies/pastimes completed
7. `$cutscene`: Shows the script name for the current/upcoming cutscene
8. `$script`: Shows the currently loaded mission script
9. `$scriptPretty`: Shows the corresponding mission name for a given script (if available)
10. `$percent`: Shows the current game completion percentage
11. `$golfhole`: Shows the current golf hole
12. `$ctime`: Shows the current system time


## Chapter Labels

  `$:[Insert Name Here]$`

Marks the start of a new chapter in a text file; the end of the name can be indicated by either a single dollar sign, or a new line character. 

Note: a single comma following the closing dollar sign will be left out from the resulting text, this is to allow a zero-width seperator between a chapter label and an autoswitch specifier.

## Autoswitch Specifiers

For autoswitching to a chapter based on certain conditions, there are two different specifiers:

  `$@[variable]=[value]$` or `$@[variable]$`

In the case of the first variety, the chapter will be switched to when the variable is equal to the specified value; similar to the chapter labels the end of the value field can be indicated by either a single dollar sign or a newline character. In the second case, the chapter will be switched to any time the specified variable changes. 

In either case, these specifiers can be placed anywhere in a given chapter, and will autoswitch to the chapter when the requisite condition is met.
