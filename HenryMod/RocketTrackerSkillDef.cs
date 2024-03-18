using UnityEngine;
using RoR2;
using RoR2.Skills;
using RocketSurvivor.Components;
using UnityEngine.Bindings;
using JetBrains.Annotations;

namespace RocketSurvivor
{
    public class RocketTrackerSkillDef : SkillDef
	{
		public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
		{
			return new RocketTrackerSkillDef.InstanceData
			{
				rocketTracker = skillSlot.GetComponent<RocketTrackerComponent>()
			};
		}

		public override bool CanExecute([NotNull] GenericSkill skillSlot)
		{
			return RocketTrackerSkillDef.HasRocket(skillSlot) && base.CanExecute(skillSlot);
		}

		public override bool IsReady([NotNull] GenericSkill skillSlot)
		{
			return base.IsReady(skillSlot) && RocketTrackerSkillDef.HasRocket(skillSlot);
		}

		protected class InstanceData : SkillDef.BaseSkillInstanceData
		{
			public RocketTrackerComponent rocketTracker;
		}

		private static bool HasRocket([NotNull] GenericSkill skillSlot)
		{
			RocketTrackerComponent rocketTracker = ((RocketTrackerSkillDef.InstanceData)skillSlot.skillInstanceData).rocketTracker;
			return (rocketTracker != null) ? rocketTracker.IsRocketAvailable() : false;
		}
	}
}
