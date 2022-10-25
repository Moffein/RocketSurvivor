using System;
using System.Collections.Generic;
using System.Text;

namespace EntityStates.RocketSurvivorSkills.Special
{
    public class FireFlakScepter : FireFlak
    {
        public override void ModifyStats()
        {
            this.shotsRemaining *= 2;
        }
    }
}
