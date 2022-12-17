using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.RocketSurvivorSkills.Primary
{
	public class FireRocketAlt : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireRocket.baseDuration / this.attackSpeedStat;
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 3f, false);

			base.PlayAnimation("Gesture, Override", "Shoot"/*, "Shoot.playbackRate", 0.169f*/);
			Util.PlaySound(attackSoundString, base.gameObject);

			if (FireRocketAlt.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireRocketAlt.effectPrefab, base.gameObject, muzzleString, false);
			}
			if (base.isAuthority)
			{
				float damageMult = RocketSurvivor.RocketSurvivorPlugin.GetICBMDamageMult(base.characterBody);

				//Copied from Bandit2
				if (RocketSurvivor.RocketSurvivorPlugin.pocketICBM && base.characterBody && base.characterBody.inventory && base.characterBody.inventory.GetItemCount(DLC1Content.Items.MoreMissile) > 0)
				{
					Vector3 rhs = Vector3.Cross(Vector3.up, aimRay.direction);
					Vector3 axis = Vector3.Cross(aimRay.direction, rhs);

					float currentSpread = 0f;
					float angle = 0f;
					float num2 = 0f;
					num2 = UnityEngine.Random.Range(1f + currentSpread, 1f + currentSpread) * 3f;   //Bandit is x2
					angle = num2 / 2f;  //3 - 1 rockets

					Vector3 direction = Quaternion.AngleAxis(-num2 * 0.5f, axis) * aimRay.direction;
					Quaternion rotation = Quaternion.AngleAxis(angle, axis);
					Ray aimRay2 = new Ray(aimRay.origin, direction);
					for (int i = 0; i < 3; i++)
					{
						ProjectileManager.instance.FireProjectile(FireRocketAlt.projectilePrefab, aimRay2.origin, Util.QuaternionSafeLookRotation(aimRay2.direction), base.gameObject, damageMult * this.damageStat * FireRocket.damageCoefficient, ((i != 1 && !RocketSurvivor.RocketSurvivorPlugin.pocketICBMEnableKnockback) ? 0f : FireRocket.force), base.RollCrit(), DamageColorIndex.Default, null, -1f);
						aimRay2.direction = rotation * aimRay2.direction;
					}
				}
				else
				{
					ProjectileManager.instance.FireProjectile(FireRocketAlt.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireRocket.damageCoefficient, FireRocket.force, base.RollCrit(), DamageColorIndex.Default, null, -1f);
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
		public static string muzzleString = "MuzzleCenter";
		public static string attackSoundString = "Play_Moffein_RocketSurvivor_M1_Alt_Shoot";
		public static GameObject projectilePrefab;
		public static GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/MuzzleflashFMJ.prefab").WaitForCompletion();
		public static float damageCoefficient = 6f;
		public static float force = 2250f;
		public static float baseDuration = 0.8f;

		private float duration;
	}
}
