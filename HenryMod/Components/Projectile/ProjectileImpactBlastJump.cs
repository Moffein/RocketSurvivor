using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RocketSurvivor.Components.Projectile
{
    public class ProjectileImpactBlastJump : MonoBehaviour, IProjectileImpactBehavior
    {
        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            BlastJumpComponent bjc = base.GetComponent<BlastJumpComponent>();
            if (bjc) bjc.AttemptBlastJump();
        }
    }
}
