### CHANGELOG

## 1.5.0

Added:
* GuildsAPI integration. Now guild icons of each player are shown at the end of each data player row.
* new option to configure a small delay between biomes when PvP status changes (to avoid abusing running to PvE area to be safe). Delay is zero by default (disabled). Adjust as you need.
* new option to give a pvp immunity buff for some minutes before being able to engage in pvp battle after dying (disabled by default). Damage done and received is zero while this buff is active.

Changed some configurations:
* Added translations for pvp immunity buff
* "Show Message When Looting Your Tombstone" moved to "2 - PvP Settings" category
* "Map Players List Refresh" moved to "1 - General"

Fixed:
* some console issues when showing some UI elements for first time

## 1.4.1

* Fixed log error when using "die" console command

## 1.4.0

Added:
* number of times a player has killed (green) to and has been killed (red) by each player connected in the server in the map players list
* new button to sort map players list by name

Fixed:
* Removed configuration for players panel size (it was useless) but you can still reposition it.
* Removed warnings when opening map

IMPORTANT: delete config file and let the mod recreate it to resize the panel properly (if you feel confident, change values manually after update)

## 1.3.2

* Fix for white squares in icons not showing sometimes the corresponding sprite in map window when not loaded correctly

## 1.3.1

* Fixed issue when opening map with players of the same name in the same world (there was a disabled feature for future interfering)

## 1.3.0

* Added new rules in config for wards placement in each biome to decide if it can be placed by the player (default = true)
* Included an extra option to consider different wards from other mod pieces by adding their prefabIds (initiated with value for WoL mod)

## 1.2.3

* Map players list position and size configuration changes apply immediately now
* Fixed issue showing the map players list if admins are exempt of position sharing
* Attempt to fix position sharing issue when player spawns into a world

## 1.2.2

* End of player name replaced by "..." if too long (configurable in .cfg)
* Added toggle to enable/disable the alerts sent to the tombstone owners
* Fixed issue in compendium to hide unnecessary custom entries

## 1.2.1

* Group members are now shown at the top of the map players list and then the rest of the connected players in the server
* Added protection against some exceptions until UI components are loaded

## 1.2.0

* Added to the map players list:
  * title header with connected players count
  * re-position from configuration file if playing with different screen sizes
  * epicMMO level (if installed)
* Added option to enable/disable the map player pins coloring according to their pvp status (enabled by default)
* Fixed tombstone alerts when the owner is far away from the player who opens the tombstone
* Fixed translation of the button to show/hide map players list

## 1.1.1

* Fix for Pinnacle mod player pins, they don't turn blue anymore.
* Added GroupsAPI integration.
* Fixed some colouring maps, but still some known issues to be resolved.

## 1.1.0

* New: added panel on the minimap with a list of connected players and their pve/pvp status. It can be hidden with a new button in the bottom left corner of the map.
* New: if WackyEpicMMOSystem mod from WackyMole is installed, set up the maximum difference in levels that players can fight between them in pvp areas.
  * Disabled by default
  * Default by 100. Use lower number in the configuration file to make it more effective.
  * Tip: If this is enabled and you still want to have friendly battles at someone's house, choose the option "PlayerChoose" in "PvP Rule In Wards (override biome)" to let the owner set up pvp and use wooden weapons!
* Changed minimap player pin colors: blue if they are in pve areas and red if they are in pvp areas (when their positions are visible)
* Added option to refresh config for sync issues when showing players position (default = 30 seconds)

## 1.0.2

* Added alerts to tombstone owner screen when the tomb is looted or destroyed by a different player. Leave blank in config file for not sending anything.
* Modified position sharing icon to be blue in pve areas and red in pvp areas.

## 1.0.1

Something went wrong with the packaging and the dll didn't want to load. Now it will. So sorry!!! Big apologies!

## 1.0.0

Initial version featuring PvP and Position Sharing management