using MathNet.Numerics.LinearAlgebra;
using OSM2020.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.DistributionGenerator
{
    interface I_DistGenerator
    {
        CustomDistribution MyCustomDistribution { get; set; }
        CustomDistribution Generate();
    }
}
