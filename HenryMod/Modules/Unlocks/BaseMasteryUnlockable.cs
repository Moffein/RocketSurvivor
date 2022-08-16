using RoR2;
using System.Runtime.CompilerServices;

namespace RocketSurvivor.Modules
{
    public abstract class BaseMasteryUnlockable : GenericModdedUnlockable
    {
        public abstract string RequiredCharacterBody { get; }
        public abstract float RequiredDifficultyCoefficient { get; }

        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            Run.onClientGameOverGlobal += OnClientGameOverGlobal;
        }
        public override void OnBodyRequirementBroken()
        {
            Run.onClientGameOverGlobal -= OnClientGameOverGlobal;
            base.OnBodyRequirementBroken();
        }
        private void OnClientGameOverGlobal(Run run, RunReport runReport)
        {
            if ((bool)runReport.gameEnding && runReport.gameEnding.isWin)
            {

                DifficultyDef infernoDef = null;
                if (RocketSurvivorPlugin.infernoPluginLoaded)
                {
                    infernoDef = GetInfernoDef();
                }

                DifficultyDef runDifficulty = DifficultyCatalog.GetDifficultyDef(runReport.ruleBook.FindDifficulty());
                if ((infernoDef != null && runDifficulty == infernoDef) || (runDifficulty.countsAsHardMode && runDifficulty.scalingValue >= RequiredDifficultyCoefficient))
                {
                    Grant();
                }
            }
        }

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex(RequiredCharacterBody);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private DifficultyDef GetInfernoDef()
        {
            return Inferno.Main.InfernoDiffDef;
        }
    }
}