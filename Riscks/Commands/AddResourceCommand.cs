using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Riscks.ViewModel;

namespace Riscks.Commands
{
    public class AddResourceCommand : ICommand
    {
        private IndexResourcesViewModel IndexResourceViewModel;

        public AddResourceCommand(IndexResourcesViewModel irvm)
        {
            this.IndexResourceViewModel = irvm;
            this.IndexResourceViewModel.PropertyChanged += new PropertyChangedEventHandler(resource_PropertyChanged);
        }

        public event EventHandler CanExecuteChanged;

        private void resource_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
            IndexResourceViewModel.AddResource();
        }
    }
}
