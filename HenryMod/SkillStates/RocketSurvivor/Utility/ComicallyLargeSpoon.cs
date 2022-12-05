using R2API;
using RocketSurvivor;
using RocketSurvivor.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace EntityStates.RocketSurvivorSkills.Utility
{
    public class PrepComicallyLargeSpoon : BaseState
    {

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlayAttackSpeedSound("Play_Moffein_RocketSurvivor_R_Alt_Prep", base.gameObject, base.attackSpeedStat);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && !(base.inputBank && base.inputBank.skill3.down))
            {
                this.outer.SetNextState(new ComicallyLargeSpoon());
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }

    public class ComicallyLargeSpoon : BaseMeleeAttack
    {
        public override void OnEnter()
        {
            this.hitboxName = "Sword";

            //Just use overlap attack to tell if you're actually hitting something. Damage is dealt via Explosion.
            this.damageType = DamageType.Generic;
            this.damageCoefficient = 0f;
            this.procCoefficient = 0f;
            this.pushForce = 0f;
            this.bonusForce = Vector3.zero;
            this.baseDuration = 0.6f;
            this.attackStartTime = 0f;
            this.attackEndTime = 0.8f;
            this.baseEarlyExitTime = 1f;
            this.hitStopDuration = 0.012f;
            this.attackRecoil = 0.5f;
            this.hitHopVelocity = 36f;

            this.swingSoundString = "Play_Moffein_RocketSurvivor_R_Alt_Swing";
            this.hitSoundString = "Play_Moffein_RocketSurvivor_R_Alt_Hit";
            this.muzzleString = "SwordHitbox";
            this.swingEffectPrefab = RocketSurvivor.Modules.Assets.spoonSwingEffect;// Nullrefs, no clue why. Added null check to BaseMeleeAttack.
            this.hitEffectPrefab = null;//RocketSurvivor.Modules.Assets.spoonImpactEffect;

            this.impactSound = RocketSurvivor.Modules.Assets.spoonHitSoundEvent.index;

            base.OnEnter();
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayAttackAnimation();

            PlayCrossfade("Gesture, Additive", "SwingShovel", "Swing.playbackRate", duration, 0.05f);
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();

            if (!firedExplosion)
            {
                firedExplosion = true;

                float speed = 0f;
                if (base.characterMotor)
                {
                    speed = base.characterMotor.velocity.magnitude;
                }

                if (base.characterBody)
                {
                    BlastAttack ba = new BlastAttack
                    {
                        attacker = base.gameObject,
                        attackerFiltering = AttackerFiltering.NeverHitSelf,
                        baseDamage = (ComicallyLargeSpoon.blastDamageCoefficient + speed * ComicallyLargeSpoon.speedDamageCoefficient) * this.damageStat,
                        baseForce = 2400f,
                        bonusForce = Vector3.zero,
                        canRejectForce = true,
                        crit = false,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageType.Stun1s,
                        falloffModel = BlastAttack.FalloffModel.None,
                        inflictor = base.gameObject,
                        position = base.characterBody.corePosition,
                        procChainMask = default,
                        procCoefficient = 1f,
                        radius = 12f,
                        teamIndex = base.GetTeam()
                    };
                    ba.AddModdedDamageType(DamageTypes.ScaleForceToMass);
                    ba.AddModdedDamageType(DamageTypes.SlamDunk);
                    ba.AddModdedDamageType(DamageTypes.MarketGarden);

                    ba.Fire();

                    EffectManager.SpawnEffect(EntityStates.RocketSurvivorSkills.Secondary.AirDet.explosionEffectPrefab, new EffectData { origin = base.characterBody.corePosition, scale = ba.radius }, true);
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        private bool firedExplosion = false;
        public static float blastDamageCoefficient = 13f;
        public static float speedDamageCoefficient = 0.2f;  //Loader is 0.3
    }
}