using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riscks.ViewModel
{
    public class BothFixedCostsViewModel : ViewModel
    {
        public IndexFixedCostViewModel IndexFixedCostViewModel_In { get; set; }
        public IndexFixedCostViewModel IndexFixedCostViewModel_Out { get; set; }

        public BothFixedCostsViewModel(CommonViewModel commonViewModel):base(commonViewModel)
        {
            IndexFixedCostViewModel_In = new IndexFixedCostViewModel(SQLiteLibrary.Model.TypeOfFixedCosts.incoming, CommonViewModel);
            IndexFixedCostViewModel_Out = new IndexFixedCostViewModel(SQLiteLibrary.Model.TypeOfFixedCosts.outcoming, CommonViewModel);
            OnPropertyChanged(nameof(IndexFixedCostViewModel_In));
            OnPropertyChanged(nameof(IndexFixedCostViewModel_Out));
            Saldo_Diagramm = new List<DiagramItem>();
            Saldo_Diagramm.Add(new DiagramItem(1, this));
            Saldo_Diagramm.Add(new DiagramItem(2, this));
            Saldo_Diagramm.Add(new DiagramItem(3, this));
            OnPropertyChanged(nameof(Saldo_Diagramm));
        }

        public void Reload()
        {
            IndexFixedCostViewModel_In.Reload();
            IndexFixedCostViewModel_Out.Reload();
            OnPropertyChanged(nameof(IndexFixedCostViewModel_In));
            OnPropertyChanged(nameof(IndexFixedCostViewModel_Out));
            Saldo_Diagramm.Clear();
            Saldo_Diagramm.Add(new DiagramItem(1, this));
            Saldo_Diagramm.Add(new DiagramItem(2, this));
            Saldo_Diagramm.Add(new DiagramItem(3, this));
            OnPropertyChanged(nameof(Saldo_Diagramm));
        }

        public void ReloadUnits()
        {
                IndexFixedCostViewModel_In.ReloadUnits();
                IndexFixedCostViewModel_Out.ReloadUnits();
        }

        public class DiagramItem : ViewModel
        {
            public int Name { get; set; }
            public double Value { get; set; }
            public BothFixedCostsViewModel BothFixedCostsViewModel { get; }
            public DiagramItem(int name, BothFixedCostsViewModel bothFixedCostsViewModel):base(null)
            {
                Name = name;
                BothFixedCostsViewModel = bothFixedCostsViewModel;
                BothFixedCostsViewModel.IndexFixedCostViewModel_In.PropertyChanged += IndexFixedCostViewModel_PropertyChanged;
                BothFixedCostsViewModel.IndexFixedCostViewModel_Out.PropertyChanged += IndexFixedCostViewModel_PropertyChanged;
                Reload();
            }

            private void IndexFixedCostViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                Reload();
            }

            public void Reload()
            {
                switch (Name)
                {
                    case 1:
                        {
                            Value = BothFixedCostsViewModel.IndexFixedCostViewModel_In.Summ;
                            break;
                        }
                    case 2:
                        {
                            Value = BothFixedCostsViewModel.IndexFixedCostViewModel_Out.Summ;
                            break;
                        }
                    case 3:
                        {
                            Value = BothFixedCostsViewModel.IndexFixedCostViewModel_In.Summ - BothFixedCostsViewModel.IndexFixedCostViewModel_Out.Summ;
                            break;
                        }
                }
                OnPropertyChanged(nameof(Value));
            }

        }
               
        public string AxisHeader
        {
            get
            {
                return "Сумма, "+ TransferOfUnitsDAL.GetCommonUnitForType(TypeOfUnit.Money).Symbol;
            }
        }
        public List<DiagramItem> Saldo_Diagramm { get; set; }

    }

}
