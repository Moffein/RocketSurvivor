using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using RoR2.Projectile;
using RocketSurvivor.Components.Projectile;
using R2API;
using RocketSurvivor.Modules.Survivors;

namespace RocketSurvivor.Components {
    public class RocketTrackerComponent : NetworkBehaviour
    {
        private List<RocketInfo> rocketList;
        private List<RocketInfo> c4List;
        private SkillLocator skillLocator;

        public static NetworkSoundEventDef detonateSuccess;
        public static NetworkSoundEventDef detonateFail;

        //[SyncVar]
        //private bool _rocketAvailable = false;

        public void Awake()
        {
            c4List = new List<RocketInfo>();
            rocketList = new List<RocketInfo>();
            skillLocator = base.GetComponent<SkillLocator>();
        }

        public void FixedUpdate()
        {
            ClearEmptyRockets();
            /*if (NetworkServer.active)
            {
                UpdateRocketAvailable();
            }*/
        }

        public void OnDestroy()
        {
            if (NetworkServer.active && c4List != null)
            {
                foreach (RocketInfo ri in c4List)
                {
                    if (ri.gameObject) Destroy(ri.gameObject);
                }
            }
        }

        private void ClearEmptyRockets()
        {
            c4List.RemoveAll(item => item.gameObject == null);
            rocketList.RemoveAll(item => item.gameObject == null);
        }

        /*[Server]
        private void UpdateRocketAvailable()
        {
            if (!NetworkServer.active) return;
            bool newRocketAvailable = false;

            if ((rocketList.Count + c4List.Count) > 0)
            {
                newRocketAvailable = true;
            }

            if (newRocketAvailable != _rocketAvailable)
            {
                _rocketAvailable = newRocketAvailable;
            }
        }*/

        public bool IsRocketAvailable()
        {
            return rocketList.Count + c4List.Count > 0;
            //return _rocketAvailable;
        }

        public void AddRocket(GameObject rocket, bool applyAirDetBuff, bool isC4 = false)
        {
            RocketInfo info = new RocketInfo(rocket, applyAirDetBuff, isC4);
            if (info.isC4)
            {
                int c4InList = c4List.Count;
                int maxC4 = 1;
                if (this.skillLocator && this.skillLocator.utility)
                {
                    maxC4 = Mathf.Max(maxC4, this.skillLocator.utility.maxStock);
                }
                if (c4InList >= maxC4)
                {
                    RocketInfo oldestC4 = c4List.FirstOrDefault<RocketInfo>();
                    c4List.Remove(oldestC4);

                    if (oldestC4.gameObject)
                    {
                        BlastJumpComponent bjc = oldestC4.gameObject.GetComponent<BlastJumpComponent>();
                        if (bjc)
                        {
                            bjc.BlastJumpServer();
                        }
                    }

                    DetonateRocketInfo(oldestC4);
                }
                c4List.Add(info);
            }
            else
            {
                rocketList.Add(info);
            }
        }

        private bool DetonateRocketInfo(RocketInfo info)
        {
            bool detonatedSuccessfully = false;
            GameObject toDetonate = info.gameObject;
            if (!toDetonate) return false;

            ProjectileDamage pd = toDetonate.GetComponent<ProjectileDamage>();
            ProjectileController pc = toDetonate.GetComponent<ProjectileController>();
            ProjectileImpactExplosion pie = toDetonate.GetComponent<ProjectileImpactExplosion>();
            TeamFilter tf = toDetonate.GetComponent<TeamFilter>();
            BlastJumpComponent bjc = toDetonate.GetComponent<BlastJumpComponent>();

            if (bjc && bjc.runOnServer)
            {
                float origAoe = bjc.aoe;
                float origForce = bjc.force;
                if (info.applyAirDetBonus)
                {
                    bjc.aoe *= EntityStates.RocketSurvivorSkills.Secondary.AirDet.radiusMult;
                    bjc.force *= EntityStates.RocketSurvivorSkills.Secondary.AirDet.forceMult;
                }
                bjc.AttemptBlastJump();

                //Reset these in case it attempts to stack after a failed detonation or something weird.
                bjc.aoe = origAoe;
                bjc.force = origForce;
            }

            if (pc && pie)
            {
                if (tf && pd && pc.owner)
                {
                    BlastAttack ba = new BlastAttack
                    {
                        attacker = pc.owner,
                        attackerFiltering = AttackerFiltering.NeverHitSelf,
                        baseDamage = pd.damage * pie.blastDamageCoefficient,
                        baseForce = pd.force,
                        bonusForce = pie.bonusBlastForce,
                        canRejectForce = pie.canRejectForce,
                        crit = pd.crit,
                        damageColorIndex = (!info.isC4 && pd.damageColorIndex == DamageColorIndex.Default) ? DamageColorIndex.WeakPoint : pd.damageColorIndex,
                        damageType = pd.damageType,
                        falloffModel = BlastAttack.FalloffModel.None,
                        inflictor = pc.owner,
                        position = toDetonate.transform.position,
                        procChainMask = default,
                        procCoefficient = pie.blastProcCoefficient,
                        radius = pie.blastRadius,
                        teamIndex = tf.teamIndex
                    };

                    if (info.applyAirDetBonus)
                    {
                        ba.baseForce *= EntityStates.RocketSurvivorSkills.Secondary.AirDet.forceMult;
                        ba.bonusForce *= EntityStates.RocketSurvivorSkills.Secondary.AirDet.forceMult;
                        ba.baseDamage *= EntityStates.RocketSurvivorSkills.Secondary.AirDet.damageMult;
                        ba.radius = Mathf.Max(ba.radius * EntityStates.RocketSurvivorSkills.Secondary.AirDet.radiusMult, EntityStates.RocketSurvivorSkills.Secondary.AirDet.minRadius);
                    }

                    DamageAPI.ModdedDamageTypeHolderComponent mdc = toDetonate.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
                    if (mdc)
                    {
                        if (mdc.Has(DamageTypes.ScaleForceToMass)) ba.AddModdedDamageType(DamageTypes.ScaleForceToMass);
                        if (mdc.Has(DamageTypes.AirborneBonus)) ba.AddModdedDamageType(DamageTypes.AirborneBonus);
                    }

                    GameObject effectPrefab = EntityStates.RocketSurvivorSkills.Secondary.AirDet.explosionEffectPrefab;
                    if (pd.damageType.HasFlag(DamageType.Silent) && pd.damageType.HasFlag(DamageType.Stun1s))
                    {
                        effectPrefab = EntityStates.RocketSurvivorSkills.Secondary.AirDet.concExplosionEffectPrefab;
                    }

                    EffectManager.SpawnEffect(effectPrefab, new EffectData { origin = toDetonate.transform.position, scale = ba.radius }, true);

                    ba.Fire();
                    detonatedSuccessfully = true;
                }
            }
            Destroy(toDetonate);
            return detonatedSuccessfully;
        }

