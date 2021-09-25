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
     public class DeleteProductCommand:ICommand
    {
        private IndexProductViewModel IndexProductViewModel;

        public DeleteProductCommand(IndexProductViewModel ipvm)
        {
            this.IndexProductViewModel = ipvm;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }




        public bool CanExecute(object parameter)
        {
            return !(IndexProductViewModel.CurrentProduct is null);
        }

        public void Execute(object parameter)
        {
            IndexProductViewModel.DeleteProduct();
        }
    }
}
