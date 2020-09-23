using Newtonsoft.Json;
using OSM2020.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.Graph
{
    public class RawGraph
    {
        public bool Directed { get; set; }
        public bool Multigraph { get; set; }
        //public string Graph { get; set; }
        public List<Node> Nodes { get; set; }
        public List<Link> Links { get; set; }
        internal GraphEnum MyGraphEnum { get; set; }

        public void OutputGraphJSON() //設定で指定したパスにjsonで出力
        {
            var working_path = Properties.Settings.Default.WorkingFolderPath;
            string graph_filepath = Properties.Settings.Default.TmpGraphFile;
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(working_path + graph_filepath, false, System.Text.Encoding.UTF8);
            sw.Write(json);
            sw.Close();
        }

        public List<Link> GetLinksOfSource(int source_id)
        {
            return this.Links.Where(l => l.Source == source_id).ToList();
        }
    }
}
