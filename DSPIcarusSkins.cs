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
        public const string pluginVersion = "1.0.0";
        new internal static ManualLogSource Logger;
        new internal static BepInEx.Configuration.ConfigFile Config;
        Harmony harmony;

        public static BepInEx.Configuration.ConfigEntry<UInt16> configSkinSelection;
        public static BepInEx.Configuration.ConfigEntry<string> configSkinPath;

        public static readonly string[] resourceNames = {
                "",
                "DSPIcarusSkins.Built_In_Skins.camoDark.png",
                "DSPIcarusSkins.Built_In_Skins.camoLight.png",
                "DSPIcarusSkins.Built_In_Skins.camoRWB.png",
                "DSPIcarusSkins.Built_In_Skins.blue.png",
                "DSPIcarusSkins.Built_In_Skins.color.png",
                "DSPIcarusSkins.Built_In_Skins.bluegold.png",
                "DSPIcarusSkins.Built_In_Skins.redgold.png",
            };

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

            configSkinSelection = Config.Bind<UInt16>("Skin", "Selection", 2, new BepInEx.Configuration.ConfigDescription("0:Path Provided;  1:Dark Camo;  2:Light Camo;  3:Red/White/Blue Camo;  4:Blue;  5:Color;  6:Blue/gold;  7:Red/gold", new BepInEx.Configuration.AcceptableValueRange<UInt16>(0, 8)));
            configSkinPath = Config.Bind<string>("Skin", "Path", "", "Path to the 2048 x 2048 skin image.  This setting is only used if Selection is set to 0.");
            OnConfigChanged(null, null);
            Config.ConfigReloaded += OnConfigChanged;
            Config.SettingChanged += OnConfigChanged;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerAnimator), "Start")]
        public static void PlayerAnimator_Start_Postfix()
        {
            Config.Reload();
            OnConfigChanged(null, null);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(GameMain), "Resume")]
        public static void GameMain_Resume_Postfix()
        {
            Config.Reload();
            OnConfigChanged(null, null);
        }

        public static void OnConfigChanged(object sender, EventArgs e)
        {
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
                        Texture2D newIcarusArmorTexture = new Texture2D(2048, 2048);
                        if (configSkinSelection.Value == 0 || configSkinSelection.Value >= resourceNames.Length)
                        {
                            string icarusArmorFilePath = configSkinPath.Value;
                            if (System.IO.File.Exists(icarusArmorFilePath))
                            {
                                byte[] fileData = System.IO.File.ReadAllBytes(icarusArmorFilePath);
                                if (newIcarusArmorTexture.LoadImage(fileData))
                                {
                                    Logger.LogInfo($"Successfully loaded custom icarus armour skin {icarusArmorFilePath}");
                                }
                            }
                        }
                        else
                        {
                            string resourceName = resourceNames[configSkinSelection.Value];
                            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
                            if (stream != null)
                            {
                                byte[] fileData = new byte[stream.Length];
                                stream.Read(fileData, 0, (int)stream.Length);
                                if (newIcarusArmorTexture.LoadImage(fileData))
                                {
                                    Logger.LogInfo($"Successfully loaded built-in icarus armour skin {resourceName}");
                                }
                            }
                        }

                        for (int x = 0; x < newIcarusArmorTexture.width; x++)
                        {
                            for (int y = 0; y < newIcarusArmorTexture.height; y++)
                            {
                                Color pixel = newIcarusArmorTexture.GetPixel(x, y);
                                newIcarusArmorTexture.SetPixel(x, y, new Color(pixel.r, pixel.g, pixel.b, 0));
                            }
                        }
                        newIcarusArmorTexture.Apply();

                        mat.mainTexture = newIcarusArmorTexture;

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
