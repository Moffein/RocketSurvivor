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
using System.Runtime.CompilerServices;
using System.Linq;
using EntityStates.RocketSurvivorSkills;
using RoR2.CharacterAI;

namespace RocketSurvivor.Modules.Survivors
{
    internal class RocketSurvivorSetup : SurvivorBase
    {
        public override string bodyName => "Rocket";

        public override string cachedName => "RocketSurvivor";

        public const string Rocket_Prefix = RocketSurvivorPlugin.DEVELOPER_PREFIX + "_ROCKET_BODY_";
        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => Rocket_Prefix;

        public static SkillDef FireRocketSkillDef, FireRocketAltSkillDef, AirDetDef, C4Def, ShovelDef;
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
            healthGrowth = 33f,

            healthRegen = 1f,
            regenGrowth = 0.2f,

            armor = 0f,

            damage = 12f,
            damageGrowth = 2.4f,

            jumpCount = 1,
            aimOriginPosition = new Vector3(0f, 1.1f, -0.1f)
        };

        //taken care of by characterbody on prefab
        public override CustomRendererInfo[] customRendererInfos { get; set; } 

        public override UnlockableDef characterUnlockableDef => null;

        public override Type characterMainState => typeof(RocketMainState);

        public override ItemDisplaysBase itemDisplays => new RocketItemDisplays();

                                                                          //if you have more than one character, easily create a config to enable/disable them like this
        public override ConfigEntry<bool> characterEnabledConfig => null; //Modules.Config.CharacterEnableConfig(bodyName);

