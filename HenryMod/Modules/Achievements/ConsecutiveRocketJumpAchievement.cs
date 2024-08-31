using RocketSurvivor.Components;
using RoR2;
using RoR2.Achievements;
using UnityEngine;
using UnityEngine.Networking;

namespace RocketSurvivor.Modules.Achievements
{
    [RegisterAchievement("MoffeinRocketMarketGardenUnlock", "Skills.MoffeinRocketSurvivor.MarketGaden", null, 3u, null)]
    public class ConsecutiveRocketJumpAchievement : BaseAchievement
	{
		private int consecutiveJumps;

		public override void OnInstall()
		{
			base.OnInstall();
			NetworkedBodyBlastJumpHandler.onBlastJumpClient += OnBlastJumpClient;
		}

		public override void OnUninstall()
		{
			NetworkedBodyBlastJumpHandler.onBlastJumpClient -= OnBlastJumpClient;
			base.OnUninstall();
		}

		private void OnBlastJumpClient(NetworkedBodyBlastJumpHandler self)
        {
			if (self.characterBody == base.localUser.cachedBody)
            {
				if (!self.characterBody.HasBuff(Buffs.RocketJumpSpeedBuff)) consecutiveJumps = 0;
				consecutiveJumps++;

				if (consecutiveJumps >= 10)
                {
					base.Grant();
                }
            }
        }
	}
}
