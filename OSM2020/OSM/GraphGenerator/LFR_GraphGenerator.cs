using OSM2020.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.GraphGenerator
{
    class LFR_GraphGenerator : GraphGeneratorBase
    {
        int NodeSize;
        int AttachEdges;
        public override GraphEnum MyGraphEnum { get; }
        public override string GeneratePath { get; protected set; }
        public override bool SeedEnable { get; protected set; }
        private static Object lockObj = new Object();

        public LFR_GraphGenerator()
        {
            this.MyGraphEnum = GraphEnum.LFR;
            this.SeedEnable = false;
            this.SetGeneratePath();
        }

        public LFR_GraphGenerator SetNodeSize(int n)
        {
            this.NodeSize = n;
            this.SetGeneratePath();
            return this;
        }

        public LFR_GraphGenerator SetAttachEdges(int m)
        {
            this.AttachEdges = m;
            this.SetGeneratePath();
            return this;
        }

        protected override void SetGeneratePath()
        {
            var path = Properties.Settings.Default.GraphGeneratorFolderPath + "lfr_benchmark_graph.py";
            this.GeneratePath = "" + path + " " + this.NodeSize;
        }
    }
}
