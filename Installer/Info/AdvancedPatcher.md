# Advanced Patcher
With the new Patcher by @dr490n you can now binary patch the game at will, as well as enable / disable patches via a hotkey
Furthermore you can make P127 display the values of autosplit-style pointer paths via the Overlay 

You can access the Patcher Page by scrolling down to the very bottom of `Settings` -> `Extra Features`

An example for a Patch comes with P127 and can be loaded by clicking the last button on the Patcher Page (@Special Fors TaxiLongLoadFix)


### dr490n's Assembly Patcher
You can now test your patches/ideas on the fly with the Special Patcher integrated into Project 1.27, without the need of using external tools.
The menu is pretty simple, the only **required** are the `RVA` and the `patch itself in binary form`. 
Optinal parameters are a `Name`, a `Keybind`, and the information if the patch should be enabled by default.
On disabling a patch, the original ata is written back to the RVA.

Patches can be enabled/disabled on the go with the specific keybind assigned to them, and they can be enabled by default.
There is also a primary toggle for the patcher, which disables/enables the patcher itself on the fly. All hotkeys work while you are in-game.
Note: If your Overlay is set to Borderless, the keybinds will only work when the game is running AND in foreground.

###  PointerPath Tester

Project 1.27 can now fully interpret autosplit-style pointer paths, and display the resultant values to the overlay. You can access the feature inside the Special Patcher menu.
You can now view arbitrary memory and such directly in the NoteOverlay. 