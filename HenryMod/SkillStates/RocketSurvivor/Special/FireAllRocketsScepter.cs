using System;
using System.Collections.Generic;
using System.Text;

namespace EntityStates.RocketSurvivorSkills.Special
{
    class FireAllRocketsScepter : FireAllRockets
    {
        public override void ModifyStats()
        {
            this.shotsRemaining *= 2;
            this.delayBetweenShots *= 0.5f;
        }
    }
}
