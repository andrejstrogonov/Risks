using Riscks.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riscks.Commands
{
    public class SelectMenuItemCommand : ICommand
    {
        private MenuItemViewModel menuItemViewModel;
        public SelectMenuItemCommand(MenuItemViewModel menuItemViewModel)
        {
            this.menuItemViewModel = menuItemViewModel;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            menuItemViewModel.IsSelected = true;
        }
    }
}
