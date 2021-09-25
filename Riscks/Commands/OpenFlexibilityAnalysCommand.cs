using Riscks.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riscks.Commands
{
    public class OpenFlexibilityAnalysCommand : ICommand
    {
        private FlexibilityAnalysViewModel FAVM;

        public OpenFlexibilityAnalysCommand(FlexibilityAnalysViewModel ipvm)
        {
            this.FAVM = ipvm;
        }

        public event EventHandler CanExecuteChanged;


        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            FAVM.Execute();
        }
    }
}
