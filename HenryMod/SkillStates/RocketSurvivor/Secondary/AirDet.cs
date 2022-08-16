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
        public static NetworkSoundEventDef detonateSuccess;
        public static NetworkSoundEventDef detonateFail;
        public static GameObject explosionEffectPrefab;
        public static GameObject concExplosionEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerMineExplosion.prefab").WaitForCompletion();

        public static float forceMult = 1.3f;
        public static float radiusMult = 1.3f;
        public static float damageMult = 1.3f;

        public static float baseDuration = 0.3f;

        private bool buttonReleased = false;

        public override void OnEnter()
        {
            base.OnEnter();

            if (NetworkServer.active)
            {
                RocketTrackerComponent rtc = base.GetComponent<RocketTrackerComponent>();
                if (rtc)
                {
                    bool success = rtc.DetonateRocket();
                    if (!success)
                    {
                        rtc.RpcAddSecondaryStock();
                    }

                    //Uncomment when sounds are set up properly
                    EffectManager.SimpleSoundEffect(success ? detonateSuccess.index : detonateFail.index, base.transform.position, true);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!buttonReleased && base.inputBank && !base.inputBank.skill2.down)
            {
                buttonReleased = true;
            }

            if (base.isAuthority && base.fixedAge > AirDet.baseDuration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return buttonReleased ? InterruptPriority.Skill : InterruptPriority.PrioritySkill;
        }
    }
}
