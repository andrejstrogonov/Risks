
using ProjectManagerLibrary.WorkWithProjects;
using SQLiteLibrary;
using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Riscks.ViewModel
{
    public class SessionDataViewModel : ViewModel
    {
        public string Title { get; set; }
        public string Title_Input { get; set; }
        public string Name { get; set; }
        public List<string> VAT_Types { get; set; }
        public Settings Settings { get; set; }
        private Dictionary<string, VATType> FromStrToVAT;
        private Dictionary<VATType, string> FromVATToStr;

        public Visibility Visibility { get; set; }

        public Dictionary<string,DirectoryNode> StrToNodes { get; set; }

        public ObservableCollection<string> Nodes { get; set; }

        public DirectoryNode selectedNode;
        public string SelectedNode
        {
            get
            {
                return selectedNode.Name;
            }
            set
            {
                selectedNode = StrToNodes[value];
                OnPropertyChanged(nameof(SelectedNode));
            }
        }


        public double Period
        {
            get
            {
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
                return UnitDAL.GetUnit( Settings.PeriodUnitID).Symbol;
            }
            set
            {
                Settings.PeriodUnit = UnitDAL.GetUnits(UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Time).ID).Where(p => p.Symbol == value).FirstOrDefault();
                OnPropertyChanged(nameof(PeriodUnit));
            }
        }

        public bool[] FieldsToCopy;

        /*
         0 - resources names
         1 - resources parameters
         2 - products names
         3 - products parameters
         4 - products composition
         5 - products risks

             */

        public void Init(DirectoryTree directoryTree)
        {
            Name = "Новый расчет";
            FieldsToCopy = new bool[6];
            Settings = SettingsDAL.GetSettings();
            VAT_Types = new List<string>();
            FromStrToVAT = new Dictionary<string, VATType>();
            FromVATToStr = new Dictionary<VATType, string>();
            VAT_Types.Add("УСН 6%");
            VAT_Types.Add("УСН 15%");
            VAT_Types.Add("ПСН");
            FromStrToVAT[VAT_Types[0]] = VATType.main_6;
            FromStrToVAT[VAT_Types[1]] = VATType.main_15;
            FromStrToVAT[VAT_Types[2]] = VATType.full;
            FromVATToStr[VATType.main_6] = "УСН 6%";
            FromVATToStr[VATType.main_15] = "УСН 15%";
            FromVATToStr[VATType.full] = "ПСН";
            PeriodUnits = new List<string>();
            foreach (var p in UnitDAL.GetUnits(UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Time).ID))
            {
                PeriodUnits.Add(p.Symbol);
            }
            OnPropertyChanged(nameof(PeriodUnits));


            Nodes = new ObservableCollection<string>();
            StrToNodes = new Dictionary<string, DirectoryNode>();
            foreach(var n in directoryTree.SelectedProject.Nodes.Where(p=>p is Node_Section))
            {
                Nodes.Add(n.Name);
                StrToNodes[n.Name] = n;
            }
            Nodes.Add(directoryTree.SelectedProject.Name);
            StrToNodes[directoryTree.SelectedProject.Name] = directoryTree.SelectedProject;
            SelectedNode = Nodes.First();
        }
        public SessionDataViewModel(DirectoryTree directoryTree)
        {
            Init(directoryTree);
        }

        public string VAT_Type
        {
            get
            {
                return FromVATToStr[Settings.VAT_type];
            }
            set
            {
                Settings.VAT_type = FromStrToVAT[value];
                OnPropertyChanged(nameof(VAT_Type));
            }
        }

        public double Budget
        {
            get
            {
                return Settings.Budget;
            }
            set
            {
                Settings.Budget = value;
                OnPropertyChanged(nameof(Budget));
            }
        }

        public bool ResourceNames
        {
            get
            {
                return FieldsToCopy[0];
            }
            set
            {
                FieldsToCopy[0] = value;
                if (!value)
                {
                    ResourceParams = false;
                    ResourceOfProducts = false;
                }
                OnPropertyChanged(nameof(ResourceNames));
            }
        }

        public bool ResourceParams
        {
            get
            {
                return FieldsToCopy[1];
            }
            set
            {
                FieldsToCopy[1] = value;
                if (value)
                    ResourceNames = true;
                OnPropertyChanged(nameof(ResourceParams));
            }
        }

        public bool ProductsNames
        {
            get
            {
                return FieldsToCopy[2];
            }
            set
            {
                FieldsToCopy[2] = value;
                if (!value)
                {
                    RiskOfProducts = false;
                    ResourceOfProducts = false;
                    ProductsParams = false;
                }
                OnPropertyChanged(nameof(ProductsNames));
            }
        }

        public bool ProductsParams
        {
            get
            {
                return FieldsToCopy[3];
            }
            set
            {
                FieldsToCopy[3] = value;
                if (value)
                    ProductsNames = true;
                OnPropertyChanged(nameof(ProductsParams));
            }
        }

        public bool ResourceOfProducts
        {
            get
            {
                return FieldsToCopy[4];
            }
            set
            {
                FieldsToCopy[4] = value;
                if (value)
                {
                    ProductsNames = true;
                    ResourceNames = true;
                }
                OnPropertyChanged(nameof(ResourceOfProducts));
            }
        }

        public bool RiskOfProducts
        {
            get
            {
                return FieldsToCopy[5];
            }
            set
            {
                FieldsToCopy[5] = value;
                if (value)
                    ProductsNames = true;
                OnPropertyChanged(nameof(RiskOfProducts));
            }
        }
    }
}