        //Doesnt use secondary since it has no clue how to use it well.
        public override void InitializeDoppelganger(string clone)
        {
            GameObject doppelganger = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/CommandoMonsterMaster"), bodyName + "MonsterMaster", true);
            doppelganger.GetComponent<CharacterMaster>().bodyPrefab = bodyPrefab;
            Modules.ContentPacks.masterPrefabs.Add(doppelganger);
            Modules.Prefabs.RemoveAISkillDrivers(doppelganger);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Utility", SkillSlot.Utility, RocketSurvivorSetup.ShovelDef,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 8f,
                true, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                true, false, false,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                false,
                false,
                AISkillDriver.ButtonPressType.TapContinuous,
                -1,
                false,
                true,
                null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Utility", SkillSlot.Utility, RocketSurvivorSetup.C4Def,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 20f,
                true, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                true, false, false,
                AISkillDriver.MovementType.StrafeMovetarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                false,
                false,
                AISkillDriver.ButtonPressType.Hold,
                -1,
                false,
                false,
                null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Special", SkillSlot.Special, null,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 25f,
                true, false, true, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                true, true, true,
                AISkillDriver.MovementType.StrafeMovetarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                false,
                false,
                AISkillDriver.ButtonPressType.Hold,
                -1,
                false,
                false,
                null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Primary", SkillSlot.Primary, null,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 50f,
                true, false, true, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                true, true, true,
                AISkillDriver.MovementType.StrafeMovetarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                false,
                false,
                AISkillDriver.ButtonPressType.Hold,
                -1,
                false,
                false,
                null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Strafe", SkillSlot.None, null,
                false, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 20f,
                false, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                false, false, false,
                AISkillDriver.MovementType.StrafeMovetarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                true,
                false,
                AISkillDriver.ButtonPressType.Abstain,
                -1,
                false,
                false,
                null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Chase", SkillSlot.None, null,
                false, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                20f, Mathf.Infinity,
                false, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                false, false, false,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                true,
                false,
                AISkillDriver.ButtonPressType.Abstain,
                -1,
                false,
                false,
                null);
        }

        protected override void InitializeDisplayPrefab()
        {
            base.InitializeDisplayPrefab();
            if (displayPrefab) displayPrefab.AddComponent<MenuSoundComponent>();
        }

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
            base.bodyPrefab.AddComponent<RocketTrackerComponent>();
            base.bodyPrefab.AddComponent<NetworkedBodyBlastJumpHandler>();

            EntityStateMachine offhandMachine = base.bodyPrefab.AddComponent<EntityStateMachine>();
            offhandMachine.customName = "Offhand";
            offhandMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            offhandMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));

            NetworkStateMachine nsm = base.bodyPrefab.GetComponent<NetworkStateMachine>();
            List<EntityStateMachine> stateMachines = nsm.stateMachines.ToList();
            stateMachines.Add(offhandMachine);
            nsm.stateMachines = stateMachines.ToArray();
            SetStateOnHurt ssoh = bodyPrefab.GetComponent<SetStateOnHurt>();
            if (ssoh)
            {
                List<EntityStateMachine> idleStateMachines = ssoh.idleStateMachine.ToList();
                idleStateMachines.Add(offhandMachine);
                ssoh.idleStateMachine = idleStateMachines.ToArray();
            }
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
            sk.passiveSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSkillPassive" + (Modules.Config.msPaintIcons.Value ? "_mspaint" : ""));

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
            primarySkillDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSkillPrimary" + (Modules.Config.msPaintIcons.Value ? "_mspaint" : ""));
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
            Skills.AddSkillToFamily(sk.primary.skillFamily, primarySkillDef);

            EntityStates.RocketSurvivorSkills.Special.FireAllRockets.rocketSkillInfoList.Add(new EntityStates.RocketSurvivorSkills.Special.FireAllRockets.RocketSkillInfo
            {
                skillDef = primarySkillDef,
                projectilePrefab = EntityStates.RocketSurvivorSkills.Primary.FireRocket.projectilePrefab,
                projectilePrefabICBM = EntityStates.RocketSurvivorSkills.Primary.FireRocket.projectilePrefabICBM,
                effectPrefab = EntityStates.RocketSurvivorSkills.Primary.FireRocket.effectPrefab,
                muzzleString = EntityStates.RocketSurvivorSkills.Primary.FireRocket.muzzleString,
                damageCoefficient = EntityStates.RocketSurvivorSkills.Primary.FireRocket.damageCoefficient,
                force = EntityStates.RocketSurvivorSkills.Primary.FireRocket.force,
                attackSoundString = EntityStates.RocketSurvivorSkills.Primary.FireRocket.attackSoundString
            });

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
            primaryAltSkillDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSkillPrimary_DirectHit" + (Modules.Config.msPaintIcons.Value ? "_mspaint" : ""));
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

            EntityStates.RocketSurvivorSkills.Special.FireAllRockets.rocketSkillInfoList.Add(new EntityStates.RocketSurvivorSkills.Special.FireAllRockets.RocketSkillInfo
            {
                skillDef = primaryAltSkillDef,
                projectilePrefab = EntityStates.RocketSurvivorSkills.Primary.FireRocketAlt.projectilePrefab,
                projectilePrefabICBM = EntityStates.RocketSurvivorSkills.Primary.FireRocketAlt.projectilePrefabICBM,
                effectPrefab = EntityStates.RocketSurvivorSkills.Primary.FireRocketAlt.effectPrefab,
                muzzleString = EntityStates.RocketSurvivorSkills.Primary.FireRocketAlt.muzzleString,
                damageCoefficient = EntityStates.RocketSurvivorSkills.Primary.FireRocketAlt.damageCoefficient,
                force = EntityStates.RocketSurvivorSkills.Primary.FireRocketAlt.force,
                attackSoundString = EntityStates.RocketSurvivorSkills.Primary.FireRocketAlt.attackSoundString
            });

            UnlockableDef samUnlock = ScriptableObject.CreateInstance<UnlockableDef>();
            samUnlock.cachedName = "Skills.MoffeinRocketSurvivor.Homing";
            samUnlock.nameToken = "ACHIEVEMENT_MOFFEINROCKETHOMINGUNLOCK_NAME";
            samUnlock.achievementIcon = primaryAltSkillDef.icon;
            Modules.ContentPacks.unlockableDefs.Add(samUnlock);
            Skills.AddSkillToFamily(sk.primary.skillFamily, primaryAltSkillDef, Config.ForceUnlock.Value ? null : samUnlock);

            RocketSurvivorSetup.FireRocketSkillDef = primarySkillDef;
            RocketSurvivorSetup.FireRocketAltSkillDef = primaryAltSkillDef;
            #endregion

            #region Secondary

            RocketTrackerSkillDef airDetTrackerDef = RocketTrackerSkillDef.CreateInstance<RocketTrackerSkillDef>();
            airDetTrackerDef.activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.RocketSurvivorSkills.Secondary.AirDet));
            airDetTrackerDef.activationStateMachineName = "Slide";
            airDetTrackerDef.baseMaxStock = 1;
            airDetTrackerDef.baseRechargeInterval = 2f;
            airDetTrackerDef.beginSkillCooldownOnSkillEnd = false;
            airDetTrackerDef.canceledFromSprinting = false;
            airDetTrackerDef.dontAllowPastMaxStocks = true;
            airDetTrackerDef.forceSprintDuringState = false;
            airDetTrackerDef.fullRestockOnAssign = true;
            airDetTrackerDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSkillSecondary" + (Modules.Config.msPaintIcons.Value ? "_mspaint" : ""));
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
            ec.soundName = "Play_Moffein_RocketSurvivor_R_Flak_Explode";  //Play_MULT_m2_main_explode
            Modules.Content.AddEffectDef(new EffectDef(airDetEffect));
            EntityStates.RocketSurvivorSkills.Secondary.AirDet.explosionEffectPrefab = airDetEffect;
            #endregion

            #region Utility
            SkillDef c4Def = SkillDef.CreateInstance<SkillDef>();
            c4Def.activationState = new SerializableEntityStateType(typeof(EntityStates.RocketSurvivorSkills.Utility.C4));
            c4Def.activationStateMachineName = "Offhand";
            c4Def.baseMaxStock = 1;
            c4Def.baseRechargeInterval = 5f;
            c4Def.beginSkillCooldownOnSkillEnd = false;
            c4Def.canceledFromSprinting = false;
            c4Def.dontAllowPastMaxStocks = true;
            c4Def.forceSprintDuringState = false;
            c4Def.fullRestockOnAssign = true;
            c4Def.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSkillUtility_C4" + (Modules.Config.msPaintIcons.Value ? "_mspaint" : ""));
            c4Def.interruptPriority = InterruptPriority.PrioritySkill;
            c4Def.isCombatSkill = true;
            c4Def.keywordTokens = new string[] { };
            c4Def.mustKeyPress = false;
            c4Def.cancelSprintingOnActivation = false;
            c4Def.rechargeStock = 1;
            c4Def.requiredStock = 1;
            c4Def.skillName = "ThrowC4";
            c4Def.skillNameToken = Rocket_Prefix + "UTILITY_NAME";
            c4Def.skillDescriptionToken = Rocket_Prefix + "UTILITY_DESCRIPTION";
            c4Def.stockToConsume = 1;
            (c4Def as ScriptableObject).name = "ThrowC4";
            Modules.Content.AddSkillDef(c4Def);
            RocketSurvivorSetup.C4Def = c4Def;
            Skills.AddSkillToFamily(sk.utility.skillFamily, c4Def);

            SkillDef marketGardenDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "MarketGarden",
                skillNameToken = Rocket_Prefix + "UTILITY_ALT_NAME",
                skillDescriptionToken = Rocket_Prefix + "UTILITY_ALT_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSkillUtility_Shovel" + (Modules.Config.msPaintIcons.Value ? "_mspaint" : "")),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.RocketSurvivorSkills.Utility.PrepComicallyLargeSpoon)),
                activationStateMachineName = "Offhand",
                baseMaxStock = 1,
                baseRechargeInterval = 5f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_HEAVY", "KEYWORD_STUNNING" }
            });
            (marketGardenDef as ScriptableObject).name = "MarketGarden";
            Modules.Content.AddSkillDef(marketGardenDef);
            RocketSurvivorSetup.ShovelDef = marketGardenDef;

            UnlockableDef spoonUnlock = ScriptableObject.CreateInstance<UnlockableDef>();
            spoonUnlock.cachedName = "Skills.MoffeinRocketSurvivor.MarketGaden";
            spoonUnlock.nameToken = "ACHIEVEMENT_MOFFEINROCKETMARKETGARDENUNLOCK_NAME";
            spoonUnlock.achievementIcon = marketGardenDef.icon;
            Modules.ContentPacks.unlockableDefs.Add(spoonUnlock);
            Skills.AddSkillToFamily(sk.utility.skillFamily, marketGardenDef, Config.ForceUnlock.Value ? null : spoonUnlock);

            #endregion

            #region Special

            SkillDef rearmDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Rearm",
                skillNameToken = Rocket_Prefix + "SPECIAL_NAME",
                skillDescriptionToken = Rocket_Prefix + "SPECIAL_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSkillSpecial" + (Modules.Config.msPaintIcons.Value ? "_mspaint" : "")),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.RocketSurvivorSkills.Special.FireAllRockets)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 8f,
                beginSkillCooldownOnSkillEnd = true,
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
                stockToConsume = 1
            });
            (rearmDef as ScriptableObject).name = "Rearm";
            Modules.Content.AddSkillDef(rearmDef);
            Modules.Skills.AddSpecialSkills(bodyPrefab, rearmDef);

            SkillDef rearmScepterDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "RearmScepter",
                skillNameToken = Rocket_Prefix + "SPECIAL_SCEPTER_NAME",
                skillDescriptionToken = Rocket_Prefix + "SPECIAL_SCEPTER_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSkillSpecial_Scepter" + (Modules.Config.msPaintIcons.Value ? "_mspaint" : "")),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.RocketSurvivorSkills.Special.FireAllRocketsScepter)),
                activationStateMachineName = "Weapon",
                baseMaxStock = rearmDef.baseMaxStock,
                baseRechargeInterval = rearmDef.baseRechargeInterval,
                beginSkillCooldownOnSkillEnd = rearmDef.beginSkillCooldownOnSkillEnd,
                canceledFromSprinting = rearmDef.canceledFromSprinting,
                forceSprintDuringState = rearmDef.forceSprintDuringState,
                fullRestockOnAssign = rearmDef.fullRestockOnAssign,
                interruptPriority = rearmDef.interruptPriority,
                resetCooldownTimerOnUse = rearmDef.resetCooldownTimerOnUse,
                isCombatSkill = rearmDef.isCombatSkill,
                mustKeyPress = rearmDef.mustKeyPress,
                cancelSprintingOnActivation = rearmDef.cancelSprintingOnActivation,
                rechargeStock = rearmDef.rechargeStock,
                requiredStock = rearmDef.requiredStock,
                stockToConsume = rearmDef.stockToConsume
            });
            (rearmScepterDef as ScriptableObject).name = "RearmScepter";
            Modules.Content.AddSkillDef(rearmScepterDef);
            RocketSurvivor.RocketSurvivorPlugin.SetupScepterClassic("RocketSurvivorBody", rearmScepterDef, rearmDef);
            RocketSurvivor.RocketSurvivorPlugin.SetupScepterStandalone("RocketSurvivorBody", rearmScepterDef, SkillSlot.Special, 0);
            #endregion
        }

        public override void InitializeSkins()
        {
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            List<GameObject> gameObjectsToActivate = Skins.CreateAllActivatedGameObjectsList(childLocator,
                "MeshRocketDefault0Backpack",//0
                "MeshRocketDefault1FragsAttached",//1
                "MeshRocketDefault42Body");//2

            #region DefaultSkin
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef("DEFAULT_SKIN",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketSkinDefault"),
                defaultRenderers,
                model);

            defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(defaultRenderers,
                "MeshRocketDefault0Backpack",
                "MeshRocketDefault1Frag",
                "MeshRocketDefault2FragsAttached",
                "MeshRocketDefault3Launcher",
                "MeshRocketDefault4Body",
                "MeshRocketDefault5Shovel",
                "MeshRocketDefault6Rocket");
                                                                                              
            defaultSkin.gameObjectActivations = Skins.GetGameObjectActivationsFromList(gameObjectsToActivate, 0, 1, 2); //activate 0, 1, and 2

            skins.Add(defaultSkin);
            #endregion
            
            #region MasterySkin
            UnlockableDef masteryUnlockable = ScriptableObject.CreateInstance<UnlockableDef>();
            masteryUnlockable.cachedName = Achievements.Mastery.unlockableIdentifier;
            masteryUnlockable.nameToken = LanguageTokens.GetAchievementNameToken(Achievements.Mastery.identifier);
            masteryUnlockable.achievementIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketSkinMastery");
            Modules.ContentPacks.unlockableDefs.Add(masteryUnlockable);

            SkinDef masterySkin = Modules.Skins.CreateSkinDef(Rocket_Prefix + "MASTERY_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketSkinMastery"),
                defaultRenderers,
                model,
                masteryUnlockable);

            masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(defaultRenderers,
                null,//"MeshRocketDefaultBackpack",
                null,//"MeshRocketDefaultFrag",
                null,//"MeshRocketDefaultFragsAttached",
                "MeshRocketBombardierLauncher",
                "MeshRocketBombardierBody",
                null,//"MeshRocketDefaultShovel",
                null//"MeshRocketDefaultRocket"
                );

            masterySkin.rendererInfos[3].defaultMaterial = Materials.CreateHopooMaterial("MatRocketBombardier").SetSpecular(0.54f, 5.82f);
            masterySkin.rendererInfos[4].defaultMaterial = Materials.CreateHopooMaterial("MatRocketBombardier");
                                                                                              
            masterySkin.gameObjectActivations = Skins.GetGameObjectActivationsFromList(gameObjectsToActivate, 2); //only activate 2, deactivate 0 and 2

            skins.Add(masterySkin);

            #endregion

            skinController.skins = skins.ToArray();
        }
    }
}