using Riscks.ViewModel;
using System;
using System.Windows.Input;

namespace Riscks.Commands
{
    public class DeleteProjectCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        ProjectInfo ProjectInfo;
        public DeleteProjectCommand(ProjectInfo projectInfo)
        {
            ProjectInfo = projectInfo;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ProjectInfo.DeleteProject();
        }
    }
}
