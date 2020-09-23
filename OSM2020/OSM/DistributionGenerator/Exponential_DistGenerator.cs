using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using OSM2020.Utility;

namespace OSM2020.OSM.DistributionGenerator
{
    class Exponential_DistGenerator : I_DistGenerator
    {
        public CustomDistribution MyCustomDistribution { get; set; } 
        public Exponential_DistGenerator(int dim, double dist_weight, int main_index) //意見数，
        {
            var range = Enumerable.Range(0, dim); //0から意見数-1(意見数分).要素も0,1,2,..
            var dist_enumerable = range.Select((item, index) => Math.Pow(1 - dist_weight, index)); //1-dist_weightのindex乗
            var sum = dist_enumerable.Sum();
            var dist_list = dist_enumerable.Select(item => Math.Round(item / sum, 5)); //各要素をsumで割り算して小数第5位で丸める
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
