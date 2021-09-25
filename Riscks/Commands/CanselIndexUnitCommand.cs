using Riscks.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riscks.Commands
{
    public class CanselIndexUnitCommand:ICommand
    {
        private IndexUnitViewModel indexUnitViewModel;
        public CanselIndexUnitCommand(IndexUnitViewModel indexUnitViewModel)
        {
            this.indexUnitViewModel = indexUnitViewModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            indexUnitViewModel.Cansel();
        }
    }
}
