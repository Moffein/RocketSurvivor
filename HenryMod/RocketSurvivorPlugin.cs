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
        public const string MODVERSION = "0.2.17";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "MOFFEIN";

        public static RocketSurvivorPlugin instance;
        public static bool infernoPluginLoaded = false;
        public static bool scepterStandaloneLoaded = false;
        public static bool scepterClassicLoaded = false;

        public static bool msPaintIcons = true;

        private void Awake()
        {
            instance = this;
            infernoPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("HIFU.Inferno");
            scepterStandaloneLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
            scepterClassicLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.ClassicItems");

            Log.Init(Logger);

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