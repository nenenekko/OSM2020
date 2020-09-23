using CsvHelper;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
using OSM2020.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.Component
{
    class RecordRound
    {
        public int Round { get; private set; }
        public List<int> CorrectSizes { get; private set; }
        public List<int> IncorrectSizes { get; private set; }
        public List<int> UndeterSizes { get; private set; }
        public List<int> StepMessageSizes { get; private set; }
        public List<int> ActiveSensorSizes { get; private set; }
        public List<int> ActiveAgentSizes { get; private set; }
        public List<int> DeterminedSensorSizes { get; private set; }
        public List<int> SensorSizes { get; private set; }
        public List<double> CorrectSensorSizeRates { get; private set; }
        public List<int> NetworkSizes { get; private set; }
        public List<int> FinalSteps { get; private set; }
        public List<double> AverageWeight { get; private set; }
        public List<double> VarWeight { get; private set; }
        public Dictionary<Agent, Vector<double>> AgentReceiveOpinionsInRound { get; private set; }
        public Dictionary<Agent, Dictionary<Agent, Vector<double>>> ReceiveOpinionsInRound { get; private set; } //receive,send
        public SubjectManager MySubjectManager { get; private set; }
        public Dictionary<OpinionSubject, Dictionary<int, List<int>>> AllOpinionSizes;
        public List<double> SimpsonsDs { get; private set; }
        public List<double> BetterSimpsonsDs { get; private set; }

        public List<List<int>> CommunityList;
        public List<int> BadCommunityList;
        public List<int> BadCommuintyCorrectSizes { get; private set; }
        public List<int> BadCommunityIncorrectSizes { get; private set; }
        public List<int> BadCommunityUndeterSizes { get; private set; }

        public List<int> NormalCommuintyCorrectSizes { get; private set; }
        public List<int> NormalCommunityIncorrectSizes { get; private set; }
        public List<int> NormalCommunityUndeterSizes { get; private set; }

        public RecordRound()
        {

        }

        public RecordRound(int cur_round, List<Agent> agents, SubjectManager subject_manager)
        {
            this.Round = cur_round;
            this.MySubjectManager = subject_manager;
            this.AgentReceiveOpinionsInRound = new Dictionary<Agent, Vector<double>>(); //エージェントと受け取った意見の組
            this.ReceiveOpinionsInRound = new Dictionary<Agent, Dictionary<Agent, Vector<double>>>(); //エージェントと上記の組の組
            this.CorrectSizes = new List<int>(); 
            this.IncorrectSizes = new List<int>();
            this.UndeterSizes = new List<int>();
            this.StepMessageSizes = new List<int>();
            this.ActiveSensorSizes = new List<int>();
            this.ActiveAgentSizes = new List<int>();
            this.DeterminedSensorSizes = new List<int>();
            this.SensorSizes = new List<int>();
            this.CorrectSensorSizeRates = new List<double>();
            this.NetworkSizes = new List<int>();
            this.FinalSteps = new List<int>();
            this.AverageWeight = new List<double>();
            this.VarWeight = new List<double>();
            this.AllOpinionSizes = new Dictionary<OpinionSubject, Dictionary<int, List<int>>>();
            this.SimpsonsDs = new List<double>();
            this.BetterSimpsonsDs = new List<double>();

            this.CommunityList = new List<List<int>>();
            this.BadCommunityList = new List<int>();
            this.BadCommuintyCorrectSizes = new List<int>();
            this.BadCommunityIncorrectSizes = new List<int>();
            this.BadCommunityUndeterSizes = new List<int>();
            this.NormalCommuintyCorrectSizes = new List<int>();
            this.NormalCommunityIncorrectSizes = new List<int>();
            this.NormalCommunityUndeterSizes = new List<int>();

            foreach (var receive_agent in agents) //各ノードについて
            {
                var undeter_op = receive_agent.InitOpinion.Clone(); //意見をコピー
                undeter_op.Clear(); //意見を初期化(未形成状態)
                this.AgentReceiveOpinionsInRound.Add(receive_agent, undeter_op); //エージェントと初期意見の組を追加

                var send_rec_op_in_round = new Dictionary<Agent, Vector<double>>(); //そのラウンド内のエージェントと送信_受信意見の組
                foreach (var send_agent in receive_agent.GetNeighbors()) //receive_agentとリンクがつながっているエージェントを取得
                {
                    send_rec_op_in_round.Add(send_agent, undeter_op); //意見の送信相手と意見の値の組を追加
                }
                if (receive_agent.IsSensor)
                    send_rec_op_in_round.Add(this.MySubjectManager.OSM_Env.EnvironmentAgent, undeter_op); //ノードがセンサなら.環境とセンサの組も追加
                this.ReceiveOpinionsInRound.Add(receive_agent, send_rec_op_in_round); //ノードとそのノードの意見送信の記録の組を追加
            }

            foreach (var subject in this.MySubjectManager.Subjects)
            {
                var dim_dic = new Dictionary<int, List<int>>();
                foreach (var dim in Enumerable.Range(0, subject.SubjectDimSize)) //0番から(意見数-1)番
                {
                    dim_dic.Add(dim, new List<int>());
                }
                this.AllOpinionSizes.Add(subject, dim_dic);
            }
        }

        public void RecordFinalStep(int cur_step)
        {
            this.FinalSteps.Add(cur_step); //最終ステップ(数)をリストに追加
        }

        public void RecordStepAgentNetwork(List<Agent> agents)
        {
            var cor_dim = this.MySubjectManager.OSM_Env.CorrectDim;     //正解次元(意見)
            var cor_subject = this.MySubjectManager.OSM_Env.EnvSubject; //正解subject
            //var correct_size = agents.Where(agent => agent.MySubject.SubjectName == cor_subject.SubjectName && agent.GetOpinionDim() == cor_dim).Count();
            var correct_size = agents.Where(agent => agent.MySubject.SubjectName == cor_subject.SubjectName && agent.GetOpinionDim() == cor_dim).Count(); //subjectが一致していてかつ正解意見を持っているエージェントの数
            var undeter_size = agents.Where(agent => agent.GetOpinionDim() == -1).Count(); //まだ意見を形成していないエージェントの数
            var network_size = agents.Count; //エージェント(ノード)の数
            var incorrect_size = network_size - correct_size - undeter_size; //誤った意見を形成しているエージェントの数
            var sensor_size = agents.Where(agent => agent.IsSensor).Count(); //センサの数
            var correct_sensor_size_rate = agents.Where(agent => agent.IsSensor && agent.MySubject.SubjectName == cor_subject.SubjectName && agent.GetOpinionDim() == cor_dim).Count() / (double)sensor_size; //センサの中で正解意見を持っている割合
            var determined_sensor_size = agents.Where(agent => agent.IsSensor && agent.IsDetermined()).Count(); //意見を形成したセンサの数
            var ave_weights = agents.Select(agent => agent.AgentLinks.Average(link => link.GetWeight(agent))).Mean(); //ネットワークの重みの平均
            var var_weights = agents.Select(agent => agent.AgentLinks.Average(link => link.GetWeight(agent))).PopulationVariance(); //ネットワークの重みの分散
            //Bad Communityの話
            var bad_community_correct_size = agents.Where(agent => this.BadCommunityList.Contains(agent.AgentID) && agent.MySubject.SubjectName == cor_subject.SubjectName && agent.GetOpinionDim() == cor_dim).Count();
            var bad_community_undeter_size = agents.Where(agent => this.BadCommunityList.Contains(agent.AgentID) && agent.GetOpinionDim() == -1).Count(); //まだ意見を形成していないエージェントの数
            var bad_community_size = this.BadCommunityList.Count; //エージェント(ノード)の数
            var bad_community_incorrect_size = bad_community_size - bad_community_correct_size - bad_community_undeter_size; //誤った意見を形成しているエージェントの数

            var normal_community_correct_size = agents.Where(agent => !this.BadCommunityList.Contains(agent.AgentID) && agent.MySubject.SubjectName == cor_subject.SubjectName && agent.GetOpinionDim() == cor_dim).Count();
            var normal_community_undeter_size = agents.Where(agent => !this.BadCommunityList.Contains(agent.AgentID) && agent.GetOpinionDim() == -1).Count(); //まだ意見を形成していないエージェントの数
            var noraml_community_size = network_size - this.BadCommunityList.Count; //エージェント(ノード)の数
            var normal_community_incorrect_size = noraml_community_size - normal_community_correct_size - normal_community_undeter_size;

            var simpsond = 0.0;
            foreach (var dim in Enumerable.Range(0, cor_subject.SubjectDimSize)) //意見の種類数分(0から種類数-1)
            {
                var relative_dominance = agents.Where(agent => agent.MySubject.SubjectName == cor_subject.SubjectName && agent.GetOpinionDim() == dim).Count() / (double)agents.Count; //各意見を持つエージェントの総数/エージェントの総数
                simpsond += Math.Pow(relative_dominance, 2); //2乗して加算
            }
            simpsond = 1 - simpsond;

            var undeter_rate = Math.Round(bad_community_undeter_size / (double)network_size, 4); //未形成エージェントの総数/エージェントの総数を小数点4位でまるめる
            //var bad_community_undeter_rate = Math.Round(bad_community_undeter_size / (double)bad_community_size, 4);
            var better_simpsons_d = (simpsond * (1 - undeter_rate));

            this.CorrectSizes.Add(correct_size);
            this.UndeterSizes.Add(undeter_size);
            this.NetworkSizes.Add(network_size);
            this.IncorrectSizes.Add(incorrect_size);
            this.SensorSizes.Add(sensor_size);
            this.CorrectSensorSizeRates.Add(correct_sensor_size_rate);
            this.DeterminedSensorSizes.Add(determined_sensor_size);
            this.AverageWeight.Add(ave_weights);
            this.VarWeight.Add(var_weights);
            this.RecordAllOpinion(agents);
            this.SimpsonsDs.Add(simpsond);
            this.BetterSimpsonsDs.Add(better_simpsons_d);

            this.BadCommuintyCorrectSizes.Add(bad_community_correct_size);
            this.BadCommunityUndeterSizes.Add(bad_community_undeter_size);
            this.BadCommunityIncorrectSizes.Add(bad_community_incorrect_size);

            this.NormalCommuintyCorrectSizes.Add(normal_community_correct_size);
            this.NormalCommunityUndeterSizes.Add(normal_community_undeter_size);
            this.NormalCommunityIncorrectSizes.Add(normal_community_incorrect_size);
        }

        void RecordAllOpinion(List<Agent> agents)
        {
            foreach (var subject in this.AllOpinionSizes.Keys)
            {
                foreach (var dim in Enumerable.Range(0, subject.SubjectDimSize))
                {
                    var each_op_size = agents.Where(agent => agent.MySubject.SubjectName == subject.SubjectName && agent.GetOpinionDim() == dim).Count();
                    this.AllOpinionSizes[subject][dim].Add(each_op_size);
                }
            }
        }

        public void RecordStepMessages(List<Message> step_messages)
        {
            foreach (var step_message in step_messages) //今ステップの全てのメッセージ中の各message
            {
                Vector<double> receive_op = null;
                if (step_message.Subject.SubjectName != step_message.ToAgent.MySubject.SubjectName) //受信エージェントのsubjectとmessageのsubjectが一致してなければ
                {
                    var to_subject = step_message.ToAgent.MySubject; //受信エージェントのsubjectをコピー
                    receive_op = step_message.Subject.ConvertOpinionForSubject(step_message.Opinion, to_subject);
                }
                else
                {
                    receive_op = step_message.Opinion.Clone(); //意見をコピー
                }

                this.AgentReceiveOpinionsInRound[step_message.ToAgent] += receive_op; //意見を受け取ったエージェントに対応する配列に受け取った意見を加算→どのエージェントがどんな意見をどれくらいもらったか
                this.ReceiveOpinionsInRound[step_message.ToAgent][step_message.FromAgent] += receive_op; //toのagentIDとfromのagentIDが一致する要素に意見を加算する
            }

            //var active_sensor_size = step_messages.Where(message => message.FromAgent.AgentID < 0).Count();
            var active_sensor_size = step_messages.Where(message => message.FromAgent.AgentID < 0).Select(message => message.ToAgent.AgentID).Distinct().Count(); //環境から意見を受け取ったセンサの数
            //var active_agent_size = step_messages.Where(message => message.FromAgent.AgentID >= 0).Count();
            var active_agent_size = step_messages.Where(message => message.FromAgent.AgentID >= 0).Select(message => message.ToAgent.AgentID).Distinct().Count(); //エージェントから意見を受け取ったエージェントの数
            var step_message_size = step_messages.Count; //メッセージの数

            this.ActiveSensorSizes.Add(active_sensor_size); //それぞれ数をリストに追加
            this.ActiveAgentSizes.Add(active_agent_size);
            this.StepMessageSizes.Add(step_message_size);
        }

        public bool IsReceived(Agent agent)
        {
            if (this.AgentReceiveOpinionsInRound[agent].L2Norm() == 0) return false;
            return true;
        }

        public void PrintRecord()
        {
            double network_size = this.NetworkSizes.Last();

            Console.WriteLine(
               $"round:{this.Round:D4}|" +
               $"cor:{Math.Round(this.CorrectSizes.Last() / network_size, 3):F3}|" +
               $"incor:{Math.Round(this.IncorrectSizes.Last() / network_size, 3):F3}|" +
               $"undeter:{Math.Round(this.UndeterSizes.Last() / network_size, 3):F3}|" +
               $"simpsond:{Math.Round(this.SimpsonsDs.Last(), 3):F3}|" +
               $"bettersimpsond:{Math.Round(this.BetterSimpsonsDs.Last(), 3):F3}|"
               );

            foreach (var subject in this.MySubjectManager.Subjects)
            {
                Console.WriteLine($" -subject:{subject.SubjectName}|");
                foreach (var dim in Enumerable.Range(0, subject.SubjectDimSize))
                {
                    Console.Write($"  dim {dim}:");
                    Console.WriteLine($"{Math.Round(this.AllOpinionSizes[subject][dim].Last() / network_size, 3):F3}|");
                }
                //Console.WriteLine();
            }
        }

        public void SetCommunity(List<List<int>> community)
        {
            this.CommunityList = community;
        }

        public void SetBadCommunity(Agent bad_sensor)
        {
            foreach(List<int > community in this.CommunityList)
            {
                if (community.Contains(bad_sensor.AgentID))
                {
                    this.BadCommunityList = community;
                    break;
                }
            }
        }
    }
}
