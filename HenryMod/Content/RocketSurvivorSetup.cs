using BepInEx.Configuration;
using EntityStates;
using RocketSurvivor.Modules.Characters;
using RocketSurvivor.Components;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using R2API;

namespace RocketSurvivor.Modules.Survivors
{
    internal class RocketSurvivorSetup : SurvivorBase
    {
        public override string bodyName => "Rocket";

        public const string Rocket_Prefix = RocketSurvivorPlugin.DEVELOPER_PREFIX + "_ROCKET_BODY_";
        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => Rocket_Prefix;

        public static SkillDef FireRocketSkillDef, FireRocketAltSkillDef, AirDetDef;
        public static Color RocketSurvivorColor = new Color(62f / 255f, 137f / 255f, 72f / 255f);

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "RocketSurvivorBody",
            bodyNameToken = RocketSurvivorPlugin.DEVELOPER_PREFIX + "_ROCKET_BODY_NAME",
            subtitleNameToken = RocketSurvivorPlugin.DEVELOPER_PREFIX + "_ROCKET_BODY_SUBTITLE",

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texIconRocket"),
            bodyColor = RocketSurvivorColor,

            crosshair = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherCrosshair.prefab").WaitForCompletion(),
            podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 110f,
            healthRegen = 1f,
            armor = 0f,
            damage = 12f,

            jumpCount = 1,
        };

        public override CustomRendererInfo[] customRendererInfos { get; set; } 
        //    = new CustomRendererInfo[] 
        //{
        //        new CustomRendererInfo
        //        {
        //            childName = "SwordModel",
        //            material = Materials.CreateHopooMaterial("matHenry"),
        //        },
        //        new CustomRendererInfo
        //        {
        //            childName = "GunModel",
        //        },
        //        new CustomRendererInfo
        //        {
        //            childName = "Model",
        //        }
        //};

        public override UnlockableDef characterUnlockableDef => null;

        public override Type characterMainState => typeof(EntityStates.GenericCharacterMain);

        public override ItemDisplaysBase itemDisplays => new HenryItemDisplays();

                                                                          //if you have more than one character, easily create a config to enable/disable them like this
        public override ConfigEntry<bool> characterEnabledConfig => null; //Modules.Config.CharacterEnableConfig(bodyName);

