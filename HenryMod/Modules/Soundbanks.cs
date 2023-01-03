using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RocketSurvivor.Modules
{
    internal static class SoundBanks
    {

        public static string SoundBankDirectory
        {
            get
            {
                return Path.Combine(Files.assemblyDir, "SoundBanks");
            }
        }

        public static void Init()
        {
            AKRESULT akResult = AkSoundEngine.AddBasePath(SoundBankDirectory);

            AkSoundEngine.LoadBank("RocketSoundbank.bnk", out _);
        }
    }
}
