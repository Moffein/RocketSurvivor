﻿using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.RocketSurvivorSkills.Primary
{
    public class FireRocket : BaseState
    {
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireRocket.baseDuration / this.attackSpeedStat;
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 3f, false);

			base.PlayAnimation("Gesture, Override", "Shoot"/*, "Shoot.playbackRate", 0.169f*/);
			Util.PlaySound(attackSoundString, base.gameObject);

			if (FireRocket.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireRocket.effectPrefab, base.gameObject, muzzleString, false);
			}
			if (base.isAuthority)
			{
				float damageMult = RocketSurvivor.RocketSurvivorPlugin.GetICBMDamageMult(base.characterBody);
				DamageTypeCombo damageTypeInternal = DamageTypeCombo.GenericPrimary;
				damageTypeInternal.AddModdedDamageType(RocketSurvivor.DamageTypes.ScaleForceToMass);

				//Copied from Bandit2
				if (RocketSurvivor.Modules.Config.pocketICBM.Value && base.characterBody && base.characterBody.inventory && base.characterBody.inventory.GetItemCount(DLC1Content.Items.MoreMissile) > 0)
                {
					Vector3 rhs = Vector3.Cross(Vector3.up, aimRay.direction);
					Vector3 axis = Vector3.Cross(aimRay.direction, rhs);

					float currentSpread = 0f;
					float angle = 0f;
					float num2 = 0f;
					num2 = UnityEngine.Random.Range(1f + currentSpread, 1f + currentSpread) * 3f;	//Bandit is x2
					angle = num2 / 2f;  //3 - 1 rockets

					Vector3 direction = Quaternion.AngleAxis(-num2 * 0.5f, axis) * aimRay.direction;
					Quaternion rotation = Quaternion.AngleAxis(angle, axis);
					Ray aimRay2 = new Ray(aimRay.origin, direction);
					for (int i = 0; i < 3; i++)
					{
						bool centerRocket = i == 1 || RocketSurvivor.Modules.Config.pocketICBMEnableKnockback.Value;
						ProjectileManager.instance.FireProjectile(centerRocket ? FireRocket.projectilePrefab : FireRocket.projectilePrefabICBM, aimRay2.origin, Util.QuaternionSafeLookRotation(aimRay2.direction), base.gameObject, damageMult * this.damageStat * FireRocket.damageCoefficient, centerRocket ? FireRocket.force : 0f, base.RollCrit(), DamageColorIndex.Default, null, -1f, damageTypeInternal);
						aimRay2.direction = rotation * aimRay2.direction;
					}
				}
				else
				{
					ProjectileManager.instance.FireProjectile(FireRocket.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireRocket.damageCoefficient, FireRocket.force, base.RollCrit(), DamageColorIndex.Default, null, -1f, damageTypeInternal);
				}
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		public static GameObject explosionEffectPrefab;
		public static string muzzleString = "MuzzleRocketLauncher";
		public static string attackSoundString = "Play_Moffein_RocketSurvivor_M1_Shoot";
		public static GameObject projectilePrefab;
		public static GameObject projectilePrefabICBM;
		public static GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/MuzzleflashBarrage.prefab").WaitForCompletion();
		public static float damageCoefficient = 6f;
		public static float force = 2000f;
		public static float baseDuration = 0.8f;

		private float duration;
	}
}
