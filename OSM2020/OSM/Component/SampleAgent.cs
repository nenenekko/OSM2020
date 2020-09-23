using MathNet.Numerics.LinearAlgebra;
using OSM2020.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OSM2020.OSM.Component
{
    class SampleAgent : AgentBase<SampleAgent>
    {
        InitBeliefGenerator MyInitBeliefGene; //信念値を生成するクラス

        public SampleAgent SetInitBeliefGene(InitBeliefGenerator init_belief_gene)
        {
            this.MyInitBeliefGene = init_belief_gene; //信念値生成器をセット
            return this;
        }


        public void Generate(ExtendRandom agent_network_rand, Agent agent)
        {
            var init_belief = this.MyInitBeliefGene.Generate(this.InitOpinion, agent_network_rand); //信念値生成器
            agent.SetInitBelief(init_belief); //agentに各意見の信念値を設定
            agent.SetSubject(this.MySubject);
            agent.SetInitOpinion(this.InitOpinion);
            agent.SetThreshold(this.OpinionThreshold);
        }
    }
}
