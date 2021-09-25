using Riscks.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riscks.Commands
{
    public class BackAnalysSettingsCommand : ICommand
    {
        private AnalysSettingsViewModel analysSettingsViewModel;
        public event EventHandler CanExecuteChanged;

        public BackAnalysSettingsCommand(AnalysSettingsViewModel asvm)
        {
            analysSettingsViewModel = asvm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            analysSettingsViewModel.Back();
        }
    }
}
