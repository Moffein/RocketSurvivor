using R2API;
using RoR2;
using UnityEngine;

namespace RocketSurvivor
{
    public class DamageTypes
    {
        public static bool initialized = false;
        public static DamageAPI.ModdedDamageType ScaleForceToMass;
        public static DamageAPI.ModdedDamageType AirborneBonus;
        public static DamageAPI.ModdedDamageType MarketGarden;
        public static DamageAPI.ModdedDamageType SlamDunk;
        public static DamageAPI.ModdedDamageType LaunchIntoAir;

        public static void Initialize()
        {
            if (initialized) return;

            ScaleForceToMass = DamageAPI.ReserveDamageType();
            AirborneBonus = DamageAPI.ReserveDamageType();
            MarketGarden = DamageAPI.ReserveDamageType();
            SlamDunk = DamageAPI.ReserveDamageType();
            LaunchIntoAir = DamageAPI.ReserveDamageType();

            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;

            initialized = true;
        }

        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo damageInfo)
        {
            if (damageInfo.HasModdedDamageType(DamageTypes.ScaleForceToMass))
            {

                CharacterBody cb = self.body;
                if (cb)
                {
                    bool isGrounded = true;
                    float mass = 0f;
                    if (cb.characterMotor)
                    {
                        mass = cb.characterMotor.mass;
                        if (!cb.characterMotor.isFlying && !cb.isFlying)
                        {
                            if (damageInfo.force.y < 0f) damageInfo.force.y = 0f;
                            damageInfo.force.y += 800f;
                            isGrounded = cb.characterMotor.isGrounded;
                            //Negate falling speed
                            if (!cb.characterMotor.isGrounded && cb.characterMotor.velocity.y < 0f)
                            {
                                damageInfo.force.y += cb.characterMotor.velocity.y * -150f;
                            }
                        }
                    }
                    else if (cb.rigidbody)
                    {
                        mass = cb.rigidbody.mass;
                    }

                    float scalingFactor = Mathf.Max(1f, (mass / 100f) * ((self.body.isChampion && isGrounded) ? 0.3f : 1f));

                    damageInfo.force *= scalingFactor;
                }
            }

            if (damageInfo.HasModdedDamageType(DamageTypes.AirborneBonus))
            {
                CharacterBody cb = self.body;
                if (cb.isFlying || (cb.characterMotor && !cb.characterMotor.isGrounded))
                {
                    damageInfo.damage *= 1.4f;
                    if (damageInfo.damageColorIndex == DamageColorIndex.Default) damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;

                    EffectManager.SimpleSoundEffect(RocketSurvivor.Modules.Assets.spoonHitSoundEvent.index, damageInfo.position, true);
                }
            }

            if (damageInfo.HasModdedDamageType(DamageTypes.MarketGarden) && damageInfo.attacker)
            {
                CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                if (attackerBody && attackerBody.HasBuff(Buffs.RocketJumpSpeedBuff))
                {
                    damageInfo.crit = true;

                    float bonusCritMult = attackerBody.crit / 100f;
                    float totalMultiplier = (attackerBody.critMultiplier + bonusCritMult) / attackerBody.critMultiplier;
                    damageInfo.damage *= totalMultiplier;
                }
            }

            if (damageInfo.HasModdedDamageType(DamageTypes.SlamDunk))
            {
                Vector3 direction = Vector3.down;
                CharacterBody cb = self.body;
                if (cb && cb.isFlying)
                {
                    //Scale force to match mass
                    Rigidbody rb = cb.rigidbody;
                    if (rb)
                    {
                        //Reset Y force so that it overrides ScaleForceToMass
                        damageInfo.force.y = 0f;

                        direction *= Mathf.Min(4f, Mathf.Max(rb.mass / 100f, 1f));  //Greater Wisp 300f, SCU 1000f
                        damageInfo.force += 1600f * direction;
                    }
                }
            }

            if (damageInfo.HasModdedDamageType(DamageTypes.LaunchIntoAir))
            {
                CharacterBody cb = self.body;
                if (cb)
                {
                    if (!cb.isFlying && cb.characterMotor != null)
                    {
                        if (damageInfo.force.y < 0f) damageInfo.force.y = 0f;
                        Vector3 addForce = Vector3.up * 2000f;
                        float forceMult = Mathf.Max(1f, cb.characterMotor.mass / 100f);
                        damageInfo.force += forceMult * addForce;
                    }
                }
            }

            orig(self, damageInfo);
        }
    }
}
