using MathNet.Numerics.LinearAlgebra;
using OSM2020.OSM.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.Component
{
    class Agent : AgentBase<Agent>
    {
        public Vector<double> InitBelief { get; private set; } //初期信念値
        public Vector<double> Belief { get; private set; } //信念値(こいつが変動する)

        public int DispatchNum; //意見発信回数(意見形成時に1に初期化される)
        public bool IsOpinionForm; //意見を形成したかどうか
        public bool IsBadSensor; //必ず間違えるセンサかどうか
        public bool IsBadCommunity;
        public int CommunityId;


        public Agent()
        {
        }

        public Agent(Node node) //nodeのIDとagentのIDを結びつける
        {
            this.AgentID = node.ID;
            this.AgentLinks = new List<AgentLink>(); //リンクを持つ
            this.IsSensor = false;
            this.SetDispatchNum(0);
            this.SetOpinionForm(false);
            this.SetBadSensor(false);
            this.SetBadCommunity(false);
        }

        public Agent(int node_id)
        {
            this.AgentID = node_id;
            this.AgentLinks = new List<AgentLink>();
            this.IsSensor = false;
            this.SetDispatchNum(0);
            this.SetOpinionForm(false);
            this.SetBadSensor(false);
            this.SetBadCommunity(false);
        }

        public Agent AttachAgentLinks(List<AgentLink> agent_links)
        {
            //source あるいはTargetならリンクに追加
            this.AgentLinks.AddRange(agent_links.Where(agent_link => agent_link.SourceAgent.AgentID == this.AgentID || agent_link.TargetAgent.AgentID == this.AgentID).ToList());
            return this;
        }

        public Agent SetInitBelief(Vector<double> init_belief)
        {
            this.InitBelief = init_belief.Clone();
            this.Belief = init_belief.Clone();
            return this;
        }

        public Agent SetBelief(Vector<double> belief)
        {
            if (Belief.Count != belief.Count)
            {
                throw new Exception(nameof(Agent) + " Error irregular beleif dim");
            }

            this.Belief = belief.Clone(); //Beliefに最新の信念値が反映される
            return this;
        }


        public Agent SetBeliefFromList(List<double> belief_list)
        {
            if (Belief.Count != belief_list.Count)
            {
                throw new Exception(nameof(Agent) + " Error irregular beleif list");
            }

            var new_belief = Vector<double>.Build.Dense(belief_list.ToArray());
            this.Belief = new_belief;
            //Console.WriteLine(this.Belief.ToString());
            return this;
        }

        public void SetCommonWeight(double common_weight, double other_community_weight=-1)
        {
            foreach (var link in this.AgentLinks)
            {
                if(other_community_weight != -1 &&  link.TargetAgent.GetCommunityId() != link.SourceAgent.GetCommunityId())
                {
                    link.SetWeight(this, other_community_weight);
                }
                else
                {
                    link.SetWeight(this, common_weight);
                }
            }
        }

        public void SetWeights(Dictionary<Agent, double> weights)
        {
            foreach (var link in this.AgentLinks.Where(l => l.SourceAgent.AgentID >= 0 && l.TargetAgent.AgentID >= 0))
            {
                var weight = weights.First(wei => wei.Key == link.SourceAgent || wei.Key == link.TargetAgent).Value;
                link.SetWeight(this, weight);
            }
        }

        public void SetDispatchNum(int num)
        {
            DispatchNum = num;
        }

        public int GetDispatchNum()
        {
            return DispatchNum;
        }

        public void SetOpinionForm(bool formFlag)
        {
            IsOpinionForm = formFlag;
        }

        public bool GetOpinionForm()
        {
            return IsOpinionForm;
        }

        public void SetBadSensor(bool isbad)
        {
            IsBadSensor = isbad;
        }
        public bool GetBadSensor()
        {
            return IsBadSensor;
        }

        public void SetBadCommunity(bool isbad)
        {
            this.IsBadCommunity = isbad;
        }
        public bool GetBadCommunity()
        {
            return this.IsBadCommunity;
        }

        public void SetCommunityId(int id)
        {
            this.CommunityId = id;
        }

        public int GetCommunityId()
        {
            return this.CommunityId;
        }
    }
}
