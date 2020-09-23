using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using OSM2020.Utility;

namespace OSM2020.OSM.DistributionGenerator
{
    class Shitei_DistGenerator : I_DistGenerator
    {
        public CustomDistribution MyCustomDistribution { get; set; }
        public Shitei_DistGenerator(int dim, double dist_weight, int main_index) //意見数，
        {
            var dist_list = new List<double>() { 0.8, 0.2};
            var dist = Vector<double>.Build.Dense(dist_list.ToArray()); //double型のリストにコピー
            var max_index = dist.MaximumIndex(); //最大値のindexを返す(同値の場合は最小のindex)
            var max = dist[max_index]; //最大値取得
            dist[max_index] = dist[main_index]; //max_indexとmain_indexを交換
            dist[main_index] = max;

            this.MyCustomDistribution = new CustomDistribution(dist, dist_weight); //意見を確立によって決定.distとdist_weightをセットしてCustomDistributionをセット
        }
        public CustomDistribution Generate()
        {
            return this.MyCustomDistribution;
        }
    }
}
