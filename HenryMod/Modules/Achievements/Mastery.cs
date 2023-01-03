using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace RocketSurvivor.Modules.Achievements
{
    [RegisterAchievement("MoffeinRocketClearGameMonsoon", "Skins.MoffeinRocketSurvivor.Mastery", null, null)]
    public class Mastery : BasePerSurvivorClearGameMonsoonAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("RocketSurvivorBody");
        }
    }
}
