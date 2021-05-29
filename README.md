# Icarus Skins for Dyson Sphere Program

**DSP Icarus Skins** is a mod for the Unity game Dyson Sphere Program developed by Youthcat Studio and published by Gamera Game.  The game is available on [here](https://store.steampowered.com/app/1366540/Dyson_Sphere_Program/).

This mod makes it possible for you to create different skins for Icarus.  You can either select one of the pre-built skins, or provide a path to a 2048 x 2048 image png or jpg file with your own skin.

![Image of built-in skins, set 1](https://raw.githubusercontent.com/GreyHak/dsp-icarus-skins/master/BuiltInSkins.jpg)
![Image of built-in skins, set 2](https://raw.githubusercontent.com/GreyHak/dsp-icarus-skins/master/BuiltInSkins2.jpg)

If you like this mod, please click the thumbs up at the [top of the page](https://dsp.thunderstore.io/package/GreyHak/DSP_Icarus_Skins/) (next to the Total rating).  That would be a nice thank you for me, and help other people to find a mod you enjoy.

If you have issues with this mod, please report them on [GitHub](https://github.com/GreyHak/dsp-icarus-skins/issues).  I try to respond within 12 hours.    You can also contact me at GreyHak#2995 on the [DSP Modding](https://discord.gg/XxhyTNte) Discord #tech-support channel.

## Config Settings
Configuration settings or loaded when you game is loaded.  Settings will be reloaded when a game resumes after being paused.

This mod is also compatible with [BepInEx.ConfigurationManager](https://github.com/BepInEx/BepInEx.ConfigurationManager) which provides an in-game GUI for changing the settings in real-time.

![Config Settings Window image](https://raw.githubusercontent.com/GreyHak/dsp-icarus-skins/master/ConfigSettingsWindow.jpg)

The configuration file is called `greyhak.dysonsphereprogram.icarusskins.cfg`.  It is generated the first time you run the game with this mod installed.  On Windows 10 it is located at
 - If you installed manually:  `%PROGRAMFILES(X86)%\Steam\steamapps\common\Dyson Sphere Program\BepInEx\config\greyhak.dysonsphereprogram.icarusskins.cfg`
 - If you installed with r2modman:  `C:\Users\<username>\AppData\Roaming\r2modmanPlus-local\DysonSphereProgram\profiles\Default\BepInEx\config\greyhak.dysonsphereprogram.icarusskins.cfg`

## Installation
This mod uses the BepInEx mod patch framework.  So BepInEx must be installed to use this mod.  Find details for installing BepInEx [in their user guide](https://bepinex.github.io/bepinex_docs/master/articles/user_guide/installation/index.html#installing-bepinex-1).  This mod was tested with BepInEx x64 5.4.5.0 and Dyson Sphere Program 0.7.18.6931 on Windows 10.

To manually install this mod, add the `DSPIcarusSkins.dll` to your `%PROGRAMFILES(X86)%\Steam\steamapps\common\Dyson Sphere Program\BepInEx\plugins\` folder.

This mod can also be installed using ebkr's [r2modman](https://dsp.thunderstore.io/package/ebkr/r2modman/) mod manager by clicking "Install with Mod Manager" on the [DSP Modding](https://dsp.thunderstore.io/package/GreyHak/DSP_Icarus_Skins/) site.

## Open Source
The source code for this mod is available for download, review and forking on GitHub [here](https://github.com/GreyHak/dsp-icarus-skins) under the BSD 3 clause license.

## Change Log
### v1.0.5
 - Fixed non-critical error on startup.
 - Five more built-in skins from @ignite.
### v1.0.4
 - Added ArgumentException handler.
### v1.0.3
 - Fixed bug; added exception handling for sharing violations while reading in custom skin file.
 - One more built-in skin from @ignite.
 - Popup notification when the skin is loaded.  This is helpful when using the AutoReload setting.
 - Decreased DLL size further.
### v1.0.2
 - Fixed bug where skin Selection was set to 0 (zero) without a Path specified which was causing an exception.
### v1.0.1
 - Decrease DLL size from 30MB to 5MB.
 - Auto-reload updated skin.
 - Added support for jpg files.
 - Fixed a bug when changing Selection config using BepInEx.ConfigurationManager, switching to or from Selection 0 not taking effect immediately.
 - Added error log entry when loading of custom skin fails.
### v1.0.0
 - Initial release.
