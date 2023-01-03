using UnityEngine;
using RocketSurvivor;
using RoR2;
using BepInEx.Configuration;
using RocketSurvivor.Modules;
using System;
using EntityStates.RocketSurvivorSkills.Emote;

namespace EntityStates.RocketSurvivorSkills {

    public class RocketMainState : GenericCharacterMain
    {
        private Animator animator;

        public LocalUser localUser;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = base.GetModelAnimator();
            FindLocalUser();
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

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.characterBody)
            {
                if (animator)
                {
                    animator.SetFloat("RocketJump", base.characterBody.HasBuff(Buffs.RocketJumpSpeedBuff) ? 1f : 0f);
                }
            }

            if (base.isAuthority && base.characterMotor.isGrounded)
            {
                CheckEmote<Sit>(Config.KeybindEmoteSit);
                CheckEmote<Explode>(Config.KeybindEmoteShovel);
                CheckEmote<MenuPose>(Config.KeybindEmoteCSS);
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
    }
}
