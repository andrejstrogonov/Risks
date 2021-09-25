using Riscks.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riscks.Commands
{
    public class SaveAnalysSettingsCommand : ICommand
    {
        private AnalysSettingsViewModel analysSettingsViewModel;
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public SaveAnalysSettingsCommand(AnalysSettingsViewModel asvm)
        {
            analysSettingsViewModel = asvm;
        }

        public bool CanExecute(object parameter)
        {
            return !analysSettingsViewModel.OverRange;
        }

        public void Execute(object parameter)
        {
            analysSettingsViewModel.Save();
        }
    }
}
