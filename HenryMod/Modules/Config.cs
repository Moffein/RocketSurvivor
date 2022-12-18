using BepInEx.Configuration;
using UnityEngine;

namespace RocketSurvivor.Modules
{
    public static class Config
    {
        public static ConfigEntry<KeyboardShortcut> KeybindEmote1 { get; private set; }
        public static ConfigEntry<KeyboardShortcut> KeybindEmote2 { get; private set; }

        public static void ReadConfig()
        {

            KeybindEmote1 = RocketSurvivorPlugin.instance.Config.Bind("Keybinds", "Emote - Sit", new KeyboardShortcut(KeyCode.Alpha1), "Button to play this emote.");
            KeybindEmote2 = RocketSurvivorPlugin.instance.Config.Bind("Keybinds", "Emote - Over-Enthusiasm", new KeyboardShortcut(KeyCode.Alpha2), "Button to play this emote.");
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