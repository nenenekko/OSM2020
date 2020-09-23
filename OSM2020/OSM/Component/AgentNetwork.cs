using OSM2020.Utility;
using OSM2020.OSM.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.Component
{
    class AgentNetwork
    {
        public ExtendRandom AgentGenerateRand { get; private set; }
        public RawGraph MyGraph { get; private set; }
        public Layout MyLayout { get; private set; }
        public SubjectManager MySubjectManager { get; private set; }
        public List<Agent> Agents { get; private set; } //エージェントのリスト
        public List<AgentLink> AgentLinks { get; private set; }

        public SensorGenerator MySensorGenerator { get; private set; }

        public bool BadSensorMode;
        public List<List<int>> CommunityList;
        public SensorArrangementEnum SensorArragne;

        public AgentNetwork()
        {
            this.Agents = new List<Agent>(); //エージェントのリストをインスタンス化
            this.AgentLinks = new List<AgentLink>(); //エージェントリンクのリストをインスタンス化
        }

        public AgentNetwork SetRand(ExtendRandom agent_gene_rand) //ランダム生成器セット
        {
            this.AgentGenerateRand = agent_gene_rand;
            return this;
        }

        public AgentNetwork GenerateNetworkFrame(RawGraph graph) 
        {
            this.MyGraph = graph; //グラフ(ノードとリンク)をセット

            foreach (var node in graph.Nodes) //各ノード
            {
                this.Agents.Add(new Agent(node)); //エージェントのリストにグラフに存在するエージェント(node)を追加
            }

            int link_index = 0;
            foreach (var link in graph.Links) //各リンク
            {
                this.AgentLinks.Add(new AgentLink(link_index).SetLink(link, this.Agents)); //sourceweight, targetweightをAgentLinkにセット．元からリンクにsource,target情報があるのでそれをAgentlinkに設定
                link_index++;
            }

            foreach (var agent in this.Agents) //各エージェント
            {
                agent.AttachAgentLinks(this.AgentLinks); //エージェントにリンクを結合する
            }

            return this;
        }

        public AgentNetwork ApplySampleAgent(SampleAgent sample_agent, SampleAgentSetMode mode, double random_set_rate = 0.0)
        {
            switch (mode)
            {
                case SampleAgentSetMode.RandomSetRate:
                    if (random_set_rate == 0.0) new Exception(nameof(AgentNetwork) + " Error no random set rate");

                    var set_agent_size = (int)(this.MyGraph.Nodes.Count * random_set_rate); //
                    var list = this.Agents.Select(agent => agent.AgentID).OrderBy(id => this.AgentGenerateRand.Next()).Take(set_agent_size)
                        .ToList();
                    this.Agents.Where(agent => list.Contains(agent.AgentID)).ToList().ForEach(agent => sample_agent.Generate(this.AgentGenerateRand, agent));
                    break;
                case SampleAgentSetMode.RemainSet:
                    this.Agents.Where(agent => agent.InitBelief == null).ToList().ForEach(agent => sample_agent.Generate(this.AgentGenerateRand, agent)); //全てのagentに信念値をランダム生成.ランダム生成器とagentを引数．
                    break;
                default:
                    break;
            }
            return this;
        }

        public AgentNetwork GenerateSensor(SensorGenerator sensor_gene) //ランダム生成器をセット．
        {
            this.MySensorGenerator = sensor_gene;
            this.MySensorGenerator.Generate(this.AgentGenerateRand, this.Agents, this.BadSensorMode, this.SensorArragne, this.CommunityList); //正情報，誤情報センサを設定
            return this;
        }

        public AgentNetwork GenerateSensor()
        {
            Debug.Assert(this.MySensorGenerator != null);
            this.MySensorGenerator.Generate(this.AgentGenerateRand, this.Agents, this.BadSensorMode, this.SensorArragne, this.CommunityList); //正情報，誤情報センサを設定
            return this;
        }

        public AgentNetwork SetLayout(Layout layout)
        {
            this.MyLayout = layout;
            return this;
        }

        public AgentNetwork SetSubjectManager(SubjectManager subject_manager)
        {
            this.MySubjectManager = subject_manager;
            return this;
        }

        public AgentNetwork SetBadSensorMode(bool flag)
        {
            this.BadSensorMode = flag;
            return this;
        }

        public AgentNetwork SetCommnityList(List<List<int>> community)
        {
            this.CommunityList = community;
            return this;
        }

        public AgentNetwork SetSensorArrange(SensorArrangementEnum arrange)
        {
            this.SensorArragne = arrange;
            return this;
        }

        public int GetBadCommunityIndex()
        {
            return this.MySensorGenerator.BadCommunityIndex;
        }

        public AgentNetwork SetCommunityId()
        {
            if(CommunityList.Count != 0)
            {
                var i = 0;
                foreach(var community in this.CommunityList)
                {
                    foreach(var agent_id in community)
                    {
                        this.Agents[agent_id].SetCommunityId(i);
                    }
                    i++;
                }
            }
            return this;
        }
    }
}
