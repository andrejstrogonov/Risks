using Riscks.ViewModel;
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
    public class AddResourceOfProductCommand : ICommand
    {
        private IndexResourceOfProductViewModel IndexResourceOfProductViewModel;

        public AddResourceOfProductCommand(IndexResourceOfProductViewModel ipvm)
        {
            this.IndexResourceOfProductViewModel = ipvm;
       }

        public event EventHandler CanExecuteChanged;   


        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            IndexResourceOfProductViewModel.AddResource();
        }
    }
}
