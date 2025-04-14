# THIS MOD IS EARLY IN DEVELOPMENT AND IS NOT READY FOR USE!

# Open Sea of Stars
The idea of this mod for Sea of Stars is to offer an open world save and experience, removing blocking cutscenes / scenes that don't make sense for open world, and working around the codebase to provide a clean base for a Randomizer (Be it standalone or Archipelago). 

## Save File Warning
As this is in development, we need to be cautious with save files. Before installing this mod, PLEASE back up the save files in the AppData\LocalLow\Sabotage Studio\Sea of Stars location on your PC.
If you have no save files, create save files in the first 3 slots of the game before loading the mod.

## Goals
The goals for this project include:
- Identifying Flags that need to be added to created save files.
- Identifying Cutscene Triggers that need to be deleted or modified for a cleaner experience. (Ex: Mid Level Cutscenes that can be accessed by going through a level backwards, cutscenes that move the player to a different location, etc.).
- Identifying Cutscenes that can be used but need character party modification (Ex: Boss Cutscenes).
- Providing a sample experience that will allow the player to unlock things such as the sea of stars.

## Requirements
To run this mod, you will need:
- Sea of Stars on Steam
- [Melon Loader](https://melonwiki.xyz/#/?id=requirements)
- [.NET 6.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

To develop for this mod, you will also need:
- [Visual Studio Community](https://visualstudio.microsoft.com/vs/community/)
- [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Unity Explorer](https://github.com/GrahamKracker/UnityExplorer)
- [Git](https://git-scm.com/downloads)

## Setup for Mods
Run Melon Loader and follow its instructions to install version 0.7.0 to Sea of Stars

If you are just wanting to play around with OpenSeaOfStars, wait for a release and then install the .dll from that release to the mods folder that Melon Loader created. You will then be able to explore the OpenSeaOfStars mod.
As for development, here is what you will need to do:

1. Follow the instructions for Unity Explorer to install the mods and userlibs folders to your sea of stars location.
2. Download the OpenSeaOfStars project by cloning the repository in git bash or through visual studio.
3. Open the OpenSeaOfStars project in visual studio by opening the .sln if you haven't done so.
4. A lot of dependencies will be needed, do this by going to the "Dependencies" in solution explorer, right clicking, and selecting add project dependencies. There will be a tab to browse for dependencies to add.
![image](https://github.com/user-attachments/assets/8356ba7f-d956-4b9a-97b9-8965d7dce101)

Several dependencies will need to be added. These will be found under SeaOfStars/MelonLoader/Il2CppAssemblies and SeaOfStars/MelonLoader/net6. The list of dependencies needed from these folders should be found under .csproj

5. Once you have verified all dependencies are built, build your solution. Once that is built successfully, find your .dll under OpenSeaOfStars/OpenSeaOfStars/obj/debug/net6.0/OpenSeaOfStars.dll in your OpenSeaOfStars C# project. Copy that to your sea of stars mods folder.

6. Run Sea of Stars!

## Working with UnityExplorer
Unity Explorer is finicky with sea of stars. There are a few tips to keep in mind when working with the UnityExplorer mod:
1. Menus will freeze when opened while UnityExplorer is active. To avoid this, turn of UnityExplorer using F7, then turn it back on when in game.
2. Unity Explorer will be pixelated and not be legible at first. There are a few ways to resolve this: Either put your monitor to the lowest resolution, enable freecam when you wish to view the Object Explorer, or set up your game to put Sea of Stars on a second monitor. (Instructions for second monitor setup later)
