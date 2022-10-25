using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RocketSurvivor.Components.Projectile
{
    public class BlastJumpComponent : MonoBehaviour
    {
        private ProjectileImpactExplosion pie;
        private ProjectileController pc;
        private ProjectileDamage pd;

        private HealthComponent healthComponent;

        public float force = 0f;
        public float aoe = 0f;
        public float horizontalMultiplier = 1f;
        public bool requireAirborne = true;

        public static Vector3 bodyPositionOffset = new Vector3(0f, 0.5f, 0f);

        private bool fired = false;

        public void Awake()
        {
            pie = base.GetComponent<ProjectileImpactExplosion>();
            pc = base.GetComponent<ProjectileController>();
            pd = base.GetComponent<ProjectileDamage>();

            if (!pie || !pc)
            {
                Destroy(this);
                return;
            }
            else
            {
                if (aoe == 0f) aoe = pie.blastRadius;
            }
        }

        public void FixedUpdate()
        {
            if (!NetworkServer.active) return;

            if (fired)
            {
                Destroy(this);
                return;
            }

            if (!pie.alive)
            {
                BlastJump();
            }
        }

        public void OnDestroy()
        {
            if (NetworkServer.active && !fired && !pie.alive)
            {
                BlastJump();
            }
        }

        public void BlastJump()
        {
            if (!NetworkServer.active) return;
            fired = true;

            //Used to disable rocket jumps on extra ICBM rockets.
            if (pd && pd.force <= 0f) return;

            if (pc && pc.owner)
            {
                if (!healthComponent) healthComponent = pc.owner.GetComponent<HealthComponent>();

                if (healthComponent && healthComponent.body && healthComponent.body.characterMotor && (!requireAirborne || !healthComponent.body.characterMotor.isGrounded))
                {
                    Collider[] array = Physics.OverlapSphere(base.transform.position, aoe, LayerIndex.entityPrecise.mask);
                    for (int i = 0; i < array.Length; i++)
                    {
                        HurtBox hurtBox = array[i].GetComponent<HurtBox>();
                        if (hurtBox)
                        {
                            HealthComponent hc = hurtBox.healthComponent;
                            if (hc == healthComponent)
                            {
                                Vector3 dist = healthComponent.body.corePosition + bodyPositionOffset - base.transform.position;

                                Vector3 finalForce = dist.normalized * force;

                                //Attempt to break your fall, should help with pogos
                                if (healthComponent.body.characterMotor.velocity.y < 0f)
                                {
                                    healthComponent.body.characterMotor.velocity.y = 0f;

                                    //Not much use to downwards launch, just results in accidental craters
                                    if (finalForce.y < 0f) finalForce.y = 0f;
                                }

                                //Encourage proper rocket jumps: doesn't work well in practice due to the nature of force/air control in RoR2.
                                finalForce.x *= horizontalMultiplier;
                                finalForce.z *= horizontalMultiplier;


                                healthComponent.TakeDamageForce(finalForce, true, false);

                                //Split on whether this should be a thing or not. Leaning towards excluding it since it only artificially encourages rocket jumping.
                                if (!healthComponent.body.HasBuff(Buffs.RocketJumpSpeedBuff))
                                {
                                    healthComponent.body.AddBuff(Buffs.RocketJumpSpeedBuff);
                                }

                                break;
                            }
                        }
                    }

                }
            }
        }
    }
}
