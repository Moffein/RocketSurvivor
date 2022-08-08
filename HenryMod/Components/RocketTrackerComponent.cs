using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace RocketSurvivor.Components
{
    public class RocketTrackerComponent : NetworkBehaviour
    {
        private List<GameObject> rocketList;

        public void Awake()
        {
            rocketList = new List<GameObject>();
        }
    }
}
