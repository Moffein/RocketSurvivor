using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace RocketSurvivor.Modules.Achievements 
{
    [RegisterAchievement(identifier, unlockableIdentifier, null, 10u, null)]
    public class Mastery : BasePerSurvivorClearGameMonsoonAchievement
    {
        public const string identifier = "MoffeinRocketClearGameMonsoon";
        public const string unlockableIdentifier = "Skins.MoffeinRocketSurvivor.Mastery";

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("RocketSurvivorBody");
        }
    }
}
