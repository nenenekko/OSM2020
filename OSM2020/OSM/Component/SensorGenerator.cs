using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSM2020.Utility;

namespace OSM2020.OSM.Component
{
    class SensorGenerator
    {
        public int SensorSize { get; private set; }
        public int MaliciousSensorSize { get; private set; }

        public int BadCommunityIndex;

        public SensorGenerator SetSensorSize(int sensor_size, int malicious_sensor_size = 0)
        {
            Debug.Assert(sensor_size >= malicious_sensor_size);
            this.SensorSize = sensor_size;
            this.MaliciousSensorSize = malicious_sensor_size;
            return this;
        }

        public void Generate(ExtendRandom agent_network_rand, List<Agent> agents ,bool bad_sensor_mode , SensorArrangementEnum sensor_arrange,List<List<int>> communityList = null)
        {
            foreach (var agent in agents) //全てのエージェントを初期化
            {
                agent.SetSensor(false);
            }
            List<int> sensor_list = new List<int>();

            int bad_sensor = -1;
            if (communityList.Count != 0 && sensor_arrange == SensorArrangementEnum.Each) //ネットワークがコミュニティ構造を持つとき
            {
                int rest = 0;
                if (this.SensorSize > communityList.Count) //コミュニティの数よりセンサの数の方が多いとき
                {
                    for (var i = 0; i < this.SensorSize; i++)
                    {
                        //sensor_list = agents.Select(agent => agent.AgentID).OrderBy(id => agent_network_rand.Next()).Take(this.SensorSize).ToList();
                        var sensors = agents.Where(agent => communityList[i % communityList.Count].Contains(agent.AgentID)); //コミュニティのメンバー．センサ候補
                        var sensor = sensors.Select(agent => agent.AgentID).OrderBy(id => agent_network_rand.Next()).Take(1).ToList().First(); //一人センサを選ぶ
                        sensor_list.Add(sensor);
                    }
                    rest = this.SensorSize % communityList.Count;
                }
                else //コミュニティの数がセンサの数以下の時
                {
                    for (var i = 0; i < this.SensorSize; i++)
                    {
                        //sensor_list = agents.Select(agent => agent.AgentID).OrderBy(id => agent_network_rand.Next()).Take(this.SensorSize).ToList();
                        var sensors = agents.Where(agent => communityList[i].Contains(agent.AgentID)); //コミュニティのメンバー．センサ候補
                        var sensor = sensors.Select(agent => agent.AgentID).OrderBy(id => agent_network_rand.Next()).Take(1).ToList().First(); //一人センサを選ぶ
                        sensor_list.Add(sensor);
                    }
                }

                if (bad_sensor_mode)
                {
                    if (rest != 0)
                    {
                        while (true)
                        {
                            int candidate = agent_network_rand.Next(0, sensor_list.Count);
                            bool flag = false;
                            for (var j = 0; j < rest; j++)
                            {
                                if (candidate % communityList.Count == j)
                                {
                                    flag = true;
                                }
                            }
                            if (!flag)
                            {
                                bad_sensor = candidate;
                                break;
                            }
                        }
                    }
                    else
                    {
                        bad_sensor = agent_network_rand.Next(0, sensor_list.Count);
                    }
                    agents.Where(agent => sensor_list[bad_sensor] == agent.AgentID).ToList().ForEach(agent => agent.SetBadSensor(true));
                    this.BadCommunityIndex = bad_sensor;
                    agents.Where(agent => communityList[bad_sensor].Contains(agent.AgentID)).ToList().ForEach(agent => agent.SetBadCommunity(true));
                }
            }
            else //ネットワークがコミュニティ構造を持たないとき
            {
                sensor_list = agents.Select(agent => agent.AgentID).OrderBy(id => agent_network_rand.Next()).Take(this.SensorSize).ToList();
                if (bad_sensor_mode)
                {
                    bad_sensor = agent_network_rand.Next(0, sensor_list.Count);
                    agents.Where(agent => sensor_list[bad_sensor] == (agent.AgentID)).ToList().ForEach(agent => agent.SetBadSensor(true));
                    this.BadCommunityIndex = bad_sensor;
                }
            }

            var malicious_sensor_list = sensor_list.OrderBy(id => agent_network_rand.Next()).Take(this.MaliciousSensorSize).ToList();
            agents.Where(agent => sensor_list.Contains(agent.AgentID)).ToList().ForEach(agent => agent.SetSensor(true, false)); //センサエージェントに選ばれたエージェントをセンサとして認定
            agents.Where(agent => malicious_sensor_list.Contains(agent.AgentID)).ToList().ForEach(agent => agent.SetSensor(true, true)); //誤情報センサに選ばれたエージェントを認定
        }
    }
}