        [Server]
        public bool DetonateRocket()
        {
            bool detonatedSuccessfully = false;
            if (NetworkServer.active)
            {
                if (this.IsRocketAvailable())
                {
                    foreach (RocketInfo info in rocketList)
                    {
                        bool detonate = DetonateRocketInfo(info);
                        detonatedSuccessfully = detonatedSuccessfully || detonate;
                    }

                    foreach (RocketInfo info in c4List)
                    {
                        bool detonate = DetonateRocketInfo(info);
                        detonatedSuccessfully = detonatedSuccessfully || detonate;
                    }
                }
                ClearEmptyRockets();
                //UpdateRocketAvailable();
            }
            return detonatedSuccessfully;
        }

        [ClientRpc]
        public void RpcAddSecondaryStock()
        {
            if (!this.hasAuthority) return;
            if (skillLocator & skillLocator.secondary.stock < skillLocator.secondary.maxStock)
            {
                skillLocator.secondary.AddOneStock();
            }
        }

        public void ServerDetonateRocket()
        {
            if (NetworkServer.active)
            {
                bool success = DetonateRocket();
                EffectManager.SimpleSoundEffect(success ? detonateSuccess.index : detonateFail.index, base.transform.position, true); //Play a sound to let the player know if they whiffed on the server.
                //No need for this anymore since the detonation check is Clientside and ensures that you'll at least get to blast jump if you trigger it.
                /*if (!success)
                {
                    RpcAddSecondaryStock();   
                }*/
            }
        }

        public void ClientDetonateBlastJump()
        {
            foreach (RocketInfo info in rocketList)
            {
                TriggerBlastJump(info);
            }

            foreach (RocketInfo info in c4List)
            {
                TriggerBlastJump(info);
            }
        }

        private void TriggerBlastJump(RocketInfo info)
        {
            if (!info.gameObject) return;
            GameObject toDetonate = info.gameObject;
            BlastJumpComponent bjc = toDetonate.GetComponent<BlastJumpComponent>();

            if (!bjc || bjc.runOnServer) return;

            float origAoe = bjc.aoe;
            float origForce = bjc.force;
            if (info.applyAirDetBonus)
            {
                bjc.aoe *= EntityStates.RocketSurvivorSkills.Secondary.AirDet.radiusMult;
                bjc.force *= EntityStates.RocketSurvivorSkills.Secondary.AirDet.forceMult;
            }
            bjc.AttemptBlastJump();

            //Reset these in case it attempts to stack after a failed detonation or something weird.
            bjc.aoe = origAoe;
            bjc.force = origForce;
        }

        public class RocketInfo
        {
            public GameObject gameObject;
            public bool applyAirDetBonus;
            public bool isC4;

            public RocketInfo(GameObject gameObject, bool applyAirDetBonus, bool isC4 = false)
            {
                this.gameObject = gameObject;
                this.applyAirDetBonus = applyAirDetBonus;
                this.isC4 = isC4;
            }
        }
    }
}
