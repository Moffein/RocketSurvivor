using RoR2;

namespace RocketSurvivor.Modules.Achievements 
{
    //[RegisterAchievement(identifier, unlockableIdentifier, null, null)]
    public class GrandMastery : BaseMasteryAchievement 
    {
        public const string identifier = "MoffeinRocketClearGameTyphoon";
        public const string unlockableIdentifier = "Skins.MoffeinRocketSurvivor.GrandMastery";

        public override string RequiredCharacterBody => "RocketSurvivorBody";

        public override float RequiredDifficultyCoefficient => 3.5f;
    }
}
