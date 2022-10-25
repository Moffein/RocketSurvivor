using UnityEngine;
using RoR2;
using RoR2.Projectile;
using UnityEngine.Networking;

namespace RocketSurvivor.Components.Projectile
{
    //Need to fire this in a specific spread pattern, don't rely on ProjectileImpactExplosion FireChildren
    public class FlakShotgunComponent : MonoBehaviour
    {
        public static GameObject projectilePrefab;

        private ProjectileImpactExplosion pie;
        private ProjectileDamage pd;
        private ProjectileController pc;

        public static float damageCoefficient = 4f/15f;   //Percentage of original rocket's damage.
        public static int shotCount = 5;
        
        public void Awake()
        {
            pie = base.GetComponent<ProjectileImpactExplosion>();
            pd = base.GetComponent<ProjectileDamage>();
            pc = base.GetComponent<ProjectileController>();
        }

        public void OnDestroy()
        {
            if (NetworkServer.active && pie && !pie.hasImpact && pd && pc)
            {
                FireFlakProjectiles(1f);
            }
        }

        //Pass in a damageMult for external use
        public void FireFlakProjectiles(float damageMult)
        {
            base.gameObject.layer = LayerIndex.noCollision.intVal;

            Vector3 aimDirection = base.transform.forward;
            Vector3 rhs = Vector3.Cross(Vector3.up, aimDirection);
            Vector3 axis = Vector3.Cross(aimDirection, rhs);

            float angle = 0f;
            float num2 = 10f;   //Bandit is x2
            angle = num2 / (float)FlakShotgunComponent.shotCount;  //5 - 1 rockets

            Vector3 direction = Quaternion.AngleAxis(-num2 * 0.5f, axis) * aimDirection;
            Quaternion rotation = Quaternion.AngleAxis(angle, axis);
            Ray aimRay2 = new Ray(base.transform.position, direction);
            for (int i = 0; i < FlakShotgunComponent.shotCount; i++)
            {
                aimDirection = aimRay2.direction;

                ProjectileManager.instance.FireProjectile(FlakShotgunComponent.projectilePrefab, aimRay2.origin, Util.QuaternionSafeLookRotation(aimDirection), pc.owner,
                    damageMult * pd.damage * damageCoefficient * damageMult, 0f, pd.crit, DamageColorIndex.Default, null, -1f);
                aimRay2.direction = rotation * aimRay2.direction;
            }
        }
    }
}
