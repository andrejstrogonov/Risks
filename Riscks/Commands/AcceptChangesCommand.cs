using Riscks.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riscks.Commands
{
    public class AcceptChangesCommand : ICommand
    {
        private FlexibilityAnalysViewModel favm;

        public AcceptChangesCommand(FlexibilityAnalysViewModel FAVM)
        {
            favm = FAVM;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            favm.SetChanges();
        }
    }
}
