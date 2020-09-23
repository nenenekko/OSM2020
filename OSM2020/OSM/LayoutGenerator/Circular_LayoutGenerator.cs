using OSM2020.OSM.Graph;
using OSM2020.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.LayoutGenerator
{
    class Circular_LayoutGenerator : LayoutGeneratorBase
    {
        public override LayoutEnum MyLayoutEnum { get; }
        protected override string GeneratePath { get; } //pythonのレイアウト生成ファイルのパス
        protected override RawGraph MyGraph { get; }

        public Circular_LayoutGenerator(RawGraph graph)
        {
            this.MyGraph = graph;
            this.MyLayoutEnum = LayoutEnum.Circular;
            var path = Properties.Settings.Default.LayoutGeneratorFolderPath + "circular_layout.py";
            this.GeneratePath = path;
        }
    }
}
