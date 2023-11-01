using EntityStates.RocketSurvivorSkills.Primary;
using RocketSurvivor.Modules.Survivors;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.RocketSurvivorSkills.Special
{
    public class FireAllRockets : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            fireStopwatch = 0f;
            delayBetweenShots = FireAllRockets.baseDelayBetweenShots / base.attackSpeedStat;
            shotsRemaining = base.skillLocator && base.skillLocator.primary ? base.skillLocator.primary.maxStock : FireAllRockets.baseShotCount;
            isCrit = base.RollCrit();

            SkillDef selectedPrimarySkill;
            if (base.skillLocator)
            {
                selectedPrimarySkill = base.skillLocator.primary.baseSkill;
            }
            else
            {
                selectedPrimarySkill = RocketSurvivorSetup.FireRocketSkillDef;
            }
            selectedRocketSkill = FindRocketSkillInfo(selectedPrimarySkill);

            ModifyStats();
        }

        public virtual void ModifyStats() { }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fireStopwatch <= 0f)
            {
                fireStopwatch += delayBetweenShots;
                if(shotsRemaining > 0) FireProjectile();
            }
            fireStopwatch -= Time.fixedDeltaTime;

            if (base.isAuthority)
            {
                if (base.skillLocator)
                {
                    if (shotsRemaining <= 0)//(base.skillLocator.primary.stock <= 0)
                    {
                        this.outer.SetNextState(new Rearm());
                        return;
                    }
                }
            }
        }

        public void FireProjectile()
        {
            if (base.isAuthority)
            {
                /*if (base.skillLocator && base.skillLocator.primary.stock > 0)
                {
                    base.skillLocator.primary.DeductStock(1);
                }*/

                Ray aimRay = base.GetAimRay();

                Vector3 aimDirection = aimRay.direction;


                float damageMult = RocketSurvivor.RocketSurvivorPlugin.GetICBMDamageMult(base.characterBody);

                //Copied from Bandit2
                if (RocketSurvivor.Modules.Config.pocketICBM.Value&& base.characterBody && base.characterBody.inventory && base.characterBody.inventory.GetItemCount(DLC1Content.Items.MoreMissile) > 0)
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
                        aimDirection = aimRay2.direction;
                        if (shotsRemaining != baseShotCount)
                        {
                            //aimDirection = Util.ApplySpread(aimRay2.direction, 0f, 3f, 1f, 1f);
                        }

                        bool centerRocket = i == 1 || RocketSurvivor.Modules.Config.pocketICBMEnableKnockback.Value;
                        ProjectileManager.instance.FireProjectile(GetProjectilePrefab(centerRocket), aimRay2.origin, Util.QuaternionSafeLookRotation(aimDirection), base.gameObject, damageMult * this.damageStat * GetDamageCoefficient(), (centerRocket ? GetForce() * 0.25f : 0f), isCrit, DamageColorIndex.Default, null, -1f);
                        aimRay2.direction = rotation * aimRay2.direction;
                    }
                }
                else
                {
                    if (shotsRemaining != baseShotCount)
                    {
                        //aimDirection = Util.ApplySpread(aimRay.direction, 0f, 3f, 1f, 1f);
                    }
                    ProjectileManager.instance.FireProjectile(GetProjectilePrefab(), aimRay.origin, Util.QuaternionSafeLookRotation(aimDirection), base.gameObject, this.damageStat * GetDamageCoefficient(), GetForce() * 0.25f, isCrit, DamageColorIndex.Default, null, -1f);
                }


                base.StartAimMode(aimRay, 3f, false);
            }

            GameObject effectPrefab = GetEffectPrefab();
            if (effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(effectPrefab, base.gameObject, GetMuzzleString(), false);
            }
            //"SpecialShoot" for the one with the exhuast anim
            base.PlayAnimation("Gesture, Override", "SpecialShoot"/*, "Shoot.playbackRate", 0.169f*/);
            Util.PlaySound(GetAttackSoundString(), base.gameObject);
            shotsRemaining--;
        }

        private string GetAttackSoundString()
        {
            if (selectedRocketSkill != null) return selectedRocketSkill.attackSoundString;
            return FireRocket.attackSoundString;
        }

        private GameObject GetProjectilePrefab(bool enableKnockback = true)
        {
            if (selectedRocketSkill == null) return null;
            if (enableKnockback)
            {
                return selectedRocketSkill.projectilePrefab;
            }
            else
            {
               return selectedRocketSkill.projectilePrefabICBM;
            }
        }

        private GameObject GetEffectPrefab()
        {
            if (selectedRocketSkill != null) return selectedRocketSkill.effectPrefab;
            return null;
        }

        private string GetMuzzleString()
        {
            if (selectedRocketSkill != null) return selectedRocketSkill.muzzleString;
            return string.Empty;
        }


        private float GetDamageCoefficient()
        {
            if (selectedRocketSkill != null) return selectedRocketSkill.damageCoefficient;
            return 0f;
        }

        private float GetForce()
        {
            if (selectedRocketSkill != null) return selectedRocketSkill.force;
            return 0f;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }

        private RocketSkillInfo selectedRocketSkill;
        private bool isCrit;
        private float fireStopwatch;

        public float delayBetweenShots;
        public int shotsRemaining;

        public static int baseShotCount = 4;
        public static float baseDelayBetweenShots = 0.2f;

        public static List<RocketSkillInfo> rocketSkillInfoList = new List<RocketSkillInfo>();
        public class RocketSkillInfo
        {
            public SkillDef skillDef;

            public GameObject projectilePrefab;
            public GameObject projectilePrefabICBM; //Extra rockets fired when using ICBM. Has blast jump component disabled.

            public GameObject effectPrefab;
            public string muzzleString;
            public string attackSoundString;
            public float damageCoefficient;
            public float force;
        }
        public static RocketSkillInfo FindRocketSkillInfo(SkillDef sd)
        {
            foreach(RocketSkillInfo rs in rocketSkillInfoList)
            {
                if (rs.skillDef == sd) return rs;
            }
            return null;
        }
    }
}
