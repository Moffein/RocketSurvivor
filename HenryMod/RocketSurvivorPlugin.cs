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
using RocketSurvivor.Modules;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace RocketSurvivor
{
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.weliveinasociety.CustomEmotesAPI", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("HIFU.Inferno", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.DrBibop.VRAPI", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ThinkInvisible.ClassicItems", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency(R2API.PrefabAPI.PluginGUID)]
    [BepInDependency(R2API.UnlockableAPI.PluginGUID)]
    [BepInDependency(R2API.DamageAPI.PluginGUID)]
    [BepInDependency(R2API.SoundAPI.PluginGUID)]
    [BepInDependency(R2API.RecalculateStatsAPI.PluginGUID)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]

    public class RocketSurvivorPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.EnforcerGang.RocketSurvivor";
        public const string MODNAME = "RocketSurvivor";
        public const string MODVERSION = "1.0.11";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "MOFFEIN";

        public static RocketSurvivorPlugin instance;
        public static bool infernoPluginLoaded = false;
        public static bool scepterStandaloneLoaded = false;
        public static bool scepterClassicLoaded = false;
        public static bool emoteAPILoaded = false;
        public static bool riskOfOptionsLoaded = false;
        public static bool VRAPILoaded = false;

        private void Awake()
        {
            instance = this;
            Files.PluginInfo = this.Info;

            infernoPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("HIFU.Inferno");
            scepterStandaloneLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
            scepterClassicLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.ClassicItems");
            emoteAPILoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.weliveinasociety.CustomEmotesAPI");
            riskOfOptionsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
            VRAPILoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DrBibop.VRAPI");

            Log.Init(Logger);

            DamageTypes.Initialize();   //Init this first. Other things depend on this.
            Buffs.Initialize();

            Modules.Assets.Initialize(); // load assets and read config
            SoundBanks.Init();
            Modules.Config.ReadConfig();
            Modules.States.RegisterStates(); // register states for networking
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            new Modules.LanguageTokens();
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules

            // survivor initialization
            new RocketSurvivorSetup().Initialize();

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();
            if (emoteAPILoaded) EmoteAPICompat();
        }
        
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void EmoteAPICompat()
        {
            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();
                foreach (var item in SurvivorCatalog.allSurvivorDefs)
                {
                    if (item.bodyPrefab.name == "RocketSurvivorBody")
                    {
                        var skele = Modules.Assets.mainAssetBundle.LoadAsset<UnityEngine.GameObject>("animRocketEmote.prefab");
                        EmotesAPI.CustomEmotesAPI.ImportArmature(item.bodyPrefab, skele);
                        skele.GetComponentInChildren<BoneMapper>().scale = 1.5f;
                    }
                }
            };
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
