using Riscks.ViewModel;
using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riscks.Commands
{
    public class DeleteFixedCostCommand : ICommand
    {
        private IndexFixedCostViewModel indexFixedCostViewModel;
        private VATType vattype = VATType.full;
        public DeleteFixedCostCommand(IndexFixedCostViewModel indexFixedCostViewModel)
        {
            this.indexFixedCostViewModel = indexFixedCostViewModel;
            vattype = SettingsDAL.GetSettings().VAT_type;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return !(indexFixedCostViewModel.CurrentFixedCost is null) && !(indexFixedCostViewModel.CurrentFixedCost.TypeOfFixedCosts== SQLiteLibrary.Model.TypeOfFixedCosts.admortization && vattype== SQLiteLibrary.Model.VATType.full);
        }

        public void Execute(object parameter)
        {
            indexFixedCostViewModel.DeleteFixedCost();
        }
    }
}
