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
    public class SaveAsSessionCommand: ICommand
    {
        private ProjectViewModel ProjectViewModel;

        public SaveAsSessionCommand(ProjectViewModel ipvm)
        {
            this.ProjectViewModel = ipvm;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return (ProjectViewModel.CurrentNode is Node_Session);
        }

        public void Execute(object parameter)
        {
            if (!ProjectViewModel.CopySession())
            {
                
            }
        }
    }
}
