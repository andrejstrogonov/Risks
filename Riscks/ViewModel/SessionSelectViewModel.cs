using DataProcessLibrary;
using DataProcessLibrary.CommonAnalys;
using DataProcessLibrary.InitialData;
using ProjectManagerLibrary.WorkWithProjects;
using Riscks.Commands;
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
    public class HierarhyElemIsChecked:ViewModel
    {
        public bool IsLast { get; set; }
        public HierarhyElem_Analys HierarhyElem { get; }

        public Visibility ChildrenVisibility { get; }

        public HierarhyElemIsChecked(HierarhyElem_Analys hierarhyElem)
        {
            IsLast = false;
            HierarhyElem = hierarhyElem;
            Name = hierarhyElem.HierarhyElem.Name;
            hierarhyElem.HierarhyElem.Activate();
            Settings settings = SettingsDAL.GetSettings();
            Period = settings.Period;
            UnitPeriod = UnitDAL.GetUnit(settings.PeriodUnitID);
            Amount = 0;
            
            Children = new ObservableCollection<HierarhyElemIsChecked>();
            foreach (var s in hierarhyElem.Children)
            {
                Children.Add(new HierarhyElemIsChecked(s));
            }
            ChildrenVisibility = Children.Count() > 0 ? Visibility.Visible : Visibility.Collapsed;

            var commonunit = hierarhyElem.HierarhyElem is Session || Children.Count()==0? UnitPeriod: HierarhyElemIsChecked.GetCommonUnit(Children.Select(p=>p.UnitPeriod).ToList());
            foreach (var s in Children)
            {
                s.TransferToCommonUnit(commonunit);
                s.PropertyChanged += SummPeriod_PropertyChanged;
            }
            if(Children.Count>0)
                Children.Last().IsLast = true;
            Period = Period * TransferOfUnitsDAL.GetTransferOfUnits(UnitPeriod, commonunit);
            UnitPeriod = commonunit;
            OnPropertyChanged(nameof(SessionPeriod));
                       
            OnPropertyChanged(nameof(Children));

        }


        private void SummPeriod_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SummOfChild));
            OnPropertyChanged(nameof(SummOfChild_Str));
        }

        public static Unit GetCommonUnit(List<Unit> units)
        {
            return TransferOfUnitsDAL.GetCommonUnitForType(units.First().UnitSequence_ID);
        }

        public string Name
        {
            get; set;
        }

        public double Period { get; set; }
        private Unit UnitPeriod { get; set; }

        public string SessionPeriod {
            get
            {
                return Period.ToString() + " " + UnitPeriod.Symbol;
            }
        }

        private int amount;
        public int Amount
        {
            get
            {
                return amount;
            }
            set
            {
                amount = value;
                OnPropertyChanged(nameof(Amount));
            }
        }
        
        public HierarhyElem_Analys LoadChildren_Analys()
        {
            HierarhyElem.Children_Analys = new List<HierarhyElem_Analys>();
            foreach(var c in Children)
            {
                HierarhyElem_Analys analys = c.LoadChildren_Analys();
                for (int i = 1; i <= c.Amount; i++)
                {
                    var copy = analys.Copy();
                    copy.SetName(i);
                    HierarhyElem.Children_Analys.Add(copy);
                }
            }
            return HierarhyElem;
        }

        public void TransferToCommonUnit(Unit unit)
        {
            Period = Period * TransferOfUnitsDAL.GetTransferOfUnits(UnitPeriod, unit);
            UnitPeriod = unit;
            OnPropertyChanged(nameof(SessionPeriod));
        }


        private HierarhyElemIsChecked currentItem;
        public HierarhyElemIsChecked CurrentItem
        {
            get
            {
                return currentItem;
            }
            set
            {
                currentItem = value;
                OnPropertyChanged(nameof(CurrentItem));
            }
        }

        public ObservableCollection<HierarhyElemIsChecked> Children { get; set; }

        public double SummOfChild
        {
            get
            {
                double sum = 0;
                foreach(var c in Children)
                {
                    sum += c.Amount * c.Period;
                }
                return sum;
            }
        }

        public string SummOfChild_Str
        {
            get
            {
                return SummOfChild.ToString() + ' ' + UnitPeriod.Symbol;
            }
        }
        
        public bool Posible
        {
            get
            {
                return (SummOfChild>0 || Children.Count()==0 ) && SummOfChild <= Period && (Children.Count(p=>!p.Posible)==0);
            }
        }

        public SerializeHierarhy GetSerializeHierarhy()
        {
            SerializeHierarhy serializeHierarhy = new SerializeHierarhy() { Name = Name, Amount=Amount };
            serializeHierarhy.children = new List<SerializeHierarhy>();
            foreach(var c in Children)
            {
                serializeHierarhy.children.Add(c.GetSerializeHierarhy());
            }
            return serializeHierarhy;
        }

        public void LoadFromSerializeHierarhy(SerializeHierarhy serializeHierarhy)
        {
            if (serializeHierarhy is null)
                return;
            this.Amount = serializeHierarhy.Amount;
            foreach (var c in serializeHierarhy.children)
            {
                if (Children.Any(p => p.Name == c.Name))
                {
                    var equal = Children.Where(p => p.Name == c.Name).First();
                    equal.LoadFromSerializeHierarhy(c);
                }
            }
        }
    }

    public class HierarhySelectViewModel:ViewModel
    {
        public HierarhyElemIsChecked HierarhyElemIsChecked { get; set; }

        private HierarhyElem HierarhyElem;
        private HierarhyElem_Analys HierarhyElem_Analys;

        public HierarhySelectViewModel(HierarhyElem_Analys parentElem)
        {
            HierarhyElem_Analys = parentElem;
            if (!(parentElem is null))
            {
                HierarhyElemIsChecked = new HierarhyElemIsChecked(HierarhyElem_Analys);
                HierarhyElemIsChecked.LoadFromSerializeHierarhy(HierarhyElem.AnalysSettingsSetuper.Get().SerializeHierarhy);
                OnPropertyChanged(nameof(HierarhyElemIsChecked));
            }
            SaveCommonAnalys = new SaveCommonAnalysSettings(this);
            BackCommonAnalys = new BackCommonAnalysSettings(this);
        }

        public HierarhySelectViewModel(HierarhyElem parentHierarhiElem)
        {
            HierarhyElem = parentHierarhiElem;
            HierarhyElem_Analys = InitialData.GetAnalys(parentHierarhiElem);
            if (!(HierarhyElem_Analys is null))
            {
                HierarhyElemIsChecked = new HierarhyElemIsChecked(HierarhyElem_Analys);
                HierarhyElemIsChecked.LoadFromSerializeHierarhy(HierarhyElem.AnalysSettingsSetuper.Get().SerializeHierarhy);
                OnPropertyChanged(nameof(HierarhyElemIsChecked));
            }
            SaveCommonAnalys = new SaveCommonAnalysSettings(this);
            BackCommonAnalys = new BackCommonAnalysSettings(this);
        }

        public SaveCommonAnalysSettings SaveCommonAnalys { get; set; }

        public BackCommonAnalysSettings BackCommonAnalys { get; set; }

        public void GetHierarhyItem()
        {
            InitialData.HierarhyElem_Analys= HierarhyElemIsChecked.LoadChildren_Analys();
        }

        public SerializeHierarhy GetSerializeHierarhy()
        {
            return HierarhyElemIsChecked.GetSerializeHierarhy();
        }

        public void Save()
        {
            HierarhyElem.AnalysSettingsSetuper.Get().SerializeHierarhy = GetSerializeHierarhy();
            HierarhyElem.AnalysSettingsSetuper.Save();
            InitialData.ReloadCommonAnalysSettings(HierarhyElem.GetProject());
        }

        public void Back()
        {
            HierarhyElemIsChecked = new HierarhyElemIsChecked(HierarhyElem_Analys);
            HierarhyElemIsChecked.LoadFromSerializeHierarhy(HierarhyElem.AnalysSettingsSetuper.GetFromFile().SerializeHierarhy);
            OnPropertyChanged(nameof(HierarhyElemIsChecked));
        }
    }


}
