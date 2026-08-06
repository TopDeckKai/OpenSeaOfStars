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
- [Unity Explorer 4.9.4](https://github.com/GrahamKracker/UnityExplorer)
- [Git](https://git-scm.com/downloads)

## Setup for Mods
Run Melon Loader and follow its instructions to install version 0.7.0 to Sea of Stars

If you are just wanting to play around with OpenSeaOfStars, wait for a release and then install the .dll from that release to the mods folder that Melon Loader created. You will then be able to explore the OpenSeaOfStars mod.
As for development, here is what you will need to do:

1. Run Sea of Stars once, until you reach the game menu, then exit. This will generate the needed dependencies.
2. Follow the instructions for Unity Explorer to install the mods and userlibs folders to your sea of stars location.
3. Download the OpenSeaOfStars project by cloning the repository in git bash or through visual studio.
4. Open the OpenSeaOfStars project in visual studio by opening the .sln if you haven't done so.
5. A lot of dependencies will be needed, do this by going to the "Dependencies" in solution explorer, right clicking, and selecting add project dependencies. There will be a tab to browse for dependencies to add.
![image](https://github.com/user-attachments/assets/8356ba7f-d956-4b9a-97b9-8965d7dce101)

Several dependencies will need to be added. These will be found under SeaOfStars/MelonLoader/Il2CppAssemblies and SeaOfStars/MelonLoader/net6. The list of dependencies needed from these folders should be found under .csproj

6. Once you have verified all dependencies are built, build your solution. Once that is built successfully, find your .dll under OpenSeaOfStars/OpenSeaOfStars/obj/debug/net6.0/OpenSeaOfStars.dll in your OpenSeaOfStars C# project. Copy that to your sea of stars mods folder.

7. Run Sea of Stars!

### Linux Setup
If you are wanting to help with development but are on a Linux machine, you can still help out!

1. You will need to make sure you have the [.Net 6 SDK](https://learn.microsoft.com/en-us/dotnet/core/install/linux?WT.mc_id=dotnet-35129-website) installed on your system. Since .Net 6 is an older version you might need to look into `ppa:dotnet/backports` to get it.
2. Run Sea of Stars once, until you reach the game menu, then exit. This will generate the needed dependencies.
3. Follow the instructions for Unity Explorer to install the mods and userlibs folders to your sea of stars location.
4. Download the OpenSeaOfStars project by cloning the repository.
5. Open the OpenSeaOfStars folder in your favorite text editor. VS Code or VS Codium work great.
6. A lot of dependencies will be needed. Once you have Melon Loader installed and you've ran the game once, you will have access to all of them. Go to the .csproj file and find and replace `D:\SteamLibrary` to your local Steam Library path. The rest should follow. It should be something similar to `\home\<username>\.steam\steam`. This is the default location on a Debian system but you can easily find it by right clicking on Sea of Stars in Steam, browsing local files, and copying everything before `/steamapps/`. Note the direction of the slash.
7. Once you have completed all this, you should be able to run `dotnet build` from the terminal in the OpenSeaOfStars folder and it should build successfully. Find your .dll under `OpenSeaOfStars/OpenSeaOfStars/obj/debug/net6.0/OpenSeaOfStars.dll` in your OpenSeaOfStars C# project. Copy that to your sea of stars mods folder.
8. Run Sea of Stars!

### About the .csproj
Don't commit these changes to the repo. There isn't a way to ignore changes on the remote side but you can do some untracking locally so it won't ever get committed if that would help you.

## Working with UnityExplorer
Unity Explorer is finicky with sea of stars. There are a few tips to keep in mind when working with the UnityExplorer mod:
1. Menus will freeze when opened while UnityExplorer is active. To avoid this, turn of UnityExplorer using F7, then turn it back on when in game.
    - If you happen to get stuck, you can hold left and right shift and press P to close UnityExplorer and go back to the title menu. This will unlock the menus.
2. Unity Explorer will be pixelated and not be legible at first. There are a few ways to resolve this: Either put your monitor to the lowest resolution, enable freecam when you wish to view the Object Explorer, or set up your UnityExplorer to be on a second monitor.
    - To set up UnityExplorer on a second monitor with Melon Loader follow these steps.
        1. Go to your Sea of Stars folder by browsing local files from Steam.
        2. Go to `UserData/MelonPreferences.cfg`
        3. Set `Target Display` to 1.
        4. Relaunch Sea of Stars and you should see UnityExplorer open in a different window.