        private static UnlockableDef masterySkinUnlockableDef;

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
            base.bodyPrefab.AddComponent<RocketTrackerComponent>();
        }

        public override void InitializeUnlockables()
        {
            //uncomment this when you have a mastery skin. when you do, make sure you have an icon too
            //masterySkinUnlockableDef = Modules.Unlockables.AddUnlockable<Modules.Achievements.MasteryAchievement>();
        }

        public override void InitializeHitboxes()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;
            
            //example of how to create a hitbox
            Transform hitboxTransform = childLocator.FindChild("ShovelHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "Sword");
        }

        public override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);

            //Passive
            SkillLocator sk = bodyPrefab.GetComponent<SkillLocator>();
            sk.passiveSkill.enabled = true;
            sk.passiveSkill.skillNameToken = Rocket_Prefix + "PASSIVE_NAME";
            sk.passiveSkill.skillDescriptionToken = Rocket_Prefix + "PASSIVE_DESCRIPTION";
            sk.passiveSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSkillPassive");

            #region Primary
            ReloadSkillDef primarySkillDef = ReloadSkillDef.CreateInstance<ReloadSkillDef>();
            primarySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.RocketSurvivorSkills.Primary.FireRocket));
            primarySkillDef.activationStateMachineName = "Weapon";
            primarySkillDef.baseMaxStock = 4;
            primarySkillDef.baseRechargeInterval = 0f;
            primarySkillDef.beginSkillCooldownOnSkillEnd = false;
            primarySkillDef.canceledFromSprinting = false;
            primarySkillDef.dontAllowPastMaxStocks = true;
            primarySkillDef.forceSprintDuringState = false;
            primarySkillDef.fullRestockOnAssign = true;
            primarySkillDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSkillPrimary");
            primarySkillDef.interruptPriority = InterruptPriority.Skill;
            primarySkillDef.isCombatSkill = true;
            primarySkillDef.keywordTokens = new string[] { "KEYWORD_AGILE" };
            primarySkillDef.mustKeyPress = false;
            primarySkillDef.cancelSprintingOnActivation = false;
            primarySkillDef.rechargeStock = 0;
            primarySkillDef.requiredStock = 1;
            primarySkillDef.skillName = "FireRocket";
            primarySkillDef.skillNameToken = Rocket_Prefix + "PRIMARY_NAME";
            primarySkillDef.skillDescriptionToken = Rocket_Prefix + "PRIMARY_DESCRIPTION";
            primarySkillDef.stockToConsume = 1;
            primarySkillDef.graceDuration = 0.4f;
            primarySkillDef.reloadState = new SerializableEntityStateType(typeof(EntityStates.RocketSurvivorSkills.Primary.EnterReload));
            primarySkillDef.reloadInterruptPriority = InterruptPriority.Any;
            (primarySkillDef as ScriptableObject).name = "FireRocket";
            Modules.Content.AddSkillDef(primarySkillDef);

            ReloadSkillDef primaryAltSkillDef = ReloadSkillDef.CreateInstance<ReloadSkillDef>();
            primaryAltSkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.RocketSurvivorSkills.Primary.FireRocketAlt));
            primaryAltSkillDef.activationStateMachineName = "Weapon";
            primaryAltSkillDef.baseMaxStock = 4;
            primaryAltSkillDef.baseRechargeInterval = 0f;
            primaryAltSkillDef.beginSkillCooldownOnSkillEnd = false;
            primaryAltSkillDef.canceledFromSprinting = false;
            primaryAltSkillDef.dontAllowPastMaxStocks = true;
            primaryAltSkillDef.forceSprintDuringState = false;
            primaryAltSkillDef.fullRestockOnAssign = true;
            primaryAltSkillDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSkillPrimary_DirectHit");
            primaryAltSkillDef.interruptPriority = InterruptPriority.Skill;
            primaryAltSkillDef.isCombatSkill = true;
            primaryAltSkillDef.keywordTokens = new string[] { "KEYWORD_AGILE" };
            primaryAltSkillDef.mustKeyPress = false;
            primaryAltSkillDef.cancelSprintingOnActivation = false;
            primaryAltSkillDef.rechargeStock = 0;
            primaryAltSkillDef.requiredStock = 1;
            primaryAltSkillDef.skillName = "FireRocketAlt";
            primaryAltSkillDef.skillNameToken = Rocket_Prefix + "PRIMARY_ALT_NAME";
            primaryAltSkillDef.skillDescriptionToken = Rocket_Prefix + "PRIMARY_ALT_DESCRIPTION";
            primaryAltSkillDef.stockToConsume = 1;
            primaryAltSkillDef.graceDuration = 0.4f;
            primaryAltSkillDef.reloadState = new SerializableEntityStateType(typeof(EntityStates.RocketSurvivorSkills.Primary.EnterReload));
            primaryAltSkillDef.reloadInterruptPriority = InterruptPriority.Any;
            (primaryAltSkillDef as ScriptableObject).name = "FireRocketAlt";
            Modules.Content.AddSkillDef(primaryAltSkillDef);

            RocketSurvivorSetup.FireRocketSkillDef = primarySkillDef;
            RocketSurvivorSetup.FireRocketAltSkillDef = primaryAltSkillDef;
            Modules.Skills.AddPrimarySkills(bodyPrefab, new SkillDef[] { primarySkillDef, primaryAltSkillDef });
            #endregion

            #region Secondary

            RocketTrackerSkillDef airDetTrackerDef = RocketTrackerSkillDef.CreateInstance<RocketTrackerSkillDef>();
            airDetTrackerDef.activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.RocketSurvivorSkills.Secondary.AirDet));
            airDetTrackerDef.activationStateMachineName = "Slide";
            airDetTrackerDef.baseMaxStock = 1;
            airDetTrackerDef.baseRechargeInterval = 3f;
            airDetTrackerDef.beginSkillCooldownOnSkillEnd = false;
            airDetTrackerDef.canceledFromSprinting = false;
            airDetTrackerDef.dontAllowPastMaxStocks = true;
            airDetTrackerDef.forceSprintDuringState = false;
            airDetTrackerDef.fullRestockOnAssign = true;
            airDetTrackerDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSkillSecondary");
            airDetTrackerDef.interruptPriority = InterruptPriority.Skill;
            airDetTrackerDef.isCombatSkill = false;
            airDetTrackerDef.keywordTokens = new string[] { };
            airDetTrackerDef.mustKeyPress = false;
            airDetTrackerDef.cancelSprintingOnActivation = false;
            airDetTrackerDef.rechargeStock = 1;
            airDetTrackerDef.requiredStock = 1;
            airDetTrackerDef.skillName = "AirDet";
            airDetTrackerDef.skillNameToken = Rocket_Prefix + "SECONDARY_NAME";
            airDetTrackerDef.skillDescriptionToken = Rocket_Prefix + "SECONDARY_DESCRIPTION";
            airDetTrackerDef.stockToConsume = 1;
            (airDetTrackerDef as ScriptableObject).name = "AirDet";
            Modules.Content.AddSkillDef(airDetTrackerDef);

            Modules.Skills.AddSecondarySkills(bodyPrefab, airDetTrackerDef);
            RocketSurvivorSetup.AirDetDef = airDetTrackerDef;

            NetworkSoundEventDef detSuccessSound = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            detSuccessSound.eventName = "Play_Moffein_RocketSurvivor_M2_Trigger";
            Modules.Content.AddNetworkSoundEventDef(detSuccessSound);
            RocketTrackerComponent.detonateSuccess = detSuccessSound;

            NetworkSoundEventDef detFailSound = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            detFailSound.eventName = "Play_Moffein_RocketSurvivor_M2_NoFire";
            Modules.Content.AddNetworkSoundEventDef(detFailSound);
            RocketTrackerComponent.detonateFail = detFailSound;

            GameObject airDetEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFX.prefab").WaitForCompletion().InstantiateClone("RocketSurvivorAirDetVFX", false);
            EffectComponent ec = airDetEffect.GetComponent<EffectComponent>();
            ec.soundName = "Play_Moffein_RocketSurvivor_M1_Explode";  //Play_MULT_m2_main_explode
            Modules.Content.AddEffectDef(new EffectDef(airDetEffect));
            EntityStates.RocketSurvivorSkills.Secondary.AirDet.explosionEffectPrefab = airDetEffect;
            #endregion

            #region Utility
            SkillDef concDef = SkillDef.CreateInstance<SkillDef>();
            concDef.activationState = new SerializableEntityStateType(typeof(EntityStates.RocketSurvivorSkills.Utility.ConcRocket));
            concDef.activationStateMachineName = "Weapon";
            concDef.baseMaxStock = 1;
            concDef.baseRechargeInterval = 5f;
            concDef.beginSkillCooldownOnSkillEnd = false;
            concDef.canceledFromSprinting = false;
            concDef.dontAllowPastMaxStocks = true;
            concDef.forceSprintDuringState = false;
            concDef.fullRestockOnAssign = true;
            concDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSkillUtility_Conc");
            concDef.interruptPriority = InterruptPriority.PrioritySkill;
            concDef.isCombatSkill = true;
            concDef.keywordTokens = new string[] { };
            concDef.mustKeyPress = false;
            concDef.cancelSprintingOnActivation = false;
            concDef.rechargeStock = 1;
            concDef.requiredStock = 1;
            concDef.skillName = "FireConcRocket";
            concDef.skillNameToken = Rocket_Prefix + "UTILITY_NAME";
            concDef.skillDescriptionToken = Rocket_Prefix + "UTILITY_DESCRIPTION";
            concDef.stockToConsume = 1;
            (concDef as ScriptableObject).name = "FireConcRocket";
            Modules.Content.AddSkillDef(concDef);

            SkillDef marketGardenDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "MarketGarden",
                skillNameToken = Rocket_Prefix + "UTILITY_ALT_NAME",
                skillDescriptionToken = Rocket_Prefix + "UTILITY_ALT_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSkillUtility_Shovel"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.RocketSurvivorSkills.Utility.ComicallyLargeSpoon)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 5f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_STUNNING" }
            });
            (marketGardenDef as ScriptableObject).name = "MarketGarden";
            Modules.Content.AddSkillDef(marketGardenDef);

            Modules.Skills.AddUtilitySkills(bodyPrefab, new SkillDef[] { concDef, marketGardenDef });

            #endregion

            #region Special

            SkillDef rearmDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Rearm",
                skillNameToken = Rocket_Prefix + "SPECIAL_NAME",
                skillDescriptionToken = Rocket_Prefix + "SPECIAL_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSkillSpecial"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.RocketSurvivorSkills.Special.FireAllRockets)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 10f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });
            (rearmDef as ScriptableObject).name = "Rearm";
            Modules.Content.AddSkillDef(rearmDef);
            Modules.Skills.AddSpecialSkills(bodyPrefab, rearmDef);
            #endregion
        }

        public override void InitializeSkins()
        {
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = characterModel.mainSkinnedMeshRenderer;

            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(RocketSurvivorPlugin.DEVELOPER_PREFIX + "_HENRY_BODY_DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRenderers,
                mainRenderer,
                model);

            defaultSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                //place your mesh replacements here
                //unnecessary if you don't have multiple skins
                //new SkinDef.MeshReplacement
                //{
                //    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshHenrySword"),
                //    renderer = defaultRenderers[0].renderer
                //},
                //new SkinDef.MeshReplacement
                //{
                //    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshHenryGun"),
                //    renderer = defaultRenderers[1].renderer
                //},
                //new SkinDef.MeshReplacement
                //{
                //    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshHenry"),
                //    renderer = defaultRenderers[2].renderer
                //}
            };

            skins.Add(defaultSkin);
            #endregion

            //uncomment this when you have a mastery skin
            #region MasterySkin
            /*
            Material masteryMat = Modules.Materials.CreateHopooMaterial("matHenryAlt");
            CharacterModel.RendererInfo[] masteryRendererInfos = SkinRendererInfos(defaultRenderers, new Material[]
            {
                masteryMat,
                masteryMat,
                masteryMat,
                masteryMat
            });

            SkinDef masterySkin = Modules.Skins.CreateSkinDef(HenryPlugin.DEVELOPER_PREFIX + "_HENRY_BODY_MASTERY_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                masteryRendererInfos,
                mainRenderer,
                model,
                masterySkinUnlockableDef);

            masterySkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshHenrySwordAlt"),
                    renderer = defaultRenderers[0].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshHenryAlt"),
                    renderer = defaultRenderers[2].renderer
                }
            };

            skins.Add(masterySkin);
            */
            #endregion

            skinController.skins = skins.ToArray();
        }
    }
}