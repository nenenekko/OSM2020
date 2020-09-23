using MathNet.Numerics.LinearAlgebra;
using OSM2020.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.Component
{
    class AggregationFunctions
    {
        private Vector<double> GetLikelihoodsForParticleFilter(double weight, int dim, int sample_index, Vector<double> receive_opinions)
        {
            var exist_count = receive_opinions.Count(n => n != 0);
            var turara = MyMath.MakeTurara(exist_count, weight);
            var max = turara.max;
            var other = turara.other;

            var likelihoods = Vector<double>.Build.DenseOfVector(receive_opinions);
            for (int vector_index = 0; vector_index < likelihoods.Count; vector_index++)
            {
                if (vector_index == sample_index)
                {
                    likelihoods[vector_index] = max;
                }
                else
                {
                    if (likelihoods[vector_index] != 0)
                    {
                        likelihoods[vector_index] = other;
                    }
                }
            }
            return likelihoods;
        }

        //↓昔の名残
        private Vector<double> GetPostBeliefsByParticleFilter(Vector<double> prior_beliefs, Vector<double> receive_opinions, double weight)
        {
            int dim = prior_beliefs.Count;
            var sample_index = receive_opinions.MaximumIndex();

            var likelihoods = this.GetLikelihoodsForParticleFilter(weight, dim, sample_index, receive_opinions);
            //var weight_dist = likelihoods.PointwiseMultiply(receive_opinions);
            //var post_beliefs = weight_dist.PointwiseMultiply(prior_beliefs);
            var post_beliefs = likelihoods.PointwiseMultiply(prior_beliefs);

            post_beliefs = post_beliefs.Map(value => Math.Round(value, 3));
            if (post_beliefs.Sum() == 0)
            {
                post_beliefs.Clear();
                return post_beliefs + (1.0 / dim);
            }
            return post_beliefs.Normalize(1.0);
        }

        private Vector<double> GetLikelihoodsForBayseFilter(double weight, int dim, int sample_index)
        {
            var turara = MyMath.MakeTurara(dim, weight); //maxとotherの組
            var max = turara.max;
            var other = turara.other;

            var likelihoods = Vector<double>.Build.Dense(dim, other); //意見数の要素数でotherの値で初期化
            likelihoods[sample_index] = max; //sample_indexの方はmaxに．他は全部otherの値
            return likelihoods;
        }


        private Vector<double> GetPostBeliefsByBayseFilter(Vector<double> prior_beliefs, Vector<double> receive_opinions, double weight)
        {
            Debug.Assert(!Double.IsNaN(weight));

            int dim = prior_beliefs.Count;

            var likelihoods = Vector<double>.Build.Dense(dim, 0); //意見数分の配列(0で初期化)
            var post_beliefs = prior_beliefs; 

            foreach (var sample_index in Enumerable.Range(0, receive_opinions.Count)) //意見数分回す.受け取った意見についてなので1項だけ1.あとは0
            {
                foreach (var i in Enumerable.Range(0, (int)receive_opinions[sample_index])) //意見がない項(receive_opinions[sample_index]が0のとき)は飛ばされる
                {
                    likelihoods = this.GetLikelihoodsForBayseFilter(weight, dim, sample_index); //意見数分数値が返ってくる.sample_indexの項はmax,他はother
                    post_beliefs = post_beliefs.PointwiseMultiply(likelihoods); //信念値更新．post_beliefs(この時点では事前分布)×likelihoods．要素同士の掛け算
                }
            }
            //Debug.Assert(receive_opinion[sample_index] == 1);

            post_beliefs = post_beliefs.Map(value => Math.Round(value, 3)); //小数第三位までに丸める


            if (post_beliefs.Sum() == 0) //信念値の合計が0のとき
            {
                post_beliefs.Clear();
                return post_beliefs + (1.0 / dim);
            }
            return post_beliefs.Normalize(1.0); //全要素合計して1になるように正規化
        }

        private Vector<double> GetPostBeliefsByAATPaperBayseFilter(Vector<double> prior_beliefs, Vector<double> receive_opinions, double weight)
        {
            Debug.Assert(!Double.IsNaN(weight));

            int dim = prior_beliefs.Count;

            var likelihoods = Vector<double>.Build.Dense(dim, 0); //意見数分の配列(0で初期化)
            var post_beliefs = prior_beliefs;

            if(prior_beliefs[0] != 1 && prior_beliefs[0] != 0)
            {
                if (receive_opinions[0] == 1)
                {
                    post_beliefs[0] = weight * prior_beliefs[0] / ((1 - weight) * (1 - prior_beliefs[0]) + weight * prior_beliefs[0]);
                }
                else
                {
                    post_beliefs[0] = (1 - weight) * prior_beliefs[0] / (weight * (1 - prior_beliefs[0]) + (1 - weight) * prior_beliefs[0]);
                }
                if (post_beliefs[0] > 1)
                    post_beliefs[0] = 1;
                else if (post_beliefs[0] < 0)
                    post_beliefs[0] = 0;
            }

            post_beliefs[1] = 1 - post_beliefs[0];

            post_beliefs = post_beliefs.Map(value => Math.Round(value, 3)); //小数第三位までに丸める

            /*if(post_beliefs[0]  <= 1 && post_beliefs[0] >= 0)
            {
                Console.WriteLine(post_beliefs);
            }
            else
            {
                Console.WriteLine(post_beliefs);
            }*/

            /*if (post_beliefs.Sum() == 0) //信念値の合計が0のとき
            {
                post_beliefs.Clear();
                return post_beliefs + (1.0 / dim);
            }*/
            return post_beliefs.Normalize(1.0); //全要素合計して1になるように正規化
        }

        private Vector<double> GetPostBeliefsBySameOpinionAdjustBayseFilter(Vector<double> prior_beliefs, Vector<double> receive_opinions, double weight, double same_opinion_rate)
        {
            Debug.Assert(!Double.IsNaN(weight));

            int dim = prior_beliefs.Count;

            var likelihoods = Vector<double>.Build.Dense(dim, 0); //意見数分の配列(0で初期化)
            var post_beliefs = prior_beliefs;

            if(same_opinion_rate != 0)
            {
                foreach (var sample_index in Enumerable.Range(0, receive_opinions.Count)) //意見数分回す.受け取った意見についてなので1項だけ1.あとは0
                {
                    foreach (var i in Enumerable.Range(0, (int)receive_opinions[sample_index])) //意見がない項(receive_opinions[sample_index]が0のとき)は飛ばされる
                    {
                        /*if (same_opinion_rate >= 1 / 2)
                            weight = weight * 1.2;
                        else
                            weight = weight * 0.8;*/
                        if (same_opinion_rate < 0 || same_opinion_rate > 1)
                            Console.WriteLine(same_opinion_rate);
                        var rate = Math.Round((1 + same_opinion_rate * 2) / 2,4);
                        //Console.WriteLine(rate);
                        weight = weight * rate;
                        if (weight >= 1) weight = 1;
                        if (weight <= 0) weight = 0;
                        likelihoods = this.GetLikelihoodsForBayseFilter(weight, dim, sample_index); //意見数分数値が返ってくる.sample_indexの項はmax,他はother
                        post_beliefs = post_beliefs.PointwiseMultiply(likelihoods); //信念値更新．post_beliefs(この時点では事前分布)×likelihoods．要素同士の掛け算
                    }
                }
                //Debug.Assert(receive_opinion[sample_index] == 1);

                post_beliefs = post_beliefs.Map(value => Math.Round(value, 3)); //小数第三位までに丸める


                if (post_beliefs.Sum() == 0) //信念値の合計が0のとき
                {
                    post_beliefs.Clear();
                    return post_beliefs + (1.0 / dim);
                }
                return post_beliefs.Normalize(1.0); //全要素合計して1になるように正規化
            }
            else //一度も意見形成時に意見を受け取ったことがないorセンサからの情報の場合
            {
                foreach (var sample_index in Enumerable.Range(0, receive_opinions.Count)) //意見数分回す.受け取った意見についてなので1項だけ1.あとは0
                {
                    foreach (var i in Enumerable.Range(0, (int)receive_opinions[sample_index])) //意見がない項(receive_opinions[sample_index]が0のとき)は飛ばされる
                    {
                        likelihoods = this.GetLikelihoodsForBayseFilter(weight, dim, sample_index); //意見数分数値が返ってくる.sample_indexの項はmax,他はother
                        post_beliefs = post_beliefs.PointwiseMultiply(likelihoods); //信念値更新．post_beliefs(この時点では事前分布)×likelihoods．要素同士の掛け算
                    }
                }
                //Debug.Assert(receive_opinion[sample_index] == 1);

                post_beliefs = post_beliefs.Map(value => Math.Round(value, 3)); //小数第三位までに丸める


                if (post_beliefs.Sum() == 0) //信念値の合計が0のとき
                {
                    post_beliefs.Clear();
                    return post_beliefs + (1.0 / dim);
                }
                return post_beliefs.Normalize(1.0); //全要素合計して1になるように正規化
            }
        }

        public Vector<double> UpdateBelief(Vector<double> belief, double weight, Vector<double> receive_opinions, BeliefUpdateFunctionEnum func_mode, double same_opinion_rate=0)
        {
            if (Double.IsNaN(weight))
            {
                Console.WriteLine();
            }

            switch (func_mode)
            {
                case BeliefUpdateFunctionEnum.Bayse: //ベイズ推定
                    return this.GetPostBeliefsByBayseFilter(belief, receive_opinions, weight);
                case BeliefUpdateFunctionEnum.Particle:
                    return this.GetPostBeliefsByParticleFilter(belief, receive_opinions, weight);
                case BeliefUpdateFunctionEnum.AATPaperBayse:
                    return this.GetPostBeliefsByAATPaperBayseFilter(belief, receive_opinions, weight);
                case BeliefUpdateFunctionEnum.SameOpinionAdjustBayse:
                    return this.GetPostBeliefsBySameOpinionAdjustBayseFilter(belief, receive_opinions, weight,same_opinion_rate);

            }
            Debug.Assert(false);
            return null;
        }

        public double CalcSingleBelief(Vector<double> pre_beliefs, int belief_dim, int op_dim, double weight, double op_dust = 0.0)
        {
            var upper = pre_beliefs[belief_dim] * this.ConvertWeight(weight, belief_dim, op_dim, pre_beliefs.Count, op_dust);

            var lower = 0.001;
            foreach (var lower_belief_dim in Enumerable.Range(0, pre_beliefs.Count))
            {
                var pre_belief = pre_beliefs[lower_belief_dim];
                lower += pre_belief * this.ConvertWeight(weight, lower_belief_dim, op_dim, pre_beliefs.Count, op_dust);
            }

            var pos_belief = upper / lower;

            return Math.Round(pos_belief, 4);
        }

        public double ConvertWeight(double weight, int belief_dim, int op_dim, int dim_size, double op_dust)
        {
            if (op_dust != 0.0)
            {
                weight = (weight - 1 / dim_size) * op_dust + 1 / dim_size;
            }

            if (belief_dim == op_dim)
            {
                return weight;
            }
            else
            {
                return (1 - weight) / (dim_size - 1);
            }
        }
    }
}
