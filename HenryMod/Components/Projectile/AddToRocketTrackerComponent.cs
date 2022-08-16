using RoR2;
using UnityEngine;
using RoR2.Projectile;
using UnityEngine.Networking;

namespace RocketSurvivor.Components.Projectile
{
    public class AddToRocketTrackerComponent : MonoBehaviour
    {
        public void Start()
        {
            if (NetworkServer.active)
            {
                ProjectileController pc = base.GetComponent<ProjectileController>();
                if (pc && pc.owner)
                {
                    RocketTrackerComponent rtc = pc.owner.GetComponent<RocketTrackerComponent>();
                    if (rtc)
                    {
                        rtc.AddRocket(base.gameObject);
                    }
                }
            }
            Destroy(this);
        }
    }
}
