using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RocketSurvivor.Components.Projectile
{
    public class BlastJumpComponent : MonoBehaviour, IProjectileImpactBehavior
    {
        private ProjectileImpactExplosion pie;
        private ProjectileController pc;
        private ProjectileDamage pd;

        public float force = 0f;
        public float aoe = 0f;
        public float horizontalMultiplier = 1f;
        public bool requireAirborne = true;
        public bool triggerOnImpact = true;

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
            if (!pie.alive)
            {
                BlastJump();
            }
        }

        public void BlastJump()
        {
            Debug.Log("Attempting blast jump");
            if (fired || !pc || !pc.owner || (pd && pd.force <= 0f)) return;

            NetworkedBodyBlastJumpHandler nb = pc.owner.GetComponent<NetworkedBodyBlastJumpHandler>();
            if (nb && nb.hasAuthority)
            {
                fired = true;
                nb.BlastJumpAuthority(base.transform.position, aoe, force, horizontalMultiplier, requireAirborne);
                if (!triggerOnImpact) Debug.Log("Triggered Blast Jump on C4");
            }
        }

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            if (triggerOnImpact) BlastJump();
        }
    }
}
