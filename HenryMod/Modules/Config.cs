﻿using BepInEx.Configuration;
using RiskOfOptions;
using RocketSurvivor.Components;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RocketSurvivor.Modules
{
    public static class Config
    {
        public static ConfigEntry<bool> ForceUnlock { get; private set; }
        public static ConfigEntry<KeyboardShortcut> KeybindEmoteSit { get; private set; }
        public static ConfigEntry<KeyboardShortcut> KeybindEmoteShovel { get; private set; }
        public static ConfigEntry<KeyboardShortcut> KeybindEmoteCSS { get; private set; }

        public static ConfigEntry<bool> msPaintIcons;
        public static ConfigEntry<bool> pocketICBM;
        public static ConfigEntry<bool> pocketICBMEnableKnockback;
        public static ConfigEntry<bool> samTracking;

        public static ConfigEntry<bool> muteM2Sound;

        public static void ReadConfig()
        {

            msPaintIcons = RocketSurvivorPlugin.instance.Config.Bind("General", "Use MSPaint icons", false, "Use the original MSPaint icons from the mod's release.");
            pocketICBM = RocketSurvivorPlugin.instance.Config.Bind("Gameplay", "Pocket ICBM Interaction", true, "Pocket ICBM works with Rocket's skills.");
            pocketICBMEnableKnockback = RocketSurvivorPlugin.instance.Config.Bind("Gameplay", "Pocket ICBM Knockback", false, "Extra rockets from Pocket ICBM have knockback.");
            samTracking = RocketSurvivorPlugin.instance.Config.Bind("Primaries - HG4 SAM Launcher", "Enable Homing (Server-Side)", true, "SAM Rockets will home towards targets.");
            EntityStates.RocketSurvivorSkills.Utility.C4.vrUseOffhand = RocketSurvivorPlugin.instance.Config.Bind("Utilities - Nitro Charge", "VR: Throw from Offhand", true, "When VR Motion Controls are enabled, throw C4 from your nondominant hand. Throws from your dominant hand if false.");
            muteM2Sound = RocketSurvivorPlugin.instance.Config.Bind("General", "Mute Remote Detonator Sound", false, "Mute Remote Detonator sound for yourself.");
            muteM2Sound.SettingChanged += MuteM2Sound_SettingChanged;

            ForceUnlock = RocketSurvivorPlugin.instance.Config.Bind("General", "Force Unlock", false, "Unlock all gameplay-related features.");
            KeybindEmoteSit = RocketSurvivorPlugin.instance.Config.Bind("Keybinds", "Emote - Sit", new KeyboardShortcut(KeyCode.Alpha1), "Button to play this emote.");
            KeybindEmoteCSS = RocketSurvivorPlugin.instance.Config.Bind("Keybinds", "Emote - Pose", new KeyboardShortcut(KeyCode.Alpha2), "Button to play this emote.");
            KeybindEmoteShovel = RocketSurvivorPlugin.instance.Config.Bind("Keybinds", "Emote - Over-Enthusiasm", new KeyboardShortcut(KeyCode.Alpha3), "Button to play this emote.");

            if (RocketSurvivorPlugin.riskOfOptionsLoaded)
            {
                RiskOfOptionsCompat();
            }
        }

        public static void MuteM2Sound_SettingChanged(object sender, System.EventArgs e)
        {
            if (RocketTrackerComponent.detonateSuccess)
            {
                RocketTrackerComponent.detonateSuccess.eventName = muteM2Sound.Value ? "" : "Play_Moffein_RocketSurvivor_M2_Trigger";
            }

            if (RocketTrackerComponent.detonateFail)
            {
                RocketTrackerComponent.detonateFail.eventName = muteM2Sound.Value ? "" : "Play_Moffein_RocketSurvivor_M2_NoFire";
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void RiskOfOptionsCompat()
        {
            ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(muteM2Sound));
            ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(KeybindEmoteSit));
            ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(KeybindEmoteShovel));
            ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(KeybindEmoteCSS));
            ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(EntityStates.RocketSurvivorSkills.Utility.C4.vrUseOffhand));
        }

        // this helper automatically makes config entries for disabling survivors
        public static ConfigEntry<bool> CharacterEnableConfig(string characterName, string description = "Set to false to disable this character", bool enabledDefault = true) {

            return RocketSurvivorPlugin.instance.Config.Bind<bool>("General",
                                                          "Enable " + characterName,
                                                          enabledDefault,
                                                          description);
        }

        //Taken from https://github.com/ToastedOven/CustomEmotesAPI/blob/main/CustomEmotesAPI/CustomEmotesAPI/CustomEmotesAPI.cs
        public static bool GetKeyPressed(ConfigEntry<KeyboardShortcut> entry) {
            foreach (var item in entry.Value.Modifiers) {
                if (!Input.GetKey(item)) {
                    return false;
                }
            }
            return Input.GetKeyDown(entry.Value.MainKey);
        }
    }
}