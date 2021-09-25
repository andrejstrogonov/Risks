using Riscks.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riscks.Commands
{
    public class AddFixedCostCommand : ICommand
    {
        private IndexFixedCostViewModel indexFixedCostViewModel;

        public AddFixedCostCommand(IndexFixedCostViewModel indexFixedCostViewModel)
        {
            this.indexFixedCostViewModel = indexFixedCostViewModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            indexFixedCostViewModel.AddFixedCost();
        }
    }
}
