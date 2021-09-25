using Riscks.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riscks.Commands
{
    public class SaveFlexibilityCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private FlexibilityAnalysViewModel flexibilityAnalysViewModel;

        public SaveFlexibilityCommand(FlexibilityAnalysViewModel flexibilityAnalysViewModel)
        {
            this.flexibilityAnalysViewModel = flexibilityAnalysViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            flexibilityAnalysViewModel.SaveChanges();
        }
    }
}
