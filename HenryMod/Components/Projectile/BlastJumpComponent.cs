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

        public float force = 0f;
        public float aoe = 0f;
        public float horizontalMultiplier = 1f;
        public bool requireAirborne = true;
        public bool triggerOnImpact = true;
        public bool blastJumpOnDestroy = true;  //Set to false for C4 so that the server can handle it
        public bool runOnServer = false;    //Set to true for isPrediction projectiles that need to explode on impact.

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
                AttemptBlastJump();
            }
        }

        public void OnDestroy()
        {
            if (!pie.alive && blastJumpOnDestroy)
            {
                AttemptBlastJump();
            }
        }

        //Other 2 methods are still public because there are cases where you'd want to use them even if runOnServer is on/off, maybe.
        public void AttemptBlastJump()
        {
            if (runOnServer)
            {
                BlastJumpServer();
            }
            else
            {
                BlastJump();
            }
        }

        public void BlastJump()
        {
            //Removed force check because it doesnt work with new Blast Jump stuff.
            if (fired || !pc || !pc.owner) return; // || (pd && pd.force <= 0f)

            NetworkedBodyBlastJumpHandler nb = pc.owner.GetComponent<NetworkedBodyBlastJumpHandler>();
            if (nb && nb.hasAuthority)
            {
                fired = true;
                nb.BlastJumpAuthority(base.transform.position, aoe, force, horizontalMultiplier, requireAirborne);
            }
        }

        public void BlastJumpServer()
        {
            if (!NetworkServer.active) return;
            fired = true;

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
