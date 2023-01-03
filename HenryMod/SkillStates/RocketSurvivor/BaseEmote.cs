using UnityEngine;
using RoR2;
using static RoR2.CameraTargetParams;
using RocketSurvivor.Modules;
using BepInEx.Configuration;
using System;

namespace EntityStates.RocketSurvivorSkills.Emote {
    public class BaseEmote : BaseState {
        public string soundString;
        public string animString;
        public float duration;
        public float animDuration;

        private uint activePlayID;
        private Animator animator;
        private ChildLocator childLocator;

        public LocalUser localUser;

        private CharacterCameraParamsData emoteCameraParams = new CharacterCameraParamsData() {
            maxPitch = 70,
            minPitch = -70,
            pivotVerticalOffset = 0.5f,
            idealLocalCameraPos = emoteCameraPosition,
            wallCushion = 0.1f,
        };

        public static Vector3 emoteCameraPosition = new Vector3(0, -0.5f, -8.9f);

        private CameraParamsOverrideHandle camOverrideHandle;

        public override void OnEnter() {
            base.OnEnter();
            this.animator = base.GetModelAnimator();
            this.childLocator = base.GetModelChildLocator();
            FindLocalUser();
            
            base.characterBody.hideCrosshair = true;

            if (base.GetAimAnimator()) base.GetAimAnimator().enabled = false;
            this.animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 0);
            this.animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 0);

            if(duration == 0 && animDuration == 0) {
                duration = GetAnimatorClipDuration();
            }

            if (this.animDuration == 0 && this.duration != 0) this.animDuration = this.duration;

            if(animDuration > 0) {
                base.PlayAnimation("FullBody, Override", this.animString, "Emote.playbackRate", this.animDuration);
            } else {
                base.PlayAnimation("FullBody, Override", this.animString);
            }

            this.activePlayID = Util.PlaySound(soundString, base.gameObject);

            CameraParamsOverrideRequest request = new CameraParamsOverrideRequest {
                cameraParamsData = emoteCameraParams,
                priority = 0,
            };

            camOverrideHandle = base.cameraTargetParams.AddParamsOverride(request, 0.5f);

        }

        private float GetAnimatorClipDuration() {

            Animator modelAnimator = GetModelAnimator();

            modelAnimator.speed = 1f;
            modelAnimator.Update(0f);
            int layerIndex = modelAnimator.GetLayerIndex("FullBody, Override");
            if (layerIndex >= 0) {
                //modelAnimator.SetFloat(playbackRateParam, 1f);
                modelAnimator.PlayInFixedTime(animString, layerIndex, 0f);
                modelAnimator.Update(0f);
                float length = modelAnimator.GetCurrentAnimatorStateInfo(layerIndex).length;
                return length;
            }
            
            return 0;
        }

        private void FindLocalUser() {
            if (localUser == null) {
                if (base.characterBody) {
                    foreach (LocalUser lu in LocalUserManager.readOnlyLocalUsersList) {
                        if (lu.cachedBody == base.characterBody) {
                            this.localUser = lu;
                            break;
                        }
                    }
                }
            }
        }

        public override void OnExit() {
            base.OnExit();

            base.characterBody.hideCrosshair = false;

            if (base.GetAimAnimator()) base.GetAimAnimator().enabled = true;
            this.animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 1);
            this.animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 1);

            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            if (this.activePlayID != 0) AkSoundEngine.StopPlayingID(this.activePlayID);

            base.cameraTargetParams.RemoveParamsOverride(camOverrideHandle, 0.5f);
        }

        public override void FixedUpdate() {
            base.FixedUpdate();

            bool flag = false;

            if (base.characterMotor) {
                if (!base.characterMotor.isGrounded) flag = true;
                if (base.characterMotor.velocity != Vector3.zero) flag = true;
            }

            if (base.inputBank) {
                if (base.inputBank.skill1.down) flag = true;
                if (base.inputBank.skill2.down) flag = true;
                if (base.inputBank.skill3.down) flag = true;
                if (base.inputBank.skill4.down) flag = true;
                if (base.inputBank.jump.down) flag = true;

                if (base.inputBank.moveVector != Vector3.zero) flag = true;
            }

            //emote cancels
            if (base.isAuthority && base.characterMotor.isGrounded) {

                CheckEmote<Sit>(Config.KeybindEmoteSit);
                CheckEmote<Explode>(Config.KeybindEmoteShovel);
                CheckEmote<MenuPose>(Config.KeybindEmoteCSS);
            }

            if (this.duration > 0 && base.fixedAge >= this.duration) flag = true;

            if (flag) {
                this.outer.SetNextStateToMain();
            }
        }

        private void CheckEmote<T>(ConfigEntry<KeyboardShortcut> keybind) where T : EntityState, new() {
            if (Config.GetKeyPressed(keybind)) {

                FindLocalUser();

                if (localUser != null && !localUser.isUIFocused) {
                    outer.SetInterruptState(new T(), InterruptPriority.Any);
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority() {
            return InterruptPriority.Any;
        }
    }


    public class Sit : BaseEmote {
        public override void OnEnter() {
            this.animString = "TauntSit";
            this.duration = -1;
            base.OnEnter();
        }
    }

    public class Explode : BaseEmote {
        private bool explosion = false;
        public override void OnEnter() {
            this.animString = "TauntShovel";
            base.OnEnter();
        }

        public override void FixedUpdate() {
            base.FixedUpdate();

            if (!explosion && base.fixedAge / this.duration >= (1f / 1f)) {
                if (base.isAuthority)
                {
                    BlastAttack ba = new BlastAttack
                    {
                        attacker = null,
                        attackerFiltering = AttackerFiltering.AlwaysHit,
                        baseDamage = 100f * this.damageStat,
                        baseForce = 8000f,
                        bonusForce = Vector3.zero,
                        canRejectForce = true,
                        crit = base.RollCrit(),
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageType.Stun1s | DamageType.BypassOneShotProtection,
                        falloffModel = BlastAttack.FalloffModel.SweetSpot,
                        inflictor = null,
                        position = base.characterBody.corePosition,
                        procChainMask = default,
                        procCoefficient = 1f,
                        radius = 12f,
                        teamIndex = base.GetTeam()
                    };

                    ba.Fire();

                    EffectManager.SpawnEffect(EntityStates.RocketSurvivorSkills.Secondary.AirDet.explosionEffectPrefab, new EffectData { origin = base.characterBody.corePosition, scale = ba.radius }, true);

                    if (base.characterMotor) base.SmallHop(base.characterMotor, 36f);
                }
            }
        }
    }

    public class MenuPose: BaseEmote
    {
        public static float thumpSoundDelay = 1.5f;
        public static float loadSoundDelay = 8f / 30f;

        private bool playedThump = false;
        private bool playedLoad = false;

        public override void OnEnter()
        {
            this.animString = "TauntCSS";
            this.duration = -1;
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!playedLoad && base.fixedAge >= loadSoundDelay)
            {
                playedLoad = true;
                Util.PlaySound("Play_Moffein_RocketSurvivor_Shift_Rearm", base.gameObject);
            }
            if (!playedThump && base.fixedAge >= thumpSoundDelay)
            {
                playedThump = true;
                Util.PlaySound("Play_MULT_shift_hit", base.gameObject);
            }
        }
    }
}
