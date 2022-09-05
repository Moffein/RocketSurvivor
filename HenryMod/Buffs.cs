using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RocketSurvivor
{
    public class Buffs
    {
        public static BuffDef RocketJumpSpeedBuff;
        public static BuffDef AirshotVulnerableDebuff;

        public static bool initialized = false;

        public static void Initialize()
        {
            if (initialized) return;

            BuffDef vanillaSpeedBuff = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/SprintOutOfCombat/bdWhipBoost.asset").WaitForCompletion();    //Steal icon + color from here
            RocketJumpSpeedBuff = CreateBuffDef("RocketSurvivorRocketJumpSpeedBuff", false, false, false, new Color(0.376f, 0.843f, 0.898f), vanillaSpeedBuff.iconSprite);


            BuffDef vanillaCritBuff = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/CritOnUse/bdFullCrit.asset").WaitForCompletion();    //Steal icon + color from here
            AirshotVulnerableDebuff = CreateBuffDef("RocketSurvivorAirshotVulnerableDebuff", false, false, true, Modules.Survivors.RocketSurvivorSetup.RocketSurvivorColor, vanillaCritBuff.iconSprite);

            R2API.RecalculateStatsAPI.GetStatCoefficients += (sender, args) =>
            {
                if (sender.HasBuff(RocketJumpSpeedBuff))
                {
                    args.moveSpeedMultAdd += 0.4f;
                }

                if (sender.HasBuff(AirshotVulnerableDebuff))
                {
                    args.armorAdd -= 40f;
                }
            };

            //Remove the Rocket Jump Speed buff upon touching the ground
            On.RoR2.CharacterMotor.FixedUpdate += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active && self.isGrounded && self.body)
                {
                    if (self.body.HasBuff(RocketJumpSpeedBuff))
                    {
                        self.body.RemoveBuff(RocketJumpSpeedBuff);
                    }
                }
            };

            //Adjust Rocket Jump physics
            IL.RoR2.CharacterMotor.PreMove += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.After,
                    x => x.MatchCall<CharacterMotor>("get_walkSpeed")
                    );
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, CharacterMotor, float>>((walkSpeed, self) =>
                {
                    if (self.body && self.body.HasBuff(RocketJumpSpeedBuff))
                    {
                        Vector2 currentVelocity = new Vector2(self.velocity.x, self.velocity.z);
                        float targetSpeed = currentVelocity.magnitude;

                        //Scale targetspeed between current speed and walking speed, based on movement input compared to current velocity direction.
                        if (targetSpeed > walkSpeed)
                        {
                            Vector2 newDirection = new Vector2(self.moveDirection.x, self.moveDirection.z);

                            float angle = Vector2.Angle(currentVelocity, newDirection);
                            float lerp = angle > 135f ? 1f : 1f - angle / 135f; //Allow for gradual turning without losing speed.

                            walkSpeed = Mathf.Lerp(walkSpeed, targetSpeed, lerp);
                        }
                    }

                    return walkSpeed;
                });

            };

            initialized = true;
        }

        public static BuffDef CreateBuffDef(string name, bool canStack, bool isCooldown, bool isDebuff, Color color, Sprite iconSprite)
        {
            BuffDef bd = ScriptableObject.CreateInstance<BuffDef>();
            bd.name = name;
            bd.canStack = canStack;
            bd.isCooldown = isCooldown;
            bd.isDebuff = isDebuff;
            bd.buffColor = color;
            bd.iconSprite = iconSprite;
            Modules.Content.AddBuffDef(bd);

            (bd as UnityEngine.Object).name = bd.name;
            return bd;
        }
    }
}
