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
    public class DeleteResourceOfProductCommand : ICommand
    {
        private IndexResourceOfProductViewModel IndexResourceOfProductViewModel;

        public DeleteResourceOfProductCommand(IndexResourceOfProductViewModel irpvm)
        {
            this.IndexResourceOfProductViewModel = irpvm;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return !(IndexResourceOfProductViewModel.CurrentResource is null);
        }

        public void Execute(object parameter)
        {
            IndexResourceOfProductViewModel.DeleteResource();
        }
    }
}
