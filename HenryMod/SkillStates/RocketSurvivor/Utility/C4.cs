using BepInEx.Configuration;
using RoR2;
using RoR2.Projectile;
using System.Runtime.CompilerServices;
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

			Ray aimRay;
			if (RocketSurvivor.RocketSurvivorPlugin.VRAPILoaded)
            {
				aimRay = GetVRAimRay();
            }
			else
            {
				aimRay = base.GetAimRay();
			}				

			base.StartAimMode(aimRay, 3f, false);

			base.PlayAnimation("Gesture, Override", "NitroCharge", "ThrowBomb.playbackRate", this.duration); //TODO: REPLACE
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

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private Ray GetVRAimRay()
        {
			Ray aimRay = base.GetAimRay();

			if (VRAPI.Utils.IsInVR(this) && base.characterBody && VRAPI.Utils.IsUsingMotionControls(base.characterBody))
            {
				aimRay = vrUseOffhand.Value ? VRAPI.MotionControls.nonDominantHand.aimRay : VRAPI.MotionControls.dominantHand.aimRay;
            }

			return aimRay;
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
		public static float damageCoefficient = 12f;
		public static float force = 2400f;
		public static ConfigEntry<bool> vrUseOffhand;

		public static float baseDuration = 0.8f;
		public static float baseMinDuration = 0.2f;

		private float minDuration;
		private float duration;
		private bool buttonReleased = false;
	}
}
