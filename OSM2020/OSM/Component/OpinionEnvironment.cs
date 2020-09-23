using MathNet.Numerics.LinearAlgebra;
using OSM2020.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.Component
{
    class OpinionEnvironment
    {
        public double SensorWeight { get; protected set; }
        public OpinionSubject EnvSubject { get; protected set; }
        public int CorrectDim { get; protected set; }
        public int MaliciousDim { get; protected set; }

        public Agent EnvironmentAgent { get; protected set; }
        public CustomDistribution MyCustomDistribution { get; protected set; }
        public CustomDistribution MyMaliciousCustomDistribution { get; protected set; }

        public OpinionEnvironment()
        {
        }

        public OpinionEnvironment SetSubject(OpinionSubject subject)
        {
            this.EnvSubject = subject;
            return this;
        }

        public OpinionEnvironment SetCorrectDim(int cor_dim)
        {
            this.CorrectDim = cor_dim;
            return this;
        }

        public OpinionEnvironment SetSensorWeight(double sensor_weight)
        {
            this.SensorWeight = sensor_weight;
            return this;
        }

        public OpinionEnvironment SetCustomDistribution(CustomDistribution custom_dist)
        {
            this.MyCustomDistribution = custom_dist;
            return this;
        }
        public OpinionEnvironment SetMaliciousCustomDistribution(CustomDistribution custom_malicious_dist)
        {
            this.MyMaliciousCustomDistribution = custom_malicious_dist;
            return this;
        }


        public OpinionEnvironment SetMaliciousDim(int malicious_dim)
        {
            this.MaliciousDim = malicious_dim;
            return this;
        }

        public void AddEnvironment(AgentNetwork agent_network)
        {
            this.EnvironmentAgent = new Agent(-1).SetSubject(this.EnvSubject); //環境エージェント(≒環境)
            var op_vector = Vector<double>.Build.Dense(this.EnvSubject.SubjectDimSize, 0.0); //意見の種類だけ要素を0で初期化
            this.EnvironmentAgent.SetInitOpinion(op_vector);
            List<AgentLink> env_links = new List<AgentLink>(); //環境とセンサエージェントをつなぐリンク
            var sensors = agent_network.Agents.Where(agent => agent.IsSensor).ToList();

            int link_index = -1;
            foreach (var sensor in sensors)
            {
                env_links.Add(new AgentLink(link_index, sensor, this.EnvironmentAgent));
                link_index--;
            }

            foreach (var sensor in sensors)
            {
                sensor.AttachAgentLinks(env_links);
            }

            this.EnvironmentAgent.AttachAgentLinks(env_links);
        }

        public List<Message> SendMessages(List<Agent> sensor_agents, ExtendRandom update_step_rand, bool bad_sensor_mode, int sample_size = 1) //観測したセンサ，乱数生成器
        {
            List<Message> messages = new List<Message>(); //メッセージのリスト
            foreach (var sensor_agent in sensor_agents) //観測したセンサ1つずつ
            {
                var agent_link = this.EnvironmentAgent.AgentLinks.Where(link => link.SourceAgent == sensor_agent || link.TargetAgent == sensor_agent).First(); //観測したセンサと環境エージェントのリンク
                var opinion = this.EnvironmentAgent.Opinion.Clone(); //環境の意見コピー
                opinion.Clear(); //初期化

                if (bad_sensor_mode) //バッドセンサモードの時
                {
                    if (!sensor_agent.IsBadSensor)
                    {
                        foreach (int i in Enumerable.Range(0, sample_size)) //0からsample_size(デフォで1)-1の間
                        {
                            int sample_index = -1; //初期化
                            if (sensor_agent.IsMalicious)
                            {
                                sample_index = this.MyMaliciousCustomDistribution.SampleCustomDistribution(update_step_rand);
                            }
                            else
                            {
                                sample_index = this.MyCustomDistribution.SampleCustomDistribution(update_step_rand); //閾値を乱数生成して，myDistributionを足し合わせてそれを超えたら
                            }
                            opinion[sample_index] += 1.0; //opinionの対象要素をインクリメント．おそらくそこで意見が形成されたことになる？
                        }
                    }
                    else
                    {
                        opinion[1] += 1.0;
                    }
                }
                else //確定で間違えるセンサがいないとき
                {
                    foreach (int i in Enumerable.Range(0, sample_size)) //0からsample_size(デフォで1)-1の間
                    {
                        int sample_index = -1; //初期化
                        if (sensor_agent.IsMalicious)
                        {
                            sample_index = this.MyMaliciousCustomDistribution.SampleCustomDistribution(update_step_rand);
                        }
                        else
                        {
                            sample_index = this.MyCustomDistribution.SampleCustomDistribution(update_step_rand); //閾値を乱数生成して，myDistributionを足し合わせてそれを超えたら
                        }
                        opinion[sample_index] += 1.0; //opinionの対象要素をインクリメント．おそらくそこで意見が形成されたことになる？
                    }
                }


                messages.Add(new Message(this.EnvironmentAgent, sensor_agent, agent_link, opinion)); //from, to, link, (subject),opinionの順にそのままセット
            }

            return messages;
        }
    }
}
