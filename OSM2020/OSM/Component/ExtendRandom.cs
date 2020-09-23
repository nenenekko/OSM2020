using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using OSM2020.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.Component
{
    class ExtendRandom
    {
        public int Seed { get; }
        public SeedEnum MySeedEnum { get; }
        MersenneTwister rand;

        public ExtendRandom(SeedEnum seed_enum, int seed)
        {
            this.MySeedEnum = seed_enum;
            this.Seed = seed;
            rand = new MersenneTwister(this.Seed);
        }

        public ExtendRandom(int seed)
        {
            this.Seed = seed;
            rand = new MersenneTwister(this.Seed);
        }

        public void Initialize()
        {
            rand = new MersenneTwister(this.Seed);
        }

        public int Next()
        {
            return rand.Next(); //乱数生成
        }

        public int Next(int max)
        {
            return rand.Next(max);
        }

        public int Next(int min, int max)
        {
            return rand.Next(min, max);
        }

        public double NextDouble()
        {
            return rand.NextDouble();
        }

        public double NextDouble(double min, double max)
        {
            return (max - min) * rand.NextDouble() + min;
        }

        public double NextNormal(double mean, double stddev)
        {
            return Normal.Sample(rand, mean, stddev);
        }

        public virtual List<double> NextNormals(double mean, double stddev, int size, double bound_rate, double bias_index = -1, double bias_p = 0)
        {
            double sample;
            List<double> sample_list = new List<double>();

            foreach (var count in Enumerable.Range(0, size))
            {
                if (bias_index == count)
                {
                    sample = bias_p;
                }
                else
                {
                    do
                    {
                        sample = Normal.Sample(rand, mean, stddev);
                        sample = Math.Abs(sample);
                        sample = Math.Round(sample, 4);
                    } while (!(sample > mean - mean * bound_rate && sample < mean + mean * bound_rate) || sample < 0);
                }
                sample_list.Add(sample);
            }
            return sample_list;
        }

    }
}
