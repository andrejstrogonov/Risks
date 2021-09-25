using Riscks.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riscks.Commands
{
    public class CreateSectionCommand:ICommand
    {
        private ProjectViewModel ProjectViewModel;

        public CreateSectionCommand(ProjectViewModel ipvm)
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
            return true;
        }

        public void Execute(object parameter)
        {
            ProjectViewModel.CreateSection();
        }
    }
}
