using EntityStates.RocketSurvivorSkills.Primary;
using RocketSurvivor.Modules.Survivors;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.RocketSurvivorSkills.Special
{
    public class FireFlak : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            fireStopwatch = 0f;
            delayBetweenShots = FireFlak.baseDelayBetweenShots / base.attackSpeedStat;
            shotsRemaining = FireFlak.baseShotCount;    //Skill felt underwhelming when it was tied to primary stocks: only useful when primary is fully loaded, which is the opposite of the skill's intended purpose.
            isCrit = base.RollCrit();

            ModifyStats();
        }

        public virtual void ModifyStats() { }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fireStopwatch <= 0f)
            {
                fireStopwatch += delayBetweenShots;
                if (shotsRemaining > 0) FireProjectile();
            }
            fireStopwatch -= Time.fixedDeltaTime;

            if (base.isAuthority)
            {
                if (base.skillLocator)
                {
                    if (shotsRemaining <= 0)//(base.skillLocator.primary.stock <= 0)
                    {
                        this.outer.SetNextState(new Rearm());
                        return;
                    }
                }
            }
        }

        public void FireProjectile()
        {
            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();

                Vector3 aimDirection = aimRay.direction;

                float damageMult = RocketSurvivor.RocketSurvivorPlugin.GetICBMDamageMult(base.characterBody);

                //Copied from Bandit2
                if (RocketSurvivor.RocketSurvivorPlugin.pocketICBM && base.characterBody && base.characterBody.inventory && base.characterBody.inventory.GetItemCount(DLC1Content.Items.MoreMissile) > 0)
                {
                    Vector3 rhs = Vector3.Cross(Vector3.up, aimRay.direction);
                    Vector3 axis = Vector3.Cross(aimRay.direction, rhs);

                    float currentSpread = 0f;
                    float num2 = UnityEngine.Random.Range(1f + currentSpread, 1f + currentSpread) * 3f;   //Bandit is x2
                    float angle = num2 / 2f;  //3 - 1 rockets

                    Vector3 direction = Quaternion.AngleAxis(-num2 * 0.5f, axis) * aimRay.direction;
                    Quaternion rotation = Quaternion.AngleAxis(angle, axis);
                    Ray aimRay2 = new Ray(aimRay.origin, direction);
                    for (int i = 0; i < 3; i++)
                    {
                        aimDirection = aimRay2.direction;

                        ProjectileManager.instance.FireProjectile(FireFlak.projectilePrefab, aimRay2.origin, Util.QuaternionSafeLookRotation(aimDirection), base.gameObject, damageMult * this.damageStat * FireFlak.damageCoefficient, ((i != 1 && !RocketSurvivor.RocketSurvivorPlugin.pocketICBMEnableKnockback) ? 0f : FireFlak.force), isCrit, DamageColorIndex.Default, null, -1f);
                        aimRay2.direction = rotation * aimRay2.direction;
                    }
                }
                else
                {
                    ProjectileManager.instance.FireProjectile(FireFlak.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimDirection), base.gameObject, this.damageStat * FireFlak.damageCoefficient, FireFlak.force, isCrit, DamageColorIndex.Default, null, -1f);
                }

                base.StartAimMode(aimRay, 3f, false);
            }

            GameObject effectPrefab = FireFlak.effectPrefab;
            if (effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(effectPrefab, base.gameObject, FireFlak.muzzleString, false);
            }
            base.PlayAnimation("Gesture, Override", "Shoot", "Shoot.playbackRate", 0.169f);
            Util.PlaySound(FireRocket.attackSoundString, base.gameObject);
            shotsRemaining--;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
        
        private bool isCrit;
        private float fireStopwatch;

        public float delayBetweenShots;
        public int shotsRemaining;

        public static int baseShotCount = 1;
        public static float baseDelayBetweenShots = 0.2f;
        public static float damageCoefficient = 9f;
        public static float force = 2000f;

        public static GameObject explosionEffectPrefab;
        public static GameObject projectilePrefab;
        public static GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/MuzzleflashFMJ.prefab").WaitForCompletion();
        public static string muzzleString = "MuzzleCenter";
    }
}
