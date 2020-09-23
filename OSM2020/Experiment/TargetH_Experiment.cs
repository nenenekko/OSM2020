using Konsole;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OSM2020.OSM;
using OSM2020.OSM.Algorithm.AAT;
using OSM2020.OSM.Component;
using OSM2020.OSM.Graph;
using OSM2020.OSM.GraphGenerator;
using OSM2020.OSM.LayoutGenerator;
using OSM2020.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OSM2020.Experiment
{
    class TargetH_Experiment
    {
        string ExperimentName;
        double SensorWeight;
        bool SensorSizeFixMode;
        int SensorSize;
        List<double> SensorSizeRates;
        List<double> MaliciousSensorSizeRates;
        double SensorCommonWeight;
        List<double> EnvDistWeights;
        List<double> MaliciousEnvDistWeights;
        BeliefUpdater MyBeliefUpdater;
        List<double> CommonWeights;
        double CommonCuriocity;
        string LogFolder;
        //int Round;
        List<int> Rounds;
        int Steps;
        List<GraphEnum> MyGraphs;
        List<AlgoEnum> MyAlgos;
        List<int> MyDims;
        List<double> TargetHs;
        List<double> InfoWeightRates;
        double OpinionThreshold;
        string SubjectName;
        public List<bool> IsDynamics;
        List<EnvDistributionEnum> EnvDisModes;
        List<int> NetworkSizes;

        List<bool> BadSensorMode;
        List<int> OpinionShareNum;
        List<SensorArrangementEnum> SensorArrangementMode;
        List<bool> IsAdditionalShareCommunityOnly;

        OSMSetting osm_setting;

        public List<string> PythonCsvPath = new List<string>();

        static object lock_object = new object();

        public TargetH_Experiment()
        {
            //this.SensorCommonWeightMode = false;
            this.SensorSizeFixMode = false;
            this.MyGraphs = new List<GraphEnum>(); //grid2Dなどのネットワークを生成するアルゴリズムをまとめたリスト
            this.MyAlgos = new List<AlgoEnum>();   //AATなどの意見共有アルゴリズムをまとめたリスト
            this.MyDims = new List<int>();         //次元のリスト
            this.IsDynamics = new List<bool>();    //
            this.ExperimentName = "TH";            //実験名．TargetH.ファイル名に記載される
            this.EnvDisModes = new List<EnvDistributionEnum>();  //つらら，エクスポネンシャル
            this.EnvDistWeights = new List<double>();
            this.MaliciousEnvDistWeights = new List<double>();
            this.InfoWeightRates = new List<double>();
            this.NetworkSizes = new List<int>();
            this.SensorSizeRates = new List<double>();
            this.MaliciousSensorSizeRates = new List<double>();
            this.CommonWeights = new List<double>();

            this.BadSensorMode = new List<bool>();
            this.OpinionShareNum = new List<int>();
            this.SensorArrangementMode = new List<SensorArrangementEnum>();

            this.PythonCsvPath = new List<string>();
        }

        public TargetH_Experiment SetAlgos(List<AlgoEnum> algos)
        {
            this.MyAlgos = algos;
            return this;
        }

        public TargetH_Experiment SetDims(List<int> dims)
        {
            this.MyDims = dims;
            return this;
        }

        public TargetH_Experiment SetGraphs(List<GraphEnum> graphs)
        {
            this.MyGraphs = graphs;
            return this;
        }

        public TargetH_Experiment SetNetworkSize(List<int> sizes)
        {
            this.NetworkSizes = sizes;
            return this;
        }

        public TargetH_Experiment SetSensorWeight(double sensor_weight)
        {
            this.SensorWeight = sensor_weight;
            return this;
        }

        public TargetH_Experiment SetLogFolder(string dt_name, string folder_name = "")
        {
            //var sensor_size_comment = this.SensorSizeFixMode ? $"fix{this.SensorSize}" : $"rate{this.SensorSizeRates}";
            //this.LogFolder = $"{dt_name}_{"th"}_dim{this.DimSize}_sr{this.SensorRate}_scw{this.SensorCommonWeight}_{sensor_size_comment}_cc{this.CommonCuriocity}_r{this.Rounds}_s{this.Steps}_" + folder_name;
            this.LogFolder = $"{dt_name}_{"th"}_" + folder_name;
            return this;
        }

        public TargetH_Experiment SetBeliefUpdater(BeliefUpdater belief_updater)
        {
            this.MyBeliefUpdater = belief_updater;
            return this;
        }

        public TargetH_Experiment SetSensorFixSize(int sensor_size)
        {
            this.SensorSizeFixMode = true;
            this.SensorSize = sensor_size;
            return this;
        }

        public TargetH_Experiment SetSensorSizeRate(List<double> sensor_size_rates)
        {
            this.SensorSizeFixMode = false;
            this.SensorSizeRates = sensor_size_rates;
            return this;
        }
        public TargetH_Experiment SetMaliciousSensorSizeRate(List<double> malicious_sensor_size_rates)
        {
            this.SensorSizeFixMode = false;
            this.MaliciousSensorSizeRates = malicious_sensor_size_rates;
            return this;
        }

        public TargetH_Experiment SetCommonWeights(List<double> common_weights)
        {
            this.CommonWeights = common_weights;
            return this;
        }
        public TargetH_Experiment SetSubjectName(string subject_name)
        {
            this.SubjectName = subject_name;
            return this;
        }

        public TargetH_Experiment SetEnvDistWeights(List<double> dist_weights)
        {
            this.EnvDistWeights = dist_weights;
            return this;
        }
        public TargetH_Experiment SetMaliciousEnvDistWeights(List<double> malicious_dist_weights)
        {
            this.MaliciousEnvDistWeights = malicious_dist_weights;
            return this;
        }

        public TargetH_Experiment SetInfoWeightRates(List<double> info_weight_rate)
        {
            this.InfoWeightRates = info_weight_rate;
            return this;
        }

        public TargetH_Experiment SetCommonCuriocity(double common_curiocity)
        {
            this.CommonCuriocity = common_curiocity;
            return this;
        }

        //public TargetH_Experiment SetRounds(int rounds)
        public TargetH_Experiment SetRounds(List<int> rounds)
        {
            this.Rounds = rounds;
            return this;
        }

        public TargetH_Experiment SetSteps(int steps)
        {
            this.Steps = steps;
            return this;
        }

        public TargetH_Experiment SetTargetHs(List<double> target_hs)
        {
            this.TargetHs = target_hs;
            return this;
        }

        public TargetH_Experiment SetOpinionThreshold(double op_threshold)
        {
            this.OpinionThreshold = op_threshold;
            return this;
        }
        public TargetH_Experiment SetDynamics(List<bool> is_dynamics)
        {
            this.IsDynamics = is_dynamics;
            return this;
        }

        public TargetH_Experiment SetEnvDistModes(List<EnvDistributionEnum> env_dist_modes)
        {
            this.EnvDisModes = env_dist_modes;
            return this;
        }

        public TargetH_Experiment SetBadSensorMode(List<bool> flag)
        {
            this.BadSensorMode = flag;
            return this;
        }

        public TargetH_Experiment SetOpinionShareNum(List<int> nums)
        {
            this.OpinionShareNum = nums;
            return this;
        }

        public TargetH_Experiment SetSensorArrangement(List<SensorArrangementEnum> arrangements)
        {
            this.SensorArrangementMode = arrangements;
            return this;
        }

        public TargetH_Experiment SetAdditionalShareCommunityOnly(List<bool> flag)
        {
            this.IsAdditionalShareCommunityOnly = flag;
            return this;
        }

        public TargetH_Experiment SetOsmSetting(OSMSetting osm_setting)
        {
            this.osm_setting = osm_setting;
            return this;
        }

        public void Run(int seed)
        {
            this.Run(seed, seed);  //同じシード入れてる 
        }


        public void Run(int start_seed, int final_seed)
        {
            string save_folder = this.LogFolder;
            var graphs = this.MyGraphs;
            var algos = this.MyAlgos;
            var dims = this.MyDims;

            //int op_dim_size = this.DimSize;
            double sensor_weight = this.SensorWeight;


            int max = 0;
            foreach (var size in this.NetworkSizes) //リスト中のサイズを一つ取り出す
            {
                foreach (var select_graph in graphs) //リスト中のグラフを一つ取り出す
                {
                    for (int seed = start_seed; seed <= final_seed; seed++) //スタートからファイナルまでシードをインクリメント
                    {
                        foreach (var algo in algos) //リスト中の意見共有アルゴリズムを一つ取り出す
                        {
                            foreach (var dim in dims) //リスト中の「意見数」を一つ取り出す
                            {
                                foreach (var target_h in this.TargetHs) //リスト中の目標意見形成率を一つ取り出す
                                {
                                    foreach (var is_dynamic in this.IsDynamics) //リスト中のダイナミクスを一つ取り出す
                                    {
                                        foreach (var env_dist_mode in this.EnvDisModes)
                                        {
                                            foreach (var env_dist_weight in this.EnvDistWeights) //環境からセンサエージェントへのリンクの重み
                                            {
                                                foreach (var mal_env_dist_weight in this.MaliciousEnvDistWeights) //誤った環境からセンサエージェントへのリンクの重み
                                                {
                                                    foreach (var info_weight_rate in this.InfoWeightRates)
                                                    {
                                                        foreach (var round in this.Rounds)  //リスト中のラウンドを一つ取り出す
                                                        {
                                                            foreach (var sensor_size_rate in this.SensorSizeRates)  //環境観測センサの比率
                                                            {
                                                                foreach (var mal_sensor_size_rate in this.MaliciousSensorSizeRates) //誤情報環境観測センサの比率
                                                                {
                                                                    foreach (var common_weight in this.CommonWeights)
                                                                    {
                                                                        foreach(var bad_sensor_mode in this.BadSensorMode)
                                                                        {
                                                                            foreach(var opinion_share_num in this.OpinionShareNum)
                                                                            {
                                                                                foreach (var sensor_arrangement in this.SensorArrangementMode)
                                                                                {
                                                                                    foreach (var add_share_only_community in this.IsAdditionalShareCommunityOnly)
                                                                                    {
                                                                                        max++;
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var pb = new ExtendProgressBar(max); //maxが総実行数．プログレスバーに100%の実行数を渡す

            foreach (var size in this.NetworkSizes) //サイズはネットワークのノード数
            {
                GraphGeneratorBase graph_generator = new Null_GraphGenerator(); // GraphGeneratorBase型として生成

                foreach (var select_graph in graphs)
                {
                    foreach (var op_dim_size in dims)  //意見の種類
                    {
                        foreach (var is_diynamic in this.IsDynamics)
                        {
                            foreach (var env_dist_mode in this.EnvDisModes)
                            {
                                foreach (var env_dist_weight in this.EnvDistWeights)
                                {
                                    foreach (var mal_env_dist_weight in this.MaliciousEnvDistWeights)
                                    {
                                        foreach (var info_weight_rate in this.InfoWeightRates)
                                        {
                                            foreach (var round in this.Rounds) //設定した「ラウンド数」のリストから一つ取り出す
                                            {
                                                foreach (var sensor_size_rate in this.SensorSizeRates) //センササイズ率
                                                {
                                                    foreach (var mal_sensor_size_rate in this.MaliciousSensorSizeRates) 
                                                    {
                                                        foreach (var common_weight in this.CommonWeights)
                                                        {
                                                            foreach (var bad_sensor_mode in this.BadSensorMode)
                                                            {
                                                                foreach (var opinion_share_num in this.OpinionShareNum)
                                                                {
                                                                    foreach (var sensor_arrangement in this.SensorArrangementMode)
                                                                    {
                                                                        foreach (var add_share_only_community in this.IsAdditionalShareCommunityOnly)
                                                                        {
                                                                            for (int seed = start_seed; seed <= final_seed; seed++)
                                                                            {
                                                                                switch (select_graph)
                                                                                {
                                                                                    case GraphEnum.WS:
                                                                                        graph_generator = new WS_GraphGenerator().SetNodeSize(size).SetNearestNeighbors(6).SetRewireP(0.01);
                                                                                        //graph_generator = new WS_GraphGenerator().SetNodeSize(size).SetNearestNeighbors(15).SetRewireP(0.1);
                                                                                        break;
                                                                                    case GraphEnum.BA:
                                                                                        graph_generator = new BA_GraphGenerator().SetNodeSize(size).SetAttachEdges(2);
                                                                                        //graph_generator = new BA_GraphGenerator().SetNodeSize(size).SetAttachEdges(10);
                                                                                        break;
                                                                                    case GraphEnum.Hexagonal:
                                                                                        graph_generator = new Hexagonal_GraphGenerator().SetNodeSize(size);
                                                                                        break;
                                                                                    case GraphEnum.Grid2D:
                                                                                        graph_generator = new Grid2D_GraphGenerator().SetNodeSize(size);//ノード数を渡す
                                                                                        break;
                                                                                    case GraphEnum.Triangular:
                                                                                        graph_generator = new Triangular_GraphGenerator().SetNodeSize(size);
                                                                                        break;
                                                                                    case GraphEnum.KarateClub:
                                                                                        graph_generator = new KarateClub_GraphGenerator().SetNodeSize(size);
                                                                                        break;
                                                                                    case GraphEnum.LFR:
                                                                                        graph_generator = new LFR_GraphGenerator().SetNodeSize(size);
                                                                                        break;
                                                                                    default:
                                                                                        new Exception();
                                                                                        return;
                                                                                }
                                                                                var graph = new RawGraph();
                                                                                var layout = new Layout();
                                                                                graph_generator.SetOsmSetting(osm_setting);

                                                                                lock (lock_object)
                                                                                {
                                                                                    graph = graph_generator.Generate(seed, pb); //pythonを使ってグラフ(ノードとリンク)生成
                                                                                    layout = new Circular_LayoutGenerator(graph).Generate(pb); //グラフに合わせたレイアウト生成
                                                                                }

                                                                                List<List<int>> communityList = new List<List<int>>(); //LFRでないときはnullでok
                                                                                if (bad_sensor_mode && select_graph == GraphEnum.LFR)
                                                                                {
                                                                                    communityList = graph_generator.GetCommunity();
                                                                                }


                                                                                var init_belief_gene = new InitBeliefGenerator() //信念値更新式生成
                                                                                                        .SetInitBeliefMode(mode: InitBeliefMode.NormalNarrow);

                                                                                var subject_test = new OpinionSubject(this.SubjectName, op_dim_size); //今回はName=test.OpinionSubjectにそのままセットしてインスタンス化

                                                                                var sample_agent_test = new SampleAgent() //信念値生成器，意見形成率，意見数，意見をもつ
                                                                                                    .SetInitBeliefGene(init_belief_gene) //信念値生成器をセット
                                                                                                    .SetThreshold(this.OpinionThreshold) //意見形成の閾値をセット
                                                                                                    .SetSubject(subject_test) //サブジェクトをセット
                                                                                                    .SetInitOpinion(Vector<double>.Build.Dense(op_dim_size, 0.0)); //InitOpinionに意見数分,0.0の初期値でinit

                                                                                var sensor_gene = new SensorGenerator(); //エージェント生成器
                                                                                if (this.SensorSizeFixMode)
                                                                                {
                                                                                    //you will add malicious sensor size 
                                                                                    sensor_gene.SetSensorSize(this.SensorSize);
                                                                                }
                                                                                else
                                                                                {
                                                                                    int sensor_size = (int)(sensor_size_rate * graph.Nodes.Count); //nodeの数×センササイズ比＝(正情報)環境観測センサの数
                                                                                    int mal_sensor_size = (int)(mal_sensor_size_rate * graph.Nodes.Count); //nodeの数×誤情報センササイズ比＝誤情報環境観測センサの数
                                                                                    sensor_gene.SetSensorSize(sensor_size, mal_sensor_size); //そのままセンササイズをセット
                                                                                }

                                                                                int agent_gene_seed = seed; //シードをセット
                                                                                var agent_gene_rand = new ExtendRandom(agent_gene_seed); //ランダム生成器

                                                                            
                                                                                var agent_network = new AgentNetwork() //エージェントとリンクを生成
                                                                                                        .SetRand(agent_gene_rand) //ランダム生成器セット
                                                                                                        .GenerateNetworkFrame(graph) //pythonで生成したエージェントとリンクをAgentsクラス，AgentLinksクラスに結びつける
                                                                                                        .ApplySampleAgent(sample_agent_test, mode: SampleAgentSetMode.RemainSet) //サンプルエージェント，セットモード

                                                                                                        /*センサの設定*/
                                                                                                        .SetBadSensorMode(bad_sensor_mode)
                                                                                                        .SetCommnityList(communityList)
                                                                                                        .SetCommunityId()
                                                                                                        .GenerateSensor(sensor_gene)//各エージェントに対して，一定の確率で選ばれたエージェントを正情報センサ，誤情報センサとする
                                                                                                        .SetLayout(layout); //AgenetNetworkにレイアウトをセット

                                                                                /***************************************多分ここで環境設定終わり**********************************************/

                                                                                int update_step_seed = seed; //シードセット

                                                                                foreach (var algo in algos) //設定したアルゴリズムを順番に試すよ
                                                                                {
                                                                                    OSMBase osm = new OSM_Only(); //とりあえずベースとなるクラスで初期化
                                                                                    foreach (var target_h in this.TargetHs) //意見形成目標率を順番に試すよ
                                                                                    {
                                                                                        var sample_size = 10; //現在は使ってない説
                                                                                        var awa_rate_window = 100; //こっちは使ってない?
                                                                                        var info_value_window = 50;
                                                                                        //var info_value_window = 100;
                                                                                        var op_intro_interval = osm_setting.op_intro_interval;
                                                                                        var op_intro_rate = 0.1;
                                                                                        var dinamic_interval = 25; //is_dinamicの時じゃないと使わん説

                                                                                        var algo_extend = algo.ToString(); //文字列化
                                                                                        switch (algo)
                                                                                        {
                                                                                            case AlgoEnum.OSMonly:
                                                                                                var osm_only = new OSM_Only();
                                                                                                osm_only.SetCommonWeight(common_weight);
                                                                                                osm = osm_only;
                                                                                                break;
                                                                                            case AlgoEnum.AAT:
                                                                                                var osm_aat = new AAT_OSM();  //AATをセット
                                                                                                osm_aat.SetTargetH(target_h); //AATモデルに意見形成目標率をセット
                                                                                                osm = osm_aat; //オーバーライドされる関数が出てくることに注意
                                                                                                break;
                                                                                            case AlgoEnum.CCAAT:
                                                                                                var osm_ccaat = new CCAAT_OSM();  //CCAATをセット
                                                                                                osm_ccaat.SetTargetH(target_h); //CCAATモデルに意見形成目標率をセット
                                                                                                osm = osm_ccaat; //オーバーライドされる関数が出てくることに注意
                                                                                                break;
                                                                                        }

                                                                                        var update_step_rand_tmp = new ExtendRandom(update_step_seed); //ランダム生成器
                                                                                        var subject_mgr_dic = new Dictionary<int, SubjectManager>(); //subjectManagerディクショナリを生成
                                                                                        if (is_diynamic) //ダイナミックなら
                                                                                        {
                                                                                            for (int i = 0; i < 100; i++)
                                                                                            {
                                                                                                subject_mgr_dic.Add(i * dinamic_interval, new SubjectManagerGenerator().Generate(subject_test, env_dist_weight, i % op_dim_size, sensor_weight, env_dist_mode));//実験タイトル，
                                                                                            }

                                                                                        }
                                                                                        else //subject_testはsubjectNameと意見数を含むOpinion.envDistweightは環境からセンサへの重み.sensor_weightは
                                                                                        {
                                                                                            subject_mgr_dic.Add(0, new SubjectManagerGenerator().Generate(subject_test, env_dist_weight, 0, sensor_weight, env_dist_mode, 1, mal_env_dist_weight)); //正解次元，誤り次元を0,1とするsubjectManagerを生成
                                                                                        }
                                                                                        osm.SetSubjectManagerDic(subject_mgr_dic); //サブジェクトマネージャーディクショナリをセット(OSM環境などが含まれる)
                                                                                        osm.SetRand(update_step_rand_tmp); // Update_Step_Randにシードを設定
                                                                                        osm.SetAgentNetwork(agent_network, opinion_share_num); //agenet_network(リンク，ノード，グラフ，センサ情報など)をセット.各エージェントについて重み候補集合を作成しリンクに初期重みをセット
                                                                                        osm.SetInitWeightsMode(mode: CalcWeightMode.FavorMyOpinion); //ウェイトinitの方法をセット
                                                                                        osm.SetOpinionIntroInterval(op_intro_interval); //環境を観測する間隔．オピニオンインターバルを追加(単純に上で指定)
                                                                                        osm.SetOpinionIntroRate(op_intro_rate); //実際に環境を観測するセンサの割合.オピニオンイントロ率をセット(単純に上で指定)
                                                                                        osm.SimpleRecordFlag = true;
                                                                                        osm.SetBeliefUpdater(this.MyBeliefUpdater); //信念値更新モデルセットするだけ(MainFormで指定)

                                                                                        osm.SetAddShareCommunityOnly(add_share_only_community);

                                                                                        if (bad_sensor_mode && select_graph == GraphEnum.LFR)
                                                                                        {
                                                                                            osm.SetCommunityList(communityList);
                                                                                        }

                                                                                        pb.Tag = $"{select_graph.ToString()} {size.ToString()} {algo_extend.ToString()} {op_dim_size} {seed}"; //グラフの形状，ノード数，アルゴリズム，意見数，シードを文字列に

                                                                                        /***************実験***************/
                                                                                        osm.Execute(round, this.Steps, bad_sensor_mode, opinion_share_num, pb); //ここで1セット分の実験を行う．ラウンド数，ステップ数，プログレスバー
                                                                                        /**********************************/

                                                                                        string sensor_size_mode = "";
                                                                                        if (this.SensorSizeFixMode)
                                                                                        {
                                                                                            sensor_size_mode = "fix" + this.SensorSize.ToString();
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            sensor_size_mode = "rate" + Math.Round(sensor_size_rate, 3).ToString(); //math.Roundは少数第三位までに丸め込む
                                                                                        }

                                                                                        string sensor_weight_mode = $"{this.MyBeliefUpdater.SensorWeightMode}"; //センサウェイトモードの名前を文字列に

                                                                                        var exp_setting = new ExperimentSetting(); //実験設定
                                                                                        exp_setting.Algorithm = algo.ToString();   //アルゴリズム名を文字列に
                                                                                        exp_setting.AwaRateWindowSize = awa_rate_window;
                                                                                        exp_setting.BeliefUpdater = this.MyBeliefUpdater.MyBeliefUpdateFunctionMode.ToString(); //センサの信念値更新式を文字列に
                                                                                        exp_setting.CommonCuriocity = this.CommonCuriocity; //MainFormでセット
                                                                                        exp_setting.Dim = op_dim_size; //意見数
                                                                                        exp_setting.EnvDistWeight = env_dist_weight;
                                                                                        exp_setting.EnvDistMode = env_dist_mode.ToString();
                                                                                        exp_setting.GraphType = select_graph.ToString(); //グラフの名前
                                                                                        exp_setting.IsDynamic = is_diynamic;
                                                                                        exp_setting.InfoWeightRate = info_weight_rate;
                                                                                        exp_setting.NetworkSize = size; //ノード数
                                                                                        exp_setting.OpinionIntroInteval = op_intro_interval;
                                                                                        exp_setting.OpinionIntroRate = op_intro_rate;
                                                                                        exp_setting.Round = round;
                                                                                        exp_setting.SampleSize = sample_size;
                                                                                        exp_setting.SensorRate = sensor_weight;
                                                                                        exp_setting.SensorSize = sensor_gene.SensorSize;
                                                                                        exp_setting.SensorSizeMode = this.SensorSizeFixMode.ToString();
                                                                                        exp_setting.SensorWeightMode = sensor_weight_mode;
                                                                                        exp_setting.Step = this.Steps;
                                                                                        exp_setting.TargetAwareness = target_h;

                                                                                        var json_exp_each_setting = JsonConvert.SerializeObject(exp_setting, Formatting.Indented);

                                                                                        var additional_setting = "";
                                                                                        if (bad_sensor_mode)
                                                                                            additional_setting += "BM";
                                                                                        else
                                                                                            additional_setting += "NM";

                                                                                        var output_path = Properties.Settings.Default.OutputLogPath //ファイル出力パス
                                                                                        + $"/{save_folder}/data/"
                                                                                        + $"S{seed}_"
                                                                                        + select_graph.ToString()
                                                                                        + $"_{additional_setting}"
                                                                                        + $"_{size.ToString()}"
                                                                                        + $"_SN{opinion_share_num.ToString()}"
                                                                                        + $"_{algo_extend.ToString()}"
                                                                                        //+ $"_{op_dim_size}"
                                                                                        //+ $"_{is_diynamic}"
                                                                                        + $"_{target_h}"
                                                                                        + $"_{env_dist_mode.ToString()}"
                                                                                        //+ $"_{env_dist_weight}"
                                                                                        + $"_{info_weight_rate}"
                                                                                        + $"_{round}"
                                                                                        + $"_{sensor_size_mode}"
                                                                                        //+ $"_{mal_sensor_size_rate}"
                                                                                        + $"_{common_weight.ToString()}";
                                                                                        //+ $"_{est_func.ToString()}"
                                                                                        //+ $"_";

                                                                                        //Output.OutputRounds(output_pass, osm.MyRecordRounds, seed.ToString());
                                                                                        Output.OutputRounds(output_path, osm.MyRecordRounds, json_exp_each_setting, seed.ToString());

                                                                                        string python_path = "C:/Users/nekko/Documents/TakadamaLab/OSM/OSM2020/OSM2020/bin/Debug/";
                                                                                        string[] path_split = output_path.Split('/'); //[1]にOutputLog,[2]に条件付きファイル名
                                                                                        python_path += path_split[1] + '/' + osm_setting.python_name + " " + path_split[2];
                                                                                        this.PythonCsvPath.Add(python_path);

                                                                                        pb.Next();
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Output.GraphicalizeWithPython(this.PythonCsvPath);
        }
    }
}
