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
    public class DeleteGroupCommand:ICommand
    {
        private IndexGroupViewModel indexGroupViewModel;

        public DeleteGroupCommand(IndexGroupViewModel igvm)
        {
            indexGroupViewModel = igvm;

        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

 

        public bool CanExecute(object parameter)
        {
            return !(indexGroupViewModel.CurrentGroup is null);
        }

        public void Execute(object parameter)
        {
            indexGroupViewModel.DeleteGroup();
        }
    }
}
