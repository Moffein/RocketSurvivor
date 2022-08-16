using R2API;
using RocketSurvivor.Components.Projectile;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RocketSurvivor.Modules
{
    internal static class Projectiles
    {
        internal static void RegisterProjectiles()
        {
            CreateRocket();
            CreateRocketAlt();
            CreateConcRocket();
        }

        internal static void AddProjectile(GameObject projectileToAdd)
        {
            Modules.Content.AddProjectilePrefab(projectileToAdd);
        }

        private static void CreateRocket()
        {
            GameObject rocketPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/PaladinRocket.prefab").WaitForCompletion().InstantiateClone("RocketSurvivorRocketProjectile", true);

            ProjectileSimple ps = rocketPrefab.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 75f;// 20.96f should be equivalent to tf2 rockets (1100HU/S) but this doesn't seem to be the case in-game.
            ps.lifetime = 20f;

            ProjectileImpactExplosion pie = rocketPrefab.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(pie);

            GameObject explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFX.prefab").WaitForCompletion().InstantiateClone("RocketSurvivorRocketExplosionVFX", false);
            EffectComponent ec = explosionEffect.GetComponent<EffectComponent>();
            ec.soundName = "Play_Moffein_RocketSurvivor_M1_Explode";
            Modules.Content.AddEffectDef(new EffectDef(explosionEffect));

            pie.blastDamageCoefficient = 1f;
            pie.blastRadius = 8f;
            pie.destroyOnEnemy = true;
            pie.destroyOnWorld = true;
            pie.lifetime = 12f;
            pie.impactEffect = explosionEffect;
            pie.timerAfterImpact = false;
            pie.lifetimeAfterImpact = 0f;
            pie.blastAttackerFiltering = AttackerFiltering.NeverHitSelf;
            pie.falloffModel = BlastAttack.FalloffModel.SweetSpot;

            //Remove built-in sounds
            AkEvent[] akEvents = rocketPrefab.GetComponentsInChildren<AkEvent>();
            for (int i = 0; i < akEvents.Length; i++)
            {
                UnityEngine.Object.Destroy(akEvents[i]);
            }

            AkGameObj akgo = rocketPrefab.GetComponent<AkGameObj>();
            if (akgo)
            {
                UnityEngine.Object.Destroy(akgo);
            }

            rocketPrefab.AddComponent<AddToRocketTrackerComponent>();
            BlastJumpComponent bjc = rocketPrefab.AddComponent<BlastJumpComponent>();
            bjc.force = 2000f;
            bjc.horizontalMultiplier = 1.5f;
            //bjc.aoe = 8f;

            DamageAPI.ModdedDamageTypeHolderComponent mdc = rocketPrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(DamageTypes.ScaleForceToMass);

            AddProjectile(rocketPrefab);

            EntityStates.RocketSurvivorSkills.Primary.FireRocket.projectilePrefab = rocketPrefab;
        }

        //Use a different rocket model for this.
        private static void CreateRocketAlt()
        {
            GameObject rocketPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/PaladinRocket.prefab").WaitForCompletion().InstantiateClone("RocketSurvivorRocketAltProjectile", true);

            ProjectileSimple ps = rocketPrefab.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 75f * 1.8f;// 20.96f should be equivalent to tf2 rockets (1100HU/S) but this doesn't seem to be the case in-game.
            ps.lifetime = 20f;

            ProjectileImpactExplosion pie = rocketPrefab.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(pie);

            GameObject explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFX.prefab").WaitForCompletion().InstantiateClone("RocketSurvivorRocketAltExplosionVFX", false);
            EffectComponent ec = explosionEffect.GetComponent<EffectComponent>();
            ec.soundName = "Play_Moffein_RocketSurvivor_M1_Alt_Explode";
            Modules.Content.AddEffectDef(new EffectDef(explosionEffect));

            pie.blastDamageCoefficient = 1f;
            pie.blastRadius = 4f;
            pie.destroyOnEnemy = true;
            pie.destroyOnWorld = true;
            pie.lifetime = 12f;
            pie.impactEffect = explosionEffect;
            pie.timerAfterImpact = false;
            pie.lifetimeAfterImpact = 0f;
            pie.blastAttackerFiltering = AttackerFiltering.NeverHitSelf;
            pie.falloffModel = BlastAttack.FalloffModel.None;   //Sweetspot becomes unreliable at low radius.

            //Remove built-in sounds
            AkEvent[] akEvents = rocketPrefab.GetComponentsInChildren<AkEvent>();
            for (int i = 0; i < akEvents.Length; i++)
            {
                UnityEngine.Object.Destroy(akEvents[i]);
            }

            AkGameObj akgo = rocketPrefab.GetComponent<AkGameObj>();
            if (akgo)
            {
                UnityEngine.Object.Destroy(akgo);
            }

            rocketPrefab.AddComponent<AddToRocketTrackerComponent>();
            BlastJumpComponent bjc = rocketPrefab.AddComponent<BlastJumpComponent>();
            bjc.force = 2000f;
            bjc.horizontalMultiplier = 1.5f;
            bjc.aoe = 8f;  //Keep the Rocket Jump AoE the same for consistency

            DamageAPI.ModdedDamageTypeHolderComponent mdc = rocketPrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(DamageTypes.ScaleForceToMass);
            mdc.Add(DamageTypes.AirborneBonus);

            AddProjectile(rocketPrefab);

            EntityStates.RocketSurvivorSkills.Primary.FireRocketAlt.projectilePrefab = rocketPrefab;
        }

        //Use a model that's less harmful looking.
        private static void CreateConcRocket()
        {
            GameObject rocketPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherProjectile.prefab").WaitForCompletion().InstantiateClone("RocketSurvivorConcProjectile", true);

            ProjectileSimple ps = rocketPrefab.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 100f;
            ps.lifetime = 20f;

            ProjectileImpactExplosion pie = rocketPrefab.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(pie);

            GameObject explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerMineExplosion.prefab").WaitForCompletion();

            pie.blastDamageCoefficient = 1f;
            pie.blastRadius = 12f;
            pie.destroyOnEnemy = true;
            pie.destroyOnWorld = true;
            pie.lifetime = 12f;
            pie.impactEffect = explosionEffect;
            pie.timerAfterImpact = false;
            pie.lifetimeAfterImpact = 0f;
            pie.blastAttackerFiltering = AttackerFiltering.NeverHitSelf;
            pie.falloffModel = BlastAttack.FalloffModel.SweetSpot;

            //Remove built-in sounds
            AkEvent[] akEvents = rocketPrefab.GetComponentsInChildren<AkEvent>();
            for (int i = 0; i < akEvents.Length; i++)
            {
                UnityEngine.Object.Destroy(akEvents[i]);
            }

            AkGameObj akgo = rocketPrefab.GetComponent<AkGameObj>();
            if (akgo)
            {
                UnityEngine.Object.Destroy(akgo);
            }

            rocketPrefab.AddComponent<AddToRocketTrackerComponent>();
            BlastJumpComponent bjc = rocketPrefab.AddComponent<BlastJumpComponent>();
            bjc.force = 3000f;
            bjc.horizontalMultiplier = 1.5f;
            bjc.requireAirborne = false;

            ProjectileDamage pd = rocketPrefab.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.Stun1s | DamageType.Silent;

            DamageAPI.ModdedDamageTypeHolderComponent mdc = rocketPrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(DamageTypes.ScaleForceToMass);

            AddProjectile(rocketPrefab);

            EntityStates.RocketSurvivorSkills.Utility.ConcRocket.projectilePrefab = rocketPrefab;
        }

        private static void InitializeImpactExplosion(ProjectileImpactExplosion projectileImpactExplosion)
        {
            projectileImpactExplosion.blastDamageCoefficient = 1f;
            projectileImpactExplosion.blastProcCoefficient = 1f;
            projectileImpactExplosion.blastRadius = 1f;
            projectileImpactExplosion.bonusBlastForce = Vector3.zero;
            projectileImpactExplosion.childrenCount = 0;
            projectileImpactExplosion.childrenDamageCoefficient = 0f;
            projectileImpactExplosion.childrenProjectilePrefab = null;
            projectileImpactExplosion.destroyOnEnemy = false;
            projectileImpactExplosion.destroyOnWorld = false;
            projectileImpactExplosion.falloffModel = RoR2.BlastAttack.FalloffModel.None;
            projectileImpactExplosion.fireChildren = false;
            projectileImpactExplosion.impactEffect = null;
            projectileImpactExplosion.lifetime = 0f;
            projectileImpactExplosion.lifetimeAfterImpact = 0f;
            projectileImpactExplosion.lifetimeRandomOffset = 0f;
            projectileImpactExplosion.offsetForLifetimeExpiredSound = 0f;
            projectileImpactExplosion.timerAfterImpact = false;

            projectileImpactExplosion.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
        }

        private static GameObject CreateGhostPrefab(string ghostName)
        {
            GameObject ghostPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName);
            if (!ghostPrefab.GetComponent<NetworkIdentity>()) ghostPrefab.AddComponent<NetworkIdentity>();
            if (!ghostPrefab.GetComponent<ProjectileGhostController>()) ghostPrefab.AddComponent<ProjectileGhostController>();

            Modules.Assets.ConvertAllRenderersToHopooShader(ghostPrefab);

            return ghostPrefab;
        }

        private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/" + prefabName), newPrefabName);
            return newPrefab;
        }
    }
}