### CHANGELOG

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