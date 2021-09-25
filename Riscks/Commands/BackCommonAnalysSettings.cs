using Riscks.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riscks.Commands
{
    public class BackCommonAnalysSettings : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private HierarhySelectViewModel Hsvm;

        public BackCommonAnalysSettings(HierarhySelectViewModel hsvm)
        {
            Hsvm = hsvm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Hsvm.Back();
        }
    }
}
