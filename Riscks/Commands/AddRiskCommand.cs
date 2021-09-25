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
    public class AddRiskCommand : ICommand
    {
        private IndexRiskOfProductViewModel IndexRiskOfProductViewModel;

        public AddRiskCommand(IndexRiskOfProductViewModel irpvm)
        {
            this.IndexRiskOfProductViewModel = irpvm;
            this.IndexRiskOfProductViewModel.PropertyChanged += new PropertyChangedEventHandler(product_PropertyChanged);
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
            IndexRiskOfProductViewModel.AddRiskOfProduct();
        }
    }
}
