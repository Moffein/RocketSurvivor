using EntityStates.RocketSurvivorSkills.Utility;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityStates.RocketSurvivorSkills.Special
{
    public class PrepComicallyLargeSpoonScepter : PrepComicallyLargeSpoon
    {
        public override void SetNextState()
        {
            this.outer.SetNextState(new ComicallyLargeSpoonScepter());
        }
        public override bool ButtonReleased()
        {
            return !(base.inputBank && base.inputBank.skill4.down);
        }
    }

    public class ComicallyLargeSpoonScepter : ComicallyLargeSpoon
    {
        public override float CalculateAdditionalSpeedDamage(float speed)
        {
            return base.CalculateAdditionalSpeedDamage(speed) * 2f;
        }

        public override void ModifyBlastAttack(ref BlastAttack ba)
        {
            ba.damageType.damageSource = DamageSource.Special;
            ba.radius *= 1.5f;
            ba.baseDamage *= 1.5f;
        }
    }
}
