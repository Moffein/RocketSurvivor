using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.RocketSurvivorSkills.Special
{
    public class Rearm : BaseState
    {
        public static string soundString = "Play_Moffein_RocketSurvivor_Shift_Rearm";
        public static float baseDuration = 0.96f;
        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = Rearm.baseDuration / base.attackSpeedStat;
            Util.PlayAttackSpeedSound(soundString, base.gameObject, base.attackSpeedStat);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge > duration && base.isAuthority)
            {
                if (base.skillLocator)
                {
                    base.skillLocator.primary.Reset();
                }
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}
