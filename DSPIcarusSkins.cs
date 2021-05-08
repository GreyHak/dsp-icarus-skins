//
// Copyright (c) 2021, Aaron Shumate
// All rights reserved.
//
// This source code is licensed under the BSD-style license found in the
// LICENSE.txt file in the root directory of this source tree. 
//
// Dyson Sphere Program is developed by Youthcat Studio and published by Gamera Game.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.IO;

namespace DSPIcarusSkins
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    [BepInProcess("DSPGAME.exe")]
    public class DSPIcarusSkins : BaseUnityPlugin
    {
        public const string pluginGuid = "greyhak.dysonsphereprogram.icarusskins";
        public const string pluginName = "DSP Icarus Skins";
        public const string pluginVersion = "1.0.4";
        new internal static ManualLogSource Logger;
        new internal static BepInEx.Configuration.ConfigFile Config;
        Harmony harmony;

        public static BepInEx.Configuration.ConfigEntry<UInt16> configSkinSelection;
        public static BepInEx.Configuration.ConfigEntry<string> configSkinPath;
        public static BepInEx.Configuration.ConfigEntry<bool> configAutoReload;

        public static readonly string[] resourceNames = {
                "",  // Custom skin
                "DSPIcarusSkins.Built_In_Skins.camoDark.jpg",
                "DSPIcarusSkins.Built_In_Skins.camoLight.jpg",
                "DSPIcarusSkins.Built_In_Skins.camoRWB.jpg",
                "DSPIcarusSkins.Built_In_Skins.blue.jpg",
                "DSPIcarusSkins.Built_In_Skins.color.jpg",
                "DSPIcarusSkins.Built_In_Skins.bluegold.jpg",
                "DSPIcarusSkins.Built_In_Skins.redgold.jpg",
                "DSPIcarusSkins.Built_In_Skins.ignite.jpg",
            };

        public static UInt16 loadedSkinSelection = UInt16.MaxValue;
        public static string loadedSkinPath = "";
        public static DateTime loadedSkinFileModificationTime = DateTime.Now;

        public void Awake()
        {
            Logger = base.Logger;  // "C:\Program Files (x86)\Steam\steamapps\common\Dyson Sphere Program\BepInEx\LogOutput.log"
            Config = base.Config;  // "C:\Program Files (x86)\Steam\steamapps\common\Dyson Sphere Program\BepInEx\config\"

            harmony = new Harmony(pluginGuid);
            harmony.PatchAll(typeof(DSPIcarusSkins));

            /*string[] resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            Logger.LogDebug("Resources:");
            foreach (string resourceName in resourceNames)
            {
                Logger.LogDebug($"Resource Name \"{resourceName}\".");
            }*/

            configSkinSelection = Config.Bind<UInt16>("Skin", "Selection", 2, new BepInEx.Configuration.ConfigDescription("0:Path provided with custom skin;  1:Dark Camo;  2:Light Camo;  3:Red/White/Blue Camo;  4:Blue;  5:Color;  6:Blue/gold;  7:Red/gold;  8:Blue/Red", new BepInEx.Configuration.AcceptableValueRange<UInt16>(0, 8)));
            configSkinPath = Config.Bind<string>("Skin", "Path", "", "Path to the 2048 x 2048 skin image.  This setting is only used if Selection is set to 0.");
            configAutoReload = Config.Bind<bool>("Skin", "AutoReload", false, "Continually monitor skin file timestamp and automatically reload skin when the file changes.  This is useful while you are creating a skin.  It is recommended that this setting be disabled during normal gameplay.  This setting is only used if Selection is set to 0.");
            Config.ConfigReloaded += OnConfigChanged;
            Config.SettingChanged += OnConfigChanged;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerAnimator), "Start")]
        public static void PlayerAnimator_Start_Postfix()
        {
            loadedSkinSelection = UInt16.MaxValue;
            loadedSkinPath = "";
            loadedSkinFileModificationTime = DateTime.Now;

            Config.Reload();
            OnConfigChanged(null, null);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(GameMain), "Resume")]
        public static void GameMain_Resume_Postfix()
        {
            Config.Reload();
            OnConfigChanged(null, null);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Player), "GameTick")]
        public static void Player_GameTick_Postfix(long time)
        {   // time is increased by 1 every tick.  Called at 60 Hz.
            // GameMain.mainPlayer is always valid
            if (configSkinSelection.Value == 0 && configAutoReload.Value && ((time % 45) == 0))
            {
                try
                {
                    if (configSkinPath.Value.Length > 0 && File.GetLastWriteTime(configSkinPath.Value) != loadedSkinFileModificationTime)
                    {
                        OnConfigChanged(null, null);
                    }
                }
                catch (ArgumentException argException)
                {
                    Logger.LogWarning($"WARNING: ArgumentException while checking write time \"{configSkinPath.Value}\": {argException.Message}");
                }
            }
        }

        public static void OnConfigChanged(object sender, EventArgs e)
        {
            if (GameMain.mainPlayer == null || GameMain.mainPlayer.animator == null)
            {
                return;
            }
            bool icarusArmorFlag = false;
            bool icarusSkeletonFlag = false;
            Renderer[] componentsInChildren = GameMain.mainPlayer.animator.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in componentsInChildren)
            {
                var mat = renderer.sharedMaterial;
                if (mat != null)
                {
                    if (!icarusArmorFlag && mat.name.StartsWith("icarus-armor"))
                    {
                        bool loadFlag = false;
                        Texture2D icarusArmorTextureFile = new Texture2D(2048, 2048);
                        if (configSkinSelection.Value == 0 || configSkinSelection.Value >= resourceNames.Length)
                        {
                            string icarusArmorFilePath = configSkinPath.Value;
                            try
                            {
                                if (icarusArmorFilePath.Length > 0 &&
                                    (configSkinSelection.Value != loadedSkinSelection ||
                                    icarusArmorFilePath != loadedSkinPath ||
                                    File.GetLastWriteTime(icarusArmorFilePath) != loadedSkinFileModificationTime))
                                {
                                    if (System.IO.File.Exists(icarusArmorFilePath))
                                    {
                                        try
                                        {
                                            byte[] fileData = System.IO.File.ReadAllBytes(icarusArmorFilePath);
                                            if (icarusArmorTextureFile.LoadImage(fileData))
                                            {
                                                Logger.LogInfo($"Successfully loaded custom icarus armour skin {icarusArmorFilePath}");
                                                loadedSkinSelection = configSkinSelection.Value;
                                                loadedSkinPath = icarusArmorFilePath;
                                                loadedSkinFileModificationTime = File.GetLastWriteTime(icarusArmorFilePath);
                                                loadFlag = true;
                                            }
                                            else
                                            {
                                                Logger.LogError($"Failed to load custom icarus armour skin {icarusArmorFilePath}");
                                            }
                                        }
                                        catch (IOException ioException)
                                        {
                                            Logger.LogWarning($"WARNING: IOException while reading \"{icarusArmorFilePath}\": {ioException.Message}");
                                        }
                                    }
                                }
                            }
                            catch (ArgumentException argException)
                            {
                                Logger.LogWarning($"WARNING: ArgumentException while trying \"{icarusArmorFilePath}\": {argException.Message}");
                            }
                        }
                        else
                        {
                            if (configSkinSelection.Value != loadedSkinSelection)
                            {
                                string resourceName = resourceNames[configSkinSelection.Value];
                                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
                                if (stream != null)
                                {
                                    byte[] fileData = new byte[stream.Length];
                                    stream.Read(fileData, 0, (int)stream.Length);
                                    if (icarusArmorTextureFile.LoadImage(fileData))
                                    {
                                        Logger.LogInfo($"Successfully loaded built-in icarus armour skin {resourceName}");
                                        loadedSkinSelection = configSkinSelection.Value;
                                        loadFlag = true;
                                    }
                                }
                            }
                        }

                        if (loadFlag)
                        {
                            Texture2D icarusArmorTextureARGB = new Texture2D(2048, 2048);
                            for (int x = 0; x < icarusArmorTextureARGB.width; x++)
                            {
                                for (int y = 0; y < icarusArmorTextureARGB.height; y++)
                                {
                                    Color pixel = icarusArmorTextureFile.GetPixel(x, y);
                                    icarusArmorTextureARGB.SetPixel(x, y, new Color(pixel.r, pixel.g, pixel.b, 0));
                                }
                            }
                            icarusArmorTextureARGB.Apply();

                            mat.mainTexture = icarusArmorTextureARGB;
                            UIRoot.instance.uiGame.generalTips.InvokeRealtimeTipAhead("Icarus skin loaded");
                        }

                        icarusArmorFlag = true;
                        if (icarusArmorFlag && icarusSkeletonFlag)
                            break;
                    }
                    else if (!icarusSkeletonFlag && mat.name.StartsWith("icarus-skeleton"))
                    {
                        icarusSkeletonFlag = true;
                        if (icarusArmorFlag && icarusSkeletonFlag)
                            break;
                    }
                }
            }
        }
    }
}
