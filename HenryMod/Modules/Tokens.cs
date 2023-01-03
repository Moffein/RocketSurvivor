using R2API;
using System;

namespace RocketSurvivor.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            //Todo:Convert to external language file
            string prefix = RocketSurvivorPlugin.DEVELOPER_PREFIX + "_ROCKET_BODY_";

            LanguageAPI.Add(prefix + "NAME", "Rocket");

            LanguageAPI.Add(prefix + "SUBTITLE", "Shock and Awe");

            LanguageAPI.Add(prefix + "LORE", "bio: rocket was born with a special power");

            LanguageAPI.Add(prefix + "DESCRIPTION", "The Rocket is a highly mobile explosives expert who deals devastating area damage.<style=cSub>\r\n\r\n< ! > Blast jump often to traverse the map quickly and escape danger!\r\n\r\n< ! > Your Rocket Launcher has high knockback against enemies. Use this to knock them off of ledges.\r\n\r\n< ! > Remote Detonator is useful for hitting flying enemies, and can also be used for mobility.\r\n\r\n< ! > Nitro Charge is great for both area damage and mobility.\r\n\r\n< ! > Rapid Rearm can be used to skip your rocket launcher's lengthy reload time.");

            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", "..and so he left, blasting off again.");
            LanguageAPI.Add(prefix + "MAIN_ENDING_ESCAPE_FAILURE_FLAVOR", "..and so he vanished, never to touch the ground again.");
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", "..and so he vanished, never to touch the ground again.");

            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Blastoff");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "Your explosives can be used to <style=cIsUtility>blast jump</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_NAME", "HG4 Rocket Launcher");
            LanguageAPI.Add(prefix + "PRIMARY_DESCRIPTION", "<style=cIsUtility>Agile</style>. Fire a rocket for <style=cIsDamage>600% damage</style>. Can hold up to 4.");

            LanguageAPI.Add(prefix + "PRIMARY_ALT_NAME", "HG4 SAM Launcher");
            LanguageAPI.Add(prefix + "PRIMARY_ALT_DESCRIPTION", "<style=cIsUtility>Agile</style>. Fire a <style=cIsUtility>heat-seeking</style> missile with a smaller <style=cIsHealth>blast radius</style> that deals <style=cIsDamage>600% damage</style>. Can hold up to 4.");

            LanguageAPI.Add(prefix + "SECONDARY_NAME", "Remote Detonator");
            LanguageAPI.Add(prefix + "SECONDARY_DESCRIPTION", "Detonate <style=cIsDamage>all explosives</style>. Detonated <style=cIsDamage>rockets</style> gain <style=cIsDamage>50%</style> extra <style=cIsDamage>damage</style> and <style=cIsDamage>blast radius</style>.");

            LanguageAPI.Add(prefix + "UTILITY_NAME", "Nitro Charge");
            LanguageAPI.Add(prefix + "UTILITY_DESCRIPTION", "<style=cIsDamage>Stunning</style>. Place an explosive charge. Use <style=cIsDamage>Remote Detonator</style> to detonate it for <style=cIsDamage>1200% damage</style>.");

            LanguageAPI.Add(prefix + "UTILITY_ALT_NAME", "Bombing Run");
            LanguageAPI.Add(prefix + "UTILITY_ALT_DESCRIPTION", "<style=cIsUtility>Heavy</style>. <style=cIsDamage>Stunning</style>. Swing your explosive shovel at an enemy for <style=cIsDamage>1200% damage</style>. Deals <style=cIsDamage>Critical Strikes</style> while <style=cIsUtility>blast jumping</style>.");

            LanguageAPI.Add(prefix + "SPECIAL_NAME", "Rapid Rearm");
            LanguageAPI.Add(prefix + "SPECIAL_DESCRIPTION", "Rapidly <style=cIsDamage>fire 4 rockets</style>, then <style=cIsUtility>reload your weapon</style>.");

            LanguageAPI.Add(prefix + "SPECIAL_SCEPTER_NAME", "Rocketstorm");
            LanguageAPI.Add(prefix + "SPECIAL_SCEPTER_DESCRIPTION", "Rapidly <style=cIsDamage>fire 8 rockets</style>, then <style=cIsUtility>reload your weapon</style>.");

            LanguageAPI.Add("ACHIEVEMENT_MOFFEINROCKETHOMINGUNLOCK_NAME", "Rocket: Speed is Life");
            LanguageAPI.Add("ACHIEVEMENT_MOFFEINROCKETHOMINGUNLOCK_DESCRIPTION", "As Rocket, reach and proceed through the Celestial Portal in 25 minutes or less.");

            LanguageAPI.Add("ACHIEVEMENT_MOFFEINROCKETMARKETGARDENUNLOCK_NAME", "Rocket: Pogo");
            LanguageAPI.Add("ACHIEVEMENT_MOFFEINROCKETMARKETGARDENUNLOCK_DESCRIPTION", "As Rocket, blast jump 10 times in a row without touching the ground.");
        }
    }
}