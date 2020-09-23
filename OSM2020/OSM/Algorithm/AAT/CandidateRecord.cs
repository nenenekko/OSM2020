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
    class CandidateRecord
    {
        public int BeliefDim { get; protected set; }
        public int RequireOpinionNum { get; protected set; }
        public double CanWeight { get; protected set; }

        public int AwaCount { get; set; }
        public int AwaWindowSize { get; private set; }
        public Queue<double> AwaRates { get; private set; }
        public double AwaRate
        {
            get
            {
                if (this.AwaRates.Count == 0) return 0;
                return this.AwaRates.Last();
            }
            set
            {
                this.AwaRates.Enqueue(value);
                if (this.AwaRates.Count > this.AwaWindowSize) this.AwaRates.Dequeue();
            }
        }

        AggregationFunctions MyAggFuncs;

        public CandidateRecord(int belief_dim, int requre_num, Agent agent, int awa_window_size = 1)
        {
            this.MyAggFuncs = new AggregationFunctions();
            this.BeliefDim = belief_dim;
            this.RequireOpinionNum = requre_num;
            this.CanWeight = Math.Round(this.CalcCanWeight(belief_dim, requre_num, agent), 4); //候補重みを返してきて，それを少数第4位で丸める
            this.AwaRates = new Queue<double>();
            this.AwaWindowSize = awa_window_size;
        }

        public CandidateRecord(double can_weight, Agent agent, int awa_window_size = 1)
        {
            this.MyAggFuncs = new AggregationFunctions();
            this.BeliefDim = -1;
            this.RequireOpinionNum = this.CalcRequireNum(can_weight, agent);
            this.CanWeight = Math.Round(can_weight, 4);
            this.AwaRates = new Queue<double>();
            this.AwaWindowSize = awa_window_size;
        }

        int CalcRequireNum(double can_weight, Agent agent)
        {
            var max_count = agent.GetNeighbors().Count;
            var init_belief = agent.InitBelief;
            Vector<double> receive_op = agent.InitOpinion.Clone();
            Vector<double> belief = agent.InitBelief.Clone();
            receive_op.Clear();
            belief.Clear();

            int require_num = 1;
            for (; belief[0] < agent.OpinionThreshold && require_num < max_count; require_num++)
            {
                receive_op[0] = require_num;
                belief = this.MyAggFuncs.UpdateBelief(init_belief, can_weight, receive_op, BeliefUpdateFunctionEnum.Bayse);
            }

            return require_num;
        }

        double CalcCanWeight(int belief_dim, int requre_num, Agent agent)
        {
            var diff = 0.01;
            //var init_can_weight = (1.0 / agent.InitBelief.Count) + diff;
            var init_can_weight = 0.0;
            init_can_weight = Math.Round(init_can_weight, 4);
            var init_belief = agent.InitBelief; //agentの初期信念値
            Vector<double> receive_op = agent.InitOpinion.Clone(); //意見値コピー
            receive_op.Clear(); //意見値を初期化
            receive_op[belief_dim] = requre_num; //意見値のbelief_dim項をrequire_numに．各意見についてrequire_num(1～隣人数)が作られる

            double can_weight = 0.00;
            for (can_weight = init_can_weight; can_weight < 1.0; can_weight += diff) //初期信念に対して隣人から来た意見数(必要数)をかけて意見形成するまで重みを調整する
            {
                var belief = this.MyAggFuncs.UpdateBelief(init_belief, can_weight, receive_op, BeliefUpdateFunctionEnum.Bayse); //信念値更新
                if (belief[belief_dim] >= agent.OpinionThreshold) break; //意見形成の閾値を超したら終了
            }

            if (can_weight >= 1)
            {
                //throw new Exception("error over can weight");
            }

            can_weight = MyMath.Ceiling(can_weight, 4); //小数第4位に繰り上げ

            return can_weight;
        }
    }
}
