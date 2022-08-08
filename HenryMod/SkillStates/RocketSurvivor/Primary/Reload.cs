using RoR2;
using UnityEngine;

namespace EntityStates.RocketSurvivor.Primary
{
	public class Reload : BaseState
	{
		public static float enterSoundPitch = 1f;
		public static float exitSoundPitch = 1f;
		public static string enterSoundString = "Play_bandit2_m1_reload_bullet";
		public static GameObject reloadEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/Bandit2Reload");
		public static float baseDuration = 0.8f;

		private bool hasGivenStock;

		private float duration
		{
			get
			{
				return Reload.baseDuration / this.attackSpeedStat;
			}
		}

		public override void OnEnter()
		{
			base.OnEnter();
			//base.PlayAnimation("Gesture, Additive", (base.characterBody.isSprinting && base.characterMotor && base.characterMotor.isGrounded) ? "ReloadSimple" : "Reload", "Reload.playbackRate", this.duration);
			Util.PlayAttackSpeedSound(Reload.enterSoundString, base.gameObject, Reload.enterSoundPitch);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration / 2f)
			{
				this.GiveStock();
			}
			if (!base.isAuthority || base.fixedAge < this.duration)
			{
				return;
			}
			if (base.skillLocator.primary.stock < base.skillLocator.primary.maxStock)
			{
				this.outer.SetNextState(new Reload());
				return;
			}
			this.outer.SetNextStateToMain();
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		private void GiveStock()
		{
			if (this.hasGivenStock)
			{
				return;
			}
			if (base.isAuthority && base.skillLocator.primary.stock < base.skillLocator.primary.maxStock)
			{
				base.skillLocator.primary.AddOneStock();
			}
			this.hasGivenStock = true;
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}
	}
}
