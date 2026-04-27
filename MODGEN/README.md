# PVP Biome Dominions

PVP Biome Dominions is a mod that allows creating rules for:
- PVP control
- Position sharing management
- Ward Creation
- Tombstone management
- Automatic messages when logging in and out

based on the current biome of the player. You can apply different combat rules to Meadows, Black Forest, etc even for Ocean!

In addition, a list of the current players will be shown on the left side of the big map when you open it up with a summary of the current player statuses.

## Compatibility

This mod has been created with compatibility for the next mods:

* WardIsLove: thor wards will follow the rule configured in the wards options
* Marketplace: rules are skipped while in the territory of the server marketplace
* EpicMMO: player levels are visible in the map players list
* Groups: player icons are colored and shown at the top in the map players list
* Guilds: player guild icons are shown at the end of each row in the map players list
* Expand World: you can use custom biome names created by Expand World mods

## Usage instructions

Generate the configuration file "Turbero.PvPBiomeDominions.cfg" that will be created once the mod is executed for first time. Then, edit the file or use a ConfigurationManager mod to apply the rules to each biome.

### -- PvP Settings Configuration --

Here you can find each existing biome in the game to configure the pvp rule to apply:

![](https://i.imgur.com/m2ZaA6P.png)

* Pvp: Enforced that players can kill players
* Pve: Enforced that players cannot kill players
* Player Choose: Players decide if they can get killed by other players or not

On the other hand, Wards areas take priority and override the biome rule:

![](https://i.imgur.com/mfXT1j2.png)

![](https://i.imgur.com/JyYYBWs.png)

* Pvp: Enforced that players can kill players
* Pve: Enforced that players cannot kill players
* Player Choose: Players decide if they can get killed by other players or not
* Follow Biome Rule: players can kill players only if the biome rule allows it

Alternatively, admins can bypass these rules if the check is On.

![](https://i.imgur.com/QG8nDZE.png)

### -- Map Position Configuration --

Here you can find each existing biome in the game to configure if the player position should be revealed at all this or not in the in-game map/minimap.

![](https://i.imgur.com/5tYqIOE.png)

* Hide Player: Enforce hiding player position to everyone else
* Show Player: Enforce showing player position to everyone else
* Player Choice: player can choose either hiding or showing the map position to everyone else

On the other hand, Wards areas take priority and override the biome rule:

![](https://i.imgur.com/iFiGyED.png)

* Hide Player: Enforce hiding player position to everyone else
* Show Player: Enforce showing player position to everyone else
* Player Choice: player can choose either hiding or showing the map position to everyone else
* Follow Biome Rule: the position of the player is revealed or not according to the current biome rule

Alternatively, admins can bypass these rules if the check is On.

![](https://i.imgur.com/SZUbMu5.png)

### -- Ward Creation Configuration --

Here you can find each existing biome in the game to configure if a ward can be created

If you want to use a custom mod guard stone, add the ids in the field "wardModsPrefabIds" separated by comma ("Thorward" by default for WardIsLove mod)

### -- Tombstone management --

Here you can find options to configure which items will be recovered from tombstone after dying in each biome in PvE and PvP biomes.
The next options are available to toggle on/off:

* Show Message When Looting Your Tombstone
* PvE Biomes:
  * PvE - Allow Loot Other Tombstones
  * PvE - No Items Loss On Death
  * PvE - Keep Equipped Items On Death
  * PvE - Keep Hotbar Items On Death
  * PvE - Exception PrefabIds
  * Death Pin Map Rule in PvE. The behaviour can be:
    * Default
    * Remove When Looting Or Empty
    * No Pins
* PvP Biomes:
  * PvP - Allow Loot Other Tombstones
  * PvP - No Items Loss On Death
  * PvP - Keep Equipped Items On Death
  * PvP - Keep Hotbar Items On Death
  * PvP - Exception PrefabIds
  * Death Pin Map Rule in PvP. The behaviour can be:
    * Default
    * Remove When Looting Or Empty
    * No Pins

### -- Automatic messages when logging in and out --

Here you can find options to enable/disable the login message and additionally configure a logout message, in case you want to keep the privacy in your server world


### -- Full Configuration screenshots --

![](https://i.imgur.com/23EIXo1.png)

![](https://i.imgur.com/cv8extN.png)

![](https://i.imgur.com/gGeqBhB.png)

![](https://i.imgur.com/ATMDqJZ.png)

![](https://i.imgur.com/35wSQAb.png)

![](https://i.imgur.com/oplOZHW.png)

![](https://i.imgur.com/xNGipOF.png)

![](https://i.imgur.com/yVIC1Z1.png)

## About myself

DISCORD: Turbero (Turbero#3465)

All mods: https://thunderstore.io/c/valheim/p/turbero/

STEAM: https://steamcommunity.com/id/turbero

Ko-fi: https://ko-fi.com/turbero

For any concerns or doubts, please dm me or open tickets in my GitHub repos.

<a href="https://discord.gg/y67YeVw62K"><img src="https://i.imgur.com/A9b3EGB.png"></a>