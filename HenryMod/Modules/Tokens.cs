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


            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");

            LanguageAPI.Add(prefix + "SUBTITLE", "Shock and Awe");

            LanguageAPI.Add(prefix + "LORE", "bio: rocket was born with a special power");

            LanguageAPI.Add(prefix + "DESCRIPTION", "The Rocket is an explosives expert who deals devastating area damage.<style=cSub>\r\n\r\n< ! > Blast jump often to traverse the map quickly and escape danger!\r\n\r\n< ! > Your Rocket Launcher has high knockback against enemies. Use this to knock them off of ledges!\r\n\r\n< ! > Remote Detonator is useful for hitting flying enemies, and can also be used for mobility.\r\n\r\n< ! > Concussion Blast's armor debuff can be maintained by juggling enemies in the air with your rockets.\r\n\r\n< ! > Rapid Rearm can be used to skip your rocket launcher's lengthy reload time.");

            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", "..and so he left, blasting off again.");
            LanguageAPI.Add(prefix + "MAIN_ENDING_ESCAPE_FAILURE_FLAVOR", "..and so he vanished, crashing down.");

            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Blastoff");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "Your explosives can be used to <style=cIsUtility>blast jump</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_NAME", "HG4 Rocket Launcher");
            LanguageAPI.Add(prefix + "PRIMARY_DESCRIPTION", "<style=cIsUtility>Agile</style>. Fire a rocket for <style=cIsDamage>520% damage</style>. Can hold up to 4.");

            //Unlock Condition: Hit 20 airshots.
            LanguageAPI.Add(prefix + "PRIMARY_ALT_NAME", "HG4 SAM Launcher");
            LanguageAPI.Add(prefix + "PRIMARY_ALT_DESCRIPTION", "<style=cIsUtility>Agile</style>. Fire a heat-seeking precision rocket for <style=cIsDamage>650% damage</style>. Deals <style=cIsDamage>+30% damage</style> against <style=cIsUtility>airborne targets</style>. Can hold up to 4.");

            LanguageAPI.Add(prefix + "SECONDARY_NAME", "Remote Detonator");
            LanguageAPI.Add(prefix + "SECONDARY_DESCRIPTION", "Detonate <style=cIsDamage>all rockets</style>, increasing their <style=cIsDamage>damage</style> and <style=cIsDamage>blast radius</style> by <style=cIsDamage>30%</style>.");

            LanguageAPI.Add(prefix + "UTILITY_NAME", "Concussion Blast");
            LanguageAPI.Add(prefix + "UTILITY_DESCRIPTION", "<style=cIsDamage>Stunning</style>. Toss a nonlethal grenade that <style=cIsUtility>pushes</style> enemies away and <style=cIsDamage>reduces their armor</style> by <style=cIsDamage>40</style> while <style=cIsUtility>airborne</style>.");

            //Unlock Condition: Kill 5 enemies in a single rocket jump.
            LanguageAPI.Add(prefix + "UTILITY_ALT_NAME", "Bombing Run");
            LanguageAPI.Add(prefix + "UTILITY_ALT_DESCRIPTION", "<style=cIsDamage>Stunning</style>. Swing an explosive at an enemy for <style=cIsDamage>1000% damage</style>. Deals <style=cIsDamage>Critical Strikes</style> while <style=cIsUtility>rocket jumping</style>.");

            LanguageAPI.Add(prefix + "SPECIAL_NAME", "Rapid Rearm");
            LanguageAPI.Add(prefix + "SPECIAL_DESCRIPTION", "Rapidly <style=cIsDamage>fire 4 rockets</style>, then <style=cIsUtility>reload your weapon</style>.");

            LanguageAPI.Add(prefix + "SPECIAL_SCEPTER_NAME", "Rocketstorm");
            LanguageAPI.Add(prefix + "SPECIAL_SCEPTER_DESCRIPTION", "Rapidly <style=cIsDamage>fire 8 rockets</style>, then <style=cIsUtility>reload your weapon</style>.");
        }
    }
}