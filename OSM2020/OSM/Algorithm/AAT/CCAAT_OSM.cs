using MathNet.Numerics.LinearAlgebra;
using OSM2020.Utility;
using OSM2020.OSM.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.Algorithm.AAT
{
    class CCAAT_OSM : OSMBase
    {
        protected double TargetH; //目標意見形成率
        protected double Epsilon;
        protected Dictionary<Agent, Candidate> Candidates;
        public int AwaRateWindowSize { get; protected set; }

        public CCAAT_OSM() : base()
        {
            //this.Epsilon = 0.05;
            this.Epsilon = 0.00;
            this.AwaRateWindowSize = 1;
        }

        public override void PrintAgentInfo(Agent agent)
        {
            base.PrintAgentInfo(agent);

            var is_recived = this.MyRecordRound.IsReceived(agent);
            var receive_rounds = this.MyRecordRounds.Where(record_round => record_round.IsReceived(agent)).Count();
            if (is_recived) receive_rounds++;

            var candidate = this.Candidates[agent];

            int can_index = 0;
            foreach (var record in candidate.SortedDataBase)
            {
                var select = (candidate.GetCurrentSelectRecord() == record) ? "*" : " ";
                var can_weight = record.CanWeight;
                var req_num = record.RequireOpinionNum;
                var awa_count = record.AwaCount;
                var h = record.AwaRate;
                Console.WriteLine($"{select} index: {can_index,3} req: {req_num,3} can_weight: {can_weight:f3} awa_count: {awa_count,3} h_rcv_round: {receive_rounds,3} h: {h:f4} {select}");
                can_index++;
            }
        }


        protected virtual void SetCandidate(int opinion_share_num)
        {
            this.Candidates = new Dictionary<Agent, Candidate>();
            foreach (var agent in this.MyAgentNetwork.Agents)
            {
                var neighbors = agent.GetNeighbors();
                var other_community_neighbors_num = neighbors.Where(neighbor => neighbor.GetCommunityId() == agent.GetCommunityId()).ToList().Count;
                var can = new Candidate(agent,opinion_share_num,other_community_neighbors_num, this.AwaRateWindowSize); //重み候補集合作成
                this.Candidates.Add(agent, can); //Candidatesに重み候補集合とエージェントを紐づけて登録
                agent.SetCommonWeight(can.GetSelectCanWeight()); //candidate.SelectSortedIndexのindexにある重みを選択．初期は0番目.これをエージェントの周りのリンクに着ける
            }
        }

        public override void SetAgentNetwork(AgentNetwork agent_network, int opinion_share_num)
        {
            base.SetAgentNetwork(agent_network,opinion_share_num); //MyAgentNetworkにagent_networkをセット．linkInfoValueを初期化
            this.SetCandidate(opinion_share_num); //各エージェントについて重み候補集合を作成し，各リンクに初期の重みをセット

            return;
        }

        public void SetTargetH(double target_h)
        {
            this.TargetH = target_h;
            return;
        }

        //step

        //round
        public override void InitializeToFirstRound(int opinion_share_num)
        {
            base.InitializeToFirstRound(opinion_share_num);
            this.SetCandidate(opinion_share_num);
        }

        public override void FinalizeRound()
        {
            this.EstimateAwaRate(); //意見形成率の推定
            this.SelectionWeight(); //各リンクについて重みの更新
            base.FinalizeRound(); //CurretRoundをインクリメントするだけ
        }

        protected virtual void EstimateAwaRate()
        {
            foreach (var candidate in this.Candidates) //エージェントの数
            {
                var received_sum_op = this.MyRecordRounds.Last().AgentReceiveOpinionsInRound[candidate.Key]; //エージェントの受け取った意見
                double obs_u = this.GetObsU(received_sum_op); //受け取った意見の総数を返すだけ
                if (obs_u == 0) continue;
                this.UpdateAveAwaRates(candidate.Key, candidate.Value, obs_u); //エージェントをぶち込んで，重み候補集合のAwaRateとAwaCountを更新
            }
        }

        protected virtual void SelectionWeight() //重み候補集合の現在或いは前後のどれかから選ぶ．
        {
            foreach (var candidate in this.Candidates) //全てのエージェントとその重み候補集合について
            {
                var received_sum_op = this.MyRecordRounds.Last().AgentReceiveOpinionsInRound[candidate.Key]; //最新ラウンドの特定エージェントが受け取った意見
                double obs_u = this.GetObsU(received_sum_op); //受け取った意見値の合計値
                if (obs_u == 0) continue;


                var current_h = candidate.Value.GetCurrentSelectRecord().AwaRate; //現在選択されている重み情報のawaRateを持ってくる
                var current_l = candidate.Value.SelectSortedIndex; //sortedIndex
                var can_size = candidate.Value.SortedDataBase.Count; //重み候補の数

                if (current_l < can_size - 1 && current_h < this.TargetH) //sortedIndexから重み候補-1(最大の重み)まで，かつ推定意見形成率が目標意見形成率を超えない範囲で
                {
                    candidate.Value.SelectSortedIndex++; //selectIndexをインクリメント
                }
                else if (current_l > 0) //上記に当てはまらず，sortedIndexが1以上の(最初でない)時
                {
                    var pre_h = candidate.Value.GetSortedRecord(current_l - 1).AwaRate; //sortedIndex-1の重みを引っ張ってくる
                    if (pre_h >= (this.TargetH + this.Epsilon)) //目標意見形成率＋Epsilonを超えていれば
                    {
                        candidate.Value.SelectSortedIndex--; 
                    }
                }

                candidate.Key.SetCommonWeight(candidate.Value.GetSelectCanWeight(),candidate.Value.GetPlusOneCanWeight()); //エージェントの周りのリンクの重みを更新
            }
        }



        protected virtual void UpdateAveAwaRates(Agent agent, Candidate candidate, double obs_u)
        {
            var select_record = candidate.GetCurrentSelectRecord(); //重み候補集合から現在の重みを一つ抜いてくる
            var current_round = this.CurrentRound;

            //候補集合のそれぞれについて，AwARateとAwaCountを更新
            foreach (var record in candidate.SortedDataBase) //sortedDateBaseは小さい順にソートされた重みの候補集合
            {
                var pre_counts = (record.AwaRates.Count == 0) ? 0 : (current_round) * record.AwaRate; //AwaRates.Count(意見形成率の算出が初めてでない)が0なら0.それ以外ならラウンド数とrateをかける

                if (this.IsEvsOpinionFormed(agent, select_record, record, obs_u)) //evs1,evs2のいずれかを満たしていれば
                {
                    record.AwaRate = (pre_counts + 1) / (current_round + 1); //意見形成率の算出
                    record.AwaCount = (int)pre_counts + 1; //意見形成数をインクリメント
                }
                else
                {
                    record.AwaRate = (pre_counts + 0) / (current_round + 1);
                    record.AwaCount = (int)pre_counts + 0;
                }
            }

            //foreach (var record in candidate.SortedDataBase)
            //{
            //    if (this.IsEvsOpinionFormed(agent, select_record, record, obs_u))
            //    {
            //        record.AwaCount += 1;
            //    }
            //    record.AwaRate = record.AwaCount / (double)(current_round + 1); 
            //}
        }

        protected virtual bool IsEvsOpinionFormed(Agent agent, CandidateRecord select_record, CandidateRecord other_record, double obs_u)
        {
            bool evs1 = this.IsDetermined(agent) && this.IsBiggerWeight(select_record, other_record); //agentが意見形成しててother>=selectならtrue
            bool evs2 = this.IsSmallerU(other_record, agent, obs_u) && (other_record.CanWeight != select_record.CanWeight); //受信意見数が必要数以上かつweightが同じでないなら

            return evs1 || evs2; //どちらかを満たしているならtrue
        }

        protected virtual bool IsDetermined(Agent agent)
        {
            return agent.IsDetermined();
        }

        protected virtual bool IsBiggerWeight(CandidateRecord select_record, CandidateRecord other_record)
        {
            double other_canwei = other_record.CanWeight;
            double select_canwei = select_record.CanWeight;

            return (other_canwei >= select_canwei) ? true : false;
        }

        protected virtual bool IsSmallerU(CandidateRecord other_record, Agent agent, double obs_u)
        {
            int req_u = other_record.RequireOpinionNum;
            return (obs_u >= req_u) ? true : false; //受け取った意見値の総数が必要数以上ならtrue
        }
    }
}
