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
                NetworkedBodyBlastJumpHandler nb = pc.owner.GetComponent<NetworkedBodyBlastJumpHandler>();
                if (nb)
                {
                    nb.RpcBlastJump(base.transform.position, aoe, force, horizontalMultiplier, requireAirborne);
                }
            }
        }
    }
}
