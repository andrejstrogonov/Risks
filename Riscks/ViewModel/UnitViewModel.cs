using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riscks.ViewModel
{
    public class UnitViewModel:ViewModel
    {
        private Unit Unit;
        private UnitSequenceViewModel UnitSequenceViewModel;

        private IndexCommonViewModel IndexCommonViewModel;
        private bool canDelete = true;
        public UnitViewModel(Unit unit, UnitSequenceViewModel unitSequenceViewModel) 
        {
            this.IndexCommonViewModel = unitSequenceViewModel.IndexCommonViewModel;            
            Unit = unit;
            canDelete = IndexCommonViewModel.CanDeleteUnit(Unit);
            UnitSequenceViewModel = unitSequenceViewModel;
            Number = TransferOfUnitsDAL.GetTransferToCommonUnit(unit.ID);
            
        }

        public void ReloadDeleting()
        {
            canDelete = IndexCommonViewModel.CanDeleteUnit(Unit);
            OnPropertyChanged(nameof(CanDeleteUnit));
        }

        public UnitViewModel(UnitSequenceViewModel unitSequenceViewModel)
        {
            this.IndexCommonViewModel = unitSequenceViewModel.IndexCommonViewModel;
            Unit = new Unit();
            Unit.Name = "";
            Unit.IsBasis = false;
            Unit.Symbol = "";            
            Unit.UnitSequence_ID = unitSequenceViewModel.unitSequence.ID;
            UnitSequenceViewModel = unitSequenceViewModel;
            Number = 1;
        }

        public bool IsEnabled
        {
            get
            {
                if (UnitSequenceViewModel.IsEnabled_ToChange)
                    return true;
                if (UnitSequenceViewModel.Units.Count() == 1)
                    return false;
                return true;
            }
        }

        public string FullName
        {
            get
            {
                return Unit.Name;
            }
            set
            {
                Unit.Name = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string Symbol
        {
            get
            {
                return Unit.Symbol;
            }
            set
            {
                Unit.Symbol = value;
                OnPropertyChanged(nameof(Symbol));
            }
        }

        public bool IsBasis
        {
            get
            {
                return Unit.IsBasis;
            }
            set
            {
                if (value && IsBasis==false && UnitSequenceViewModel.Units.Any(p=>p.IsBasis))
                {
                    UnitSequenceViewModel.ClearBasis(this);
                }
                Unit.IsBasis = value;
                OnPropertyChanged(nameof(IsBasis));
            }
        }

        public void UpdateBasisSymbol()
        {
            OnPropertyChanged(nameof(BasisSymbol));
            OnPropertyChanged(nameof(Number));
        }

        private double number;
        public double Number
        {
            get
            {
                if (IsBasis)
                    number = 1;
                return number;
            }
            set
            {
                if(!IsBasis)
                    number = value;
                else
                {
                    number = 1;
                }
                OnPropertyChanged(nameof(Number));
            }
        }

        public string BasisSymbol
        {
            get
            {
                return UnitSequenceViewModel.Units.First(p => p.IsBasis == true).Symbol;
            }
        }
        
        public void Save(bool needSaveTransfers=false)
        {
            Unit.UnitSequence_ID = UnitSequenceViewModel.unitSequence.ID;
            UnitDAL.AddUnit(Unit);
            if (needSaveTransfers)
            {
                TransferOfUnits transferOfUnits = new TransferOfUnits();
                transferOfUnits.ID_UnitFrom = Unit.ID;
                if (IsBasis)
                    transferOfUnits.ID_UnitTo = Unit.ID;
                else
                {
                    transferOfUnits.ID_UnitTo = TransferOfUnitsDAL.GetCommonUnitForType(Unit.UnitSequence_ID).ID;
                }
                transferOfUnits.Coefficient = Number;
                TransferOfUnitsDAL.AddTransferOfUnits(transferOfUnits);
            }
        }

       

        public bool Delete()
        {
            if (!canDelete)
            {
                return false;
            }
            DeleteTransfers();
            return true;
        }

        public bool CanDeleteUnit
        {
            get
            {
                return canDelete;
            }
        }

        public void DeleteTransfers()
        {
            TransferOfUnitsDAL.DeleteAllTransfersOfUnit(Unit);
            UnitDAL.DeleteUnit(Unit.ID);
        }
    }
}
