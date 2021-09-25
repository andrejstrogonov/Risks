using Riscks.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riscks.Commands
{
    public class DeleteRiskCommand : ICommand
    {
        private IndexRiskOfProductViewModel IndexRiskOfProductViewModel;

        public DeleteRiskCommand(IndexRiskOfProductViewModel irpvm)
        {
            this.IndexRiskOfProductViewModel = irpvm;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

  



        public bool CanExecute(object parameter)
        {
            return !(IndexRiskOfProductViewModel.CurrentRiskOfProduct is null);
        }

        public void Execute(object parameter)
        {
            IndexRiskOfProductViewModel.DeleteCurrentRisk();
        }
    }
}
