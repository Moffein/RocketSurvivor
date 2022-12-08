using Mono.Cecil.Cil;
using MonoMod.Cil;
using RocketSurvivor.Components;
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

        public static bool initialized = false;

        public static void Initialize()
        {
            if (initialized) return;

            BuffDef vanillaSpeedBuff = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/SprintOutOfCombat/bdWhipBoost.asset").WaitForCompletion();    //Steal icon + color from here
            RocketJumpSpeedBuff = CreateBuffDef("RocketSurvivorRocketJumpSpeedBuff", false, false, false, new Color(0.376f, 0.843f, 0.898f), vanillaSpeedBuff.iconSprite);

            R2API.RecalculateStatsAPI.GetStatCoefficients += (sender, args) =>
            {
                if (sender.HasBuff(RocketJumpSpeedBuff))
                {
                    args.moveSpeedMultAdd += 0.4f;
                }
            };

            //Remove the Rocket Jump Speed buff upon touching the ground
            On.RoR2.CharacterMotor.FixedUpdate += (orig, self) =>
            {
                orig(self);
                if (self.hasAuthority && self.isGrounded && self.body)
                {
                    if (self.body.HasBuff(RocketJumpSpeedBuff))
                    {
                        NetworkedBodyBlastJumpHandler nb = self.GetComponent<NetworkedBodyBlastJumpHandler>();
                        if (nb)
                        {
                            nb.CmdRemoveRocketJumpBuff();
                        }
                    }
                }
            };

            //Remove Rocket Jump Speed buff when jumping, since jumping resets momentum
            On.RoR2.CharacterMotor.Jump += (orig, self, hMult, vMult, vault) =>
            {
                orig(self, hMult, vMult, vault);
                if (self.hasAuthority && self.body.HasBuff(RocketJumpSpeedBuff))
                {
                    NetworkedBodyBlastJumpHandler nb = self.GetComponent<NetworkedBodyBlastJumpHandler>();
                    if (nb)
                    {
                        nb.CmdRemoveRocketJumpBuff();
                    }
                }
            };

            //Adjust Rocket Jump physics
            //no need to nullcheck self.body since it is a [RequiredComponent] of CharacterMotor
            IL.RoR2.CharacterMotor.PreMove += (il) =>
            {
                ILCursor c = new ILCursor(il);

                //Conserve momentum if no move keys are pressed
                c.GotoNext(MoveType.After,
                    x => x.MatchCall<CharacterMotor>("get_moveDirection")
                    );
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Vector3, CharacterMotor, Vector3>>((moveDirection, self) =>
                {
                    if (self.body.HasBuff(RocketJumpSpeedBuff))
                    {
                        if (moveDirection.x == 0f && moveDirection.z == 0f)
                        {
                            moveDirection = self.velocity;
                            moveDirection.y = 0f;
                            moveDirection.Normalize();
                        }
                    }
                    return moveDirection;
                });


                //Add momentum conservation
                c.GotoNext(MoveType.After,
                    x => x.MatchCall<CharacterMotor>("get_walkSpeed")
                    );
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, CharacterMotor, float>>((walkSpeed, self) =>
                {
                    if (self.body.HasBuff(RocketJumpSpeedBuff))
                    {
                        Vector2 currentVelocity = new Vector2(self.velocity.x, self.velocity.z);
                        float targetSpeed = currentVelocity.magnitude;

                        if (targetSpeed > walkSpeed)
                        {
                            Vector2 newDirection = new Vector2(self.moveDirection.x, self.moveDirection.z);

                            float maxTurnAngle = 45f;
                            float angle = Vector2.Angle(currentVelocity, newDirection);
                            float lerp = (angle <= maxTurnAngle) ? 1f : 1f - ((angle - maxTurnAngle) / (180f - maxTurnAngle)); //Allow for gradual turning without losing speed.

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
