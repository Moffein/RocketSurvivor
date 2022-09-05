using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RocketSurvivor.Components.Projectile
{
    public class BlastJumpComponent : MonoBehaviour
    {
        private ProjectileImpactExplosion pie;
        private ProjectileController pc;

        private Vector3 rocketVelocity;

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
            if (pc && pc.owner)
            {
                HealthComponent hc = pc.owner.GetComponent<HealthComponent>();
                if (hc && hc.body && hc.body.characterMotor && (!requireAirborne || !hc.body.characterMotor.isGrounded))
                {
                    float aoeSqr = aoe * aoe;
                    Vector3 dist = hc.body.corePosition + bodyPositionOffset - base.transform.position;
                    if (dist.sqrMagnitude <= aoeSqr)
                    {
                        Vector3 finalForce = dist.normalized * force;

                        //Attempt to break your fall, should help with pogos
                        if (hc.body.characterMotor.velocity.y < 0f)
                        {
                            hc.body.characterMotor.velocity.y = 0f;

                            //Not much use to downwards launch, just results in accidental craters
                            if (finalForce.y < 0f) finalForce.y = 0f;
                        }

                        //Encourage proper rocket jumps: doesn't work well in practice due to the nature of force/air control in RoR2.
                        finalForce.x *= horizontalMultiplier;
                        finalForce.z *= horizontalMultiplier;


                        hc.TakeDamageForce(finalForce, true, false);

                        //Split on whether this should be a thing or not. Leaning towards excluding it since it only artificially encourages rocket jumping.
                        if (!hc.body.HasBuff(Buffs.RocketJumpSpeedBuff))
                        {
                            hc.body.AddBuff(Buffs.RocketJumpSpeedBuff);
                        }
                    }
                }

                //Debug.Log("Rocket Position" + base.transform.position);
                //Debug.Log("Body Position" + hc.body.corePosition);
            }
        }
    }
}
