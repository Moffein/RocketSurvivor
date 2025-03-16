using EntityStates.RocketSurvivorSkills.Utility;
using RoR2;

namespace EntityStates.RocketSurvivorSkills.Special
{
    public class PrepComicallyLargeSpoonSpecial : PrepComicallyLargeSpoon
    {
        public override void SetNextState()
        {
            this.outer.SetNextState(new ComicallyLargeSpoonSpecial());
        }

        public override bool ButtonReleased()
        {
            return !(base.inputBank && base.inputBank.skill4.down);
        }
    }
    public class ComicallyLargeSpoonSpecial : ComicallyLargeSpoon
    {
        public override void ModifyBlastAttack(ref BlastAttack ba)
        {
            ba.damageType.damageSource = DamageSource.Special;
        }
    }
}
