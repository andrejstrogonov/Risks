using Riscks.Commands;
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
    public class UnitSequenceViewModel:ViewModel
    {
        public UnitSequence unitSequence { get; }
        public ObservableCollection<UnitViewModel> Units { get; set; }
        private UnitViewModel currentUnit;
        public UnitViewModel CurrentUnit
        {
            get
            {
                return currentUnit;
            }
            set
            {
                currentUnit = value;                
                OnPropertyChanged(nameof(CurrentUnit));
            }
        }
        
        public string Name
        {
            get
            {
                return unitSequence.Name;
            }
            set
            {
                unitSequence.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public bool IsEnabled_ToChange
        {
            get
            {
                return unitSequence.TypeOfUnit ==TypeOfUnit.Custom;
            }
        }

        public bool IsUsing
        {
            get
            {
                return unitSequence.IsUsing;
            }
            set
            {
                unitSequence.IsUsing = value;
                OnPropertyChanged(nameof(IsUsing));
            }
        }
        
        public UnitSequenceViewModel(UnitSequence unitSequence, IndexCommonViewModel commonViewModel)
        {
            this.IndexCommonViewModel = commonViewModel;
            this.unitSequence = unitSequence;
            Units = new ObservableCollection<UnitViewModel>();
            foreach(var u in UnitDAL.GetUnits(unitSequence.ID))
            {
                Units.Add(new UnitViewModel(u,this));
            }
            if (Units.Count() > 0)
            {
                CurrentUnit = Units.First();
            }

            AddUnitCommand = new AddUnitCommand(this);
            DeleteUnitCommand = new DeleteUnitCommand(this);
        }
        public IndexCommonViewModel IndexCommonViewModel { get; private set; }
        public UnitSequenceViewModel(IndexCommonViewModel commonViewModel)
        {
            IndexCommonViewModel = commonViewModel;
            unitSequence = new UnitSequence();
            unitSequence.TypeOfUnit = TypeOfUnit.Custom;
            Units = new ObservableCollection<UnitViewModel>();
            AddUnitCommand = new AddUnitCommand(this);
            DeleteUnitCommand = new DeleteUnitCommand(this);
            //Save();
        }

        public void ReloadDeleting()
        {
            foreach(var u in Units)
            {
                u.ReloadDeleting();
            }
        }
        public AddUnitCommand AddUnitCommand { get; set; }
        public DeleteUnitCommand DeleteUnitCommand { get; set; }
    
        public void ClearBasis(UnitViewModel unitViewModel)
        {
            if(Units.Any(p=>p.IsBasis==true))
                Units.First(p => p.IsBasis == true).IsBasis = false;
            unitViewModel.IsBasis = true; 
            foreach(var u in Units)
            {
                u.UpdateBasisSymbol();
            }
            OnPropertyChanged(nameof(Units));
        }

        public void AddUnit()
        {            
            UnitViewModel unitViewModel = new UnitViewModel(this);
            Units.Add(unitViewModel);
            if (!Units.Any(p => p.IsBasis))
            {
                unitViewModel.IsBasis = true;
            }
            //unitViewModel.Save();
            CurrentUnit = unitViewModel;
            //Save();
        }

        public void DeleteUnit()
        {
            if (CurrentUnit.CanDeleteUnit)
            {
                CurrentUnit.Delete();
                Units.Remove(CurrentUnit);
                CurrentUnit = Units.Count() > 0 ? Units.Last() : null;
                //Save();
            }
            else
            {
                MessageBox.Show("Невозможно удалить Е.И., т.к. она используется");
            }
        }

        public bool CanDeleteSequence()
        {
            if (!IsEnabled_ToChange)
            {
                return false;
            }
            foreach(var u in Units)
            {
                if (!u.CanDeleteUnit)
                    return false;
            }
            return true;
        }

        public void Delete()
        {
            if (CanDeleteSequence())
            {
                foreach(var u in Units)
                {
                    u.Delete();
                }
                UnitSequenceDAL.DeleteUnitSequence(unitSequence.ID);
            }
        }

        public void Save()
        {
            UnitSequenceDAL.SetUnitSequence(unitSequence);
            if (Units.Count() > 0)
            {
                Units.First(p => p.IsBasis).Save(true);
                foreach (var u in Units.Where(p => !p.IsBasis))
                {
                    u.Save(true);
                }
            }
        }

        public void ReloadInderface()
        {
            OnPropertyChanged(nameof(Units));
        }
    }

    public class IndexUnitViewModel:ViewModel
    {
        public ObservableCollection<UnitSequenceViewModel> UnitSequences { get; set; }

        private UnitSequenceViewModel currentSeq;
        public UnitSequenceViewModel CurrentSeq {
            get
            {
                return currentSeq;
            }
            set
            {
                currentSeq = value;
                if(!(currentSeq is null))
                currentSeq.ReloadInderface();
                OnPropertyChanged(nameof(CurrentSeq));
            }
        }

        private IndexCommonViewModel IndexCommonViewModel;
        public IndexUnitViewModel(IndexCommonViewModel indexCommonViewModel)
        {
            IndexCommonViewModel = indexCommonViewModel;
            UnitSequences = new ObservableCollection<UnitSequenceViewModel>();
            foreach(var U in UnitSequenceDAL.GetUnitSequences())
            {
                UnitSequences.Add(new UnitSequenceViewModel(U, indexCommonViewModel));
            }
            CurrentSeq = UnitSequences.Count() > 0 ? UnitSequences.First() : null;
            AddUnitSeqCommand = new AddUnitSeqCommand(this);
            DeleteUnitSeqCommand = new DeleteUnitSeqCommand(this);
            SaveIndexUnitCommand = new SaveIndexUnitCommand(this);
            CanselIndexUnitCommand = new CanselIndexUnitCommand(this);
        }

        public void ReloadDeleting()
        {
            foreach(var u in UnitSequences)
            {
                u.ReloadDeleting();
            }
        }

        public AddUnitSeqCommand AddUnitSeqCommand { get; set; }
        public DeleteUnitSeqCommand DeleteUnitSeqCommand { get; set; }

        public SaveIndexUnitCommand SaveIndexUnitCommand { get; set; }
        public CanselIndexUnitCommand CanselIndexUnitCommand { get; set; }

        public void AddUnitSeq()
        {
            UnitSequenceViewModel unitSequenceViewModel = new UnitSequenceViewModel(IndexCommonViewModel);
            UnitSequences.Add(unitSequenceViewModel);
            currentSeq = unitSequenceViewModel;
            OnPropertyChanged(nameof(UnitSequences));
            OnPropertyChanged(nameof(CurrentSeq));
        }

        public void DeleteUnitSeq()
        {
            if (CurrentSeq.CanDeleteSequence())
            {
                UnitSequences.Remove(CurrentSeq);
                CurrentSeq.Delete();
                CurrentSeq = UnitSequences.Count() > 0 ? UnitSequences.Last() : null;
                OnPropertyChanged(nameof(UnitSequences));
                OnPropertyChanged(nameof(CurrentSeq));
            }
        }

        public void Save()
        {
            foreach(var s in UnitSequences)
            {
                s.Save();
            }
            IndexCommonViewModel.ReloadUnits();
            if(!(OnSaved is null))
                OnSaved();
        }

        public delegate void Saved();
        public event Saved OnSaved;

        public void Cansel()
        {
            UnitSequences.Clear();
            foreach (var U in UnitSequenceDAL.GetUnitSequences())
            {
                UnitSequences.Add(new UnitSequenceViewModel(U, IndexCommonViewModel));
            }
            CurrentSeq = UnitSequences.Count() > 0 ? UnitSequences.First() : null;
            OnPropertyChanged(nameof(UnitSequences));            
        }
    }
}
