using OSM2020.Utility;
using OSM2020.OSM.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OSM2020.OSM.LayoutGenerator
{
    class KamadaKawai_LayoutGenerator : LayoutGeneratorBase
    {
        public override LayoutEnum MyLayoutEnum { get; }
        protected override string GeneratePath { get; }
        protected override RawGraph MyGraph { get; }

        public KamadaKawai_LayoutGenerator(RawGraph graph)
        {
            this.MyGraph = graph;
            this.MyLayoutEnum = LayoutEnum.KamadaKawai;
            var path = Properties.Settings.Default.LayoutGeneratorFolderPath + "kamada_kawai_layout.py";
            this.GeneratePath = path;
        }
    }
}
