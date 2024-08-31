using RoR2;
using RoR2.Achievements;
using UnityEngine;
using UnityEngine.Networking;

namespace RocketSurvivor.Modules.Achievements
{
	[RegisterAchievement("MoffeinRocketHomingUnlock", "Skills.MoffeinRocketSurvivor.Homing", null, 3u, null)]
	public class SpeedrunAchievement : BaseAchievement
    {
		public override void OnInstall()
		{
			base.OnInstall();
			this.requiredSceneDef = SceneCatalog.GetSceneDefFromSceneName("mysteryspace");
		}

		public override BodyIndex LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("RocketSurvivorBody");
		}

		public override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
			SceneCatalog.onMostRecentSceneDefChanged += this.OnMostRecentSceneDefChanged;
		}

		private void OnMostRecentSceneDefChanged(SceneDef sceneDef)
		{
			if (sceneDef == this.requiredSceneDef && Run.instance.GetRunStopwatch() <= 1500f)
			{
				base.Grant();
			}
		}

		public override void OnBodyRequirementBroken()
		{
			SceneCatalog.onMostRecentSceneDefChanged -= this.OnMostRecentSceneDefChanged;
			base.OnBodyRequirementBroken();
		}

		private SceneDef requiredSceneDef;
	}
}
