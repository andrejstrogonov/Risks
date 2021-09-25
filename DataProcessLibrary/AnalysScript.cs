using DataProcessLibrary.Analys;
using ProjectManagerLibrary.WorkWithProjects;
using SQLiteLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataProcessLibrary
{
    [Serializable]
    public class AnalysScript
    {
        public SerializeHierarhy SerializeHierarhy { get; set; }

        public DecisionGraphic DecidionGraphic { get; set; }

        public double StartRisk { get; set; }
        public double FinishRisk { get; set; }

        public DateTime DateOfCreating { get; set; }

        public AnalysScript(DecisionGraphic decidionGraphic)
        {
            DecidionGraphic = decidionGraphic;
            DateOfCreating = DateTime.Now;
        }
        public AnalysScript()
        {
            DecidionGraphic = null;
            
        }

        public void Deserialize()
        {
            if (DecidionGraphic is null)
                return;
            DecidionGraphic.Deserialize();
        }
    }

    public class AnalysSetuper
    {
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
                AnalysScript = Get();
            }
        }

        private AnalysScript AnalysScript { get; set; }

        public AnalysScript Get(DateTime dateTime, double startRisk, double finishRisk)
        {
            if (AnalysScript is null)
                return null;
            if (AnalysScript.DateOfCreating >= dateTime && AnalysScript.StartRisk==startRisk && AnalysScript.FinishRisk==finishRisk)
            {
                return AnalysScript;
            }
            return null;
        }

        public AnalysScript GetCommon(DateTime dateTime, double startRisk, double finishRisk, Project project)
        {
            if (AnalysScript is null)
                return null;
            if (!(AnalysScript.SerializeHierarhy is null) && AnalysScript.SerializeHierarhy.Equal(project.AnalysSettingsSetuper.Get().SerializeHierarhy) && AnalysScript.DateOfCreating >= dateTime && AnalysScript.StartRisk == startRisk && AnalysScript.FinishRisk == finishRisk)
            {
                return AnalysScript;
            }
            return null;
        }

        private AnalysScript Get()
        {
            if (AnalysScript != null)
                return AnalysScript;
            if (!File.Exists(Path))
            {
                AnalysScript = null;
                return null;
            }
            var Serializer = new XmlSerializer(typeof(AnalysScript));
            var file = new StreamReader(Path);
            try
            {
                AnalysScript analys = (AnalysScript)Serializer.Deserialize(file);
                file.Close();                
                analys.Deserialize();
                AnalysScript = analys;
                return AnalysScript;
            }
            catch (System.InvalidOperationException e)
            {
                AnalysScript = null;
                file.Close();
                return null;
            }

        }

        public void SaveNew(DecisionGraphic decidionGraphic, Project project)
        {
            AnalysScript = new AnalysScript(decidionGraphic);
            AnalysScript.StartRisk =Math.Round( SimplexStage.StartRisk * 100);
            AnalysScript.FinishRisk =Math.Round( SimplexStage.FinishRisk * 100);
            AnalysScript.SerializeHierarhy = project.AnalysSettingsSetuper.Get().SerializeHierarhy;
            var writer = new XmlSerializer(typeof(AnalysScript));
            var wfile = new StreamWriter(_path);
            writer.Serialize(wfile, AnalysScript);
            wfile.Close();
        }

    }
}
