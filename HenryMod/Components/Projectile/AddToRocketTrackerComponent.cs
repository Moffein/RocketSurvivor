﻿using RoR2;
using RoR2.Projectile;
using UnityEngine;
using RoR2.Projectile;
using UnityEngine.Networking;

namespace RocketSurvivor.Components.Projectile
{
    public class AddToRocketTrackerComponent : MonoBehaviour
    {
        public bool applyAirDetBonus = true;
        public bool isC4 = false;

        public void Start()
        {
            ProjectileController pc = base.GetComponent<ProjectileController>();
            if (pc && pc.owner)
            {
                RocketTrackerComponent rtc = pc.owner.GetComponent<RocketTrackerComponent>();
                if (rtc)
                {
                    rtc.AddRocket(base.gameObject, applyAirDetBonus, isC4); //Add on both client and server so clients can immediately trigger blast jump from M2.
                }
            }
            Destroy(this);
        }
    }
}
