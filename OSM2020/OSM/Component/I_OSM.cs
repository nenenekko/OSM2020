using MathNet.Numerics.LinearAlgebra;
using OSM2020.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.Component
{
    interface I_OSM
    {
        int CurrentStep { get; set; }
        int CurrentRound { get; set; }
        //Dictionary<int, RecordStep> MyRecordSteps { get; set; }
        //Dictionary<int, RecordRound> MyRecordRounds { get; set; }
        //RecordStep MyRecordStep { get; set; }
        RecordRound MyRecordRound { get; set; }
        List<RecordRound> MyRecordRounds { get; set; }
        AgentNetwork MyAgentNetwork { get; set; }

        //step
        void InitializeToFirstStep(int opinion_share_num);
        void InitializeStep();
        void NextStep(bool bad_sensor_mode, int opinion_share_num);
        void RecordStep(bool final);
        void FinalizeStep();
        void UpdateSteps(int step_count, bool bad_sensor_mode, int opinion_share_num);
        void PrintStepInfo();

        //round
        void InitializeToFirstRound(int opinion_share_num);
        void InitializeRound(int opinion_share_num);
        void NextRound(int step_count, bool bad_sensor_mode, int opinion_share_num);
        void RecordRound();
        void FinalizeRound();
        void Execute(int round_count, int step_count,  bool bad_sensor_mode, int opinion_share_num,ExtendProgressBar pb = null);
        void PrintRoundInfo();

        //agent
        void PrintAgentInfo(Agent agent);

        //common
    }
}
