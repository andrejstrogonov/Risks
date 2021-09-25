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
    public class OpenSessionSelectionCommand:ICommand
    {
        private ProjectViewModel ProjectViewModel;

        public OpenSessionSelectionCommand(ProjectViewModel ipvm)
        {
            this.ProjectViewModel = ipvm;
            this.ProjectViewModel.PropertyChanged += new PropertyChangedEventHandler(product_PropertyChanged);
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
           // SessionSelectWindow commonAnalysWindow = new SessionSelectWindow();
            //commonAnalysWindow.InitModel(ProjectViewModel.Project);
            //commonAnalysWindow.Show();
        }
    }
}
