using SQLiteLibrary.DataAccess;
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
    /*
    public enum VATType
    {
        main_6,
        main_15,
        full
    }

    [Serializable]
    public class Settings
    {
        public VATType VAT_type { get; set; }
        public double Budget { get; set; }

        public Unit PeriodUnit { get; set; }
        public double Period { get; set; }

        public int Level { get; set; }

        public Settings(VATType vt, double budget, Unit unit,double Period, int Level=3)
        {
            VAT_type = vt;
            Budget = budget;
            PeriodUnit = unit;
            this.Period = Period;
            this.Level = Level;
        }
        public Settings()
        {
            Level = 3;
            VAT_type = VATType.full;
            this.Budget = double.PositiveInfinity;
            Period = 1;
            PeriodUnit = UnitDAL.GetUnits(TypeOfUnit.Time).FirstOrDefault();
        }
    }

    public static class SettingsSetuper
    {
        private static Settings Settings;

        private static bool path_changed = false;

        private static string _path = "MainSettings.xml";

        public static string Path
        {
            get
            {
                return _path;
            }
            set
            {
                /*if (!(_path is null))
                    Save();*/
                /*_path = value;
                path_changed = true;
                Settings  = Get();
            }
        }

        private static Settings CreateNew()
        {
            var set = new Settings();
            var writer = new XmlSerializer(typeof(Settings));
            var wfile = new StreamWriter(Path);
            writer.Serialize(wfile, set);
            wfile.Close();
            return set;
        }

        public static Settings Get()
        {
            if (Settings != null && !path_changed)
                return Settings;
            path_changed = false;
            if (!File.Exists(Path)) {
                Settings = CreateNew();
                return Settings;
            }
            var Serializer = new XmlSerializer(typeof(Settings));
            var file = new StreamReader(Path);
            try
            {
                Settings settings = (Settings)Serializer.Deserialize(file);
                file.Close();
                Settings = settings;
                if(Settings is null)
                {
                    Settings = CreateNew();
                }
                return Settings;
            }
            catch (System.InvalidOperationException e)
            {
                file.Close();
                Settings = CreateNew();
                return Settings;
            }
        }

        public static void Save(Settings settings=null)
        {
            if(!(settings is null))
            {
                Settings = settings;
            }
            var writer = new XmlSerializer(typeof(Settings));
            var wfile = new StreamWriter(Path);
            writer.Serialize(wfile, Settings);
            wfile.Close();
        }

    }*/
}
