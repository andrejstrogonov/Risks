using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riscks.ViewModel
{
    public class TransferOfUnitsViewModel:ViewModel
    {
        private TransferOfUnits transferOfUnits;
        public TransferOfUnitsViewModel(TransferOfUnits transferOfUnits)
        {
            this.transferOfUnits = transferOfUnits;
        }

        public string UnitToName{
            get
            {
                return UnitDAL.GetUnit(transferOfUnits.ID_UnitTo).Name;
            }
            private set { }
        }

        public double Coefficient
        {
            get
            {
                return transferOfUnits.Coefficient;
            }
            set
            {
                transferOfUnits.Coefficient = value;
                OnPropertyChanged("Coefficient");
            }
        }

        public void Save()
        {
            TransferOfUnitsDAL.AddTransferOfUnits(transferOfUnits);
        }

        public bool Delete()
        {
            return TransferOfUnitsDAL.DeleteTransferOfUnits(transferOfUnits);
        }

    }
}
