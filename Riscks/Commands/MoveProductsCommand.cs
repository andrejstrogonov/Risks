using Riscks.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riscks.Commands
{
    public class MoveProductsCommand : ICommand
    {

        private GroupViewModel groupViewModel;

        public MoveProductsCommand(GroupViewModel groupViewModel)
        {
            this.groupViewModel = groupViewModel;
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return groupViewModel.MoveIsEnable();
        }

        public void Execute(object parameter)
        {
            groupViewModel.Move();
        }
    }
}
