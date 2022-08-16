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

            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Blastoff");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "Your explosives can be used to <style=cIsUtility>blast jump</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_NAME", "HG4 Missile Launcher");
            LanguageAPI.Add(prefix + "PRIMARY_DESCRIPTION", "Fire a missile for <style=cIsDamage>390% damage</style>. Can hold up to 4.");

            //Unlock Condition: Hit 20 airshots.
            LanguageAPI.Add(prefix + "PRIMARY_ALT_NAME", "HG4 SAM Launcher");
            LanguageAPI.Add(prefix + "PRIMARY_ALT_DESCRIPTION", "Fire a precision missile for <style=cIsDamage>450% damage</style>. Deals <style=cIsDamage>+30% damage</style> against <style=cIsUtility>airborne targets</style>. Can hold up to 4.");

            LanguageAPI.Add(prefix + "SECONDARY_NAME", "Remote Detonator");
            LanguageAPI.Add(prefix + "SECONDARY_DESCRIPTION", "Detonate your <style=cIsDamage>last-fired missile</style>, increasing its <style=cIsDamage>damage</style> and <style=cIsDamage>blast radius</style> by <style=cIsDamage>30%</style>.");

            LanguageAPI.Add(prefix + "UTILITY_NAME", "Concussion Blast");
            LanguageAPI.Add(prefix + "UTILITY_DESCRIPTION", "<style=cIsDamage>Stunning</style>. Fire a nonlethal missile that <style=cIsUtility>pushes</style> enemies away.");

            //Unlock Condition: Kill 5 enemies in a single rocket jump.
            LanguageAPI.Add(prefix + "UTILITY_ALT_NAME", "Bombing Run");
            LanguageAPI.Add(prefix + "UTILITY_ALT_DESCRIPTION", "<style=cIsDamage>Stunning</style>. Swing an explosive at an enemy for <style=cIsDamage>800% damage</style>. Deals <style=cIsDamage>Critical Strikes</style> while <style=cIsUtility>rocket jumping</style>.");

            LanguageAPI.Add(prefix + "SPECIAL_NAME", "Rapid Rearm");
            LanguageAPI.Add(prefix + "SPECIAL_DESCRIPTION", "Rapidly <style=cIsDamage>fire 4 missiles</style>, then <style=cIsUtility>reload your weapon</style>.");
        }
    }
}