using OSM2020.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.GraphGenerator
{
    class Grid2D_GraphGenerator : GraphGeneratorBase
    {
        int Height;
        int Width;
        public override GraphEnum MyGraphEnum { get; }
        public override string GeneratePath { get; protected set; }
        public override bool SeedEnable { get; protected set; }

        public Grid2D_GraphGenerator()
        {
            this.MyGraphEnum = GraphEnum.Grid2D;
            this.SeedEnable = false;
            this.SetGeneratePath(); //grid2d.pyのパス指定．その際，ノードの縦幅＆横幅指定
        }

        public Grid2D_GraphGenerator SetHeight(int height)
        {
            this.Height = height;
            this.SetGeneratePath();
            return this;
        }

        public Grid2D_GraphGenerator SetWidth(int width)
        {
            this.Width = width;
            this.SetGeneratePath();
            return this;
        }

        public Grid2D_GraphGenerator SetNodeSize(int n)
        {
            var upper = Math.Ceiling(Math.Sqrt(n));
            var lower = Math.Floor(Math.Sqrt(n));

            this.Height = (int)upper; //縦幅
            this.Width = (int)lower;  //横幅
            this.SetGeneratePath();
            return this;
        }

        protected override void SetGeneratePath()
        {
            var path = Properties.Settings.Default.GraphGeneratorFolderPath + "grid_2d_graph.py";
            this.GeneratePath = path + " " + this.Height + " " + this.Width;
        }
    }
}
