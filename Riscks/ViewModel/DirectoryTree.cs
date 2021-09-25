using ProjectManagerLibrary.WorkWithProjects;
using SQLiteLibrary;
using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace Riscks.ViewModel
{
    public class NodeProperty_Parent:ViewModel
    {
        public Settings Settings { get; set; }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (!value.Any(p => HierarhyElem.UnallowableChars.Contains(p)))
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
                else
                {
                    MessageBox.Show("Недопустимые символы");
                }
            }
        }
        public double Period
        {
            get
            {
                if (Settings is null)
                    return 0;
                return Settings.Period;
            }
            set
            {
                Settings.Period = value;
                OnPropertyChanged(nameof(Period));
            }
        }

        public List<string> PeriodUnits { get; set; }

        public string PeriodUnit
        {
            get
            {
                if (Settings is null)
                    return "";
                return UnitDAL.GetUnit(Settings.PeriodUnitID).Symbol;
            }
            set
            {
                var time = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Time).ID;
                Settings.PeriodUnitID = UnitDAL.GetUnits(time).Where(p => p.Symbol == value).FirstOrDefault().ID;
                OnPropertyChanged(nameof(PeriodUnit));
            }
        }

        public List<string> BudgetUnits { get; set; }

        public string BudgetUnit
        {
            get
            {
                if (Settings is null)
                    return "";
                return UnitDAL.GetUnit(Settings.BudgetUnitID).Symbol;
            }
            set
            {
                var lastval = Settings.BudgetUnitID;
                var money = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Money).ID;
                Settings.BudgetUnitID = UnitDAL.GetUnits(money).Where(p => p.Symbol == value).FirstOrDefault().ID;
                OnPropertyChanged(nameof(BudgetUnits));
                Logger(TypeOfHistoryItem.Settings, 0, "BudgetUnitID", lastval, value, " \n ", " \n ");
            }
        }

        public string DateOfCreation { get; set; }
        public string DateOfChange { get; set; }


        public NodeProperty_Parent(HierarhyElem hierarhyElem):base(null)
        {
            DateOfChange = hierarhyElem.DateOfChange.ToString();
            DateOfCreation = hierarhyElem.DateOfCreation.ToString();
            Name = hierarhyElem.Name;
            hierarhyElem.Activate();
            Settings = SettingsDAL.GetSettings();
            PeriodUnits = new List<string>();
            var money = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Money).ID;
            var time = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Time).ID;
            foreach (var p in UnitDAL.GetUnits(time))
            {
                PeriodUnits.Add(p.Symbol);
            }
            OnPropertyChanged(nameof(PeriodUnits));
            BudgetUnits = new List<string>();
            foreach (var p in UnitDAL.GetUnits(money))
            {
                BudgetUnits.Add(p.Symbol);
            }
            OnPropertyChanged(nameof(BudgetUnits));
        }

        public NodeProperty_Parent():base(null)
        {
        }
    }

    public class NodeProperty_Session : NodeProperty_Parent
    {
        public double Budget
        {
            get
            {
                return Settings.Budget;
            }
            set
            {
                var lastval = Settings.Budget;
                Settings.Budget = value;
                OnPropertyChanged(nameof(Budget));
                Logger(TypeOfHistoryItem.Settings, 0, "Budget", lastval, value, " \n ", " \n ");
            }
        }

        public string VAT_Type
        {
            get
            {
                return FromVATToStr[Settings.VAT_type];
            }
            set
            {
                var last = Settings.VAT_type;
                Settings.VAT_type = FromStrToVAT[value];
                OnPropertyChanged(nameof(VAT_Type));
                if ((last == VATType.full || Settings.VAT_type == VATType.full) && !(OnVATTypeChanged is null))
                    OnVATTypeChanged();
            }
        }

        public delegate void VATTypeChanged();
        public event VATTypeChanged OnVATTypeChanged;

        public List<string> VAT_Types { get; set; }
        private Dictionary<string, VATType> FromStrToVAT;
        private Dictionary<VATType, string> FromVATToStr;

        public void Init()
        {
            VAT_Types = new List<string>();
            FromStrToVAT = new Dictionary<string, VATType>();
            FromVATToStr = new Dictionary<VATType, string>();
            VAT_Types.Add("УСН 6%");
            VAT_Types.Add("УСН 15%");
            VAT_Types.Add("ОСН");
            VAT_Types.Add("Без налогообложения");
            FromStrToVAT[VAT_Types[0]] = VATType.main_6;
            FromStrToVAT[VAT_Types[1]] = VATType.main_15;
            FromStrToVAT[VAT_Types[2]] = VATType.full;
            FromStrToVAT[VAT_Types[3]] = VATType.None;
            FromVATToStr[VATType.main_6] = "УСН 6%";
            FromVATToStr[VATType.main_15] = "УСН 15%";
            FromVATToStr[VATType.full] = "ОСН";
            FromVATToStr[VATType.None] = "Без налогообложения";

        }

        public NodeProperty_Session(Session session):base()
        {
            Name = session.Name;
            session.Activate();
            Init();
            Settings = SettingsDAL.GetSettings();
            PeriodUnits = new List<string>();
            var money = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Money).ID;
            var time = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Time).ID;
            foreach (var p in UnitDAL.GetUnits(time))
            {
                PeriodUnits.Add(p.Symbol);
            }
            OnPropertyChanged(nameof(PeriodUnits));
            BudgetUnits = new List<string>();
            foreach (var p in UnitDAL.GetUnits(money))
            {
                BudgetUnits.Add(p.Symbol);
            }
            OnPropertyChanged(nameof(BudgetUnits));
        }

    }

    public class DirectoryNode: ViewModel
    {
        public int Level { get; set; }
        public NodeProperty_Parent Property { get; set; }

        public HierarhyElem HierarhyElem { get; set; }
        private bool isChecked;
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }
        private bool isexpanded;
        public bool IsExpanded_Prop { get
            {
                return isexpanded;
            }
            set
            {
                isexpanded = value;
                OnPropertyChanged(nameof(IsExpanded_Prop));
            }
        }
        public DateTime DateOfChange { get; set; }
        public DateTime DateOfCreation { get; set; }

        private string name;

        public string Name { get
            {
                return name;
            }
            set
            {
                if (!value.Any(p => HierarhyElem.UnallowableChars.Contains(p))){
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public string NameOfElem { get; set; }

        public List<DirectoryNode> Nodes { get; set; }
        public DirectoryNode CurrentNode { get; set; }

        public DirectoryNode(string Name):base(null)
        {
            this.Name = Name;
            IsChecked = false;
            IsExpanded_Prop = true;
            Nodes = null;
            CurrentNode = null;
        }

        public void Select()
        {
            IsChecked = true;
            IsExpanded_Prop = true;
            HierarhyElem.Activate();
        }
        public void Unselect(bool needsave=false){
            IsChecked = false;
            IsExpanded_Prop = true;
            if(needsave)
            HierarhyElem.SaveChanges();
        }

        public void SelectNode(DirectoryNode directoryNode)
        {
            if (!Nodes.Contains(directoryNode))
            {
                return;
            }
            if(CurrentNode is null)
            {
                CurrentNode = directoryNode;
                CurrentNode.Select();
            }
            else
            {
                CurrentNode.Unselect();
                CurrentNode = directoryNode;
                CurrentNode.Select();
            }
        }

        public void Expand_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsExpanded_Prop))
            {
                OnPropertyChanged(nameof(Margin));
            }
        }

        public void Prop_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Name":
                    {
                        if ((string)Property.Name != HierarhyElem.Name)
                        {
                            HierarhyElem.Rename((string)Property.Name);
                            Name = HierarhyElem.Name;
                            OnPropertyChanged(nameof(Name));
                        }
                        break;
                    }
                default:
                    {
                        HierarhyElem.Activate();
                        SettingsDAL.SaveSettings(Property.Settings);
                        break;
                    }
            }
        }


        public Thickness Margin
        {
            get
            {
                if (this is Node_Project)
                {
                    Thickness thickness = new Thickness();
                    if (Nodes.Count() == 0)
                        thickness.Bottom= 12;
                    else
                    if (Nodes.Last() is Node_Session)
                        thickness.Bottom = 12;
                    else
                    if (Nodes.Last().Nodes.Count() == 0)
                        thickness.Bottom = 12;
                    else
                    thickness.Bottom=12+ 26*(Nodes.Last().IsExpanded_Prop ? Nodes.Last().Nodes.Count() : 0);
                    return thickness;
                }
                return new Thickness(0,13,0,12);
            }
        }

        public bool CanDeleteUnit(Unit unit)
        {
            HierarhyElem.Activate();
            bool start = UnitDAL.CanDeleteUnit(unit);
            if (!start)
            {
                return false;
            }
            if(!(Nodes is null))
            foreach(var c in Nodes)
            {
                if (!c.CanDeleteUnit(unit))
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class Node_Project : DirectoryNode
    {
        public Node_Project(string Name):base(Name)
        {
            NameOfElem = "Проект:";
            Level = 0;
            HierarhyElem = new Project(Name);
        }

        public void DeleteProject()
        {
            HierarhyElem.Delete();
        }

        public Node_Project(string Name, List<DirectoryNode> nodes) : base(Name)
        {
            NameOfElem = "Проект:";
            Level = 0;
            this.Nodes = nodes;
            //HierarhyElem = new Project(Name);
        }
        public void LoadProjectHierarhy()
        {
            //HierarhyElem = new Project(Name);
            Property = new NodeProperty_Parent(HierarhyElem);
            Property.PropertyChanged += Prop_PropertyChanged;            
        }

        public Node_Project(Project project):base(project.Name)
        {
            NameOfElem = "Проект:";
            Level = 0;
            HierarhyElem = project;
            Property = new NodeProperty_Parent(project);
            Property.PropertyChanged += Prop_PropertyChanged;
        }
                
    }

    public class Node_Section : DirectoryNode
    {
        public Node_Section(Section section) : base(section.Name)
        {
            NameOfElem = "Раздел:";
            Level = 1;
            HierarhyElem = section;
            Property = new NodeProperty_Parent(section);
            Property.PropertyChanged += Prop_PropertyChanged;
        }

        public Node_Section(string Name) : base(Name)
        {
            NameOfElem = "Раздел:";
            Level = 1;
            //HierarhyElem = null;
            //Property = new NodeProperty_Parent(section);
            //Property.PropertyChanged += Prop_PropertyChanged;
        }
    }



    public class Node_Session : DirectoryNode
    {
        public Node_Session(Session session):base(session.Name)
        {
            NameOfElem = "Расчет:";
            Level = 2;
            HierarhyElem = session;
            Property = new NodeProperty_Session(session);
            Property.PropertyChanged += Prop_PropertyChanged;
        }

        public Node_Session(string Name) : base(Name)
        {
            NameOfElem = "Расчет:";
            Level = 2;
            //HierarhyElem = session;
            //Property = new NodeProperty_Session(session);
            //Property.PropertyChanged += Prop_PropertyChanged;
        }
    }

    public class DirectoryTree:ViewModel
    {
        private List<DirectoryNode> RootNodes { get; set; }

        public Node_Project SelectedProject { get; set; }

        public bool CanDeleteUnit(Unit unit)
        {
            var res =  SelectedProject.CanDeleteUnit(unit);
            LatestNode.Select();
            return res;
        }
        
        public DirectoryNode LatestNode { get; set; }

        public DirectoryTree()
        {
                Init();
        }

        public void RefreshHierarhy()
        {
            LoadSelectedProject();
        }

        public void SelectProject(Node_Project node_Project)
        {
            SelectedProject = node_Project;
            LoadSelectedProject();
        }

        public List<DirectoryNode> Projects
        {
            get
            {
                return RootNodes;
            }
        }

        public List<DirectoryNode> NodesTree
        {
            get
            {
                List<DirectoryNode> directoryNodes = new List<DirectoryNode>();
                directoryNodes.Add(SelectedProject);
                return directoryNodes;
            }
        }

        private bool Init()
        {
            OpenCommonDirectory();
            LoadProjects();
            return true;
        }

        private void OpenCommonDirectory()
        {
            HierarhyElem.OpenCommonProjectDirectory();
        }
        
        private void LoadSelectedProject()
        {
            SelectedProject.LoadProjectHierarhy();
            DateTime lastHigh = new DateTime(1900, 1, 1);
            SelectedProject.Nodes = new List<DirectoryNode>();
            foreach (var hi in SelectedProject.HierarhyElem.HierarhyElems)
            {
                DirectoryInfo di = new DirectoryInfo(hi.Value.DirectoryPath);
                DirectoryNode directoryNode;
                if(hi.Value is Session)
                {
                    directoryNode = new Node_Session(hi.Value as Session);
                }
                else
                {
                    directoryNode = new Node_Section(hi.Value as Section);
                    directoryNode.PropertyChanged += SelectedProject.Expand_PropertyChanged;
                    directoryNode.Nodes = new List<DirectoryNode>();
                    foreach(var hi2 in hi.Value.HierarhyElems)
                    {
                        DirectoryNode ses_node = new Node_Session(hi2.Value as Session);
                        DirectoryInfo di2 = new DirectoryInfo(hi2.Value.DirectoryPath);
                        ses_node.DateOfChange = di2.LastWriteTime;
                        ses_node.IsChecked = false;
                        ses_node.IsExpanded_Prop = true;
                        directoryNode.Nodes.Add(ses_node);
                    }
                    directoryNode.Nodes.Sort((p1, p2) =>
                    {
                        return p2.DateOfChange > p1.DateOfChange ? 1 : -1;
                    });
                }

                directoryNode.DateOfChange = di.LastWriteTime;
                directoryNode.IsChecked = false;
                directoryNode.IsExpanded_Prop = true;
                SelectedProject.Nodes.Add(directoryNode);
                
            }
            SelectedProject.Nodes.Sort((p1, p2) =>
            {
                return p2.DateOfChange > p1.DateOfChange ? 1 : -1;
            });
            if (SelectedProject.Nodes.Count() > 0)
            {
                if(SelectedProject.Nodes[0] is Node_Session)
                {
                    LatestNode = SelectedProject.Nodes[0];
                }
                else
                {
                    if (SelectedProject.Nodes[0].Nodes.Count() > 0)
                    {
                        LatestNode = SelectedProject.Nodes[0].Nodes[0];
                    }
                    else
                    {
                        LatestNode = SelectedProject.Nodes[0];
                    }
                }
            }
            else
            {
                LatestNode = SelectedProject;
            }
            LatestNode.Select();
            OnPropertyChanged(nameof(SelectedProject));
            OnPropertyChanged(nameof(NodesTree));

        }

       

        private void LoadProjects()
        {
            RootNodes = new List<DirectoryNode>();
            foreach (string subdir in Directory.GetDirectories(Project.commonProjectsDirectory))
            {
                DirectoryInfo fi1 = new DirectoryInfo(subdir);
                Node_Project project = new Node_Project(fi1.Name);
                project.DateOfChange = project.HierarhyElem.DateOfChange;
                project.DateOfCreation = project.HierarhyElem.DateOfCreation;
                project.IsChecked = false;
                project.IsExpanded_Prop = true;
                RootNodes.Add(project);
            }
            RootNodes.Sort((p1,p2) => {
                if (p1.DateOfChange == p2.DateOfChange)
                    return 0;
                return p1.DateOfChange > p2.DateOfChange?-1:1;
            });
            OnPropertyChanged(nameof(RootNodes));
        }
        
        public void CreateProject(string name)
        {
            Project project = new Project(name);
            SelectedProject = new Node_Project(project);
            RootNodes.Add(SelectedProject);
        }

    }
}
