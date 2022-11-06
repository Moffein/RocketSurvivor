using BepInEx;
using RocketSurvivor.Modules.Survivors;
using R2API.Utils;
using RocketSurvivor;
using RoR2;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using RoR2.Skills;
using System.Runtime.CompilerServices;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace RocketSurvivor
{
    [BepInDependency("HIFU.Inferno", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ThinkInvisible.ClassicItems", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "LanguageAPI",
        "SoundAPI",
        "UnlockableAPI",
        nameof(R2API.DamageAPI),
        nameof(R2API.RecalculateStatsAPI)
    })]

    public class RocketSurvivorPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.EnforcerGang.RocketSurvivor";
        public const string MODNAME = "RocketSurvivor";
        public const string MODVERSION = "0.5.3";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "MOFFEIN";

        public static RocketSurvivorPlugin instance;
        public static bool infernoPluginLoaded = false;
        public static bool scepterStandaloneLoaded = false;
        public static bool scepterClassicLoaded = false;

        public static bool msPaintIcons = false;
        public static bool pocketICBM = true;
        public static bool pocketICBMEnableKnockback = false;
        public static bool samTracking = true;

        private void Awake()
        {
            instance = this;
            infernoPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("HIFU.Inferno");
            scepterStandaloneLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
            scepterClassicLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.ClassicItems");

            Log.Init(Logger);

            ReadConfig();

            DamageTypes.Initialize();   //Init this first. Other things depend on this.
            Buffs.Initialize();

            Modules.Assets.Initialize(); // load assets and read config
            Modules.Config.ReadConfig();
            Modules.States.RegisterStates(); // register states for networking
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules

            // survivor initialization
            new RocketSurvivorSetup().Initialize();

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();
        }

        private void ReadConfig()
        {
            msPaintIcons = Config.Bind("General", "Use MSPaint icons", false, "Use the original MSPaint icons from the mod's release.").Value;
            pocketICBM = Config.Bind("Gameplay", "Pocket ICBM Interaction", true, "Pocket ICBM works with Rocket's skills.").Value;
            pocketICBMEnableKnockback = Config.Bind("Gameplay", "Pocket ICBM Knockback", false, "Extra rockets from Pocket ICBM have knockback.").Value;
            samTracking = Config.Bind("Primaries - HG4 SAM Launcher", "Enable Homing (Server-Side)", true, "SAM Rockets will home towards targets.").Value;
        }

        public static float GetICBMDamageMult(CharacterBody body)
        {
            float mult = 1f;
            if (body && body.inventory)
            {
                int itemcount = body.inventory.GetItemCount(DLC1Content.Items.MoreMissile);
                int stack = itemcount - 1;
                if (stack > 0) mult += stack * 0.5f;
            }
            return mult;
        }

        public static void SetupScepterClassic(string bodyName, SkillDef scepterSkill, SkillDef origSkill)
        {
            if (scepterClassicLoaded) SetupScepterClassicInternal(bodyName, scepterSkill, origSkill);
        }

        public static void SetupScepterStandalone(string bodyName, SkillDef scepterSkill, SkillSlot skillSlot, int skillIndex)
        {
            if (scepterStandaloneLoaded) SetupScepterStandaloneInternal(bodyName, scepterSkill, skillSlot, skillIndex);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void SetupScepterStandaloneInternal(string bodyName, SkillDef scepterSkill, SkillSlot skillSlot, int skillIndex)
        {
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterSkill, bodyName, skillSlot, skillIndex);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void SetupScepterClassicInternal(string bodyName, SkillDef scepterSkill, SkillDef origSkill)
        {
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(scepterSkill, bodyName, SkillSlot.Special, origSkill);
        }
    }
}