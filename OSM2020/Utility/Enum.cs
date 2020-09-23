using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.Utility
{
    enum GUIEnum
    {
        AgentGUI,
        AnimationGUI,
        ExperimentGUI,
        LearningGUI,
        MainFormGUI,
        GraphGUI,
    }

    public enum GraphEnum
    {
        WS,
        NewmanWS,
        ConnectedWS,
        BA,
        FastGnp,
        GnpRandom,
        DenseGnm,
        Gnm,
        ER,
        Binomial,
        RandomRegular,
        PowerLawCluster,
        RandomKernel,
        RandomLobster,
        Grid2D,
        Hexagonal,
        Triangular,
        Custom,
        KarateClub,
        LFR,
        Void
    }

    enum LayoutEnum
    {
        Circular,
        FruchtermanReingold,
        KamadaKawai,
        Random,
        Shell,
        Spectral,
        Spring,
        Square,
        Null
    }


    enum SeedEnum
    {
        AgentGenerateSeed,
        PlayStepSeed,
    }

    enum InitBeliefMode
    {
        NormalNarrow,
        Normal,
        NormalWide,
        NoRandom
    }

    enum CalcWeightMode
    {
        FavorMyOpinion,
        Equality
    }

    enum SampleAgentSetMode
    {
        RandomSetRate,
        RemainSet
    }

    public enum AlgoEnum
    {
        None,
        AAT,
        CCAAT,
        OSMonly
    }

    enum SensorWeightEnum
    {
        DependSensorRate,
        Custom,
        SameNoneSensor,
        FollowEnvDistWeight
    }

    public enum BeliefUpdateFunctionEnum
    {
        Bayse,
        Particle,
        AATPaperBayse,
        SameOpinionAdjustBayse
    }

    public enum EnvDistributionEnum
    {
        Turara,
        Exponential,
        Shitei

    }

    public enum SensorArrangementEnum
    {
        Each,
        Random
    }

    public enum ExperimentType
    {
        OSM_LFR_Bad_Each_Exponential,
        OSM_LFR_Bad_Each_Shitei_SameOpinionBayse,
        OSM_LFR_Normal_Each_Exponential,
        AAT_LFR_Bad_Each_Shitei,
        AAT_LFR_Normal_Each_Shitei,
        AAT_LFR_Bad_Each_Exponential,
        AAT_LFR_Bad_Each_Exponential_SameOpinionBayse,
        AAT_LFR_Bad_Each_Shitei_SameOpinionBayse,
        AAT_LFR_Normal_Each_Exponential,
        AAT_SW_Bad_Random_Shitei,
        AAT_SW_Bad_Random_Exponential,
        AAT_SW_Normal_Random_Shitei,
        AAT_SW_Normal_Random_Exponential
    }
}
