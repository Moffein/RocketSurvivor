using UnityEngine;
using RoR2;
using RocketSurvivor;

namespace EntityStates.RocketSurvivorSkills
{
    public class RocketMainState : GenericCharacterMain
    {
        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = base.GetModelAnimator();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.characterBody)
            {
                if (animator)
                {
                    animator.SetFloat("RocketJump", base.characterBody.HasBuff(Buffs.RocketJumpSpeedBuff) ? 1f : 0f);
                }
            }
        }
    }
}
