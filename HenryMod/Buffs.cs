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
            RocketJumpSpeedBuff = CreateBuffDef("RocketSurvivorRocketJumpSpeedBuff", false, false, false, new Color(0.376f, 0.843f, 0.898f), vanillaSpeedBuff.iconSprite);//Give a bonus to move speed when affected by the buff.


            BuffDef vanillaCritBuff = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/CritOnUse/bdFullCrit.asset").WaitForCompletion();    //Steal icon + color from here
            AirshotVulnerableDebuff = CreateBuffDef("RocketSurvivorAirshotVulnerableDebuff", false, false, true, Modules.Survivors.RocketSurvivorSetup.RocketSurvivorColor, vanillaCritBuff.iconSprite);//Give a bonus to move speed when affected by the buff.

            //Need to do a stacking speed boost since RoR2 physics cancel out the force taken from rocket jumps, preventing pogos/combos. Seems clunky.
            R2API.RecalculateStatsAPI.GetStatCoefficients += (sender, args) =>
            {
                int rjCount = sender.GetBuffCount(RocketJumpSpeedBuff);
                args.moveSpeedMultAdd += 0.4f * rjCount;

                if (sender.HasBuff(AirshotVulnerableDebuff))
                {
                    args.armorAdd -= 40f;
                }
            };

            //Remove the buff upon touching the ground
            On.RoR2.CharacterMotor.FixedUpdate += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active && self.isGrounded && self.body)
                {
                    if (self.body.HasBuff(RocketJumpSpeedBuff))
                    {
                        /*int buffCount = self.body.GetBuffCount(RocketJumpSpeedBuff);
                        for (int i = 0; i < buffCount; i++)
                        {
                            self.body.RemoveBuff(RocketJumpSpeedBuff);
                        }*/
                        //self.body.ClearTimedBuffs(RocketJumpSpeedBuff);
                        self.body.RemoveBuff(RocketJumpSpeedBuff);
                    }
                }
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
