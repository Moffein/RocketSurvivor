using R2API;
using System;
using System.Linq;
using Zio.FileSystems;

namespace RocketSurvivor.Modules
{
    internal class LanguageTokens
    {
        public static SubFileSystem fileSystem;
        internal static string languageRoot => System.IO.Path.Combine(LanguageTokens.assemblyDir, "language");

        internal static string assemblyDir
        {
            get
            {
                return System.IO.Path.GetDirectoryName(RocketSurvivor.RocketSurvivorPlugin.pluginInfo.Location);
            }
        }
        public LanguageTokens()
        {
            RegisterLanguageTokens();
            //string handPrefix = Content.HANDSurvivor.HANDSurvivor.HAND_PREFIX;

            /*string rmorPrefix = Content.RMORSurvivor.RMORSurvivor.RMOR_PREFIX;
            LanguageAPI.Add(rmorPrefix + "NAME", "R-MOR");

            LanguageAPI.Add(rmorPrefix + "PRIMARY_NAME", "ERADICATE");
            LanguageAPI.Add(rmorPrefix + "PRIMARY_DESC", "Charge up your cannons and fire a barrage of rockets for up to <style=cIsDamage>3x420% damage</style>.");*/
        }

        public static void RegisterLanguageTokens()
        {
            On.RoR2.Language.SetFolders += fixme;
        }

        //Credits to Anreol for this code
        private static void fixme(On.RoR2.Language.orig_SetFolders orig, RoR2.Language self, System.Collections.Generic.IEnumerable<string> newFolders)
        {
            if (System.IO.Directory.Exists(LanguageTokens.languageRoot))
            {
                var dirs = System.IO.Directory.EnumerateDirectories(System.IO.Path.Combine(LanguageTokens.languageRoot), self.name);
                orig(self, newFolders.Union(dirs));
                return;
            }
            orig(self, newFolders);
        }
    }
}