using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riscks.Commands;
using System.Windows;
using DataProcessLibrary.InitialData;

namespace Riscks.ViewModel
{
    public class IndexFixedCostViewModel:ViewModel
    {

        public ObservableCollection<FixedCostViewModel> FixedCosts { get; set; }

        public static bool IsFullVAT = IndexCommonViewModel.IsFullVAT;

        public Visibility isVisible { get
            {
                return InitialData.Settings.VAT_type == VATType.full ? Visibility.Visible : Visibility.Collapsed;
            } 
        }

        private FixedCostViewModel currentFixedCost;
        public FixedCostViewModel CurrentFixedCost
        {
            get
            {
                return currentFixedCost;
            }
            set
            {
                currentFixedCost = value;
                if (!(value is null))
                    currentFixedCost.Save();
                OnPropertyChanged(nameof(CurrentFixedCost));
            }
        }

        private TypeOfFixedCosts TypeOfFixedCosts;

        public IndexFixedCostViewModel(TypeOfFixedCosts typeOfFixedCosts, CommonViewModel commonViewModel):base(commonViewModel)
        {
            TypeOfFixedCosts = typeOfFixedCosts;
            FixedCosts = new ObservableCollection<FixedCostViewModel>();
            foreach(var fc in FixedCostDAL.GetFixedCosts(typeOfFixedCosts))
            {
                var fvvm = new FixedCostViewModel(fc, CommonViewModel);
                fvvm.PropertyChanged += Fvvm_PropertyChanged;
                FixedCosts.Add(fvvm);
            }
            if (FixedCosts.Count() > 0)
                currentFixedCost = FixedCosts.First();

            AddFixedCostCommand = new AddFixedCostCommand(this);
            DeleteFixedCostCommand = new DeleteFixedCostCommand(this);
            RecountSumm();
            OnPropertyChanged(nameof(FixedCosts));
            OnPropertyChanged(nameof(CurrentFixedCost));

            diagramm = new ObservableCollection<DiagramItem>();
            foreach(var fc in FixedCosts)
            {
                diagramm.Add(new DiagramItem(fc));
            }
        }

        public void Reload()
        {
            foreach(var f in FixedCosts)
            {
                f.PropertyChanged -= Fvvm_PropertyChanged;
            }
            FixedCosts.Clear();
            foreach (var fc in FixedCostDAL.GetFixedCosts(TypeOfFixedCosts))
            {
                var fvvm = new FixedCostViewModel(fc, CommonViewModel);
                fvvm.PropertyChanged += Fvvm_PropertyChanged;
                FixedCosts.Add(fvvm);
            }
            if (FixedCosts.Count() > 0)
                currentFixedCost = FixedCosts.First();
            OnPropertyChanged(nameof(isVisible));
            RecountSumm();

            diagramm.Clear();
            foreach (var fc in FixedCosts)
            {
                diagramm.Add(new DiagramItem(fc));
            }
        }

        private void Fvvm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName!=nameof(currentFixedCost.Name))
                RecountSumm();
        }

        public AddFixedCostCommand AddFixedCostCommand { get; set; }
        public DeleteFixedCostCommand DeleteFixedCostCommand { get; set; }

        private void RecountSumm()
        {
            double summ = 0;
            foreach (var fv in FixedCosts)
            {
                summ += fv.PriceInCommonUnit;
            }
            Summ = summ;
            OnPropertyChanged(nameof(Summ));
        }
        public double Summ { get; private set; }

        public string Unit
        {
            get
            {
                var money = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Money);
                return TransferOfUnitsDAL.GetCommonUnitForType(money.ID).Symbol;
            }
        }

        public void ReloadUnits()
        {
                foreach (var fc in FixedCosts)
            {
                fc.ReloadUnits();
            }
        }


        public class DiagramItem:ViewModel
        {
            public string Name { get
                {
                    return FixedCostViewModel.Name;
                }
            }
            public double Value
            {
                get
                {
                    return FixedCostViewModel.PriceInCommonUnit;
                }
            }
            public FixedCostViewModel FixedCostViewModel { get; }
            public DiagramItem(FixedCostViewModel fixedCostViewModel)
            {
                this.FixedCostViewModel = fixedCostViewModel;
                FixedCostViewModel.PropertyChanged += FixedCostViewModel_PropertyChanged;
            }

            private void FixedCostViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Value));
            }

        }

        public ObservableCollection<DiagramItem> diagramm { get; set; }

        public void AddFixedCost()
        {
            FixedCostViewModel fixedCostViewModel  = new FixedCostViewModel(TypeOfFixedCosts, CommonViewModel);
            fixedCostViewModel.PropertyChanged += Fvvm_PropertyChanged;
            FixedCosts.Add(fixedCostViewModel);
            CurrentFixedCost = fixedCostViewModel;
            RecountSumm();
            OnPropertyChanged(nameof(FixedCosts));

            diagramm.Add(new DiagramItem(fixedCostViewModel));
        }

        public void DeleteFixedCost()
        {
            if (!(CurrentFixedCost is null))
            {
                diagramm.Remove(diagramm.Where(d => d.FixedCostViewModel == CurrentFixedCost).First());

                currentFixedCost.Delete();
                var copy = currentFixedCost;
                FixedCosts.Remove(copy);
                RecountSumm();
                OnPropertyChanged(nameof(FixedCosts));
                //IndexGroupViewModel.ReloadProducts();
            }
        }
    }
}
