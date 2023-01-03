using UnityEngine;

namespace EntityStates.RocketSurvivorSkills.Special
{
    public class FireAllRocketsScepter : FireAllRockets
    {
        public override void ModifyStats()
        {
            this.shotsRemaining = Mathf.FloorToInt(this.shotsRemaining * 2.5f);
            this.delayBetweenShots *= 0.5f;
        }
    }
}
