using Riscks.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riscks.Commands
{
    public class DeleteSessionCommand : ICommand
    {
        private ProjectViewModel projectViewModel;
        public DeleteSessionCommand(ProjectViewModel projectViewModel)
        {
            this.projectViewModel = projectViewModel;
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public bool CanExecute(object parameter)
        {
            return !(projectViewModel.CurrentNode is null);
        }

        public void Execute(object parameter)
        {
            projectViewModel.DeleteNode();
        }
    }
}
