using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.RocketSurvivor.Primary
{
    public class FireRocket : BaseState
    {
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireRocket.baseDuration / this.attackSpeedStat;
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
			string muzzleName = "MuzzleCenter";

			base.PlayAnimation("LeftArm, Override", "ShootGun", "ShootGun.playbackRate", 1.8f);	//TODO: REPLACE
			Util.PlaySound(attackSoundString, base.gameObject);

			if (FireRocket.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireRocket.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireRocket.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireRocket.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
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
			return InterruptPriority.Skill;
		}

		public static string attackSoundString = "Play_MULT_m1_grenade_launcher_shoot";
		public static GameObject projectilePrefab;
		public static GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/MuzzleflashFMJ.prefab").WaitForCompletion();
		public static float damageCoefficient = 3.9f;
		public static float force = 2000f;
		public static float baseDuration = 0.8f;

		private float duration;
	}
}
