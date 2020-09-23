using OSM2020.OSM.Component;
using OSM2020.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.Algorithm.AAT
{
    class Candidate
    {
        public List<CandidateRecord> DataBase { get; protected set; }
        public List<CandidateRecord> SortedDataBase { get; protected set; }
        public int SelectSortedIndex;

        public Candidate()
        {

        }

        public Candidate(Agent agent, int opinion_share_num,int awa_window_size = 1)  //agentを受け取る
        {
            this.DataBase = new List<CandidateRecord>();
            if (agent.GetNeighbors().Count == 0) return; //agentに隣人がいなければ

            int max_require_num = agent.GetNeighbors().Count * opinion_share_num; //隣人の数*意見シェア数
            int dim_size = agent.Belief.Count; //意見数


            for (int dim = 0; dim < dim_size; dim++) //意見数分回す
            {
                for (int req_num = 1; req_num <= max_require_num; req_num++) //req_numは意見一つに対して隣人数分
                {
                    this.DataBase.Add(new CandidateRecord(dim, req_num, agent, awa_window_size)); //各リンクに対して重み候補を出しリストにぶち込む
                }
            }


            this.SortedDataBase = this.DataBase.OrderBy(record => record.CanWeight).ToList(); //重み候補集合完成
            this.SelectSortedIndex = 0;
        }

        public Candidate(Agent agent, int opinion_share_num, int other_community_neighbor_num,int awa_window_size = 1)  //agentを受け取る
        {
            this.DataBase = new List<CandidateRecord>();
            if (agent.GetNeighbors().Count == 0) return; //agentに隣人がいなければ

            int max_require_num = agent.GetNeighbors().Count * opinion_share_num + other_community_neighbor_num; //隣人の数*意見シェア数
            int dim_size = agent.Belief.Count; //意見数


            for (int dim = 0; dim < dim_size; dim++) //意見数分回す
            {
                for (int req_num = 1; req_num <= max_require_num; req_num++) //req_numは意見一つに対して隣人数分
                {
                    this.DataBase.Add(new CandidateRecord(dim, req_num, agent, awa_window_size)); //各リンクに対して重み候補を出しリストにぶち込む
                }
            }


            this.SortedDataBase = this.DataBase.OrderBy(record => record.CanWeight).ToList(); //重み候補集合完成
            this.SelectSortedIndex = 0;
        }


        public CandidateRecord GetSortedRecord(int index)
        {
            return this.SortedDataBase[index];
        }

        public CandidateRecord GetCurrentSelectRecord()
        {
            return this.SortedDataBase[this.SelectSortedIndex];
        }

        public CandidateRecord GetPlusOneSelectRecord()
        {
            if (SortedDataBase.Count - 2 > this.SelectSortedIndex)
                return this.SortedDataBase[this.SelectSortedIndex + 2];
            else if (SortedDataBase.Count-1 > this.SelectSortedIndex)
                return this.SortedDataBase[this.SelectSortedIndex + 1];
            else
                return this.SortedDataBase[this.SelectSortedIndex];
        }

        public double GetSelectCanWeight()
        {
            return this.GetCurrentSelectRecord().CanWeight;
        }
        public double GetPlusOneCanWeight()
        {
            return this.GetPlusOneSelectRecord().CanWeight;
        }

    }
}
