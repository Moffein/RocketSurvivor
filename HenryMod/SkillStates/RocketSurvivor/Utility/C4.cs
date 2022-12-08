using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.RocketSurvivorSkills.Utility
{
	public class C4 : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = C4.baseDuration / this.attackSpeedStat;
			this.minDuration = C4.baseMinDuration / this.attackSpeedStat;
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 3f, false);

			base.PlayAnimation("Gesture, Override", "ThrowBomb", "ThrowBomb.playbackRate", this.duration); //TODO: REPLACE
			Util.PlaySound(attackSoundString, base.gameObject);

			if (C4.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(C4.effectPrefab, base.gameObject, muzzleString, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(C4.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * C4.damageCoefficient, C4.force, false, DamageColorIndex.Default, null, -1f);
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			if (!buttonReleased && base.inputBank && !base.inputBank.skill3.down)
			{
				buttonReleased = true;
			}

			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return (base.fixedAge >= this.minDuration && buttonReleased) ? InterruptPriority.Skill : InterruptPriority.Pain;
		}

		public static string muzzleString = "MuzzleCenter";
		public static string attackSoundString = "Play_commando_M2_grenade_throw";//"Play_Moffein_RocketSurvivor_R_Shoot";//"Play_MULT_m1_grenade_launcher_shoot";
		public static GameObject projectilePrefab;
		public static GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/MuzzleflashFMJ.prefab").WaitForCompletion();    //Use a less threatening VFX for this
		public static float damageCoefficient = 13f;
		public static float force = 2400f;

		public static float baseDuration = 0.8f;
		public static float baseMinDuration = 0.2f;

		private float minDuration;
		private float duration;
		private bool buttonReleased = false;
	}
}
