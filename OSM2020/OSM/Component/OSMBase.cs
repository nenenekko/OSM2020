using Konsole;
using MathNet.Numerics.LinearAlgebra;
using OSM2020.Utility;
using OSM2020.OSM.Algorithm.SWT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.Component
{
    abstract class OSMBase : I_OSM
    {
        public AgentNetwork MyAgentNetwork { get; set; }
        public int CurrentStep { get; set; }
        public int CurrentRound { get; set; }

        protected ExtendRandom UpdateStepRand;
        public OpinionEnvironment MyEnvManager { get; protected set; }
        public Dictionary<int, SubjectManager> MySubjectManagerDic { get; protected set; }
        public List<int> MySensorChangeRoundList { get; protected set; }
        public SubjectManager MySubjectManager { get; protected set; }
        public double OpinionIntroRate { get; protected set; }
        public double OpinionIntroInterval { get; protected set; }
        public CalcWeightMode MyCalcWeightMode { get; protected set; }
        public AggregationFunctions MyAggFuncs { get; protected set; }
        protected List<Message> Messages;
        protected List<Agent> OpinionFormedAgents;
        //public RecordStep MyRecordStep { get; set; }
        //public Dictionary<int, RecordRound> MyRecordRounds { get; set; }

        protected List<Agent> AdditionalMessangerAgents;
        public List<List<int>> CommunityList;
        public List<int> StopRound;
        public bool IsAdditionalShareCommunityOnly;

        public double CommonWeight { get; private set; }
        public bool CommonWeightMode { get; private set; }
        public RecordRound MyRecordRound { get; set; }
        public List<RecordRound> MyRecordRounds { get; set; }
        public bool SimpleRecordFlag { get; set; }
        public BeliefUpdater MyBeliefUpdater { get; protected set; }
        public List<LinkInfoValue> LinkInfoValues { get; protected set; }
        public int LinkInfoValueWindowSize { get; protected set; }

        public OSMBase()
        {
            this.CommonWeight = 0.0;
            this.CommonWeightMode = false;
            this.CurrentStep = 0;
            this.CurrentRound = 0;
            //this.SensorCommonWeightMode = false;
            this.SimpleRecordFlag = false;
            this.MyAggFuncs = new AggregationFunctions();
            Messages = new List<Message>();
            OpinionFormedAgents = new List<Agent>();
            AdditionalMessangerAgents = new List<Agent>();
            //this.MyRecordRounds = new Dictionary<int, RecordRound>();
            //this.MyRecordSteps = new Dictionary<int, RecordStep>();
            this.MyRecordRound = new RecordRound();
            this.MyRecordRounds = new List<RecordRound>();
            this.LinkInfoValues = new List<LinkInfoValue>();
            this.LinkInfoValueWindowSize = 10;
        }

        public void SetRand(ExtendRandom update_step_rand)
        {
            this.UpdateStepRand = update_step_rand;
            return;
        }

        public virtual void SetAgentNetwork(AgentNetwork agent_network, int opinion_share_num) //opinion_share_numはオーバーライド用
        {
            this.MyAgentNetwork = agent_network;

            if (this.CommonWeightMode)
            {
                foreach (var link in this.MyAgentNetwork.AgentLinks)
                {
                    link.SetInitSourceWeight(this.CommonWeight);
                    link.SetInitTargetWeight(this.CommonWeight);
                }
            }

            this.SetLinkInfoValues(agent_network); //linkInfoValueを初期化
            return;
        }

        public virtual void SetAgentNetwork()
        {
            Debug.Assert(this.MyAgentNetwork != null);
            this.MyAgentNetwork.GenerateSensor(); //誤情報，正情報センサを設定
        }

        protected void SetLinkInfoValues(AgentNetwork agent_network)
        {
            this.LinkInfoValues = new List<LinkInfoValue>();
            foreach (var agent in agent_network.Agents) //agent一つ一つに対して
            {
                foreach (var neighbor_agent in agent.GetNeighbors()) //リンクでつながる隣人一人一人(そのエージェントの持つリンク数分)
                {
                    this.LinkInfoValues.Add(new LinkInfoValue(agent, neighbor_agent, this.LinkInfoValueWindowSize)); //receiver,sender,weightの順 .linkinfovalueはSWT用
                }
            }
        }

        public void SetSubjectManagerDic(Dictionary<int, SubjectManager> subject_manager_dic)
        {
            this.MySubjectManagerDic = subject_manager_dic;
        }

        public void SetSensorChangeRoundList(List<int> sensor_change_round_list)
        {
            this.MySensorChangeRoundList = sensor_change_round_list;
        }

        public void SetSubjectManager(SubjectManager subject_mgr)
        {
            this.MySubjectManager = subject_mgr; //受け取ったサブジェクトマネージャーをOSMにセット
            this.MyEnvManager = subject_mgr.OSM_Env; //OSMのEnvManagerに環境をセット
            this.MyEnvManager.AddEnvironment(this.MyAgentNetwork); //MyEnvManagerのEnvironmentAgentにエージェントネットワークをセット
            return;
        }

        public void SetOpinionIntroRate(double op_intro_rate)
        {
            this.OpinionIntroRate = op_intro_rate;
            return;
        }

        public void SetOpinionIntroInterval(int interval_step)
        {
            this.OpinionIntroInterval = interval_step;
            return;
        }

        public void SetInitWeightsMode(CalcWeightMode mode)
        {
            this.MyCalcWeightMode = mode;
            return;
        }

        public virtual void SetCommonWeight(double common_weight)
        {
            this.CommonWeight = common_weight;
            this.CommonWeightMode = true;
        }

        public void SetLinkInfoValueWindowSize(int size)
        {
            this.LinkInfoValueWindowSize = size;
        }

        public virtual void SetBeliefUpdater(BeliefUpdater belief_updater)
        {
            this.MyBeliefUpdater = belief_updater;
        }

        public void SetCommunityList(List<List<int>> community)
        {
            this.CommunityList = community;
        }

        public void SetStopRound(List<int> stop_rounds)
        {
            this.StopRound = stop_rounds;
        }

        public void SetAddShareCommunityOnly(bool flag)
        {
            this.IsAdditionalShareCommunityOnly = flag;
        }

        public bool GetAddShareCommunityOnly()
        {
            return this.IsAdditionalShareCommunityOnly;
        }

        //step
        public virtual void InitializeToFirstStep(int opinion_share_num)
        {
            if (this.MySubjectManagerDic.ContainsKey(this.CurrentRound))
            {
                //this.SetAgentNetwork(); //MyAgentNetworkに正情報，誤情報センサを設定
                this.SetSubjectManager(this.MySubjectManagerDic[this.CurrentRound]); //OSMに環境をセット
            }

            foreach (var agent in this.MyAgentNetwork.Agents) //ネットワーク中のノード一つずつ
            {
                agent.SetBelief(agent.InitBelief.Clone()); //領域を確保して値をコピーしてセット
                agent.Opinion = agent.InitOpinion.Clone(); //もしかしたらここ意味ないかも？既に信念値はセットされてる
                agent.SetOpinionForm(false);
            }
            this.CurrentStep = 0; //CurrentStepを初期化
            this.Messages.Clear(); //Messsagesを初期化
            this.OpinionFormedAgents.Clear(); //OpinonFormedAgents(意見形成したエージェントのリスト)を初期化
            this.MyRecordRound = new RecordRound(this.CurrentRound, this.MyAgentNetwork.Agents, this.MySubjectManager);
            if(this.CommunityList != null)
            {
                this.MyRecordRound.SetCommunity(this.CommunityList);
                Agent bad_agent = this.MyAgentNetwork.Agents.Where(agent => agent.GetBadSensor()).ToList().First();
                this.MyRecordRound.SetBadCommunity(bad_agent);
            }
        }

        public virtual void InitializeStep()
        {
            this.Messages.Clear(); //messagesを初期化
        }

        public virtual void NextStep(bool bad_sensor_mode ,int opinion_share_num) //ステップの数だけ呼ばれる
        {
            //sensor observe
            if (this.CurrentStep % this.OpinionIntroInterval == 0) //ステップ数がOpinionIntroIntervalの倍数(-1)のとき
            {
                var all_sensors = this.MyAgentNetwork.Agents.Where(agent => agent.IsSensor).ToList(); //all_sensorsにはセンサエージェントを入れる
                var observe_num = (int)Math.Ceiling(all_sensors.Count * this.OpinionIntroRate); //観測数＝センサ数×OpinionIntroRate(小数点以下切り上げ)
                var observe_sensors = all_sensors.Select(agent => agent.AgentID).OrderBy(a => this.UpdateStepRand.Next()).Take(observe_num).Select(id => this.MyAgentNetwork.Agents[id]).ToList();//観測したセンサをセンサIDの低い順に設定
                var env_messages = this.MyEnvManager.SendMessages(observe_sensors, this.UpdateStepRand, bad_sensor_mode); //引数は観測したセンサ，乱数生成器．誰から誰に(どのリンクで)どんなsubjectかをまとめたmessageが観測数分
                Messages.AddRange(env_messages); //env_messageを追加
            }

            //agent observe
            var op_form_messages = this.AgentSendMessages(this.OpinionFormedAgents); //opinionFormedAgentsはエージェントのリスト.初期値は要素なし．前ステップで意見更新したエージェントのリスト
            var already_op_form_agents= this.MyAgentNetwork.Agents.Where(agent => agent.GetOpinionForm() && agent.GetDispatchNum() < opinion_share_num ).ToList();//&&OpinionFormedAgents.IndexOf(agent) == -1
            if (this.GetAddShareCommunityOnly())
            {
                already_op_form_agents = already_op_form_agents.Where(agent => agent.GetBadCommunity()).ToList();
            }
            if (op_form_messages.Count != 0)
              Messages.AddRange(op_form_messages); //op_form_messagesもMessagesに追加
            foreach(var agent in this.OpinionFormedAgents) //前ステップで意見形成したagentの意見発信数を1に(この後すぐ意見発信するので)
            {
                agent.SetDispatchNum(1);
            }
            this.OpinionFormedAgents.Clear(); //OpinionFormedAgentsを初期化．今回のステップで意見形成したエージェントを本ステップ終了前に追加する．

            //既に意見を形成しているエージェントが再度意見発信する
            foreach(var agent in already_op_form_agents)
            {
                List<int> a = new List<int>();
                if (this.UpdateStepRand.Next(100) < 10)
                {
                    AdditionalMessangerAgents.Add(agent);
                    agent.SetDispatchNum(agent.GetDispatchNum()+1);
                }
            }
            var AdditionalMessages = this.AgentSendMessages(AdditionalMessangerAgents);
            if (AdditionalMessangerAgents.Count != 0)
                Messages.AddRange(AdditionalMessages);
            this.AdditionalMessangerAgents.Clear();
            //agent receive
            foreach (var message in this.Messages) //メッセージはこのステップ中に伝搬された全ての意見(リンク，誰から誰，どんな話題など全て含む)
            {
                //ソースが環境でなく，受け手が既に意見形成していて，かつ受けとった意見が自分と同じとき
                if (message.MyAgentLink.AgentLinkID >= 0 && message.ToAgent.GetOpinionForm()) {
                    if(message.Opinion[0] == message.ToAgent.Opinion[0])
                    {
                        message.MyAgentLink.AddSameOpinion();
                    }
                    else
                    {
                        message.MyAgentLink.AddDifferentOpinon();
                    }
                }
                var same_opinion_rate = message.MyAgentLink.GetSameOpinionRate();
                this.UpdateBeliefByMessage(message,same_opinion_rate); //意見を受け取ったエージェントの信念値を更新
                var op_form_agent = this.UpdateOpinion(message); //更新された信念値に基づいて意見形成を試みる．意見形成すれば受信エージェントが，しなければnulが返る．なお既に同一意見で意見形成されている場合はnull
                if (op_form_agent != null)
                {
                    this.OpinionFormedAgents.Add(op_form_agent); //意見形成したエージェントをリストに追加．次回のステップで使用される
                    op_form_agent.SetOpinionForm(true);
                }
            }

            this.CurrentStep++; //ステップ更新
        }

        public virtual void RecordStep(bool final) //finalは最終ステップのみtrue.MyRecordRoundに結果を保存
        {
            if (this.SimpleRecordFlag) //flagがtrueなら.全ステップ記録するか否か
            {
                if (final) this.MyRecordRound.RecordFinalStep(this.CurrentStep); //最終ステップ数をリストに追加するだけ
                this.MyRecordRound.RecordStepMessages(this.Messages); //Messagesを突っ込む.意見の数，受信ノードのうちの環境数及びエージェント数，誰が誰にどのような意見をどれくらい送ったか，誰がどれくらい意見をもらったかを記録
                if (final) this.MyRecordRound.RecordStepAgentNetwork(this.MyAgentNetwork.Agents);
            }
            else
            {
                this.MyRecordRound.RecordFinalStep(this.CurrentStep);
                this.MyRecordRound.RecordStepMessages(this.Messages);
                this.MyRecordRound.RecordStepAgentNetwork(this.MyAgentNetwork.Agents);
            }
        }

        public virtual void FinalizeStep()
        {
        }

        protected virtual void UpdateLinkInfoValues(RecordRound record_round)
        {
            foreach (var link_info_value in this.LinkInfoValues) //双方向なのでリンク数の倍
            {
                //if (link_info_value.SourceAgent.IsSensor) continue;
                //ReceiveOpinionsInRoundは全てのパターンについて初期化されているので意見を受け取っていない組に対しても実行される
                var received_sum_op = record_round.ReceiveOpinionsInRound[link_info_value.ReceiveAgent][link_info_value.SendAgent]; //receiveがsendから受け取った意見.
                var belief = link_info_value.ReceiveAgent.Belief; //receiveの信念値
                link_info_value.Regist(received_sum_op, belief);
            }
        }

        public virtual void UpdateSteps(int step_count, bool bad_sensor_mode ,int opinion_share_num)
        {
            foreach (var step in Enumerable.Range(0, step_count)) //0からstep_count-1
            {
                var final = (step == (step_count - 1)); //finalは最終stepの時のみtrue,それ以外はfalse
                this.InitializeStep(); //Messagesを初期化するだけ
                this.NextStep(bad_sensor_mode,opinion_share_num); //環境観測(数回に一回)，前ステップで意見形成したエージェントからの意見発信，信念値更新，意見形成，ステップ更新
                this.RecordStep(final); //記録を取る
                this.FinalizeStep(); //アルゴリズムによってオーバーライドしてる．なんもしない？？
            }
        }


        public virtual void PrintStepInfo()
        {
        }

        //round
        public virtual void InitializeToFirstRound(int opinion_share_num)
        {
            this.CurrentRound = 0;
            this.InitializeToFirstStep(opinion_share_num);
            this.MyRecordRounds = new List<RecordRound>();
            this.SetLinkInfoValues(this.MyAgentNetwork);
        }

        public virtual void InitializeRound(int opinion_share_num)
        {
            this.InitializeToFirstStep(opinion_share_num);
        }

        public virtual void NextRound(int step_count,bool bad_sensor_mode ,int opinion_share_num)
        {
            this.UpdateSteps(step_count,bad_sensor_mode ,opinion_share_num);
        }

        public virtual void RecordRound()
        {
            this.UpdateLinkInfoValues(this.MyRecordRound);
            if (this.StopRound != null  && this.StopRound.Contains(this.MyRecordRound.Round))
            {
                Console.WriteLine("StopToSnapShot!!");
            }
            this.MyRecordRounds.Add(this.MyRecordRound); //Recordsが総まとめ．ここで最終的に確定したrecordRoundを追加する
        }

        public virtual void FinalizeRound()
        {
            this.CurrentRound++;
        }

        public virtual void Execute(int round_count, int step_count, bool bad_sensor_mode,int opinion_share_num ,ExtendProgressBar pb = null)
        {
            string ori_tag = "";
            if (pb != null) ori_tag = pb.Tag;

            foreach (var round in Enumerable.Range(0, round_count)) //ラウンド数分回す
            {
                this.InitializeRound(opinion_share_num); //初期設定.センサセット，エージェントの信念値初期化など
                this.NextRound(step_count,bad_sensor_mode,opinion_share_num); //ステップ数を渡す．1ラウンド分が実行される.記録も取る．
                this.RecordRound(); //↑でとった1ラウンドの記録の重みを更新してラウンドの記録とし，RecordRoundsに加える
                pb.RefreshWithoutChange($"{pb.Tag} {this.CurrentRound} ");
                this.FinalizeRound(); //AAT_OSMでオーバーライド．意見形成率の推定．リンクの重み更新．＋CurrentRound数をインクリメント
            }
        }

        public virtual void PrintRoundInfo()
        {
            this.MyRecordRound.PrintRecord();
        }


        //agent
        public virtual void PrintAgentInfo(Agent agent)
        {
            Console.WriteLine($"Agent ID: {agent.AgentID}");
            Console.WriteLine($"Sensor: {agent.IsSensor}");
            Console.WriteLine($"Belief");
            int dim = 0;
            foreach (var belief in agent.Belief.ToList())
            {
                Console.WriteLine($"- Dim: {dim} Value {belief:f4}");
                dim++;
            }

            var is_changed = agent.IsChanged();
            Console.WriteLine($"Opinion (Changed:{is_changed})");
            dim = 0;
            foreach (var op in agent.Opinion.ToList())
            {
                Console.WriteLine($"- Dim: {dim} Value {op}");
                dim++;
            }

            foreach (var link in agent.AgentLinks)
            {
                Console.WriteLine($"- Weight: {link.GetOtherAgent(agent).AgentID} Value {link.GetWeight(agent):f4}");
            }

            if (this.MyRecordRounds.Count == 0) return;
            var is_recived = this.MyRecordRound.IsReceived(agent);
            Console.WriteLine($"Receive Opinion (Received:{is_recived})");
            var receive_op = this.MyRecordRound.AgentReceiveOpinionsInRound[agent];
            dim = 0;
            foreach (var op in receive_op.ToList())
            {
                Console.WriteLine($"- Dim: {dim} Value {op}");
                dim++;
            }
        }

        //osm
        protected virtual void UpdateBeliefByMessage(Message message, double same_opinion_rate)
        {
            var updated_belief = this.MyBeliefUpdater.UpdateBelief(this, message, same_opinion_rate); //計算によって更新された信念値が返ってくる
            Debug.Assert(updated_belief.Max() <= 1.0);
            message.ToAgent.SetBelief(updated_belief); //意見を受け取ったエージェントの信念値を更新
        }

        protected virtual Agent UpdateOpinion(Message message)
        {
            var belief_list = message.ToAgent.Belief.ToList(); //意見を受信したエージェントの信念値をコピー
            var op_list = message.ToAgent.Opinion.ToList(); //エージェントの意見を持っているか否かのリストをコピー
            var op_threshold = message.ToAgent.OpinionThreshold; //意見形成の閾値をコピー

            for (int dim = 0; dim < belief_list.Count; dim++) //意見数分
            {
                if (belief_list[dim] > op_threshold && op_list[dim] != 1) //その意見について意見形成されていない状態で意見形成閾値を信念値が上回ったら
                {
                    message.ToAgent.Opinion.Clear();
                    message.ToAgent.Opinion[dim] = 1; //意見形成
                    return message.ToAgent; //意見形成したエージェントを返す
                }
            }
            return null; //意見形成しなかったらnullを返す
        }

        protected virtual List<Message> AgentSendMessages(List<Agent> op_formed_agents)
        {
            List<Message> messages = new List<Message>();
            foreach (var agent in op_formed_agents)
            {
                if (agent == null) continue;
                var opinion = agent.Opinion.Clone();
                foreach (var to_agent in agent.GetNeighbors())
                {
                    var agent_link = agent.AgentLinks.Where(link => link.SourceAgent == to_agent || link.TargetAgent == to_agent).First();
                    messages.Add(new Message(agent, to_agent, agent_link, opinion));
                }
            }

            //messages.RemoveAll(message => message.ToAgent.IsSensor && message.FromAgent.AgentID >= 0);

            return messages;
        }


        protected double GetObsU(Vector<double> received_sum_op)
        {
            return received_sum_op.Sum();

            //var max_op_len = received_sum_op.Max();
            //var max_index = received_sum_op.MaximumIndex();

            //for (int index = 0; index < received_sum_op.Count; index++)
            //{
            //    if (index == max_index) continue;
            //    max_op_len -= received_sum_op[index];
            //    if (max_op_len <= 0) return 0;
            //}
            //return max_op_len;
        }
    }
}
