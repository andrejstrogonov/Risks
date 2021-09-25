using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SQLiteLibrary
{
    [Serializable]
    public class SerializeHierarhy
    {
        public int Amount { get; set; }
        public string Name { get; set; }
        public List<SerializeHierarhy> children { get; set; }
        public SerializeHierarhy() { }

        public bool Equal(SerializeHierarhy serializeHierarhy)
        {
            if (this.Amount == serializeHierarhy.Amount &&
            this.Name == serializeHierarhy.Name && serializeHierarhy.children.Count() == children.Count())
            {
                foreach (var c in children)
                {
                    if (!serializeHierarhy.children.Any(p => p.Name == c.Name))
                        return false;
                    var equal = serializeHierarhy.children.Where(p => p.Name == c.Name).First();
                    if (!equal.Equal(c))
                        return false;
                }
                return true;
            }
            else
                return false;

        }

        public bool EqualHierarhy(SerializeHierarhy serializeHierarhy)
        {
            if (this.Name == serializeHierarhy.Name && serializeHierarhy.children.Count() == children.Count())
            {
                foreach (var c in children)
                {
                    if (!serializeHierarhy.children.Any(p => p.Name == c.Name))
                        return false;
                    var equal = serializeHierarhy.children.Where(p => p.Name == c.Name).First();
                    if (!equal.EqualHierarhy(c))
                        return false;
                }
                return true;
            }
            else
                return false;

        }


        public bool Rename(List<string> path, string next, int index=0)
        {
            if (Name == path[index])
            {
                if (index == path.Count - 1)
                {
                    Name = next;
                    return true;
                }
                else
                {
                    foreach (var c in children)
                    {
                        if (c.Rename(path, next, index+1))
                            return true;
                    }
                }
            }
            return false;
        }
    }

    public class AnalysSettings
    {
        public SerializeHierarhy SerializeHierarhy { get; set; }

        public int StartRisk { get; set; }
        public int FinishRisk { get; set; }

        public bool[] AnalysParams { get; set; }
        public bool[] AnalysGraphics { get; set; }

        public AnalysSettings()
        {
            AnalysParams = new bool[9] { true, true, true, true, true, true, true, true, true};            
            AnalysGraphics = new bool[5] { true, true, true, true, false};
            StartRisk = 70;
            FinishRisk = 100;
        }
    }

    public class AnalysSettingsSetuper
    {
        private AnalysSettings AnalysSettings;
        private string _path;
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
                AnalysSettings = Get();
            }
        }

        public AnalysSettingsSetuper(string path)
        {
            Path = path;
        }

        public AnalysSettings Get()
        {
            if (!(AnalysSettings is null))
            {
                return AnalysSettings;
            }
            return GetFromFile();
        }

        public AnalysSettings GetFromFile()
        {
            if (!File.Exists(Path))
            {
                AnalysSettings = CreateNew();
                return AnalysSettings;
            }
            var Serializer = new XmlSerializer(typeof(AnalysSettings));
            var file = new StreamReader(Path);
            try
            {
                AnalysSettings analys = (AnalysSettings)Serializer.Deserialize(file);
                file.Close();
                AnalysSettings = analys;
                if (AnalysSettings is null)
                {
                    AnalysSettings = CreateNew();
                }
                return AnalysSettings;
            }
            catch (System.InvalidOperationException)
            {
                file.Close();
                AnalysSettings = CreateNew();
                return AnalysSettings;
            }
        }

        private AnalysSettings CreateNew()
        {
            if (Path is null)
                return null;
            var set = new AnalysSettings();
            var writer = new XmlSerializer(typeof(AnalysSettings));
            var wfile = new StreamWriter(Path);
            writer.Serialize(wfile, set);
            wfile.Close();
            return set;
        }

        public void Save(AnalysSettings analysSettings=null)
        {
            if (!(analysSettings is null))
            {
                AnalysSettings = analysSettings;
            }
            if (AnalysSettings is null)
                return;
            var writer = new XmlSerializer(typeof(AnalysSettings));
            var wfile = new StreamWriter(_path);
            writer.Serialize(wfile, AnalysSettings);
            wfile.Close();
        }
    }
}
