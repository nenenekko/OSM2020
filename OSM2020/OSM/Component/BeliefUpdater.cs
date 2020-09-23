using MathNet.Numerics.LinearAlgebra;
using OSM2020.OSM;
using OSM2020.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.Component
{
    class BeliefUpdater
    {
        public double SensorWeight { get; private set; }
        public SensorWeightEnum SensorWeightMode { get; private set; }
        public BeliefUpdateFunctionEnum MyBeliefUpdateFunctionMode { get; private set; }

        public BeliefUpdater(BeliefUpdateFunctionEnum mode)
        {
            this.MyBeliefUpdateFunctionMode = mode;
        }
        public BeliefUpdater SetBeliefUpdateFunctionMode(BeliefUpdateFunctionEnum func_mode)
        {
            this.MyBeliefUpdateFunctionMode = func_mode;
            return this;
        }
        public BeliefUpdater SetSensorWeightMode(SensorWeightEnum sw_mode, double value = 0.0)
        {
            this.SensorWeightMode = sw_mode;
            switch (this.SensorWeightMode)
            {
                case SensorWeightEnum.DependSensorRate:
                    break;
                case SensorWeightEnum.Custom:
                    this.SensorWeight = value;
                    break;
                case SensorWeightEnum.SameNoneSensor:
                    break;
            }
            return this;
        }

        public Vector<double> UpdateBelief(OSMBase osm, Message message, double same_opinion_rate) //OSMとメッセージ(誰から誰へ．どのリンクで，どんなsubjectでどの意見が形成されているか)
        {
            Vector<double> receive_op;
            var pre_belief = message.ToAgent.Belief; //意見送信先のエージェントの信念値コピー
            var weight = message.GetToWeight(); //messageのリンクのソースAgentとToAgentとFromAgentの一致している方のweightを返す

            if (message.ToAgent.IsSensor && message.ToAgent.IsDetermined() && osm.GetType() == typeof(OSM_Only)) return pre_belief; //OSM_Onlyはweightいじる必要とかもないのでここで終わり
            //if (message.ToAgent.IsSensor && message.ToAgent.IsDetermined()) return pre_belief;

            if (message.Subject.SubjectName != message.ToAgent.MySubject.SubjectName) //意見を受け取ったエージェント(toAgent)のsubjectがmessageのsubjectと一致していないなら
            {
                var to_subject = message.ToAgent.MySubject;
                receive_op = message.Subject.ConvertOpinionForSubject(message.Opinion, to_subject);
            }
            else //話題が一緒なら
            {
                receive_op = message.Opinion.Clone(); //受け取った意見として意見値をコピーする
            }


            Vector<double> updated_belief = null;

            if (message.FromAgent.AgentID < 0) //意見の送信元(fromAgent)が負のID(環境エージェント)なら
            {
                switch (this.SensorWeightMode) //センサウェイトモード
                {
                    //受信エージェントの元の信念値，環境からセンサへの重み，受け取った意見，信念値の更新方法を引数にとって，信念値更新.環境からセンサへの重みのかけ方が異なる
                    case SensorWeightEnum.DependSensorRate:
                        updated_belief = osm.MyAggFuncs.UpdateBelief(pre_belief, osm.MyEnvManager.SensorWeight, receive_op, this.MyBeliefUpdateFunctionMode); 
                        break;
                    case SensorWeightEnum.Custom:
                        updated_belief = osm.MyAggFuncs.UpdateBelief(pre_belief, this.SensorWeight, receive_op, this.MyBeliefUpdateFunctionMode);
                        break;
                    case SensorWeightEnum.SameNoneSensor:
                        updated_belief = osm.MyAggFuncs.UpdateBelief(pre_belief, weight, receive_op, this.MyBeliefUpdateFunctionMode);
                        break;
                    case SensorWeightEnum.FollowEnvDistWeight:
                        updated_belief = osm.MyAggFuncs.UpdateBelief(pre_belief, osm.MyEnvManager.MyCustomDistribution.MyDistWeight, receive_op, this.MyBeliefUpdateFunctionMode);
                        break;
                    default:
                        break;
                }
            }
            else //エージェント同士で意見を送信している場合
            {
                //updated_belief = osm.MyAggFuncs.UpdateBelief(pre_belief, weight, receive_op, this.MyBeliefUpdateFunctionMode);
                updated_belief = osm.MyAggFuncs.UpdateBelief(pre_belief, weight, receive_op, this.MyBeliefUpdateFunctionMode, same_opinion_rate); //weightはリンクに紐づいた重みを用いる
            }

            Debug.Assert(updated_belief != null);
            return updated_belief; //更新した信念値を返す
        }
    }
}
