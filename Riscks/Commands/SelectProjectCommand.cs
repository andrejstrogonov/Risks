using Riscks.ViewModel;
using System;
using System.Windows.Input;

namespace Riscks.Commands
{

    public class SelectProjectCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        ProjectInfo ProjectInfo;
        public SelectProjectCommand(ProjectInfo projectInfo)
        {
            ProjectInfo = projectInfo;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ProjectInfo.Select();
        }
    }
}
