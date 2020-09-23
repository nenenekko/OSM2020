using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.Utility
{
    public class OSMSetting
    {
        public string community_file_path = "community.txt";
        public string python_name = "RMAO_auto.py";

        public int op_intro_interval = 10;
        public double op_intro_rate = 0.1;

        /*Visualizeテスト用*/
        public int agent_size;
        public int dim;
        public int correct_dim;
        public int malicious_dim;
        public AlgoEnum algo;
        public double targeth;
        public double common_weight;
        public double common_curiocity;
        public double sensor_weight; //0.8
        public double dist_weight;
        public double malicious_dist_weight; //0.8
        public int sensor_size;
        //int malicious_sensor_size = (int)(0.04 * agent_size);
        public int malicious_sensor_size;
        public double op_form_threshold;
        public int sample_size;
        public int change_round;

        public bool bad_sensor_mode;
        public int opinion_share_num;
        public SensorArrangementEnum sensor_arrange;
        public EnvDistributionEnum env_distribution;
        public BeliefUpdateFunctionEnum belief_update;
        public bool add_share_only_community;

        public GraphEnum select_graph;

        public List<int> stop_rounds;

        public OSMSetting(ExperimentType experiment_type)
        {
            common_weight = 0.5;
            common_curiocity = 0.5;
            dist_weight = 0.5;
            malicious_dist_weight = 0; //0.8
            //int malicious_sensor_size = (int)(0.04 * agent_size);
            sample_size = 1;
            change_round = 0;
            dim = 2;
            correct_dim = 0;
            malicious_dim = 1;
            switch (experiment_type)
            {
                case ExperimentType.OSM_LFR_Bad_Each_Exponential:
                    agent_size = 100;
                    algo = AlgoEnum.OSMonly;
                    targeth = 0.9;
                    sensor_weight = 0.8; //0.8

                    malicious_sensor_size = 0;
                    op_form_threshold = 0.9;

                    bad_sensor_mode = true;
                    opinion_share_num = 1;
                    sensor_arrange = SensorArrangementEnum.Each;
                    env_distribution = EnvDistributionEnum.Exponential;
                    belief_update = BeliefUpdateFunctionEnum.Bayse;
                    add_share_only_community = false;

                    select_graph = GraphEnum.LFR;
                    break;
                case ExperimentType.OSM_LFR_Bad_Each_Shitei_SameOpinionBayse:
                    agent_size = 100;
                    algo = AlgoEnum.OSMonly;
                    targeth = 0.9;
                    sensor_weight = 0.8; //0.8

                    malicious_sensor_size = 0;
                    op_form_threshold = 0.9;

                    bad_sensor_mode = true;
                    opinion_share_num = 1;
                    sensor_arrange = SensorArrangementEnum.Each;
                    env_distribution = EnvDistributionEnum.Shitei;
                    belief_update = BeliefUpdateFunctionEnum.SameOpinionAdjustBayse;
                    add_share_only_community = false;

                    select_graph = GraphEnum.LFR;
                    break;
                case ExperimentType.OSM_LFR_Normal_Each_Exponential:
                    agent_size = 100;
                    algo = AlgoEnum.OSMonly;
                    targeth = 0.9;
                    sensor_weight = 0.8; //0.8
                    op_form_threshold = 0.9;

                    bad_sensor_mode = true;
                    opinion_share_num = 1;
                    sensor_arrange = SensorArrangementEnum.Each;
                    env_distribution = EnvDistributionEnum.Exponential;
                    belief_update = BeliefUpdateFunctionEnum.Bayse;
                    add_share_only_community = false;

                    select_graph = GraphEnum.LFR;
                    break;
                case ExperimentType.AAT_LFR_Bad_Each_Shitei:
                    agent_size = 100;
                    algo = AlgoEnum.AAT;
                    targeth = 0.9;
                    sensor_weight = 0.55; //0.8

                    op_form_threshold = 0.9;
                    sample_size = 1;
                    change_round = 0;

                    bad_sensor_mode = true;
                    opinion_share_num = 1;
                    sensor_arrange = SensorArrangementEnum.Each;
                    env_distribution = EnvDistributionEnum.Shitei;
                    belief_update = BeliefUpdateFunctionEnum.Bayse;
                    add_share_only_community = false;

                    select_graph = GraphEnum.LFR;
                    break;
                case ExperimentType.AAT_LFR_Bad_Each_Exponential:
                    agent_size = 100;

                    algo = AlgoEnum.AAT;
                    targeth = 0.9;
                    sensor_weight = 0.8; //0.8
                    op_form_threshold = 0.9;

                    bad_sensor_mode = true;
                    opinion_share_num = 1;
                    sensor_arrange = SensorArrangementEnum.Each;
                    env_distribution = EnvDistributionEnum.Exponential;
                    belief_update = BeliefUpdateFunctionEnum.Bayse;
                    add_share_only_community = false;

                    select_graph = GraphEnum.LFR;
                    break;
                case ExperimentType.AAT_LFR_Bad_Each_Exponential_SameOpinionBayse:
                    agent_size = 100;

                    algo = AlgoEnum.AAT;
                    targeth = 0.7;
                    sensor_weight = 0.8; //0.8
                    op_form_threshold = 0.9;

                    bad_sensor_mode = true;
                    opinion_share_num = 1;
                    sensor_arrange = SensorArrangementEnum.Each;
                    env_distribution = EnvDistributionEnum.Exponential;
                    belief_update = BeliefUpdateFunctionEnum.SameOpinionAdjustBayse;
                    add_share_only_community = false;

                    select_graph = GraphEnum.LFR;
                    break;
                case ExperimentType.AAT_LFR_Bad_Each_Shitei_SameOpinionBayse:
                    agent_size = 100;

                    algo = AlgoEnum.AAT;
                    targeth = 0.8;
                    sensor_weight = 0.8; //0.8
                    op_form_threshold = 0.9;

                    bad_sensor_mode = true;
                    opinion_share_num = 1;
                    sensor_arrange = SensorArrangementEnum.Each;
                    env_distribution = EnvDistributionEnum.Exponential;
                    belief_update = BeliefUpdateFunctionEnum.SameOpinionAdjustBayse;
                    add_share_only_community = false;

                    select_graph = GraphEnum.LFR;
                    break;
                case ExperimentType.AAT_LFR_Normal_Each_Shitei:
                    agent_size = 100;
                    algo = AlgoEnum.AAT;
                    targeth = 0.9;
                    sensor_weight = 0.55; //0.8
                    op_form_threshold = 0.8;

                    bad_sensor_mode = false;
                    opinion_share_num = 1;
                    sensor_arrange = SensorArrangementEnum.Each;
                    env_distribution = EnvDistributionEnum.Shitei;
                    belief_update = BeliefUpdateFunctionEnum.Bayse;
                    add_share_only_community = false;

                    select_graph = GraphEnum.LFR;
                    break;
                case ExperimentType.AAT_LFR_Normal_Each_Exponential:
                    agent_size = 100;
                    algo = AlgoEnum.AAT;
                    targeth = 0.9;

                    sensor_weight = 0.8; //0.8
                    op_form_threshold = 0.9;

                    bad_sensor_mode = false;
                    opinion_share_num = 1;
                    sensor_arrange = SensorArrangementEnum.Each;
                    env_distribution = EnvDistributionEnum.Exponential;
                    belief_update = BeliefUpdateFunctionEnum.Bayse;
                    add_share_only_community = false;

                    select_graph = GraphEnum.LFR;
                    break;
                case ExperimentType.AAT_SW_Normal_Random_Exponential:
                    agent_size = 100;

                    algo = AlgoEnum.AAT;
                    targeth = 0.9;

                    sensor_weight = 0.8; //0.8
                    op_form_threshold = 0.9;


                    bad_sensor_mode = false;
                    opinion_share_num = 1;
                    sensor_arrange = SensorArrangementEnum.Random;
                    env_distribution = EnvDistributionEnum.Exponential;
                    belief_update = BeliefUpdateFunctionEnum.Bayse;
                    add_share_only_community = false;

                    select_graph = GraphEnum.Grid2D;
                    break;
            }
            sensor_size = (int)(0.05 * agent_size);
        }
    }
}
