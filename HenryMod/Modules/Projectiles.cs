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

            CreateRocketNoBlastJump();
            CreateRocketAltNoBlastJump();
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

            GameObject explosionEffect;
            if (!EntityStates.RocketSurvivorSkills.Primary.FireRocket.explosionEffectPrefab)
            {
                explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFXQuick.prefab").WaitForCompletion().InstantiateClone("RocketSurvivorRocketExplosionVFX", false);
                EffectComponent ec = explosionEffect.GetComponent<EffectComponent>();
                ec.soundName = "Play_Moffein_RocketSurvivor_M1_Explode";
                Modules.Content.AddEffectDef(new EffectDef(explosionEffect));
                EntityStates.RocketSurvivorSkills.Primary.FireRocket.explosionEffectPrefab = explosionEffect;
            }
            else
            {
                explosionEffect = EntityStates.RocketSurvivorSkills.Primary.FireRocket.explosionEffectPrefab;
            }

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
            rocketPrefab.AddComponent<ProjectileImpactBlastJump>();

            DamageAPI.ModdedDamageTypeHolderComponent mdc = rocketPrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(DamageTypes.ScaleForceToMass);
            mdc.Add(DamageTypes.SweetSpotModifier);

            AddProjectile(rocketPrefab);

            EntityStates.RocketSurvivorSkills.Primary.FireRocket.projectilePrefab = rocketPrefab;
        }

        private static void CreateRocketNoBlastJump()
        {
            GameObject rocketPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherProjectile.prefab").WaitForCompletion().InstantiateClone("RocketSurvivorRocketNoBlastJumpProjectile", true);//"RoR2/Base/Drones/PaladinRocket.prefab"

            ProjectileSimple ps = rocketPrefab.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 75f;// 20.96f should be equivalent to tf2 rockets (1100HU/S) but this doesn't seem to be the case in-game.
            ps.lifetime = 20f;

            ProjectileImpactExplosion pie = rocketPrefab.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(pie);

            GameObject explosionEffect;
            if (!EntityStates.RocketSurvivorSkills.Primary.FireRocket.explosionEffectPrefab)
            {
                explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFXQuick.prefab").WaitForCompletion().InstantiateClone("RocketSurvivorRocketExplosionVFX", false);
                EffectComponent ec = explosionEffect.GetComponent<EffectComponent>();
                ec.soundName = "Play_Moffein_RocketSurvivor_M1_Explode";
                Modules.Content.AddEffectDef(new EffectDef(explosionEffect));
                EntityStates.RocketSurvivorSkills.Primary.FireRocket.explosionEffectPrefab = explosionEffect;
            }
            else
            {
                explosionEffect = EntityStates.RocketSurvivorSkills.Primary.FireRocket.explosionEffectPrefab;
            }

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

            DamageAPI.ModdedDamageTypeHolderComponent mdc = rocketPrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(DamageTypes.ScaleForceToMass);
            mdc.Add(DamageTypes.SweetSpotModifier);

            AddProjectile(rocketPrefab);

            EntityStates.RocketSurvivorSkills.Primary.FireRocket.projectilePrefabICBM = rocketPrefab;
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

            GameObject explosionEffect;
            if (!EntityStates.RocketSurvivorSkills.Primary.FireRocketAlt.explosionEffectPrefab)
            {
                explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFXQuick.prefab").WaitForCompletion().InstantiateClone("RocketSurvivorRocketAltExplosionVFX", false);
                EffectComponent ec = explosionEffect.GetComponent<EffectComponent>();
                ec.soundName = "Play_Moffein_RocketSurvivor_M1_Alt_Explode";
                Modules.Content.AddEffectDef(new EffectDef(explosionEffect));
                EntityStates.RocketSurvivorSkills.Primary.FireRocketAlt.explosionEffectPrefab = explosionEffect;
            }
            else
            {
                explosionEffect = EntityStates.RocketSurvivorSkills.Primary.FireRocketAlt.explosionEffectPrefab;
            }

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
            bjc.runOnServer = true;
            //rocketPrefab.AddComponent<ProjectileImpactBlastJump>();   //isPrediction prevents this from running on clients

            DamageAPI.ModdedDamageTypeHolderComponent mdc = rocketPrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(DamageTypes.ScaleForceToMass);
            //mdc.Add(DamageTypes.AirborneBonus);

            if (Modules.Config.samTracking.Value)
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

            ProjectileController pc = rocketPrefab.GetComponent<ProjectileController>();
            pc.allowPrediction = false;

            AddProjectile(rocketPrefab);

            EntityStates.RocketSurvivorSkills.Primary.FireRocketAlt.projectilePrefab = rocketPrefab;
        }

        private static void CreateRocketAltNoBlastJump()
        {
            GameObject rocketPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherProjectile.prefab").WaitForCompletion().InstantiateClone("RocketSurvivorRocketAltNoBlastJumpProjectile", true);//"RoR2/Base/Drones/PaladinRocket.prefab"

            ProjectileSimple ps = rocketPrefab.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 75f * 1.8f;// 20.96f should be equivalent to tf2 rockets (1100HU/S) but this doesn't seem to be the case in-game.
            ps.lifetime = 20f;
            ps.updateAfterFiring = true;

            ProjectileImpactExplosion pie = rocketPrefab.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(pie);

            GameObject explosionEffect;
            if (!EntityStates.RocketSurvivorSkills.Primary.FireRocketAlt.explosionEffectPrefab)
            {
                explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFXQuick.prefab").WaitForCompletion().InstantiateClone("RocketSurvivorRocketAltExplosionVFX", false);
                EffectComponent ec = explosionEffect.GetComponent<EffectComponent>();
                ec.soundName = "Play_Moffein_RocketSurvivor_M1_Alt_Explode";
                Modules.Content.AddEffectDef(new EffectDef(explosionEffect));
                EntityStates.RocketSurvivorSkills.Primary.FireRocketAlt.explosionEffectPrefab = explosionEffect;
            }
            else
            {
                explosionEffect = EntityStates.RocketSurvivorSkills.Primary.FireRocketAlt.explosionEffectPrefab;
            }

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

            DamageAPI.ModdedDamageTypeHolderComponent mdc = rocketPrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(DamageTypes.ScaleForceToMass);
            //mdc.Add(DamageTypes.AirborneBonus);

            if (Modules.Config.samTracking.Value)
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

            ProjectileController pc = rocketPrefab.GetComponent<ProjectileController>();
            pc.allowPrediction = false;

            AddProjectile(rocketPrefab);

            EntityStates.RocketSurvivorSkills.Primary.FireRocketAlt.projectilePrefabICBM = rocketPrefab;
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
            pie.blastRadius = 12f;
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
            bjc.triggerOnImpact = false;
            bjc.blastJumpOnDestroy = false;

            ProjectileDamage pd = c4Projectile.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.Stun1s;

            DamageAPI.ModdedDamageTypeHolderComponent mdc = c4Projectile.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(DamageTypes.ScaleForceToMass);

            ProjectileController pc = c4Projectile.GetComponent<ProjectileController>();
            pc.allowPrediction = false;

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
            //Modules.Assets.ConvertAllRenderersToHopooShader(c4Ghost);
            //Breaks ring
            c4Ghost.AddComponent<ProjectileGhostController>();
            pc.ghostPrefab = c4Ghost;

            AddProjectile(c4Projectile);
            EntityStates.RocketSurvivorSkills.Utility.C4.projectilePrefab = c4Projectile;
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