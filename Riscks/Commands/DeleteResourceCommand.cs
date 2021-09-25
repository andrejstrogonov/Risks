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
    public class DeleteResourceCommand:ICommand
    {
        private IndexResourcesViewModel IndexResourceViewModel;

        public DeleteResourceCommand(IndexResourcesViewModel irvm)
        {
            this.IndexResourceViewModel = irvm;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }



        public bool CanExecute(object parameter)
        {
            return !(IndexResourceViewModel.CurrentResource is null);
        }

        public void Execute(object parameter)
        {
            IndexResourceViewModel.DeleteResource();
        }
    }
}
