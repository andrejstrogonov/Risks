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
    public class OpenGraphicCommand : ICommand
    {
        private AnalysViewModel AnalysViewModel;

        public OpenGraphicCommand(AnalysViewModel ipvm)
        {
            this.AnalysViewModel = ipvm;
            this.AnalysViewModel.PropertyChanged += new PropertyChangedEventHandler(product_PropertyChanged);
        }

        public event EventHandler CanExecuteChanged;

        private void product_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }


        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            AnalysViewModel.Execute();
        }
    }
}
