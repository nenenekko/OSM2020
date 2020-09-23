using MathNet.Numerics.LinearAlgebra;
using OSM2020.OSM;
using OSM2020.Utility;
using OSM2020.OSM.DistributionGenerator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.Component
{
    class SubjectManagerGenerator
    {
        public SubjectManager Generate(OpinionSubject opinion_subject, double dist_weight, int correct_dim, double sensor_weight, EnvDistributionEnum env_dis_mode, int malicious_dim = 0, double malicious_dist_weight = 0.0)
        {
            CustomDistribution env_dist = null;
            CustomDistribution env_malicious_dist = null;
            switch (env_dis_mode)
            {
                case EnvDistributionEnum.Turara:
                    env_dist = new Turara_DistGenerator(opinion_subject.SubjectDimSize, dist_weight, correct_dim).Generate();　 //maxとotherを計算して返す
                    env_malicious_dist = new Turara_DistGenerator(opinion_subject.SubjectDimSize, malicious_dist_weight, malicious_dim).Generate(); //同上
                    break;
                case EnvDistributionEnum.Exponential:
                    env_dist = new Exponential_DistGenerator(opinion_subject.SubjectDimSize, dist_weight, correct_dim).Generate();
                    env_malicious_dist = new Exponential_DistGenerator(opinion_subject.SubjectDimSize, malicious_dist_weight, malicious_dim).Generate();
                    break;
                case EnvDistributionEnum.Shitei:
                    env_dist = new Shitei_DistGenerator(opinion_subject.SubjectDimSize, dist_weight, correct_dim).Generate();
                    env_malicious_dist = new Shitei_DistGenerator(opinion_subject.SubjectDimSize, malicious_dist_weight, malicious_dim).Generate();
                    break;
            }
            Debug.Assert(env_dist != null); //計算できてなかったらエラー
            Debug.Assert(env_malicious_dist != null);

            var subject_tv = new OpinionSubject("good_tv", 3);
            var subject_test = new OpinionSubject("test", opinion_subject.SubjectDimSize);
            var subject_company = new OpinionSubject("good_company", 2);
            double[] conv_array = { 1, 0, 0, 1, 1, 0 };
            var conv_matrix = Matrix<double>.Build.DenseOfColumnMajor(2, 3, conv_array); //2*3の形にリシェイプ

            var osm_env = new OpinionEnvironment()
                          .SetSubject(subject_test)
                          .SetCorrectDim(correct_dim) //正しい次元
                          .SetMaliciousDim(malicious_dim) //間違った次元
                          .SetSensorWeight(sensor_weight) //センサウェイト
                          .SetCustomDistribution(env_dist) 
                          .SetMaliciousCustomDistribution(env_malicious_dist);


            var subject_manager = new SubjectManager() //サブジェクトマネージャー生成
                                    .AddSubject(subject_test) 
                                    .RegistConversionMatrix(subject_tv, subject_company, conv_matrix) //オピニオンにサブジェクトマネージャーを登録.サブジェクトマネージャーに意見交換クラスとしてこれらの情報を登録
                                    .SetEnvironment(osm_env); //環境をセット
            return subject_manager;
        }
    }
}
