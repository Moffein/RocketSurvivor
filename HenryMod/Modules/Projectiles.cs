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
            CreateC4();
            CreateFlak();
        }

        internal static void AddProjectile(GameObject projectileToAdd)
        {
            Modules.Content.AddProjectilePrefab(projectileToAdd);
        }

        private static void CreateRocket()
        {
            GameObject rocketPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherProjectile.prefab").WaitForCompletion().InstantiateClone("RocketSurvivorRocketProjectile", true);//"RoR2/Base/Drones/PaladinRocket.prefab"

            ProjectileSimple ps = rocketPrefab.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 75f;// 20.96f should be equivalent to tf2 rockets (1100HU/S) but this doesn't seem to be the case in-game.
            ps.lifetime = 20f;

            ProjectileImpactExplosion pie = rocketPrefab.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(pie);

            GameObject explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFXQuick.prefab").WaitForCompletion().InstantiateClone("RocketSurvivorRocketExplosionVFX", false);
            EffectComponent ec = explosionEffect.GetComponent<EffectComponent>();
            ec.soundName = "Play_Moffein_RocketSurvivor_M1_Explode";
            Modules.Content.AddEffectDef(new EffectDef(explosionEffect));
            EntityStates.RocketSurvivorSkills.Primary.FireRocket.explosionEffectPrefab = explosionEffect;

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
            /*AkEvent[] akEvents = rocketPrefab.GetComponentsInChildren<AkEvent>();
            for (int i = 0; i < akEvents.Length; i++)
            {
                UnityEngine.Object.Destroy(akEvents[i]);
            }

            AkGameObj akgo = rocketPrefab.GetComponent<AkGameObj>();
            if (akgo)
            {
                UnityEngine.Object.Destroy(akgo);
            }*/

            rocketPrefab.AddComponent<AddToRocketTrackerComponent>();
            BlastJumpComponent bjc = rocketPrefab.AddComponent<BlastJumpComponent>();
            bjc.force = 2000f;
            bjc.horizontalMultiplier = 1.5f;
            bjc.aoe = 8f;
            bjc.requireAirborne = true;

            DamageAPI.ModdedDamageTypeHolderComponent mdc = rocketPrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(DamageTypes.ScaleForceToMass);

            AddProjectile(rocketPrefab);

            EntityStates.RocketSurvivorSkills.Primary.FireRocket.projectilePrefab = rocketPrefab;
        }

        //Use a different rocket model for this.
        private static void CreateRocketAlt()
        {
            GameObject rocketPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherProjectile.prefab").WaitForCompletion().InstantiateClone("RocketSurvivorRocketAltProjectile", true);//"RoR2/Base/Drones/PaladinRocket.prefab"

            ProjectileSimple ps = rocketPrefab.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 75f * 1.8f;// 20.96f should be equivalent to tf2 rockets (1100HU/S) but this doesn't seem to be the case in-game.
            ps.lifetime = 20f;
            ps.updateAfterFiring = true;

            ProjectileImpactExplosion pie = rocketPrefab.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(pie);

            GameObject explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFXQuick.prefab").WaitForCompletion().InstantiateClone("RocketSurvivorRocketAltExplosionVFX", false);
            EffectComponent ec = explosionEffect.GetComponent<EffectComponent>();
            ec.soundName = "Play_Moffein_RocketSurvivor_M1_Alt_Explode";
            Modules.Content.AddEffectDef(new EffectDef(explosionEffect));
            EntityStates.RocketSurvivorSkills.Primary.FireRocketAlt.explosionEffectPrefab = explosionEffect;

            pie.blastDamageCoefficient = 1f;
            pie.blastRadius = 4.5f;   //Artificer is 2
            pie.destroyOnEnemy = true;
            pie.destroyOnWorld = true;
            pie.lifetime = 12f;
            pie.impactEffect = explosionEffect;
            pie.timerAfterImpact = false;
            pie.lifetimeAfterImpact = 0f;
            pie.blastAttackerFiltering = AttackerFiltering.NeverHitSelf;
            pie.falloffModel = BlastAttack.FalloffModel.None;   //Sweetspot becomes unreliable at low radius.

            //Remove built-in sounds
            /*AkEvent[] akEvents = rocketPrefab.GetComponentsInChildren<AkEvent>();
            for (int i = 0; i < akEvents.Length; i++)
            {
                UnityEngine.Object.Destroy(akEvents[i]);
            }

            AkGameObj akgo = rocketPrefab.GetComponent<AkGameObj>();
            if (akgo)
            {
                UnityEngine.Object.Destroy(akgo);
            }*/

            rocketPrefab.AddComponent<AddToRocketTrackerComponent>();
            BlastJumpComponent bjc = rocketPrefab.AddComponent<BlastJumpComponent>();
            bjc.force = 2000f;
            bjc.horizontalMultiplier = 1.5f;
            bjc.aoe = 8f;  //Keep the Rocket Jump AoE the same for consistency
            bjc.requireAirborne = true;

            DamageAPI.ModdedDamageTypeHolderComponent mdc = rocketPrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(DamageTypes.ScaleForceToMass);
            mdc.Add(DamageTypes.AirborneBonus);

            if (RocketSurvivorPlugin.samTracking)
            {
                rocketPrefab.AddComponent<ProjectileTargetComponent>();
                ProjectileSteerTowardTarget pstt = rocketPrefab.AddComponent<ProjectileSteerTowardTarget>();
                pstt.yAxisOnly = false;
                pstt.rotationSpeed = 60f;   //90f

                ProjectileDirectionalTargetFinder2 pdtf = rocketPrefab.AddComponent<ProjectileDirectionalTargetFinder2>();
                pdtf.lookRange = 60f;   //25f
                pdtf.lookCone = 12f;    //20f
                pdtf.targetSearchInterval = 0.1f;
                pdtf.onlySearchIfNoTarget = true;
                pdtf.allowTargetLoss = false;
                pdtf.testLoS = true;
                pdtf.ignoreAir = false;
                pdtf.flierAltitudeTolerance = Mathf.Infinity;
                pdtf.SearchMode = BullseyeSearch.SortMode.Angle;
            }

            AddProjectile(rocketPrefab);

            EntityStates.RocketSurvivorSkills.Primary.FireRocketAlt.projectilePrefab = rocketPrefab;
        }

        private static void CreateC4()
        {
            GameObject c4Projectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoGrenadeProjectile.prefab").WaitForCompletion().InstantiateClone("RocketSurvivorC4Projectile", true);

            ProjectileSimple ps = c4Projectile.GetComponent<ProjectileSimple>();
            //ps.desiredForwardSpeed = 100f;
            ps.lifetime = 9999999f;

            GameObject c4Effect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFX.prefab").WaitForCompletion().InstantiateClone("RocketSurvivorConcEffect", false);
            EffectComponent ec = c4Effect.GetComponent<EffectComponent>();
            ec.soundName = "Play_Moffein_RocketSurvivor_R_Flak_Explode";
            Content.AddEffectDef(new EffectDef(c4Effect));

            ProjectileImpactExplosion pie = c4Projectile.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(pie);

            pie.blastDamageCoefficient = 1f;
            pie.blastRadius = 18f;
            pie.destroyOnEnemy = false;
            pie.destroyOnWorld = false;
            pie.lifetime = 9999999f;
            pie.impactEffect = c4Effect;
            pie.timerAfterImpact = false;
            pie.blastAttackerFiltering = AttackerFiltering.NeverHitSelf;
            pie.falloffModel = BlastAttack.FalloffModel.None;

            AddToRocketTrackerComponent atr = c4Projectile.AddComponent<AddToRocketTrackerComponent>();
            atr.applyAirDetBonus = false;
            atr.isC4 = true;

            BlastJumpComponent bjc = c4Projectile.AddComponent<BlastJumpComponent>();
            bjc.force = 3600f;
            bjc.horizontalMultiplier = 1f;
            bjc.requireAirborne = false;

            ProjectileDamage pd = c4Projectile.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.Stun1s;

            DamageAPI.ModdedDamageTypeHolderComponent mdc = c4Projectile.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(DamageTypes.ScaleForceToMass);

            ProjectileController pc = c4Projectile.GetComponent<ProjectileController>();

            ProjectileStickOnImpact pst = c4Projectile.AddComponent<ProjectileStickOnImpact>();
            pst.alignNormals = true;
            pst.stickSoundString = "Play_Moffein_RocketSurvivor_C4_Land";

            Collider[] existingColliders = c4Projectile.GetComponentsInChildren<Collider>();
            foreach (Collider c in existingColliders)
            {
                UnityEngine.Object.Destroy(c);
            }

            BoxCollider bc = c4Projectile.AddComponent<BoxCollider>();
            bc.size = new Vector3(0.5f, 0.3f, 0.7f);

            GameObject c4Ghost = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlC4").InstantiateClone("RocketSurvivorC4Ghost", false);
            c4Ghost.layer = LayerIndex.noCollision.intVal;
            Modules.Assets.ConvertAllRenderersToHopooShader(c4Ghost);
            c4Ghost.AddComponent<ProjectileGhostController>();
            pc.ghostPrefab = c4Ghost;

            AddProjectile(c4Projectile);
            EntityStates.RocketSurvivorSkills.Utility.C4.projectilePrefab = c4Projectile;
        }

        private static void CreateFlak()
        {
            GameObject explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFX.prefab").WaitForCompletion().InstantiateClone("RocketSurvivorFlakExplosionVFX", false);
            EffectComponent ec = explosionEffect.GetComponent<EffectComponent>();
            ec.soundName = "Play_Moffein_RocketSurvivor_R_Flak_Explode";
            Modules.Content.AddEffectDef(new EffectDef(explosionEffect));
            EntityStates.RocketSurvivorSkills.Special.FireFlak.explosionEffectPrefab = explosionEffect;

            CreateFlakMini();

            GameObject flakProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/PaladinRocket.prefab").WaitForCompletion().InstantiateClone("RocketSurvivorFlakProjectile", true);

            flakProjectile.AddComponent<AddToRocketTrackerComponent>();
            flakProjectile.AddComponent<FlakShotgunComponent>();
            BlastJumpComponent bjc = flakProjectile.AddComponent<BlastJumpComponent>();
            bjc.force = 2400f;
            bjc.horizontalMultiplier = 1.5f;
            bjc.aoe = 10f;
            bjc.requireAirborne = true;

            ProjectileImpactExplosion pie = flakProjectile.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(pie);

            pie.blastDamageCoefficient = 1f;
            pie.blastRadius = 10f;
            pie.destroyOnEnemy = true;
            pie.destroyOnWorld = true;
            pie.lifetime = 0.5f;
            pie.impactEffect = EntityStates.RocketSurvivorSkills.Special.FireFlak.explosionEffectPrefab;
            pie.timerAfterImpact = false;
            pie.lifetimeAfterImpact = 0f;
            pie.blastAttackerFiltering = AttackerFiltering.NeverHitSelf;
            pie.falloffModel = BlastAttack.FalloffModel.SweetSpot;

            ProjectileSimple ps = flakProjectile.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 100f;
            ps.lifetime = 1f;

            //Remove built-in sounds
            AkEvent[] akEvents = flakProjectile.GetComponentsInChildren<AkEvent>();
            for (int i = 0; i < akEvents.Length; i++)
            {
                UnityEngine.Object.Destroy(akEvents[i]);
            }

            AkGameObj akgo = flakProjectile.GetComponent<AkGameObj>();
            if (akgo)
            {
                UnityEngine.Object.Destroy(akgo);
            }

            AddProjectile(flakProjectile);
            EntityStates.RocketSurvivorSkills.Special.FireFlak.projectilePrefab = flakProjectile;
        }

        private static void CreateFlakMini()
        {
            GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/PaladinRocket.prefab").WaitForCompletion().InstantiateClone("RocketSurvivorFlakMiniProjectile", true);
            projectilePrefab.transform.localScale *= 0.3f;
            ProjectileImpactExplosion pie = projectilePrefab.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(pie);

            pie.blastDamageCoefficient = 1f;
            pie.blastRadius = 6f;
            pie.destroyOnEnemy = true;
            pie.destroyOnWorld = true;
            pie.lifetime = 4f;
            pie.impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFXQuick.prefab").WaitForCompletion();
            pie.timerAfterImpact = false;
            pie.lifetimeAfterImpact = 0f;
            pie.blastAttackerFiltering = AttackerFiltering.NeverHitSelf;
            pie.falloffModel = BlastAttack.FalloffModel.None;

            ProjectileSimple ps = projectilePrefab.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 100f;
            ps.lifetime = 5f;

            //Remove built-in sounds
            AkEvent[] akEvents = projectilePrefab.GetComponentsInChildren<AkEvent>();
            for (int i = 0; i < akEvents.Length; i++)
            {
                UnityEngine.Object.Destroy(akEvents[i]);
            }

            AkGameObj akgo = projectilePrefab.GetComponent<AkGameObj>();
            if (akgo)
            {
                UnityEngine.Object.Destroy(akgo);
            }

            AddProjectile(projectilePrefab);
            FlakShotgunComponent.projectilePrefab = projectilePrefab;
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