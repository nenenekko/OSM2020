using MathNet.Numerics.LinearAlgebra;
using OSM2020.OSM.Component;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.Utility
{
    class CustomDistribution
    {
        public Vector<double> MyDistribution { get; private set; }
        public double MyDistWeight { get; private set; }
        public CustomDistribution(Vector<double> dist, double dist_weight)
        {
            this.MyDistribution = dist;
            this.MyDistWeight = dist_weight;
        }

        public int SampleCustomDistribution(ExtendRandom rand)
        {
            double accumlation_value = 0;
            double rand_value = rand.NextDouble(); //double型の乱数
            int index = 0;
            foreach (var value in this.MyDistribution) //MydistributionのValuesから一つずつ
            {
                accumlation_value += value; //valueを足し合わせ
                if (rand_value <= accumlation_value) //生成した乱数を超えたら
                {
                    return index; //valueのindexを返す
                }
                index += 1;
            }
            return this.MyDistribution.Count() - 1;
        }
    }
}
