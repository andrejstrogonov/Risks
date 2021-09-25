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
    public class AddGroupCommand : ICommand
    {
        private IndexGroupViewModel indexGroupViewModel;

        public AddGroupCommand(IndexGroupViewModel igvm)
        {
            indexGroupViewModel = igvm;

            this.indexGroupViewModel.PropertyChanged += new PropertyChangedEventHandler(group_PropertyChanged);
        }

        public event EventHandler CanExecuteChanged;

        private void group_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
            indexGroupViewModel.AddGroup();
        }
    }
}
