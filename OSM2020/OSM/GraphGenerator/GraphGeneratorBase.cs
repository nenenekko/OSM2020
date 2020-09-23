using Konsole;
using Newtonsoft.Json;
using OSM2020.OSM.Graph;
using OSM2020.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.GraphGenerator
{
    abstract class GraphGeneratorBase
    {
        public abstract GraphEnum MyGraphEnum { get; }
        public abstract string GeneratePath { get; protected set; }
        public abstract bool SeedEnable { get; protected set; }

        protected abstract void SetGeneratePath();
        private static Object lockObj = new Object();
        private static Object lockObj2 = new Object();

        public OSMSetting osm_setting;

        public void SetOsmSetting(OSMSetting osm_setting)
        {
            this.osm_setting = osm_setting;
        }
        public RawGraph Generate(int graph_seed, ExtendProgressBar pb_graph)
        {
            var working_folder_path = Properties.Settings.Default.WorkingFolderPath;
            var state = 0;
            switch (state)
            {
                case 0:

                    //Console.WriteLine("-----");
                    //Console.WriteLine("ok Start Graph Generation");
                    var delete_success = this.DeleteGraphJSON(pb_graph); //グラフjson削除
                    if (!delete_success) goto default;

                    var python_success = this.PythonGraphGenerate(graph_seed, this.SeedEnable); //グラフjson生成
                    if (!python_success) goto default;

                    var raw_graph = this.ReadJSON(); //生成したグラフjson読み込み
                    var graph_enum = this.MyGraphEnum; //graphGeneratorのインスタンス化時に設定済み
                    raw_graph.MyGraphEnum = graph_enum;
                    if (raw_graph.Links == null || raw_graph.Nodes.Count == 0 || graph_enum == GraphEnum.Void) goto default;

                    pb_graph.Refresh($"{graph_enum.ToString()} size:{raw_graph.Nodes.Count} edge:{raw_graph.Links.Count} seed:{graph_seed}");
                    //Console.Write("Node: " + raw_graph.Nodes.Count);
                    //Console.Write(" Edge: " + raw_graph.Links.Count);
                    //Console.Write(" GraphEnum: " + graph_enum.ToString());
                    //Console.WriteLine(" Graph Seed: " + graph_seed);
                    return raw_graph;

                default:
                    //Console.WriteLine("no Failure Graph Generation");
                    pb_graph.Refresh($"failure graph generate");
                    return null;
            }
        }

        bool DeleteGraphJSON(ExtendProgressBar pb_graph)
        {
            lock (lockObj)
            {
                var path = Properties.Settings.Default.WorkingFolderPath;

                if (!Directory.Exists(path))
                {
                    return true;
                }
                string[] filePaths = Directory.GetFiles(path);

                try
                {
                    foreach (string filePath in filePaths)
                    {
                        var graph = Properties.Settings.Default.RawGraphFile;
                        var graph_flag = Properties.Settings.Default.RawGraphFlag;
                        if (filePath.Contains(graph) || filePath.Contains(graph_flag))
                        {
                            File.SetAttributes(filePath, FileAttributes.Normal);
                            File.Delete(filePath);
                        }

                    }
                    pb_graph.Refresh("delete json graph.");
                    return true;
                }
                catch (Exception)
                {
                    pb_graph.Refresh("fail to delete json graph");
                    return false;
                }
            }
        }

        bool PythonGraphGenerate(int graph_seed, bool seed_enable)
        {
            if (seed_enable)
            {
                PythonProxy.ExecutePythonScript(this.GeneratePath + " " + graph_seed);
            }
            else
            {
                PythonProxy.ExecutePythonScript(this.GeneratePath);
            }
            bool exist_flag = false;
            while (!exist_flag)
            {
                var working_path = Properties.Settings.Default.WorkingFolderPath;
                var graph_flag = Properties.Settings.Default.RawGraphFile;
                exist_flag = File.Exists(working_path + graph_flag);
                System.Threading.Thread.Sleep(100);
                if (PythonProxy.ErrorFlag)
                {
                    System.Threading.Thread.Sleep(100);
                    return false;
                }
            }
            return true;
        }

        RawGraph ReadJSON()
        {
            lock (lockObj2)
            {
                var working_path = Properties.Settings.Default.WorkingFolderPath;
                string graph_filepath = Properties.Settings.Default.RawGraphFile;
                string jsonString = System.IO.File.ReadAllText(working_path + graph_filepath, Encoding.UTF8);
                RawGraph raw_graph = JsonConvert.DeserializeObject<RawGraph>(jsonString);
                return raw_graph;
            }
        }

        public List<List<int>> GetCommunity()
        {
            string communityString = ReadText();
            string[] communityArray = communityString.Split('/');
            List<List<int>> communityList = new List<List<int>>();
            int i = 0;
            foreach(var community in communityArray)
            {
                string[] community_member = community.Split(',');
                List<int> tempList = new List<int>();
                if(community != "")
                {
                    foreach (var member in community_member)
                    {
                        tempList.Add(Int32.Parse(member));
                    }
                    communityList.Add(new List<int>());
                    communityList[i++].AddRange(tempList);
                }
            }
            return communityList;
        }
        public string ReadText()
        {
            lock (lockObj)
            {
                var working_path = Properties.Settings.Default.WorkingFolderPath;
                string community_filepath = osm_setting.community_file_path;
                string communityString = System.IO.File.ReadAllText(working_path + community_filepath, Encoding.UTF8);
                communityString = communityString.Replace("\r","");
                communityString = communityString.Replace("\n", "");
                return communityString;
            }
        }
    }
}
