using Riscks.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riscks.Commands
{
    public class SetFlexibilityParametersCommand : ICommand
    {

        private FlexibilityAnalysViewModel flexibilityViewModel;

        public SetFlexibilityParametersCommand(FlexibilityAnalysViewModel flexibilityAnalysViewModel)
        {
            this.flexibilityViewModel = flexibilityAnalysViewModel;
        }


        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        public bool CanExecute(object parameter)
        {
            if (flexibilityViewModel.ProfitFlexibilities is null)
                return false;
            return flexibilityViewModel.ProfitFlexibilities.Any(p => p.Posibility is true);
        }

        public void Execute(object parameter)
        {
            flexibilityViewModel.SetChanges();
        }
    }
}
