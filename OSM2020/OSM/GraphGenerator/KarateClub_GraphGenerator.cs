using OSM2020.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;   

namespace OSM2020.OSM.GraphGenerator
{
    class KarateClub_GraphGenerator : GraphGeneratorBase
    {
        int NodeSize;
        int AttachEdges;
        public override GraphEnum MyGraphEnum { get; }
        public override string GeneratePath { get; protected set; }
        public override bool SeedEnable { get; protected set; }

        public KarateClub_GraphGenerator()
        {
            this.MyGraphEnum = GraphEnum.KarateClub;
            this.SeedEnable = true;
            this.SetGeneratePath();
        }

        public KarateClub_GraphGenerator SetNodeSize(int n)
        {
            this.NodeSize = n;
            this.SetGeneratePath();
            return this;
        }

        public KarateClub_GraphGenerator SetAttachEdges(int m)
        {
            this.AttachEdges = m;
            this.SetGeneratePath();
            return this;
        }

        protected override void SetGeneratePath()
        {
            var path = Properties.Settings.Default.GraphGeneratorFolderPath + "karate_club_graph.py";
            this.GeneratePath = "" + path + " " + this.NodeSize + " " + this.AttachEdges;
        }
    }
}
