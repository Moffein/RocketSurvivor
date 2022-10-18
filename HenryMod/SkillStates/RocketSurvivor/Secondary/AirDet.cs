using RocketSurvivor.Components;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EntityStates.RocketSurvivorSkills.Secondary
{
    public class AirDet : BaseState
    {
        public static GameObject explosionEffectPrefab;
        public static GameObject concExplosionEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerMineExplosion.prefab").WaitForCompletion();

        public static float forceMult = 1.3f;
        public static float radiusMult = 1.5f;
        public static float damageMult = 1.5f;
        public static float minRadius = 10f;

        public static float baseDuration = 0.25f;

        private bool buttonReleased = false;

        public override void OnEnter()
        {
            base.OnEnter();

            if (base.isAuthority)
            {
                RocketTrackerComponent rtc = base.GetComponent<RocketTrackerComponent>();
                if (rtc)
                {
                    rtc.CmdDetonateRocket();
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                if (!buttonReleased && base.inputBank && !base.inputBank.skill2.down)
                {
                    buttonReleased = true;
                }

                if (base.fixedAge > AirDet.baseDuration)
                {
                    this.outer.SetNextStateToMain();
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return buttonReleased ? InterruptPriority.Any : InterruptPriority.PrioritySkill;
        }
    }
}
