using SQLiteLibrary;
using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ProjectManagerLibrary.WorkWithProjects
{
    [Serializable]
    [XmlInclude(typeof(Session))]
    [XmlInclude(typeof(Section))]
    [XmlInclude(typeof(Project))]
    public class HierarhyElem
    {
        protected static string HistoryName = "History.xml";
        protected static string AnalysName = "Analys.xml";
        protected static string AnalysSettings = "AnalysSettings.xml";
        public static string commonProjectsDirectory = @"Projects";

        public static string UnallowableChars = "\\|/:;*?+<>.%!@*";


        [XmlIgnore]
        public HistorySetuper HistorySetuper { get; set; }

        private AnalysSettingsSetuper analysSettingsSetuper;
        [XmlIgnore]
        public AnalysSettingsSetuper AnalysSettingsSetuper { get
            {
                if(this is Project)
                {
                    return analysSettingsSetuper;
                }
                return GetProject().AnalysSettingsSetuper;
            }
            set
            {
                if(this is Project)
                {
                    analysSettingsSetuper = value;
                }
                else
                    GetProject().AnalysSettingsSetuper = value;
            }
        }

        public void SaveChanges()
        {
            HistorySetuper.SaveReal();
            if (HierarhyElems is null)
                return;
            foreach(var h in HierarhyElems)
            {
                h.Value.SaveChanges();
            }
        }

        public static void OpenCommonProjectDirectory()
        {
            if (!Directory.Exists(commonProjectsDirectory))
            {
                Directory.CreateDirectory(commonProjectsDirectory);
            }
            else
                CheckAndFixCommonProjectDirectory(commonProjectsDirectory);
        }

        private static void CheckAndFixCommonProjectDirectory(string path)
        {
            foreach (var f in Directory.GetFiles(path))
            {
                    File.Delete(Path.Combine(path, f));
            }
        }

        public DateTime DateOfChange
        {
            get
            {
                Activate();
                DateTime min = SettingsDAL.GetSettings().DateOfCreation > HistorySetuper.Get().LastTimeAccess? SettingsDAL.GetSettings().DateOfCreation : HistorySetuper.Get().LastTimeAccess;
                if (HierarhyElems is null)
                    return min;
                foreach(var h in HierarhyElems)
                {
                    var d = h.Value.DateOfChange;
                    if (d > min)
                    {
                        min = d;
                    }
                }
                HistorySetuper.Get().LastTimeAccess = min;
                return min;
            }
        }

        public void SetAnalysSettings(AnalysSettings analysSettings)
        {
            AnalysSettingsSetuper.Save(analysSettings);
            /*foreach(var c in HierarhyElems)
            {
                c.Value.SetAnalysSettings(analysSettings);
            }*/
        }
        
        public delegate void ChildrenChanged();
        public event ChildrenChanged OnChange;

        public DateTime DateOfChange_History
        {
            get
            {
                var hist = HistorySetuper.Get();
                Activate();
                var sett = SettingsDAL.GetSettings().DateOfCreation;
                DateTime min =hist.HistoryList.Count()>0? hist.HistoryList.Max(p=>p.DateTime): sett;
                if (HierarhyElems is null)
                    return min;
                foreach (var h in HierarhyElems)
                {
                    var d = h.Value.DateOfChange_History;
                    if (d > min)
                    {
                        min = d;
                    }
                }
                return min;
            }
        }
        public DateTime DateOfCreation
        {
            get
            {
                Activate();
                return SettingsDAL.GetSettings().DateOfCreation;
            }
        }

        public string Name { get; set; }
        public HierarhyElem Parent { get; set; }
        public string parentDirectory { get; set; }

        public string ParentDirectory
        {
            get
            {
                if(Parent is null)
                {
                    return parentDirectory;
                }
                return Parent.DirectoryPath;
            }
        }



        public string DirectoryPath
        {
            get
            {
                return Path.Combine(ParentDirectory, Name);
            }
        }
        public string DBName
        {
            get
            {
                return Name + ".db";
            }
        }
               
        public string DBPath
        {
            get
            {
                return Path.Combine(DirectoryPath, DBName);
            }
        }

        public string DBString
        {
            get
            {
                return "Data Source=" + Path.Combine(DirectoryPath, DBName);
            }
        }
        public string HistoryPath
        {
            get
            {
                return Path.Combine(DirectoryPath, HistoryName);
            }
        }

        public string AnalysSettingsPath
        {
            get
            {
                return Path.Combine(DirectoryPath, AnalysSettings);
            }
        }

        public string AnalysPath
        {
            get
            {
                return Path.Combine(DirectoryPath, AnalysName);
            }
        }

        public HierarhyElem(HierarhyElem parent, string name, bool needload=true)
        {
            Parent = parent;
            Name = name;
            CheckAndFix();
            Activate();
            if (needload)
            {
                
                LoadElems();
            }
        }

        public HierarhyElem(string parentDirectory, string name, bool needload = true)
        {
            Parent = null;
            this.parentDirectory = parentDirectory;

            if (!(name is null))
                this.Name = name;
            else
            {
                this.Name = GenerateDefaultName(parentDirectory, "Проект");
            }
            CheckAndFix();
            Activate();
            if (needload)
            {
                
                LoadElems();
            }
        }
        public HierarhyElem()
        {
            //
        }

        public void ResetPath()
        {
            HistorySetuper.Path = HistoryPath;
            AnalysSettingsSetuper.Path = AnalysSettingsPath;
            
            SampleContext.Path = DBString;
            foreach(var c in HierarhyElems)
            {
                c.Value.ResetPath();
            }
        }


        [XmlIgnore]
        public Dictionary<string, HierarhyElem> HierarhyElems { get; set; }

        public void Delete()
        {
            System.IO.Directory.Delete(DirectoryPath, true);
            if (!(Parent is null))
            {
                Parent.HierarhyElems.Remove(Name);
                if (!(Parent.OnChange is null))
                    Parent.OnChange();
            }
        }

        public bool Rename(string name)
        {
            if (Directory.Exists(Path.Combine(ParentDirectory, name)))
                return false;

            var lastsessname = Name;
            var lastnamedb = DBName;
            var newpath = Path.Combine(ParentDirectory, name);
            Directory.Move(DirectoryPath, newpath);
            Name = name;
            var lastdbpath = Path.Combine(DirectoryPath, lastnamedb);
            File.Move(lastdbpath, DBPath);
            ResetPath();
            if (!(Parent is null))
            {
                Parent.HierarhyElems.Remove(lastsessname);
                Parent.HierarhyElems[name] = this;
                if (!(Parent.OnChange is null))
                    Parent.OnChange();
                var set = AnalysSettingsSetuper.Get();
                List<string> path = new List<string>();
                path.Insert(0, lastsessname);
                var parent = Parent;
                while (!(parent is null))
                {
                    path.Insert(0, parent.Name);
                    parent = parent.Parent;
                }
                set.SerializeHierarhy.Rename(path, name);
                AnalysSettingsSetuper.Save(set);
            }
            return true;
        }

        public void Activate(bool needcreate=false)
        {
            SampleContext.Path = DBString;
            if (needcreate)
            {
                SampleContext db = new SampleContext(false);
                db.Dispose();//ни в коем случае не удалять
            }
        }

        protected static string GenerateDefaultName(string ParentDirectory,string start_name)
        {
            var name = start_name;
            int i = 1;
            while (Directory.Exists(Path.Combine(ParentDirectory,name)))
            {
                name = start_name + ' ' + i;
                i++;
            }
            return name;
        }

        protected void CreateMainDirectoryAndFiles()
        {
            Directory.CreateDirectory(DirectoryPath);
            Activate(true);
        }

        public bool AddElem(HierarhyElem hierarhyElem)
        {
            if (HierarhyElems.ContainsKey(hierarhyElem.Name))
                return false;
            HierarhyElems[hierarhyElem.Name] = hierarhyElem;
            if(!(OnChange is null))
            OnChange();
            return true;
        }

        public bool UpdateElem(string name, HierarhyElem hierarhyElem)
        {
            if (!HierarhyElems.ContainsKey(name))
                return false;
            HierarhyElems.Remove(name);
            HierarhyElems[hierarhyElem.Name] = hierarhyElem;
            return true;
        }

        public bool DeleteElem(string name)
        {
            if (!HierarhyElems.ContainsKey(name))
                return false;
            HierarhyElems[name].Delete();
            HierarhyElems.Remove(name);
            return true;
        }

        protected void LoadElems()
        {
            var directories = Directory.GetDirectories(DirectoryPath);
            HierarhyElems = new Dictionary<string, HierarhyElem>();
            foreach (var dir in directories)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                var item = new HierarhyElem(this, directoryInfo.Name);
                item.Activate();
                var lvl = SettingsDAL.GetSettings().Level;
                switch (lvl)
                {
                    case 3:item= new Session(this, directoryInfo.Name);break;
                    case 2:item = new Section(this, directoryInfo.Name);break;
                    default:item = new Project(directoryInfo.Name);break;
                }

                HierarhyElems[directoryInfo.Name] = item;
            }

            if (this is Project)
            {
                var h = AnalysSettingsSetuper.Get().SerializeHierarhy;
                var cur = GetSerializeHierarhy();
                if(h is null || (!h.EqualHierarhy(cur)))
                    AnalysSettingsSetuper.Get().SerializeHierarhy = cur;
            }
            else
            {
            }
        }

        public Project GetProject()
        {
            var elem = this;
            while(!(elem is Project))
            {
                elem = elem.Parent;
            }
            return elem as Project;
        }


        protected void CheckAndFix()
        {
            if (!Directory.Exists(DirectoryPath))
            {
                CreateMainDirectoryAndFiles();
            }
            if (!File.Exists(DBPath))
            {
                SampleContext.Path = DBString;
                SampleContext sampleContext = new SampleContext(false);
            }

            HistorySetuper = new HistorySetuper(HistoryPath);
            if (this is Project)
            {
                AnalysSettingsSetuper = new AnalysSettingsSetuper(AnalysSettingsPath);                
            }
            /*foreach (var f in Directory.GetFiles(DirectoryPath))
            {
                DirectoryInfo di = new DirectoryInfo(f);
                if(di.Name!=DBName && di.Name != HistoryName && di.Name!=AnalysName && di.Name!=AnalysSettings)
                {
                    File.Delete(f);
                }
            }*/
        }


        public bool AddCopy(Session session, string new_name, bool[] fieldsToCopy, Settings settings)
        {
            if (new_name is null || session is null)
                return false;
            if (HierarhyElems.ContainsKey(new_name))
                return false;
            HierarhyElems[new_name] = session.SaveAs(new_name, fieldsToCopy, settings,this);
            if (!(OnChange is null))
                OnChange();
            return true;
        }

        public SerializeHierarhy GetSerializeHierarhy()
        {
            SerializeHierarhy serializeHierarhy = new SerializeHierarhy() { Name = Name, Amount = 0 };
            serializeHierarhy.children = new List<SerializeHierarhy>();
            foreach (var c in HierarhyElems)
            {
                serializeHierarhy.children.Add(c.Value.GetSerializeHierarhy());
            }
            return serializeHierarhy;
        }

        public void CopyUnitsFrom(string path_from)
        {
            Activate();
            UnitDAL.CopyUnitDataFrom(path_from);
        }

        public static bool TryCopyProject(string ProjectPath)
        {
            if (!Directory.Exists(ProjectPath))
            {
                return false;
            }
            //CheckProjectPath
            if (!CheckProjectDirectory(ProjectPath))
                return false;
            //CopyFilesToProjectDirectory
            var newDir = new DirectoryInfo(ProjectPath);
            var newPath = Path.Combine(commonProjectsDirectory, newDir.Name);
            Directory.CreateDirectory(newPath);
            return CopyDirectory(ProjectPath,newPath);
                
        }
        private static bool CopyDirectory(string directoryFrom, string directoryTo)
        {
            if(!Directory.Exists(directoryFrom) || !Directory.Exists(directoryTo))
            {
                return false;
            }
            var CurrentDirectory = new DirectoryInfo(directoryFrom);
            foreach (var dir in CurrentDirectory.GetDirectories()){
                var newDir = Path.Combine(directoryTo, dir.Name);
                Directory.CreateDirectory(newDir);
                if (!CopyDirectory(dir.FullName, newDir))
                    return false;
            }
            foreach(var file in CurrentDirectory.GetFiles())
            {
                var newDir = Path.Combine(directoryTo, file.Name);
                File.Copy(file.FullName, newDir);
            }
            return true;
        }
    
        private static bool CheckProjectDirectory(string projectPath)
        {
            if (!Directory.Exists(projectPath))
                return false;
            DirectoryInfo directoryInfo = new DirectoryInfo(projectPath);
            var cpd = commonProjectsDirectory;
            try
            {
                commonProjectsDirectory = directoryInfo.Parent.FullName;
                Project project = new Project(directoryInfo.Name);
                try
                {
                    UnitDAL.GetUnits();
                    ProductDAL.GetProducts();
                    ResourceDAL.GetResources();
                }
                catch(Exception e)
                {
                    MessageBox.Show("Проект устарел и не может быть открыт");
                    return false;
                }
                return true;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                commonProjectsDirectory = cpd;
            }
            return false;
        }
    }
}
