using OSM2020.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.GraphGenerator
{
    class Null_GraphGenerator : GraphGeneratorBase
    {
        public override GraphEnum MyGraphEnum => throw new NotImplementedException();

        public override string GeneratePath { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override bool SeedEnable { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        protected override void SetGeneratePath()
        {
            throw new NotImplementedException();
        }
    }
}
