using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RocketSurvivor.Components;
using RoR2;
using UnityEngine;
using System;

namespace RocketSurvivor
{
    public class DamageTypes
    {
        public static bool initialized = false;
        public static DamageAPI.ModdedDamageType ScaleForceToMass;
        public static DamageAPI.ModdedDamageType AirborneBonus;
        public static DamageAPI.ModdedDamageType MarketGarden;
        public static DamageAPI.ModdedDamageType SlamDunk;
        public static DamageAPI.ModdedDamageType SweetSpotModifier;

        public static void Initialize()
        {
            if (initialized) return;

            ScaleForceToMass = DamageAPI.ReserveDamageType();
            AirborneBonus = DamageAPI.ReserveDamageType();
            MarketGarden = DamageAPI.ReserveDamageType();
            SlamDunk = DamageAPI.ReserveDamageType();
            SweetSpotModifier = DamageAPI.ReserveDamageType();   //Makes sweetspot falloff do -50% instead of -75%

            IL.RoR2.BlastAttack.HandleHits += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(MoveType.After, x => x.MatchLdcR4(0.75f)))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<float, BlastAttack, float>>((damagePenalty, self) =>
                    {
                        if (self.HasModdedDamageType(SweetSpotModifier))
                        {
                            damagePenalty *= 0.6666666666f;
                        }
                        return damagePenalty;
                    });
                }
                else
                {
                    Debug.LogError("RocketSurvivor: SweetSpotModifier IL Hook failed.");
                }
            };

            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;

            initialized = true;
        }

        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo damageInfo)
        {
            CharacterBody cb = self.body;

            if (cb)
            {
                bool playAirshotSound = false;

                if (damageInfo.HasModdedDamageType(DamageTypes.ScaleForceToMass))
                {
                    bool isGrounded = true;
                    float mass = 0f;

                    if (cb.characterMotor)
                    {
                        //Change direction to be based on coreposition
                        isGrounded = cb.characterMotor.isGrounded;
                        float magnitude = damageInfo.force.magnitude;
                        if (damageInfo.inflictor && damageInfo.inflictor.transform)
                        {
                            //RoR2 force calculations can't be trusted because it's calculated based on hitbox instead of coreposition
                            damageInfo.force = (cb.corePosition - damageInfo.inflictor.transform.position).normalized * magnitude;
                        }

                        mass = cb.characterMotor.mass;
                        if (!cb.characterMotor.isFlying)
                        {
                            //if (damageInfo.force.y < 0f) damageInfo.force.y = 0f;

                            //Negate falling speed
                            //damageInfo.force.y += magnitude;
                            if (!isGrounded && cb.characterMotor.velocity.y < 0f)
                            {
                                damageInfo.force.y += cb.characterMotor.velocity.y * -120f;
                            }
                        }
                    }
                    else if (cb.rigidbody)
                    {
                        mass = cb.rigidbody.mass;
                    }

                    float scalingFactor = Mathf.Max(1f, (mass / 100f) * ((self.body.isChampion && isGrounded) ? 0.3f : 1f));
                    scalingFactor = Mathf.Min(scalingFactor, 2.5f); //2.5f is Loader limit

                    damageInfo.force *= scalingFactor;
                }


                if (damageInfo.HasModdedDamageType(DamageTypes.AirborneBonus))
                {
                    if (cb.isFlying || (cb.characterMotor && !cb.characterMotor.isGrounded))
                    {
                        damageInfo.damage *= 1.3f;
                        playAirshotSound = true;
                    }
                }

                if (damageInfo.HasModdedDamageType(DamageTypes.SlamDunk))
                {
                    Vector3 direction = Vector3.down;
                    if (cb.isFlying)
                    {
                        //Scale force to match mass
                        Rigidbody rb = cb.rigidbody;
                        if (rb)
                        {
                            //Reset Y force so that it overrides ScaleForceToMass
                            damageInfo.force.y = 0f;

                            direction *= Mathf.Min(2.5f, Mathf.Max(rb.mass / 100f, 1f));  //Greater Wisp 300f, SCU 1000f, loader limit is 250f
                            damageInfo.force += 1600f * direction;
                        }
                    }
                }

                if (playAirshotSound && damageInfo.HasModdedDamageType(DamageTypes.ScaleForceToMass) && damageInfo.damage > 0 && ((damageInfo.damageType & DamageType.Silent) == 0))    //Check for ScaleForceToMass damageType so that only Rocket Skills play the sound.
                {
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

            orig(self, damageInfo);
        }
    }
}
