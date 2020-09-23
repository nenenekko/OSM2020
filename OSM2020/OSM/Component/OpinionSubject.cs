using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.Component
{
    class OpinionSubject
    {
        public string SubjectName { get; private set; }  //実験名
        public int SubjectDimSize { get; private set; }  //意見数
        SubjectManager MySubjectManager;

        public OpinionSubject(string subject_name, int subject_dim_size) //話題名と次元数
        {
            this.SubjectName = subject_name; //そのままセット
            this.SubjectDimSize = subject_dim_size;
        }

        public void SetSubjectManager(SubjectManager subject_manager)
        {
            this.MySubjectManager = subject_manager;
        }

        public Vector<double> ConvertOpinionForSubject(Vector<double> opinion, OpinionSubject to_subject)
        {
            var conv_matrix = this.MySubjectManager.GetConversionMatrix(this, to_subject);
            return conv_matrix * opinion;
        }
    }
}
