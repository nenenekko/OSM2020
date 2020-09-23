using MathNet.Numerics.LinearAlgebra;
using OSM2020.OSM.Component;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.Algorithm.SWT
{
    class LinkInfoValue
    {
        public Agent ReceiveAgent { get; private set; }
        public Agent SendAgent { get; private set; }
        public Queue<double> RecOpinionProbs { get; private set; }
        public double RecOpinionProb
        {
            get
            {
                if (RecOpinionProbs.Count == 0) return 1;
                return this.RecOpinionProbs.Last();
            }
            set
            {
                this.RecOpinionProbs.Enqueue(value);
                if (this.RecOpinionProbs.Count > this.WindowSize) this.RecOpinionProbs.Dequeue();
            }
        }

        public int WindowSize { get; private set; }

        public LinkInfoValue(Agent receiver, Agent sender, int window_size)
        {
            this.ReceiveAgent = receiver;
            this.SendAgent = sender;
            this.RecOpinionProbs = new Queue<double>();
            this.WindowSize = window_size;
        }

        public void Regist(Vector<double> op, Vector<double> belief) //受け取った意見，元々の信念値
        {
            Debug.Assert(op.Count == belief.Count);
            double p = 1.0;
            double each_p;

            foreach (var i in Enumerable.Range(0, op.Count)) //意見数分=0から(意見数-1)
            {
                if (belief[i] == 0) continue;
                each_p = Math.Round(Math.Pow(belief[i], op[i]), 5); //受け取った意見の項は1乗，他は0乗(=1)を少数第5位まで丸める
                if (each_p == 0) continue;
                p *= each_p;
            }

            Debug.Assert(p != 0);
            this.RecOpinionProb = p; //何だろうこれ

        }

        public double GetInfoValueSum()
        {
            double info_value_sum = 0.0;
            foreach (var p in this.RecOpinionProbs)
            {
                info_value_sum += -1 * Math.Log(p, 2);
            }
            return info_value_sum;
        }
    }
}
