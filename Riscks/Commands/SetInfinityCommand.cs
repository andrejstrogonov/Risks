using Riscks.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riscks.Commands
{
    public class SetInfinityCommand : ICommand
    {
        private ResourceViewModel ResourceViewModel;
        public SetInfinityCommand(ResourceViewModel resourceViewModel)
        {
            ResourceViewModel = resourceViewModel;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {            
            ResourceViewModel.SetMaximumInfinity();
        }
    }
}
