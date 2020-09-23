using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Random;
using OSM2020.Experiment;
using OSM2020.OSM.Algorithm.AAT;
using OSM2020.OSM.Component;
using OSM2020.OSM.GraphGenerator;
using OSM2020.OSM.LayoutGenerator;
using OSM2020.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSM2020.GUI
{
    public partial class MainForm : Form
    {
        internal GUIEnum MyGUIEnum;

        List<UserControl> GUI_List;
        internal GraphGUI MyGraphGUI;
        internal AgentGUI MyAgentGUI;
        internal LearningGUI MyLearningGUI;
        internal ExperimentGUI MyExperimentGUI;
        internal AnimationForm MyAnimationForm;
        I_OSM MyOSM;
        OSMSetting osm_setting;


        public MainForm()
        {
            this.MyGUIEnum = GUIEnum.MainFormGUI;
            this.InitializeGUIs();
            InitializeComponent();
            this.UserInitialize();
            this.MyAnimationForm = new AnimationForm();
            Test();
            //TargethExp();
            this.MyAnimationForm.Show();
            this.MyAnimationForm.Left = this.Right;
        }
        void TargethExp()
        {
            var dt = DateTime.Now;
            var dt_name = dt.ToString("yyyyMMddHHmm");
            var seeds = new List<int>() {3}; //0,1,2
            double sensor_weight = 0.8; //0.8
            int steps = 2000;

            dt = DateTime.Now;
            dt_name = dt.ToString("yyyyMMddHHmm");
            //Parallel.For(0, seeds, seed =>

            osm_setting = new OSMSetting(ExperimentType.AAT_LFR_Normal_Each_Exponential);

            Parallel.ForEach(seeds, seed =>
            {
                /* リストで定義されているクラスの引数には任意個数セットできる．出力は全体の直積*/
                new TargetH_Experiment()
                      .SetGraphs(new List<GraphEnum>() { GraphEnum.Grid2D, GraphEnum.BA, GraphEnum.WS })//グラフをセット
                      .SetAlgos(new List<AlgoEnum>() { AlgoEnum.AAT})       //意見共有アルゴリズムをセット
                      .SetNetworkSize(new List<int>() { 100  ,300,500,1000})               //ネットワークサイズ(ノード数)をセット
                      .SetDims(new List<int>() { 2 })                        //意見の種類
                      .SetSensorWeight(sensor_weight)                        //環境とセンサの間のリンクの重みをセット.これはリストじゃあない
                      .SetSensorSizeRate(new List<double>() { 0.05 })        //環境情報を認識するエージェントの割合を設定
                      .SetMaliciousSensorSizeRate(new List<double>() { 0.00 }) //誤った環境情報を認識するエージェントの割合を設定
                                                                               //.SetSensorFixSize(10)
                      .SetBeliefUpdater(new BeliefUpdater(BeliefUpdateFunctionEnum.SameOpinionAdjustBayse).SetSensorWeightMode(SensorWeightEnum.DependSensorRate))//信念値更新式
                      .SetSubjectName("test")                               //実験名？
                      .SetEnvDistWeights(new List<double>() { 0.5 })         //環境からセンサエージェントへの意見の種類に関する確率分布
                      .SetMaliciousEnvDistWeights(new List<double>() { 0.0 }) //誤った環境からセンサエージェントへの重みをセット
                      .SetCommonCuriocity(0.1)                              //
                                                                            //.SetTargetHs(new List<double>() {0.5, 0.525, 0.55, 0.575, 0.6, 0.625, 0.65, 0.675, 0.7, 0.725, 0.75, 0.775, 0.8, 0.825, 0.85, 0.875, 0.9, 0.925, 0.95, 0.975,1 })             //目標意見形成率
                      .SetTargetHs(new List<double>() { 0.9 })                                                      //.SetTargetHs(Enumerable.Range(20, 21).Select(x => x * 0.025).ToList())
                      .SetLogFolder(dt_name, "gdwt_size_grid_0.95")         //ログフォルダ名
                      .SetRounds(new List<int>() { 500 })                   //ラウンド数 (繰り返し数)をセット
                      .SetSteps(steps)                                      //ステップ数(1ラウンドあたりの更新数)をセット
                      .SetOpinionThreshold(0.9)                             //意見形成の閾値.0.9
                                                                            //.SetOpinionThreshold(0.8)
                      .SetDynamics(new List<bool>() { false })              //ダイナミクス？
                      .SetEnvDistModes(new List<EnvDistributionEnum> { EnvDistributionEnum.Exponential })
                      .SetInfoWeightRates(new List<double>() { 1.0 }) //昔の名残
                      .SetCommonWeights(new List<double>() { 0.5 }) //OSM_Onlyにしか使わない.事前に重みが分かってるとき用ってことか
                                                                    //.SetCommonWeights(Enumerable.Range(0, 21).Select(x => x * 0.05).ToList())

                      .SetBadSensorMode(new List<bool>() { false }) // BadSensorモードの有無
                      .SetOpinionShareNum(new List<int>() { 1 })   //意見発信回数
                      .SetSensorArrangement(new List<SensorArrangementEnum>() { SensorArrangementEnum.Random }) //センサの配置方法
                      .SetAdditionalShareCommunityOnly(new List<bool>() { false }) //Badコミュニティのみの追加意見発信か否か

                      .SetOsmSetting(osm_setting)
                      .Run(seed);
            });

            Environment.Exit(0);
        }
        void Test()
        {
            this.osm_setting = new OSMSetting(ExperimentType.AAT_SW_Normal_Random_Exponential);
            int agent_size = osm_setting.agent_size;
            int dim = osm_setting.dim;
            int correct_dim = osm_setting.correct_dim;
            int malicious_dim = osm_setting.malicious_dim;
            AlgoEnum algo = osm_setting.algo;
            double targeth = osm_setting.targeth;
            double common_weight = osm_setting.common_weight;
            double common_curiocity = osm_setting.common_curiocity;
            double sensor_weight = osm_setting.sensor_weight;
            double dist_weight = osm_setting.dist_weight;
            double malicious_dist_weight = osm_setting.malicious_dist_weight;
            int sensor_size = osm_setting.sensor_size;
            //int malicious_sensor_size = (int)(0.04 * agent_size);
            int malicious_sensor_size = osm_setting.malicious_sensor_size;
            var op_form_threshold = osm_setting.op_form_threshold;
            int sample_size = osm_setting.sample_size;
            int change_round = osm_setting.change_round;

            bool bad_sensor_mode = osm_setting.bad_sensor_mode;
            int opinion_share_num = osm_setting.opinion_share_num;
            bool is_add_share_only_community = osm_setting.add_share_only_community;

            EnvDistributionEnum env_distribution = osm_setting.env_distribution;
            BeliefUpdateFunctionEnum belief_update = osm_setting.belief_update;

            var belief_updater = new BeliefUpdater(belief_update).SetSensorWeightMode(SensorWeightEnum.DependSensorRate);

            GraphGeneratorBase graph_generator;

            switch (osm_setting.select_graph)
            {
                case GraphEnum.WS:
                    graph_generator = new WS_GraphGenerator().SetNodeSize(agent_size).SetNearestNeighbors(6).SetRewireP(0.01);
                    break;
                //case GraphEnum.PowerLawCluster:
                    //graph_generator = new PC_GraphGenerator().SetNodeSize(500).SetRandomEdges(3).SetAddTriangleP(0.1);
                   // break;
                case GraphEnum.BA:
                    graph_generator = new BA_GraphGenerator().SetNodeSize(agent_size).SetAttachEdges(2);
                    break;
                case GraphEnum.Grid2D:
                    graph_generator = new Grid2D_GraphGenerator().SetNodeSize(agent_size);
                    break;
                //case GraphEnum.ER:
                    //graph_generator = new ER_GraphGenerator().SetNodeSize(agent_size).SetEdgeCreateP(0.01);
                    //break;
                case GraphEnum.LFR:
                    graph_generator = new LFR_GraphGenerator().SetNodeSize(agent_size);
                    break;
                default:
                    graph_generator = new LFR_GraphGenerator().SetNodeSize(agent_size);
                    break;
            }
            graph_generator.SetOsmSetting(osm_setting);
            var pb = new ExtendProgressBar(100);
            var graph = graph_generator.Generate(0, pb);
            var layout = new KamadaKawai_LayoutGenerator(graph).Generate(pb);
            //var layout = new Circular_LayoutGenerator(graph).Generate();

            //LFRの時のみ
            List<List<int>> communityList = new List<List<int>>();
            if(osm_setting.select_graph == GraphEnum.LFR)
                communityList = graph_generator.GetCommunity();
            var init_belief_gene = new InitBeliefGenerator()
                                    .SetInitBeliefMode(mode: InitBeliefMode.NormalNarrow);
            //.SetInitBeliefMode(mode: InitBeliefMode.NormalWide);

            var subject_test = new OpinionSubject("test", dim);

            var sample_agent_test = new SampleAgent()
                                .SetInitBeliefGene(init_belief_gene)
                                .SetThreshold(op_form_threshold)
                                .SetSubject(subject_test)
                                .SetInitOpinion(Vector<double>.Build.Dense(dim, 0.0));

            var sensor_gene = new SensorGenerator()
            //                .SetSensorSize((int)5);
            .SetSensorSize(sensor_size, malicious_sensor_size);

            int agent_gene_seed = 11;//4
            var agent_gene_rand = new ExtendRandom(agent_gene_seed);


            var agent_network = new AgentNetwork()
                                    .SetRand(agent_gene_rand)
                                    .GenerateNetworkFrame(graph)
                                    //.ApplySampleAgent(sample_agent_1, mode: SampleAgentSetMode.RandomSetRate, random_set_rate: 0.5)
                                    //.ApplySampleAgent(sample_agent_2, mode: SampleAgentSetMode.RemainSet)
                                    .ApplySampleAgent(sample_agent_test, mode: SampleAgentSetMode.RemainSet)
                                    .SetBadSensorMode(osm_setting.bad_sensor_mode)
                                    .SetSensorArrange(osm_setting.sensor_arrange)
                                    .SetCommnityList(communityList)
                                    .GenerateSensor(sensor_gene)
                                    .SetLayout(layout);
            var bad_community_index = agent_network.GetBadCommunityIndex();

            int update_step_seed = 1;
            var update_step_rand = new ExtendRandom(update_step_seed);

            OSMBase osm = new OSM_Only();
            switch (algo)
            {
                case AlgoEnum.AAT:
                    var osm_aat = new AAT_OSM();
                    osm_aat.SetTargetH(targeth);
                    osm = osm_aat;
                    break;

                default:
                    break;
            }
            osm.SetRand(update_step_rand);
            osm.SetAgentNetwork(agent_network, opinion_share_num);
            var subject_mgr_dic = new Dictionary<int, SubjectManager>();
            subject_mgr_dic.Add(0, new SubjectManagerGenerator()
                .Generate(subject_test, dist_weight, correct_dim, sensor_weight, env_distribution, malicious_dim, malicious_dist_weight));
            //for (int i = 0; i < 1; i++)
            //{
            //  subject_mgr_dic.Add(i * change_round, new SubjectManagerGenerator().Generate(subject_test, dist_weight, i % dim, sensor_rate, EnvDistributionEnum.Turara));
            //}
            osm.SetSubjectManagerDic(subject_mgr_dic);
            osm.SetInitWeightsMode(mode: CalcWeightMode.FavorMyOpinion);
            osm.SetOpinionIntroInterval(osm_setting.op_intro_interval);
            osm.SetOpinionIntroRate(osm_setting.op_intro_rate);
            //osm.SetSensorCommonWeight(0.70);
            osm.SetBeliefUpdater(belief_updater);
            osm.SetAddShareCommunityOnly(is_add_share_only_community);

            osm.SetStopRound(osm_setting.stop_rounds);

            this.MyOSM = osm;
            this.MyAnimationForm.RegistOSM(osm,osm_setting.opinion_share_num, communityList, bad_community_index);

        }

        void UserInitialize()
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            PythonProxy.StartUpPython();
            var working_folder_path = Properties.Settings.Default.WorkingFolderPath;
            if (!Directory.Exists(working_folder_path))
            {
                Directory.CreateDirectory(working_folder_path);
            }

            this.radioButtonGraphGUI.Checked = true;
            this.radioButtonRoundCheck.Checked = true;
            this.numericUpDownStepsControl.Value = 3000;
            this.numericUpDownSpeedControl.Value = 1;
            this.labelRoundNum.Text = 0.ToString();
            this.PlayStopFlag = true;
            this.checkBoxMenu.Checked = true;

        }

        void InitializeGUIs()
        {
            this.GUI_List = new List<UserControl>();
            this.DoubleBuffered = true;

            this.MyGraphGUI = new GraphGUI();
            this.MyGraphGUI.Dock = DockStyle.Fill;
            this.MyGraphGUI.Name = "GraphGUI";
            this.Controls.Add(this.MyGraphGUI);
            this.GUI_List.Add(this.MyGraphGUI);
            this.MyGraphGUI.Visible = true;

            this.MyAgentGUI = new AgentGUI();
            this.MyAgentGUI.Dock = DockStyle.Fill;
            this.MyAgentGUI.Name = "AgentGUI";
            this.Controls.Add(this.MyAgentGUI);
            this.GUI_List.Add(this.MyAgentGUI);
            this.MyAgentGUI.Visible = false;

            this.MyLearningGUI = new LearningGUI();
            this.MyLearningGUI.Dock = DockStyle.Fill;
            this.MyLearningGUI.Name = "LearningGUI";
            this.Controls.Add(this.MyLearningGUI);
            this.GUI_List.Add(this.MyLearningGUI);
            this.MyLearningGUI.Visible = false;

            this.MyExperimentGUI = new ExperimentGUI();
            this.MyExperimentGUI.Dock = DockStyle.Fill;
            this.MyExperimentGUI.Name = "ExperimentGUI";
            this.Controls.Add(this.MyExperimentGUI);
            this.GUI_List.Add(this.MyExperimentGUI);
            this.MyExperimentGUI.Visible = false;
        }


        void SettingChanged(RadioButton b)
        {
            string name = b.Name;

            foreach (var setting in this.GUI_List)
            {
                setting.Visible = false;
            }

            switch (name)
            {
                case "radioButtonGraphGUI":
                    this.GUI_List.First(s => s.Name == "GraphGUI").Visible = true;
                    break;
                case "radioButtonAgentGUI":
                    this.GUI_List.First(s => s.Name == "AgentGUI").Visible = true;
                    break;
                case "radioButtonLearningGUI":
                    this.GUI_List.First(s => s.Name == "LearningGUI").Visible = true;
                    break;
                case "radioButtonExperimentGUI":
                    this.GUI_List.First(s => s.Name == "ExperimentGUI").Visible = true;
                    break;
                default:
                    break;
            }
        }

        #region Event

        private void radioButtonSetting_CheckedChanged(object sender, EventArgs e)
        {
            this.SettingChanged(sender as RadioButton);
        }

        private void checkBoxMenu_CheckedChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized) return;

            if (this.checkBoxMenu.Checked)
            {
                this.Height = 150;
            }
            else
            {
                this.Height = 800;
            }
        }

        #endregion

        void PlayStep()
        {
            var control_seed = (int)this.numericUpDownControlSeed.Value;
            var control_speed = (int)this.numericUpDownSpeedControl.Value;
            var max_steps = (int)this.numericUpDownStepsControl.Value;

            control_seed += int.Parse(this.labelStepNum.Text);
            control_seed += int.Parse(this.labelRoundNum.Text);

            this.MyOSM.UpdateSteps(control_speed, osm_setting.bad_sensor_mode, osm_setting.opinion_share_num);

            this.labelStepNum.Text = this.MyOSM.CurrentStep.ToString();
            this.labelRoundNum.Text = this.MyOSM.CurrentRound.ToString();
            this.MyAnimationForm.UpdatePictureBox();

            if (this.radioButtonRoundCheck.Checked)
            {
                if (max_steps <= this.MyOSM.CurrentStep)
                {
                    this.PlayRound();
                    this.MyOSM.PrintRoundInfo();
                    this.MyOSM.InitializeRound(osm_setting.opinion_share_num);
                }
            }

        }

        void PlayRound()
        {
            this.MyOSM.RecordRound();
            this.MyOSM.FinalizeRound();
        }

        private void timerAnimation_Tick(object sender, EventArgs e)
        {
            this.PlayStep();
        }


        bool PlayStopFlag;
        void ChangePlayButton(bool turn_mode)
        {
            if (turn_mode)
            {
                this.PlayStopFlag = !this.PlayStopFlag;
            }

            if (!this.PlayStopFlag)
            {
                this.timerAnimation.Enabled = true;
                this.radioButtonPlay.Image = Properties.Resources.icon_pause;
            }
            else
            {
                this.timerAnimation.Enabled = false;
                this.radioButtonPlay.Image = Properties.Resources.icon_play;
            }

        }

        private void radioButtonPlay_Click(object sender, EventArgs e)
        {
            this.ChangePlayButton(true);
        }

        internal void PlayStop()
        {
            this.PlayStopFlag = true;
            this.ChangePlayButton(false);
            if (this.MyOSM == null) return;

            if (this.radioButtonStepCheck.Checked)
            {
                this.MyOSM.InitializeToFirstStep(osm_setting.opinion_share_num);
                this.labelStepNum.Text = this.MyOSM.CurrentStep.ToString();
            }
            else if (this.radioButtonRoundCheck.Checked)
            {
                this.MyOSM.InitializeToFirstRound(osm_setting.opinion_share_num);
                this.labelRoundNum.Text = this.MyOSM.CurrentRound.ToString();
                this.labelStepNum.Text = this.MyOSM.CurrentStep.ToString();

            }
            this.MyAnimationForm.UpdatePictureBox();
        }

        private void radioButtonPlayStop_Click(object sender, EventArgs e)
        { 
            this.PlayStop();
        }

        private void radioButtonPlayStep_Click(object sender, EventArgs e)
        {
            this.PlayStep();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
